using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EAImplementation
{
    /// <summary>
    ///  Verifies the AMS tags of different events
    /// </summary>
    public class VerifyAMSTags : IEX.ElementaryActions.BaseCommand
    {

        private Service _service;
        private EnumAMSEvent _AMSEvent;
        private Manager _manager;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private string _IsRBPlayback;
        private Double _Speed;
		private string _CommonVariable;
        private string _FinalOutput;
        private string boxID;
        private string AMSCommand;
        private string AMSlogFolderPath;
        private string CIResultsPath;
        /// <param name="AMSEvent">Contains the tags the different AMS Events like Stanby in , out, Playback etc</param>
        /// <param name="service">Servic object</param>
        /// <param name="IsRBPlayback">Whether we are in Recording playback:False or RB playback:True</param>
        /// <param name="Speed">For Example : 1 For Play, 0 For Pause,0.5,2,6,12,30</param>
        /// <param name="manager">the manager</param>
        /// <remarks>
        /// </remarks>

        public VerifyAMSTags(EnumAMSEvent AMSEvent, Service service, string IsRBPlayback, double Speed,string commonVariable, Manager manager)
        {
            _AMSEvent = AMSEvent;
            _service = service;
            _IsRBPlayback = IsRBPlayback;
            _Speed = Speed;
			_CommonVariable = commonVariable;
            _manager = manager;
            EPG = _manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {
            var combinedXML = new XDocument();
            boxID = Convert.ToInt64(EPG.Utils.GetValueFromEnvironment("BOX_ID")).ToString();
			string RFFeed = EPG.Utils.GetValueFromEnvironment("RFPort");
            if (RFFeed.ToUpper() == "NL")
            {
                //Command which is used to connect to the the AMS Server
                AMSCommand = EPG.Utils.GetValueFromEnvironment("AMSServerConnectionCmd_NL");
                //AMS SERVER log folder path--from where we will be fetching the AMS files
                AMSlogFolderPath = EPG.Utils.GetValueFromProject("AMS", "LOG_FOLDER_PATH_NL");
            }
            else
            {
                //Command which is used to connect to the the AMS Server
                AMSCommand = EPG.Utils.GetValueFromEnvironment("AMSServerConnectionCmd_UM");
                //AMS SERVER log folder path--from where we will be fetching the AMS files
                AMSlogFolderPath = EPG.Utils.GetValueFromProject("AMS", "LOG_FOLDER_PATH_UM");
            }
            //CI Results path where we will be copying the AMS files
            CIResultsPath = EPG.Utils.GetValueFromEnvironment("LogDirectory");
			
            _FinalOutput = EPG.OTA.StartProcess(AMSCommand, "ERROR");
            DirectoryInfo directory = new DirectoryInfo(AMSlogFolderPath);
            var latestFile = directory.GetFiles("*" + boxID + "*Format1*").Where(f => f.CreationTime >= DateTime.Now.AddMinutes(-10));
            if (latestFile.Count() <= 0)
            {                
                EPG.Utils.LogCommentWarning("There are no files Generated in the AMS in the last 10 minutes");
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.GetEventInfoFailure, "Did not find any AMS files in the server"));
            }
            bool firstLoop = true;
            foreach (FileInfo file in latestFile)
            {
                //Copying the files to the result folder for the Verification
                try
                {
                    File.Copy(AMSlogFolderPath + file.Name, CIResultsPath +"\\"+ file.Name);
                    EPG.Utils.LogCommentInfo("Successfully copied the "+file.Name+" to "+CIResultsPath);
                }
                catch
                {
                    EPG.Utils.LogCommentWarning("Failed to copy the AMS files from the server to the result folder");
                }
                //Combining all the AMS log files into single AMS fils which will make it easy to verify the tags
                if (firstLoop)
                {
                    firstLoop = false;
                    combinedXML = XDocument.Load(@AMSlogFolderPath + file.Name);
                }
                else
                {
                    var xmlFile = XDocument.Load(@AMSlogFolderPath + file.Name);
                    combinedXML.Descendants("Event").LastOrDefault().AddAfterSelf(xmlFile.Descendants("Event"));
                }

            }
            //converting the xdocument to an xmldocument
            XmlDocument xdoc = new XmlDocument();
            using (var xmlReader = combinedXML.CreateReader())
            {
                xdoc.Load(xmlReader);
            }
            //depending upon the AMS event which needs to be verified we will be verifying the tags
            switch (_AMSEvent)
            {
                case EnumAMSEvent.SurfingEvent:

                    XmlNodeList Surfingnodes = xdoc.DocumentElement.SelectNodes("//" + _AMSEvent + "");
                    if (Surfingnodes.Count == 1)
                    {
                        EPG.Utils.LogCommentImportant("Found the Surfing Event");
                    }
                    else if (Surfingnodes.Count > 1)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Found more then one instance of the Surfing Event"));
                    }
                    else
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Surfing Event"));
                    }

                    break;
                case EnumAMSEvent.StandByIn:
                case EnumAMSEvent.StandByOut:
                case EnumAMSEvent.PowerOn:
                    try
                    {
                        string standbyEvent = xdoc.SelectSingleNode("AMSSubscriberLog/Event/STBEvent[@State='"+_AMSEvent+"']").OuterXml;
                        if (standbyEvent != "")
                        {
                            EPG.Utils.LogCommentImportant("Found the AMS Event" + _AMSEvent);
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the " + _AMSEvent + " Event"));
                        }

                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the " + _AMSEvent + " Event"));
                    }
                    break;
                case EnumAMSEvent.SignalFailureEvent:
                    try
                    {
					    string expectedtransponderID = EPG.Utils.GetValueFromTestIni("TEST PARAMS", "TRANSPONDERID");
                        string expectednetworkId = EPG.Utils.GetValueFromTestIni("TEST PARAMS", "NETWORKID");
                        string TransponderID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/SignalFailureEvent/ChannelID/Event/DVBTriple[@SIServiceId='" + _service.ServiceId + "']/@TransponderId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Siganl Failure Event");
                        if (TransponderID != expectedtransponderID)
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Transponer ID" + TransponderID + " is same as expected " + expectedtransponderID));
                        }
                        EPG.Utils.LogCommentImportant("Verified the Transponder ID "+TransponderID+" is same as expected"+expectedtransponderID);
                        string networkID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/SignalFailureEvent/ChannelID/Event/DVBTriple[@SIServiceId='" + _service.ServiceId + "']/@OriginalNetworkId").Value.ToString();
                        if (networkID != expectednetworkId)
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Network ID" + networkID + " is same as expected " + expectednetworkId));
                        }
                        EPG.Utils.LogCommentImportant("Verified the Network ID " + networkID + " is same as expected" + expectednetworkId);
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Signal Failure Event"));
                    }
                    break;
                case EnumAMSEvent.LiveViewEvent:
                    try
                    {
                        string TransponderID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/LiveViewEvent/ContentID/Event/DVBTriple[@SIServiceId='" + _service.ServiceId+ "']/@TransponderId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Live Viewing Event Event");
						string eventTime = xdoc.SelectSingleNode("AMSSubscriberLog/Event/@EventTime").Value;
                        
                        DateTime dt;
                        var formats = new[] { "yyyy-MM-ddTHH:mm:ss" };
                        if (DateTime.TryParseExact(eventTime, formats,
                                               CultureInfo.InvariantCulture, 
                                               DateTimeStyles.RoundtripKind,
                                               out dt))
                        {
                            EPG.Utils.LogCommentImportant("Verified the Date Format of the Live view event "+eventTime+" is in the Expected Format");
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Event time Date format of the Live viewing Event"));
                        }
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Live viewing Event"));
                    }
                    break;
                case EnumAMSEvent.StreamEvent:
                    try
                    {
                        string liveViewEventStreamID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/LiveViewEvent/Stream[@streamType='Audio']/@streamId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Live Viewing Event Event with the Stream ID " + liveViewEventStreamID);
                        string streamEventStreamID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/StreamEvent/Stream[@streamType='Audio']/@streamId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the Stream Event with the Stream ID " + streamEventStreamID);
                        if (streamEventStreamID == liveViewEventStreamID)
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Stream id of the Stream View Event"));
                        }
                        else
                        {
                            EPG.Utils.LogCommentImportant("AMS Live Viewing Event Stream ID " + liveViewEventStreamID + " is different from stream event Stream ID " + streamEventStreamID);
                        }
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Stream view Event"));
                    }
                    break;
                case EnumAMSEvent.DeletionEvent:
                    try
                    {
                        string TransponderID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/DeletionEvent/DeletedProgram/ContentID/Event/DVBTriple[@SIServiceId='" + _service.ServiceId + "']/@TransponderId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Verified the Service ID of the AMS Deletion Event");
                        string channelName = xdoc.SelectSingleNode("AMSSubscriberLog/Event/DeletionEvent/DeletedProgram/ContentID/Event/ChannelName").InnerText.ToString();
                        if (channelName == _service.Name)
                        {
                            EPG.Utils.LogCommentImportant("Verified the Service name"+channelName+" of the AMS Deletion Event is same as expected "+_service.Name);
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Service name"+channelName+" of the AMS Deletion Event is same as expected "+_service.Name));
                        }
                        string reasonForDeletion = xdoc.SelectSingleNode("AMSSubscriberLog/Event/DeletionEvent/DeletedProgram/@ReasonOfDeletion").Value;
                        if (reasonForDeletion != "8")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Reason for Deletion for the Deletion Event"));
                        }
                        EPG.Utils.LogCommentImportant("Verified the Reason for deletion is same as expected which is 8");

                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Deletion Event"));
                    }

                    break;
                case EnumAMSEvent.EndOfPlaybackFile:
                    try
                    {
                        string endOfPlaybackEvent = "";
                        if (_IsRBPlayback.ToUpper() == "TRUE")
                        {
                            endOfPlaybackEvent = xdoc.SelectSingleNode("AMSSubscriberLog/Event/EndOfPlaybackFile/PlaybackProgram[@eventSource='1']").OuterXml;
                            EPG.Utils.LogCommentImportant("Found the AMS End of Playback Event");
                        }
                        else
                        {
                            endOfPlaybackEvent = xdoc.SelectSingleNode("AMSSubscriberLog/Event/EndOfPlaybackFile/PlaybackProgram[@eventSource='0']").OuterXml;
                            EPG.Utils.LogCommentImportant("Found the AMS End of Playback Event");
                        }
                       
                        string serviceIDstring = @"SIServiceId=""" + _service.ServiceId + "";
                        if (endOfPlaybackEvent.Contains(serviceIDstring))
                        {
                            EPG.Utils.LogCommentImportant("Verified that the Service ID"+_service.ServiceId+" is same as expected");
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Service ID "+_service.ServiceId+" is different from Expected"));
                        }

                        //Fetch the Booking type and verify that it is not null
                        string bookingType = xdoc.SelectSingleNode("AMSSubscriberLog/Event/EndOfPlaybackFile/PlaybackProgram/@BookingType").Value;
                        if (bookingType == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Booking type fetched from AMS file is null"));
                        }
                        EPG.Utils.LogCommentImportant("Booking type fetched from the AMS file is " + bookingType);
                       
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the End of Playback Event"));
                    }
                    break;
                case EnumAMSEvent.BookingforTime:
                    try
                    {
                        string TransponderID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookingforTime/ContentID/DVBTriple[@SIServiceId='" + _service.ServiceId + "']/@TransponderId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Booking for time Event");
                        string recordingStartTime = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookingforTime/RecordingStartTime").InnerText.ToString();
                        if (recordingStartTime == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Recording Start time of Booking for time AMS event"));
                        }
                        EPG.Utils.LogCommentImportant("Found the Recording Start time for Booking for time Event");
                        string recordingEndTime = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookingforTime/RecordingEndTime").InnerText.ToString();
                        if (recordingEndTime == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Recording End time of Booking for time AMS event"));
                        }
                        EPG.Utils.LogCommentImportant("Found the Recording End time for Booking for time Event");
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Live viewing Event"));
                    }
                    break;
			    case EnumAMSEvent.BookingforRecording:
                    try
                    {
                        string TransponderID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookingforRecording/RecordedProgram/ContentID/Event/DVBTriple[@SIServiceId='" + _service.ServiceId + "']/@TransponderId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Booking for Recording Event");
                        //Fetch the Booking type and verify that it is not null
                        string bookingType = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookingforRecording/RecordedProgram/@BookingType").Value;
                        if (bookingType == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Booking type fetched from AMS file is null"));
                        }
                        EPG.Utils.LogCommentImportant("Booking type fetched from the AMS file is " + bookingType);
                      
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify Booking for recording Event"));
                    }
                    break;
                case EnumAMSEvent.RecordingEvent:
                    try
                    {
                        string TransponderID = xdoc.SelectSingleNode("AMSSubscriberLog/Event/RecordingEvent/RecordedProgram/ContentID/Event/DVBTriple[@SIServiceId='" + _service.ServiceId + "']/@TransponderId").Value.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Recording Event");
                        string channelName = xdoc.SelectSingleNode("AMSSubscriberLog/Event/RecordingEvent/RecordedProgram/ContentID/Event/ChannelName").InnerText.ToString();
                        if (channelName == _service.Name)
                        {
                            EPG.Utils.LogCommentImportant("Verified the Service name of the AMS Recording Event");
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the service name of the Recording Event"));
                        }
                        //Fetch the Booking type and verify that it is not null
                        string bookingType = xdoc.SelectSingleNode("AMSSubscriberLog/Event/RecordingEvent/RecordedProgram/@BookingType").Value;
                        if (bookingType == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Booking type fetched from AMS file is null"));
                        }
                        EPG.Utils.LogCommentImportant("Booking type fetched from the AMS file is "+bookingType);
                        //Fetch the Reason of stop of recording and verify that it is not null
                        string reasonOfStopofRecording = xdoc.SelectSingleNode("AMSSubscriberLog/Event/RecordingEvent/RecordedProgram/@ReasonOfStopofRecording").Value;
                        if (reasonOfStopofRecording == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Reason Of Stop of Recording fetched from AMS file is null"));
                        }
                        EPG.Utils.LogCommentImportant("Reason Of Stop of Recording fetched from the AMS file is not null " + reasonOfStopofRecording);
                        //Verify the start time and End time of the Recording Event
                        string StartTime = xdoc.SelectSingleNode("AMSSubscriberLog/Event/RecordingEvent/StartTime").InnerText.ToString();
                        if (StartTime == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Recording Start time of Booking for time AMS event"));
                        }
                        EPG.Utils.LogCommentImportant("Found the Start time for Recording Event");
                        string EndTime = xdoc.SelectSingleNode("AMSSubscriberLog/Event/RecordingEvent/EndTime").InnerText.ToString();
                        if (EndTime == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Recording End time of Booking for time AMS event"));
                        }
                        EPG.Utils.LogCommentImportant("Found the End time for Recording Event");
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Recording Event"));
                    }
                    break;
                case EnumAMSEvent.PlaybackEvent:
                    try
                    {
                        string eventSource = xdoc.SelectSingleNode("AMSSubscriberLog/Event/PlaybackEvent/PlaybackProgram[@speed='" + _Speed + "']/@eventSource").Value.ToString();
                        if (_IsRBPlayback.ToUpper() == "TRUE")
                        {
                            if (eventSource == "1")
                            {
                                EPG.Utils.LogCommentImportant("Verified the Speed and Event source");
                            }
                            else
                            {
                                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the event source is same as expected"));
                            }
                        }
                        else
                        {
                            if (eventSource == "0")
                            {
                                EPG.Utils.LogCommentImportant("Verified the Speed and Event source");
                            }
                            else
                            {
                                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the event source is same as expected"));
                            }
                        }
                        string playbackEvent = xdoc.SelectSingleNode("AMSSubscriberLog/Event/PlaybackEvent/PlaybackProgram[@speed='" + _Speed + "']").OuterXml.ToString();
                        EPG.Utils.LogCommentImportant("Found the AMS Playback Event");
                        string serviceIDstring = @"SIServiceId=""" + _service.ServiceId + "";
                        if (playbackEvent.Contains(serviceIDstring))
                        {
                            EPG.Utils.LogCommentImportant("Verified that the Service ID is same as expected");
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Service ID is different from Expected"));
                        }
                        //Fetch the Booking type and verify that it is not null
                        string bookingType = xdoc.SelectSingleNode("AMSSubscriberLog/Event/PlaybackEvent/PlaybackProgram/@BookingType").Value;
                        if (bookingType == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Booking type fetched from AMS file is null"));
                        }
                        EPG.Utils.LogCommentImportant("Booking type fetched from the AMS file is " + bookingType);
						string recordedTime = xdoc.SelectSingleNode("AMSSubscriberLog/Event/PlaybackEvent/PlaybackProgram/@recordedTime").Value;
                        if (recordedTime == "")
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Recorded Time fetched from AMS file is null"));
                        }
                        EPG.Utils.LogCommentImportant("Recorded Time fetched from the AMS file is " + recordedTime);
                      
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the Playback event with speed " + _Speed));
                    }

                    break;
				case EnumAMSEvent.ViewedReport:
                    try
                    {
                        string viewedReport = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ViewedReport").OuterXml;
                        EPG.Utils.LogCommentImportant("Found the Viewed report Event");
                        XmlNodeList liveViewingEventnodes = xdoc.DocumentElement.SelectNodes("//LiveViewEvent");
                        if (liveViewingEventnodes.Count > 0)
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Found Livew view event while verifying the Viewed report"));
                        }
                        EPG.Utils.LogCommentImportant("Did not receive any LiveViewEvent nodes");
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the Stream view Event"));
                    }
                    break;
                case EnumAMSEvent.ApplicationEvent:
                    XmlNodeList ApplicationEventNodes = xdoc.DocumentElement.SelectNodes("//" + _AMSEvent + "");
                    foreach (XmlNode nodes in ApplicationEventNodes)
                    {
 
                    }
                    break;
			    case EnumAMSEvent.CallbackFailure:
                    try
                    {
                        string NumberofFailures = xdoc.SelectSingleNode("AMSSubscriberLog/Event/CallbackFailure/@NumberofFailures").Value.ToString();
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the Call back Failure Event"));
                    }
                    break;
                case EnumAMSEvent.GuideByGenre:
                  
                    string []genreids=(EPG.Utils.GetValueFromTestIni("TEST PARAMS","GENREIDs")).Split(',');
                    string failedIDs = "";

                    if(genreids.Count()==0)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to fetch Genre IDs from Test INI"));
                    }
                    
                    foreach (string genreID in genreids)
                    {
                        try
                        {
                          //  string value = xdoc.GetElementsByTagName("x").Cast<XmlNode>().Where(a => a.InnerXml == "UI_SCREEN_ENTRY BY_GENRE_GUIDE  " + genreID + " FROM MAIN_MENU").ToString();
                            string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY BY_GENRE_GUIDE  " + genreID + " FROM MAIN_MENU']").OuterXml;

                            EPG.Utils.LogCommentImportant("Verified AMS Tage for Genre ID: " + genreID);
                          
                        }
                        catch
                        {
                            EPG.Utils.LogCommentWarning("Failed to verify Genre ID :" + genreID);
                            failedIDs=failedIDs+"  "+ genreID;
                        }

                    }

                    if (failedIDs == "")
                    {
                        EPG.Utils.LogCommentImportant("Verified AMS Tag for all Genre IDs");
                    }
                    else
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to fetch Genre IDs " + failedIDs + " from Test INI"));
                    }

                    break;
                case EnumAMSEvent.GuideBySingleChannel:
                    try
                        {
                          //  string value = xdoc.GetElementsByTagName("x").Cast<XmlNode>().Where(a => a.InnerXml == "UI_SCREEN_ENTRY BY_GENRE_GUIDE  " + genreID + " FROM MAIN_MENU").ToString();
                            string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY BY_CHANNELS_GUIDE FROM MAIN_MENU']").OuterXml;

                            EPG.Utils.LogCommentImportant("Verified AMS Tage for Single Channel");
                          
                        }
                        catch
                        {

                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify AMS Tag for Single Channel"));
                        }

                    break;
				case EnumAMSEvent.ACTION_MENU:
                    try
                    {
                        string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY "+_AMSEvent+" "+_CommonVariable+"']").OuterXml;
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the Action Menu Event"));
                    }
                    break;
                case EnumAMSEvent.CHANNEL_BAR:
                case EnumAMSEvent.MORE_LIKE_THIS:
                    if (_AMSEvent == EnumAMSEvent.MORE_LIKE_THIS)
                    {
                        _AMSEvent = EnumAMSEvent.ACTION_MENU;
                    }
                    try
                    {
                        string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY MORE_LIKE_THIS FROM "+_AMSEvent+" "+_CommonVariable+"']").OuterXml;
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the "+_AMSEvent+" Event"));
                    }
                    break;
                case EnumAMSEvent.SUGGESTED:
                case EnumAMSEvent.FEATURED:
                    try
                    {
                        string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY "+_AMSEvent+" FROM MAIN_MENU']").OuterXml;
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the "+_AMSEvent+" Event"));
                    }
                    break;
				case EnumAMSEvent.GUIDE:
                    try
                    {
                        string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY ALL_CHANNELS_GUIDE FROM MAIN_MENU']").OuterXml;
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Fail to verify UI_SCREEN_ENTRY ALL_CHANNELS_GUIDE FROM MAIN_MENU"));
                    }
                    
                       break;

                case EnumAMSEvent.GUIDEATL:
                       
                    string[] adjustTimeArr = (EPG.Utils.GetValueFromTestIni("TEST PARAMS", "ADJUSTTIMELINE_ARRAY")).Split(',');

                    string failedID = "";

                    if (adjustTimeArr.Count() == 0)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to fetch Genre IDs from Test INI"));
                    }

                    foreach (string adjustTime in adjustTimeArr)
                    {
                        try
                        {
                            string acstiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY ALL_CHANNELS_GUIDE FROM TIMELINE_CHANGE " + adjustTime + " MINUTES']").OuterXml;
                            EPG.Utils.LogCommentImportant("Verified AMS Tage for ATL: " + adjustTime);      
                        }
                        catch
                        {
                            EPG.Utils.LogCommentWarning("Failed to verify forATL :" + adjustTime);
                            failedID = failedID + "  " + adjustTime;
                        }

                    }

                    if (failedID == "")
                    {
                        EPG.Utils.LogCommentImportant("Verified AMS Tag for all ATL");
                    }
                    else
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to fetch Adujust Time Line " + adjustTimeArr + " from Test INI"));
                    }
                       break;

                case EnumAMSEvent.UNSUBSCRIBED:
                       try
                       {
                           string actiomMenu = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY MORE_LIKE_THIS FROM ACTION_MENU " + _IsRBPlayback + "']").OuterXml;
                       }
                       catch
                       {
                           ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Fail to verify AMS for Unsubscribed"));
                       }

                       break;
                case EnumAMSEvent.SEARCH_MAIN_MENU:
                case EnumAMSEvent.SEARCH_LIBRARY:
                    string[] searchList = _AMSEvent.ToString().Split('_');
                    if (searchList.Count() == 3)
                    {
                        searchList[1] = searchList[1] + "_" + searchList[2];
                    }
                    try
                    {
                        string search = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SCREEN_ENTRY "+searchList[0]+" FROM "+searchList[1]+"']").OuterXml;
                        EPG.Utils.LogCommentImportant("Found the Search event");
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the "+_AMSEvent+" Event"));
                    }
                    break;
                case EnumAMSEvent.SEARCH_TRIGGERED:
                    try
                    {
                        string search = xdoc.SelectSingleNode("AMSSubscriberLog/Event/ComponentPrivateData[Data='UI_SEARCH_TRIGGERED " +_CommonVariable+"']").OuterXml;
                        EPG.Utils.LogCommentImportant("Found the Search triggered event");
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the " + _AMSEvent + " Event"));
                    }
                    break;
                case EnumAMSEvent.REFINE_SEARCH:
                    try
                    {
                        string search = xdoc.SelectSingleNode("AMSSubscriberLog").OuterXml;
                        int count = Regex.Matches(search, "UI_SCREEN_ENTRY REFINE_SEARCH "+_CommonVariable).Count;
                        if (count != 2)
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the number of occurences of the Refine Event"));
                        }

                        EPG.Utils.LogCommentImportant("Found the Search Refine Events tags :"+count);
                    }
                    catch
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to find the " + _AMSEvent + " Event"));
                    }
                    break;
                case EnumAMSEvent.REMINDER:
                    try
                    {
                        string contSourId = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookmarkReport/@contentSourceId").Value;

                        string subId = xdoc.SelectSingleNode("AMSSubscriberLog/Event/BookmarkReport/@subscriberId").Value;

                        if (string.IsNullOrEmpty(contSourId) && string.IsNullOrEmpty(subId))
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Did not found CRID and IMI for " + _CommonVariable));
                        }
                        else
                        {
                            EPG.Utils.LogCommentImportant("Found CRID and IMI tags for " + _CommonVariable);
                        }
                    }
                    catch
                    {
                        if (_CommonVariable.Equals("Reminder for without IMI"))
                        {
                            EPG.Utils.LogCommentImportant("Did not found CRID and IMI tags for " + _CommonVariable);
                        }
                        else
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to verify the CRID and IMI for " + _CommonVariable));
                        }
                    }
                    break;

                default:
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "No Valid AMS Event Has Been Entered"));
                    break;
            }

        }
    }
}



