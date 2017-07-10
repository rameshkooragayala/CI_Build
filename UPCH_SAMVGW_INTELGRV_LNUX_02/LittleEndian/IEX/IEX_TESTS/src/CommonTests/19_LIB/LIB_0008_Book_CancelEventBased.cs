/// <summary>
///  Script Name : LIB_0008_Book_CancelEventBased.cs
///  Test Name   : LIB-0008-Book-Cancel Event Based From Booking List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class LIB_0008 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service _1st_RecordingChannel;
    private static Service _2nd_RecordingChannel;
    private static Service _3rd_RecordingChannel;
    static string event1 = "EB_1";
    static string event2 = "EB_2";
    static string event3 = "EB_3";

    private static class Constants
    {
        public const int minTimeBeforeEventStart = 1; // In Minutes
        public const int numOfPresses = 5; // For booking a future event that will not start recording during test execution
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Verify Events On Planner");
        this.AddStep(new Step2(), "Step 2: Cancel One Of The Events");
        this.AddStep(new Step3(), "Step 3: Verify Only Canceled Event is Missing From Planner");

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
            _1st_RecordingChannel = CL.EA.GetServiceFromContentXML("IsRecordable=True", "ParentalRating=High");
            if (_1st_RecordingChannel == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            _2nd_RecordingChannel = CL.EA.GetServiceFromContentXML("IsRecordable=True", "ParentalRating=High;LCN=" + _1st_RecordingChannel.LCN);
            if (_2nd_RecordingChannel == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            _3rd_RecordingChannel = CL.EA.GetServiceFromContentXML("IsRecordable=True","ParentalRating=High;LCN=" + _1st_RecordingChannel.LCN +","+_2nd_RecordingChannel.LCN);
            if (_3rd_RecordingChannel == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            res = CL.EA.PVR.BookFutureEventFromGuide(event1, _1st_RecordingChannel.LCN, Constants.numOfPresses, Constants.minTimeBeforeEventStart, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide");
            }


            res = CL.EA.PVR.BookFutureEventFromGuide(event2, _2nd_RecordingChannel.LCN, Constants.numOfPresses, Constants.minTimeBeforeEventStart, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide(event3, _3rd_RecordingChannel.LCN, Constants.numOfPresses, Constants.minTimeBeforeEventStart, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide");
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

            res = CL.EA.PVR.VerifyEventInPlanner(event1, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_1");
            }

            res = CL.EA.PVR.VerifyEventInPlanner(event2, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_2");
            }

            res = CL.EA.PVR.VerifyEventInPlanner(event3, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_3");
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

            res = CL.EA.PVR.CancelBookingFromPlanner(event1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Cancel Booking From Planner EB_1");
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

            res = CL.EA.PVR.VerifyEventInPlanner(event1, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_1");
            }

            res = CL.EA.PVR.VerifyEventInPlanner(event2, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_3");
            }

            res = CL.EA.PVR.VerifyEventInPlanner(event3, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_2");
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {

        IEXGateway._IEXResult res;

        //Delete all recordings in planner
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }

    }

    #endregion PostExecute
}