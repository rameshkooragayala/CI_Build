using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0603
public class FullSanity_0603 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Medium_SD_1;
    static string Medium_HD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //'Check that RB is not growing more then Max RB depth both for persistent RB and for RB that is on the same channel.
        //'Max RB depth is 180 min.

        //'Precondition
        //'Channel A is SD
        //'Channel B is HD

        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File");

        this.AddStep(new Step1(), "Step 1: Power on the box, tune to SD channel A and wait 10 seconds.");
        this.AddStep(new Step2(), "Step 2: After 60 seconds tune to some HD channel B and wait 10 seconds");
        this.AddStep(new Step3(), "Step 3: After 60 seconds tune again to SD channel A and wait 30 seconds");
        this.AddStep(new Step4(), "Step 4: REW till BOF");
        this.AddStep(new Step5(), "Step 5: FF till EOF");
        this.AddStep(new Step6(), "Step 6: Power on the box, tune to SD channel A and wait 3000 seconds.");
        this.AddStep(new Step7(), "Step 7: Tune to some HD channel B and wait 3000 seconds");
        this.AddStep(new Step8(), "Step 8: Tune again to SD channel A and wait 4800 seconds");
        this.AddStep(new Step9(), "Step 9: REW till BOF");
        this.AddStep(new Step10(), "Step 10: FF till EOF");
        this.AddStep(new Step6(), "Step 11: Power on the box, tune to SD channel A and wait 10800 seconds.");
        this.AddStep(new Step9(), "Step 12: REW till BOF");
        this.AddStep(new Step10(), "Step 13: FF till EOF");

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
            CL.IEX.LogComment("Retrieved Value From ini File: Medium_SD_1 = " + Medium_SD_1);

            Medium_HD_1 = CL.EA.GetValue("Medium_HD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Medium_HD_1 = " + Medium_HD_1);


            // set the auto standby to off, in order to avoid standby while fill in the RB
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AUTO STANDBY menu");
            }
            PassStep();
        }
    }
    #endregion
    //'***** Steps are Not Clear.SetVerify before running the test.*****	
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Better to run this test with new RB - after mount or standby
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds, to make sure RB was Deleted
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            //'Tune to channel A
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Tune to SD Channel");
            }

            //'Wait for RB to start
            int RB_Start_Time = 10;
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_HD_1);
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
            //'After 60 seconds tune to channel A and wait 30 seconds
            int Test_Wait = 60;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to SD Channel");
            }

            //'Wait 30 seconds
            int RB_Start_Time = 30;
            CL.IEX.Wait(RB_Start_Time);

            //'Verify RB Appended
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 140, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Appended after 2 Channel Change");
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

            //'pause RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to EOF");
            }

            int Test_Wait = 60;
            CL.IEX.Wait(Test_Wait);

            //'FF the RB till the EOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 6, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to EOF");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify Video is Present after RB EOF Reached ");
            }
            PassStep();
        }
    }
    #endregion
    #region Step6
    private class Step6 : _Step
    {
        // repeat steps 1-5 with full RB (180 min = 10800 seconds)
        public override void Execute()
        {
            StartStep();

            //Better to run this test with new RB - after mount or standby
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds, to make sure RB was Deleted
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            //'Tune to channel A
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Tune to SD Channel");
            }

            //'Wait for RB to start
            int Test_Wait = 3000;
            CL.IEX.Wait(Test_Wait);

            //'Verify RB started
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 2990, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Started");
            }

            PassStep();
        }
    }
    #endregion
    #region Step7
    private class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to HD Channel");
            }

            //'Wait for RB to start on 2nd channel
            int Test_Wait = 3000;
            CL.IEX.Wait(Test_Wait);

            //'Verify RB Appended
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on HD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 5980, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Appended after One Channel Change");
            }

            PassStep();
        }
    }
    #endregion
    #region Step8
    private class Step8 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to SD Channel");
            }

            //'Wait rest of the 180 minutes 
            int Test_Wait = 4800;
            CL.IEX.Wait(Test_Wait);

            //'Verify RB Appended
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 10800, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Appended after 2 Channel Change");
            }

            PassStep();
        }
    }
    #endregion
    #region Step9
    private class Step9 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //'Rew the RB till the BOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -30, true);
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
    #region Step10
    private class Step10 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //'Rew the RB till the EOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 30, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to EOF");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify Video is Present after RB EOF Reached ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step11
    private class Step11 : _Step
    {
        // repeat steps 1-5 with full RB (180 min = 10800 seconds), without channel change
        public override void Execute()
        {
            StartStep();

            //Better to run this test with new RB - after mount or standby
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds, to make sure RB was Deleted
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            //'Tune to channel A
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Tune to SD Channel");
            }

            //'Wait for RB to start
            int Test_Wait = 10800;
            CL.IEX.Wait(Test_Wait);

            //'Verify RB started
            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar on SD Channel ");
            }

            res = CL.EA.VerifyEventDuration("RB", 10800, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB Started");
            }

            PassStep();
        }
    }
    #endregion

    #region Step12
    private class Step12 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //'Rew the RB till the BOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -30, true);
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
    #region Step13
    private class Step13 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //'Rew the RB till the EOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 30, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to EOF");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify Video is Present after RB EOF Reached ");
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