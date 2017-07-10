using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-016-STB-Default Standby state
public class LightSanity_016 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_1st_Mux_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-016-STB-Default Standby state
        //Pre-conditions: There is no recording scheduled with next 15min.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Put CPE in Standby");
        this.AddStep(new Step2(), "Step 2: Wait For 10 Minutes");
        this.AddStep(new Step3(), "Step 3: Wake Up CPE and Check A/V");

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
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:STANDBY POWER USAGE");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.MilestonesEPG.Navigate("HIGH");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to HIGH");
            }
            CL.EA.ReturnToLiveViewing();
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(false, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
            }

            res = CL.EA.CheckForAudio(false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Audio is Present After Standby");
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

            //Stay in Standby for 10 Minutes.
            double timeInStandby = 660;
            CL.IEX.Wait(timeInStandby);

            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present After Exiting From Standby");
            }

            res = CL.EA.CheckForAudio(true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Audio is Present After Exiting From Standby");
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