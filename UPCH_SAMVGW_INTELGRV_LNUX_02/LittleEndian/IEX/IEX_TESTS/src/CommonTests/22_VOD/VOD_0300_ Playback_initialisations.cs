/// <summary>
///  Script Name : VOD-0300-VOD Playback initialisations 
///  Test Name   : VOD-0300-VOD Playback initialisations  
///  TEST ID     : 74509
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



[Test("VOD-0300")]
class VOD_0300 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset;
    static VODAsset vodAsset_2;




    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play/Stop an asset");
        this.AddStep(new Step2(), "Step 2: Play/Stop an asset then resume ");


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

            // Get a VOD asset without parental control
            vodAsset = CL.EA.GetVODAssetFromContentXML("Type=TVOD");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset from ini file");
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


            // Play an asset 
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset");
            }
            
            CL.IEX.Wait(10);
            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
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
                FailStep(CL, res, "Failed to play the asset ");
            }
            CL.IEX.Wait(10);
            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            // Resume asset Play
            res = CL.EA.VOD.PlayAsset(vodAsset,false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resume the asset playing ");
            }
            CL.IEX.Wait(10);
            // Stop the playback
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            PassStep();
        }
    }
    #endregion




}

