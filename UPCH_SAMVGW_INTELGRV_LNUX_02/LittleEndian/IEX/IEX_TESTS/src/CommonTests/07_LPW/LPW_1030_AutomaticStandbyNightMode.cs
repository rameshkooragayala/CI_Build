/// <summary>
///  Script Name : LPW_1030_AutomaticStandbyNightMode.cs
///  Test Name   : LPW-1030-AutomaticStandbyNightMode
///  TEST ID     : 73747
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using System.Threading;

[Test("LPW_1030")]
public class LPW_1030 : _Test
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
    static string expectedStandByWarningDuration = "";
    static string waitTime = "";
    static Int32 defaultDelayTime = 0;
    static string defaultNightStartTime = "";
    static string defaultNightEndTime = "";
   

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the default values and required values from ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Set the Power mode to default StandyBy AND Set the automatic standby option to 'AT NIGHT' and verify for default night start time and end time ";
    private const string STEP2_DESCRIPTION = "Step 2: Set the night frame window time";
    private const string STEP3_DESCRIPTION = "Step 3: Verify that box moves to default Standby";

    private static class Constants
    {
        public const int milestonewait = 5;//in mins
        public const int bufferTime = 5;//in mins

    }

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

            //Get default standby from project ini
            expectedDefaultstandyBy = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_STANDBY");
            if (string.IsNullOrEmpty(expectedDefaultstandyBy))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Get default standby Duration from project ini
            expectedStandByWarningDuration = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "WARNING_MESSAGE_TIMEOUT");
            if (string.IsNullOrEmpty(expectedStandByWarningDuration))
            {
                FailStep(CL, res, "Unable to fetch the default default standby Duration value from project ini");
            }

            //Get default defaultNightStartTime from project ini
            defaultNightStartTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "NIGHT_START_TIME");
            if (string.IsNullOrEmpty(defaultNightStartTime))
            {
                FailStep(CL, res, "Unable to fetch the default NIGHT_START_TIME value from project ini");
            }

            //Get default defaultNightEndTime from project ini
            defaultNightEndTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "NIGHT_END_TIME");
            if (string.IsNullOrEmpty(defaultNightEndTime))
            {
                FailStep(CL, res, "Unable to fetch the default NIGHT_END_TIME value from project ini");
            }

            //Wait to make the box keep idle for set period
            waitTime = period.Split(' ')[0];
            defaultDelayTime = (Int32.Parse(waitTime));
            LogCommentInfo(CL, "Idle Time wait in minutes : " + defaultDelayTime);

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

            //Clear EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to clear EPG Info", false);
            }
            //Navigate to night start time
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DEFINE AUTO STANDBY TIME NIGHT START TIME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to DEFINE AUTO STANDBY TIME NIGHT START TIME on ToolBox ");
            }

           //Verify for default night start time value
            string obtainedNightStartTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("data", out obtainedNightStartTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to get the default startNightTime Value");
            }
            else
            {
                LogCommentInfo(CL, "obtainedNightStartTime :" + obtainedNightStartTime);
            }
            if (defaultNightStartTime.Equals(obtainedNightStartTime))
            {
                LogCommentInfo(CL, "Default night start time is set to :" + defaultNightStartTime);
            }
            else
            {
                FailStep(CL, res, "Default night strat time is not set to :" + defaultNightStartTime +" but it is set to: "+obtainedNightStartTime);
            }

            //Clear EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to clear EPG Info", false);
            }
            //Navigate to night end time
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DEFINE AUTO STANDBY TIME NIGHT END TIME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to DEFINE AUTO STANDBY TIME NIGHT END TIME on ToolBox ");
            }

            //Verify for default night end time value
            string obtainedEndNightTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("data", out obtainedEndNightTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to get the default endNightTime Value");
            }
            else
            {
                LogCommentInfo(CL, "Obtained end night time is set to :" + obtainedEndNightTime);
            }
            if (defaultNightEndTime.Equals(obtainedEndNightTime))
            {
                LogCommentInfo(CL, "Default end night time is set to :" + defaultNightEndTime);
            }
            else
            {
                FailStep(CL, res, "Default night time is not set to :" + defaultNightEndTime + " but it is set to: " + obtainedEndNightTime);
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

            string EPG_Time = "";
            CL.EA.UI.Live.GetEpgTime(ref EPG_Time);
            LogCommentInfo(CL, "Current EPG Time: " + EPG_Time);

            DateTime startTime = DateTime.Parse(EPG_Time).AddMinutes(2);
            DateTime endTime = startTime.AddMinutes(defaultDelayTime+Constants.bufferTime);

            string srtTime = Convert.ToString(startTime);
            LogCommentInfo(CL, "Start Time: " + startTime);

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

    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            defaultDelayTime = defaultDelayTime * 60;//in seconds
            LogCommentInfo(CL, "Idle Time wait in minutes : " + defaultDelayTime);
            res = CL.IEX.Wait(defaultDelayTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed During Idle wait");
            }

            //wait for 180 seconds for stand by warining alert.
            defaultDelayTime = (Int32.Parse(expectedStandByWarningDuration));   //in seconds
            //Add the delay of 5 second to get the milestone.
            res = CL.IEX.Wait(defaultDelayTime + Constants.milestonewait);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed During Idle wait");
            }

            string obtainedPowerMode = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("DefaultStandByPref is", out obtainedPowerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch obtained power mode value");
            }
            else
            {
                LogCommentInfo(CL, "The device is in default standBy during the Set Night window." + obtainedPowerMode);

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

        //wait for 3 min so that set up box can be in standBy mode.
        defaultDelayTime = (Int32.Parse(expectedStandByWarningDuration));   //in seconds
        res = CL.IEX.Wait(defaultDelayTime);
        if (!(res.CommandSucceeded))
        {
            LogCommentInfo(CL, "Failed During Idle wait ");
        }

        //Wake up the Set box or exiting from the stand by.
        res = CL.EA.StandBy(true);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to make set up box waking up ");

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
