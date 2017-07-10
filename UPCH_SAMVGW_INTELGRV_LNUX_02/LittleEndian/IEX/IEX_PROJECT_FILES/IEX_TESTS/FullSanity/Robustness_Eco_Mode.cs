using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;



//Robustness_Eco_Mode
public class Robustness_Eco_Mode : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service service;
    static string powerMode = "";
    static string defaultPowerMode = "";

    #region Create Structure
    public override void CreateStructure()
    {
        //Brief Description: Robustness_Eco_Mode - Eco Mode Robustness in loop
        //Pre-conditions: none.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Navigate to AUTO STANDBY menu and set the AUTOMATIC option");
        this.AddStep(new Step2(), "Step 2: Validate the Eco standby mode in loop for 30 loops");

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

            //Get Values From xml File
            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch service from content xml.");

            }
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
            PassStep();
        }
    }
    #endregion
    #region Step2
    //Step 2: Validate the Eco standby mode in loop
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string msg_utm_start = "IEX_UTM_STARTED";
            string msg_utm_end = "IEX_UTM_COMPLETED";
            string actual_msg;
            string maginlines;

            for (int Iteration = 1; Iteration < 30; Iteration++)
            {
                LogCommentInfo(CL, "Ecomode loop number: " + Iteration);
                CL.IEX.Wait(120);
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
                    FailStep(CL, res, "Failed To Verify Message :" + actual_msg + " at ecomode iteration: " + Iteration);
                }

                res = CL.IEX.Debug.EndWaitForMessage(msg_utm_end, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Verify Message :" + actual_msg  + "at ecomode iteration: " + Iteration);
                }

                // Check any of the component goes to shutdown run level
                res = CL.IEX.Debug.BeginWaitForMessage("IEX_SHUTDOWN_FAS", 0, 3 * 60, IEXGateway.DebugDevice.Udp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "at ecomode iteration: " + Iteration);
                }

                res = CL.IEX.Debug.EndWaitForMessage("IEX_SHUTDOWN_FAS", out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Verify Message :" + actual_msg + "at ecomode iteration: " + Iteration);
                }
                // Wait for 120 sec to box goes to shutdown runlevel
                CL.IEX.Wait(120);

                string timeStamp = "";
                res = CL.IEX.IR.SendIR("POWER", out timeStamp, 3);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Send IR Key POWER");
                }

                // Wait for 90 sec to box to reboot
                CL.IEX.Wait(90);

                //// mount GW without FORMAT
                res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
                //if (!res.CommandSucceeded)
                //{
                //    FailStep(CL, res);
                //}
                //Check video
                res = CL.EA.CheckForVideo(true, false, 240);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify that Video is Present After wakeup from Lukewarm Standby" + "at ecomode iteration: " + Iteration);
                }
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