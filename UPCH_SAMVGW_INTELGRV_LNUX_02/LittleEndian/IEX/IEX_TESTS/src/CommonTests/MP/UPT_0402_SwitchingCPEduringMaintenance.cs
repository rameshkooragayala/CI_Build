/// <summary>
///  Script Name : UPT_0402_SwitchingCPEduringMaintenance.cs
///  Test Name   :UPT-0402-CPE device in active standby-postponed-Resource-Unavailable  
///  TEST ID     :67120
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar
/// </summary>
/// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;


public class UPT_0402 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static string futureEventRecording = "FUTURE_EVENT";
    private static int minDelayUntilBeginning = 0; //Minutes after recording has to be scheduled. 
    static string defaultMaintenanceDelay = "";
    //Shared members between steps
    private static Service serviceToRecord; //The service where recordings will happen
    static string powerMode = "";
    static string defaultPowerMode = "";
    static string HourAfterRecordingStart = "";
    static string resumableService = "";
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File and Set Power Mode to hot stand";
    private const string STEP1_DESCRIPTION = "Step 1: Schedule a recording R1 that starts in the two hours following the start of maintenance phase .";
    private const string STEP2_DESCRIPTION = "Step 2: Switch the box to stand By and wait for default maintenance delay ,after this delay maintenance phase should not start!!";
    private const string STEP3_DESCRIPTION = "Step 3: Verify the Event is recording and maintenance phase should starts and completes once the recording is done.";
    private static double RecordingWillStartAfter_Wait = 0;
    private static double RecordingWillEndAfter_Wait = 0;
    private static double RecordingWaitUntilEventStarts = 0;
    private static double RecordingWaitstandby = 0;

    private static class Constants
    {
        public const int totalDurationInMin = 10; //Duration of Recording Event to be scheduled. 
        public const int bufferTimeAfterRecordingEnds = 60;//in seconds
        public const int waitForLive = 20; //in seconds
        public const int defaultMaintenanceCompleteDelayInt = 300; //in seconds
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

            resumableService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RESUMABLE_SERVICE");
            if (string.IsNullOrEmpty(resumableService))
            {
                FailStep(CL, "Failed to get resumable Service test ini file");
            }

            //Fetching the recordable service from content xml.
            serviceToRecord = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True;LCN=" + resumableService, "ParentalRating=High");
            if (serviceToRecord == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }

            //Get default mode from PROJECT ini file
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Fetching : After how many hours recording will starts from PROJECT INI file.                  
            HourAfterRecordingStart = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_CHECK_TIME");
            if (HourAfterRecordingStart == "")
            {
                FailStep(CL, res, "Failed to fetch the MAINTENANCE_CHECK_TIME from Project INI file");
            }

            //Get POWER_MODE Values From TEST ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

            //Get default maintenance delay from project ini
            defaultMaintenanceDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_DEALY");
            if (string.IsNullOrEmpty(defaultMaintenanceDelay))
            {
                FailStep(CL, res, "Unable to fetch the default maintenance delay value from project ini");
            }

            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the standby mode to MEDIUM standby");
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

            //Schedule Recording within 2 Hour after the standby delay of 10 mins , so the maintanance will not starts.  
            minDelayUntilBeginning = ((Convert.ToInt32(HourAfterRecordingStart) * 60) / 6); //
            LogComment(CL, "Recording will Start After: " + minDelayUntilBeginning + " minutes in the Planner");

            res = CL.EA.PVR.RecordManualFromPlanner(futureEventRecording, serviceToRecord.Name, -1, minDelayUntilBeginning, Constants.totalDurationInMin, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Book a Time Based Recording on " + serviceToRecord.LCN);
            }

            //Refreeshing the EPG to get the Current time from EPG
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to main menu", exitTest: false);
            }

            //Getting Event info from the event collection
            string evtStartTime = CL.EA.GetEventInfo(futureEventRecording, EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event Start time is " + evtStartTime);

            string evtEndTime = CL.EA.GetEventInfo(futureEventRecording, EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event End time is " + evtEndTime);

            string currentTime = "";
            //Get the Current EPG Time:
            CL.EA.UI.Live.GetEpgTime(ref currentTime);
            if (string.IsNullOrEmpty(currentTime))
            {
                FailStep(CL, "Failed to Get the EPG time from LIVE");
            }
            LogComment(CL, "Current time is " + currentTime);

            //Calculating Time Difference between RecordingEventStartTime AND EPG CurrentTime  

            RecordingWillStartAfter_Wait = (Convert.ToDateTime(evtStartTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds;

            //Calculating Time Difference between RecordingEventEndTime AND EPG CurrentTime  
            RecordingWillEndAfter_Wait = (Convert.ToDateTime(evtEndTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds;


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
            System.Diagnostics.Stopwatch swatch = System.Diagnostics.Stopwatch.StartNew();

            //Put CPE Device in standby

            LogCommentWarning(CL, "CPE should put off the A/V and should switch to standby.");
            res = CL.EA.StandBy(false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to enter standby");
            }

            //wait for maintenance to start
            LogCommentWarning(CL, "Wait for maintenance to start after 10 mins in hot standby.");
            int defaultMaintenanceDelayInt = Convert.ToInt32(defaultMaintenanceDelay);
            defaultMaintenanceDelayInt = defaultMaintenanceDelayInt * 60;

            //Fetch the maintenaceStart milestone from milestones.ini
            String maintenanceStartMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceStart");

            //Begin wait for maintenaceStart milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceStartMilestone, defaultMaintenanceDelayInt);

            ArrayList arrayList = new ArrayList();
            bool isMaintenanceMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceStartMilestone, ref arrayList);
            if (isMaintenanceMilestoneRecieved)
            {
                FailStep(CL, res, " Mainteannace started event if there is a recording schedule within 2 hours.");
            }
            else
            {
                LogCommentWarning(CL, "Maintenance Not started because there is a recording schedule within 2 hours.");
            }
            swatch.Stop();

            LogCommentInfo(CL, "Wait for event to start recording");

            RecordingWaitstandby = Convert.ToInt32(swatch.Elapsed.TotalSeconds);
            RecordingWaitUntilEventStarts = RecordingWillStartAfter_Wait - RecordingWaitstandby;

            LogCommentWarning(CL, "Wait until the Recording Event starts");
            res = CL.IEX.Wait(RecordingWaitUntilEventStarts);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until the Recording Event starts !");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            System.Diagnostics.Stopwatch VerifyEventIsRecordingWatch = System.Diagnostics.Stopwatch.StartNew();

            //Verifying the current recording status in PCAT
            LogCommentInfo(CL, "Verify Currently Recording status in PCAT");

            res = CL.EA.PCAT.VerifyEventIsRecording(futureEventRecording);
            if (!res.CommandSucceeded)
            {
               FailStep(CL, res, "Failed to validate RECORD IN PROGRESS in PCAT!");
            }

            VerifyEventIsRecordingWatch.Stop();

            //Wait for recording to end
            LogCommentInfo(CL, "Waiting until the recording event ends");
            res = CL.IEX.Wait(RecordingWillEndAfter_Wait - (RecordingWillStartAfter_Wait) - Convert.ToInt32(VerifyEventIsRecordingWatch.Elapsed.TotalSeconds) - Constants.bufferTimeAfterRecordingEnds);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until the recording event ends!");
            }

            LogCommentWarning(CL, "Now the Maintenance should start and completes once the Recording event Ends.");

            //Fetch the MaintenanceComplete milestone from milestones.ini
            String MaintenanceComplete = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceComplete");
            
            //Begin wait for MaintenanceComplete milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(MaintenanceComplete, Constants.defaultMaintenanceCompleteDelayInt);

            ArrayList arrayList = new ArrayList();
            bool isMaintenanceMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(MaintenanceComplete, ref arrayList);
            if (!isMaintenanceMilestoneRecieved)
            {
                FailStep(CL, res, " Failed to complete the Maintenance after the recording ends.");
            }
            else
            {
                LogCommentWarning(CL, "Maintenance completed after the recording ends.");
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

        //Wake up the box during maintenance
        res = CL.EA.StandBy(true);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to exit from standby");
        }

        //Verify for live after wake up
        bool isLive = CL.EA.UI.Utils.VerifyState("LIVE", Constants.waitForLive);
        if (!isLive)
        {
            LogCommentFailure(CL, "Failed to verify live state");
        }

        //Restore the power mode
        res = CL.EA.STBSettings.SetPowerMode(defaultPowerMode);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default POWER MODE");
        }
        else
        {
            LogCommentInfo(CL, "Restored to default POWER MODE SUCCESSFULLY");
        }

    }
    #endregion
}




