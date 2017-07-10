/// <summary>
///  Script Name : PVREPG_0883_Cancel_Record_Series
///  Test Name   : PVREPG-0883-Cancel Record on a Series Event
///  TEST ID     : 74674
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 17/09/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0883 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();
    private static String cancelSeries = "";
    private static Boolean cancelAll;

    private static class Constants
    {
        public const int minTimeForEventToEnd = 4;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        
        //Pre-conditions: There is currently a record on-going on channel S1 and also future event is booked
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1");
        this.AddStep(new Step1(), "Step 1: Book a series from Action Bar using REC key ");
        this.AddStep(new Step2(), "Step 2: Cancel the booking of a single/All events of the Entire Series");
        this.AddStep(new Step3(), "Step 3: Verify that Single/All events are cancelled successfully");

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

            //Get Values From xml File, +ve criteria as IsEIT, video, IsSeries as True
            recordingService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video;IsSeries=True", "IsDefault=True;ParentalRating=High");
            if (recordingService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }


            cancelSeries = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "CANCEL_ALL");

            cancelAll = Convert.ToBoolean(cancelSeries);

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
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

            //Book a complate Series using REC key
            res = CL.EA.PVR.RecordUsingRECkey(EnumRecordIn.ActionBar, "BookSeries", recordingService1.LCN, Constants.minTimeForEventToEnd, IsSeries: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book complete series");
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

            res = CL.IEX.Wait(120);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait for 2 min");
            }

            if (cancelAll)
            {
                //Cancel Entire episode of Series
                res = CL.EA.PVR.StopRecordUsingStopKey(EnumRecordIn.ChannelBar, "BookSeries", recordingService1.LCN, IsSeries: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to cancel booking from Channel Bar ");
                }

            }
            else
            {
                //Stop current event only of series
                res = CL.EA.PVR.StopRecordUsingStopKey(EnumRecordIn.ChannelBar, "BookSeries", recordingService1.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to cancel booking from Channel Bar ");
                }
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            if (cancelAll)
            {
                res = CL.EA.PVR.VerifyEventInPlanner("BookSeries", SupposedToFindEvent: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Booking is still available in planner");
                }

            }
            else
            {
                res = CL.EA.PVR.VerifyEventInArchive("BookSeries");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify recording is in Archive");
                }
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

        //Cancel all booking from Planner
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to Cancel Booking because" + res.FailureReason);
        }

        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete recording because" + res.FailureReason);
        }
    }

    #endregion PostExecute
}