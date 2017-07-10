using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity_370 - Nominal Boot
public class LightSanity_370 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static string channelNumBeforeStandBy = "";
    static string channelNumAfterStandBy = "";
    //Channels used by the test
    static string FTA_1st_Mux_2;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity_370 - Nominal Boot
        //Performs live surfing between Free To Air services NOT Fast Zapping.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not testing Channel Bar Info in any step and not checking how fast the tuning is performed.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to a Free To Air Service Using DCA & Set Lastviewed channel as default");
        this.AddStep(new Step2(), "Step 2: Goto standby and check start channel is changed");
        this.AddStep(new Step3(), "Step 3: Perform a Manual Reboot and allow the CPE to complete the power cycle");
        this.AddStep(new Step4(), "Step 4: Wake-up the CPE from the stand-by and check the channel number is the same as start channel");

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
            FTA_1st_Mux_2 = CL.EA.GetValue("FTA_1st_Mux_2");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_1st_Mux_2 = " + FTA_1st_Mux_2);

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

            // Tune to the given channel
            res = CL.EA.TuneToChannel(FTA_1st_Mux_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Tune to the given channel ");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumBeforeStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the channel number ");
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
            StartStep();

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            //Stay in Standby for a few seconds, to make sure RB was Deleted
            double timeInStandby = 5;
            CL.IEX.Wait(timeInStandby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.EA.CheckForVideo(true, false, 120);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Channel Bar Surfing");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumAfterStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel After DCA");
            }

            CL.IEX.LogComment("channelNumAfterStandBy = " + channelNumAfterStandBy + " channelNumBeforeStandBy = " + channelNumBeforeStandBy);

            if (channelNumAfterStandBy != channelNumBeforeStandBy)
            {
                FailStep(CL, res, "Failed to Verify that the CPE tuned to same channel after standby");
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
            res = CL.EA.PowerCycle(0, true, false);
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 240;
            CL.IEX.Wait(Time_In_Standby);

            PassStep();

        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumAfterStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel After DCA");
            }

            CL.IEX.LogComment("channelNumAfterStandBy = " + channelNumAfterStandBy + " channelNumBeforeStandBy = " + channelNumBeforeStandBy);

            if (channelNumAfterStandBy != channelNumBeforeStandBy)
            {
                FailStep(CL, res, "Failed to Verify that the CPE tuned to same channel after standby");
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