/// <summary>
///  Script Name : REC_0204_EventBased_InterruptionByUser.cs
///  Test Name   : REC-0204-Event Based-Interruption By User Stopped
///  TEST ID     : 61075
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_0204 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string serviceToBeRecorded; //The service where recordings will happen
    private static string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public static int minMinRequiredInEvent = 5;
        public const int secToWaitAfterStartOfRecord = 60;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Start recording on the tuned service");
        this.AddStep(new Step2(), "Step 2: After some time stop the recording");
        this.AddStep(new Step3(), "Step 3: Access the recording and browse content");

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
            serviceToBeRecorded = CL.EA.GetValue("Short_SD_1");

            //Tune to service to be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + serviceToBeRecorded);
            }

            //Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Check for video failed on service " + serviceToBeRecorded);
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

            //Record the current event
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minMinRequiredInEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service " + serviceToBeRecorded);
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

            //Wait for some time
            res = CL.IEX.Wait(Constants.secToWaitAfterStartOfRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of record!");
            }

            //Stop recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from archive!");
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

            //Check if the recording is partial
            res = CL.EA.PCAT.VerifyEventPartialStatus(eventToBeRecorded, "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Event recording is not marked as partial!");
            }

            //Playback event to check for glitches
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the event recording!");
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