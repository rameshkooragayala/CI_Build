/// <summary>
///  Script Name : BANNER_1211_MultipleEvents.cs
///  Test Name   : BANNER-1211-Event Based-Multiple Events,PLB-0500-Playback-information-during-cross-events
///  TEST ID     : 63793/71601
///  JIRA ID     : FC-294
///  QC Version  : 2
///  Repository  :STB_DIVISION
/// -----------------------------------------------
///  Modified by          : Bharath Pai /Appanna Kangira
///  Deviations from HPQC : Elements on Playback banner cannot be validated as those are UI elements. We will validate only the event names by
///  raising action bar. This Script covers PLB_0500.
///  Last Modified        :Nov 19 2013
/// </summary>



using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BANNER_1211 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceToBeRecorded; //Service to be recorded
    static Helper helper = new Helper();
    private static string eventToBeRecorded = "CURRENT_EVENT"; //The current event to be recorded
    private static int sgtValue; //SGT value
    private static int egtValue; // EGT value
    private static int fwdTrickMode; //Forward Trick mode
    private static int rewTrickMode; //Rewind Trick mode
    private static string sgtEventName; //Name of the SGT event
    private static string eventToBeRecordedName; //Name of the event to be recorded
    private static string egtEventName; //Name of the EGT event
    private static int defaultTimeToCheckForVideo; //Default Time to check for video
    private static bool rewindFlag; //to inlude/exlude Step 4 which covers Test PLB-500.
    private static string context="";

    static System.Diagnostics.Stopwatch sw;
    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int noOfKeyPresses = 1;
        public const int trickModeForPlay = 1;
        public const bool playbackFromBeginning = true;
        public const bool verifyEofOnPlayback = false;
        public const int secsToPlay = 0;
        public const int trickModeForPause = 0;
       
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Get Client Platform
        CL = GetClient();

        String RewindflagStr = "";

        try
        {
            RewindflagStr = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "REWIND_FLAG");
        }
        catch (Exception ex)
        {
            RewindflagStr = "false";
            LogComment(CL, "Exception caught" + ex);
        }
        //Adding steps of the test case
        this.AddStep(new PreCondition(), "Precondition: Fetch a service required for the test case. Set SGT and EGT to minimum. Initiate an event based recording. Wait for the recording to complete.");
        this.AddStep(new Step1(), "Step 1: Playback the recording, check the SGT event name.");
        this.AddStep(new Step2(), "Step 2: Fast forward into event body, check the event name.");
        this.AddStep(new Step3(), "Step 3: Fast forward into EGT, check the EGT event name.");

		rewindFlag = Convert.ToBoolean(RewindflagStr);
        if (rewindFlag)
        {
            this.AddStep(new Step4(), "Step 4:Rewind from EGT to Event body,check the event name.");
        }

    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Channel Values From ini File
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsMinEventDuration=True;IsConstantEventDuration=True", "IsDefault=True;ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            //Fetch the minimum SGT and EGT values
            String sgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT");
            if (String.IsNullOrEmpty(sgtValueInStr))
            {
                FailStep(CL, "SGT value not present in Test.ini file.");
            }
            LogCommentInfo(CL, "SGT value in minutes - " + sgtValueInStr);
            
            sgtValue = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtValueInStr, true);

            //Fetch EGT Value from TEST INI
            String egtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT");


            if (String.IsNullOrEmpty(egtValueInStr))
            {
                FailStep(CL, " EGT value not present in Test.ini file.");
            }
            LogCommentInfo(CL, "EGT value fetched from Test INI in minutes - " + egtValueInStr);
           
            egtValue = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtValueInStr, false);
            

            //Set SGT & EGT to minimum
            res = CL.EA.STBSettings.SetGuardTime(true, sgtValueInStr);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Start Guard time - " + sgtValueInStr);
            }
            res = CL.EA.STBSettings.SetGuardTime(false, egtValueInStr);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set End Guard time in minutes - " + egtValueInStr);
            }

            //Fetch the  FF trickmode speed
            String fwdTrickModeValueInStrTestINI = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TM_FWD");
            if (String.IsNullOrEmpty(fwdTrickModeValueInStrTestINI))
            {
                FailStep(CL, "Forward Trick mode array not present in Test.ini file.");
            }

            LogCommentInfo(CL, " Forward Trick Mode  fetched from Test INI is -> " + fwdTrickModeValueInStrTestINI);

           //Fetch the Trick mode list from Project INI
            String fwdTrickModeArrayInStrProjectINI = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_FWD");
            if (String.IsNullOrEmpty(fwdTrickModeArrayInStrProjectINI))
            {
                FailStep(CL, "Forward Trick mode List not present in Project.ini file.");
            }

            //compare the Value fetched from Test INI is available in the Project INI List
            if (!fwdTrickModeArrayInStrProjectINI.Contains(fwdTrickModeValueInStrTestINI))
            {
                FailStep(CL, "The fwdtrckmode Value fetched from Test INI does not exist in Project INI,Hence specify Valid Trickmode Value");
            }

            
            
            //Parse the Trick mode FF Value to Int
            fwdTrickMode = int.Parse(fwdTrickModeValueInStrTestINI);
            

           
            //Fetch the default time to check for video
            String defaultVideoCheck = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultVideoCheck))
            {
                FailStep(CL, "Default Video check time not present in Project.ini file.");
            }
            defaultTimeToCheckForVideo = int.Parse(defaultVideoCheck);
            LogCommentInfo(CL, "Default video check time -> " + defaultTimeToCheckForVideo);

            //Tune to service to be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + serviceToBeRecorded.LCN);
            }

            ////Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, defaultTimeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Check for video failed on service " + serviceToBeRecorded.LCN);
            }

            //Book future event from banner
            res = CL.EA.PVR.BookFutureEventFromGuide(eventToBeRecorded, serviceToBeRecorded.LCN, Constants.noOfKeyPresses, sgtValue + 1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event on service " + serviceToBeRecorded.LCN);
            }


            //Launch the channel banner to fetch the current event name
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch channel banner to fetch SGT event name!");
            }

            //Fetch the event name and return to live
            CL.EA.UI.Banner.GetEventName(ref sgtEventName);
            LogCommentInfo(CL, "The SGT Event name is - " + sgtEventName);
            if (String.IsNullOrEmpty(sgtEventName))
            {
                FailStep(CL, "Failed to get the event name of the SGT event.");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to dismiss channel banner.");
            }

            //Get the time left for the current event to end
            int sgtEventTimeLeft = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref sgtEventTimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the time left for event " + sgtEventName + " to end.");
            }
            LogCommentInfo(CL, "Time left for the event " + sgtEventName + " to end is - " + sgtEventTimeLeft + " seconds.");

            //Wait until end of the SGT event
            res = CL.IEX.Wait(sgtEventTimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait " + sgtEventTimeLeft + " second for the event " + sgtEventName + " to end.");
            }

            //Fetch the name of the event which is getting recorded
            eventToBeRecordedName = CL.EA.GetEventInfo(eventToBeRecorded, EnumEventInfo.EventName);
            if (String.IsNullOrEmpty(eventToBeRecordedName))
            {
                FailStep(CL, "Failed to fetch the name of the event to be recorded");
            }

            LogCommentInfo(CL, "The Event recorded(body) name is" + eventToBeRecordedName);
            //Wait until recording is complete
            res = CL.EA.WaitUntilEventEnds(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until the end of event - " + eventToBeRecordedName);
            }

            //Launch the channel banner to fetch the current event name
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch channel banner!");
            }

            //Fetch the name of the EGT event and return to live
            CL.EA.UI.Banner.GetEventName(ref egtEventName);
            LogCommentInfo(CL, "The EGT Event name is - " + egtEventName);
            if (String.IsNullOrEmpty(egtEventName))
            {
                FailStep(CL, "Failed to get the event name of the EGT event.");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to dismiss channel banner.");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step 1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Starting Stopwatch to calculate the Time Elapsed to Playback from SGT,fetch evtname.
            sw = System.Diagnostics.Stopwatch.StartNew();
            context = "SGT";
            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.secsToPlay, Constants.playbackFromBeginning, Constants.verifyEofOnPlayback);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recorded content");
            }
            LogCommentInfo(CL, "Playback started succesfully!");


            if (!helper.VerifyEventNameatPlayback(sgtEventName,context))
            {
                FailStep(CL, "Mismatch in sgtEventName fetched during recording and in Playback");
            } 

            PassStep();
        }
    }

    #endregion Step 1

    #region Step 2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Stopping Stopwatch
            sw.Stop();
            context = "EVTBody";
            LogCommentImportant(CL, "TimeElapsedInSec is" + sw.Elapsed.TotalSeconds);
            //Calculate the time to fast forward to event body
            double timeToFFTillEventBody = (((sgtValue * 60) - (sw.Elapsed.TotalSeconds)) / fwdTrickMode);
            LogCommentInfo(CL, "The time in seconds to wait in fast forward till event body - " + timeToFFTillEventBody);

            //Set trick mode speed to fast forward to event body
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, fwdTrickMode, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set trickmode speed to - " + fwdTrickMode);
            }

            //Wait till event body is reached
            res = CL.IEX.Wait(timeToFFTillEventBody);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fast forward to event body for - " + timeToFFTillEventBody + " second.");
            }

            //Restarting Stopwatch to calculate the time needed for EA trickmodespeed to execute,fetch evtname.
            sw.Restart();
            //Resume normal playback
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, Constants.trickModeForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to switch back playback to normal speed.");
            }

            //Fetch the Evtname from the banner and compare with the recorded Evtname
            if (!helper.VerifyEventNameatPlayback(eventToBeRecordedName,context))
            {

                FailStep(CL, "Mismatch in eventBeRecordedName fetched during recording and in Playback");
            }
            PassStep();
        }
    }

    #endregion Step 2

    #region Step 3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Stopping Stopwatch
            sw.Stop();
            context = "EGT";
            LogCommentImportant(CL, "TimeElapsedInSec taken in Millisec is" + sw.Elapsed.TotalSeconds);
            //Calculate the time to fast forward to EGT
            double timeToFFTillEgt = ((int.Parse(serviceToBeRecorded.EventDuration) * 60 - (sw.Elapsed.TotalSeconds)) / fwdTrickMode);
            LogCommentInfo(CL, "The time in seconds to wait in fast forward till EGT - " + timeToFFTillEgt);

            //Set trick mode speed to fast forward to EGT
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, fwdTrickMode, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set trickmode speed to - " + fwdTrickMode);
            }

            //Wait till event body is reached

            res = CL.IEX.Wait(timeToFFTillEgt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fast forward to EGT for - " + timeToFFTillEgt + " second.");
            }
            if (rewindFlag)
            {
                sw.Restart();
            }
            //PAUSE the PLAYBACK
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded,Constants.trickModeForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to switch back playback to normal speed.");
            }

            //Fetch the name from the banner and compare with the EGT name
            sw.Stop();
            if (!helper.VerifyEventNameatPlayback(egtEventName,context))
            {

                FailStep(CL, "Mismatch in egtEventName fetched during recording and in Playback");
            }


            PassStep();
        }
    }

    #endregion Step 3

    #region Step 4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
                //Fetch the REW speed from Test INI
                String rwdTrickModeValueInStrTestINI = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TM_REW");
                if (String.IsNullOrEmpty(rwdTrickModeValueInStrTestINI))
                {
                    FailStep(CL, "Rewind Trick Value not present in Test.ini file.");
                }

               //Fetch the REW Speed List from Project INI
                String rwdTrickModeListInStrProjectINI = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_REW");
                if (String.IsNullOrEmpty(rwdTrickModeListInStrProjectINI))
                {
                    FailStep(CL, "Rewind Trick mode List not present in Test.ini file.");
                }

            //Compare the REW Value fetched from Test INI exists in REW List of Project INI.
                if (!rwdTrickModeListInStrProjectINI.Contains(rwdTrickModeValueInStrTestINI))
                {
                    FailStep(CL, "The fwdtrckmode Value fetched from Test INI does not exist in Project INI,Hence specify Valid Trickmode Value");
                }
                rewTrickMode = int.Parse(rwdTrickModeValueInStrTestINI);
                LogCommentInfo(CL, "Rewind Trick Mode  fetched from Test INI is -> " + rwdTrickModeValueInStrTestINI);
            

            
            LogCommentImportant(CL, "TimeElapsedInSec is" + (sw.Elapsed.TotalSeconds));
            //Calculate the time to Rewind to event body Adding 60 secs buffer to make sure it reaches Evt body
            double timeToRWTillEventBody = (((sw.Elapsed.TotalSeconds) + 60) / Math.Abs(rewTrickMode));
            LogCommentInfo(CL, "The time in seconds to wait in Rewind till event body - " + timeToRWTillEventBody);

            //Set trick mode speed to Rewind to event body
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, rewTrickMode, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set trickmode speed to - " + rewTrickMode);
            }

            //Wait till event body is reached
            res = CL.IEX.Wait(timeToRWTillEventBody);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to REWIND to event body for - " + timeToRWTillEventBody + " second.");
            }

            //Pause the  playback
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, Constants.trickModeForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to switch back playback to normal speed.");
            }

            //Fetch the name from the banner and compare with  the event which is getting recorded
            if (!helper.VerifyEventNameatPlayback(eventToBeRecordedName,context))
            {

                FailStep(CL, res, "Mismatch in EventName(body) fetched during recording and in Playback");
            }
            PassStep();
        }
    }
    #endregion Step 4
    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Stop playback
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to stop playback.");
        }

        //Delete all recordings in planner
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete the first recorded event  from archive!");
        }
       

        //Fetch the minimum SGT and EGT default
        String defSgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (String.IsNullOrEmpty(defSgtValueInStr))
        {
            LogCommentFailure(CL, "Default SGT value not present in Project.ini file.");
            
        }
      
        LogCommentInfo(CL, "Default SGT value in minutes - " + defSgtValueInStr);
        res = CL.EA.STBSettings.SetGuardTime(true, defSgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to set Start Guide time - " + defSgtValueInStr);
        }

        String defEgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (String.IsNullOrEmpty(defEgtValueInStr))
        {
            LogCommentFailure(CL,"Default EGT value not present in Project.ini file.");
         
        }
        LogCommentInfo(CL, "Default EGT value in minutes - " + defEgtValueInStr);


        res = CL.EA.STBSettings.SetGuardTime(false, defEgtValueInStr);
        if (!res.CommandSucceeded)
        {
           LogCommentFailure(CL,"Failed to set End Guide time in minutes - " + defEgtValueInStr);
        }
       
    }

    #endregion PostExecute

    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        public bool VerifyEventNameatPlayback(string recordingEventName,string playbackcontext)
        {
            
            bool isPass = true;

            String EventNameInPlayback = "";
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to launch banner on playback! context:" + playbackcontext);
                isPass = false;
            }
            CL.EA.UI.Banner.GetEventName(ref EventNameInPlayback);
            if (String.IsNullOrEmpty(EventNameInPlayback))
            {
                LogCommentFailure(CL, "Failed to get Event name during Playback" + "context:" + playbackcontext);
                isPass = false;
            }
            else
            {
                LogCommentInfo(CL, "The  event name fetched from playback in context:" + playbackcontext + "is" + EventNameInPlayback);
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
              
                LogCommentFailure(CL, "Failed to dispose banner to go back to playback!" + "context:" + playbackcontext);
                isPass=false;
            }

            //Compare the fetched name during recording from banner with fetched name from playback
            if (String.Compare(recordingEventName,EventNameInPlayback) != 0)
            {

                LogCommentFailure(CL, "Mismatch in event name,context: " + playbackcontext + "Eventname received during recording:" + recordingEventName + "Event name received  during playback:" + EventNameInPlayback);
                isPass = false; ;
            }
            return isPass;
        }
    }
    #endregion
}