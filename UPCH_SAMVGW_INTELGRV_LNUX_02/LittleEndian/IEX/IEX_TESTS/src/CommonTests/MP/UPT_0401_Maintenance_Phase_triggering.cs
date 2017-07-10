/// <summary>
///  Script Name : UPT-0401-Maintenance Phase triggering.cs
///  Test Name   : UPT-0401-Maintenance Phase triggering
///  TEST ID     : 10642
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

//UPT_0401_Maintenance Phase triggering
public class UPT_0401 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service videoService;
    static string powerMode = "";
    static string defaultMaintenanceDelay = "";
    static string MaintenanceDuration = "";
    static string defaultPowerMode = "";


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Settings Menu Power Mode and set default to hot stand by power mode";
    private const string STEP2_DESCRIPTION = "Step 2: Switch the box to stand By and wait for default maintenance delay";
    private const string STEP3_DESCRIPTION = "Step 3: Maintenance phase in standby should trigger and completes in Hot standby state.";


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
                FailStep(CL, "Failed to fetch service " + videoService.LCN + " from content xml.");

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
            if (string.IsNullOrEmpty(defaultMaintenanceDelay))
            {
                FailStep(CL, res, "Unable to fetch the default maintenance delay value from project ini");
            }

            //Get default maintenance delay from project ini
            MaintenanceDuration = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_DURATION");
            if (string.IsNullOrEmpty(MaintenanceDuration))
            {
                FailStep(CL, res, "Unable to fetch the default MaintenanceDuration value from project ini");
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

            LogCommentWarning(CL, "CPE should put off the A/V and should switch to Hot standby.");
            
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
            LogCommentWarning(CL, "Wait for maintenance to start after 10 mins in hot standby.");
            int defaultMaintenanceDelayInt = Convert.ToInt32(defaultMaintenanceDelay);
            defaultMaintenanceDelayInt = defaultMaintenanceDelayInt * 60;

            //Fetch the maintenaceStart milestone from milestones.ini
            String maintenanceStartMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceStart");

            //Begin wait for maintenaceStart milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceStartMilestone, defaultMaintenanceDelayInt);

            ArrayList arrayList = new ArrayList();
            bool isMaintenanceMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceStartMilestone, ref arrayList);
            if (!isMaintenanceMilestoneRecieved)
            {
                FailStep(CL, res, "Failed to start maintenance ", false);
            }         

            LogCommentWarning(CL, "Wait for maintenance to completes.");

             //Fetch the maintenaceComplete milestone from milestones.ini
            String maintenanceCompleteMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceComplete");

            int MaintenanceDurationInt = Convert.ToInt32(MaintenanceDuration)*60;

            //Begin wait for maintenaceComplete milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceCompleteMilestone, MaintenanceDurationInt);
            
        
            bool maintenanceCompleteMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceCompleteMilestone, ref arrayList);
            if (!maintenanceCompleteMilestoneRecieved)
            {
                FailStep(CL, res, "Failed to complete the maintenance", false);
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