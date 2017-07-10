/// <summary>
///  Script Name : SET_Subtitle_DefaultDisable_PLB.cs
///  Test Name | TEST ID: SET-SUBT-0013-Playback-Subtitle default
///  Test Name | TEST ID: SET-SUBT-0015-Playback-Subtitle disable
///  Test Name | TEST ID: SET-SUBT-0019-Playback- subtitle format default
///  Test ID   : 71564,71565,71587
///  Test Repository : Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Developed by : Sandesh Mainkar.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("SET_Subtitle_DefaultDisable_PLB")]
public class SET_Subtitle_DefaultDisable_PLB : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    private static Service service;
    static String eventToBeRecorded = "EVENT TO BE RECORDED";
    static String SubtState = "";
    static String SubtDisable = "";


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Service From Content.xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Verify the Subtitles settings in the SETUP and playback any of the Recorded event.";
    private const string STEP2_DESCRIPTION = "Step 2: Verify that Subtitles are not getting displayed during the playback.";


    //Constants used in the test
    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 4;     // In minutes
        public const int waitForRecording = 4;          // In Minutes
        public const int secToPlay = 0;                 // amount of time to playback the Recorded event.
        public const Boolean playFromBeginning = true;  //Where to start the playback from.
        public const Boolean verifyEOF = false;         //Need to verify End-Of_File ?
    }

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From content.xml File
            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;NoOfSubtitleLanguages=0,1");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }

            //Tune to a service where Subtittles has to be verified.
            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }

            //If Subtitles option is not set to OFF by default, Set it to OFF.

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLE DISPLAY OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Switch Off the SUBTITLES under settings ");
            }

            //Now Record some event in any of the services.
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            LogCommentInfo(CL, "Waiting for " + Constants.waitForRecording + " minutes to ensure sufficient duration of recording to test all trickmodes");
            res = CL.IEX.Wait(Constants.waitForRecording * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required");
            }

            // Stop Recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            // playback the recordings from the Archive.
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.secToPlay, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + eventToBeRecorded + " From Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            // while the playback is going on, Verify the absence of Subtitles(Since it has been disabled by default)
            // Verification done by verifying the default option highlighted on the SETUP-SUBTITLES DISPLAY Option.
            //Navigate to STATE:SUBTITLES SETTING

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLES SETTING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to SUBTITLES DISPLAY under settings ");
            }

            CL.IEX.MilestonesEPG.GetEPGInfo("title", out SubtState);

            SubtDisable = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DISABLE");
            if (SubtDisable == "")
            {
                FailStep(CL, "Field DISABLE is not available in the SUBTITLE section.");
            }
            if (SubtState != SubtDisable)
            {
                FailStep(CL, res, "Subtitle was not disabled during the playback.");
            }


            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to return to playback because of the reason:" + res.FailureReason);
            }


            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        // Clean-up the recorded events from the disk.
        IEXGateway._IEXResult res;
        String defSubtDisp = "";

        //Delete all recordings in archive
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to stop the playback because of the reason:" + res.FailureReason);
        }
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLES SETTING");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to navigate to SUBTITLES DISPLAY under settings " + res.FailureReason);
        }
        defSubtDisp = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DEFAULT");
        if (defSubtDisp == "")
        {
            LogCommentFailure(CL, "Field DISABLE is not available in the SUBTITLE section.");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defSubtDisp);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Select the Default option under the Subtitles settings" + res.FailureReason);
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }
    }
    #endregion
}