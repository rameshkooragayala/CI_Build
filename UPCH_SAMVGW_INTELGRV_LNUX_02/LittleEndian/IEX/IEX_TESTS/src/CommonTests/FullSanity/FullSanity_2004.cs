/// <summary>
///  Script Name : FullSanity_2004.cs
///  Test Name   : FullSanity-2004-VOD-CatchUpTV
///  TEST ID     : 24533
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

//FullSanity-2004-VOD-CatchUpTV
public class FullSanity_2004 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset_1;
    static VODAsset vodAsset_2;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Tune and access the catchup category from channel bar");
        this.AddStep(new Step2(), "Step 2: Check that broadcast date time is displayed for each asset");
        this.AddStep(new Step3(), "Step 3: Check that broadcast date time is displayed in action menu of a TVOD and SVOD");
        this.AddStep(new Step4(), "Step 4: Start and stop playback");
        this.AddStep(new Step5(), "Step 5: Go back to live");

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

            // Get a catchup TVOD asset
            vodAsset_1 = CL.EA.GetVODAssetFromContentXML("Type=TVOD;IsCatchUp=True");
            if (vodAsset_1 == null)
            {
                FailStep(CL, res, "Failed to get catchUp TVOD asset from ini file");
            }

            // Get a catchup SVOD asset
            vodAsset_2 = CL.EA.GetVODAssetFromContentXML("Type=SVOD;IsCatchUp=True");
            if (vodAsset_2 == null)
            {
                FailStep(CL, res, "Failed to get catchUp SVOD asset from ini file");
            }

            // Set purchase protection to ON
            /*res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to UNLOCK ALL");
            }*/
            
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

            // Tune to a channel with catchup category
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, vodAsset_1.CatchupService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + vodAsset_1.CatchupService);
            }

            // Access the ON DEMAND in channel bar
            if(! CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CATCHUP"))
            {
                FailStep(CL, res, "Failed to access the catchup category from channel bar");
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
			
			CL.IEX.Wait(2);

            // Check that broadcast date time is displayed for each asset
            DateTime broadcastDateTime;
            string currentAssetTitle = "";
            string lastAssetTitle = "";
            string initialAssetTitle = "";
            CL.EA.UI.Utils.GetEpgInfo("title", ref initialAssetTitle);
            currentAssetTitle = initialAssetTitle;
            do
            {
                // Check broadcast date time format
                broadcastDateTime = CL.EA.UI.Vod.GetAssetBroadcastDateTime();
                if (broadcastDateTime == new DateTime())
                {
                    FailStep(CL, res, "Wrong broadcast datetime format for asset: " + currentAssetTitle);
                }

                // Focus next asset
                CL.EA.UI.Vod.NextAsset();
                lastAssetTitle = currentAssetTitle;
                CL.EA.UI.Utils.GetEpgInfo("title", ref currentAssetTitle);

            } while ((currentAssetTitle != initialAssetTitle) && (currentAssetTitle != lastAssetTitle));
            
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

            // Go into the action menu of a TVOD
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset_1, true);            
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to catchup asset: " + vodAsset_1.Title);
            }

            // Check broadcast date time format
            DateTime broadcastDateTime = CL.EA.UI.Vod.GetAssetBroadcastDateTime();
            if (broadcastDateTime == new DateTime())
            {
                FailStep(CL, res, "Wrong broadcast datetime format in action menu of asset: " + vodAsset_1.Title);
            }

            // Go into the action menu of a SVOD
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset_2, true);            
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to catchup asset: " + vodAsset_2.Title);
            }

            // Check broadcast date time format
            broadcastDateTime = CL.EA.UI.Vod.GetAssetBroadcastDateTime();
            if (broadcastDateTime == new DateTime())
            {
                FailStep(CL, res, "Wrong broadcast datetime format in action menu of asset: " + vodAsset_2.Title);
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

            // Start playback
            res = CL.EA.VOD.PlayAsset(null);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to start playback");
            }

            // ### INFO ### : Not possible to check broadcast date time in timeline

            // Stop playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
            }

            // Check CPE goes back to the CatchUp Category
            string stateName = "";
            CL.EA.UI.Utils.GetActiveState(ref stateName);
            if (stateName != "STORE_LEAF_CATEGORY")
            {
                FailStep(CL, res, "Failed to verify CatchUp category. Expected: 'STORE', Returned: '" + stateName + "'");
            }

            // Check focus is on the played asset
            string assetTitle = "";
            CL.EA.UI.Utils.GetEpgInfo("title", ref assetTitle);
            if (assetTitle != vodAsset_2.Title)
            {
                FailStep(CL, res, "Focus is not on the played asset after stopping the playback. Expected: Focus on " + vodAsset_2.Title);
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

            // Press 'BACK' to go to live
            string backKey = CL.EA.UI.Utils.GetValueFromProject("KEY_MAPPING", "BACK_KEY");
            CL.EA.UI.Utils.SendIR(backKey, 30000);
            
            // Check CPE goes back to live
            string stateName = "";
            CL.EA.UI.Utils.GetActiveState(ref stateName);
            if (stateName != "LIVE")
            {
                FailStep(CL, res, "Failed to verify LIVE state. Expected: 'LIVE', Returned: '" + stateName + "'");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}

