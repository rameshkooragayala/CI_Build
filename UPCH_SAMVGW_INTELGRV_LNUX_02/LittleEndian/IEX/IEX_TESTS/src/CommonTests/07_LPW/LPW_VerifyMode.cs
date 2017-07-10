/// <summary>
///  Script Name : LPW_VerifyMode.cs
///  Test Name   : LPW-VerifyMode
///  TEST ID     : 
///  QC Version  : 2
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Madhu R
///  QC Repository : UPC/FR_FUSION 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VerifyMode")]
public class VerifyMode : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps
    static Service service;
    static string powerMode = "";
    static string defaultPowerMode = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Settings Menu Power Mode and set default to any power mode";
    private const string STEP2_DESCRIPTION = "Step 2: Verify Default Standby mode can be set to any of the power modes";

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
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }
           // powerMode = "MEDIUM";

            //Get default mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
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

            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL,"Power mode set Successfully");
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
            //verify set power mode
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode, false, "", "", "");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to verify power mode option" );
            }
            else
            {
                LogCommentInfo(CL, "Set Power mode verified Successfully");
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
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to navigate to settings POWER MANAGEMENT");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defaultPowerMode);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default POWER MODE");
        }
        else
        {
            LogCommentFailure(CL, "Restored to default POWER MODE SUCCESSFULLY");
        }

    }
    #endregion
}