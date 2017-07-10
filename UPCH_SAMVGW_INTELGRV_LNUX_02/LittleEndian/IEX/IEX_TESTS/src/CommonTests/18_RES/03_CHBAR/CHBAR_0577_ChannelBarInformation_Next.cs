
/// <summary>
///  Script Name : CHBAR_0577_ChannelBarInformation_Next.cs
///  Test Name   : CHBAR_0577_ChannelBarInformation_Next
///  TEST ID     : 67303
///  JIRA ID     : FC-562
///  QC Version  : 1
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

[Test("CHBAR_0577")]
public class CHBAR_0577 : _Test
{
  
    [ThreadStatic]
    static _Platform CL;
   
    static Service videoService1;
    static Service videoService2;
     static string recordOnFuture;
    static EnumChannelBarTimeout maxChannelBarTimeOutVal;
    static string ChannelList ="";
    static string futureEventRecording = "FUTURE_EVENT"; //The future event to be recorded 
    static EnumChannelBarTimeout defaultChannelBarTimeOutVal;
  
    //Constants used in the test
    private static class Constants
    {
        public const int numOfPressesForNextEvent = 1;
        public const int minTimeBeforeEventStart = 5;
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get channel fron content xml and set SetBannerDisplayTime.");
        this.AddStep(new Step1(), "Step 1: Schedule recording on the Next event of service s1 and Launch the channel bar and move the focus on Next event and get the channel bar informations ");
        this.AddStep(new Step2(), "Step 2: Tune to service S2, Launch the channel bar and move the focus on next event and get the chbar informations");
       
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

            //Book the Next Event Recording on service s1
            res = CL.EA.PVR.BookFutureEventFromBanner(futureEventRecording, Constants.numOfPressesForNextEvent, Constants.minTimeBeforeEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event for recording!");
            }

            //Navigate to channel bar and move the focus on Next event & verify Title, Start time, End times, Recording indication, Thumnail and synopsis.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            //res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Select the next (channel bar) event
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //Get Event Name from channel bar on Next Event
            string obtainedValue = "";     
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name from channel bar");
            }
            LogComment(CL, "Obtained event name : " + obtainedValue);

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
            //Tune to the service whose future event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService2.LCN);
            }

            //Navigate to channel bar and move the focus on Next event & verify Title, Start time, End times, Recording indication, Thumnail and synopsis.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Select the next (channel bar) event
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //Check Next Event Name in Channel Bar
            string obtainedValue = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name from channel bar");
            }
            LogComment(CL, "Obtained event name : " + obtainedValue);

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