/// <summary>
///  Script Name : VOD-0370-PP-VOD-Stop-Playback 
///  Test Name   : VOD-0370-PP-VOD-Stop-Playback 
///  TEST ID     : 74512
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


[Test("VOD-0370")]
class VOD_0370 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset_PP;
    static VODAsset vodAsset_Adult;

    const bool FROM_START = true;
    const bool FROM_LAST_POSITION = false;
    const bool PIN_REQUESTED = true;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play a PP asset then stop it");
        this.AddStep(new Step2(), "Step 2: Resume the PP asset");
        this.AddStep(new Step3(), "Step 3: Play the PP asset from the beginning");
        this.AddStep(new Step4(), "Step 4: Play an adult asset then stop it");
        this.AddStep(new Step5(), "Step 5: Resume the adult asset");
        this.AddStep(new Step6(), "Step 6: Play the adult asset from the beginning");
        
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

            /* PARENTAL_RATING_IN_SETTINGS should contain the age rating to be configured in settings */
            string ageRating = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PARENTAL_RATING_IN_SETTINGS");
            if (string.IsNullOrEmpty(ageRating))
            {
                FailStep(CL, "PARENTAL_RATING_IN_SETTINGS is empty in Test.ini in section TEST_PARAMS");
            }

            /* PP_ASSET_TITLE should contain the title of a PP asset */
            string PPAssetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PP_ASSET_TITLE");
            if (string.IsNullOrEmpty(PPAssetTitle))
            {
                FailStep(CL, "PP_ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            /* ADULT_ASSET_TITLE should contain the title of an adult assset */
            string adultAssetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ADULT_ASSET_TITLE");
            if (string.IsNullOrEmpty(adultAssetTitle))
            {
                FailStep(CL, "ADULT_ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get a VOD asset without parental control
            vodAsset_PP = CL.EA.GetVODAssetFromContentXML("Title=" + PPAssetTitle);
            if (vodAsset_PP == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset wihout parental control");
            }

            // Get an adult VOD asset
            vodAsset_Adult = CL.EA.GetVODAssetFromContentXML("Title=" + adultAssetTitle);
            if (vodAsset_Adult == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset with parental control ini file");
            }

            // Set purchase protection to ON
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // Set parental rating threshold
            res = CL.EA.STBSettings.SetParentalControlAgeLimit((EnumParentalControlAge)Enum.Parse(typeof(EnumParentalControlAge), "_" + ageRating));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to " + ageRating);
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

            // Play a PP asset
            res = CL.EA.VOD.PlayAsset(vodAsset_PP, FROM_START, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the PP asset");
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
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

            // Resume the PP asset
            res = CL.EA.VOD.PlayAsset(vodAsset_PP, FROM_LAST_POSITION, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resume the PP asset playback");
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }
            PassStep();
        }
    }
    #endregion

    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the PP asset from the beginning
            res = CL.EA.VOD.PlayAsset(vodAsset_PP, FROM_START, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the PP asset from beginning");
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }            

            PassStep();
        }
    }
    #endregion

    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play an adult asset
            res = CL.EA.VOD.PlayAsset(vodAsset_Adult, FROM_START, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the adult asset");
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step5
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Resume the adult asset
            res = CL.EA.VOD.PlayAsset(vodAsset_Adult, FROM_LAST_POSITION, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resume the adult asset playback");
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }
            PassStep();
        }
    }
    #endregion

    #region Step6
    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the adult asset from the beginning
            res = CL.EA.VOD.PlayAsset(vodAsset_Adult, FROM_START, PIN_REQUESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the adult asset from beginning");
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            PassStep();
        }
    }
    #endregion
}

