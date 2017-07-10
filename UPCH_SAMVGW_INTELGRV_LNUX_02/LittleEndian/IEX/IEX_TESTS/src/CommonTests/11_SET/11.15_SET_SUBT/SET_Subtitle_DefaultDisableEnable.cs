/// <summary>
///  Script Name : SET_Subtitle_DefaultDisableEnable.cs
///  Test Name   : SET-SUBT-0001-Live-Subtitle-Default-Disable
///  TEST ID     : 
///  QC Version  : 2
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Madhu Renukaradhya
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("SET_SUBT_DefaultDisableEnable")]
public class SET_SUBT_DefaultDisableEnable : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps
    private static Service service;
    private static string obtainedDefaultSubtitle = "";
    private static string expectedDefaultSubtitle = "";
    private static string obtainedDisabledSubtitle = "";
    private static string expectedDisabledSubtitle = "";
    private static string obtainedEnabledSubtitle = "";
    private static string expectedEnabledSubtitle = "";



    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Settings>>Subtitle";
    private const string STEP2_DESCRIPTION = "Step 2: Verify for the Default subtitle settings";
    private const string STEP3_DESCRIPTION = "Step 3: Disable the subtitle and verify that the subtitles are disabled";
    private const string STEP4_DESCRIPTION = "Step 4: Enable the subtitle and verify that the subtitles are Enabled";


    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
      


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
            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;NoOfSubtitleLanguages=0,1");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }

            expectedDefaultSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DEFAULT");

            if (String.IsNullOrEmpty(expectedDefaultSubtitle))
            {
                FailStep(CL, res, "DEFAULT subtitle value is not present in the Project.ini");
            }
            else
            {
                LogCommentInfo(CL, "Expected default subtitle is : " + expectedDefaultSubtitle);
            }

            expectedDisabledSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DISABLE");
            if (String.IsNullOrEmpty(expectedDisabledSubtitle))
            {
                FailStep(CL, res, "DISABLED subtitle value is not present in the Project.ini");
            }
            else
            {
                LogCommentInfo(CL, "Expected disabled subtitle is : " + expectedDisabledSubtitle);
            }

            expectedEnabledSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "ENABLE");
            if (String.IsNullOrEmpty(expectedEnabledSubtitle))
            {
                FailStep(CL, res, "ENABLED subtitle value is not present in the Project.ini");
            }
            else
            {
                LogCommentInfo(CL, "Expected enabled subtitle is : " + expectedEnabledSubtitle);
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

            //Tune to the service having AV

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
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

            //Navigate to subtitles under settings and verify for default subtitle
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to SUBTITLES under settings ");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDefaultSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the default subtitle");
            }
            else
            {
                LogCommentInfo(CL, "Obtained default subtitle is : " + obtainedDefaultSubtitle);
            }

            if (expectedDefaultSubtitle.Equals(obtainedDefaultSubtitle.ToUpper().Trim()))
            {
                LogCommentInfo(CL, "Default Subtitle is verified");
            }
            else
            {
                FailStep(CL, res, "Default Subtitle is incorrect.");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Navigate to subtitles under settings,disable subtitle and verify
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to disable SUBTITLES under settings ");
            }

            //verify wheather the subtitle is set to OFF 
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to back to SUBTITLES");
            }


            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDisabledSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the obtainedDisabledSubtitle");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Diabaled subtitle is : " + obtainedDisabledSubtitle);
            }

            if (expectedDisabledSubtitle.Equals(obtainedDisabledSubtitle.ToUpper().Trim()))
            {
                LogCommentInfo(CL, "Subtitles are disabled successfully");
            }
            else
            {
                FailStep(CL, res, "Subtitles are not disabled");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();


                //Navigate to subtitles under settings,enable subtitle and verify
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE ON");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to enable SUBTITLES under settings ");
                }

                //verify wheather the subtitle is enabled 
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate back to SUBTITLES under Settings");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedEnabledSubtitle);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the subtitle");
                }

                if (expectedEnabledSubtitle.Equals(expectedEnabledSubtitle.ToUpper().Trim()))
                {
                    LogCommentInfo(CL, "Subtitles are Enabled successfully");
                }
                else
                {
                    FailStep(CL, res, "Subtitles are not Enabled");
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
        IEXGateway._IEXResult res;

        //Set back to default subtitle
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to navigate to SUBTITLE");
        }

        res = CL.IEX.MilestonesEPG.Navigate(expectedDefaultSubtitle);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to set to DEFAULT SUBTITLE");
        }

    }
    #endregion
}