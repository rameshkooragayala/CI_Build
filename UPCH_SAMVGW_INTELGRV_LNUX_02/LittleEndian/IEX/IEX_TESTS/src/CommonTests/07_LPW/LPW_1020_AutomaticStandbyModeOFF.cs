/// <summary>
///  Script Name : LPW_1020_AutomaticStandbyModeOFF.cs
///  Test Name   : LPW-1020-Automatic Stand by Mode-OFF
///  TEST ID     : 73746
///  QC Version  : 2
///  Variations from QC:none
///  QC Repository : UPC/FR_FUSION 
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

[Test("LPW_1020")]
public class LPW_1020 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps
    static string powerMode = "";
    static string defaultPowerMode = "";
    static string period = "";
    static string defaultPeriod = "";
    static string standyBy = "";
    static string expectedDefaultstandyBy = "";
    static string waitTime = "";
    static Int32 defaultDelayTime = 0;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the default values and required values from ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Set the Default power mode to High,Set the Default Delay Stand By time to minium time and Set the stand by mode to OFF";
    private const string STEP2_DESCRIPTION = "Step 2: Wait for Standby delay and verify that even after inactivity period is over its not moved to default stand by";
    private const string STEP3_DESCRIPTION = "Step 3: Set the night frame window time and wait till inactivity period is over.";
    private const string STEP4_DESCRIPTION = "Step 4: Verify that box never moves to default Standby";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        //Get Client Platform
        CL = GetClient();

        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);


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
            //Get Values From ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }
            // powerMode = "MEDIUM";
            //Get default mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Get Values From ini File
            period = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PERIOD");
            if (string.IsNullOrEmpty(period))
            {
                FailStep(CL, res, "Unable to fetch the period from test ini file");
            }
            //period = "30 min.";

            //Get default period from project ini
            defaultPeriod = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_PERIOD");
            if (string.IsNullOrEmpty(defaultPeriod))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Get standby value to set from test ini File
            standyBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STANDBY");
            if (string.IsNullOrEmpty(standyBy))
            {
                FailStep(CL, res, "Unable to fetch the standBy value from test ini file");
            }
            //  standyBy = "AUTOMATIC";

            //Get default standby from project ini
            expectedDefaultstandyBy = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_STANDBY");
            if (string.IsNullOrEmpty(expectedDefaultstandyBy))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
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

            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully to : " + powerMode);
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
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Wait to make the box keep idle for set period

            waitTime = period.Split(' ')[0];
            defaultDelayTime = (Int32.Parse(waitTime)) * 60;//in mins
            LogCommentInfo(CL, "Idle Time wait : " + defaultDelayTime);
            res = CL.IEX.Wait(defaultDelayTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed During Idle wait");
            }
            bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (liveState)
            {
                LogCommentInfo(CL, "Live state verified sucessfully after idle wait time since the stand by mode is OFF");
            }
            else
            {
                FailStep(CL, res, "Unable to verify the live state after idle wait time");
            }

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

            //Refreeshing the EPG to get the Current time from EPG
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to main menu", exitTest: false);
            }
            string EPG_Time = "";
            CL.EA.UI.Live.GetEpgTime(ref EPG_Time);
            LogCommentInfo(CL, "Current EPG Time: " + EPG_Time);

            DateTime startTime = DateTime.Parse(EPG_Time);
            startTime = startTime.AddMinutes(2);
            DateTime endTime = startTime.AddHours(1);

            string srtTime = Convert.ToString(startTime);
            LogCommentInfo(CL, "Start Time: " + srtTime);

            string endtime = Convert.ToString(endTime);
            LogCommentInfo(CL, "End Time: " + endtime);

            res = CL.EA.STBSettings.SetNightTime(srtTime, endtime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set night time.");
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

            LogCommentInfo(CL, "Verifying after default delay time within night time window the box is still in live state when standby mode is OFF ");
            
            res = CL.IEX.Wait(defaultDelayTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed During Idle wait");
            }


            bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (liveState)
            {
                LogCommentInfo(CL, "Live state verified sucessfully even after night window defined after idle wait time since the stand by mode is OFF");
            }
            else
            {
                FailStep(CL, res, "Unable to verify the live state after after idle wait time after night window defined");
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

        //Restore the standyBy option
        res = CL.EA.STBSettings.SetAutoStandBy(expectedDefaultstandyBy);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default standBy");
        }
        else
        {
            LogCommentInfo(CL, "Restored to default STANDBY SUCCESSFULLY");
        }

    }
    #endregion
}