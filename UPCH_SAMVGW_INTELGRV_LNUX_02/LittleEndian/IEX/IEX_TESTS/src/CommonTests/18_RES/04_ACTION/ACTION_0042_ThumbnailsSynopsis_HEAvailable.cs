/// <summary>
///  Script Name : ACTION_0042_ThumbnailsSynopsis_HEAvailable
///  Test Name   : ACTION-0042-Thumbnails Synopsis-PLB
///  TEST ID     : 64226
///  QC Version  : 1
///  Jira ID     : FC-514
///  Variations from QC: 1. Thumbnails & Synopsis unavailablity due to n/w issue or braodcast issue not tested
///                      2. Verification of Thumbnails & Synopsis on the programme where thumbnails & Synopsis are not available from HE is covered as part of test script ACTION_0042_ThumbnailsSynopsisPLB_HEUnavailable.cs
/// -----------------------------------------------
///  Scripted by : Madhu Renukaradhya
///  Last modified : 30 JULY 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0042_A")]
public class ACTION_0042_A : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceWithThumbnail;
    private static String defaultThumbnail;
    private static Boolean validThumbnail = true;
    private static int timeToPopulateThumbnail = 0;
    private static String obtainedThumbnail = "";
    private static String obtainedSynopsis = "";
    private static String expectedThumbnail = "";
    private static String expectedSynopsis = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File,Tune to service S1 having thumbnails and Synopsis and Record";
    private const string STEP1_DESCRIPTION = "Step 1: Launch action menu on playback of recorded content and verify Thumbnail & Synopsis is displayed.";

    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 3;//in mins
        public const int recordDuration = 2 * 60;//in mins
        public const int secToPlay = 0;
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

            //Get Values From ini File
            serviceWithThumbnail = CL.EA.GetServiceFromContentXML("Type=Video;HasThumbnail=True;HasSynopsis=True;IsEITAvailable=True;IsRecordable=True", "ParentalRating=High");
            if (serviceWithThumbnail == (null))
            {
                FailStep(CL, "Failed to fetch the Service With Thumbnail from content xml");
            }
            else
            {
                LogCommentInfo(CL, "Service With Thumbnail is: " + serviceWithThumbnail.LCN);
            }

            defaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
            LogCommentInfo(CL, "Default thumbnail value fetched from project ini is:  " + defaultThumbnail);

            timeToPopulateThumbnail = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "TIME_TO_POPULATE"));
            LogCommentInfo(CL, "Value of time to polulate thumbnail fetched from project ini is: " + timeToPopulateThumbnail);

            //Tune to Channel S1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithThumbnail.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to serviceWithThumbnail " + serviceWithThumbnail.LCN);
            }

            //Record event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent", Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }

            //Wait to Record for 2 mins
            res = CL.IEX.Wait(Constants.recordDuration);
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "Failed during wait on record");
            }

            //Fetch the thumbnail and synopsis during recording
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu during recording");
            }

            res = CL.IEX.Wait(timeToPopulateThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed while waiting for populating thumbnail", false);
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out expectedThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail during recording from Action Menu: " + expectedThumbnail);
            }

            //Validate the expected Thumbnail

            if (expectedThumbnail.Equals(defaultThumbnail) || string.IsNullOrEmpty(expectedThumbnail))
            {
                FailStep(CL, res, "Expected thumbnail not recieved from action menu during recording : " + expectedThumbnail, false);
                validThumbnail = false;
            }
            else
            {
                LogCommentInfo(CL, "Thumbnail displayed on Action Menu during recording " + expectedThumbnail);
            }

            //Fetching Synopsis on recording

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out expectedSynopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display synopsis during recording from Action Menu:" + expectedSynopsis);
            }

            if (string.IsNullOrEmpty(expectedSynopsis))
            {
                FailStep(CL, res, "Synopsis not available during recording from Action Menu ");
            }
            else
            {
                LogCommentInfo(CL, "Synopsis displayed on Action Menu " + expectedSynopsis);
            }
            if (validThumbnail.Equals(false))
            {
                FailStep(CL, res, "Failed to display thumbnail during recording from Action Menu: " + validThumbnail);
            }

            res = CL.EA.PVR.StopRecordingFromBanner("RecEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
            }

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

            //Playback the Already Recorded Event from Archive.
            res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording from archive");
            }

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to clear EPG info", false);
            }

            //Launch Action Bar on PLAYBACK
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu during playback");
            }

            res = CL.IEX.Wait(timeToPopulateThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed while waiting for populating thumbnail", false);
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail during playback from Action Menu: " + obtainedThumbnail);
            }

            //Validate for obtained Thumbnail

            if (obtainedThumbnail.Equals(defaultThumbnail) || string.IsNullOrEmpty(obtainedThumbnail))
            {
                FailStep(CL, res, "Thumbnail on playback from action menu not recieved : " + obtainedThumbnail, false);
                validThumbnail = false;
            }
            else
            {
                LogCommentInfo(CL, "Obtained thumbnail is: " + obtainedThumbnail);
                LogCommentInfo(CL, "Expected thumbnail is: " + expectedThumbnail);
                LogCommentInfo(CL, "Vaild Thumbnail is present");
            }
            //Fetching Synopsis on recording

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedSynopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display synopsis during playback from Action Menu:" + obtainedSynopsis);
            }

            if (string.IsNullOrEmpty(obtainedSynopsis))
            {
                FailStep(CL, res, "Synopsis not available during recording from Action Menu");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Synopsis is: " + obtainedSynopsis);
                LogCommentInfo(CL, "Expected Synopsis is: " + expectedSynopsis);

                if (!expectedSynopsis.Equals(obtainedSynopsis))
                {
                    FailStep(CL, "Obtained synopsis is not same as expected synopsis");
                }
                else
                {
                    LogCommentInfo(CL, "Valid Snopsis displayed on Action Menu " + obtainedSynopsis);
                }
            }
            if (!validThumbnail)
            {
                FailStep(CL, res, "Failed to display thumbnail during recording from Action Menu: " + validThumbnail);
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
        IEXGateway._IEXResult res;

        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to Delete recording from Archieve");
        }
    }

    #endregion PostExecute
}