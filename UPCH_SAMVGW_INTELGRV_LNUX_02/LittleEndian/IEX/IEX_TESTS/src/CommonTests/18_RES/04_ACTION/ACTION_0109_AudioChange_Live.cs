/// <summary>
///  Script Name : ACTION_0109_AudioChange_Live
///  Test Name   : ACTION-0109-Audio Change-Live
///  TEST ID     : 67766
///  QC Version  : 1
///  Jira ID     : FC-243
///  Variations from QC:None
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 30 JULY 2013

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using IEX.Tests.Utils;
[Test("ACTION_0109")]
public class ACTION_0109 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service multipleAudioService;
    private static string defaultAudio = "";
    private static string audioToChangeTo = "";
    private static string nextAudio = "";
    private static int audioCheckTimeout = 0;
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Change Audio stream on Live and verify that audio is changed. ";

    private static class Constants
    {
        public const int msWaitAfterSendingIRKey = 5000;
        public const int timeoutForAudioMilestone = 60;
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

            //Get value from project ini
            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            LogCommentInfo(CL, "Next Audio feteched from project ini is : " + nextAudio);

            audioCheckTimeout = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC"));
            LogCommentInfo(CL, "Audio check timeout feteched from project ini is : " + audioCheckTimeout);

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

            //Change the audio from action menu
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
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
            CL.EA.UI.Utils.BeginWaitForDebugMessages(audioUpdateMilestone, Constants.timeoutForAudioMilestone);

            //Select the audio
            //dictionary.Add(EnumEpgKeys.TITLE, audioToChangeTo);
            //res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to Navigate to AV SETTINGS AUDIO");
            //}

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

        //res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTINGS AUDIO ");
        //if (!res.CommandSucceeded)
        //{
        //    CL.IEX.FailStep("Failed to traverse to AV settings Audio");
        //}
        //res = CL.IEX.MilestonesEPG.Navigate(defaultAudio);
        dictionary.Clear();
        dictionary.Add(EnumEpgKeys.TITLE, defaultAudio);
        res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to Navigate to AV SETTINGS AUDIO");
        }
        CL.EA.UI.Utils.SendIR("SELECT");
        res = CL.EA.ReturnToLiveViewing();
    }

    #endregion PostExecute
}