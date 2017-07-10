/// <summary>
///  Script Name : FullSanity_2003.cs
///  Test Name   : FullSanity-2003-VOD-SVOD
///  TEST ID     : 24532
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

//FullSanity-2003-VOD-SVOD
public class FullSanity_2003 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAssetNotSubscribed;
    static VODAsset vodAssetFree;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the asset path From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Focus a non-subscribed SVOD and check information displayed");
        this.AddStep(new Step2(), "Step 2: Select a non-subscribed SVOD and try to subscribe");
        this.AddStep(new Step3(), "Step 3: Focus a subscribed SVOD and check the information displayed");
        this.AddStep(new Step4(), "Step 4: Select a subscribed SVOD and check playback");
        this.AddStep(new Step5(), "Step 5: Stop the playback");
        this.AddStep(new Step6(), "Step 6: Check the action menu");
        this.AddStep(new Step7(), "Step 7: Check the SVOD is not added in 'My library'");

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

            // Get information from ini file
            // non-subscribed SVOD
            vodAssetNotSubscribed = CL.EA.GetVODAssetFromContentXML("Type=SVOD", "Price=0");
            if (vodAssetNotSubscribed == null)
            {
                FailStep(CL, res, "Failed to get a not-free SVOD asset from ini file");
            }

            // subscribed SVOD (free)
            vodAssetFree = CL.EA.GetVODAssetFromContentXML("Type=SVOD;Price=0");
            if (vodAssetFree == null)
            {
                FailStep(CL, res, "Failed to get a free SVOD asset from ini file");
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

            // Focus a non-subscribed SVOD asset in the store
            res = CL.EA.VOD.NavigateToVODAsset(vodAssetNotSubscribed, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus the non-subscribed SVOD asset in the store: " + vodAssetNotSubscribed.Title);
            }

            //  Check the asset title
            string title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (title != vodAssetNotSubscribed.Title)
            {
                FailStep(CL, res, "Wrong asset title displayed. Expected: " + vodAssetNotSubscribed.Title + ", Received: " + title);
            }

            // Check "Monthly Subscription" is displayed
            if (CL.EA.UI.Vod.GetAssetSubscriptionStatus() != EnumSubscriptionStatus.NOT_SUBSCRIBED) 
            {
                FailStep(CL, res, "Wrong information displayed. Expected: 'Monthly subscription - Genre'");
            }

            // Check asset genre
            string genre = CL.EA.UI.Vod.GetAssetGenre();
            if (genre != vodAssetNotSubscribed.Genre)
            {
                FailStep(CL, res, "Wrong asset genre. Expected: " + vodAssetNotSubscribed.Genre + ", Received: " + genre);
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
            res = CL.EA.VOD.NavigateToVODAsset(vodAssetNotSubscribed, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to asset: " + vodAssetNotSubscribed.Title);
            }

            // Verify asset details
            res = CL.EA.VOD.VerifyAssetDetails(vodAssetNotSubscribed, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check asset details");
            }

            // Subscribe
            res = CL.EA.VOD.SubscribeAsset(null);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check Subscription message");
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

            // Focus a Free SVOD asset in the store
            res = CL.EA.VOD.NavigateToVODAsset(vodAssetFree, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus the free SVOD asset in the store: " + vodAssetFree.Title);
            }

            //  Check the asset title
            string title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (title != vodAssetFree.Title)
            {
                FailStep(CL, res, "Wrong asset title displayed. Expected: " + vodAssetFree.Title + ", Received: " + title);
            }

            // Check "FREE" is displayed
            if (CL.EA.UI.Vod.GetAssetSubscriptionStatus() != EnumSubscriptionStatus.FREE ) 
            {
                FailStep(CL, res, "Wrong information displayed. Expected: 'FREE - Genre'");
            }

            // Check asset genre
            string genre = CL.EA.UI.Vod.GetAssetGenre();
            if (genre != vodAssetFree.Genre)
            {
                FailStep(CL, res, "Wrong asset genre. Expected: " + vodAssetFree.Genre + ", Received: " + genre);
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

            // Launch action menu
            res = CL.EA.VOD.NavigateToVODAsset(vodAssetFree, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to asset: " + vodAssetFree.Title);
            }

            // Verify asset details
            res = CL.EA.VOD.VerifyAssetDetails(vodAssetFree, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check asset details");
            }

            // Play the asset
            res = CL.EA.VOD.PlayAsset(null);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset");
            }
            CL.IEX.Wait(5);

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

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the asset");
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
            if (title != vodAssetFree.Title)
            {
                FailStep(CL, res, "Wrong asset focused. Expected: " + vodAssetFree.Title + ", Received: " + title);
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

            // Launch action menu
            res = CL.EA.VOD.NavigateToVODAsset(vodAssetFree, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to asset: " + vodAssetFree.Title);
            }

            // Verify asset details
            res = CL.EA.VOD.VerifyAssetDetails(vodAssetFree, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check asset details");
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

            // wait more than 60 seconds
            const int DELAY_AFTER_STOP_PLAYBACK = 80;
            CL.IEX.Wait(DELAY_AFTER_STOP_PLAYBACK);

            // Check the asset is not added in My Library/Purchased on demand 
            res = CL.EA.VOD.SelectPurchasedAsset(vodAssetFree);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Succeeded to select '" + vodAssetFree.Title + "' asset in My Library/Purchased on demand. Expected: SVOD must not be added in the list.");
            }
            // ### bug in exception handling (in Exception.ParseStack()): cannot use code below
            /*else if (res.FailureCode != ExitCodes.FindEventFailure.GetHashCode())
            {
                FailStep(CL, res, "Failure to check purchased assets in My Library. Reason: " + res.FailureReason);
            }*/  

            PassStep();
        }
    }
    #endregion
    #endregion
}

