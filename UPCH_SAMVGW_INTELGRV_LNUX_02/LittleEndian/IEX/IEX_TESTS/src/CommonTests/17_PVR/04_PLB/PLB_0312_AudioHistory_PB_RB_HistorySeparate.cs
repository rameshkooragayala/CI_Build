/// <summary>
///  Script Name : PLB_0312_AudioHistory_PB_RB_HistorySeparate.cs
///  Test Name   : PLB-0312-Audio history - PB RB history separate
///  TEST ID     : 73844
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : FR_FUSION/UPC
/// -----------------------------------------------
///  Modified by : MadhuKumar K
///  Modified Date: 01st April, 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;

public class PLB_0312 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    public const string eventBasedeRecording = "EVENTBASED_RECORDING"; //Event to be Recorded on Radio channel
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    private static string nextAudio = "";
    private static string selectAudio = "";
    private static string defaultAudio = "";
    private static string selectedAudioOnLive = "";
    private static string selectedAudioOnPlayback = "";
    private static string currentSelection = "";
    private static string audioUpdateMilestone = "";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Sync; have a Recording on the disk and change the Default Audio(A1 to A2)");
        this.AddStep(new Step1(), "Step 1: Playback the Record and verify the Default Audio(A1)");
        this.AddStep(new Step2(), "Step 2: Change the Audio to Non-Default Audio(A1 to A3) and verify");
        this.AddStep(new Step3(), "Step 3: Go back to Live and verify the Audio selected Previously is present(A2)");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    private static class Constants
    {
        public const int waitAfterSendingCommand = 1000;
        public const int requiredMinDurationInMin = 5;//Minimum Duration required in the current event 
        public const double waitInRecordingInSec = 60;//Waiting for this much time inside the recording and stops the recording
        public const int timeoutForAudioMilestone = 60;
    }
    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Fetching a recordable service
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High;NoOfAudioLanguages=0,1,2");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
            }

            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            if (nextAudio == "")
            {
                FailStep(CL,res,"Next Audio key is not defined in the Project ini");
            }
            LogCommentInfo(CL, "Next Audio key fetched from project ini is : " + nextAudio);

            //Fetch Audio Select Key from Project ini file
            selectAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "SELECT_KEY");
            if (selectAudio == "")
            {
                FailStep(CL,res,"Select Audio key is not defined in the Project ini");
            }
            LogCommentInfo(CL, "Select Key fetched from Project ini : " + selectAudio);

            //Verify for milestone on change of audio
            audioUpdateMilestone = CL.EA.UI.Utils.GetValueFromMilestones("AudioChange");

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
            if (timeLeftInSec < Constants.requiredMinDurationInMin*60)
            {
                res = CL.IEX.Wait(seconds: timeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait until the event ends");
                }
            }
            //Navigate to option audio from Live

            res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to fetch the Default Audio title from EPG");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out selectedAudioOnLive);
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

            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventBasedeRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record current event from banner");
            }
            res = CL.IEX.Wait(seconds:Constants.waitInRecordingInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait few min");
            }
            res = CL.EA.PVR.StopRecordingFromBanner(eventBasedeRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop the recording from banner");
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
            res = CL.EA.PVR.PlaybackRecFromArchive(eventBasedeRecording, SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            //Navigate to option audio on Playback and verify that it is a default Audio

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }
            if (currentSelection != defaultAudio)
            {
                FailStep(CL, "Current selection "+currentSelection+" is differet from the defaut Audio "+defaultAudio);
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
            //Selecting a Audio which is non-Default by sending two Next Audio keys
            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }
            //get the Selected Audio on Playback
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
            //Verifying whether the current selection is same as the previously selected Audio on PlayBack

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }


            //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }
            if (currentSelection != selectedAudioOnPlayback)
            {
                FailStep(CL, "Current selection " + currentSelection + " is differet from the defaut Audio " + defaultAudio);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to return to Playback viewing");
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
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop the Playback");
            }
            //After Returning to Live we are verifying whether the Current selection is same as the Previously selected Audio
            //Navigate to option audio from Live

            res = CL.EA.NavigateAndHighlight("STATE:AV SETTINGS AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }
            if (currentSelection != selectedAudioOnLive)
            {
                FailStep(CL, "Current selection " + currentSelection + " is differet from the selected Audio " + selectedAudioOnLive);
            }
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //delete the failed recorded event
        res = CL.EA.PVR.DeleteRecordFromArchive(eventBasedeRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Recorded Event");
        }
    }

    #endregion PostExecute
}