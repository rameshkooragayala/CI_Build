using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;
using System.Globalization;
using IEX.ElementaryActions.EPG;

namespace EAImplementation
{
    /// <summary>
    ///   Modify Manual Recording From Planner
    /// </summary>
    public class ModifyManualRecording : IEX.ElementaryActions.BaseCommand
    {

        private IEX.ElementaryActions.EPG.SF.UI EPG;

        private IEX.ElementaryActions.Functionality.Manager _manager;
        private string _EventKeyName;
        private string _StartTime;
        private string _EndTime;
        private string _ChannelName;
        private int _Days;
        private Boolean _IsFirstTime;
		private bool recordNameChangeSupported = true;

        private EnumFrequency _Frequency;

        /// <param name="EventKeyName">Key Of The Event</param>
        /// <param name="StartTime">Channel Name</param>
        /// <param name="EndTime">Channel Number If Entered Doing DCA</param>
        /// <param name="ChannelName">Optional Parameter Default = "" : The Channel Name</param>
        /// <param name="Days">Optional Parameter Default = 0 : Adds Days From Current Date</param>
        /// <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
        /// <param name="pManager">Manager</param>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>300 - NavigationFailure</para> 
        /// <para>301 - DictionaryFailure</para> 
        /// <para>302 - EmptyEpgInfoFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>305 - PCATFailure</para> 
        /// <para>309 - GetEpgTimeFailure</para>    
        /// <para>310 - GetEpgDateFailure</para>    
        /// <para>322 - VerificationFailure</para> 
        /// <para>328 - INIFailure</para>  
        /// <para>330 - TelnetFailure</para> 
        /// <para>331 - CopyFileFailure</para> 
        /// <para>332 - NoValidParameters</para> 
        /// <para>334 - VideoNotPresent</para>  
        /// <para>339 - RecordEventFailure</para>  
        /// <para>349 - ReturnToLiveFailure</para> 
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>
        public ModifyManualRecording(string EventKeyName, string StartTime, string EndTime, string ChannelName, int Days, EnumFrequency Frequency, Boolean IsFirstTime, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._EventKeyName = EventKeyName;
            this._StartTime = StartTime;
            this._EndTime = EndTime;
            this._ChannelName = ChannelName;
            this._Days = Days;
            this._Frequency = Frequency;
            this._IsFirstTime = IsFirstTime;
            this._manager = pManager;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {

            IEXGateway._IEXResult res;
            string EventName = "";
            EpgEvent eventToBeModified = new EpgEvent(EPG.Utils);
            String EPGDate = "";
            String EPGEventdate = "";
            String PCATEventDate = "";
            String EPGDateFormat = "";
            String PCATDateFormat = "";
            String milestoneDateFormat = "";
            bool Current = true;
            string EventSource = "ManualFuture";
            string Frequency = "";
            int Occurences = 0;
            long newDuration = 0;
            String tuneToService = "";
            string TodayDateInEPGFormat = "";

            tuneToService = EPG.Utils.GetValueFromProject("MANUAL_RECORDING", "TUNE_TO_SERVICE_BEFORE_RECORD");
            milestoneDateFormat = EPG.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT");
			try
            {
                string recordNameChangeSupportedVal = EPG.Utils.GetValueFromProject("MANUAL_RECORDING", "RECORD_NAME_CHANGE_SUPPORTED");
                if (recordNameChangeSupportedVal != null)
                {
                    recordNameChangeSupported = Convert.ToBoolean(recordNameChangeSupportedVal);
                }
            }
            catch
            {
                recordNameChangeSupported = true;
            }
            PCATDateFormat = EPG.Utils.GetDateFormatForEventDictionary();
            EPGDateFormat = EPG.Utils.GetEpgDateFormat();
            if (string.IsNullOrEmpty(this._EventKeyName))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"));
            }
            else
            {
                try
                {
                    eventToBeModified = EPG.Events[_EventKeyName];
                    EventName = eventToBeModified.Name;
                }
                catch
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + this._EventKeyName + " Does Not Exists On Collection"));
                }
            }

            EPGEventdate = eventToBeModified.ConvertedDate;
            Occurences = eventToBeModified.Occurrences;

            if (!string.IsNullOrEmpty(_StartTime) && _StartTime.Contains(":") == false)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Start Time Should Be Entered With Format : HH:MM"));
            }

            if (!string.IsNullOrEmpty(_EndTime) && _EndTime.Contains(":") == false)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "End Time Should Be Entered With Format : HH:MM"));
            }

            Current = false;

            switch (_Frequency)
            {
                case EnumFrequency.ONE_TIME:
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_ONE_TIME");
                    break;
                case EnumFrequency.DAILY:
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_DAILY");
                    break;
                case EnumFrequency.WEEKLY:
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKLY");
                    break;
                case EnumFrequency.WEEKDAY:
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKDAY");
                    break;
                case EnumFrequency.WEEKEND:
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKEND");
                    break;
                case EnumFrequency.NO_CHANGE:
                    Frequency = eventToBeModified.Frequency;
                    break;
                default:
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "No Valid Frequency Has Been Entered"));
                    break;
            }

            EPG.Live.GetEpgDate(ref EPGDate);
            TodayDateInEPGFormat = DateTime.ParseExact(EPGDate, milestoneDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);

            if (_Days > 0)
            {
                EPGEventdate = DateTime.ParseExact(EPGEventdate, EPGDateFormat, CultureInfo.InvariantCulture).AddDays(_Days).ToString(EPGDateFormat);
                PCATEventDate = DateTime.ParseExact(EPGEventdate, EPGDateFormat, CultureInfo.InvariantCulture).AddDays(_Days).ToString(PCATDateFormat);
            }

            // Calculate the days delay
            if (EPGEventdate != TodayDateInEPGFormat)
            {
                _Days = (DateTime.ParseExact(EPGEventdate, EPGDateFormat, CultureInfo.InvariantCulture) - DateTime.ParseExact(TodayDateInEPGFormat, EPGDateFormat, CultureInfo.InvariantCulture)).Days;
            }


            EPG.ManualRecording.GetOccurences(Frequency, _Days, ref Occurences);

            if (!string.IsNullOrEmpty(_StartTime) && !string.IsNullOrEmpty(_EndTime))
            {
                newDuration = EPG.Utils._DateTime.SubtractInSec(DateTime.Parse(_EndTime), DateTime.Parse(_StartTime));
            }

            // Tune to service if required
            int channelNum = -1;
            String channelName = "";
            if (!string.IsNullOrEmpty(_ChannelName) && _ChannelName != eventToBeModified.Channel)
            {
                if (tuneToService.ToUpper() == "TRUE")
                {
                    try
                    {
                        channelNum = int.Parse(_manager.GetServiceFromContentXML("Name=" + _ChannelName).LCN);
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.GetChannelFailure, ex.Message));
                    }
                    res = _manager.ChannelSurf(EnumSurfIn.Live, channelNum.ToString());
                    if (!res.CommandSucceeded)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.TuneToChannelFailure, res.FailureReason));
                    }

                }
                else
                {
                    channelName = _ChannelName;
                }

                // Event name should be changed to channel name
                EventName = _ChannelName;

            }
            else
            {
				// For Event based to Time based, fetch channel name from the channel number in event collection
                try
                {
					if (recordNameChangeSupported)
					{
						EventName = _manager.GetServiceFromContentXML("LCN=" + eventToBeModified.Channel).Name;
					}
                }
                catch
                {
                    EPG.Utils.LogCommentInfo("Event channel from collection:" + eventToBeModified.Channel);
                }

            }

            res = _manager.PVR.VerifyEventInPlanner(_EventKeyName);
            if (res.CommandSucceeded == false)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.FindEventFailure, res.FailureReason));
            }

            EPG.FutureRecordings.SelectEvent(eventToBeModified.Name);

            EPG.FutureRecordings.ModifyEvent();

            //Checking if there are no changes required in setting date and Frequency
            if (EPGEventdate == eventToBeModified.ConvertedDate)
            {
                EPGEventdate = "";
            }

            if (_IsFirstTime)
            {
                if (Frequency == eventToBeModified.Frequency)
                {
                    Frequency = "";
                }
            }

            EPG.ManualRecording.SetManualRecordingParams(EPGEventdate, _StartTime, channelName, channelNum, _EndTime, Frequency, true, _IsFirstTime);

            EPG.ManualRecording.SaveAndEnd(Current, true);

            try
            {
                EPG.ManualRecording.VerifySaveAndEndFinished(Current);
            }
            catch (EAException ex)
            {
                EPG.Events.Remove(_EventKeyName);
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, ex.Message));
            }

            EPG.Utils.InsertEventToCollection(this._EventKeyName, EventName, EventSource, _StartTime, _EndTime, _ChannelName, newDuration, 0, PCATEventDate, Frequency,
            Occurences, EPGEventdate, true);

            EPG.Utils.ReturnToLiveViewing();

        }
    }

}