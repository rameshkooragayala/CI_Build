/// <summary>
///  Script Name : FullSanity_2006.cs
///  Test Name   : FullSanity-2006-VOD-Trickmodes
///  TEST ID     : 17353
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

// FullSanity-2006-VOD-Trickmodes
public class FullSanity_2006 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    const string EVENT_KEY_NAME = "vodAsset";
    static string[] rewTrickModeSpeedArray;
    static string[] fwdTrickModeSpeedArray;
    static VODAsset vodAsset;        

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Start a VOD playback");
        this.AddStep(new Step2(), "Step 2: Check various trickmodes (x2, x6, ...) in forward and rewind");
        this.AddStep(new Step3(), "Step 3: Stop the playback and check Action menu ('PLAY FROM START' and 'RESUME' options should be available)");
        this.AddStep(new Step4(), "Step 4: Start playback from beginning, stop and resume");
        this.AddStep(new Step5(), "Step 5: Rewind to reach the beginning");
        this.AddStep(new Step6(), "Step 6: Forward to reach the end");

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

            // Get a VOD asset
            vodAsset = CL.EA.GetVODAssetFromContentXML("Type=TVOD;Trailer=True");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a TVOD asset from ini file");
            }

            // Insert the vod asset into the event collection
            CL.EA.UI.Utils.InsertEventToCollection(EVENT_KEY_NAME, vodAsset.Title, "VOD", "", "", "", Int32.Parse(vodAsset.AssetDuration) * 60, 0, "", "", 0, "");

            // Fetch the supported REW speeds
            string rewTrickModeSpeeds = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            if (string.IsNullOrEmpty(rewTrickModeSpeeds))
            {
                FailStep(CL, "Rewind Trick mode list not present in Project.ini file.");
            }
            rewTrickModeSpeedArray = rewTrickModeSpeeds.Split(',');

            // Fetch the supported FWD speeds
            string fwdTrickModeSpeeds = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD");
            if (string.IsNullOrEmpty(fwdTrickModeSpeeds))
            {
                FailStep(CL, "Forward Trick mode list not present in Project.ini file.");
            }
            fwdTrickModeSpeedArray = fwdTrickModeSpeeds.Split(',');

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

            // Start a VOD playback
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if(! res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to play a VOD asset: " + vodAsset.Title);
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

            // Check the various trick modes (x2, x6, ...)
            int nbOfSpeed = (rewTrickModeSpeedArray.Length > fwdTrickModeSpeedArray.Length) ? rewTrickModeSpeedArray.Length : fwdTrickModeSpeedArray.Length;
            double fwdSpeed;
            double rewSpeed;
            for (int i = 0; i < nbOfSpeed; i++)
            {
                fwdSpeed = (i < fwdTrickModeSpeedArray.Length) ? Double.Parse(fwdTrickModeSpeedArray[i]) : Double.Parse(fwdTrickModeSpeedArray[fwdTrickModeSpeedArray.Length - 1]);
                rewSpeed = (i < rewTrickModeSpeedArray.Length) ? Double.Parse(rewTrickModeSpeedArray[i]) : Double.Parse(rewTrickModeSpeedArray[rewTrickModeSpeedArray.Length - 1]);
                 
                // FWD 
                res = CL.EA.PVR.SetTrickModeSpeed(EVENT_KEY_NAME, fwdSpeed, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play the asset at x " + fwdSpeed + " Speed");
                }

                // REW
                res = CL.EA.PVR.SetTrickModeSpeed(EVENT_KEY_NAME, rewSpeed, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play the asset at x " + rewSpeed + " Speed");
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

            LogCommentInfo(CL, "Adding a delay of 10 seconds so that the trickmode bar will be timed out");
            CL.IEX.Wait(10);

            //Retrun to playback
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to return to playback because of the reason:" + res.FailureReason);
            }

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            // Check CPE is back to store
            string stateName = "";
            CL.IEX.MilestonesEPG.GetActiveState(out stateName);
            if ((stateName != "ON DEMAND") && (stateName != "STORE_LEAF_CATEGORY"))
            {
                FailStep(CL, res, "Failed to check store state. Current state is '" + stateName + "'");
            }

            // Check focus is on the played asset
            string title;
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (title != vodAsset.Title)
            {
                FailStep(CL, res, "Wrong asset focused. Expected: " + vodAsset.Title + ", Received: " + title);
            }

            // Press SELECT to go to action menu
            string selectKey = CL.EA.UI.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY");
            CL.EA.UI.Utils.SendIR(selectKey);
            CL.IEX.Wait(5);

            // Check that 'PLAY FROM START' option is available
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM BEGINNING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select 'PLAY FROM BEGINNING' item");
            }    
                
            // Check that 'RESUME' option is available
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM LAST VIEWED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select 'PLAY FROM LAST VIEWED' item");
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
            
            // Start the playback from the beginning
            res = CL.EA.VOD.PlayAsset(null, true);
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to start playback from the beginning");
            }
            const int PLAYBACK_DURATION = 60;
            CL.IEX.Wait(PLAYBACK_DURATION);

            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback from the beginning");
            }

            // Resume playback
            res = CL.EA.VOD.PlayAsset(vodAsset, false);
            if (!res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to resume playback");
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
            
            // Get the lowest rewind speed
            string minRewSpeed = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            if (string.IsNullOrEmpty(minRewSpeed))
            {
                FailStep(CL, "Minimum rewind speed not present in Project.ini file.");
            }
            
            // Rewind to reach the beginning
            res = CL.EA.PVR.SetTrickModeSpeed(EVENT_KEY_NAME, Double.Parse(minRewSpeed), true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check restart of video when reaching beginning of the asset");
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

            // Get the lowest forward speed
            string minFwdSpeed = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "FWD_MIN");
            if (string.IsNullOrEmpty(minFwdSpeed))
            {
                FailStep(CL, "Minimum forward speed not present in Project.ini file.");
            }

            // Forward to reach the end
            res = CL.EA.PVR.SetTrickModeSpeed(EVENT_KEY_NAME, Double.Parse(minFwdSpeed), true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check stop of playback when reaching end of the asset");
            }

            // Check CPE is back to store
            string stateName = "";
            CL.IEX.MilestonesEPG.GetActiveState(out stateName);
            if ((stateName != "ON DEMAND") && (stateName != "STORE_LEAF_CATEGORY"))
            {
                FailStep(CL, res, "Failed to check store state. Current state is '" + stateName + "'");
            }

            // Check focus is on the played asset
            string title;
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (title != vodAsset.Title)
            {
                FailStep(CL, res, "Wrong asset focused. Expected: " + vodAsset.Title + ", Received: " + title);
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}



