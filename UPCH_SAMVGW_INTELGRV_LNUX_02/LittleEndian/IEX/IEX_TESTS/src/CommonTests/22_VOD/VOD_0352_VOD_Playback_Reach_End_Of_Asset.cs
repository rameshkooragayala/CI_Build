/// <summary>
///  Script Name : VOD_0352_VOD_Playback_Reach_End_Of_Asset.cs
///  Test Name   : VOD-0352-VOD-Playback-Reach-End_Of-Asset
///  TEST ID     : 18124
///  QC Version  : 6
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

[Test("VOD_0352")]
public class VOD_0352 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    const string EVENT_KEY_NAME = "vodAsset";
    static string minFwdSpeed;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play a VOD asset from the STORE";
    private const string STEP2_DESCRIPTION = "Step 2: Reach end of asset";
    private const string STEP3_DESCRIPTION = "Step 3: Play a VOD asset from the LIBRARY";
    private const string STEP4_DESCRIPTION = "Step 4: Reach end of asset";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
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

            /*TVOD_TITLE should contain a title of a TVOD asset in STORE*/
            string tVodTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TVOD_TITLE");
            if (string.IsNullOrEmpty(tVodTitle))
            {
                FailStep(CL, "TVOD_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get a VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + tVodTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset '" + tVodTitle + "' from ini file");
            }

            // Get the min forward speed
            minFwdSpeed = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "FWD_MIN");
            if (string.IsNullOrEmpty(minFwdSpeed))
            {
                FailStep(CL, "Maximum forward speed not present in Project.ini file.");
            }

            // Insert the vod asset into the event collection
            CL.EA.UI.Utils.InsertEventToCollection(EVENT_KEY_NAME, vodAsset.Title, "VOD", "", "", "", Int32.Parse(vodAsset.AssetDuration) * 60, 0, "", "", 0, "");

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
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the VOD asset from the STORE
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the VOD asset from STORE");
            }

            PassStep();
        }
    }
    #endregion

    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // forward to reach the end of asset
            res = CL.EA.PVR.SetTrickModeSpeed(EVENT_KEY_NAME, Double.Parse(minFwdSpeed), true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check that STB is back to STORE when reaching end of the asset");
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

    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Select the asset from LIBRARY
            res = CL.EA.VOD.SelectPurchasedAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select the asset in LIBRARY");
            }

            // Play the asset
            res = CL.EA.VOD.PlayAsset(null);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the VOD asset from LIBRARY");
            }

            PassStep();
        }
    }
    #endregion

    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // forward to reach the end of asset
            res = CL.EA.PVR.SetTrickModeSpeed(EVENT_KEY_NAME, Double.Parse(minFwdSpeed), true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check that STB is back to STORE when reaching end of the asset");
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




