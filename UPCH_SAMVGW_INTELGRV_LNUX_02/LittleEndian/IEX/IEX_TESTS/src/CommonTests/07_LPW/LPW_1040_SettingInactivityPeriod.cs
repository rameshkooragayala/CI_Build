/// <summary>
///  Script Name : LPW_1040_SettingInactivityPeriod.cs
///  Test Name   : LPW-1040-Default inactivity period settings
///  TEST ID     : 72084
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

[Test("LPW_1040")]
public class LPW_1040 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service service;
    static string period = "";
    static string defaultPeriod = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Settings Menu>>Set Inactivity Period and set default to any available value";
    private const string STEP2_DESCRIPTION = "Step 2: Verify Default value can be set to any of the time period available";


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
            //Get Values From ini File
            period = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PERIOD");
            if (string.IsNullOrEmpty(period))
            {
                FailStep(CL, res, "Unable to fetch the period from test ini file");
            }
            //period = "30 min.";

            //Get default period from project ini
            defaultPeriod = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_PERIOD");
            if (string.IsNullOrEmpty(defaultPeriod))
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
            //set default to any available period
            res = CL.EA.STBSettings.ActivateAutoStandByAfterTime(period);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + period);
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
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTIVATE STANDBY AFTER");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to settings ACTIVATE STANDBY AFTER");
            }

            //Fetch the set option
            string obtainedPeriod = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedPeriod);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch obtained period");
            }
            else
            {
                LogCommentInfo(CL, "Obtained period is:" + obtainedPeriod);
            }

            //Verify that selected value is set successfully
            if (!(period.Equals(obtainedPeriod)))
            {
                FailStep(CL, res, "Failed to set selected value");
            }
            else
            {
                LogCommentInfo(CL,"Selected value is set successfully");
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

        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTIVATE STANDBY AFTER");
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to navigate to settings POWER MANAGEMENT");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defaultPeriod);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default PERIOD");
        }
        else
        {
            LogCommentFailure(CL, "Restored to default PERIOD SUCCESSFULLY");
        }
    }
    #endregion
}