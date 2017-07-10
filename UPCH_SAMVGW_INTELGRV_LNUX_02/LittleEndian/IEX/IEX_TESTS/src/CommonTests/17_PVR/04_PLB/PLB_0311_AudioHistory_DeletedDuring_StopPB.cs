/// <summary>
///  Script Name : PLB_0311_AudioHistory_DeletedDuring_StopPB.cs
///  Test Name   : PLB-0311-Audio history - deleted during stop PB
///  TEST ID     : 73843
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : FR_FUSION/UPC
/// -----------------------------------------------
///  Modified by : MadhuKumar K
///  Modified Date: 01st Apr, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;

public class PLB_0311 : _Test
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
    private static string selectedAudio = "";
    private static string currentSelection = "";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Sync; have a Recording on the disk ");
        this.AddStep(new Step1(), "Step 1: Playback the Record and change the audio selection");
        this.AddStep(new Step2(), "Step 2: Stop the playback");
        this.AddStep(new Step3(), "Step 3: Restart Playback of the recording");
        this.AddStep(new Step4(), "Step 4: Verify that the default audio is played");
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    private static class Constants
    {
        public const int waitAfterSendingCommand = 1000;
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
                FailStep(CL,"Next Audio key is not defined in the Project ini");
            }
            LogCommentInfo(CL, "Next Audio key fetched from project ini is : " + nextAudio);

            //Fetch Audio Select Key from Project ini file
            selectAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "SELECT_KEY");
            if (selectAudio == "")
            {
                FailStep(CL, "Select Audio key is not defined in the Project ini");
            }
            LogCommentInfo(CL, "Select Key fetched from Project ini : " + selectAudio);

            //Navigate to next option in the list
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventBasedeRecording,MinTimeBeforeEvEnd:5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current event from Banner");
            }
            res = CL.EA.WaitUntilEventEnds(eventBasedeRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
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
            //Changing the Audio on Playback

            res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            //get the current selection
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to fetch the Default subtitle from EPG");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next audio in the list");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out selectedAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current selection");
            }

            //Verify for milestone on change of audio
            String audioUpdateMilestone = CL.EA.UI.Utils.GetValueFromMilestones("AudioChange");

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

            //Verifying whether the Audio which is set previously is the highlighted one or not.
            if (currentSelection != selectedAudio)
            {
                FailStep(CL,"Audio has been changed to "+currentSelection+" after setting it"+selectedAudio);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to return to Playback viewing");
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
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop the Playback");
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
            res = CL.EA.PVR.PlaybackRecFromArchive(eventBasedeRecording, SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }


            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            LogCommentInfo(CL,"Verifying that the Default Audio is Played Back once we start the Plack back again");

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
                FailStep(CL, "Audio has been changed to " + currentSelection + " after setting it" + selectedAudio);
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to stop the Playback");
        }
        //delete the failed recorded event
        res = CL.EA.PVR.DeleteRecordFromArchive(eventBasedeRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Recorded Event");
        }
    }

    #endregion PostExecute
}