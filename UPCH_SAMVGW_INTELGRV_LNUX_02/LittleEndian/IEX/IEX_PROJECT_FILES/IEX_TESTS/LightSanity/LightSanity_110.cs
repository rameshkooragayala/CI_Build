using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class LightSanity_110 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string FTA_1st_Mux_3;
    static string FTA_2nd_Mux_1;

    //Shared parameters between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:
        //Performs Live Surfing between Free To Air services -  NOT Fast Zapping.
        //Checks DCA and Ch+\Ch- (not fast zapping), and DCA to another Transponder.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not testing Channel Bar Info in any step, not checking Audio and not checking how fast the tuning is performed.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to a Free To Air Service Using DCA");
        this.AddStep(new Step2(), "Step 2: Switch to another FTA");
        this.AddStep(new Step3(), "Step 3: Switch to any FTA Using Channel Up and Down");
        this.AddStep(new Step4(), "Step 4: Zap Between FTA Channels on 2 Different Transponders");

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
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            FTA_2nd_Mux_1 = CL.EA.GetValue("FTA_2nd_Mux_1");

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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Service S1 after DCA");
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

            //Switch to another free to air service which is not the following channel:
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a Second FTA Channel With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Service S3");
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

            //Switch back to any free to air service Using channel up and down:
            //Do not use Fast Zapping!
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a Second FTA Channel With DCA After Channel Changes");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present After Channel Changes");
            }
            //Tune to any free to air service using channel up and down
            //Do not use Fast Zapping!
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With Channel Down");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Service After Channel Down");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With Channel Up");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Service After Channel Up");
            }

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

            //Zap between 2 channels in different transponders:
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_2nd_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel On Second Mux With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel On Second Mux");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel On First Mux With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on FTA Channel On First Mux");
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