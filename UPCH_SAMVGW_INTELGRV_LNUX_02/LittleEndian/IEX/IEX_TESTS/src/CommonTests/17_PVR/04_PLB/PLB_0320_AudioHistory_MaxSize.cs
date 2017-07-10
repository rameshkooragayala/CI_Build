/// <summary>
///  Script Name : PLB_0320_AudioHistory_MaxSize.cs
///  Test Name   : PLB-0320-Audio history - max size
///  TEST ID     : 73917
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

public class PLB_0320 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    public const string timeBasedeRecording = "TIMEBASED_RECORDING"; //Event to be Recorded on Radio channel
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    private static string nextAudio = "";
    private static string selectAudio = "";
    private static string defaultAudio = "";
    private static string selectedAudio = "";
    private static string currentSelection = "";
    private static int durationInMin;
    private static int medRewTrickmode;
    private static int medFastFwdTrickmode;
    private static int timeToNextEvent;
    private static int maxNoOfPersistentAudioRemembered;
    private static List<string> defaultAudioList = new List<string>();
    private static List<string> selectedAudioList = new List<string>();
    //Verify for milestone on change of audio
    private static string audioUpdateMilestone = "";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Sync; have a Recording on the disk and playback the recording");
        this.AddStep(new Step1(), "Step 1: Modify the Default audio for (Max No of presistent Audio remembers)+1  events");
        this.AddStep(new Step2(), "Step 2: Rewind to the beginning of the event and verify Deafult Audio is present for the first event and selected Audio for the remaining");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    private static class Constants
    {
        public const int waitAfterSendingCommand = 2000;
        public const int delayUntilEvtStartTime = 240;
        public const int timeoutForAudioMilestone = 60;
    }
    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Fetcing a recordable service
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
            LogCommentInfo(CL, "Next Audio key fetched from project ini is : " + nextAudio);

            //Fetch Subtitle Select Key from Project ini file
            selectAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "SELECT_KEY");

            LogCommentInfo(CL, "Select Key fetched from Project ini : " + selectAudio);
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
            //Navigate to next option in the list

            //Fetch the medium FF trickmode
            String fFwdfTrickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD");
            if (String.IsNullOrEmpty(fFwdfTrickModeArrayInStr))
            {
                FailStep(CL, "Fast Forward Trick mode list not present in Project.ini file.");
            }
            String[] fFwdTrickModeArray = fFwdfTrickModeArrayInStr.Split(',');
            String fFwdTrickModeInStr = fFwdTrickModeArray[((fFwdTrickModeArray.Length) / 2)-1];
            medFastFwdTrickmode = int.Parse(fFwdTrickModeInStr);
            LogComment(CL, "Medium Fast Forward Trick mode speed fetched" + medFastFwdTrickmode);
            //Navigate to next option in the list

            audioUpdateMilestone = CL.EA.UI.Utils.GetValueFromMilestones("AudioChange");

            maxNoOfPersistentAudioRemembered =Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MAX_PERSISTENT_AUDIO_REMEMBERED"));
            if (maxNoOfPersistentAudioRemembered==0)
            {
                FailStep(CL, "maxNoOfPersistentAudioRemembered is not defined in test ini");
            }


            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService.LCN);
            }

            durationInMin = Convert.ToInt32(recordableService.EventDuration) * (maxNoOfPersistentAudioRemembered+1);
            int timeLeftInSec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current event left time");
            }

            LogCommentImportant(CL, "Current event left time is " + timeLeftInSec);
            if (timeLeftInSec < Constants.delayUntilEvtStartTime)
            {
                res = CL.IEX.Wait(seconds: timeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Wait few seconds");
                }
                //Refreshing the EPG to get the Current time from EPG
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to main menu", exitTest: false);
                }
                res = CL.EA.PVR.RecordManualFromPlanner(timeBasedeRecording, recordableService.Name, DaysDelay: -1, MinutesDelayUntilBegining: Convert.ToInt32(recordableService.EventDuration), DurationInMin: durationInMin);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to record Manual from planner");
                }
            }
            else
            {
                res = CL.EA.PVR.RecordManualFromPlanner(timeBasedeRecording, recordableService.Name, DaysDelay: -1, MinutesDelayUntilBegining: Convert.ToInt32(timeLeftInSec / 60), DurationInMin: durationInMin);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to record Manual from planner");
                }
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

            //Navigate to option audio from PB
            for (int count = 0; count <= maxNoOfPersistentAudioRemembered; count++)
            {
                res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
                }

                //get the current selction
                res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultAudio);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to fetch the Default audio from EPG");
                }
                defaultAudioList.Add(defaultAudio);
                string timeStamp = "";
                res = CL.IEX.IR.SendIR(nextAudio, out timeStamp, Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to next audio/subtitle in the list");
                }
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
                selectedAudioList.Add(selectedAudio);

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

                res = CL.EA.ReturnToPlaybackViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to return to playback viewing");
                }
                if (count != maxNoOfPersistentAudioRemembered)
                {
                    res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, medFastFwdTrickmode, false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set the trick mode speed");
                    }
                    timeToNextEvent = ((Convert.ToInt32(recordableService.EventDuration) - 1) * 60) / medFastFwdTrickmode;
                    res = CL.IEX.Wait(timeToNextEvent);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to wait few min");
                    }
                    res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, Speed: 1, Verify_EOF_BOF: false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set the trick mode speed");
                    }
                    res = CL.EA.ReturnToPlaybackViewing();
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to return to playback viewing");
                    }
                }
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
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, Speed: medRewTrickmode, Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to " + medRewTrickmode);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to playback viewing");
            }
            //Navigate to option audio from Live
            for (int count = 0; count <=maxNoOfPersistentAudioRemembered; count++)
            {
                res = CL.EA.NavigateAndHighlight("STATE:PB AV SETTING AUDIO", dictionary);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
                }

                //get the current selction
                res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currentSelection);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to fetch the Default subtitle from EPG");
                }
                if (count == 0)
                {
                    if (currentSelection != defaultAudio)
                    {
                        FailStep(CL, "Current selection is different" + currentSelection + " from default audio " + defaultAudio);
                    }
                    else
                    {
                        LogCommentImportant(CL,"Current selection "+currentSelection+" is same as the "+defaultAudio);
                    }

                }
                else
                {
                    if (currentSelection != selectedAudioList[count])
                    {
                        FailStep(CL, "Current selection" + currentSelection + " is different from the previously selected audio " + selectedAudioList[count]);
                    }
                    else
                    {
                        LogCommentImportant(CL,"Current selection "+currentSelection+" is same as "+selectedAudioList[count]);
                    }
 
                }

                res = CL.EA.ReturnToPlaybackViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to return to playback viewing");
                }
                if (count != maxNoOfPersistentAudioRemembered)
                {
                    res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, medFastFwdTrickmode, false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set the trick mode speed " + medFastFwdTrickmode);
                    }
                    timeToNextEvent = ((Convert.ToInt32(recordableService.EventDuration) - 1) * 60) / medFastFwdTrickmode;
                    res = CL.IEX.Wait(timeToNextEvent);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to wait few min");
                    }
                    res = CL.EA.PVR.SetTrickModeSpeed(timeBasedeRecording, Speed: 1, Verify_EOF_BOF: false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set the trick mode speed to 1");
                    }
                    res = CL.EA.ReturnToPlaybackViewing();
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to return to playback viewing");
                    }
                }
            }
            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

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