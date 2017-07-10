/// <summary>
///  Script Name : FullSanity_2010.cs
///  Test Name   : FullSanity-2010-VOD-Adult category
///  TEST ID     : 24535
///  QC Version  : 2
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


// FullSanity-2010-VOD-Adult category
public class FullSanity_2010 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static string adultAssetTitle;
    static string adultCategoryTitle;
    static string adultPIN;
    static VODAsset vodAsset;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Set parental rating threshold to 'Unlock all'");
        this.AddStep(new Step2(), "Step 2: Focus an adult category in the store");
        this.AddStep(new Step3(), "Step 3: Enter the adult category by entering the Master PIN");
        this.AddStep(new Step4(), "Step 4: Buy an asset with Adult genre");
        this.AddStep(new Step5(), "Step 5: Stop the playback");
        this.AddStep(new Step6(), "Step 6: Check the adult asset is not added to the list of purchased On Demand");

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

            // Get the adult category title
            adultCategoryTitle = CL.EA.UI.Utils.GetValueFromProject("VOD", "ADULT_CATEGORY_NAME");

            // Get the adult asset title
            adultAssetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ADULT_ASSET_TITLE");

            // Get the adult vod asset
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + adultAssetTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get an adult VOD asset with title '" + adultAssetTitle + "' from ini file");
            }

            // Get the adult PIN
            adultPIN = CL.EA.UI.Utils.GetValueFromEnvironment("AdultPIN");
            
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

            // Set purchase protection to OFF
            res = CL.EA.STBSettings.SetPurchaseProtection(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to disable purchase protection");
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

    
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Focus an adult category in the store
            res = CL.EA.NavigateAndHighlight("STATE:STORE_ADULT_CATEGORY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus the adult category in store");
            }

            // Check minishowcase is filled with default adult posters
            // ### TODO: Check thumbnail of minishowcase --> CQ 1997467 to be tested

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

            // Enter the adult category by entering the Master PIN
            CL.EA.UI.Vod.DoSelect();

            // Check PIN is asked
            if (!CL.EA.UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL"))
            {
                FailStep(CL, res, "Failed to verify PIN is asked");
            }

            // Enter PIN
            CL.EA.UI.Utils.EnterPin(adultPIN);

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
            
            // Focus an adult asset
            res = CL.EA.NavigateAndHighlight("STATE:STORE_ADULT_ASSET_FROM_ADULT_CATEGORY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select the adult asset");
            }

            // Select the asset
            CL.EA.UI.Vod.DoSelect();

            // Buy the asset            
            res = CL.EA.VOD.BuyAsset(null, false, false);
            if (! res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to buy the adult asset");
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

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if(!res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to stop playback");
            }

            // Check the CPE is back to the category that contains the adult category
            // Limitation: cannot check the category (all VOD screens have same state)
            if (!CL.EA.UI.Utils.VerifyState("ON DEMAND"))
            {
                string state = "";
                CL.EA.UI.Utils.GetActiveState(ref state);
                FailStep(CL, res, "Failed to check that CPE is back to STORE. Expected state: STORE, Returned state:" + state);
            }

            // Check ADULT category is focused     
            string title = "";
            CL.EA.UI.Utils.GetEpgInfo("title", ref title);
            if (title != adultCategoryTitle)
            {
                FailStep(CL, res, "Failed to check that focus is on the adult category. Expected: " + adultCategoryTitle + ", Returned: " + title);
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

            // Check the asset is not added in My Library/Purchased on demand 
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Failure: adult asset '" + vodAsset.Title + "' must not be added to My Library/Purchased on demand");
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



