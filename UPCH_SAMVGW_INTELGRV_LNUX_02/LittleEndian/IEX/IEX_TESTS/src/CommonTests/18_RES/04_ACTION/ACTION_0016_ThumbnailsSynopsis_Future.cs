/// <summary>
///  Script Name : ACTION_0016_ThumbnailsSynopsis_Future
///  Test Name   : ACTION-0016-ThumbnailsSynopsis-Future
///  TEST ID     : 68032
///  QC Version  : 3
///  Jira ID     : FC-549
///  Variations from QC:none
/// ----------------------------------------------- 
///  Scripted by : Madhu Renukaradhya
///  Last modified : 14 Aug 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0016")]
public class ACTION_0016 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static Service serviceWithThumbnail;
    static String defaultThumbnail;
    static Boolean validThumbnail = true;
    static int timeToPopulateThumbnail;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to service S1 having thumbnails and Synopsis.";
    private const string STEP2_DESCRIPTION = "Step 2: Launch action menu on Future Event and verify Thumbnail & Synopsis is displayed.";



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

            //Get Values From ini File
            serviceWithThumbnail = CL.EA.GetServiceFromContentXML("Type=Video;HasThumbnail=True;HasSynopsis=True", "ParentalRating=High");
            if (serviceWithThumbnail == (null))
            {
                FailStep(CL, "Failed to fetch the ServiceWithThumbnail from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "ServiceWithThumbnail is: " + serviceWithThumbnail.LCN);

            }

            defaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
            timeToPopulateThumbnail = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "TIME_TO_POPULATE"));
            
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
            //Step 1: Tune to Channel S1
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithThumbnail.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to serviceWithThumbnail " + serviceWithThumbnail.LCN);
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

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar on Future Event
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            LogCommentInfo(CL,"CHANNEL BAR NEXT is the friendly name for launching action bar on future event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on future event ");
            }

            CL.IEX.Wait(timeToPopulateThumbnail);

            String obtainedThumbnail = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail on Future event from Action Menu");
            }

            //Validate for Thumbnail

            if (obtainedThumbnail.Equals(defaultThumbnail) || string.IsNullOrEmpty(obtainedThumbnail))
            {
                FailStep(CL, res, "Valid thumbnail not recieved : " + obtainedThumbnail, false);
                validThumbnail = false;
            }
            else
            {
                LogCommentInfo(CL, "Thumbnail displayed on Future event from Action Menu " + obtainedThumbnail);
            }

            //Validate for Synopsis.
            String obtainedSynopsis = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedSynopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display synopsis on Future event from Action Menu");
            }

            if (string.IsNullOrEmpty(obtainedSynopsis))
            {
                FailStep(CL, res, "Synopsis not displayed on future event from Action Menu.");
            }
            else
            {
                LogCommentInfo(CL, "Synopsis displayed on future event from Action Menu is: " + obtainedSynopsis);
            }

            if (!validThumbnail)
            {
                FailStep(CL, res, "Failed to display thumbnail on Future event from Action Menu.");
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

    }
    #endregion
}