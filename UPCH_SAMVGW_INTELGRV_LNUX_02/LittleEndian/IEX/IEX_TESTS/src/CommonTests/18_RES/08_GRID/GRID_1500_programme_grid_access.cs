/// <summary>
///  Script Name : GRID_1500_programme_grid_access.cs
///  Test Name   : GRID_1500_programme_grid_access
///  TEST ID     : 17732
///  QC Version  : 7
///  Variations from QC:Not doing for By Genre
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("GRID_1500_programme_grid_access")]
public class GRID_1500 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static bool isInGuide;
    static string AdjTimelineDuration;
    static string crumbTextAdjTImeline;
    static string category;

    public  const string coordinates = "159 240 761 478";
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get values from Test.INI & Tune to Service S1";
     private const string STEP1_DESCRIPTION = "Step 1: Launch  All channels Guide View & verify Audio & video is played in background ";
    private const string STEP2_DESCRIPTION = "Step 2: Launch Single channels Guide View & verify Audio & video is played in background";
    private const string STEP3_DESCRIPTION = "Step 3: Launch Adjust Time Line Guide View & verify Audio & video is played in background" ;
    private const string STEP4_DESCRIPTION = "Step 4: Launch By Genre Guide View & verify Audio & video is played in background";


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
            FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FTA_Channel");
            if (String.IsNullOrEmpty(FTA_Channel))
            {
                FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");
            }
            // fetch duration for adjust timeline
            AdjTimelineDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DURATION");

            category = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "CATEGORY");

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to Tune to service s1");
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
            // navigate to All channel

            CL.EA.UI.Guide.Navigate();
            isInGuide = CL.EA.UI.Guide.IsGuide();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch All channels Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified All Channels guide launched");
            }

            // verifying audio is present in the back ground
            res = CL.EA.CheckForAudio(true, 500);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify Audio in background");
            }
            else
            {
                LogCommentInfo(CL, "verified Audio in background");
            }

            CL.IEX.Wait(1);
            // verifying video is present in the back ground
            res = CL.EA.CheckForVideo(coordinates,false,10,false);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify video in background");
            }
            else
            {
                LogCommentInfo(CL, "verified video in background");
            }

            res = CL.EA.ReturnToLiveViewing(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to return to Live");
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
            // navigate to All channel

            CL.EA.UI.Guide.NavigateToGuideSingleChannel();

            isInGuide = CL.EA.UI.Guide.IsGuideSingleChannel();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch single channel Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified single Channels guide launched");
            }

            // verifying audio is present in the back ground
            res = CL.EA.CheckForAudio(true, 500);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify Audio in background");
            }
            else
            {
                LogCommentInfo(CL, "verified Audio in background");
            }
            CL.IEX.Wait(1);
            //// verifying video is present in the back ground
            res = CL.EA.CheckForVideo(coordinates, true, 10, false);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify video in background");
            }
            else
            {
                LogCommentInfo(CL, "verified video in background");
            }

            res = CL.EA.ReturnToLiveViewing(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to return to Live");
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
            // navigate to All channel

            CL.EA.UI.Guide.NavigateToGuideAdjustTimeline(AdjTimelineDuration);
            CL.IEX.Wait(2);
            CL.IEX.MilestonesEPG.GetEPGInfo("crumbtext", out crumbTextAdjTImeline);
            if (crumbTextAdjTImeline.ToUpper() != "ADJUST TIMELINE  ALL CHANNELS")
            {
                FailStep(CL, "Failed to launch Adjust Timeline Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified Adjust Timeline guide launched");
            }

            // verifying audio is present in the back ground
            res = CL.EA.CheckForAudio(true, 500);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify Audio in background");
            }
            else
            {
                LogCommentInfo(CL, "verified Audio in background");
            }

            CL.IEX.Wait(1);
            // verifying video is present in the back ground
            res = CL.EA.CheckForVideo(coordinates, false, 10, false);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify video in background");
            }
            else
            {
                LogCommentInfo(CL, "verified video in background");
            }

            res = CL.EA.ReturnToLiveViewing(false);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to return to Live");
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
            // navigate to All channel

            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:BY GENRE");
                CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem(category);
                CL.EA.UI.Utils.SendIR("SELECT");
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("crumbtext", out crumbTextAdjTImeline);
                if (crumbTextAdjTImeline.ToUpper() != "BY GENRE  MUSIC & DANCE")
                {
                    FailStep(CL, "Failed to launch Adjust Timeline Guide");
                }
                else
                {
                    LogCommentInfo(CL, "Verified Adjust Timeline guide launched");
                }

                // verifying audio is present in the back ground
                res = CL.EA.CheckForAudio(true, 500);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to verify Audio in background");
                }
                else
                {
                    LogCommentInfo(CL, "verified Audio in background");
                }

                CL.IEX.Wait(1);
                // verifying video is present in the back ground
                res = CL.EA.CheckForVideo(coordinates, true, 10, false);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to verify video in background");
                }
                else
                {
                    LogCommentInfo(CL, "verified video in background");
                }

                res = CL.EA.ReturnToLiveViewing(false);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to return to Live");
                }
            }
            catch (Exception ex)
            {
                FailStep(CL, "Failed in Step 4. Reason: "+ ex.Message);
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