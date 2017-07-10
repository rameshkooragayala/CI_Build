/// <summary>
///  Script Name : LPW_0912_Verify_AudioSettingRetainsAfterStandBy.cs
///  Test Name   : LPW-0912-Verify-AudioSettingRetainsAfterStandBy
///  TEST ID     : 73346
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Modified by : Madhu Renukaradhya
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("LPW_0912")]
public class LPW_0912 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    private static Service multipleAudioService;
    private static string defaultAudio = "";
    private static string audioToChangeTo = "";
    private static string nextAudio = "";
    static string powerMode = "";
    static string defaultPowerMode = "";
    static string audioAfterStandBy = "";
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Tune to service S1 and change the audio  ";
    private const string STEP2_DESCRIPTION = "Step 2: Switch the box to standby ";
    private const string STEP3_DESCRIPTION = "Step 3: Verify that the audio setting are reset to default after waking from stand by";


    private static class Constants
    {
        public const int msWaitAfterSendingIRKey = 5000;
        public const int timeoutForAudioMilestone = 10;

    }

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
    

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
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            LogCommentInfo(CL, "Next Audio feteched from project ini is : " + nextAudio);

            //Get Values From ini File
            multipleAudioService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;NoOfAudioLanguages=0,1");
            if (multipleAudioService == null)
            {
                FailStep(CL, "Failed to fetch MultipleAudioService from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "MultipleAudioService fetched from content xml is : " + multipleAudioService.LCN);
            }

            //Get Values From ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }
            //powerMode = "HOT STANDBY";

            //Get default mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini",true);
            }
            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, multipleAudioService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to MultipleAudioService");
            }

            //Change the audio from action menu
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Audio on action menu ");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Audio Track Name");
            }
            else
            {
                LogCommentInfo(CL, "The default Audio is: " + defaultAudio);
            }

            //Change to any audio
            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.msWaitAfterSendingIRKey);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            //Get destination audio
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out audioToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Audio Track Name" + audioToChangeTo);
            }
            else
            {
                LogCommentInfo(CL, "The Changed Audio is: " + audioToChangeTo);
            }

           
            //Select the audio
            res = CL.IEX.MilestonesEPG.Navigate(audioToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select Audio Track: " + audioToChangeTo);
            }


            if (audioToChangeTo.Equals(defaultAudio))
            {
                FailStep(CL, "Failed to change audio,as only one audio is present in the list.");
            }


            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully");
            }

            //verify set power mode
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode, jobPresent:false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to verify power mode option");
            }
            else
            {
                LogCommentInfo(CL, "Set Power mode verified Successfully");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Audio on action menu ");
            }
            //Get destination audio
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out audioAfterStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Audio Track Name" + audioToChangeTo);
            }
            else
            {
                LogCommentInfo(CL, "Audio settings after standby" + audioAfterStandBy);
            }
            if (audioAfterStandBy.Equals(defaultAudio))
            {
                LogCommentInfo(CL, "Audio settings retained to default after standby");
            }
            else
            {
                FailStep(CL, res, "Audio settings not retained to default after standby");
            }
            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        //Restore default settings
        IEXGateway._IEXResult res;
        res = CL.EA.NavigateAndHighlight("STATE:STANDBY POWER USAGE", dictionary);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to navigate to settings POWER MANAGEMENT");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defaultPowerMode);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default POWER MODE");
        }
        else
        {
            LogCommentFailure(CL, "Restored to default POWER MODE SUCCESSFULLY");
        }
        
        //Restore to default Audio
        res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to traverse to AV settings Audio");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defaultAudio);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Select back to default Track: " + defaultAudio);
        }

        res = CL.EA.ReturnToLiveViewing();
    }
    #endregion
}