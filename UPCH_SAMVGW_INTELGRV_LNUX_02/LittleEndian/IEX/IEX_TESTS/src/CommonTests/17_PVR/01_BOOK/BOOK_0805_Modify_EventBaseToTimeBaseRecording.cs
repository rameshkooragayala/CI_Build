/// <summary>
///  Script Name : BOOK_0805_Modify_EventBaseToTimeBaseRecording.cs
///  Test Name   : BOOK-0805-Modify-Event Base To Time Base Recording
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.Tests.Engine;

public class BOOK_0805 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Book Future Event Base Recording");
        this.AddStep(new Step2(), "Step 2: Modify Event Base To Time Base Recording");
        this.AddStep(new Step3(), "Step 3: Verify That Modification of Event Base To Time Base Recording Succeed");

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

            res = CL.EA.PVR.BookFutureEventFromGuide("eR1", FTA_1st_Mux_1, 3, 10, false, true, false);
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

            res = CL.EA.PVR.ModifyManualRecording("eR1", "22:00", "23:00");
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

            res = CL.EA.PVR.VerifyEventInPlanner("eR1", true, true, "22:00", "23:00");
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