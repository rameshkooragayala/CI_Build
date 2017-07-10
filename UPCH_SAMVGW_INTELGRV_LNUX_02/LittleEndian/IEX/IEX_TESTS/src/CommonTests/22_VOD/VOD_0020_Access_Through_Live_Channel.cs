/// <summary>
///  Script Name : VOD_0020_Access_Through_Live_Channel.cs
///  Test Name   : VOD-0020-Access-Through-Live-Channel
///  TEST ID     : 73956
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 28/04/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0020")]
public class VOD_0020 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset[] vodAssets = new VODAsset[2];

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Access VOD assets related to a channel";

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

            /*ASSET_TITLE1 should contain the title of a catchup asset*/
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

            /*ASSET_TITLE2 should contain the title of a catchup asset on same channel as ASSET_TITLE1*/
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

            // Tune to a channel with catchup category
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, vodAssets[0].CatchupService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + vodAssets[0].CatchupService);
            }

            foreach (VODAsset asset in vodAssets)
            {
                // Highlight an asset related to this channel
                res = CL.EA.VOD.NavigateToVODAsset(asset, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to navigate to asset " + asset.Title);
                }

                // Check basic info (price, duration, genre)
                // Check the asset price
                double price = CL.EA.UI.Vod.GetAssetPrice();
                if (price != Double.Parse(asset.Price))
                {
                    FailStep(CL, "Wrong asset price for asset " + asset.Title + ". Expected: " + asset.Price + ", Received: " + price);
                }

                // Check asset rental duration
                int rentalDuration = CL.EA.UI.Vod.GetAssetRentalDuration();
                if (rentalDuration != Int32.Parse(asset.RentalDuration))
                {
                    FailStep(CL, "Wrong asset rental duration for asset " + asset.Title + ". Expected: " + asset.RentalDuration + ", Received: " + rentalDuration);
                }

                // Check asset genre
                string genre = CL.EA.UI.Vod.GetAssetGenre();
                if (genre != asset.Genre)
                {
                    FailStep(CL, "Wrong asset genre for asset " + asset.Title + ". Expected: " + asset.Genre + ", Received: " + genre);
                }
            }

            PassStep();
        }
    }
    #endregion

    #endregion
}

