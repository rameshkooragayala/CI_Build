/// <summary>
///  Script Name : LIB_1200_Nav_InfoRecordings.cs
///  Test Name   : LIB-1200-Information on Event Based Recordings
///  TEST ID     : 61954
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_1200 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceToBeRecorded; //Service to be recorded
    private static string eventToBeRecorded = "CURRENT_EVENT"; //The current event to be recorded
    private static string manualRecording = "MANUAL_RECORDING"; //Manual recording key

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int minTimeBeforeEventEnd = 2;
        public const int secToWaitAfterStartOfRecord = minTimeBeforeEventEnd * 60;
        public const int daysDelay = 0;
        public const int minuteDelayInBeginning = 2;
        public const int durationInMinsOfManualRecord = 4;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition:  Get Channel Number. Have Event Based and Time Based Recording in Library");
        this.AddStep(new Step1(), "Step 1: Verify Event Recording is Present in Planner");
        this.AddStep(new Step2(), "Step 2: Verify Time Based Recording is Present in Planner");

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

            //Get Channel Values From ini File
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to fetch a service with the passed criteria!");
            }

            //Tune to service to be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + serviceToBeRecorded.LCN);
            }

            //Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Check for video failed on service " + serviceToBeRecorded.LCN);
            }

            //Record current event from banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to initiate recording of current event on service " + serviceToBeRecorded.LCN);
            }

            //Wait a few minutes on recording
            res = CL.IEX.Wait(Constants.secToWaitAfterStartOfRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after record start!");
            }

            //Stop recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from archive!");
            }

            //Schedule a time based recording
            res = CL.EA.PVR.RecordManualFromPlanner(manualRecording, serviceToBeRecorded.Name, Constants.daysDelay, Constants.minuteDelayInBeginning, Constants.durationInMinsOfManualRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule time based recording");
            }

            //Wait for sometime for the recording to happen
            res = CL.IEX.Wait((Constants.durationInMinsOfManualRecord + Constants.durationInMinsOfManualRecord) * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of record!");
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

            //Verify whether the event based recording is present in the library
            res = CL.EA.PVR.VerifyEventInArchive(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the event based recording in the library!");
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

            //Verify whether the manual recording is present in the library
            res = CL.EA.PVR.VerifyEventInArchive(manualRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the manual recording in the library!");
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

        //Delete all bookings from planner
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete all bookings from planner!");
        }

        //Delete all recordings from archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete all records from archive!");
        }
    }

    #endregion PostExecute
}