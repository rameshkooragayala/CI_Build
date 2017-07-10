/// <summary>
///  Script Name : CHBAR_0571_BrowseWhileLiveViewing
///  Test Name   : EPG-0571-Channel Bar-Browse While Live Viewing
///  TEST ID     : 1
///  JIRA TASK   : FC-261
///  QC Version  : 62871
///  Variations from QC: Testing of Recommendation,Store and widgets is not included in script.
/// -----------------------------------------------
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("CHBAR_0571")]
public class CHBAR_0571 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoService;
    private static string focusOnNowVal;
    private static string focusOnNextVal;
    private static double defaultChannelBarTimeOut;

    public class Constants
    {
        public const int noOfPresses = 2;
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Surf to channel S1 , Launch channel bar and channel information";
    private const string STEP2_DESCRIPTION = "Step 2: Get channel information from channel bar";
    private const string STEP3_DESCRIPTION = "Step 3: Get event information from channel bar and browse to next event";
    private const string STEP4_DESCRIPTION = "Step 4: Navigate to NEXT from channel bar and access event information";
    private const string STEP5_DESCRIPTION = "Step 5: Access action items related to next event from channel bar";

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            focusOnNowVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "LOG_FOCUS_ON_NOW");
            focusOnNextVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "LOG_FOCUS_ON_NEXT");
            string channelBarTimeOutVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");

            if (videoService == null || string.IsNullOrEmpty(focusOnNowVal) || string.IsNullOrEmpty(focusOnNextVal) || string.IsNullOrEmpty(channelBarTimeOutVal))
            {
                FailStep(CL, "One of the obtained values is null: \n videoService: " + videoService + "\n LOG_FOCUS_ON_NOW: " + focusOnNowVal + "\n LOG_FOCUS_ON_NEXT: " + focusOnNextVal + "\n LOG_FOCUS_ON_NEXT" + channelBarTimeOutVal);
            }
            else
            {
                LogCommentInfo(CL, "Retrieved Value From XML File: videoService = " + videoService.LCN);
                LogCommentInfo(CL, "Retrieved Value From ini File: LOG_FOCUS_ON_NOW = " + focusOnNowVal);
                LogCommentInfo(CL, "Retrieved Value From ini File: LOG_FOCUS_ON_NEXT = " + focusOnNextVal);
                LogCommentInfo(CL, "Retrieved Value From ini File: DEFAULT = " + channelBarTimeOutVal);
            }

            defaultChannelBarTimeOut = Convert.ToDouble(channelBarTimeOutVal);
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Channel Surf to FTA_Channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA: " + videoService.LCN);
            }

            //Launch channel bar and browse channel list
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, Constants.noOfPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed launch channel bar and browse channel list");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            LogCommentInfo(CL, "Wait for " + defaultChannelBarTimeOut + "sec till channel bar is disposed");

            //Wait till the channel bar is dimissed
            res = CL.IEX.Wait(defaultChannelBarTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for untill channel bar is dismissed");
            }

            //Navigate to channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch channel bar");
            }

            //Verify channel information
            string obtainedVal = "";

            //Verify if focus is there on NOW
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get focused value  From Channel Bar");
            }

            if (!obtainedVal.Equals(focusOnNowVal))
            {
                FailStep(CL, "Not focused on " + focusOnNowVal);
            }

            //Verify channel number
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get channel name From Channel Bar");
            }

            if (obtainedVal != videoService.LCN)
            {
                FailStep(CL, "Not tuned to channel: " + videoService.LCN);
            }

            //Verify channel name
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out obtainedVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get channel name From Channel Bar");
            }

            if (obtainedVal != videoService.Name)
            {
                FailStep(CL, "Not tuned to channel: " + videoService.Name);
            }

            //Verify channel logo

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Verify event name.
            string obtainedVal = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out obtainedVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            if (string.IsNullOrEmpty(obtainedVal))
            {
                FailStep(CL, "event name information is not displayed");
            }

            //Verify event starttime and endtime is displayed.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out obtainedVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event start and end time From Channel Bar");
            }

            if (string.IsNullOrEmpty(obtainedVal))
            {
                FailStep(CL, "event start and end time information is not displayed");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Navigate to channel bar and select NEXT
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch channel bar");
            }

            //Focus on NEXT on channel bar
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select NEXT menu item.");
            }

            //Verify if focus is there on NEXT
            string obtainedVal = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get channel name From Channel Bar");
            }

            if (!obtainedVal.Equals(focusOnNextVal))
            {
                FailStep(CL, "Not focused on " + focusOnNextVal);
            }

            PassStep();
        }
    }

    #endregion Step4

    #region Step5

    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            LogCommentInfo(CL, "Wait for " + defaultChannelBarTimeOut + "sec till channel bar is disposed");

            //Wait till the channel bar is dimissed
            res = CL.IEX.Wait(defaultChannelBarTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for untill channel bar is dismissed");
            }

            //Navigate to Action bar of Next event
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR FROM CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to action bar of future event");
            }

            PassStep();
        }
    }

    #endregion Step5

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}