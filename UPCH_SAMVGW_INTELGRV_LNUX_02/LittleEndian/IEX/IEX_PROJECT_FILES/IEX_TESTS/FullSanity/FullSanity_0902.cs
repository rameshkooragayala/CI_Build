using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class FullSanity_0902 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static string Name_FTA_1st_Mux_1;
    static string FTA_1st_Mux_1;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File & SyncStream");
        this.AddStep(new Step1(), "Step 1:Book a Time-Based Recording on Live");
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

            //Get Values from ini File
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

            //(Start time is a few minutes into the future, to make sure the recording will not be over before the booking sequence will complete);
            res = CL.EA.PVR.RecordManualFromPlanner("MR1", Name_FTA_1st_Mux_1, -1, 4, 4, EnumFrequency.ONE_TIME, false);
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

            res = CL.EA.PVR.VerifyEventInPlanner("MR1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fined Event in  Planner");
            }

            //Wait for the Record to Start and to be Performed
            res = CL.EA.WaitUntilEventStarts("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until MR Starts");
            }

            //Wait to be sure PCAT is updated
            int PCAT_Wait = 30;
            CL.IEX.Wait(PCAT_Wait);

            res = CL.EA.PVR.VerifyEventInPlanner("MR1", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fined Event NOT in Planner");
            }

            res = CL.EA.PVR.VerifyEventInArchive("MR1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Fined Event in Archive");
            }

            // Wait for the completion of timebased recording.
            res = CL.EA.WaitUntilEventEnds("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Recording Ends ");
            }
            // Wait to be sure PCAT is updated (after recording this might take a while);
            CL.IEX.Wait(PCAT_Wait);

            // Verify recording duration (as set in step 1 - MR have no GT);
            res = CL.EA.VerifyEventDuration("MR1", 210, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item is Long Enough ");
            }
            res = CL.EA.VerifyEventDuration("MR1", 300, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item is Not too Long ");
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

            //  Playback the Record from Start to End Of File
            res = CL.EA.PVR.PlaybackRecFromArchive("MR1", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the MR Event to EOF ");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after MR Playback ");
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
