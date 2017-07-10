/// <summary>
///  Script Name : REC_EventBased_SingleRecording.cs
///
///  Test Name | TEST ID : REC-0010-Event Based-Single Recording    | 60828
///  Test Name | TEST ID : REC-0030-Video Format-MPEG4 HD           | 60832
///  Test Name | TEST ID : REC-0031-Video Format-MPEG4 SD           | 60833
///  Test Name | TEST ID : REC-0032-Video Format-MPEG2 HD           | 60834
///  Test Name | TEST ID : REC-0033-Video Format-MPEG2 SD           | 60835
///  Test Name | TEST ID : REC-0034-Video Format-MPEG4 3D           | 60840
///  Test Name | TEST ID : REC-0042-Audio Format-MPEG1              | 60836
///  Test Name | TEST ID : REC-0050-Event Based-FTA                 | 60841
///  Test Name | TEST ID : REC-0070-Event Based-Radio Channel       | 60842
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_EventBased_SingleRecording : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string serviceType;
    private static Service serviceToBeRecorded; //The service to be recorded
    private static string eventToBeRecorded = "EVENT_TO_BE_RECORDED"; //The event to be recorded

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const bool checkIfAudioIsPresent = true;
        public const int timeToCheckForAudio = 10;
        public const int minTimeBeforeEventEnd = 4;
        public const int secToWaitForRecord = 60;
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync.");
        this.AddStep(new Step1(), "Step 1: Schedule a recording to happen in the required service.Wait for the recording to finish");
        this.AddStep(new Step2(), "Step 2: Playback the record");

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
            serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML(serviceType, "ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to fetch the service matching the given criteria.");
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

            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded.LCN);
            }

            //Check for video if it is a video service
            if (serviceToBeRecorded.Type == "Video")
            {
                LogCommentInfo(CL,"Checking for video as it is a video service..");
                res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Video not present on service - " + serviceToBeRecorded.LCN);
                }
            }

            //Check if Audio is present
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on service - " + serviceToBeRecorded.LCN);
            }

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service - " + serviceToBeRecorded.LCN);
            }

            //Wait for some time
            res = CL.IEX.Wait(Constants.secToWaitForRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of recording!");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    public class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.valueForFullPlayback, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Playback of event failed.");
            }

            //Check whether the video is present
            if (serviceToBeRecorded.Type == "Video")
            {
                LogCommentInfo(CL, "Checking for video as it is a video service..");
                res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Video not present on playback!!");
                }
            }

            //Check if Audio is present
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on playback!!");
            }

            //Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback!!");
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

        //Delete all bookings from Planner
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to delete all bookings from planner!");
        }

        //Delete all records from Archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to delete all records from archive!");
        }

    }

    #endregion PostExecute
}