/// <summary>
///  Script Name : VOD_0367_playback_action_DVBsubtitle
///  Test Name   : VOD-0367-VOD playback action DVBsubtitle
///  TEST ID     : 73991
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

[Test("VOD-0367")]
    class VOD_0367 : _Test
    {
        [ThreadStatic]
        static _Platform CL;

        static VODAsset vodAsset;
        static EnumLanguage subtitleLanguage_1;
       

        #region Create Structure
        public override void CreateStructure()
        {
            this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
            this.AddStep(new Step1(), "Step 1: Play an asset with subtitle");
            this.AddStep(new Step2(), "Step 2: disable subtitle from action menu ");
            this.AddStep(new Step3(), "Step 3: change to default subtitle");
            

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
                vodAsset = CL.EA.GetVODAssetFromContentXML("NoOfSubtitleLanguages=2;SubtitleType=Dvb");
                if (vodAsset == null)
                {
                    FailStep(CL, res, "Failed to get a VOD asset with several audio and subtitle languages from ini file");
                }

                // Get  subtitle languages to use
               

                res = CL.EA.GetSubtitleLanguage(vodAsset, 0, ref subtitleLanguage_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get language of 1st subtitle");
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
                    FailStep(CL, res, "Failed to play the asset with  subtitle languages");
                }

                // Select a subtitle language from action menu
                res = CL.EA.SubtitlesLanguageChange(subtitleLanguage_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to change subtitle language in action bar from OFF to " + subtitleLanguage_1.ToString());
                }

                // Check subtitles are displayed
                EnumLanguage subtitleLanguage = EnumLanguage.Off;
                CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
                if (subtitleLanguage != subtitleLanguage_1)
                {
                    FailStep(CL, res, "Failed to check current subtitle language. Expected: " + subtitleLanguage_1.ToString() + ", Received: " + subtitleLanguage.ToString());
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

                
                // disable subtitle language from action menu
                res = CL.EA.SubtitlesLanguageChange(EnumLanguage.Off);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to change subtitle language in action bar from ON to OFF");
                }

                            

                // Check that prefered subtitle language is not displayed
                EnumLanguage subtitleLanguage = EnumLanguage.Off;
                CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
                if (subtitleLanguage !=  EnumLanguage.Off)
                {
                    FailStep(CL, res, "Failed to check current subtitle language is OFF");
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


               
                // Select a subtitle language from action menu
                res = CL.EA.SubtitlesLanguageChange(subtitleLanguage_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to change subtitle language in action bar from OFF to " + subtitleLanguage_1.ToString());
                }

               

                // Check that prefered subtitle language is displayed
                EnumLanguage subtitleLanguage = EnumLanguage.Off;
                CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
                if (subtitleLanguage != subtitleLanguage_1)
                {
                    FailStep(CL, res, "Failed to check current subtitle language. Expected: " + subtitleLanguage_1.ToString() + ", Received: " + subtitleLanguage.ToString());
                }

                PassStep();
            }
        }
        #endregion
       
    }

