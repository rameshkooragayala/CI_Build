/// <summary>
///  Script Name : VOD-0340-VOD playback play speed  
///  Test Name   : VOD-0340-VOD playback play speed  
///  TEST ID     : 74510
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

[Test("VOD340")]
    class VOD340 : _Test
    {
        [ThreadStatic]
        static _Platform CL;

        static VODAsset vodAsset;
        static EnumLanguage subtitleLanguage_1;
       

        #region Create Structure
        public override void CreateStructure()
        {
            this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
            this.AddStep(new Step1(), "Step 1: Play an asset");
            
            

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
                // Get a VOD asset with several  subtitle languages
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
                // Play an asset with with several languages and subtitles
                res = CL.EA.VOD.PlayAsset(vodAsset);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to play the asset");
                }
                int value = Convert.ToInt32(vodAsset.RentalDuration);
                CL.IEX.Wait(value);
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

