﻿/// <summary>
///  Script Name : BOOK_0808_Modify_TimeBaseRecurring.cs
///  Test Name   : BOOK-0808-Modify-Time Base Recurring
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0808 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Book Future Time Base Recording From Planner With Frequency Daily");
        this.AddStep(new Step2(), "Step 2: Modify Start Time & End Time To Time Base Recurring");
        this.AddStep(new Step3(), "Step 3: Verify That Modification of Time Base Recurring Succeed");

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

            FTA_1st_Mux_1 = CL.EA.GetValue("NAME_FTA_1st_Mux_1");

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

            res = CL.EA.PVR.RecordManualFromPlanner("MR1", FTA_1st_Mux_1, "13:00", 60, 1, EnumFrequency.DAILY, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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

            res = CL.EA.PVR.ModifyManualRecording("MR1", "00:10", "00:40", "", 0, EnumFrequency.DAILY);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.VerifyRecurringBookingInPlanner("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}