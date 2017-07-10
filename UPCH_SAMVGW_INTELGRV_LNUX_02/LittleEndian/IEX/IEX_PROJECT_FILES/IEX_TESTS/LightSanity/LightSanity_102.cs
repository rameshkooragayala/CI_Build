using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-102-STB-Low Power Mode
public class LightSanity_102 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    #region Create Structure
    public override void CreateStructure()
    {
        //Brief Description: LightSanity-102-STB-Low Power Mode
        //Pre-conditions: none.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: none.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Navigate to AUTO STANDBY menu and set the AUTOMATIC option");
        this.AddStep(new Step2(), "Step 2: Navigate to STANDBY POWER USAGE menu and Set Lukewarm standby mode");
        this.AddStep(new Step3(), "Step 3: Validate the Lukewarm standby mode");

        //Get Client Platform
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
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Step 1: Navigate to AUTO STANDBY menu and set AUTOMATIC option
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AUTO STANDBY");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.MilestonesEPG.Navigate("AUTOMATIC");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AUTOMATIC");
            }
            PassStep();
        }
    }
    #endregion
    #region Step2
    //Step 2: Navigate to STANDBY POWER USAGE menu and Set Lukewarm standby mode
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:STANDBY POWER USAGE");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.MilestonesEPG.Navigate("MEDIUM");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MEDIUM");
            }
            CL.EA.ReturnToLiveViewing();

            PassStep();
        }
    }
    #endregion
    #region Step3
    //Step 3: Validate the Lukewarm standby mode
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string msg_utm_start = "IEX_UTM_STARTED";
            string msg_utm_end = "IEX_UTM_COMPLETED";
            string actual_msg;
            string maginlines;

            // Put the box into standby mode
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            // Wait for 20 mins to complete the UT. Since after UT, box will go to the required standby mode
            res = CL.IEX.Debug.BeginWaitForMessage(msg_utm_start, 0, 20 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.IEX.Debug.BeginWaitForMessage(msg_utm_end, 0, 20 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.IEX.Debug.EndWaitForMessage(msg_utm_start, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }

            res = CL.IEX.Debug.EndWaitForMessage(msg_utm_end, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }

            // Check any of the component goes to shutdown run level
            res = CL.IEX.Debug.BeginWaitForMessage("IEX_SHUTDOWN_FAS", 0, 2 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.IEX.Debug.EndWaitForMessage("IEX_SHUTDOWN_FAS", out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }
            // Wait for 180 sec to box goes to shutdown runlevel
            CL.IEX.Wait(120);

            string timeStamp = "";
            res = CL.IEX.IR.SendIR("POWER", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key POWER");
            }

            // Wait for 180 sec to box to reboot
            CL.IEX.Wait(90);

            //// mount GW without FORMAT
            //res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res);
            //}

            //Check video
            res = CL.EA.CheckForVideo(true, false, 240);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present After wakeup from Lukewarm Standby");
            }
            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}