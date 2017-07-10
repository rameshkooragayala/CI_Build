/// <summary>
///  Script Name : LIB_0013_Book_CancelTimeBased.cs
///  Test Name   : LIB-0013-Book-Cancel Time Based From Booking List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0013 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Name_For_First_Booking;
    private static string Name_For_Secend_Booking;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File , Sync And Book Events");
        this.AddStep(new Step1(), "Step 1: Cancel One Of The TB Recordings ");
        this.AddStep(new Step2(), "Step 2: Verify Recording Is Not On Planner");
        // this.AddStep(new Step3(), "Step 3: Book on-going  event  for recording form Guide");
        // this.AddStep(new Step4(), "Step 4: Access Information (Series) On My Recordings");

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
            Name_For_First_Booking = CL.EA.GetValue("Name_FTA_1st_Mux_1");
            Name_For_Secend_Booking = CL.EA.GetValue("Name_FTA_1st_Mux_2");

            res = CL.EA.PVR.RecordManualFromPlanner("First_MR", Name_For_First_Booking, -1, 30, 30, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book First Manual Recording");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("Secend_MR", Name_For_Secend_Booking, -1, 30, 30, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Secend Manual Recording");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            //Find One of the envents and cancel is

            StartStep();

            res = CL.EA.PVR.CancelBookingFromPlanner("First_MR", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed Cancel Booking Of Event From Planner");
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

            res = CL.EA.PVR.VerifyEventInPlanner("First_MR", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify First Manual Booking is Deleted From Recordings List");
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

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}