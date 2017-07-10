/// <summary>
///  Script Name : PLB_0420_ExitByZap.cs
///  Test Name   : PLB-0420-Exit by Channel Change
///  TEST ID     : 61124
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PLB_0420 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string RecChannel;
    private static string RecChannelType = "Medium_SD_1";
    private static string SurfChannel;
    private static string SurfChannelType = "Short_SD_1";
    private static string RecEventKey = "Event";
    private static int RecDurSec = 300;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Sync; have a Recording on the disk ");
        this.AddStep(new Step1(), "Step 1: Playback the Record ");
        this.AddStep(new Step2(), "Step 2: Perform Channel Change");

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
            RecChannel = CL.EA.GetValue(RecChannelType);
            SurfChannel = CL.EA.GetValue(SurfChannelType);

            //Record an event
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, RecChannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + RecChannel);
            }

            int recMin = RecDurSec / 60;
			
            //Record current event from banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner(RecEventKey, recMin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            CL.IEX.Wait(RecDurSec);

            res = CL.EA.PVR.StopRecordingFromArchive(RecEventKey, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from Archive");
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
            //Playback the event
            res = CL.EA.PVR.PlaybackRecFromArchive(RecEventKey, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event From Archive");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present During Playback");
            }

            CL.IEX.Wait(60);

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

            res = CL.EA.TuneToChannel(SurfChannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to " + SurfChannel + " from playback");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}