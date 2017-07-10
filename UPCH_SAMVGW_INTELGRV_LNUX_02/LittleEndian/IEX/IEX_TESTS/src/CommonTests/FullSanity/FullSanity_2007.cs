/// <summary>
///  Script Name : FullSanity_2007.cs
///  Test Name   : FullSanity-2007-VOD-Video Formats
///  TEST ID     : 17354
///  QC Version  : 5
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

// FullSanity-2007-VOD-Video Formats
public class FullSanity_2007 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static string errorMsg = "";

    static VODAsset vodAsset_4_3;
    static VODAsset vodAsset_16_9;
    static VODAsset vodAsset_SD;
    static VODAsset vodAsset_HD;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play a 4:3 VOD asset");
        this.AddStep(new Step2(), "Step 2: Play a 16:9 VOD asset");
        this.AddStep(new Step3(), "Step 3: Play a SD VOD asset");
        this.AddStep(new Step4(), "Step 4: Play a HD VOD asset");

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

            // Get a 4:3 VOD asset
            vodAsset_4_3 = CL.EA.GetVODAssetFromContentXML("Type=TVOD;VideoFormat=4:3");
            if (vodAsset_4_3 == null)
            {
                FailStep(CL, res, "Failed to get 4:3 VOD asset from ini file");
            }

            // Get a 16:9 VOD asset
            vodAsset_16_9 = CL.EA.GetVODAssetFromContentXML("Type=TVOD;VideoFormat=16:9");
            if (vodAsset_16_9 == null)
            {
                FailStep(CL, res, "Failed to get 16:9 VOD asset from ini file");
            }

            // Get a SD VOD asset
            vodAsset_SD = CL.EA.GetVODAssetFromContentXML("Type=TVOD;Resolution=SD");
            if (vodAsset_SD == null)
            {
                FailStep(CL, res, "Failed to get SD VOD asset from ini file");
            }

            // Get a HD VOD asset
            vodAsset_HD = CL.EA.GetVODAssetFromContentXML("Type=TVOD;Resolution=HD");
            if (vodAsset_HD == null)
            {
                FailStep(CL, res, "Failed to get HD VOD asset from ini file");
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

            // Play a 4:3 VOD asset
            if (!checkPlayback(vodAsset_4_3, "4:3", ref errorMsg))
            {
                FailStep(CL, errorMsg);
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

            // Play a 16:9 VOD asset
            if (!checkPlayback(vodAsset_16_9, "16:9", ref errorMsg))
            {
                FailStep(CL, errorMsg);
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

            // Play a SD VOD asset
            if (!checkPlayback(vodAsset_SD, "SD", ref errorMsg))
            {
                FailStep(CL, errorMsg);
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

            // Note: Check HD icon is not possible (pb in HE + no milestone from EPG)

            // Play a HD VOD asset   
            if (!checkPlayback(vodAsset_HD, "HD", ref errorMsg))
            {
                FailStep(CL, errorMsg);
            }

            PassStep();
        }
    }
    #endregion   
    #endregion

    static private bool checkPlayback(VODAsset vodAsset, string videoFormat, ref string errorMsg)
    {
        // Play the VOD asset
        IEXGateway._IEXResult res = CL.EA.VOD.PlayAsset(vodAsset);
        if (!res.CommandSucceeded)
        {
            errorMsg = "Failed to play a " + videoFormat + " VOD asset";
            return false;
        }

        const int PLAYBACK_TIMEOUT = 10;
        CL.IEX.Wait(PLAYBACK_TIMEOUT);

        // Stop playback
        res = CL.EA.VOD.StopAssetPlayback();
        if (!res.CommandSucceeded)
        {
            errorMsg = "Failed to stop playback";
            return false;
        }
        return true;
    }
}


