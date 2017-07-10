/// <summary>
///  Script Name : PVREPG_0813_RecStopKey_Future_Series
///  Test Name   : PVREPG-0813-REC key stop-Future-Series
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 08/09/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0813 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();

    private static class Constants
    {
        public const int minTimeForEventToEnd = 3;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        
        //Pre-conditions: There is currently a record on-going on channel S1 and also future event is booked
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1");
        this.AddStep(new Step1(), "Step 1: Book a series from Channel Bar Next using REC key ");
        this.AddStep(new Step2(), "Step 2: Cancel the booking of a single event of the Entire Series from Channel Bar Next");

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
            res = CL.EA.PVR.RecordUsingRECkey(EnumRecordIn.ChannelBar, "BookSeries", recordingService1.LCN, Constants.minTimeForEventToEnd, false, IsCurrent: false, IsSeries: true);
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

            //Cancel a single event of complete Series
            res = CL.EA.PVR.StopRecordUsingStopKey(EnumRecordIn.ChannelBar, "BookSeries", recordingService1.LCN, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to cancel booking from Channel Bar ");
            }

            PassStep();
        }
    }

    #endregion Step2

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
    }

    #endregion PostExecute
}