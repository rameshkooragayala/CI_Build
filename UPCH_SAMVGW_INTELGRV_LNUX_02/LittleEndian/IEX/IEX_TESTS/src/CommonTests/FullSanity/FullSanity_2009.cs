/// <summary>
///  Script Name : FullSanity_2009.cs
///  Test Name   : FullSanity-2009-VOD-Parental rating & Purchase protection
///  TEST ID     : 17349
///  QC Version  : 3
///  QC Domain   : FR_FUSION
///  QC Project  : UPC
///  QC Path     : GO/NO GO/Full Sanity/20 Sanity-VOD
/// -----------------------------------------------
///  Modified by : Frederic Luu
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

// FullSanity-2009-VOD-Parental rating & Purchase protection
public class FullSanity_2009 : _Test
{
    const int parentalRatingThreshold = 12;

    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset_1;
    static VODAsset vodAsset_2;
    static VODAsset vodAsset_3;
    
    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Activate purchase protection and set parental rating threshold to " + parentalRatingThreshold);
        this.AddStep(new Step2(), "Step 2: Buy an asset with a parental rating <= " + parentalRatingThreshold);
        this.AddStep(new Step3(), "Step 3: Buy an asset with a parental rating > " + parentalRatingThreshold);
        this.AddStep(new Step4(), "Step 4: Play the asset with parental rating <= " + parentalRatingThreshold + " from My Library/Purchased On Demand");
        this.AddStep(new Step5(), "Step 5: Play the asset with parental rating > " + parentalRatingThreshold + " from My Library/Purchased On Demand");
        this.AddStep(new Step6(), "Step 6: Deactivate purchase protection and set parental rating to 'Unlock all'");
        this.AddStep(new Step7(), "Step 7: Buy an asset with parental rating = 18");
        this.AddStep(new Step8(), "Step 8: Play the asset with parental rating = 18 from My Library/Purchased On Demand");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Get 3 VOD assets with different parental rating
            vodAsset_1 = CL.EA.GetVODAssetFromContentXML("Type=TVOD;AgeRating=9", "Genre=ADULT");
            if (vodAsset_1 == null)
            {
                FailStep(CL, res, "Failed to get a TVOD asset with age rating 9 from ini file");
            }

            vodAsset_2 = CL.EA.GetVODAssetFromContentXML("Type=TVOD;AgeRating=16", "Genre=ADULT");
            if (vodAsset_2 == null)
            {
                FailStep(CL, res, "Failed to get a TVOD asset with age rating 16 from ini file");
            }

            vodAsset_3 = CL.EA.GetVODAssetFromContentXML("Type=TVOD;AgeRating=18", "Genre=ADULT");
            if (vodAsset_3 == null)
            {
                FailStep(CL, res, "Failed to get a TVOD asset with age rating 18 from ini file");
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

            //  Activate purchase protection
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to activate purchase protection");
            }

            // Set parental rating threshold
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.FSK_12);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to " + parentalRatingThreshold);
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

            // Buy an asset with with a parental rating <= parental rating threshold
            res = CL.EA.VOD.BuyAsset(vodAsset_1, false, true);
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to buy asset with parental rating <= parental rating threshold");
            }
            CL.IEX.Wait(5);

            // Stop asset playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
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

            // Buy an asset with with a parental rating > parental rating threshold
            res = CL.EA.VOD.BuyAsset(vodAsset_2, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to buy asset with parental rating > parental rating threshold");
            }
            CL.IEX.Wait(5);
            
            // Stop asset playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
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
                        
            // Select the asset with parental rating <= parental rating threshold in My Library/Purchased on demand 
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset_1);
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select '" + vodAsset_1.Title + "' asset in My Library/Purchased on demand");
            }

            // Play the asset
            res = CL.EA.VOD.PlayAsset(null, true, false);
            if (! res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to play the asset with parental rating <= parental rating threshold from My Library/Purchased on demand");
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

            // Select the asset with parental rating > parental rating threshold in My Library/Purchased on demand 
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select '" + vodAsset_2.Title + "' asset in My Library/Purchased on demand");
            }

            // Play the asset
            res = CL.EA.VOD.PlayAsset(null, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset with parental rating > parental rating threshold from My Library/Purchased on demand");
            }
            CL.IEX.Wait(5);

            // Stop asset playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
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

            //  Deactivate purchase protection
            res = CL.EA.STBSettings.SetPurchaseProtection(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to deactivate purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to 'Unlock all'");
            }

            PassStep();
        }
    }
    #endregion

    #region Step7
    private class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Buy an asset with with a parental rating = 18
            res = CL.EA.VOD.BuyAsset(vodAsset_3, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to buy asset with parental rating = 18");
            }
            CL.IEX.Wait(5);

            // Stop asset playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step8
    private class Step8 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Select the asset with parental rating = 18 in My Library/Purchased on demand 
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select '" + vodAsset_3.Title + "' asset in My Library/Purchased on demand");
            }

            // Play the asset
            res = CL.EA.VOD.PlayAsset(null, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset with parental rating = 18 from My Library/Purchased on demand");
            }
            CL.IEX.Wait(5);

            // Stop asset playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        //  Activate purchase protection
        CL.EA.STBSettings.SetPurchaseProtection(true);

        // Set parental rating threshold
        CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
    }
    #endregion
}




