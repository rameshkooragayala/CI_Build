/// <summary>
///  Script Name : FullSanity_2002.cs
///  Test Name   : FullSanity-2002-VOD-TVOD
///  TEST ID     : 24531
///  QC Version  : 2
///  QC Domain   : FR_FUSION
///  QC Project  : UPC
///  QC Path     : GO/NO GO/Full Sanity/20 Sanity-VOD
/// -----------------------------------------------
///  Modified by : Frederic Luu
/// </summary>

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.ElementaryActions.FunctionalityCS;
using IEX.Tests.Utils;

//FullSanity-2002-VOD-TVOD
public class FullSanity_2002 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the asset path From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Navigate in VOD catalog and focus a TVOD");
        this.AddStep(new Step2(), "Step 2: Select the TVOD and check the information displayed");
        this.AddStep(new Step3(), "Step 3: Buy the TVOD and check playback");
        this.AddStep(new Step4(), "Step 4: Stop the playback");
        this.AddStep(new Step5(), "Step 5: Check the action menu");
        this.AddStep(new Step6(), "Step 6: Check the purchased assset is added in 'My library'");

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

            // Get TVOD information from ini file
            vodAsset = CL.EA.GetVODAssetFromContentXML("Type=TVOD;Trailer=True", "Price=0");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a TVOD asset from ini file");
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
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            // Focus a TVOD asset in the store
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus the TVOD asset in the store: " + vodAsset.Title);
            }

            //  Check the asset title
            string title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (title != vodAsset.Title)
            {
                FailStep(CL, res, "Wrong asset title displayed. Expected: " + vodAsset.Title + ", Received: " + title);
            }

            // Check the asset price
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
    
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
          
            // Launch action menu
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to asset: " + vodAsset.Title);
            }

            // Verify asset details
            res = CL.EA.VOD.VerifyAssetDetails(vodAsset, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check asset details");
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

            // Buy the asset
            res = CL.EA.VOD.BuyAsset(null);    
            if (!res.CommandSucceeded)
            {
               FailStep(CL, res, "Failed to buy the asset");
            }
            CL.IEX.Wait(5);

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

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (! res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to stop the playback");
            }
            
            // Check CPE is back to store
            string stateName = "";
            CL.IEX.MilestonesEPG.GetActiveState(out stateName);
            if (stateName != "ON DEMAND")
            {
                FailStep(CL, res, "Failed to check STORE state. Current state is '" + stateName + "'");
            }

            // Check focus is on the played asset
            string title;
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (title != vodAsset.Title)
            {
                FailStep(CL, res, "Wrong asset focused. Expected: " + vodAsset.Title + ", Received: " + title);
            }

            // Check remaining rental duration
            int remainingRentalDuration = CL.EA.UI.Vod.GetAssetRemainingRentalDuration();
            if ((remainingRentalDuration <= 0) || (remainingRentalDuration > Int32.Parse(vodAsset.RentalDuration)))
            {
                FailStep(CL, res, "Wrong remaining rental duration. Received: " + remainingRentalDuration);
            }

            // Check genre
            string genre = CL.EA.UI.Vod.GetAssetGenre();
            if (genre != vodAsset.Genre)
            {
                FailStep(CL, res, "Wrong asset genre. Expected: " + vodAsset.Genre + ", Received: " + genre);
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

            // Launch action menu
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to asset: " + vodAsset.Title);
            }

            // Verify asset details
            res = CL.EA.VOD.VerifyAssetDetails(vodAsset, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check asset details");
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

            // Check the asset is added in My Library/Purchased on demand 
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset);
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select '" + vodAsset.Title + "' asset in My Library/Purchased on demand");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}
