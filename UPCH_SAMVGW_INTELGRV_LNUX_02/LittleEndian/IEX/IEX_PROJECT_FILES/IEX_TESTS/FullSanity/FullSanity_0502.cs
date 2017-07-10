using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0502-Booking Time Based Recording
public class FullSanity_0502 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Name_FTA_1st_Mux_1;
    static string FTA_1st_Mux_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0502-Booking Time Based Recording
        //Brief Description: Booking and Recording of Time-based Recording (Manual Recording);.
        //Precondition: Have Default settings.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not checking Audio. Not using all trick modes on recording (x2, x6, x12, x30, x(-2);, x(-6);, x(-12);, x(-30); and skip in both directions).
        //Testsing only one event, not SD + HD events.

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book a Time-Based Recording on Live");
        this.AddStep(new Step2(), "Step 2: Verify the Record Starts and Ends");
        this.AddStep(new Step3(), "Step 3: Playback the Record from Start to End Of File");

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
            Name_FTA_1st_Mux_1 = CL.EA.GetValue("Name_FTA_1st_Mux_1");
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");

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

            //Book a time-based recording on live
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            //Check enough time left for the event
            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 200)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            //(Start time is a few minutes into the future, to make sure the recording will not be over before the booking sequence will complete);
            res = CL.EA.PVR.RecordManualFromCurrent("MR1", FTA_1st_Mux_1, 3, EnumFrequency.ONE_TIME, false, false);
            //res = CL.EA.PVR.RecordManualFromPlanner("MR1", Name_FTA_1st_Mux_1, -1, 6, 4, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
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

            //Wait for the completion of timebased recording.
            res = CL.EA.WaitUntilEventEnds("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Recording Ends");
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

            //Playback the Record from Start to End Of File
            res = CL.EA.PVR.PlaybackRecFromArchive("MR1", 30, true, false);
            //res = CL.EA.PVR.PlaybackRecFromArchive("MR1", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the MR Event to EOF");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after MR Playback");
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
