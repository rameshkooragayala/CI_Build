/// <summary>
///  Script Name : BOOK_0103_TimeBased_ReoccurringFromPlanner.cs
///  Test Name   : BOOK-0103-Time Based-Reoccurring From Planner
///  TEST ID     : 60967
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0103 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string ChannelName;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book a Daily Time-Based Recording on Planner");
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
            ChannelName = CL.EA.GetValue("Name_FTA_1st_Mux_4");

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
            //Book a time-based recording on planner
            res = CL.EA.PVR.RecordManualFromPlanner("MR1", ChannelName, "13:00", 60, 1, EnumFrequency.DAILY, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording from planner");
            }
            res = CL.EA.ReturnToLiveViewing();
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

            res = CL.EA.PVR.VerifyRecurringBookingInPlanner("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to found event time base recording  in planner ");
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