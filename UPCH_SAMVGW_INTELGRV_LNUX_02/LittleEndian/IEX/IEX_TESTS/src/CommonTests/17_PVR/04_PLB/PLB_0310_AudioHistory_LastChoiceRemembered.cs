/// <summary>
///  Script Name : PLB_0310_AudioHistory_LastChoiceRemembered.cs
///  Test Name   : PLB-0310-Audio history - last choice remembered
///  TEST ID     : 73916
///  QC Version  : 2
///  QC Repository: FR_Fusion/UPC
///  Variations from QC: None
/// -----------------------------------------------
///  Modified by : Madhu Kumar k
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;

public class PLB_0310 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    public const string timeBasedeRecording = "TIMEBASED_RECORDING"; //Event to be Recorded
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    private static string nextAudio = "";
    private static string selectAudio = "";
    private static string defaultAudio = "";
    private static string selectedAudioOnPlayback = "";
    private static string currentSelection = "";
    private static int medRewTrickmode;
    private static int medFastFwdTrickmode;
    private static int timeToNextEvent;
    //Verify for milestone on change of audio
    private static string audioUpdateMilestone = "";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Sync; have a Recording on the disk and playback the Recording");
        this.AddStep(new Step1(), "Step 1: Change the Default Audio(A1 to A2)");
        this.AddStep(new Step2(), "Step 2: Again change the Audio while playing back the same Event(A2 to A3)");
        this.AddStep(new Step3(), "Step 3: Fast Forward to the next event in the playback and verify that the default audio is played(A1)");
        this.AddStep(new Step4(), "Step 4: Rewind to previous event and verify that the previously selected audio is played(A3)");
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    private static class Constants
    {
        public const int waitAfterSendingCommand = 1000;
        public const int timeoutForAudioMilestone = 60;
        public const int minEventDurationRequiredInSec = 300;
    }
    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Fetching a recordable service
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True;IsConstantEventDuration=True", "ParentalRating=High;NoOfAudioLanguages=0,1,2");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
            }

            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            LogCommentInfo(CL, "Next Audio key fetched from project ini is : " + nextAudio);

            //Fetch Audio Select Key from Project ini file
            selectAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "SELECT_KEY");

            LogCommentInfo(CL, "Select Key fetched from Project ini : " + selectAudio);
            audioUpdateMilestone = CL.EA.UI.Utils.GetValueFromMilestones("AudioChange");
            //Fetch the medium REW trickmode
            String rewTrickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            if (String.IsNullOrEmpty(rewTrickModeArrayInStr))
            {
                FailStep(CL, "Rewind Trick mode list not present in Project.ini file.");
            }
            String[] rewTrickModeArray = rewTrickModeArrayInStr.Split(',');
            String rewTrickModeInStr = rewTrickModeArray[(rewTrickModeArray.Length) / 2];
            medRewTrickmode = int.Parse(rewTrickModeInStr);
            LogComment(CL, "Medium rewind Trick mode speed fetched" + medRewTrickmode);

            //Fetch the medium FF trickmode
            String fFwdfTrickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD");
            if (String.IsNullOrEmpty(fFwdfTrickModeArrayInStr))
            {
                FailStep(CL, "Fast Forward Trick mode list not present in Project.ini file.");
            }
            String[] fFwdTrickModeArray = fFwdfTrickModeArrayInStr.Split(',');
            String fFwdTrickModeInStr = fFwdTrickModeArray[(fFwdTrickModeArray.Length) / 2];
            medFastFwdTrickmode = int.Parse(fFwdTrickModeInStr);
            LogComment(CL, "Medium Fast Forward Trick mode speed fetched" + medFastFwdTrickmode);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService.LCN);
            }
            int timeLeftInSec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current event left time");
            }
            LogCommentImportant(CL, "Current event left time is " + timeLeftInSec);
            int timeToWait;
            if (timeLeftInSec > Constants.minEventDurationRequiredInSec)
            {
                timeToWait = Convert.ToInt32(Convert.ToInt32(timeLeftInSec / 60) + (Convert.ToInt32(recordableService.EventDuration)) / 2);
                timeToNextEvent = Convert.ToInt32(Convert.ToInt32(timeLeftInSec / 60));
            }
            else
            {
                res = CL.IEX.Wait(seconds: timeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait until the event ends");
                }
                timeToWait = Convert.ToInt32((Convert.ToInt32(recordableService.EventDuration)) + (Convert.ToInt32(recordableService.EventDuration)) / 2);
                timeToNextEvent = Convert.ToInt32(Convert.ToInt32(recordableService.EventDuration));
            }

            LogComment(CL, "Time to wait is " + timeToWait);
            res = CL.EA.PVR.RecordManualFromCurrent(timeBasedeRecording, recordableService.LCN, timeToWait);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from current");
            }
            res = CL.EA.WaitUntilEventEnds(timeBasedeRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }
            res = CL.EA.PVR.PlaybackRecFromArchive(timeBasedeRecording, SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Navigate to option audio from Live

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG info");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out selectedAudioOnPlayback);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            //Begin wait for audio update milestone.
            CL.EA.UI.Utils.BeginWaitForDebugMessages(audioUpdateMilestone, Constants.timeoutForAudioMilestone);
            res = CL.IEX.IR.SendIR(selectAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            ArrayList arrayList = new ArrayList();

            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(audioUpdateMilestone, ref arrayList))
            {
                FailStep(CL, "Failed to verify audio update milestone");
            }
            //Navigate to option audio from Live

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:PB AV SETTING AUDIO");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            if (selectedAudioOnPlayback != currentSelection)
            {
                FailStep(CL, "Audio is not changed after selecting the next audio, Expected is " + selectedAudioOnPlayback + " current selection " + currentSelection);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to the playback viewing");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Navigate to option audio from Live

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out selectedAudioOnPlayback);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            //Begin wait for audio update milestone.
            CL.EA.UI.Utils.BeginWaitForDebugMessages(audioUpdateMilestone, Constants.timeoutForAudioMilestone);
            res = CL.IEX.IR.SendIR(selectAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            ArrayList arrayList = new ArrayList();

            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(audioUpdateMilestone, ref arrayList))
            {
                FailStep(CL, "Failed to verify audio update milestone");
            }
            //Navigate to option audio from PlayBack

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:PB AV SETTING AUDIO");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            if (selectedAudioOnPlayback != currentSelection)
            {
                FailStep(CL, "Audio is not changed after selecting the next audio, Expected is " + selectedAudioOnPlayback + " current selection " + currentSelection);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to the playback viewing");
            }


            PassStep();
        }
    }

    #endregion Step2
    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
			//Subtracting 2 from the wait to make sure we are not skipping the event in the middle due to delay for setting the trickmode and Audio
            double waitInTrickModeSpeed = Convert.ToDouble(((timeToNextEvent - 2) * 60) / medFastFwdTrickmode);

            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, Speed: medFastFwdTrickmode, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to " + medFastFwdTrickmode);
            }
            res = CL.IEX.Wait(waitInTrickModeSpeed);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in few min Trick mode speed " + waitInTrickModeSpeed);
            }
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, Speed: 1, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 1");
            }

            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to the playback viewing");
            }
            //Navigate to option audio from Live

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            if (currentSelection != defaultAudio)
            {
                FailStep(CL, "Current Selection is " + currentSelection + " different from the default selection " + defaultAudio);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to the playback viewing");
            }
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, Speed: medRewTrickmode, Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to " + medRewTrickmode);
            }

            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to the playback viewing");
            }
            //Navigate to option audio from Playback

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            if (currentSelection != selectedAudioOnPlayback)
            {
                FailStep(CL, "Current Selection is " + currentSelection + " different from the selectied Audio on playback " + selectedAudioOnPlayback);
            }

            PassStep();
        }
    }

    #endregion Step4
    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.ReturnToPlaybackViewing();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to return to the playback viewing");
        }
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to stop the Playback");
        }

        res = CL.EA.PVR.DeleteRecordFromArchive(timeBasedeRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete Time based recording from Archive");
        }
    }

    #endregion PostExecute
}