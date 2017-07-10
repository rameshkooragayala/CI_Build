/// <summary>
///  Script Name : EPG_0213_ActionMenu_RB_Change_DVB_Subtitle
///  Test Name   : EPG-0213-ActionMenu-RB-Change-DVB-Subtitle
///  TEST ID     : 63822
///  QC Version  : 1
///  Variations from QC:None
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 17 Sept 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[Test("ACTION_EPG_0213")]
public class ACTION_EPG_0213 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service multilpleSubtitleService;
    private static Service hardOfHearingSubtitleService;
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    private static string defaultSubtitle = "";
    private static string nextSubtitle = "";
    private static string subtitleToChangeTo = "";
    private static string defaultSettingSubtitle = "";
    private static string hardOfHearingLcn = "";
    private static bool isReviewBuffer = false;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get multiple DVB subtitle service from content xml";
    private const string STEP1_DESCRIPTION = "Step 1: Collect the review buffer on multiple DVB subtitle service.";
    private const string STEP2_DESCRIPTION = "Step 2: Change the subtitle  and verify the subtitle is changed. ";
    private const string STEP3_DESCRIPTION = "Step 3: Deactivate the subtitles from action menu and verify that the subtitles are disabled ";
    private const string STEP4_DESCRIPTION = "Step 4: Enable hard of hearings from settings and verify that the subtitles are enabled and displayed. ";


    private static class Constants
    {
        public const int timeInPlayback = 15;
        public const int timeToWaitinPause = 15;
        public const int timeToWaitinEachService = 90;
        public const int timeToRewind = 15;
        public const int waitForMilestones = 30;
        public const int speedForPlay = 1;
        public const int speedForPause = 0;
        public const int speedForRewind = -30;
        public const int msWaitAfterSendingIRKey = 5000;
        public const int timeoutForSubtitleMilestone = 60;
    }
    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        CL = GetClient();
        //Get value from Test ini
        isReviewBuffer = Convert.ToBoolean(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ReviewBuffer"));
        LogCommentInfo(CL, "Value Of review buffer is : " + isReviewBuffer);

        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        if (isReviewBuffer)
        {
            this.AddStep(new Step1(), STEP1_DESCRIPTION);
        }
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        //Get Client Platform
        
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

            //Get Values From Content XML File
            multilpleSubtitleService = CL.EA.GetServiceFromContentXML("Type=Video;SubtitleType=Dvb", "NoOfSubtitleLanguages=0,1;ParentalRating=High");
            if (multilpleSubtitleService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "multilpleSubtitleService: " + multilpleSubtitleService.LCN);
            }

            //Get value from Test ini
            hardOfHearingLcn = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LCN");
            LogCommentInfo(CL, "The hard of hearing LCN is : " + hardOfHearingLcn);

            //Get Values From Content XML File
            hardOfHearingSubtitleService = CL.EA.GetServiceFromContentXML("LCN="+hardOfHearingLcn);
            if (hardOfHearingSubtitleService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "hardOfHearingSubtitleService: " + hardOfHearingSubtitleService.LCN);
            }
            
            //Get value from project ini
            nextSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_SUBTITLE");
            LogCommentInfo(CL, "Next Subtitle feteched from project ini is : " + nextSubtitle);


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

            //tuning to the multiple DVB subtitle service

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, multilpleSubtitleService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + multilpleSubtitleService.LCN);
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause on Live");
            }

            res = CL.IEX.Wait(Constants.timeToWaitinPause);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play RB");
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

            //Get the default subtitle from settings
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Settings Subtitles");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultSettingSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current subtitle");
            }
            CL.IEX.Wait(5);

            //Change the subtitle from action menu
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS Subtitles");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current subtitle");
            }

            LogCommentInfo(CL, "The default Subtitle is: " + defaultSubtitle);
            bool defaultSubtitleAvailable = multilpleSubtitleService.SubtitleLanguage.Contains(defaultSubtitle);
            if (!defaultSubtitleAvailable)
            {
                FailStep(CL, res, "Default Subtitle not listed in the service fetched from Content xml ", false);
            }

            //Change to any subtitle
            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextSubtitle, out timeStamp, Constants.msWaitAfterSendingIRKey);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            //Get destination subtitle
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out subtitleToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next subtitle Track Name" + subtitleToChangeTo);
            }
            LogCommentInfo(CL, "The changed subtitle is: " + subtitleToChangeTo);

            bool isChangedSubtitleAvailable = multilpleSubtitleService.SubtitleLanguage.Contains(subtitleToChangeTo);
            if (!isChangedSubtitleAvailable)
            {
                FailStep(CL, res, "Changed subtitle not listed in the service fetched from Content xml ", false);
            }

            //Select the subbtitle
            CL.EA.UI.Utils.SendIR("SELECT");
            CL.IEX.Wait(10);
            //Verify that the subtitles are changed
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS Subtitles");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out subtitleToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current subtitle");
            }

            if ((subtitleToChangeTo) != (defaultSettingSubtitle))
            {
                LogCommentInfo(CL, "The setting overrides, for the current programme, parameter declared in the main user settings menu.");

            }
            else
            {
                FailStep(CL, "Failed to change subtitles on the current service");
            }

            PassStep();
        }
    }

    #endregion Step2
    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //change the subtitle to OFF and verify
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANGE SUBTITLE TO OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change to subtitle OFF");
            }

            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Settings Subtitles");
            }
            string OffSubtitle = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out OffSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get OffSubtitle");
            }
            if (OffSubtitle == "OFF")
            {
                LogComment(CL, "The subtitles has been disabled successfully");
            }
            else
            {
                FailStep(CL, res, "Failed to disable subtitles on action menu");
            }
            PassStep();
        }
    }

    #endregion Step3
    #region Step4

    [Step(1, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //STATE:SETTINGS SUBTITLE ON
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE ON");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set SUBTITLES ON");
            }
            //Set the hard of hearing subtitles ON
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLE HARD OF HEARING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set SUBTITLE HARD OF HEARING ON");
            }
            //tuning to the heard of hearing subtitle service

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, hardOfHearingSubtitleService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + hardOfHearingSubtitleService.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTING SUBTITLES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to subtitles on action menu");
            }
            string hardOfHearingSubtitle = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out hardOfHearingSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get OffSubtitle");
            }
            if (hardOfHearingSubtitle != "OFF")
            {
                LogComment(CL, "The hard of hearing subtitles enabled successfully");
            }
            
            else
            {
                FailStep(CL, res, "Failed to enable hard of hearing subtitles on action menu");
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            PassStep();
        }
    }

    #endregion Step4
    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE OFF");
        if (!res.CommandSucceeded)
        {
            CL.IEX.LogComment("Failed to set SUBTITLES ON");
        }
        res = CL.EA.ReturnToLiveViewing(true);
        if (!res.CommandSucceeded)
        {
            CL.IEX.LogComment("Failed to Return To Live Viewing");
        }
        res = CL.EA.PVR.StopPlayback(true);
        if (!res.CommandSucceeded)
        {
            CL.IEX.LogComment("Failed to Return to Live Viewing From RB");
        }
        
    }

    #endregion PostExecute
}