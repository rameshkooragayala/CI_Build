/// <summary>
///  Script Name : LIB_0016_Book_DeleteAll.cs
///  Test Name   : LIB-0016-Book-Delete All in Planner
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0016 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Name_For_MR_1_Booking;
    private static string Name_For_MR_2_Booking;
    private static string Channel_For_EB_1_Booking;
    private static string Channel_For_EB_2_Booking;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Verify Recordings On Planner");
        this.AddStep(new Step2(), "Step 2: Go To Delete All Recording And Dont Delete");
        this.AddStep(new Step3(), "Step 3: Delete All Recording From Planner ");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    public override void PreExecute()
    {
        base.PreExecute();
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            Name_For_MR_1_Booking = CL.EA.GetValue("Name_FTA_1st_Mux_1");
            Name_For_MR_2_Booking = CL.EA.GetValue("Name_FTA_1st_Mux_2");
            Channel_For_EB_1_Booking = CL.EA.GetValue("Long_HD_1");
            Channel_For_EB_2_Booking = CL.EA.GetValue("Long_SD_2");

			res = CL.EA.STBSettings.SetGuardTime(true, "NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + "NONE");
            }
			
            // Time Based Recordings
            res = CL.EA.PVR.RecordManualFromPlanner("MR_1", Name_For_MR_1_Booking, -1, 20, 30, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book First Manual Recording");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("MR_2", Name_For_MR_2_Booking, -1, 20, 30, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Secend Manual Recording");
            }

            // Event Based Recordings
            res = CL.EA.TuneToChannel(Channel_For_EB_1_Booking);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune To Channel");
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("EB_1", Channel_For_EB_1_Booking, 3, 5, false);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Future Event From Guide");
            }

            res = CL.EA.TuneToChannel(Channel_For_EB_2_Booking);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune To Channel");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide("EB_2", Channel_For_EB_2_Booking, 1, 5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Future Event From Guide");
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

            res = CL.EA.PVR.VerifyEventInPlanner("MR_1", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording MR_1 on Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("MR_2", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording MR_2 on Archive");
            }
            res = CL.EA.PVR.VerifyEventInPlanner("EB_1", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording EB_1 on Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EB_2", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording EB_2 on Archive");
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY PLANNER");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MY PLANNER");
            }

            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR/DELETE ALL/NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Delete Menu And Select NO");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("MR_1", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording MR_1 on Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("MR_2", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording MR_2 on Archive");
            }
            res = CL.EA.PVR.VerifyEventInPlanner("EB_1", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording EB_1 on Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EB_2", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording EB_2 on Archive");
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

            res = CL.EA.PVR.CancelAllBookingsFromPlanner();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To  Cancel All Bookings From Planner");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("MR_1", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to NOT! Find Recording MR_1 on Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("MR_2", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to NOT! Find Recording MR_2 on Archive");
            }
            res = CL.EA.PVR.VerifyEventInPlanner("EB_1", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to NOT! Find Recording EB_1 on Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EB_2", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to NOT! Find Recording EB_2 on Archive");
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