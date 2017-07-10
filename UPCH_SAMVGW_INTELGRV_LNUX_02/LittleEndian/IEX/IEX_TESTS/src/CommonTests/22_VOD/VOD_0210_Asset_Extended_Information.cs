/// <summary>
///  Script Name : VOD_0210_Asset_Extended_Information.cs
///  Test Name   : VOD_0210_Asset_Extended_Information & VOD-0210-VOD asset extended Information w. year of prod & VOD_0365_Playback_Action_Menu_Content
///  TEST ID     : 74007 & 74520 & 74019
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 04/06/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0210")]
public class VOD_0210 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to a VOD asset and check the extended information";
    private const string STEP2_DESCRIPTION = "Step 2: Play the asset";
    private const string STEP3_DESCRIPTION = "Step 3: Access the action menu";
    private const string STEP4_DESCRIPTION = "Step 4: Access and check the extended asset information";

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

            /*ASSET_TITLE should contain the title of a VOD asset*/
            string assetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE");
            if (string.IsNullOrEmpty(assetTitle))
            {
                FailStep(CL, "ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get VOD asset " + assetTitle + " from ini file");
            }

            //  Activate purchase protection
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to activate purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit");
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
            
            // Navigate to the VOD asset
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to asset " + vodAsset.Title);
            }            
 
            // *****************
            // Check basic info (duration, synopsis)
            // *****************
            // Check asset duration
            int duration = CL.EA.UI.Vod.GetAssetDuration();
            if (duration != Int32.Parse(vodAsset.AssetDuration))
            {
                FailStep(CL, "Wrong asset rental duration for asset " + vodAsset.Title + ". Expected: " + vodAsset.AssetDuration + ", Received: " + duration);
            }

            // Check asset synopsis
            string synopsis = CL.EA.UI.Vod.GetAssetSynopsis();
            if (synopsis != vodAsset.Synopsis)
            {
                FailStep(CL, "Wrong asset synopsis for asset " + vodAsset.Title + ". Expected: " + vodAsset.Synopsis + ", Received: " + synopsis);
            }

            // *****************
            // Check Viewed/Not viewed status
            // *****************
            if (CL.EA.UI.Vod.GetAssetRentalStatus() == EnumRentalStatus.UNKNOWN)
            {
                FailStep(CL, "Failed to get Viewed/Not viewed status for asset " + vodAsset.Title + ". Rental duration or remaining rental duration must be displayed.");
            }

            // Display asset extended info
            CL.EA.UI.Vod.DisplayExtendedInfo();

            // *****************
            // Check extended info (director, cast, year of production, star rating)
            // *****************
            // Check asset director
            if (vodAsset.Director != null)
            {
                string director = CL.EA.UI.Vod.GetAssetDirector();
                if (director != vodAsset.Director)
                {
                    FailStep(CL, "Wrong asset director for asset " + vodAsset.Title + ". Expected: " + vodAsset.Genre + ", Received: " + director);
                }
            }

            // Check asset cast
            if (vodAsset.Cast != null)
            {
                string cast = CL.EA.UI.Vod.GetAssetCast();
                if (cast != vodAsset.Cast)
                {
                    FailStep(CL, "Wrong asset cast for asset " + vodAsset.Title + ". Expected: " + vodAsset.Cast + ", Received: " + cast);
                }
            }

            // Check asset year of production
            if (vodAsset.YearOfProduction != null)            
            {
                string yearOfProduction = CL.EA.UI.Vod.GetAssetYearOfProduction();
                if (yearOfProduction != vodAsset.YearOfProduction)
                {
                    FailStep(CL, "Wrong asset year of production for asset " + vodAsset.Title + ". Expected: " + vodAsset.YearOfProduction + ", Received: " + yearOfProduction);
                }
            }

            // Check asset star rating
            if (vodAsset.StarRating != null)
            {
                string starRating = CL.EA.UI.Vod.GetAssetStarRating();
                if (starRating != vodAsset.StarRating)
                {
                    FailStep(CL, "Wrong asset star rating for asset " + vodAsset.Title + ". Expected: " + vodAsset.StarRating + ", Received: " + starRating);
                }
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

            // Play the VOD asset
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to play asset " + vodAsset.Title);
            }

            PassStep();
        }
    }
    #endregion

    #region Step3
    [Step(3, STEP2_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Launch the action menu
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to launch action bar");
            }

            // Check basic info (duration, synopsis)
            // Check asset duration
            int duration = CL.EA.UI.Vod.GetAssetDuration();
            if (duration != Int32.Parse(vodAsset.AssetDuration))
            {
                FailStep(CL, "Wrong asset rental duration for asset " + vodAsset.Title + ". Expected: " + vodAsset.AssetDuration + ", Received: " + duration);
            }

            // Check asset synopsis
            string synopsis = CL.EA.UI.Vod.GetAssetSynopsis();
            if (synopsis != vodAsset.Synopsis)
            {
                FailStep(CL, "Wrong asset synopsis for asset " + vodAsset.Title + ". Expected: " + vodAsset.Synopsis + ", Received: " + synopsis);
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

            // Display asset extended info
            CL.EA.UI.Vod.DisplayExtendedInfo();

            // Check asset director
            if (vodAsset.Director != null)
            {
                string director = CL.EA.UI.Vod.GetAssetDirector();
                if (director != vodAsset.Director)
                {
                    FailStep(CL, "Wrong asset director for asset " + vodAsset.Title + ". Expected: " + vodAsset.Genre + ", Received: " + director);
                }
            }

            // Check asset cast
            if (vodAsset.Cast != null)
            {
                string cast = CL.EA.UI.Vod.GetAssetCast();
                if (cast != vodAsset.Cast)
                {
                    FailStep(CL, "Wrong asset cast for asset " + vodAsset.Title + ". Expected: " + vodAsset.Cast + ", Received: " + cast);
                }
            }

            // Check asset year of production
            if (vodAsset.YearOfProduction != null)
            {
                string yearOfProduction = CL.EA.UI.Vod.GetAssetYearOfProduction();
                if (yearOfProduction != vodAsset.YearOfProduction)
                {
                    FailStep(CL, "Wrong asset year of production for asset " + vodAsset.Title + ". Expected: " + vodAsset.YearOfProduction + ", Received: " + yearOfProduction);
                }
            }

            // Check asset star rating
            if (vodAsset.StarRating != null)
            {
                string starRating = CL.EA.UI.Vod.GetAssetStarRating();
                if (starRating != vodAsset.StarRating)
                {
                    FailStep(CL, "Wrong asset star rating for asset " + vodAsset.Title + ". Expected: " + vodAsset.StarRating + ", Received: " + starRating);
                }
            }

            PassStep();
        }
    }
    #endregion

    #endregion
}