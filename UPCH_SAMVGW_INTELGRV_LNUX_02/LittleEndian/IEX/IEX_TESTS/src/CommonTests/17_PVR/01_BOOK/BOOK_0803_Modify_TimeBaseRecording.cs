/// <summary>
///  Script Name : BOOK_0803_Modify_TimeBaseRecording.cs
///  Test Name   : BOOK-0803-Modify-Time Base Recording
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0803 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Book Future Time Base Recording From Planner With Frequency One");
        this.AddStep(new Step2(), "Step 2: Modify Start Time & End Time To Time Base Recording");
        this.AddStep(new Step3(), "Step 3: Verify That Modification of Time Base Recording Succeed");

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

            res = CL.EA.PVR.RecordManualFromPlanner("MR1", FTA_1st_Mux_1, "13:00", 60, 1, EnumFrequency.ONE_TIME, false, false);
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

            res = CL.EA.PVR.ModifyManualRecording("MR1", "22:00", "23:00");
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

            res = CL.EA.PVR.VerifyEventInPlanner("MR1", true, true, "22:00", "23:00");
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