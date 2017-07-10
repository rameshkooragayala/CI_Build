/// <summary>
///  Script Name : LIB_1220_Nav_Subcategories.cs
///  Test Name   : LIB-1220-Subcategories
///  TEST ID     : 61963
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_1220 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string serviceToBeRecorded; //Service to be recorded
    private static string eventToBeRecorded = "CURRENT_EVENT"; //The current event to be recorded

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int minTimeBeforeEventEnd = 2;
        public const int secToWaitAfterStartOfRecord = minTimeBeforeEventEnd * 60;
        public const string navigationToMyRecordings = "STATE:MY LIBRARY";
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the channels.Have a event based recording each in the library.");
        this.AddStep(new Step1(), "Step 1: Access My Recordings");

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

            //Record current event from banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to initiate recording of current event on service " + serviceToBeRecorded);
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

            //Navigate to my recordings
            res = CL.IEX.MilestonesEPG.NavigateByName(Constants.navigationToMyRecordings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Could not navigate to the library!!");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}