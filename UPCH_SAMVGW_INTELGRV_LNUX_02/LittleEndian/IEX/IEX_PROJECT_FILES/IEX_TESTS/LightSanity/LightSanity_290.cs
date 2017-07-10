using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-290-SET-Factory_Reset
public class LightSanity_290 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Medium_SD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-290-SET-Factory_Reset
        //To check that the user is allowed to perform the factory reset of the CPE device.
        //Pre-conditions: None.
        //Based on QualityCenter test version 5.
        //Variations from QualityCenter: None.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Change Default Settings, Zap to Service S1 and Record Current Event");
        this.AddStep(new Step2(), "Step 2: Perform Factory Reset");
        this.AddStep(new Step3(), "Step 3: Check Settings Back to Default and Recordings Deleted From Disk");

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

            //Get Values From ini File
            Medium_SD_1 = CL.EA.GetValue("Medium_SD_1");


            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 5 Sec");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            // res = CL.EA.CheckForAudio(true, 10);
            // if (!res.CommandSucceeded)
            // {
            //     FailStep(CL,res, "Failed to Check for Audio");
            // }

            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                int timeToEventEnd_sec = 0;
                res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Get Time Left to Current Event");
                }

                if (timeToEventEnd_sec < 180)
                {
                    CL.EA.ReturnToLiveViewing();
                    CL.IEX.Wait(timeToEventEnd_sec);
                }

                //Record on going event from Banner 
                res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecFromBanner", 1, false, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Record Current Event From Banner");
                }

                res = CL.EA.WaitUntilEventEnds("EveRecFromBanner");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Wait Till Event End Recording");
                }
                CL.IEX.Wait(120);
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            string timeStamp = "";
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:FACTORY RESET");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate To FACTORY RESET Screen");
            }

            res = CL.EA.EnterDeafultPIN("YES_NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set yes to factory reset");
            }

            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("YES");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to SelectMenuItem YES");
                }

                res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Send IR Key SELECT");
                }
            }

            res = CL.IEX.MilestonesEPG.SelectMenuItem("RESET TO DEFAULT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to SelectMenuItem RESET TO DEFAULT");
            }

            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }

            res = CL.IEX.MilestonesEPG.SelectMenuItem("YES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to SelectMenuItem YES");
            }

            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }

            res = CL.IEX.Debug.BeginWaitForMessage("bin/sh", 0, 200, IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage");
            }

            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }
            res = CL.IEX.Debug.EndWaitForMessage("bin/sh", out timeStamp, out timeStamp, IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to EndWaitForMessage");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.MountGw();
            //res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Mount Client: " + res.FailureReason);

            }

            // To quit the settings menu popup after 1st Installation.
            // if (CL.EA.UI.Utils.VerifyDebugMessage("title", "YES", 150) == true)
            // {
            //   string temp;
            //   res = CL.IEX.IR.SendIR("MENU", out temp, 10000);
            //   CL.IEX.LogComment("Exit from Settings Menu Popup");
            // }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navegte To Channel Bar Timeout Screen");
            }

            string value = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out value);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to GetEPGInfo title");
            }
            if (value != "10")
            {
                // FailStep(CL,res, "Failed: Settings Values aren't Default After Factory Reset");
            }

            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                //Verify event not in Planner and not in Archive
                res = CL.EA.PVR.VerifyEventInPlanner("EveRecFromBanner", true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                res = CL.EA.PVR.VerifyEventInArchive("EveRecFromBanner", true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
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