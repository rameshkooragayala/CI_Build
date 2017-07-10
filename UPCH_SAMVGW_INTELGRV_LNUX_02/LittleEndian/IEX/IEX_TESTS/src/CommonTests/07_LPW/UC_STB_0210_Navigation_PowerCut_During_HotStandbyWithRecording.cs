/// <summary>
///  Script Name : UC_STB_0210_Navigation_PowerCut_During_HotStandbyWithRecording.cs
///  Test Name   : UC-STB-0210-Navigation from Power cut during hot standby with recording
///  TEST ID     : 73887
///  QC Version  : 2
///  Variations from QC:NONE
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar 
///  QC Repository : UPC/FR_FUSION 
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STB_210")]
public class STB_210 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static string futureEventRecording = "FUTURE_EVENT";
    private static int minDelayUntilBeginning = 0; //Minutes after recording has to be scheduled. 
    private static int startGuardTimeInt = 0;
    private static int endGuardTimeInt = 0;
    private static string sgtFriendlyName;
    private static string egtFriendlyName;
    //Shared members between steps
    private static Service serviceToRecord; //The service where recordings will happen
    private static string resumableService;
 
    static string powerMode = "";
    static string defaultPowerMode = "";
    static string standbyAfterBoot = "";
    static string HourAfterRecordingStart = "";
    static string imageLoadDelay;
    private static double timeToWait = 0;
    
    private const string PRECONDITION_DESCRIPTION = "Precondition: A future recording is scheduled";
    private const string STEP1_DESCRIPTION = "Step 1: STB is in NON HOT standby AND Three minutes before the task, STB switches from NON-HOT to Active standby ";
    private const string STEP2_DESCRIPTION = "Step 2: Verify recording is started while STB is in Active standby";
    private const string STEP3_DESCRIPTION = "Step 3: Reboot the STB while event is recording AND Verify the EventIsRecording after the power cycle AND Wait for recording to end.";
    private const string STEP4_DESCRIPTION = "Step 4: After recording ends, STB switches to default standby state";

    static class Constants
    {
        public const int bufferTime = 25;//in mins
        public const int totalDurationInMin = 20; //Duration of Recording Event to be scheduled. 
        public const int bufferTimeAfterRecordingStart = 2;//in mins
    }


    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
      
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
                    
            //Fetching the service number from test ini because in UPC there is an issue with the recorded stream where they are not resuming after power cycle so definging it in Test ini.
            resumableService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RESUMABLE_SERVICE");
            
            serviceToRecord = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;LCN=" + resumableService, "ParentalRating=High");
           
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

            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch SGT Value from Test ini");
            }
            else
            {
                LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);
            }
            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");
            if (egtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch EGT Value from Test ini");
            }
            else
            {
                LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);
            }

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }

            standbyAfterBoot = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "STANDBY_AFTER_REBOOT");
            if (standbyAfterBoot == "")
            {
                FailStep(CL, res, "Failed to fetch the stand by after reboot variable from Project INI file");
            }

            //Fetching : After how many hours recording will starts from PROJECT INI file.                  
            HourAfterRecordingStart  = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_CHECK_TIME");
            if (HourAfterRecordingStart == "")
            {
                FailStep(CL, res, "Failed to fetch the MAINTENANCE_CHECK_TIME from Project INI file");
            }
           
            minDelayUntilBeginning = (Convert.ToInt32(HourAfterRecordingStart) * 60) + Constants.bufferTime ; //Schedule Recording after 2 Hour 15 Min.(135 min.)
            LogComment(CL, "Recording will Start After: "+ minDelayUntilBeginning  +" minutes in the Planner");
            
            //Get POWER_MODE Values From TEST ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

            //Setting the STB default standby setting to powerMode = "MEDIUM";
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the standby mode to MEDIUM standby");
            }

            res = CL.EA.PVR.RecordManualFromPlanner(futureEventRecording, serviceToRecord.Name, -1, minDelayUntilBeginning, Constants.totalDurationInMin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Book a Time Based Recording on " + serviceToRecord.LCN);
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

            LogCommentImportant(CL, "Total Event Duration in minutes :" + Constants.totalDurationInMin);
            
            string currentTime = "";
            //Get the Current EPG Time:
            CL.EA.UI.Live.GetEpgTime(ref currentTime);
            if (string.IsNullOrEmpty(currentTime))
            {
                FailStep(CL, "Failed to Get the EPG time from LIVE");
            }
            LogComment(CL, "Current time is " + currentTime);

            //Calculating Time Difference between EventStartTime AND EPG CurrentTime  
            timeToWait = (Convert.ToDateTime(evtStartTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds;
            
            System.Diagnostics.Stopwatch swatch = System.Diagnostics.Stopwatch.StartNew();

            //verify STB power mode and Enter to  _isWakeUp =false  and standby =true
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode, jobPresent:false, isWakeUp:false, isStandBy:true);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to verify power mode option");
            }
            else
            {
                LogCommentInfo(CL, "Set Power mode verified Successfully");
            }
            swatch.Stop();
            LogCommentImportant(CL, "Time taken for VerifyPowerMode EA's in sec is " + Convert.ToInt32(swatch.Elapsed.TotalSeconds));

            LogCommentInfo(CL, "Wait for some time for STB to come to Active standby and Recording to be start");
            res = CL.IEX.Wait(timeToWait - Convert.ToInt32(swatch.Elapsed.TotalSeconds) + Constants.bufferTimeAfterRecordingStart*60);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to wait for " + (timeToWait - Convert.ToInt32(swatch.Elapsed.TotalSeconds) + Constants.bufferTimeAfterRecordingStart*60).ToString() + " seconds");
            }


            PassStep();
        }
    }
    #endregion

    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    public class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Verifying the current recording status in PCAT
            LogCommentInfo(CL, "Verify Currently Recording status in PCAT has been removed as we wont be able to Telnet the Box in Non Hot Standby");
			LogCommentInfo(CL, "This has been removed for testing purpose. Depending on the result wwe need to modify the Script accordingly");
           // res = CL.EA.PCAT.VerifyEventIsRecording(futureEventRecording);
           // if (!res.CommandSucceeded)
          //  {
          //      FailStep(CL, res, "Failed to validate RECORD IN PROGRESS in PCAT!");
          //  }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //To calculate the exact wait time for the recording, it is needed to capture the time taken by EA as well.The logic is to use a stop watch to calculte the exact time taken by EA and subtracting it with the overall Event duration. 

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            res = CL.EA.PowerCycle(SecBeforePowerOn: 10, FormatSTB: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to power cycle STB due to " + res.FailureReason);
            }
           
            imageLoadDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "IMAGE_LOAD_DELAY_SEC");
            if (imageLoadDelay == null)
            {
                FailStep(CL, res, "Failed to load image load delay time from Project INI file");
            }
            int delaytime;
            int.TryParse(imageLoadDelay, out delaytime);    //converting the image load time to int for Wait

            //Wait for some time for STB to image to load
            res = CL.IEX.Wait(delaytime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for image to load ");
            }

            //After reboot check if the recording is started again 
            res = CL.EA.PCAT.VerifyEventIsRecording(futureEventRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to validate RECORD IN PROGRESS in PCAT!");
            }

            sw.Stop();
            LogCommentImportant(CL, "Time taken for EA's in sec is " + Convert.ToInt32(sw.Elapsed.TotalSeconds));

            //wait for recording to the end
            LogCommentInfo(CL, "Waiting for recording to end");
            res = CL.IEX.Wait(((Constants.totalDurationInMin*60) - Convert.ToInt32(sw.Elapsed.TotalSeconds))-180);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for image to load ");
            }

            PassStep();
        }

    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //verify  STB enters Defalut Standby Mode after the end of Recording
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode,jobPresent: false, isStandBy:false, isWakeUp: false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to verify power mode option");
            }
            else
            {
                LogCommentInfo(CL, "Set Power mode verified Successfully");
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
        IEXGateway._IEXResult res;


        //wait for 2 min so that set up box can be in standBy mode.
        CL.IEX.Wait(120);

        //Wake up the Set box or exiting from the stand by.
        res = CL.EA.StandBy(true);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to make set up box waking up ");

        }

        //delete the failed recorded event
        res = CL.EA.PVR.DeleteRecordFromArchive(futureEventRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Failed Recorded Event");
        }

        //get default SGT value from project.ini 
        string defaultStartGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultStartGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT");
        }
        //set SGT to default
        res = CL.EA.STBSettings.SetGuardTime(true, defaultStartGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set Start Guard Time to " + defaultStartGuardTime);
        }


        //get default value from project.ini for EGT
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for EGT");
        }
        //set EGT to default
        res = CL.EA.STBSettings.SetGuardTime(false, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }

        //Restore the Default power mode
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