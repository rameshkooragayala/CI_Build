using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-1601-UPT-Automatic Maintenance Phase
public class FullSanity_1601 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-1601-UPT-Automatic Maintenance Phase
        //Pre-conditions: none.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: none.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Navigate to AUTO STANDBY menu and set the AUTOMATIC option");
        this.AddStep(new Step2(), "Step 2: Navigate to STANDBY POWER USAGE menu and Set Hot standby mode");
        this.AddStep(new Step3(), "Step 3: Go to Standby and check MP has started");
        this.AddStep(new Step4(), "Step 4: Wait for the MP to complete");

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
    //Step 2: Navigate to STANDBY POWER USAGE menu and Set Hot standby mode
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
            res = CL.IEX.MilestonesEPG.Navigate("HOT STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to HIGH");
            }
            CL.EA.ReturnToLiveViewing();

            PassStep();
        }
    }
    #endregion
    #region Step3
    //Step 3: Go to Standby and check MP has started
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string msg_utm_start = "IEX_UTM_STARTED";
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

            res = CL.IEX.Debug.EndWaitForMessage(msg_utm_start, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }

            CL.IEX.LogComment("MP Started!!!");

            PassStep();
        }
    }
    #endregion
    #region Step4
    //Step 4: Wait for the MP to complete
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            string msg_utm_end = "IEX_UTM_COMPLETED";
            string actual_msg;
            string maginlines;

            res = CL.IEX.Debug.BeginWaitForMessage(msg_utm_end, 0, 20 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.IEX.Debug.EndWaitForMessage(msg_utm_end, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }


            // Check the next MP is scheduled at end time +22hours + Random value in the range [0...180min]
            CL.IEX.Wait(10);

            // According to quality center we should check for the next update. It was there in SPM
            // milestone can't be added, so we are not waiting for that. But in telnet log we are getting that
            // 02:43:36.866 30Apr NDS: ^1301617186.808456 !MIL   -SPM_SERVER   < p:0000155e t:71d5d8b0 T:SPM_THREAD M:spm_update.c F:SPM_UPDATE_ScheduleNextUpdate L:01918 > Next update: day mask=0x00,  schedule time type=0,  time(local)=1367397457,  time(UTC)=1367393857
            // res = CL.IEX.Debug.BeginWaitForMessage("Next update", 0, 1 * 60, IEXGateway.DebugDevice.Telnet);
            /*if (!res.CommandSucceeded)
            {
                FailStep(CL,res);
            }

            res = CL.IEX.Debug.EndWaitForMessage("Next update", out actual_msg, out maginlines, IEXGateway.DebugDevice.Telnet);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed To Verify Message :" + actual_msg);
            }
            */

            CL.IEX.LogComment("MP Completed!!!");

            // wakeup the box
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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