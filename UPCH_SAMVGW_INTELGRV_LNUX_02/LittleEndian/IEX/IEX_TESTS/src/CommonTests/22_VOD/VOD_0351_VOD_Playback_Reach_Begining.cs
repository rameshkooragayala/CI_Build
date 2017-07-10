/// <summary>
///  Script Name : VOD_0351_VOD_Playback_Reach_Begining.cs
///  Test Name   : VOD-0351-VOD-Playback-Reach-Begining
///  TEST ID     : 73839
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

[Test("VOD_0351")]
public class VOD_0351 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    static class Constants
    {
        public const string EVENT_KEY_NAME = "vodAsset";
    }
    static string minRewSpeed;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play a VOD asset";
    private const string STEP2_DESCRIPTION = "Step 2: Reach begining of asset in rewind trick mode";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
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

            // Get a VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Price=0");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a free VOD asset from ini file");
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
            // Get the lowest rewind speed
            minRewSpeed = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            if (string.IsNullOrEmpty(minRewSpeed))
            {
                FailStep(CL, "Minimum rewind speed not present in Project.ini file.");
            }

            // Insert the vod asset into the event collection
            CL.EA.UI.Utils.InsertEventToCollection(Constants.EVENT_KEY_NAME, vodAsset.Title, "VOD", "", "", "", Int32.Parse(vodAsset.AssetDuration) * 60, 0, "", "", 0, "");

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

            // Play the VOD asset
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the VOD asset");
            }
            CL.IEX.Wait(20);

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

            // Rewind to reach the beginning
            res = CL.EA.PVR.SetTrickModeSpeed(Constants.EVENT_KEY_NAME, Double.Parse(minRewSpeed), true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check restart of video when reaching beginning of the asset");
            }            

            PassStep();
        }
    }
    #endregion
    #endregion
}



