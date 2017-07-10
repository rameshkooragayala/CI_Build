using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0602
public class FullSanity_0602 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Long_SD_1;
    static string Long_HD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //'Brief Description: When Channel Change Occurs During Live TV Viewing, the Fusion-based Product Shall Append the Content of the New Channel to
        //'the Previously Recorded Review Buffer Content [10] Seconds after the Channel Change Occurs.
        //'The Test checks that the RB is indeed Appended after Channel Changes (between SD and HD channels);, and can be Rewinded to the very Start.

        //'Channel A is SD
        //'Channel B is HD

        //'Based on QualityCenter test version 2.
        //'Variations from QualityCenter: None


        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File");
        this.AddStep(new Step1(), "Step 1: Power On the STB, Tune to SD Channel A and wait 10 seconds.");
        this.AddStep(new Step2(), "Step 2: After 60 Seconds Tune to HD Channel B and Wait 10 Seconds");
        this.AddStep(new Step3(), "Step 3: After 60 Seconds Tune Again to SD Channel A and Wait 30 Seconds (to have some content in the RB);");
        this.AddStep(new Step4(), "Step 4: RB Rewinded to BOF");
        this.AddStep(new Step5(), "Step 5: Play the RB till EOF");

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
            Long_SD_1 = CL.EA.GetValue("Long_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Long_SD_1 = " + Long_SD_1);

            Long_HD_1 = CL.EA.GetValue("Long_HD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Long_HD_1 = " + Long_HD_1);


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

            //'Tune to channel A
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Tune to SD Channel");
            }

            //'Wait for RB to start
            int RB_Start_Time = 20;
            CL.IEX.Wait(RB_Start_Time);

            //'Verify RB started
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 5, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Started");
            }

            //res = CL.EA.PVR.StopPlayback(true);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,res, "Failed to Stop RB Playback ");
            //}	

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
            //'After 60 seconds tune to channel B and wait 10 seconds
            int Test_Wait = 60;
            CL.IEX.Wait(Test_Wait);

            // res = CL.EA.TuneToChannel(Long_HD_1);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to HD Channel");
            }

            //'Wait for RB to start on 2nd channel
            int RB_Start_Time = 10;
            CL.IEX.Wait(RB_Start_Time);

            //'Verify RB Appended
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on HD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 60, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Appended after One Channel Change");
            }

            //res = CL.EA.PVR.StopPlayback(true);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,res, " Failed to Stop RB Playback ");
            //}	

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
            //'After 60 seconds tune again to SD channel A and wait 30 seconds (to have some content in the RB);
            int Test_Wait = 60;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune Again to SD Channel ");
            }

            //'Wait 30 Seconds to have some content in the RB
            Test_Wait = 30;
            CL.IEX.Wait(Test_Wait);

            //''Verify RB Appended Again
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 140, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify RB Appended After Two Channel Changes");
            }

            //res = CL.EA.PVR.StopPlayback(true);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,res, " Failed to Stop RB Playback ");
            //}	  

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
            //'Rew the RB till the BOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -6, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to BOF");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify Video is Present after RB BOF Reached ");
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

            //''Verify RB Appended Again
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            int Test_Wait = 120;
            CL.IEX.Wait(Test_Wait);

            //'Play the RB in till EOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 6, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to FF the RB to EOF (and Catch UP to Live);  ");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present Catch UP to Live ");
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