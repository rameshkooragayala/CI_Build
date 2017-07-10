/// <summary>
///  Script Name : FullSanity_2005.cs
///  Test Name   : FullSanity-2005-VOD-Series
///  TEST ID     : 24534
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

//FullSanity-2005-VOD-Series
public class FullSanity_2005 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset;
    static List<string> expectedSeasons; 
    static List<string> expectedEpisodes; 

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Select a series");
        this.AddStep(new Step2(), "Step 2: Select one season");
        this.AddStep(new Step3(), "Step 3: Select an episode");
        this.AddStep(new Step4(), "Step 4: Buy the asset");
        this.AddStep(new Step5(), "Step 5: Stop the playback");

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

            // Get a VOD asset which is part of a series
            vodAsset = CL.EA.GetVODAssetFromContentXML("Type=TVOD;IsEpisode=True", "Price=0");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a TVOD episode asset from ini file");
            }

            // Get test parameters
            string param = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EXPECTED_SEASONS");
            expectedSeasons = new List<string>(param.Split(new char [] {';'}));

            param = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EXPECTED_EPISODES");
            expectedEpisodes = new List<string>(param.Split(new char[] { ';' }));

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

            string seasonName = "";
            List<string> seasonNames = new List<string>();
                        
            // Select a series
            res = CL.EA.NavigateAndHighlight("STATE:STORE_SEASON");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to a series");
            }

            // Browse the list of season
            for (int i=0; i < expectedSeasons.Count; i++)
            {
                // Get season name                
                CL.EA.UI.Utils.GetEpgInfo("title", ref seasonName);
                seasonNames.Add(seasonName);

                // Focus next season 
                CL.EA.UI.Vod.NextSeason();
            }       

            // Check season names
            if (expectedSeasons.Except(seasonNames).Any())
            {
                string outputStr = "Failed to check list of season. Expected: ";
                foreach (string season in expectedSeasons)
                {
                    outputStr += season + ", ";
                }
                outputStr += "Returned: ";
                foreach (string season in seasonNames)
                {
                    outputStr += season + ", ";
                }
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

            string episodeName = "";
            List<string> episodeNames = new List<string>();

            // Select a season
            res = CL.EA.NavigateAndHighlight("STATE:STORE_EPISODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to a season");
            }

            // Browse the list of episode
            for (int i = 0; i < expectedEpisodes.Count; i++)
            {
                // check season name                
                CL.EA.UI.Utils.GetEpgInfo("title", ref episodeName);
                episodeNames.Add(episodeName);
 
                // Focus next episode 
                CL.EA.UI.Vod.NextEpisode();
            }

            // Check episode names
            if (expectedEpisodes.Except(episodeNames).Any())
            {
                string outputStr = "Failed to check list of episode. Expected: ";
                foreach (string episode in expectedEpisodes)
                {
                    outputStr += episode + ", ";
                }
                outputStr += "Returned: ";
                foreach (string episode in episodeNames)
                {
                    outputStr += episode + ", ";
                }
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

            // Select an episode
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to an episode");
            }

            // Check season displayed in action menu
            // ### CQ 2011018 to EPG team to get proper milestone ###
            /*string seasonName = "";
            CL.EA.UI.Utils.GetEPGInfo("xxx", ref seasonName);
            if (vodAsset.Season != seasonName)
            {
                FailStep(CL, res, "Failed to verify Season in Action menu. Expected: " + vodAsset.Season + ", Returned: " + seasonName);
            }*/

            // Check episode is displayed in action menu
            // ### Not testable: HE pb ###
            /*string episodeName = "";
            CL.EA.UI.Utils.GetEPGInfo("xxx", ref episodeName);
            if (vodAsset.Episode != episodeName)
            {
                FailStep(CL, res, "Failed to verify Episode in Action menu. Expected: " + vodAsset.Episode + ", Returned: " + episodeName);
            }*/

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

            // Buy the asset
            res = CL.EA.VOD.BuyAsset(null);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to buy the episode");
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
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
            }
            
            // Check the episode is added in My Library/Purchased ON DEMAND
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset);
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to purchased asset '" + vodAsset.Title + "'");
            }

            // Check season displayed in action menu
            // ### CQ 2011018 to EPG team to get proper milestone ###
            /*string seasonName = "";
            CL.EA.UI.Utils.GetEPGInfo("xxx", ref seasonName);
            if (vodAsset.Season != seasonName)
            {
                FailStep(CL, res, "Failed to verify Season in Action menu. Expected: " + vodAsset.Season + ", Returned: " + seasonName);
            }*/

            // Check episode is displayed in action menu
            // ### Not testable: HE pb ###
            /*string episodeName = "";
            CL.EA.UI.Utils.GetEPGInfo("xxx", ref episodeName);
            if (vodAsset.Episode != episodeName)
            {
                FailStep(CL, res, "Failed to verify Episode in Action menu. Expected: " + vodAsset.Episode + ", Returned: " + episodeName);
            }*/

            PassStep();
        }
    }
    #endregion
    #endregion
}

