/// <summary>
///  Script Name : RB_0010_SaveSDEvent
///  Test Name   : RB-0010-Save SD Event From RB
///  TEST ID     : 59176
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0010 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service SDService;
    private static String eventKeyName = "SDEvent";
    private static int videoCheckDuration;
    private static int audioCheckDuration;
    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 180;	// In Seconds
        public const int pause = 0;
        public const int play = 1;
        public const int timeToWaitInRB = 60; // In Seconds
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Tune to SD Channel, Enter Review Buffer, Play it, Check A/V");
        this.AddStep(new Step2(), "Step 2: Save The Current Playing Event");
        this.AddStep(new Step3(), "Step 3: Check The Event Was Saved in Archive");

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
            SDService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;Resolution=SD", "ParentalRating=High");
            if (SDService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            videoCheckDuration = int.Parse(CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC"));
            audioCheckDuration = int.Parse(CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC"));

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

            //Tune to SD channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SDService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            int timeToEventEnd_sec = 0;

            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.pause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Wait until TM Bar disappear
            CL.IEX.Wait(10);

            //Check video paused
            res = CL.EA.CheckForVideo(false, false, videoCheckDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }
            //Check no audio
            res = CL.EA.CheckForAudio(false, audioCheckDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Audio is Paused");
            }

            CL.IEX.Wait(Constants.timeToWaitInRB);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            //Wait until TM Bar disappear
            CL.IEX.Wait(10);

            res = CL.EA.CheckForVideo(true, false, videoCheckDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present When Starting Playing RB");
            }
            //check for audio
            res = CL.EA.CheckForAudio(true, audioCheckDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Audio is Present When Starting Playing RB");
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

            //Book from banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventKeyName, -1, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Save RB Evnet");
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

            res = CL.EA.PVR.VerifyEventInArchive(eventKeyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verfiy SD Event on Archive");
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive(eventKeyName);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete event from Archive:" +res.FailureReason);
        }
    }

    #endregion PostExecute
}