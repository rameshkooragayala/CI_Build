/// <summary>
///  Script Name : DIAG_0110_CPE_DiagnosticsRefresh.cs
///  Test Name   : DIAG-0110-CPE diagnostics refresh
///  TEST ID     :
/// 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date : 18th Sept, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using System.Collections.Generic;

public class DIAG_0110 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    static bool isRFActive = true;
    static string rfSwitch;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Navigate to diagnostics and verify the Signal Strength and Signal Quality");
        this.AddStep(new Step1(), "Step 1: Remove the RF cable and verify the Signal Strength and Signal Quality is zero in 2 secs");
        this.AddStep(new Step2(), "Step 2: Connect the RF cable back and verify the Signal Strength and Signal Quality is changed in 2 secs");

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

            rfSwitch = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RF_SWITCH");
            if (rfSwitch == "")
            {
                FailStep(CL, "RF switch is not defined in the Test ini file");
            }
            //Navigate to Diagnostics

           // res = CL.EA.NavigateAndHighlight("STATE:DIAGNOSTICS", dictionary);
          //  if (!res.CommandSucceeded)
          //  {
          //      FailStep(CL, res, "Failed to Navigate to dignostics");
          //  }

            string timeStamp = "";

         //   CL.IEX.MilestonesEPG.ClearEPGInfo();

         //   res = CL.IEX.SendIRCommand("SELECT", 1, ref timeStamp);
        //    if (!res.CommandSucceeded)
        //    {
        //        FailStep(CL, res, "Failed to send the IR commend select");
        //    }
            CL.IEX.MilestonesEPG.ClearEPGInfo();

            CL.EA.UI.Utils.NavigateToDiagnostics();
			
            res = CL.IEX.Wait(seconds: 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait few seconds after navigating to Diagnostics");
            }

            //Verifying the Signal Quality
            string signalStrength = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("signal strength", out signalStrength);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get the Signal Strength from EPG");
            }
            LogCommentInfo(CL, "Signal Strength is " + signalStrength);
            //Signal Strength should be in between 1 and 100
            if (Convert.ToInt32(signalStrength) < 1 || Convert.ToInt32(signalStrength) > 100)
            {
                FailStep(CL, "Signal strength :" + signalStrength + " is not between 1 and 100 which is expected");

            }
            else
            {
                LogCommentImportant(CL, "Signal strength :" + signalStrength + " is between 1 and 100 which is expected");
            }

            //Verifying the Signal Quality
            string signalQuality = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("signal quality", out signalQuality);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get the Signal Quality from EPG");
            }
            //Signal strength should be in between 1 and 100
            if (Convert.ToInt32(signalQuality) < 1 || Convert.ToInt32(signalQuality) > 100)
            {
                FailStep(CL, "Signal strength :" + signalQuality + " is not between 1 and 100 which is expected");
            }
            else
            {
                LogCommentImportant(CL, "Signal Quality :" + signalQuality + " is between 1 and 100 which is expected");
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
            //Unplug RF signal
            res = CL.IEX.RF.TurnOff("1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }

            isRFActive = false;

            res = CL.IEX.Wait(seconds: 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait few seconds after navigating to Diagnostics");
            }
            string signalStrength = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("signal strength", out signalStrength);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get the Signal Strength from EPG");
            }
            LogCommentInfo(CL, "Signal Strength is " + signalStrength);
            //Signal Strength should be in between 1 and 100
            if (Convert.ToInt32(signalStrength) !=0)
            {
                FailStep(CL, "Signal strength :" + signalStrength + " is not Refreshed after removing the RF cable it is still greater then Zero");

            }
            else
            {
                LogCommentImportant(CL, "Signal strength :" + signalStrength + " is Zero after removing RF cable");
            }

            //Verifying the Signal Quality
            string signalQuality = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("signal quality", out signalQuality);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get the Signal Quality from EPG");
            }
            //Signal strength should be in between 1 and 100
            if (Convert.ToInt32(signalQuality) !=0)
            {
                FailStep(CL, "Signal Quality :" + signalQuality + " is not Refreshed after removing the RF cable it is still greater then Zero");

            }
            else
            {
                LogCommentImportant(CL, "Signal Quality :" + signalQuality + " is Zero after removing RF cable");
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

            //Connecting the RF Signal
            //Connecting the RF Signal
            if (rfSwitch.Equals("A"))
            {
                res = CL.IEX.RF.ConnectToA(instanceName: "1");
            }
            else
            {
                res = CL.IEX.RF.ConnectToB(instanceName: "1");
            }
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Connect RF back");
            }

            isRFActive = true;
            res = CL.IEX.Wait(seconds: 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait few seconds after navigating to Diagnostics");
            }

            //Verifying the Signal Quality
            string signalStrength = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("signal strength", out signalStrength);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get the Signal Strength from EPG");
            }
            LogCommentInfo(CL, "Signal Strength is " + signalStrength);
            //Signal Strength should be in between 1 and 100
            if (Convert.ToInt32(signalStrength) < 1 || Convert.ToInt32(signalStrength) > 100)
            {
                FailStep(CL, "Signal strength :" + signalStrength + " is not between 1 and 100 after connecting the RF cable back which is expected");

            }
            else
            {
                LogCommentImportant(CL, "Signal strength :" + signalStrength + " is between 1 and 100 after connecting the RF cable back which is expected");
            }

            //Verifying the Signal Quality
            string signalQuality = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("signal quality", out signalQuality);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get the Signal Quality from EPG");
            }
            //Signal strength should be in between 1 and 100
            if (Convert.ToInt32(signalQuality) < 1 || Convert.ToInt32(signalQuality) > 100)
            {
                FailStep(CL, "Signal strength :" + signalQuality + " is not between 1 and 100 after connecting the RF cable back which is expected");
            }
            else
            {
                LogCommentImportant(CL, "Signal Quality :" + signalQuality + " is between 1 and 100 after connecting the RF cable back which is expected");
            }

            PassStep();
        }
    }

    #endregion Step1
    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //Plug back RF if there was failure in reconnection
        if (!isRFActive)
        {
            res = CL.IEX.RF.ConnectToA("1");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to plug back RF signal!");
                //FailStep(CL, res, "Failed to plug back RF signal!");
            }
        }
    }

    #endregion PostExecute
}