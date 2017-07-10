/// <summary>
///  Script Name : SET_0510_SGT.cs
///  Test Name   : SET-0510-REC SGT-0 Min
///  Test Name   : SET-0510-REC SGT-1 Min
///  Test Name   : SET-0510-REC SGT-2 Min
///  Test Name   : SET-0510-REC SGT-3 Min
///  Test Name   : SET-0510-REC SGT-5 Min
///  Test Name   : SET-0510-REC SGT-10 Min
///  Test Name   : SET-0510-REC SGT-15 Min
///  Test Name   : SET-0510-REC SGT-30 Min
///  TEST ID     : 
///  QC Version  : 
/// -----------------------------------------------
///  Modified by : Shruthi H M
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class SET_SGT_EGT : _Test
{

    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static string startGuardTime;
    static string endGuardTime;
    static string startGuardTimeDefault;
    static string endGuardTimeDefault;
    static int recordedEventTime = 0;
    static Service serviceToBeRecorded; //The service to be recorded
    static string eventToBeRecorded = "EVENT_TO_BE_RECORDED";
    static string eventDurationVerificationThreshold;

    static class Constants
    {
        public const int plbDuration = 30; // In seconds
        public const int recordBufferTime = 2; // In minutes
        public const int timeToWaitInStandby = 5; //in sec
        public const int minEventDuration = 3; // In Minutes
        public const int numOfPressesForNextEvent = 1;
        public const int minTimeBeforeEventEnd = 2; // In minutes
    }

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Set Start Guard Time and End Guard Time values from Test ini");
        this.AddStep(new Step2(), "Step 2: Record an event from Banner/Guide and verify event duration");
        this.AddStep(new Step3(), "Step 3: Verify event playback.");

        //Get Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            startGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");
            endGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");
            startGuardTimeDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
            endGuardTimeDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");

            eventDurationVerificationThreshold = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "EVENT_DURATION_VERIFICATION_THRESHOLD");
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            // Set SGT 
            res = CL.EA.STBSettings.SetGuardTime(true, startGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set SGT to - " + startGuardTime);
            }

            // Set EGT 
            res = CL.EA.STBSettings.SetGuardTime(false, endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set EGT to - " + endGuardTime);
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Get integer values for the passed friendly names for guard time
            int endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(endGuardTime);
            int startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(startGuardTime);

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

            // Surf to the channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded.LCN);
            }

            // Handle Event Edge
            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }

            if (timeToEventEnd_sec < Constants.minTimeBeforeEventEnd*60)
            {
                CL.EA.ReturnToLiveViewing();
                LogCommentInfo(CL,"Waiting for "+timeToEventEnd_sec+" for event end.");
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            if (startGuardTimeInt == 0)
            {
                // Record current event from banner
                res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minEventDuration, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book current event from banner");
                }

                CL.EA.WaitUntilEventEnds(eventToBeRecorded);

                // Wait till EGT is elapsed
                LogCommentInfo(CL, "Waiting " + (endGuardTimeInt * 60) + " seconds for EGT to elapse");
                CL.IEX.Wait(endGuardTimeInt * 60 + 2 * 60);
            }
            else
            {
                // Book future event if SGT is <> 0
                res = CL.EA.PVR.BookFutureEventFromGuide(eventToBeRecorded, serviceToBeRecorded.LCN, Constants.numOfPressesForNextEvent, startGuardTimeInt, false, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book the Event From Guide");
                }

                CL.EA.WaitUntilEventEnds(eventToBeRecorded);

                LogCommentInfo(CL, "Waiting " + (endGuardTimeInt * 60) + " seconds for EGT to elapse");
                CL.IEX.Wait(endGuardTimeInt * 60 + 2 * 60);

            }

			recordedEventTime = int.Parse(CL.EA.GetEventInfo(eventToBeRecorded, EnumEventInfo.EventDuration));
            // Verfify that event duration is long enough
            res = CL.EA.VerifyEventDuration(eventToBeRecorded, (startGuardTimeInt * 60) + recordedEventTime + (endGuardTimeInt * 60) - 21, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Long Enough");
            }

            // Verify that event duration is not too long
            res = CL.EA.VerifyEventDuration(eventToBeRecorded, (startGuardTimeInt * 60) + recordedEventTime + (endGuardTimeInt * 60) + int.Parse(eventDurationVerificationThreshold), false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Not Too Long");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.plbDuration, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Event From Archive");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        //Revert GT values to default
        IEXGateway._IEXResult res;

        // Set SGT value to default
        res = CL.EA.STBSettings.SetGuardTime(true, startGuardTimeDefault);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set SGT to default - " + startGuardTimeDefault);
        }

        // Set EGT value to default
        res = CL.EA.STBSettings.SetGuardTime(false, endGuardTimeDefault);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set EGT to default - " + endGuardTimeDefault);
        }

        //Delete all recordings in planner
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }


    }
    #endregion
}