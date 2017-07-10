/// <summary>
///  Script Name : VOD_0140_Asset_Basic_Information.cs
///  Test Name   : VOD-0140-Asset-Basic-Information
///  TEST ID     : 73836
///  QC Version  : 8
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

[Test("VOD_0140")]
public class VOD_0140 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    static string defaultThumbnail;
    static class Constants
    {
        public const bool HIGHLIGHT = false;
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Focus a VOD asset and check displayed information";

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

            /*ASSET_TITLE should contain a title of a VOD asset in STORE*/
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
            defaultThumbnail = CL.EA.GetValue("DefaultThumbnail");

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
            
            // Focus a VOD asset
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, Constants.HIGHLIGHT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus a VOD asset");
            }

            // Check asset poster      
            string thumbnail = "";
            CL.EA.UI.Utils.GetEpgInfo("m_imgurl", ref thumbnail);
            if (string.IsNullOrEmpty(thumbnail) || thumbnail.Equals(defaultThumbnail) || thumbnail.Length.Equals(0))
            {
                FailStep(CL, res, "Asset poster is either null or default thumbnail " + thumbnail);
            }

            // Check asset price
            double price = CL.EA.UI.Vod.GetAssetPrice();
            if (price != Double.Parse(vodAsset.Price))
            {
                FailStep(CL, res, "Wrong asset price. Expected: " + vodAsset.Price + ", Received: " + price);
            }

            // Check asset rental duration
            int rentalDuration = CL.EA.UI.Vod.GetAssetRentalDuration();
            if (rentalDuration != Int32.Parse(vodAsset.RentalDuration))
            {
                FailStep(CL, res, "Wrong asset rental duration. Expected: " + vodAsset.RentalDuration + ", Received: " + rentalDuration);
            }

            // Check asset genre
            string genre = CL.EA.UI.Vod.GetAssetGenre();
            if (genre != vodAsset.Genre)
            {
                FailStep(CL, res, "Wrong asset genre. Expected: " + vodAsset.Genre + ", Received: " + genre);
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}

