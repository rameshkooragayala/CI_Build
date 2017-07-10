/// <summary>
///  Script Name : LPW_1000_SetAutomaticStandBy.cs
///  Test Name   : LPW-1000-SetAutomaticStandBy
///  TEST ID     : 72100
///  QC Version  : 2
///  Variations from QC:none
///  QC Repository : UPC/FR_FUSION 
/// ----------------------------------------------- 
///  Modified by : Madhu.R
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("LPW_1000")]
public class LPW_1000 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service service;
    static string friendlyNameStandyBy = "";
    static string expectedDefaultstandyBy = "";
    static string obtainedDeafultStandBy = "";
    static string displayNameStandyBy = "";
    

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Settings Menu>>AutoStandBy,Verify default value and set default to any available value";
    private const string STEP2_DESCRIPTION = "Step 2: Verify the set mode is verified successfully.";


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


            //Get Values From xml File
            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch service from content xml.");

            }
            //Get friendly name values From ini File
            friendlyNameStandyBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FRIENDLY_NAME");
            if (string.IsNullOrEmpty(friendlyNameStandyBy))
            {
                FailStep(CL, res, "Unable to fetch the friendlyNameStandyBy value from test ini file");
            }

            //Get display name values From ini File
            displayNameStandyBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DISPLAY_NAME");
            if (string.IsNullOrEmpty(displayNameStandyBy))
            {
                FailStep(CL, res, "Unable to fetch the displayNameStandyBy value from test ini file");
            }
            //standyBy = "OFF";

            //Get default period from project ini
            expectedDefaultstandyBy = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_STANDBY");
            if (string.IsNullOrEmpty(expectedDefaultstandyBy))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
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
                            
            //Navigate to Auto StandBy
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to settings AUTO STANDBY");
            }
            obtainedDeafultStandBy = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDeafultStandBy);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch obtained default standBy");
            }

            //Verify for default standBy .

            if (!expectedDefaultstandyBy.Equals(obtainedDeafultStandBy))
            {
                FailStep(CL,res ,"Default standBy is incorrect");
            }
            //set default to any available standBy
            res = CL.EA.STBSettings.SetAutoStandBy(friendlyNameStandyBy);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + friendlyNameStandyBy);
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

            //Navigate back to verify that selected option is set.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to settings AUTO STANDBY");
            }

            //Fetch the set option
            string obtainedStandBy = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedStandBy);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch obtained standBy");
            }
            else
            {
                LogCommentInfo(CL, "Obtained value is:" + obtainedStandBy);
            }

            //Verify that selected value is set successfully
            if (!(displayNameStandyBy.Equals(obtainedStandBy)))
            {
                FailStep(CL, res, "Failed to set selected value");
            }
            else
            {
                LogCommentInfo(CL, "Selected value is set successfully");
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
        //Restore default settings
        IEXGateway._IEXResult res;

        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL,"Failed to navigate to settings POWER MANAGEMENT");
        }

        res = CL.IEX.MilestonesEPG.SelectMenuItem(expectedDefaultstandyBy);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to hihglight to default standBy");
        }
		CL.EA.UI.Utils.SendIR("SELECT");
    }
    #endregion
}