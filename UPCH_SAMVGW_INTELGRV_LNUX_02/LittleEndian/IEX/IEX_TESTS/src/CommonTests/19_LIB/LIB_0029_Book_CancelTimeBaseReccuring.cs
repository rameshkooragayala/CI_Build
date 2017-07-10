/// <summary>
///  Script Name : LIB_0029_Book_CancelTimeBaseReccuring.cs
///  Test Name   : LIB-0029-Book-Cancel TB Reccuring
///  TEST ID     : 61999
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0029 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string ChannelName;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:From the user bookings list, User is able to cancel any time-based recordings previously set..
        //Pre-conditions:User has booked a time-based recording with Daily recurrence:
        //- tR2 is the first item of a recurring time-based recording
        //- tR3, tR4, tR5 are the next items of the recurring time-base recording. (daily)
        //Based on QualityCenter test version 4
        //Variations from QualityCenter:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book recurring dayly MR, Verify Events On Planner");
        this.AddStep(new Step2(), "Step 2: Cancel One Of The Events, Verify Only Canceled Event is Missing From Planner ");
        this.AddStep(new Step3(), "Step 3: Cancel whole recurring MR, Verify Canceled and Missing From Planner");

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
            res = CL.EA.PVR.RecordManualFromPlanner("MR1", ChannelName, 1, -1, 60, EnumFrequency.DAILY, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Reccuring Recording from planner");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            res = CL.EA.PVR.VerifyRecurringBookingInPlanner("MR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Recurring Booking In Planner ");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
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

            string eventdetails1 = "";
            string eventdetails2 = "";

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY PLANNER");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  NavigateByName to Planner");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtdetails", out eventdetails1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  get event details from Planner");
            }

            res = CL.EA.PVR.CancelBookingFromPlanner("MR1", 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Cancel Booking From Planner MR1");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY PLANNER");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  NavigateByName to Planner");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtdetails", out eventdetails2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  get event details from Planner");
            }

            //Check that first occurence is not in planner
            if (eventdetails2 == eventdetails1)
            {
                FailStep(CL, res, "Failed to delete MR1 occurence 1 from Planner");
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
                FailStep(CL, res, "Failed to  Cancel All Bookings From Planner");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("MR1", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event Is not in Planner: MR11");
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