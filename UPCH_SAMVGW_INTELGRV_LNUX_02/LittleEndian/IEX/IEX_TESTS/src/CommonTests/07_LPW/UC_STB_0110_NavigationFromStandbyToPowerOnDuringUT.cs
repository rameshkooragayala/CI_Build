/// <summary>
///  Script Name : UC_STB_0110_NavigationFromStandbyToPowerOnDuringUT.cs
///  Test Name   : UC-STB-0110-NavigationFromStandbyToPowerOnDuringUT
///  TEST ID     : 73942
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Madhu Renukaradhya
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

[Test("LPW_UC_STB_0110")]
public class LPW_UC_STB_0110 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service videoService;
    static string powerMode = "";
    static string defaultMaintenanceDelay = "";
    static string defaultPowerMode = "";
   

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Settings Menu Power Mode and set default to hot stand by power mode";
    private const string STEP2_DESCRIPTION = "Step 2: Switch the box to stand By and wait for default maintenance delay";
    private const string STEP3_DESCRIPTION = "Step 3: Wake Up the box during maintenance and verify the box switches to power on mode by cancelling the maintenance and retrives back EIT data";


    private static class Constants
    {
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


            //Get Values From xml File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Failed to fetch service "+videoService.LCN+" from content xml.");

            }
            //Get Values From test ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }
        

            //Get default mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default power mode from project ini");
            }

            //Get default maintenance delay from project ini
            defaultMaintenanceDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_DEALY");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default maintenance delay value from project ini");
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
            
            //Tune to service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + videoService.LCN);
            }
            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
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

            //wait for maintenance to start
            int defaultMaintenanceDelayInt = Convert.ToInt32(defaultMaintenanceDelay);
            defaultMaintenanceDelayInt = defaultMaintenanceDelayInt * 60;

            //Fetch the maintenaceStart milestone from milestones.ini
            String maintenanceStartMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceStart");

            //Begin wait for maintenaceStart milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceStartMilestone, defaultMaintenanceDelayInt);
            ArrayList arrayList = new ArrayList();
            bool isMaintenanceMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceStartMilestone,ref arrayList);
            if (!isMaintenanceMilestoneRecieved)
            {
                FailStep(CL, res, "Failed to start maintenance ", false);
            }
            //clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //wait during maintenance
            //Get delay time From test ini File
            string delayTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DELAY");
            if (string.IsNullOrEmpty(delayTime))
            {
                FailStep(CL, res, "Unable to fetch delay time from test ini file");
            }
            int delayTimeInt = Convert.ToInt32(delayTime)*60;
            //wait for maintenance delay
            res = CL.IEX.Wait(delayTimeInt);


            //Wake up the box during maintenance
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

            //get Event name to verify that EIT data is recovered after wake up the box during maintenance
            string eventName = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Recover EIT data after wake up from stand by thus no Event name");
            }
            else
            {
                LogCommentInfo(CL, "EIT data recoverd successfully after wake up during maintenance delay time");
            }
            
            //get Channel logo to verify that EIT zipped is recovered after wake up the box during maintenance
           
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu ");
            }
            String obtainedChannelLogo = CL.EA.GetChannelLogo();
            LogComment(CL, "Obtained ChannelLogo is =  " + obtainedChannelLogo);

            if (!String.IsNullOrEmpty(obtainedChannelLogo))
            {
                LogComment(CL, "Channel logo is present = " + obtainedChannelLogo);
            }

            else
            {
                FailStep(CL, res, " Failed to Recover EIT ZIPPED after wake up from stand thus no channel logo. ");
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