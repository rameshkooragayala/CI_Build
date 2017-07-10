using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity_0410
public class FullSanity_0410 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static string channelNumBeforeStandBy = "";
    static string channelNumAfterStandBy = "";
    //Channels used by the test
    static string FTA_1st_Mux_2;
    static string FTA_2nd_Mux_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-002-LIVE-Zapping
        //Performs live surfing between Free To Air services NOT Fast Zapping.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not testing Channel Bar Info in any step and not checking how fast the tuning is performed.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to a Free To Air Service Using DCA");
        this.AddStep(new Step2(), "Step 2: Switch to Another FTA Which isn't The Following Channel");
        //this.AddStep(new Step3(), "Step 3: Reboot without format ");
        //this.AddStep(new Step4(), "Step 4: Check the channel number is the same as before the reboot ");


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

            FTA_2nd_Mux_1 = CL.EA.GetValue("FTA_2nd_Mux_1");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_2nd_Mux_1 = " + FTA_2nd_Mux_1);

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

            // Tune to a free to air service S1 using the numeric key of STB remote control
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel After DCA");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumBeforeStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel After DCA");
            }

            //TODO: Check audio, channel bar, event information
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
            res = CL.EA.CheckForVideo(true, false, 60);
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
            if (channelNumAfterStandBy != channelNumBeforeStandBy)
            {
                FailStep(CL, res, "Failed to Verify that the CPE tuned to same channel after standby");
            }
            PassStep();

        }
    }
    #endregion
    //#region Step3
    //private class Step3 : _Step
    //{
    //    public override void Execute()
    //    {
    //        StartStep();
    //        res = CL.EA.PowerCycle(0,true,false);
    //        if (!res.CommandSucceeded)
    //        {
    //            FailStep(CL,res);
    //        }

    //        PassStep();

    //    }
    //}
    //#endregion
    //#region Step4
    //private class Step4 : _Step
    //{
    //    public override void Execute()
    //    {
    //        StartStep();
    //        res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumAfterStandBy);
    //        if (!res.CommandSucceeded)
    //        {
    //            FailStep(CL,res, "Failed to Verify Video is Present On FTA Channel After DCA");
    //        }

    //        PassStep();

    //    }
    //}
    //#endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}