/// <summary>
///  Script Name : LPW_SwitchFromPowerOffToStandBy.cs
///  Test Name   : UC-STB-0500-Navigation to passive standby from power off,UC-STB-001-Switchfrom power
///  TEST ID     : 73944,73945
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Madhu Renukaradhya
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("LPW_SwitchFromPowerOffToStandBy")]
public class LPW_SwitchFromPowerOffToStandBy : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
   
    //Variables which are used in different steps
    private static string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded
    private static string resumableService;
    static string powerMode = "";
    static string defaultPowerMode = "";
    static string period = "";
    static string defaultPeriod = "";
    static string standyBy = "";
    static string imageLoadDelay = "";
    static int delaytime = 0 ;

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch Service from Content XML,Power modes";
    private const string STEP1_DESCRIPTION = "Step 1: Record an Ongoing Event ";
    private const string STEP2_DESCRIPTION = "Step 2: Switch the box to standby and perform power cycle ";
    private const string STEP3_DESCRIPTION = "Step 3: Verify after power cycle the box stays in standby,completes recording and moves to default standby ";
    
    private static class Constants
    {
        
        public const int totalEventDuration = 15; //Duration of Recording Event to be scheduled. 
        public const int waitForMilestones = 600;
        public const int waitForLive = 20;
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

            //Get Values From ini File
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True;LCN=" + resumableService, "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch recordableService" + recordableService.LCN + "from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "RecordableService fetched from content xml is : " + recordableService.LCN);
            }


            //Get power mode Value to be set From test ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

            //Get default power mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini", true);
            }

            //Get period to be set From test ini File
            period = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PERIOD");
            if (string.IsNullOrEmpty(period))
            {
                FailStep(CL, res, "Unable to fetch the period from test ini file");
            }

            //Get default period from project ini
            defaultPeriod = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_PERIOD");
            if (string.IsNullOrEmpty(defaultPeriod))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Get standby value to be set from test ini File
            standyBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STANDBY");
            if (string.IsNullOrEmpty(standyBy))
            {
                FailStep(CL, res, "Unable to fetch the standBy value from test ini file");
            }

            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully");
            }

            //set default to any available period
            res = CL.EA.STBSettings.ActivateAutoStandByAfterTime(period);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + period);
            }
            else
            {
                LogCommentInfo(CL, "Default Period for Stand By set successfully to: " + period);
            }

            //set default to any available standBy
            res = CL.EA.STBSettings.SetAutoStandBy(standyBy);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + standyBy);
            }
            else
            {
                LogCommentInfo(CL, "Default StandBy set successfully to: " + standyBy);
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

            res = CL.EA.PVR.RecordManualFromPlanner(eventToBeRecorded, recordableService.Name, DaysDelay: -1, DurationInMin: Constants.totalEventDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Book a Time Based Recording on " + recordableService.LCN);
            }
            
            //wait till recording starts
            res = CL.EA.WaitUntilEventStarts(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait untill event starts");
            }

            //After reboot check if the recording is started again 
            res = CL.EA.PCAT.VerifyEventIsRecording(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to validate RECORD IN PROGRESS in PCAT!");
            }
          
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

           //Switch the box to stand by
            res = CL.EA.StandBy(false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to enter standby");
            }

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
           
            int.TryParse(imageLoadDelay, out delaytime);//converting the image load time to int for Wait

            //Wait for some time for STB to image to load
            res = CL.IEX.Wait(delaytime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for image to load ");
            }

            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages("IEX_STATE_RECORDED ->event BOOKING_END", 900);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage for recording completed milestones");
            }

            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages("IEX_STATE_RECORDED ->event BOOKING_END", ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get recording completed milestones");
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
            
           //Verify fasShutdownmilestone from milestones ini
            if (powerMode == "HOT STANDBY")
            {
                res = CL.IEX.Wait(600);
                if (!(res.CommandSucceeded))
                {
                    FailStep(CL, res, "Failed during waiting for default hot stand by");
                }

                //Wake up the box
                res = CL.EA.StandBy(true);
                if (!(res.CommandSucceeded))
                {
                    FailStep(CL, res, "Failed to come out of standby");
                }

                //Verify for live after wake up
                bool isLive = CL.EA.UI.Utils.VerifyState("LIVE", Constants.waitForLive);
                if (!isLive)
                {
                    FailStep(CL, "Failed to verify live state after wake up from stand by during maintenance");
                }


            }
            else
            {
                bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages("IEX_SHUTDOWN_FAS", Constants.waitForMilestones);
                if (!verifyMilestones)
                {
                    FailStep(CL, res, "Failed to BeginWaitForMessage for IEX_SHUTDOWN_FAS milestones");
                }

                System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
                bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages("IEX_SHUTDOWN_FAS", ref ActualLines);
                if (!endVerifyMilestones)
                {
                    FailStep(CL, res, "Failed to get IEX_SHUTDOWN_FAS milestones.Box unable to go to default stand by");
                }

                CL.IEX.Wait(120);
                //Out of Stand by
                //Thus without throwing the exception the process is continued undercatch block


                res =   CL.EA.StandBy(true);
                if(!(res.CommandSucceeded))
                {

                    //Mount the box
                    CL.IEX.Wait(180);

                    //Get default power mode from project ini
                    Boolean standbyAfterBoot = Boolean.Parse(CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "STANDBY_AFTER_REBOOT"));
                    Boolean isHomeNetwork = Boolean.Parse(CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "IsHomeNetwork"));
                   

                    //if the box supports home network mount client

                    if (isHomeNetwork)
                    {
                        CL.EA.MountClient(EnumMountAs.NOFORMAT);

                    }
                    //if the box does not support home network mount only gateway
                    else
                    {
                        CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);

                        //Wait for some time for STB to come to standby mode 
                        CL.IEX.Wait(delaytime);
                    }
                    //Verify for live after wake up
                    bool isLive = CL.EA.UI.Utils.VerifyState("LIVE", Constants.waitForLive);
                    if (!isLive)
                    {
                        FailStep(CL, "Failed to verify live state after wake up from stand by during maintenance");
                    }
                }
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

        //Restore the period
        res = CL.EA.STBSettings.ActivateAutoStandByAfterTime(defaultPeriod);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default PERIOD");
        }
        else
        {
            LogCommentInfo(CL, "Restored to default PERIOD SUCCESSFULLY");
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

        //Delete the recording
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Event based recording from Archive");
        }
        //Fetch the SGT and EGT default values
        String defSgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (String.IsNullOrEmpty(defSgtValueInStr))
        {
            LogCommentFailure(CL, "Default SGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default SGT value in minutes - " + defSgtValueInStr);

        String defEgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (String.IsNullOrEmpty(defEgtValueInStr))
        {
            LogCommentFailure(CL, "Default EGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default EGT value in minutes - " + defEgtValueInStr);

        //Set SGT & EGT to default
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: defSgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set Start Guide time - " + defSgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: defEgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set End Guide time - " + defEgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
        
    }
    #endregion
}