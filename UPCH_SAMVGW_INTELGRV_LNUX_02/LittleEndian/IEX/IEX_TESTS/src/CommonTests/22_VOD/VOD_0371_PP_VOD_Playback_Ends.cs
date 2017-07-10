/// <summary>
///  Script Name : VOD_0371_PP_VOD_Playback_Ends  
///  Test Name   : VOD-0371-PP-VOD-Playback-Ends
///  TEST ID     : 74004
///  QC Version  : 2
///  QC Domain   : FR_FUSION
///  QC Project  : UPC
///  QC Path     : 
/// -----------------------------------------------
///  Modified by : Achraf Harchay
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;


[Test("VOD-0371")]
class VOD_0371 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    const bool FROM_START = true;
    const bool PIN_REQUESTED = true;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play a PP asset and wait for end of playback");
        this.AddStep(new Step2(), "Step 2: Play the PP asset again");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            // ASSET_TITLE should contain the title of a PP VOD asset
            string assetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE");
            if (string.IsNullOrEmpty(assetTitle))
            {
                FailStep(CL, "ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get a PP VOD asset 
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset with a duration equal to sven");
            }

            // Set purchase protection to ON
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // PARENTAL_RATING should contain the parental rating to be set in settings
            string parentalRating = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PARENTAL_RATING");
            if (string.IsNullOrEmpty(assetTitle))
            {
                FailStep(CL, "PARENTAL_RATING is empty in Test.ini in section TEST_PARAMS");
            }

            // Set parental rating threshold in settings
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
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the PP asset 
            res = CL.EA.VOD.PlayAsset(vodAsset, FROM_START, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the PP asset");
            }

            // wait for end of playback
            int waitingDuration = (Convert.ToInt32(vodAsset.AssetDuration) + 1) * 60;
            CL.IEX.Wait(waitingDuration);

            // Check playback is stopped
            if (CL.EA.UI.Utils.VerifyState("PLAYBACK"))
            {
                FailStep(CL, res, "ERROR: Asset is still being played. Expected: playback should stop after waiting for asset duration");
            }

            PassStep();
        }
    }
    #endregion

    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the PP asset again
            res = CL.EA.VOD.PlayAsset(vodAsset, FROM_START, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the PP asset again");
            }

            PassStep();
        }
    }
    #endregion

}

