/// <summary>
///  Script Name : VOD_0341_VOD_Playback_Pause
///  Test Name   : VOD-0341-VOD playback pause
///  TEST ID     : 74006
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



class VOD_0341 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;

    private static class Constants
    {
        public const string EVENT_KEY_NAME = "vodAsset";
        public const int pause = 0;

    }


    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play/Pause and Stop a trailer ");
        this.AddStep(new Step2(), "Step 2: Palay/Pause and Stop an asset ");



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

            // Get a VOD asset with trailer 
            vodAsset = CL.EA.GetVODAssetFromContentXML("Trailer=True");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset with Trailer");
            }
            // Insert the vod asset into the event collection
            CL.EA.UI.Utils.InsertEventToCollection(Constants.EVENT_KEY_NAME, vodAsset.Title, "VOD", "", "", "", Int32.Parse(vodAsset.AssetDuration) * 60, 0, "", "", 0, "");

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

            // Play a trailer  
            res = CL.EA.VOD.PlayTrailer(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset trailer");
            }

            CL.IEX.Wait(5);
            // Pause
            res = CL.EA.PVR.SetTrickModeSpeed(Constants.EVENT_KEY_NAME, Constants.pause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }
            CL.IEX.Wait(3);
            // Stop the trailer playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop trailer playback");
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

            // Play an asset 
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset");
            }

            CL.IEX.Wait(5);
            // Pause
            res = CL.EA.PVR.SetTrickModeSpeed(Constants.EVENT_KEY_NAME, Constants.pause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }
            CL.IEX.Wait(3);
            // Stop the asset playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop asset playback");
            }


            PassStep();
        }
    }
    #endregion
}

