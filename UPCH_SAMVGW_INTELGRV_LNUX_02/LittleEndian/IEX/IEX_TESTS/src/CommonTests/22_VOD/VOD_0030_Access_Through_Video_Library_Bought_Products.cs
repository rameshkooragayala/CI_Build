/// <summary>
///  Script Name : VOD_0030_Access_Through_Video_Library_Bought_Products.cs
///  Test Name   : VOD-0030-Access-Through-Video-Library-Bought-Products
///  TEST ID     : 73957
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 05/05/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0030")]
public class VOD_0030 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset[] vodAssets = new VODAsset[2];

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Buy/Play some TVOD assets";
    private const string STEP2_DESCRIPTION = "Step 2: Go to MyLibrary and check basic info of the viewed assets";

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

            /*ASSET_TITLE1 should contain the title of a TVOD asset*/
            string assetTitle1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE1");
            if (string.IsNullOrEmpty(assetTitle1))
            {
                FailStep(CL, "ASSET_TITLE1 is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the VOD asset object
            vodAssets[0] = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle1);
            if (vodAssets[0] == null)
            {
                FailStep(CL, res, "Failed to get VOD asset " + assetTitle1 + " from ini file");
            }

            /*ASSET_TITLE2 should contain the title of a TVOD asset*/
            string assetTitle2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE2");
            if (string.IsNullOrEmpty(assetTitle2))
            {
                FailStep(CL, "ASSET_TITLE2 is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the VOD asset object
            vodAssets[1] = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle2);
            if (vodAssets[1] == null)
            {
                FailStep(CL, res, "Failed to get VOD asset " + assetTitle2 + " from ini file");
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

            foreach (VODAsset asset in vodAssets)
            {
                // Buy some TVOD assets (or play them if already purchased)
                res = CL.EA.VOD.PlayAsset(asset);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to play asset " + asset.Title);
                }

                res = CL.EA.VOD.StopAssetPlayback();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to stop asset playback");
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
            
            foreach (VODAsset asset in vodAssets)
            {
                // Select a played asset in MyLibrary
                res = CL.EA.VOD.SelectPurchasedAsset(asset);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to select asset " + asset.Title + " in MyLibrary");
                }

                // Check basic info (genre, duration, synopsis)
                // Check asset genre
                // CQ 1974366: Need milestone for asset genre in asset details page
                /*string genre = CL.EA.UI.Vod.GetAssetGenre();
                if (genre != asset.Genre)
                {
                    FailStep(CL, "Wrong asset genre for asset " + asset.Title + ". Expected: " + asset.Genre + ", Received: " + genre);
                }*/

                // Check asset duration
                int assetDuration = CL.EA.UI.Vod.GetAssetDuration();
                if (assetDuration != Int32.Parse(asset.AssetDuration))
                {
                    FailStep(CL, "Wrong asset duration for asset " + asset.Title + ". Expected: " + asset.AssetDuration + ", Received: " + assetDuration);
                }

                // Check asset synopsis
                // CQ 2046569: No synopsis milestone returned
                /* synopsis = CL.EA.UI.Vod.GetAssetSynopsis();
                if (synopsis != asset.Synopsis)
                {
                    FailStep(CL, "Wrong asset synopsis for asset " + asset.Title + ". Expected: " + asset.Synopsis + ", Received: " + synopsis);
                }*/
            }

            PassStep();
        }
    }
    #endregion

    #endregion
}


