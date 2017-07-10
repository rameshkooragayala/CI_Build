/// <summary>
///  Script Name : VOD_0146_Asset_Information_SVOD.cs
///  Test Name   : VOD_0146_Asset_Information_SVOD
///  TEST ID     : 74650
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 04/08/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0146")]
public class VOD_0146 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    static class Constants
    {
        public const bool SELECT = true;
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Select an entitled SVOD asset and check the subscription status displayed";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
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

            /*ASSET_TITLE should contain the title of an entitled SVOD asset in STORE*/
            string assetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE"); 
            if (string.IsNullOrEmpty(assetTitle))
            {
                FailStep(CL, "ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get VOD asset from ini file");
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
            
            // Select an entitled SVOD asset
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, Constants.SELECT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select a SVOD asset");
            }
            
            // Check entitlement information displayed
            EnumSubscriptionStatus status = CL.EA.UI.Vod.GetAssetSubscriptionStatus();
            if (status != EnumSubscriptionStatus.SUBSCRIBED)
            {
                FailStep(CL, res, "Wrong asset subscription status. The asset should be entitled");
            }
            
            PassStep();
        }
    }
    #endregion
    #endregion
}


