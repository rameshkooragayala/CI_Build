/// <summary>
///  Script Name : BOOK_0102_TimeBased_ReoccurringFromLive.cs
///  Test Name   : BOOK-0102-Time Based-Reoccurring From Live
///  TEST ID     : 60966
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0102 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book a Daily Time-Based Recording on Live");
        this.AddStep(new Step2(), "Step 2: Verify the all Time-Base Recording on planner");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");

            PassStep();
        }
    }

    #endregion PreCondition

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
            res = CL.EA.PVR.RecordManualFromCurrent("MR1", FTA_1st_Mux_1, 3, EnumFrequency.DAILY, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Wait for the completion of timebased recording.
            res = CL.EA.PVR.VerifyRecurringBookingInPlanner("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Recording Ends");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}