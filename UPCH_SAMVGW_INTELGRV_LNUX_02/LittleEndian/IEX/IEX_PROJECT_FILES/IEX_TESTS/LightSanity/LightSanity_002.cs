using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-002-LIVE-Zapping
public class LightSanity_002 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string FTA_1st_Mux_2;
    static string FTA_1st_Mux_3;
    static string Short_SD_1;
    static string Short_HD_1;

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
        this.AddStep(new Step3(), "Step 3: Zap Between The Two Channels 20 Times");
        this.AddStep(new Step4(), "Step 4: Tune to Other FTA Services Using P+ / P- Several Times");
        this.AddStep(new Step5(), "Step 5: Zap Between 2 Channels With Different Video Format MPEG2 and MPEG4");

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
            FTA_1st_Mux_2 = CL.EA.GetValue("FTA_1st_Mux_2");
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");
            Short_HD_1 = CL.EA.GetValue("Short_HD_1");


            // Change the channel banner timeout to 5 sec
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }

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

            res = CL.EA.CheckForVideo(true, false, 20);
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

            //Switch to another free to air service which is not the following channel:
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a Second FTA Channel With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On Second FTA Channel After DCA");
            }
            //TODO: Check audio, channel bar, event information
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

            int numOfCC = 20;
            //Zap between the two channels 20 times
            for (int i = 1; i < numOfCC; i += 2)
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to FTA Channel With DCA After " + i + " Channel Changes");
                }

                res = CL.EA.CheckForVideo(true, false, 20);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify Video is Present After " + i + " Channel Changes");
                }

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to a Second FTA Channel With DCA After " + (i + 1) + " Channel Changes");
                }

                res = CL.EA.CheckForVideo(true, false, 20);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify Video is Present After " + (i + 1) + " Channel Changes");
                }
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a Second FTA Channel With DCA After Channel Changes");
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present After Channel Changes");
            }
            //Tune to any free to air service using channel down and up
            //Do not use Fast Zapping! (you must do first channel down after DCA)
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With Channel Down");
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Service After Channel Down");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With Channel Up");
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Service After Channel Up");
            }

            PassStep();
        }
    }
    #endregion
    #region Step5
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Zap between 2 channels with different video format
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to MPEG2 Channel");
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On MPEG2 Channel");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to MPEG4 Channel");
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on MPEG4 Channel");
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