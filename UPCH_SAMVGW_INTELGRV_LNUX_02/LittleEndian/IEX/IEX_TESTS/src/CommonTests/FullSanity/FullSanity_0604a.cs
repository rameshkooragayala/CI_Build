using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0604a : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_1;
    static string Medium_HD_1;
    static string Long_SD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File, Sync Stream");
        this.AddStep(new Step1(), "Step 1: Exit Standby and Tune to Channel A for Few Minutes");
        this.AddStep(new Step2(), "Step 2: Go to Non-Live TV Viewing, like Playback of Recorded Event");
        this.AddStep(new Step3(), "Step 3: Tune to a Channel and Playback the RB");
        this.AddStep(new Step4(), "Step 4: Standby for a Short Time and Tune Back to Live");


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
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Short_SD_1 =" + Short_SD_1);

            Medium_HD_1 = CL.EA.GetValue("Medium_HD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Medium_HD_1 =  " + Medium_HD_1);

            Long_SD_1 = CL.EA.GetValue("Long_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: : Long_SD_1 =  " + Long_SD_1);



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

            // Enter and Exit Standby, to make sure the RB is empty.
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
            }

            //Stay in Standby for a few seconds, to make sure RB was Deleted
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
            }

            //Tune to channel A for few minutes
            //(Prefferably, tune to a channel different from the one used for recording);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                // FailStep(CL,res, "Failed to Tune to Channel");
            }
            //Waiting for RB to have measurable content
            int Test_Wait = 120;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar");
            }

            //Verify RB has correct length
            res = CL.EA.VerifyEventDuration("RB", 110, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar");
            }

            res = CL.EA.VerifyEventDuration("RB", 240, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB is Not Too Long");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop RB Playback");
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

            // Tune to a channel and PB the RB
            //(Tune to a different channel than in step 1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_SD_1);
            if (!res.CommandSucceeded)
            {
                // FailStep(CL,res, "Failed to Tune to Channel");
            }

            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar after Channel Change");
            }
            //Verify RB was not deleted
            res = CL.EA.VerifyEventDuration("RB", 110, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB is Long Enough after PB");
            }
            //Verify RB was not recorded during the playbck of recorded content
            res = CL.EA.VerifyEventDuration("RB", 300, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB is Not Too Long after PB ");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop RB Playback ");
            }

            // Rewind the entire RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -12, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind RB to BOF ");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present after RB BOF Reached ");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop RB Playback after RB BOF Reached  ");
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

            //Go to standby for a short time and then tune back to Live
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
            }

            //Stay in Standby for a few seconds, to make sure RB was Deleted
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
            }

            //Remove any EPG screens
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing after Exit Standby ");
            }

            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause from Action Bar after Standby ");
            }

            //Verify RB was deleted after Standby
            res = CL.EA.VerifyEventDuration("RB", 60, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB is Not Too Long - Just Started ");
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