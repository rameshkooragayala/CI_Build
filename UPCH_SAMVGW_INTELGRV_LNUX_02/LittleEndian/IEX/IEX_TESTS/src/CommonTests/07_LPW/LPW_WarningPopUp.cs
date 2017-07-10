/// <summary>
///  Script Name : LPW_WarningPopUp.cs
///  Test Name   : LPW-0600-Warning pop up message
///  TEST ID     : 72119,72120
///  QC Version  : 2
///  Variations from QC:none
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

[Test("LPW_WarningPopUp")]
public class LPW_WarningPopUp : _Test
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
    static string warningMessageTimeOut = "";
    static string expectedPowerMode = "";
    static string cancelWarning = "";
    static bool cancelWarningMessage = false;
    static string cancelKey = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the default values and required values from ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Set the Default power mode to High and Set the Default Delay Stand By time to minium time";
    private const string STEP2_DESCRIPTION = "Step 2: Wait for Standby delay";
    private const string STEP3_DESCRIPTION = "Step 3: Verify for Notification Messsage and cancel and verify the live state after canceling the notification ";
    private const string STEP4_DESCRIPTION = "Step 4: Verify that box moves to default Standby";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
		//Get Client Platform
        CL = GetClient();
     
            cancelWarning = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "CANCEL_WARNING");
            if (cancelWarning == null)
            {
                cancelWarning = "false";
                LogComment(CL, "Unable to fetch the cancelWarnig value");
            }
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        cancelWarningMessage = Convert.ToBoolean(cancelWarning);
        if (!cancelWarningMessage)
        {
            this.AddStep(new Step4(), STEP4_DESCRIPTION);
        }

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

            //Get Values From ini File
            standyBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STANDBY");
            if (string.IsNullOrEmpty(standyBy))
            {
                FailStep(CL, res, "Unable to fetch the standBy value from test ini file");
            }
          //  standyBy = "AUTOMATIC";

            //Get default period from project ini
            expectedDefaultstandyBy = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_STANDBY");
            if (string.IsNullOrEmpty(expectedDefaultstandyBy))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Get Idle time warning message time out from project ini
            warningMessageTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "WARNING_MESSAGE_TIMEOUT");
            if (string.IsNullOrEmpty(warningMessageTimeOut))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Fetch the Expected PowerMode from project ini
            expectedPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", powerMode);
            if (string.IsNullOrEmpty(expectedPowerMode))
            {
                FailStep(CL, res, "Mode value fetched from project is empty");
            }
            else
            {
                LogCommentInfo(CL, "Expected mode value for"+powerMode+ "is: " + expectedPowerMode);
            }


            //Get key to cancel warning message for going to standBy after idle time period from project ini
            cancelKey = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "KEY");
            if (string.IsNullOrEmpty(cancelKey))
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
                LogCommentInfo(CL, "Power mode set Successfully to : "+powerMode);
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

            var waitTime = period.Split(' ')[0];
            Int32 time = (Int32.Parse(waitTime))*60;//in mins
            LogCommentInfo(CL, "Idle Time wait : " + waitTime);
            res = CL.IEX.Wait(time);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed During Idle wait");
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

            //Verify for Warning message after Idle time wait.
            bool notificationMessage = CL.EA.UI.Utils.VerifyState("NOTIFICATION MESSAGE", 20);
            if (!notificationMessage)
            {
                FailStep(CL, "Unable to display warning message after Idle Time" + period);
            }

            if (cancelWarningMessage)
            {

                //Send IR key to cancel
                string timeStamp = "";
                res = CL.IEX.IR.SendIR(cancelKey, out timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to send the "+cancelKey+"to cancel the warning message");
                }
                bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
                if (liveState)
                {
                    LogCommentInfo(CL, "Live state verified sucessfully after cancelling the warning message");
                }
                else
                {
                    FailStep(CL, res, "Unable to verify the live state after cancelling the warning message");
                }
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
            //Wait till warning message dissappears
            Int32 countDownWait = Int32.Parse(warningMessageTimeOut);
            CL.IEX.Wait(countDownWait);

            //Verify that the box goes to default standBy after idle time
            string obtainedPowerMode = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("DefaultStandByPref is", out obtainedPowerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch obtained power mode value");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Power Mode is :" + obtainedPowerMode);
            }
            
            //Verify that selected value is set successfully
            if (!(expectedPowerMode.Equals(obtainedPowerMode)))
            {
                FailStep(CL, res, "Failed to set selected value");
            }
            else
            {
                LogCommentInfo(CL, "Selected value is set successfully");
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

        //Out of Stand by 
        if (!cancelWarningMessage)
        {
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to bring box out of Standby");
            }
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