/// <summary>
///  Script Name : ACTION_0110_AudioChange_RB
///  Test Name   : ACTION-0110-Audio Change-RB
///  TEST ID     : 67765
///  QC Version  : 1
///  Jira ID     : FC-539
///  Variations from QC:None
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 30 JULY 2013

using System;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using System.Collections.Generic;
using System.Data;

[Test("ACTION-0110-Audio Change-RB")]
public class ACTION_0110 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service multipleAudioService;
    private static string defaultAudio = "";
    private static string audioToChangeTo = "";
    private static string nextAudio = "";
    private static int audioCheckTimeout = 0;
	private static int defaultBannerTimeout = 0;
    private static Dictionary<EnumEpgKeys, String> KeyLanguage = new Dictionary<EnumEpgKeys, String>();
    
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Change Audio stream on RB";

    private static class Constants
    {
        public const int msWaitAfterSendingIRKey = 5000;
        public const int timeout = 60;
        public const int pause = 0;
        public const int rbInitialDepth = 20;
        public const int play = 1;
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);

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
            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            LogCommentInfo(CL, "Next Audio feteched from project ini is : " + nextAudio);

            audioCheckTimeout = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC"));
            LogCommentInfo(CL, "Audio check timeout feteched from project ini is : " + audioCheckTimeout);

			defaultBannerTimeout = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT")); 
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, multipleAudioService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to MultipleAudioService");
            }

            //Pause and check for paused video
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.pause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }

            res = CL.EA.CheckForVideo(false, false, Constants.timeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }

            //Play  From RB
            res = CL.IEX.Wait(Constants.rbInitialDepth);
            if (!res.CommandSucceeded)
            {
                LogComment(CL, "Failed during Waiting to fill RB");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }
            res = CL.IEX.Wait(defaultBannerTimeout);
            if (!res.CommandSucceeded)
            {
                LogComment(CL, "Failed to wait till banner dismissal");
            }
            //Change the audio on RB from action menu;
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", KeyLanguage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS AUDIO");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Audio Track Name");
            }

            LogCommentInfo(CL, "The default Audio is: " + defaultAudio);
            bool defaultAudioAvailable = multipleAudioService.AudioLanguage.Contains(defaultAudio);
            if (!defaultAudioAvailable)
            {
                FailStep(CL, res, "Default audio not listed in the service fetched from Content xml ", false);
            }

            //Change to any audio
            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.msWaitAfterSendingIRKey);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            //Get destination audio
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out audioToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Audio Track Name" + audioToChangeTo);
            }

            bool isChangedAudioAvailable = multipleAudioService.AudioLanguage.Contains(audioToChangeTo);
            if (!isChangedAudioAvailable)
            {
                FailStep(CL, res, "Changed audio not listed in the service fetched from Content xml ", false);
            }
            //Verify for milestone on change of audio
            String audioUpdateMilestone = CL.EA.UI.Utils.GetValueFromMilestones("AudioChange");

            //Begin wait for audio update milestone.

            //Select the audio
            //KeyLanguage.Add(EnumEpgKeys.TITLE, audioToChangeTo);
            //res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", KeyLanguage);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to Navigate to AV SETTINGS AUDIO");
            //}
            CL.EA.UI.Utils.BeginWaitForDebugMessages(audioUpdateMilestone, Constants.timeout);

            CL.EA.UI.Utils.SendIR("SELECT");
            ArrayList arrayList = new ArrayList();

            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(audioUpdateMilestone, ref arrayList))
            {
                FailStep(CL, "Failed to verify audio update milestone");
            }

            res = CL.EA.CheckForAudio(true, audioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio");
            }

            if (audioToChangeTo.Equals(defaultAudio))
            {
                FailStep(CL, "Failed to change audio,as only one audio is present in the list.");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
        //Return to default original audio
        IEXGateway._IEXResult res;

        KeyLanguage.Clear();
        KeyLanguage.Add(EnumEpgKeys.TITLE, defaultAudio);
        res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", KeyLanguage);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to Navigate to AV SETTINGS AUDIO");
        }
        CL.EA.UI.Utils.SendIR("SELECT");
        
        res = CL.EA.ReturnToLiveViewing();
    }

    #endregion PostExecute
}