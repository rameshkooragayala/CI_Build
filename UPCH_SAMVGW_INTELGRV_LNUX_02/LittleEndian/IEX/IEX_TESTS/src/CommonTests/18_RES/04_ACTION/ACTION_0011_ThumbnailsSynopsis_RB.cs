/// <summary>
///  Script Name : ACTION_0011_ThumbnailsSynopsis_RB
///  Test Name   : ACTION-0011-Thumbnails Synopsis-Playback From RB
///  TEST ID     :
///  JIRA ID     : FC-499
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by          : SHRUTHI H M
///  Last Modified Date   : 17/07/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class ACTION_0011 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Variables used in the test
    private static Service serviceWithThumbnailSynopsis;
    private static String defaultThumbnail;
    private static Boolean hasThumbnail = true;
    private static int defaultTimeToCheckForVideo;  // In Seconds

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const bool checkEOF = false;
        public const double pause = 0;
        public const double play = 1;
        public const int secToWaitInPause = 60; // In Seconds
        public const int secToWaitForThumbnail = 5; // In Seconds
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps of the test case
        this.AddStep(new PreCondition(), "Precondition: Tune to a service and have one event in RB");
        this.AddStep(new Step1(), "Step 1: Access event of service from RB.Launch action menu and verify Thumbnail & Synopsis is displayed");

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

            serviceWithThumbnailSynopsis = CL.EA.GetServiceFromContentXML("Type=Video;HasThumbnail=True;HasSynopsis=True", "ParentalRating=High");
            LogComment(CL, "Retrieved Value From XML File: serviceWithThumbnailSynopsis = " + serviceWithThumbnailSynopsis.Name);

            // Fetch the default Thumbnail
            defaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
            if (String.IsNullOrEmpty(defaultThumbnail))
            {
                FailStep(CL, "Default thumbnail name not present in Project.ini file.");
            }

            //Fetch the default time to check for video
            String defaultVideoCheck = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultVideoCheck))
            {
                FailStep(CL, "Default Video check time not present in Project.ini file.");
            }
            defaultTimeToCheckForVideo = int.Parse(defaultVideoCheck);

            //Tune to a service with Thumbnail and Synopsis
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithThumbnailSynopsis.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service - " + serviceWithThumbnailSynopsis.LCN);
            }

            //Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, defaultTimeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Check for video failed on service " + serviceWithThumbnailSynopsis.LCN);
            }

            // Pause Video during Live viewing
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.pause, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause video");
            }

            LogComment(CL, "Wait for " + Constants.secToWaitInPause + " seconds so that RB can grow");
            // Wait for sometime so that RB can grow
            res = CL.IEX.Wait(Constants.secToWaitInPause);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in Pause!");
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

            // Playback from RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, Constants.checkEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback from RB");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to clear EPG Info");
            }

            // Launch Action Menu
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu");
            }

            LogComment(CL, "Wait for " + Constants.secToWaitForThumbnail + " seconds for Thumbnail and Synopsis to load.");
            // Wait sometime for Thumbnail and Synopsis to Load
            res = CL.IEX.Wait(Constants.secToWaitForThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after launching Action Menu!");
            }

            //Validate for Thumbnail
            String thumbnail = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out thumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get thumbnail_url from Action Menu");
            }

            if (string.IsNullOrEmpty(thumbnail))
            {
                FailStep(CL, res, "Thumbnail not available in Action Menu ", false);
                hasThumbnail = false;
            }
            else if (thumbnail.Equals(defaultThumbnail))
            {
                FailStep(CL, res, "Default thumbnail is displayed:" + thumbnail, false);
                hasThumbnail = false;
            }
            else
            {
                LogComment(CL, "Thumbnail displayed on Action Menu " + thumbnail);
            }

            //Validate for Synopsis
            String synopsis = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out synopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get synopsis from Action Menu");
            }

            if (string.IsNullOrEmpty(synopsis))
            {
                FailStep(CL, res, "Synopsis not available in Action Menu" + synopsis);
            }
            else
            {
                LogComment(CL, "Synopsis displayed on Action Menu " + synopsis);
            }
            if (hasThumbnail.Equals(false))
            {
                FailStep(CL, res, "Failed to get thumbnail from Action Menu");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps
}