using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class FullSanity_0605 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static string FTA_1st_Mux_1;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File & SyncStream");
        this.AddStep(new Step1(), "Step 1: Enter and Exit Standby, to Make Sure RB is Empty");
        this.AddStep(new Step2(), "Step 2: Start RB and Play It Back");

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

            //Get Values from ini File
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");

            //check for enough time left on the event
            int timeToEventEnd_sec = 0;

            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 240)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
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

            // Enter and Exit Standby, to make sure the RB is empty.
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby ");
            }

            // Stay in Standby for a few seconds, to make sure RB was Deleted
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
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

            //Zap to a recordable channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Recordable Channel");
            }

            // Waiting for RB to have measurable content (About 2 min.);;
            int Test_Wait = 120;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.PVR.PauseFromActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar ");
            }

            //Verify RB has correct length
            res = CL.EA.VerifyEventDuration("RB", 110, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Action Bar ");
            }

            res = CL.EA.VerifyEventDuration("RB", 210, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify RB is Not Too Long (RB was NOT Deleted while in Standby ");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop RB Playback ");
            }


            // Rewind the entire RB at x(-2);; speed
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind RB to BOF ");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present after RB BOF ");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop RB Playback after RB BOF Reached ");
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
