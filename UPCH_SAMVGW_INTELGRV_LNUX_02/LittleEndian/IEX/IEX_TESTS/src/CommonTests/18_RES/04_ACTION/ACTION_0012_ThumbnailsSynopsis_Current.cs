/// <summary>
///  Script Name : ACTION_0012_ThumbnailsSynopsis_Current
///  Test Name   : ACTION-0012-Thumbnails Synopsis-Current
///  TEST ID     : 63801
///  QC Version  : 2
///  Jira ID     : FC-290
///  Variations from QC:None
/// -----------------------------------------------
///  Scripted by : Madhu Renukaradhya
///  Last modified : 04 JULY 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION-0012-Thumbnails Synopsis-Current")]
public class ACTION_0012 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceWithThumbnail;
    private static String DefaultThumbnail;
    private static Boolean isPass = true;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to service S1";
    private const string STEP2_DESCRIPTION = "Step 2: Launch action menu and verify Thumbnail & Synopsis is displayed";

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
            serviceWithThumbnail = CL.EA.GetServiceFromContentXML("Type=Video;HasThumbnail=True;HasSynopsis=True", "ParentalRating=High");
            if (serviceWithThumbnail.Equals(null))
            {
                FailStep(CL, "serviceWithThumbnail is null: " + serviceWithThumbnail);
            }
            else
            {
                LogCommentInfo(CL, "serviceWithThumbnail: " + serviceWithThumbnail.LCN);
            }

            DefaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");

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
            //Step 1: Tune to Channel S1
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithThumbnail.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + serviceWithThumbnail);
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present " + serviceWithThumbnail + " After DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu");
            }
            CL.IEX.Wait(5);

            String thumbnail = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out thumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get thumbnail_url from Action Menu");
            }

            //Validate for Thumbnail

            if (thumbnail.Equals(DefaultThumbnail) || string.IsNullOrEmpty(thumbnail))
            {
                FailStep(CL, res, "Valid thumbnail not recieved : " + thumbnail, false);
                isPass = false;
            }
            else
            {
                LogComment(CL, "Thumbnail displayed on Action Menu " + thumbnail);
            }

            //Validate for Synopsis.
            String synopsis = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out synopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get synopsis from Action Menu");
            }

            if (string.IsNullOrEmpty(synopsis))
            {
                FailStep(CL, res, "Snopsis not available in Action Menu" + synopsis);
            }
            else
            {
                LogComment(CL, "Snopsis displayed on Action Menu " + synopsis);
            }
            if (isPass.Equals(false))
            {
                FailStep(CL, res, "Failed to get thumbnail from Action Menu");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}