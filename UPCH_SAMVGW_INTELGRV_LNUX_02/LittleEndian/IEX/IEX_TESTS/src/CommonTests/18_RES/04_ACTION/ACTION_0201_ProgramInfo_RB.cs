/// <summary>
///  Script Name  : ACTION_0201_ProgramInfo_RB
///  Test Name    : ACTION-0201-Program Information-From Review Buffer
///  TEST ID      : 68018
///  JIRA ID      : FC-485
///  QC Version   : 2
/// -----------------------------------------------
///  Modified by          : SHRUTHI H M
///  Last Modified Date   : 26/07/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class ACTION_0201 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    public class Event
    {
        public String eventName = "";
        public String eventStartTime = "";
        public String eventEndTime = "";
        public int eventTimeLeft = 0;
    }

    //Variables used in the test
    private static Service s1;
    private static Service s2;
    private static int defaultTimeToCheckForVideo;  // In Seconds
    private static int medFwdTrickMode;
    private static int medRewTrickmode;
    private static Event s2E1 = new Event(); // Previous event of Service s2
    private static Event s1E1 = new Event(); // Previous event of Service s1
    private static Event s1E2 = new Event(); // Current event of Service s1
    private static Helper helper = new Helper();

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int trickModeForPlay = 1;
        public const int minTimeforEventEnd = 5; // In Minutes
        public const int timeToWaitInService = 2; // In Minutes
        public const bool checkBOF = true;
        public const bool checkEOF = false;
        public const int timeToWaitInStandby = 5; //in sec
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps of the test case
        this.AddStep(new PreCondition(), "Precondition: Previous event of Service s2 is in RB,previous and current event of Service s1 are in RB");
        this.AddStep(new Step1(), "Step 1: Playback previous event of s2 from RB");
        this.AddStep(new Step2(), "Step 2: Playback previous event of s1 from RB");
        this.AddStep(new Step3(), "Step 3: Playback current event of s1 from RB");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Fetch a service with 10 minute event duration
            s1 = CL.EA.GetServiceFromContentXML("Type=Video;IsConstantEventDuration=True;EventDuration=10", "ParentalRating=High");
            if (s1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            // Fetch another service with 10 minute event duration
            s2 = CL.EA.GetServiceFromContentXML("Type=Video;IsConstantEventDuration=True;EventDuration=10", "ParentalRating=High;LCN=" + s1.LCN);
            if (s2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            //Fetch the default time to check for video
            String defaultVideoCheck = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultVideoCheck))
            {
                FailStep(CL, "Default Video check time not present in Project.ini file.");
            }
            defaultTimeToCheckForVideo = int.Parse(defaultVideoCheck);

            //Fetch the medium REW trickmode
            String rewTrickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            if (String.IsNullOrEmpty(rewTrickModeArrayInStr))
            {
                FailStep(CL, "Rewind Trick mode list not present in Project.ini file.");
            }
            String[] rewTrickModeArray = rewTrickModeArrayInStr.Split(',');
            String rewTrickModeInStr = rewTrickModeArray[(rewTrickModeArray.Length) / 2];
            medRewTrickmode = int.Parse(rewTrickModeInStr);

            //Fetch the medium FWD trickmode
            String fwdTrickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD");
            if (String.IsNullOrEmpty(fwdTrickModeArrayInStr))
            {
                FailStep(CL, "Forward Trick mode list not present in Project.ini file.");
            }
            String[] fwdTrickModeArray = fwdTrickModeArrayInStr.Split(',');
            String medFwdTrickModeInStr = fwdTrickModeArray[(fwdTrickModeArray.Length) / 2];
            medFwdTrickMode = int.Parse(medFwdTrickModeInStr);

            //Tune to service s2
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, s2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + s2.LCN);
            }

            // Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, defaultTimeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Check for video failed on service " + s2.LCN);
            }

            // Fetch event details from action bar
            res = helper.FetchEventDetailsFromActionBar(ref s2E1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event details from action bar!");
            }

            // Wait until event end if event time left is not sufficient
            if (s2E1.eventTimeLeft < Constants.minTimeforEventEnd * 60)
            {
                LogCommentInfo(CL, "Event time left is insufficient. Waiting " + s2E1.eventTimeLeft + " seconds till event end.");
                res = CL.IEX.Wait(s2E1.eventTimeLeft);

                // Fetch event details from action bar
                res = helper.FetchEventDetailsFromActionBar(ref s2E1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to fetch event details from action bar!");
                }
            }

            //Need to flush RB
            res = helper.FlushRB();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to flush RB");
            }

            // Wait for sometime in s2
            LogCommentInfo(CL, "Waiting for " + Constants.timeToWaitInService * 60 + " seconds in service " + s2.LCN);
            res = CL.IEX.Wait(Constants.timeToWaitInService * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in current Event on service:" + s2.LCN);
            }

            //Tune to service s1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, s1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + s1.LCN);
            }

            // Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, defaultTimeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Check for video failed on service " + s1.LCN);
            }

            // Fetch event details from action bar
            res = helper.FetchEventDetailsFromActionBar(ref s1E1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event details from action bar!");
            }

            // Wait until Event End
            LogCommentInfo(CL, "Waiting " + s1E1.eventTimeLeft + " seconds till event end in service " + s1.LCN);
            res = CL.IEX.Wait(s1E1.eventTimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till event end");
            }

            // Fetch event details from action bar
            res = helper.FetchEventDetailsFromActionBar(ref s1E2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event details from action bar!");
            }

            // Wait for sometime in Current Event
            LogCommentInfo(CL, "Waiting for " + Constants.timeToWaitInService * 60 + " seconds in current event in service " + s1.LCN);
            res = CL.IEX.Wait(Constants.timeToWaitInService * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in current Event on service:" + s1.LCN);
            }

            // Rewind till BOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", medRewTrickmode, Constants.checkBOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind till BOF!");
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

            // Playback from the beginning of RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.trickModeForPlay, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback from RB");
            }

            Event s2E1Playback = new Event();

            // Fetch event details from action bar
            res = helper.FetchEventDetailsFromActionBar(ref s2E1Playback);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event details from action bar!");
            }

            // Validate the event details in playback with the live event
            bool result = helper.ValidateEventDetailsInPlayback(s2E1, s2E1Playback);
            if (!result)
            {
                FailStep(CL, res, "Playback event validation failed");
            }
            else
            {
                LogCommentInfo(CL, "Event details from RB playback matched event name from Live");
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

            // Calculate the time to fast forward to RB event in s1
            double timeToFFTillNextRBEvent = ((Constants.timeToWaitInService * 60) / medFwdTrickMode);

            // Set trick mode speed to fast forward
            res = CL.EA.PVR.SetTrickModeSpeed("RB", medFwdTrickMode, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set trickmode speed to - " + medFwdTrickMode);
            }

            // Wait till next event in RB is reached
            LogCommentInfo(CL, "Waiting " + timeToFFTillNextRBEvent + " seconds for next event in RB.");
            res = CL.IEX.Wait(timeToFFTillNextRBEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fast forward to next event in RB for  - " + timeToFFTillNextRBEvent + " second.");
            }

            // Resume normal playback
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.trickModeForPlay, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to switch back playback to normal speed.");
            }

            Event s1E1Playback = new Event();

            // Fetch event details from action bar
            res = helper.FetchEventDetailsFromActionBar(ref s1E1Playback);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event details from action bar!");
            }

            // Validate the event details in playback with the live event
            bool result = helper.ValidateEventDetailsInPlayback(s1E1, s1E1Playback);
            if (!result)
            {
                FailStep(CL, res, "Playback event validation failed");
            }
            else
            {
                LogCommentInfo(CL, "Event details from RB playback matched event name from Live");
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

            // Calculate the time to fast forward to current event in RB in s1
            double timeToFFTillNextRBEvent = (s1E1.eventTimeLeft / medFwdTrickMode);

            //Set trick mode speed to fast forward
            res = CL.EA.PVR.SetTrickModeSpeed("RB", medFwdTrickMode, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set trickmode speed to - " + medFwdTrickMode);
            }

            // Wait till next event in RB is reached
            LogCommentInfo(CL, "Waiting " + timeToFFTillNextRBEvent + " seconds for next event in RB.");
            res = CL.IEX.Wait(timeToFFTillNextRBEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fast forward to next event in RB for  - " + timeToFFTillNextRBEvent + " second.");
            }

            //Resume normal playback
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.trickModeForPlay, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to switch back playback to normal speed.");
            }

            Event s1E2Playback = new Event();

            // Fetch event details from action bar
            res = helper.FetchEventDetailsFromActionBar(ref s1E2Playback);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event details from action bar!");
            }

            // Validate the event details in playback with the live event
            bool result = helper.ValidateEventDetailsInPlayback(s1E2, s1E2Playback);
            if (!result)
            {
                FailStep(CL, res, "Playback event validation failed");
            }
            else
            {
                LogCommentInfo(CL, "Event details from RB playback matched event name from Live");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Helper

    public class Helper : _Step
    {
        public override void Execute()
        {
        }

        public IEXGateway._IEXResult FlushRB()
        {
            IEXGateway._IEXResult res;
            //Entering Stand-By
            LogCommentInfo(CL, "Entering Standby to flush RB");
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Enter StandBy. Cannot Flush the RB");
            }
            else
            {
                LogCommentInfo(CL, "Waiting " + Constants.timeToWaitInStandby + " seconds in Standby");
                CL.IEX.Wait(Constants.timeToWaitInStandby);     //Wait for 5 sec in StandBy

                //Exiting from StandBy
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    CL.IEX.FailStep("Failed to Exit from StandBy");
                }
            }
            return res;
        }

        public IEXGateway._IEXResult FetchEventDetailsFromActionBar(ref Event eventObj)
        {
            IEXGateway._IEXResult res;
            // Clear EPG Info before launching action bar to fetch event details
            if (!CL.IEX.MilestonesEPG.ClearEPGInfo().CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to clear EPG Info");
            }

            //Launch the action bar
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to launch action bar");
            }

            //Fetch the event name
            CL.EA.UI.Banner.GetEventName(ref eventObj.eventName);
            if (String.IsNullOrEmpty(eventObj.eventName))
            {
                CL.IEX.FailStep("Failed to get the event name");
            }

            // Fetch Event Start Time
            CL.EA.UI.Banner.GetEventStartTime(ref eventObj.eventStartTime);
            if (String.IsNullOrEmpty(eventObj.eventStartTime))
            {
                CL.IEX.FailStep("Failed to get the event start time");
            }

            // Fetch Event End Time
            CL.EA.UI.Banner.GetEventEndTime(ref eventObj.eventEndTime);
            if (String.IsNullOrEmpty(eventObj.eventEndTime))
            {
                CL.IEX.FailStep("Failed to get the event end time");
            }

            // Fetch Event Time Left
            CL.EA.UI.Banner.GetEventTimeLeft(ref eventObj.eventTimeLeft);
            if (eventObj.eventTimeLeft == 0)
            {
                CL.IEX.FailStep("Failed to get event time left");
            }

            // Dismiss Action Bar
            res = CL.IEX.MilestonesEPG.Navigate("LIVE");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to dismiss action bar.");
            }
            return res;
        }

        public bool ValidateEventDetailsInPlayback(Event liveEvent, Event playbackEvent)
        {
            bool result = true;
            //Compare the fetched name from Live to fetched name from playback
            if (String.Compare(liveEvent.eventName, playbackEvent.eventName) != 0)
            {
                CL.IEX.FailStep("Mismatch in event name fetched from Live - " + liveEvent.eventName + " and during playback - " + playbackEvent.eventName);
                result = false;
            }

            //Compare the fetched start time from Live to fetched time from playback
            if (String.Compare(liveEvent.eventStartTime, playbackEvent.eventStartTime) != 0)
            {
                CL.IEX.FailStep("Mismatch in event start time fetched from Live - " + liveEvent.eventStartTime + " and during playback - " + playbackEvent.eventStartTime);
                result = false;
            }

            //Compare the fetched end time from Live to fetched time from playback
            if (String.Compare(liveEvent.eventEndTime, playbackEvent.eventEndTime) != 0)
            {
                CL.IEX.FailStep("Mismatch in event start time fetched from Live - " + liveEvent.eventEndTime + " and during playback - " + playbackEvent.eventEndTime);
                result = false;
            }

            return result;
        }
    }

    #endregion Helper

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Return to Live Viewing
        res = CL.EA.ReturnToLiveViewing();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to return to Live!");
        }
    }

    #endregion PostExecute
}