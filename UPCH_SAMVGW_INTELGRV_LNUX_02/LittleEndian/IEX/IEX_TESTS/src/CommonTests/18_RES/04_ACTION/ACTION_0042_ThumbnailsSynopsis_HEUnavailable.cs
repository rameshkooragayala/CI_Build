/// <summary>
///  Script Name : ACTION_0042_ThumbnailsSynopsis_HEUnavailable
///  Test Name   : ACTION-0042-Thumbnails Synopsis-PLB
///  TEST ID     : 64226
///  QC Version  : 1
///  Jira ID     : FC-514
///  Variations from QC: 1. Thumbnails & Synopsis unavailablity due to n/w issue or braodcast issue not tested
///                      2. Verification of Thumbnails & Synopsis on the programme where thumbnails & Synopsis are available from HE is covered as part of test script ACTION_0042_ThumbnailsSynopsisPLB_HEAvailable
/// -----------------------------------------------
///  Scripted by : Madhu Renukaradhya
///  Last modified : 31 JULY 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0042_B")]
public class ACTION_0042_B : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceWithoutThumbnail;
    private static String defaultThumbnail;
    private static Boolean validThumbnail = false;
    private static int timeToPopulateThumbnail = 0;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File,Tune to service S1 without thumbnails and Synopsis and Record";
    private const string STEP1_DESCRIPTION = "Step 1:Launch action menu on playback of recorded content and verify Thumbnail & Synopsis is displayed.";

    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 3;
        public const int recordDuration = 2 * 60;
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
            serviceWithoutThumbnail = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;IsRecordable=True", "HasThumbnail=True;HasSynopsis=True;ParentalRating=High");
            if (serviceWithoutThumbnail == (null))
            {
                FailStep(CL, "Failed to fetch the Service Without Thumbnail from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "Service Without Thumbnail is: " + serviceWithoutThumbnail.LCN);
            }

            defaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
            LogCommentInfo(CL, "Default thumbnail value fetched from project ini is:  " + defaultThumbnail);

            timeToPopulateThumbnail = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "TIME_TO_POPULATE"));
            LogCommentInfo(CL, "Value of time to polulate thumbnail fetched from project ini is: " + timeToPopulateThumbnail);

            //Tune to Channel S1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithoutThumbnail.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to serviceWithoutThumbnail " + serviceWithoutThumbnail.LCN);
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
                FailStep(CL, res, "Failed to launch Action Menu on Playback");
            }

            res = CL.IEX.Wait(timeToPopulateThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed while waiting for populating thumbnail");
            }

            String obtainedThumbnail = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail on playback from Action Menu: " + obtainedThumbnail);
            }

            //Validate for Thumbnail

            if (obtainedThumbnail.Equals(defaultThumbnail) || string.IsNullOrEmpty(obtainedThumbnail))
            {
                LogCommentInfo(CL, "Thumbnail not available : " + obtainedThumbnail);
            }
            else
            {
                validThumbnail = true;
                FailStep(CL, res, "Thumbnail displayed on Action Menu " + obtainedThumbnail, false);
                
            }

            //Validate for Synopsis.
            String obtainedSynopsis = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedSynopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display synopsis on PLB from Action Menu:" + obtainedSynopsis);
            }

            if (string.IsNullOrEmpty(obtainedSynopsis))
            {
                LogCommentInfo(CL, "Synopsis not available on playback from Action Menu");
            }
            else
            {
                FailStep(CL, res, "Snopsis displayed on Action Menu: " + obtainedSynopsis);
            }
            if (validThumbnail)
            {
                FailStep(CL, res, "Thumbnail is displayed on playback from Action Menu: " + validThumbnail);
            }
            res = CL.IEX.MilestonesEPG.Navigate("STOP");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to Stop recording from Archieve");
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
        res = CL.EA.PVR.NavigateToArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to navigate Archieve");
        }

        res = CL.EA.PVR.DeleteRecordFromArchive("RecEvent");
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to Delete recording from Archieve");
        }
    }

    #endregion PostExecute
}