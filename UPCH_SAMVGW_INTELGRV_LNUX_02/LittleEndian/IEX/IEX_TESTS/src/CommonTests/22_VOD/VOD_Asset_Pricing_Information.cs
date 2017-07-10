/// <summary>
///  Script Name : VOD_Asset_Pricing_Information.cs
///  Test Name   : VOD-0150-Asset-Pricing-Information, VOD-0151-Asset-Pricing-Information-Free-Assets
///  TEST ID     : 73837
///  QC Version  : 12
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 20/03/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_Pricing_Information")]
public class VOD_Pricing_Information : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset tVodAsset;
    static VODAsset sVodAsset;
    static VODAsset freeVodAsset;
    static class Constants
    {
        public const bool HIGHLIGHT = false;
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Focus a TVOD not yet purchased and check pricing information";
    private const string STEP2_DESCRIPTION = "Step 2: Focus a SVOD not yet purchased and check pricing information";
    private const string STEP3_DESCRIPTION = "Step 3: Focus a free VOD asset and check price information";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP2_DESCRIPTION);
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

            /*TVOD_TITLE should contain a title of a TVOD asset in STORE*/
            string tVodTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TVOD_TITLE");
            if (string.IsNullOrEmpty(tVodTitle))
            {
                FailStep(CL, "TVOD_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the TVOD asset object
            tVodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + tVodTitle);
            if (tVodAsset == null)
            {
                FailStep(CL, res, "Failed to get TVOD asset from ini file");
            }

            /*SVOD_TITLE should contain a title of a SVOD asset in STORE*/
            string sVodTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SVOD_TITLE");
            if (string.IsNullOrEmpty(sVodTitle))
            {
                FailStep(CL, "SVOD_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the SVOD asset object
            sVodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + sVodTitle);
            if (sVodAsset == null)
            {
                FailStep(CL, res, "Failed to get SVOD asset from ini file");
            }

            // Get a free VOD asset object
            freeVodAsset = CL.EA.GetVODAssetFromContentXML("Price=0");
            if (freeVodAsset == null)
            {
                FailStep(CL, res, "Failed to get a free VOD asset from ini file");
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

            // Focus a TVOD asset
            res = CL.EA.VOD.NavigateToVODAsset(tVodAsset, Constants.HIGHLIGHT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus a TVOD asset");
            }

            // Check asset price
            double price = CL.EA.UI.Vod.GetAssetPrice();
            if (price != Double.Parse(tVodAsset.Price))
            {
                FailStep(CL, res, "Wrong asset price. Expected: " + tVodAsset.Price + ", Received: " + price);
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

            // Focus a SVOD asset
            res = CL.EA.VOD.NavigateToVODAsset(sVodAsset, Constants.HIGHLIGHT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus a SVOD asset");
            }

            // Check "Monthly subscription" message
            if (CL.EA.UI.Vod.GetAssetSubscriptionStatus() != EnumSubscriptionStatus.NOT_SUBSCRIBED)
            {
                FailStep(CL, res, "Wrong SVOD pricing information");
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
            // Focus a VOD asset
            res = CL.EA.VOD.NavigateToVODAsset(freeVodAsset, Constants.HIGHLIGHT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus a free VOD asset");
            }

            // Check asset price
            double price = CL.EA.UI.Vod.GetAssetPrice();
            if (price != 0)
            {
                FailStep(CL, res, "Wrong asset price. Expected: FREE, Received: " + price);
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}


