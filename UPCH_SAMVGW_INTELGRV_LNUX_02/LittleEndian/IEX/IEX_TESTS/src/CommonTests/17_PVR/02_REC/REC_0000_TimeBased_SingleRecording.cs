/// <summary>
///  Script Name : REC_0000_TimeBased_SingleRecording.cs
///  Test Name   : REC-0011-LightSanity-Time - based recording
///  TEST ID     : 59309
///  QC Version  : 2
/// ----------------------------------------------- 
///  Modified by : Bharath Pai
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class REC_Time_Based_Recording : _Test
{
[Thread Static]
     static _Platform CL;

    //Channels used by the test
    static string serviceToBeRecorded; //The service to be recorded
    static string eventToBeRecorded = "EVENT_TO_BE_RECORDED"; //The event to be recorded

    //Constants used in the test
    static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int daysDelay = 1;
        public const int minuteDelayInBeginning = 2;
        public const int durationInMinsOfManualRecord = 4;
        public const int secToWaitAfterStartOfRecord = 60;
        public const bool checkIfAudioIsPresent = true;
        public const int timeToCheckForAudio = 10;
        public const int minTimeBeforeEventEnd = 4;
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
    }

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync.");
        this.AddStep(new Step1(), "Step 1: Schedule a time based recording to happen in the required service.Wait for the recording to finish");
        this.AddStep(new Step2(), "Step 2: Playback the record");

        //Get Client Platform
	CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
StartStep();

            //Get Channel Values From ini File
            serviceToBeRecorded = CL.EA.GetValue("Short_SD_1");
            
	PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Tune to Channel - " + serviceToBeRecorded);
            }

            //Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Video not present!");
            }

            //Schedule a time based recording
            res = CL.EA.PVR.RecordManualFromPlanner(eventToBeRecorded, serviceToBeRecorded, Constants.daysDelay, Constants.minuteDelayInBeginning, Constants.durationInMinsOfManualRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to schedule time based recording");
            }

            //Wait for sometime for the recording to happen
            res = CL.IEX.Wait(Constants.secToWaitAfterStartOfRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to wait after start of record!");
            }

            PassStep();
        }
    }
    #endregion
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
                FailStep(CL,res, "Playback failed!!");
            }

            //Check whether the video is present
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Video not present on playback!!");
            }

            //Check if Audio is present
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Audio not present on playback!!");
            }

            //Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to stop playback!!");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

#region PostExecute
 public override void PostExecute()
{
}
#endregion
}