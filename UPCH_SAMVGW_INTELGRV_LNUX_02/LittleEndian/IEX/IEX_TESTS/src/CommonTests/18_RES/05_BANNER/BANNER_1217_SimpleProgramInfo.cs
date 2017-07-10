/// <summary>
///  Script Name : BANNER_1217_SimpleProgramInfo.cs
///  Test Name   : BANNER-1217-Simple Programme Info
///  TEST ID     : 63795
///  JIRA ID	 : FC-242
///  QC Version  : 1
///  Variations from QC: Exact record duration is not verified. Verified if its displayed
/// -----------------------------------------------
///  Modified by :
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("BANNER_1217_SimpleProgramInfo")]
public class BANNER_1217 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoService;
    private static string recordedEventName;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and record an event";
    private const string STEP1_DESCRIPTION = "Step 1: Verify event title and duration of the recording while on play back of record";

    private static class Constants
    {
        public const int recordDuration = 60; //in seconds
        public const int minTimeBeforeEvtEnd = 2; //in minutes
        public const int secToPlay = 0; //in seconds
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Channel From xml File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            LogCommentInfo(CL, "Retrieved Value From XML File: videoService = " + videoService);

            //Channel Surf
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Record current event from banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("RecordedEvent", Constants.minTimeBeforeEvtEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book current event for recording!");
            }

            //wait for 1 min
            res = CL.IEX.Wait(Constants.recordDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for 1 min");
            }

            //Stop Recording
            res = CL.EA.PVR.StopRecordingFromBanner("RecordedEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
            }

            //Get name of the record
            recordedEventName = CL.EA.GetEventInfo("RecordedEvent", EnumEventInfo.EventName);
            LogCommentInfo(CL, "Recorded Event Name is: " + recordedEventName);

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive("RecordedEvent", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Playback failed!!");
            }

            //Launch Action bar on lpay back
			res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch action bar on play back");
            }

            //Verify if the event title is displayed
            string eventTitle = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out eventTitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name");
            }

            if (!eventTitle.Equals(recordedEventName))
            {
                FailStep(CL, "Current playback eventName is not same as the recorded event name");
            }

            //Verify if the record duration is displayed
            string recordDuration = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out recordDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get record duration");
            }

            if (string.IsNullOrEmpty(recordDuration))
            {
                FailStep(CL, "Current playback eventDuration is null or empty" + recordDuration);
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}