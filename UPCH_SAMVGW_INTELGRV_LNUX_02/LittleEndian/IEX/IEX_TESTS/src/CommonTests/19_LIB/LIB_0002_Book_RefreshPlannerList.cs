/// <summary>
///  Script Name : LIB_0002_Book_RefreshPlannerList.cs
///  Test Name   : LIB-0002-Book-Refresh Planner List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0002 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_Channel;
    private static string Name_TimeBased_Recorsing;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Verify Bookings In Planner");
        this.AddStep(new Step2(), "Step 2: Wait Till The EB_Rcording Start And Verfy In Planner");
        this.AddStep(new Step3(), "Step 3: Wait Till The TB_Rcording Start And Verfy In Planner");
        this.AddStep(new Step4(), "Step 4: Refresh Planner And Verfy Recordings Not Present");

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
            FTA_Channel = CL.EA.GetValue("FTA_Channel");
            Name_TimeBased_Recorsing = CL.EA.GetValue("Name_FTA_1st_Mux_2");

            res = CL.EA.PVR.RecordManualFromPlanner("TB_Rcording", Name_TimeBased_Recorsing, -1, 10, 30, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
            }

            res = CL.EA.TuneToChannel(FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune To Channel");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EB_Rcording", 1, 3, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Banner");
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
            StartStep();

            res = CL.EA.PVR.VerifyEventInPlanner("TB_Rcording", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: TB_Rcording");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EB_Rcording", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_Rcording");
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

            res = CL.IEX.Wait(60 * 8);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Wait Till  Events In Will Start");
            }
            /*
            //EA Work arund
            string TS="";
            CL.IEX.SendIRCommand("SELECT_LEFT", -1, ref TS);
            */
            res = CL.EA.PVR.VerifyEventInPlanner("TB_Rcording", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: TB_Rcording");
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

            res = CL.EA.PVR.VerifyEventInPlanner("EB_Rcording", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_Rcording");
            }

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
            res = CL.EA.PVR.VerifyEventInPlanner("EB_Rcording", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event NOT In Planner: EB_Rcording");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("TB_Rcording", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event NOT In Planner: TB_Rcording");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        Console.WriteLine("In test Post Execute");
    }

    #endregion PostExecute
}