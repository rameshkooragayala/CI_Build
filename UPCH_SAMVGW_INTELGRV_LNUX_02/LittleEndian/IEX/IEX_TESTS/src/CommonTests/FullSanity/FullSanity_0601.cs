using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0601
public class FullSanity_0601 : _Test
{
    [ThreadStatic]
    static _Platform CL;



    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string FTA_1st_Mux_4;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-008-RB-Review BufferPlayback & trickmodes
        //Checks Review Buffer Basic Functionality & Trick Modes
        //- RB start
        //- All Trick Modes (Rewind, FF, Slow motion, Pause, Play 
        //- Skip in RB (Time-based)
        //- Time Shift Viewing, including after Channel Change
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not checking Event Info in the Channel Bar during RB PB. Not checking Audio. 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync and Standby On/Off");
        this.AddStep(new Step1(), "Step 1: Tune to a Channel, Pause, wait for Timeshifting to be Activated");
        this.AddStep(new Step2(), "Step 2: Trying All Trick Mode Speeds in RB");
        this.AddStep(new Step3(), "Step 3: Use Pause, Play & Slow Motion Trick Modes in RB");
        this.AddStep(new Step4(), "Step 4: Check Event Information is Available During RB Playback");
        this.AddStep(new Step5(), "Step 5: Stop RB Playback, Tune to Another Channel, Pause, and Play RB again");
        //this.AddStep(new Step6(), "Step 6: Skip in RB");

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
            FTA_1st_Mux_4 = CL.EA.GetValue("FTA_1st_Mux_4");


            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }
            //Better to run this test with new RB - after mount or standby
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.EA.CheckForVideo(false, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
            }

            CL.IEX.Wait(20);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.EA.CheckForVideo(true, false, 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present After Exiting From Standby");
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

            //Tune to a channel, press pause, wait for timeshifting to be activated
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(true, false, 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }


            double RB_InitialDepth = 180;
            CL.IEX.Wait(RB_InitialDepth);

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }

            //Wait until Channel Bar disappear
            CL.IEX.Wait(20);

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Once RB is long enough, use all trick mode speeds:
        //Use fast rewind (x2 x6 x12 x30) tricks modes,
        //Use fast forward (x2 x6 x12 x30) tricks mode,
        //Activate pause trick mode,
        //Activate play trick mode & slow motion
        public override void Execute()
        {
            StartStep();

            double EOFWait = 15;

            //Trickmodes, both directions, to BOF or EOF
            //(The slower speeds are checked first, before the RB becomes long, to shorten test duration)
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB at x(-2) Speed to BOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 2, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fast Forward the RB at x2 Speed to EOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -6, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB at x(-6) Speed to BOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 6, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fast Forward the RB at x6 Speed to EOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -12, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB at x(-12) Speed to BOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 12, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fast Forward the RB at x12 Speed to EOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -30, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB at x(-30) Speed to BOF");
            }

            CL.IEX.Wait(EOFWait);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 30, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fast Forward the RB at x30 Speed to EOF");
            }
            CL.IEX.Wait(EOFWait);

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

            //Pause & Play
            //For Pause and Play testing, need to enter the RB, Depth is irrelevant, as we will not reach EOF in any case.
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Trickmode Banner");
            }

            //res = CL.IEX.MilestonesEPG.Navigate("TRICKMODE BAR");
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the RB at Normal Speed");
            }


            //Slow Motion
            //For Slow Motion testing, need to be in the RB, Depth is irrelevant, but we are already in RB PB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0.5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the RB at Slow Motion Speed");
            }

            //(Slow motion will never get to EOF on its own, since EOF is moving forwards faster...)
            double slowMotionTestingDuration = 10;
            CL.IEX.Wait(slowMotionTestingDuration);

            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        //check event information is available during review buffer playback
        //***** TODO: Add verifications for event info when VerifyEventInfoFromBanner is ready! ****
        public override void Execute()
        {
            StartStep();


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

            //Stop the Review Buffer Playback
            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing after Slow Motion performed in RB");
            }

            //Tune to Another Channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Another Channel");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB");
            }
            CL.IEX.Wait(5);
            //Pause and Play Again
            res = CL.IEX.MilestonesEPG.Navigate("TRICKMODE BAR");
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause After Channel Change");
            }

            res = CL.IEX.MilestonesEPG.Navigate("TRICKMODE BAR");
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the RB at Normal Speed After Channel Change");
            }



            PassStep();
        }
    }
    #endregion

    #region Step6
    private class Step6 : _Step
    {
        //(verify\set skip interval)
        //Activate Skip backward
        //Activate Skip forward
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing");
            }

            res = CL.EA.STBSettings.SetSkipForwardInterval(EnumVideoSkip._30);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Skip Forward Interval");
            }

            res = CL.EA.STBSettings.SetSkipBackwardInterval(EnumVideoSkip._30);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Skip Backward Interval");
            }

            //Activate Skip backward
            //res = CL.EA.PVR.Skip(3, false,);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Skip Backwards");
            }

            //Activate Skip forward
            //res = CL.EA.PVR.Skip(2, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Skip Forwards");
            }

            //Verify Live Viewing is Normal After Skip in RB
            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing After Skip in RB");
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