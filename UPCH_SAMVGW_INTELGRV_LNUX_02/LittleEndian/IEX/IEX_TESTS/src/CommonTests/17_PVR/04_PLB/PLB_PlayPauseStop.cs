/// <summary>
///  Script Name : PLB_PlayPauseStop.cs
///
///  Test Name | TEST ID : PLB-0200-Play Pause Stop-Clear
///  Test Name | TEST ID : PLB-0201-Play Pause Stop-Scrambled
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PLB_PlayPauseStop : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static string ChannelName = "";
    private static string EventName = "Event";
    private static int EventDuration = 30;
    private static int PLBduration = 30;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync");
        this.AddStep(new Step1(), "Step 1: Record Time-Based Event from Planner");
        this.AddStep(new Step2(), "Step 2: Verify Recordings");
        this.AddStep(new Step3(), "Step 3: Verify Pause & Stop Playback");

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
            ChannelName = CL.EA.GetTestParams("SERVICE_TYPE");

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

            //Record time based recordig from planner
            res = CL.EA.PVR.RecordManualFromPlanner(EventName, ChannelName, -1, 3, EventDuration, EnumFrequency.ONE_TIME, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
            }
            //need to wait until event end
            CL.IEX.Wait(EventDuration * 60 + 180);

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

            //Verify recordig on library
            res = CL.EA.PVR.VerifyEventInArchive(EventName, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Recording in Archive");
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

            res = CL.EA.PVR.PlaybackRecFromArchive(EventName, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + EventName + " From Archive");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present During Playback");
            }

            res = CL.EA.PVR.SetTrickModeSpeed(EventName, 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause Playback");
            }
            CL.IEX.Wait(PLBduration);

            //Stop Playing
            res = CL.EA.PVR.StopPlayback(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Playback");
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