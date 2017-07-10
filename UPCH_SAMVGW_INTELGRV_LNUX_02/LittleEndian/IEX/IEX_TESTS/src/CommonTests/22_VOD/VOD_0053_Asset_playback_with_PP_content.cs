/// <summary>
///  Script Name : VOD_0053_Asset_playback_with_PP_content.cs
///  Test Name   : VOD_0053_Asset_playback_with_PP_content
///  TEST ID     : 74522
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 30/07/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0053")]
public class VOD_0053 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset_PR_eq;
    static VODAsset vodAsset_PR_gt;
    static VODAsset vodAsset_adult;

    private const bool PARENTAL_PIN_REQUESTED = true;
    private const bool PARENTAL_PIN_NOT_REQUESTED = false;
    private const bool PLAY_FROM_BEGINNING = true;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play an adult asset and check PIN is requested before playback";
    private const string STEP2_DESCRIPTION = "Step 2: Play an adult asset trailer and check PIN is requested before playback";
    private const string STEP3_DESCRIPTION = "Step 3: Play an asset with PR > current PR and check PIN is requested before playback";
    private const string STEP4_DESCRIPTION = "Step 4: Play trailer of an asset with PR > current PR and check PIN is requested before playback";
    private const string STEP5_DESCRIPTION = "Step 5: Play an asset with PR = current PR and check PIN is not requested before playback";
    private const string STEP6_DESCRIPTION = "Step 6: Play trailer of an asset with PR = current PR and check PIN is not requested before playback";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);
        this.AddStep(new Step6(), STEP6_DESCRIPTION);
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

            /* PARENTAL_RATING should contain the age rating to be set in settings */
            string parentalRating = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PARENTAL_RATING");
            if (string.IsNullOrEmpty(parentalRating))
            {
                FailStep(CL, "PARENTAL_RATING is empty in Test.ini in section TEST_PARAMS");
            }

            /* ASSET_ADULT should contain the title of an adult asset */
            string adultAssetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_ADULT");
            if (string.IsNullOrEmpty(adultAssetTitle))
            {
                FailStep(CL, "ASSET_ADULT is empty in Test.ini in section TEST_PARAMS");
            }

            /* ASSET_PR_GREATER should contain the title of an asset with PR greater than settings */
            string assetTitle_PR_gt = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_PR_GREATER");
            if (string.IsNullOrEmpty(assetTitle_PR_gt))
            {
                FailStep(CL, "ASSET_PR_GREATER is empty in Test.ini in section TEST_PARAMS");
            }

            /* ASSET_PR_EQUAL should contain the title of an asset with PR equal to settings */
            string assetTitle_PR_eq = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_PR_EQUAL");
            if (string.IsNullOrEmpty(assetTitle_PR_eq))
            {
                FailStep(CL, "ASSET_PR_EQUAL is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the adult VOD asset object       
            vodAsset_adult = CL.EA.GetVODAssetFromContentXML("Title=" + adultAssetTitle);
            if (vodAsset_adult == null)
            {
                FailStep(CL, res, "Failed to get the adult asset " + adultAssetTitle + " from Content.xml");
            }

            // Get the VOD asset object with trailer and PR = current PR in settings        
            vodAsset_PR_eq = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle_PR_eq);
            if (vodAsset_PR_eq == null)
            {
                FailStep(CL, res, "Failed to get the asset " + assetTitle_PR_eq + " from Content.xml");
            }

            // Get the VOD asset object with trailer and PR > current PR in settings
            vodAsset_PR_gt = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle_PR_gt);
            if (vodAsset_PR_gt == null)
            {
                FailStep(CL, res, "Failed to get the asset " + assetTitle_PR_gt + " from Content.xml");
            }

            //  Activate purchase protection
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to activate purchase protection");
            }

            // Set parental rating threshold
            res = CL.EA.STBSettings.SetParentalControlAgeLimit((EnumParentalControlAge)Enum.Parse(typeof(EnumParentalControlAge), "_" + parentalRating));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to " + parentalRating);
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

            // Play an adult asset and check PIN is requested before playback
            res = CL.EA.VOD.PlayAsset(vodAsset_adult, PLAY_FROM_BEGINNING, PARENTAL_PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play an adult asset");
            }

            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
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

            // Play the trailer of an adult asset and check PIN is requested before playback
            res = CL.EA.VOD.PlayTrailer(vodAsset_adult, PARENTAL_PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the trailer of an adult asset");
            }

            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
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

            // Play an asset with PR > current PR and check PIN is requested before playback
            res = CL.EA.VOD.PlayAsset(vodAsset_PR_gt, PLAY_FROM_BEGINNING, PARENTAL_PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play an asset with PR > current PR");
            }

            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the trailer of an asset with PR > current PR and check PIN is requested before playback
            res = CL.EA.VOD.PlayTrailer(vodAsset_PR_gt, PARENTAL_PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the trailer of an asset with PR > current PR");
            }

            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play an asset with PR = current PR and check PIN is not requested before playback
            res = CL.EA.VOD.PlayAsset(vodAsset_PR_eq, PLAY_FROM_BEGINNING, PARENTAL_PIN_NOT_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play an asset with PR = current PR");
            }

            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step6
    [Step(6, STEP6_DESCRIPTION)]
    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the trailer of an asset with PR = current PR and check PIN is not requested before playback
            res = CL.EA.VOD.PlayTrailer(vodAsset_PR_eq, PARENTAL_PIN_NOT_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the trailer of an asset with PR = current PR");
            }

            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
            }

            PassStep();
        }
    }
    #endregion

    #endregion
}