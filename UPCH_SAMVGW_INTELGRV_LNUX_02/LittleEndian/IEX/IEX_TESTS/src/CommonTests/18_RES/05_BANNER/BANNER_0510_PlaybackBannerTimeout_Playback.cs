/// <summary>
///  Script Name : BANNER_0510_PlaybackBannerTimeout_Playback.cs
///  Test Name   : BANNER-0510-Playback Banner Timeout-Playback
///  TEST ID     : 63831
///  JIRA ID     : FC-240
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : SHRUTHI H M
///  Last Modified Date: 12/7/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BANNER_0510 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static EnumChannelBarTimeout bannerTimeoutToSet;
    private static EnumChannelBarTimeout bannerTimeoutDefault;
    private static string[] TM_REW = { "" };
    private static string[] TM_FF = { "" };
    private static Service service = new Service();
    private static string recordedEvent = "EVENT_TO_BE_RECORDED";

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = false;
        public const int timeToCheckForVideo = 10;	// In seconds
        public const int minTimeBeforeEventEnd = 9;	// In Minutes
        public const int minToWaitForRecord = 7;	// In minutes
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
        public const bool navigateToArchive = true;
        public const double pause = 0;
        public const double play = 1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Record current event from banner");
        this.AddStep(new Step1(), "Step 1: Playback the Recording from Archive and check for Playback banner dismissal at timeout");
        this.AddStep(new Step2(), "Step 2: Pause video and check for playback Banner");
        this.AddStep(new Step3(), "Step 3: Change Timeout Duration in Channel Bar Timeout settings");
        this.AddStep(new Step4(), "Step 4: Play Recording");
        this.AddStep(new Step5(), "Step 5: Apply FF Trickmodes");
        this.AddStep(new Step6(), "Step 6: Apply REW Trickmodes");

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

            string bannerTimeout = "";
            // Get required Channel Number and values from ini File
            service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out bannerTimeoutDefault);

            // Fetch another Banner timeout value <> Default value
            string[] banner_timeout_list = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "LIST").Split(',');

            if (banner_timeout_list[0].Equals(bannerTimeoutDefault))
            {
                bannerTimeout = banner_timeout_list[1];
            }
            else
            {
                bannerTimeout = banner_timeout_list[0];
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out bannerTimeoutToSet);
            TM_REW = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_REW").Split(',');
            TM_FF = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_FWD").Split(',');

            // Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }

            // Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video not present!");
            }

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(recordedEvent, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            //Wait for some time
            res = CL.IEX.Wait(Constants.minToWaitForRecord * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of recording!");
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

            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive(recordedEvent, Constants.valueForFullPlayback, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback event from Archive");
            }

            if (!CL.EA.UI.Utils.VerifyState("PLAYBACK", bannerTimeoutDefault.GetHashCode() + 1))
            {
                FailStep(CL, res, "Playback banner did not dismiss in timeout duration:" + bannerTimeoutDefault);
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

            // Pause video and check for playback Banner
            res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Constants.pause, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause playback");
            }

            //if (!CL.EA.UI.Utils.VerifyState("TRICKMODE BAR", bannerTimeoutDefault.GetHashCode() + 1))
            //{
            //    FailStep(CL, res, "Failed to get playback banner on pausing the video");
            //}

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

            // Change Timeout Duration in Channel Bar Timeout settings
            res = CL.EA.STBSettings.SetBannerDisplayTime(bannerTimeoutToSet);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change Banner Display Time to:" + bannerTimeoutToSet);
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive(recordedEvent, Constants.valueForFullPlayback, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback event from Archive!!");
            }

            if (!CL.EA.UI.Utils.VerifyState("PLAYBACK", bannerTimeoutToSet.GetHashCode() + 1))
            {
                FailStep(CL, res, "Playback banner did not dismiss in timeout duration:" + bannerTimeoutToSet);
            }

            PassStep();
        }
    }

    #endregion Step4

    #region Step5

    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Apply FF Trickmode and check for Playback Banner
            foreach (string TM in TM_FF)
            {
                res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Convert.ToDouble(TM), Constants.verifyEOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set playback TM to:" + TM);
                }
                if (CL.EA.UI.Utils.VerifyState("PLAYBACK", bannerTimeoutToSet.GetHashCode() + 1))
                {
                    FailStep(CL, res, "Playback banner got dismissed during Trickmode!");
                }
            }

            PassStep();
        }
    }

    #endregion Step5

    #region Step6

    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play Recording and check for Banner dismissal on new timeout
            res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Constants.play, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play video");
            }

            if (!CL.EA.UI.Utils.VerifyState("PLAYBACK", bannerTimeoutToSet.GetHashCode() + 1))
            {
                FailStep(CL, res, "Playback banner did not dismiss in timeout duration:" + bannerTimeoutToSet);
            }

            // Apply REW Trickmode and check for Playback Banner
            foreach (string TM in TM_REW)
            {
                res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Convert.ToDouble(TM), Constants.verifyEOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set playback TM to" + TM);
                }
                if (CL.EA.UI.Utils.VerifyState("PLAYBACK", bannerTimeoutToSet.GetHashCode() + 1))
                {
                    FailStep(CL, res, "Playback banner got dismissed during Trickmode!");
                }
            }

            PassStep();
        }
    }

    #endregion Step6

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Stop playback
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason);
        }

        // Set Banner Display Time to Default
        res = CL.EA.STBSettings.SetBannerDisplayTime(bannerTimeoutDefault);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason);
        }

        // Delete all recordings from Archive
        // TBD: Commenting out till EA is fixed.
        //res = CL.EA.PVR.DeleteAllRecordsFromArchive(Constants.navigateToArchive);
        //if (!res.CommandSucceeded)
        //{
        //LogCommentFailure(CL,res.FailureReason);
        //}
    }

    #endregion PostExecute
}