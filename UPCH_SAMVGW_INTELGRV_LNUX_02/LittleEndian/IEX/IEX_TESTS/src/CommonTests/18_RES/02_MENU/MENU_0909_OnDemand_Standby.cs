/// <summary>
///  Script Name : MENU_0909_OnDemand_Standby.cs
///  Test Name   : MENU-0909-Main Menu showcase for Store selected
///  TEST ID     : 68201
///  JIRA ID     : FC-383
///  QC Version  : 1
///  Variations from QC: NONE
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("MENU_0909")]
public class MENU_0909 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service videoService;
    static int standBySafeDelay;
    static string mainMenuFirstFocused;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to On Demand";
    private const string STEP2_DESCRIPTION = "Step 2: Enter and exit standby";
    private const string STEP3_DESCRIPTION = "Step 3: Navigate to On Demand";

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
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            string standbyDelayStringVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");
            mainMenuFirstFocused = CL.EA.GetValueFromINI(EnumINIFile.Project, "MENUS", "MAIN_MENU_FIRST_FOCUSSED_ITEM");

            if (videoService == null || string.IsNullOrEmpty(standbyDelayStringVal) || string.IsNullOrEmpty(mainMenuFirstFocused))
            {
               FailStep(CL, "One of the values is null. Video Service fetched from Content.xml:" + videoService + ", SAFE_DELAY_SEC:" + standbyDelayStringVal);
            }
            
            standBySafeDelay = Int32.Parse(standbyDelayStringVal);
            LogCommentInfo(CL, " Video Service: " + videoService.LCN + ", SAFE_DELAY_SEC: " + standbyDelayStringVal + ", MAIN_MENU_FIRST_FOCUSSED_ITEM" + mainMenuFirstFocused);
 
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

            //Navigate to On Demand
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STORE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Navigate to STATE:STORE");
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

            //Enter standby for few seconds and exit stand by.
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Enter Standby ");
            }

            LogCommentInfo(CL, "Wait for standby exit safe delay: " + standBySafeDelay + "seconds");
            res = CL.IEX.Wait(standBySafeDelay);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            CL.EA.UI.Utils.ClearEPGInfo();

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Exit Standby ");
            }
         
            if (!CL.EA.UI.Utils.VerifyState("LIVE", 2))
            {
                FailStep(CL, res, "Failed to verify Live after standBy ");
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

            //Navigate to OnDemand again after enter and exit stand by
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STORE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:STORE");
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