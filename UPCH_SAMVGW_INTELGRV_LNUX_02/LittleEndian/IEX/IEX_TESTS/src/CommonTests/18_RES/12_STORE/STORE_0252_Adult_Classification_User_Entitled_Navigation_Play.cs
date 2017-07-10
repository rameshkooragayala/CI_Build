/// <summary>
///  Script Name : STORE_0252_Adult_Classification_User_Entitled_Navigation_Play.cs
///  Test Name   : STORE_0252_Adult_Classification_User_Entitled_Navigation_Play
///  TEST ID     : 73981
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 19/05/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STORE_0252")]
public class STORE_0252 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Enter adult classification by entering PIN";
    private const string STEP2_DESCRIPTION = "Step 2: Select an asset and buy/play it";

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
            
            /* ASSET_TITLE should contain the title of an asset in an Adult category */
            string assetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE");
            if (string.IsNullOrEmpty(assetTitle))
            {
                FailStep(CL, "ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset '" + assetTitle + "' from Content.xml");
            }

            // Set purchase protection to ON
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to UNLOCK ALL");
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
            
            // Navigate inside an adult category by entering PIN code
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select an asset inside the Adult category");
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
            
            // Buy/Play the asset without entering the parental PIN (except for purchasing)           
            res = CL.EA.VOD.PlayAsset(null, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the adult asset");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}