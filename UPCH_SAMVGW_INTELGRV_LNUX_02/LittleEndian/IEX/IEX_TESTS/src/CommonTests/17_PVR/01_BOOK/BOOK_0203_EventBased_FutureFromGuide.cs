/// <summary>
///  Script Name : BOOK_0203_EventBased_FutureFromGuide.cs
///  Test Name   : BOOK-0203-Event Based-Future From Guide
///  TEST ID     : 61250
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0203 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string serviceToBeFutureRecorded; //The service to be future recorded

    //Shared members between steps
    private static string futureEventRecording = "FUTURE_EVENT"; //The future event to be recorded

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int numOfPressesForNextEvent = 2;
        public const int minTimeBeforeEventStart = 5;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync.");
        this.AddStep(new Step1(), "Step 1: Select a future event and book for recording");
        this.AddStep(new Step2(), "Step 2: Book the event for recording");
        this.AddStep(new Step3(), "Step 3: Select the program in recording list and look for its details");

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
            serviceToBeFutureRecorded = CL.EA.GetValue("Short_SD_1");

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

            //Tune to the service whose future event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeFutureRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeFutureRecorded);
            }

            //Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video not present!");
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

            //Book the future event for recording
            res = CL.EA.PVR.BookFutureEventFromGuide(futureEventRecording, serviceToBeFutureRecorded, Constants.numOfPressesForNextEvent, Constants.minTimeBeforeEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event for recording!");
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

            //Verify whether the recording is present in Recording list
            res = CL.EA.PVR.VerifyEventInPlanner(futureEventRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the recording in the Recording List");
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