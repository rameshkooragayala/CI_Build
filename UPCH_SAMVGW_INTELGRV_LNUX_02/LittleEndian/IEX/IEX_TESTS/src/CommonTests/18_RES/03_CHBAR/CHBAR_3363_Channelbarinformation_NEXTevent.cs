
/// <summary>
///  Script Name : CHBAR_3363_Channelbarinformation_NEXTevent.cs
///  Test Name   : EPG-3363-Channel bar information for NEXT event-FT490,186
///  TEST ID     : 
///  JIRA ID     : FC-562
///  QC Version  : 1.  Step 5 not covered because of different langauges. 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar C
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("CHBAR_3363")]
public class CHBAR_3363 : _Test
{
  
    [ThreadStatic]
    static _Platform CL;
   
    static Service videoService1;
    static Service videoService2;
    static string recordOnFuture;
    static EnumChannelBarTimeout maxChannelBarTimeOutVal;
    static string ChannelList ="";
    static EnumChannelBarTimeout defaultChannelBarTimeOutVal;
    static string futureEventRecording_1 = "FUTURE_EVENT1"; //The future event to be recorded 
    static string futureEventRecording_2 = "FUTURE_EVENT2"; //The future event to be recorded 

    //Constants used in the test   
    private static class Constants
    {
        public const int numOfPressesForNextEvent = 1;
        public const int minTimeBeforeEventStart = 5;
        public const int minTimeBeforeEvtEnds = 3;
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get channel fron content xml and set SetBannerDisplayTime.");
        this.AddStep(new Step1(), "Step 1:Tune to service s1 and book Reminder on next event,Launch the channel bar and get the channel bar informations ");
        this.AddStep(new Step2(), "Step 2:Tune to service S2 and record next event ,Launch the channel bar and get the chbar informations");
       
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get List of Channel from TEST ini File
            ChannelList = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channelNumberList");
            if (string.IsNullOrEmpty(ChannelList))
            {
                FailStep(CL, res, "Unable to fetch the ChannelList from test ini file");
            }

            string[] ChannelListNumber = ChannelList.Split(',');

            videoService1 = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[0].Trim(), "ParentalRating=High");
            if (videoService1 == null)
            {
                FailStep(CL, "Failed to fetch videoService1 from Content.xml");
            }
            LogCommentInfo(CL, "Video Service1 fetched from content.xml is: " + videoService1.LCN);

            videoService2 = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[1].Trim(), "ParentalRating=High");
            if (videoService2 == null)
            {
                FailStep(CL, "Failed to fetch videoService2 from Content.xml");
            }
        
            string bannerTimeout = "";

            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");
            if (string.IsNullOrEmpty(bannerTimeout))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, MAX fetched from Project.ini is null or empty", false);
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out maxChannelBarTimeOutVal);
            LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> MAX = " + maxChannelBarTimeOutVal);


            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
            if (string.IsNullOrEmpty(bannerTimeout))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, DEFAULT fetched from Project.ini is null or empty", false);
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out defaultChannelBarTimeOutVal);
            LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> DEFAULT = " + defaultChannelBarTimeOutVal);

            recordOnFuture = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "LOG_RECORD_FUTURE");

            //Change Timeout Duration in Channel Bar Timeout settings
            res = CL.EA.STBSettings.SetBannerDisplayTime(maxChannelBarTimeOutVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change Banner Display Time to:" + maxChannelBarTimeOutVal, false);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {

        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Tune to the service whose future event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService1.LCN);
            }

            //Book Reminder on Future Event on S1 
            res = CL.EA.BookReminderFromBanner(futureEventRecording_1, Constants.minTimeBeforeEventStart, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Reminder from action bar on future event of service 1");
            }

            //Navigate to channel bar and get Title, Start time, End times, Recording indication, Thumnail and synopsis on Next event.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            //Select the next (channel bar) event
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //Get complete Event Name from channel bar on Next Event
            string obtainedValue = "";               
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the complete event details from channel bar");
            }
            if (obtainedValue.Contains("-"))
            {
                string[] CompleteEventName = obtainedValue.Split('-');
                string seriesNameAndNumber_EpisodeNumber = CompleteEventName[0];
                string episodeName = CompleteEventName[1];
                LogComment(CL, "Obtained seriesName seriesNumber and EpisodeNumber  : " + seriesNameAndNumber_EpisodeNumber);
                LogComment(CL, "Obtained episodeName  : " + episodeName);
            }
            else
            {
                LogComment(CL, "Obtained complete event details from channel bar  : " + obtainedValue);
            }

            //Verify Start time and end time
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time from channel bar");
            }

            if (string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Event time is null or empty: ");
            }
            LogCommentInfo(CL, "Obtained event time is: " + obtainedValue);

            //Check Next Event Thumbnail in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained thumbnail Name is: " + obtainedValue);

            //Check Next Event synopsis in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained synopsis Value is: " + obtainedValue);

             //Check Next Event Reminder status in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("Reminder/Recording Status", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained Reminder/Recording Status Value is: " + obtainedValue);

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {

        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }
            //Tune to the another service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService2.LCN);
            }


            res = CL.EA.PVR.BookFutureEventFromBanner(futureEventRecording_2, Constants.numOfPressesForNextEvent, Constants.minTimeBeforeEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event for recording!");
            }

            //Navigate to channel bar and get Title, Start time, End times, Recording indication, Thumnail and synopsis on next event.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            //Select the next (channel bar) event
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //Get complete Event Name from channel bar on Next Event
            string obtainedValue = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the complete event details from channel bar");
            }
            if (obtainedValue.Contains("-"))
            {
                string[] CompleteEventName = obtainedValue.Split('-');
                string seriesNameAndNumber_EpisodeNumber = CompleteEventName[0];
                string episodeName = CompleteEventName[1];
                LogComment(CL, "Obtained seriesName seriesNumber and EpisodeNumber  : " + seriesNameAndNumber_EpisodeNumber);
                LogComment(CL, "Obtained episodeName  : " + episodeName);
            }
            else
            {
                LogComment(CL, "Obtained complete event details from channel bar  : " + obtainedValue);
            }

            //Verify Start time and end time
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time from channel bar");
            }

            if (string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Event time is null or empty: ");
            }
            LogCommentInfo(CL, "Obtained event time is: " + obtainedValue);

            //Verify Recording indication
            res = CL.IEX.MilestonesEPG.GetEPGInfo("RecordingStatus", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Recording indication from channel bar");
            }
            if (!recordOnFuture.Equals(obtainedValue))
            {
                FailStep(CL, "Recording indication is: " + obtainedValue);
            }

            LogCommentInfo(CL, "Obtained Recording indication is: " + obtainedValue);

            //Check Next Event Thumbnail in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained thumbnail Name is: " + obtainedValue);

            //Check Next Event synopsis in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained synopsis Value is: " + obtainedValue);

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Set Channel Bar Time Out to Default 
        res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to change Banner Display Time to:" + defaultChannelBarTimeOutVal);
        }
    }
    #endregion
}