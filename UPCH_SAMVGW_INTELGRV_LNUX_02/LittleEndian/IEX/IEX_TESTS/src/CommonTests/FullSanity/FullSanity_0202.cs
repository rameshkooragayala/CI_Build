using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0202-LIVE-Zapping_FTA-SCRAMBLED- P+P-&Numeric keys
public class FullSanity_0202 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FirstFTA;
    static string SecondFTA;
    static string FirstScrambled;
    static string SecondScrambled;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0202-LIVE-Zapping_FTA-SCRAMBLED- P+P-&Numeric keys
        //Performs live surfing between Free To Air services and scrambled services
        //Make sure to channel up/down not to the following service. (to avoid Fast Zapping)
        //Based on QualityCenter ...
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");

        this.AddStep(new Step1(), "Step 1: Tune to a free to air service Using DCA");
        this.AddStep(new Step2(), "Step 2: Switch to another free to air service Using DCA");
        this.AddStep(new Step3(), "Step 3: Tune to a scrambled service Using DCA");
        this.AddStep(new Step4(), "Step 4: Switch to another scrambled using channel P+P-");
        this.AddStep(new Step5(), "Step 5: Switch back to any free to air service");
        this.AddStep(new Step6(), "Step 6: Zap between 2 channels in different transponders");


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
            FirstFTA = CL.EA.GetValue("FTA_1st_Mux_1");
            SecondFTA = CL.EA.GetValue("FTA_1st_Mux_2");
            //FirstScrambled = CL.EA.GetValue("Short_SD_Scrambled_1");
            //SecondScrambled = CL.EA.GetValue("Short_HD_Scrambled_1");


            CL.IEX.LogComment("Retrieved Value From ini File: FirstFTA = " + FirstFTA);

            CL.IEX.LogComment("Retrieved Value From ini File: SecondFTA = " + SecondFTA);

            FirstScrambled = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Short_SD_Scrambled_1");
            if (FirstScrambled == "")
            {
                FailStep(CL, "Failed to get the Short_SD_Scrambled_1 value from test ini");
            }
            CL.IEX.LogComment("Retrieved Value From ini File: FirstScrambled = " + FirstScrambled);

            SecondScrambled = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Short_HD_Scrambled_1");
            if (SecondScrambled == "")
            {
                FailStep(CL, "Failed to get the Short_HD_Scrambled_1 value from test ini");
            }
            CL.IEX.LogComment("Retrieved Value From ini File: SecondScrambled = " + SecondScrambled);

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Tune to a Free Air Service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FirstFTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air Service : " + FirstFTA + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air Service : " + FirstFTA + " After DCA");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Switch to another free to air service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SecondFTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air Service : " + SecondFTA + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air Service : " + SecondFTA + " After DCA");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Tune to a scrambled service using DCA
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FirstScrambled);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Scrambled Service : " + FirstScrambled + " With DCA From Free Air");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Scrambled Service : " + FirstScrambled + " After DCA From Free Air");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        //Switch to another scrambled service SecondScrambled using channel P+P-
        //Check for video
        public override void Execute()
        {
            StartStep();
            /// waiting for EA !!!
            /*
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SecondScrambled);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Tune to Scrambled Service : " + SecondScrambled + " With DCA From Free Air");
            }
            */
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Scrambled Service : " + SecondScrambled + " After DCA From Free Air");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step5
    private class Step5 : _Step
    {
        //Switch back to any free to air service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SecondFTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air Service : " + SecondFTA + " With DCA From Scrambled");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air Service : " + SecondFTA + " After DCA From Scrambled");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step6
    private class Step6 : _Step
    {
        //Zap between 2 channels in different transponders
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FirstScrambled);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Scrambled Service : " + FirstScrambled + " With DCA From Free Air");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Scrambled Service : " + FirstScrambled + " After DCA From Free Air");
            }




            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SecondFTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air Service : " + SecondFTA + " With DCA From Scrambled");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air Service : " + SecondFTA + " After DCA From Scrambled");
            }
            //TODO: Check audio, channel bar, event information
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