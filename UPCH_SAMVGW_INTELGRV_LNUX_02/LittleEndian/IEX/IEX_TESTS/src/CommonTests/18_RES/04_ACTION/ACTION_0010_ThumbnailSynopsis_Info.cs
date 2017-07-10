/// <summary>
///  Script Name : ACTION_0010_ThumbnailSynopsis_Info
///  Test Name   : ACTION-0010-ThumbnailSynopsis-Info
///  TEST ID     : 64533
///  QC Version  : 1
///  Jira ID     : FC-535
///  Variations from QC:none
/// ----------------------------------------------- 
///  Scripted by : Madhu Renukaradhya
///  Last modified : 25 JULY 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0010")]
public class ACTION_0010 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static Service serviceWithThumbnail;
    static String defaultThumbnail;
    static Boolean ValidThumbnail = true;
    static int timeToPopulateThumbnail = 5;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to service S1 having thumbnails and Synopsis.";
    private const string STEP2_DESCRIPTION = "Step 2: Launch action menu on Future Event and verify Thumbnail & Synopsis is displayed.";
    private const string STEP3_DESCRIPTION = "Step 3: Select INFO option on action menu on Future Event and verify Thumbnail & Synopsis is displayed ";



    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

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
            if (serviceWithThumbnail.Equals(null))
            {
                FailStep(CL, "Failed to fetch the ServiceWithThumbnail from content xml: " + serviceWithThumbnail);
            }
            else
            {
                LogCommentInfo(CL, "ServiceWithThumbnail is: " + serviceWithThumbnail.LCN);

            }

            defaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
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
                FailStep(CL, res, "Failed to Tune to serviceWithThumbnail " + serviceWithThumbnail);
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
                ValidThumbnail = false;
            }
            else
            {
                LogComment(CL, "Thumbnail displayed on Action Menu " + obtainedThumbnail);
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
                FailStep(CL, res, "Synopsis not displayed on future event from Action Menu. " + obtainedSynopsis);
            }
            else
            {
                LogComment(CL, "Snopsis displayed on future event from Action Menu is: " + obtainedSynopsis);
            }

            if (ValidThumbnail.Equals(false))
            {
                FailStep(CL, res, "Failed to display thumbnail on Future event from Action Menu. ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar on Future Event
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT INFO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to INFO on future event of Action Menu.");
            }

            CL.IEX.Wait(timeToPopulateThumbnail);

            String obtainedThumbnail = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedThumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail on INFO option of future event");
            }

            //Validate for Thumbnail
            LogCommentInfo(CL, "Obtained thumbnail on INFO option of future event is: " + obtainedThumbnail);

            if (obtainedThumbnail.Equals(defaultThumbnail) || string.IsNullOrEmpty(obtainedThumbnail))
            {
                FailStep(CL, res, "Valid thumbnail not recieved on INFO option of future event: " + obtainedThumbnail, false);
                ValidThumbnail = false;
            }
            else
            {
                LogComment(CL, "Thumbnail displayed on INFO option of future event " + obtainedThumbnail);
            }

            //Validate for Synopsis.
            String obtainedSynopsis = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedSynopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display synopsis on INFO option of future event");
            }

            if (string.IsNullOrEmpty(obtainedSynopsis))
            {
                FailStep(CL, res, "Synopsis not displayed on INFO option of future event " + obtainedSynopsis);
            }
            else
            {
                LogComment(CL, "Snopsis displayed on INFO option of future event: " + obtainedSynopsis);
            }

            if (ValidThumbnail.Equals(false))
            {
                FailStep(CL, res, "Valid thumbnail not recieved on INFO option of future event. ");
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