/// <summary>
///  Script Name : VOD_0366_playback_action_menu_audio_stream_available
///  Test Name   : VOD-0366-VOD playback action menu audio stream available
///  TEST ID     : 73992
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

[Test("VOD-0366")]
    class VOD_0366 : _Test
    {
        [ThreadStatic]
        static _Platform CL;

        static VODAsset vodAsset;
        static EnumLanguage audioLanguage_1;
        static EnumLanguage audioLanguage_2;

        #region Create Structure
        public override void CreateStructure()
        {
            this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
            this.AddStep(new Step1(), "Step 1: Play an asset with default audio language ");
            this.AddStep(new Step2(), "Step 2: Change  audio language and resume ");
            this.AddStep(new Step3(), "Step 3: Change audio language to default and resume");
            
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

                // Get a VOD asset with several audio and subtitle languages
                vodAsset = CL.EA.GetVODAssetFromContentXML("NoOfAudioLanguages=2");
                if (vodAsset == null)
                {
                    FailStep(CL, res, "Failed to get a VOD asset with several audio and subtitle languages from ini file");
                }

                // Get audio  to use                
                res = CL.EA.GetAudioLanguage(vodAsset, 0, ref audioLanguage_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get language of 1st audio");
                }

                res = CL.EA.GetAudioLanguage(vodAsset, 1, ref audioLanguage_2);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get type of 2nd audio");
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

                // Set default audio language
                res = CL.EA.STBSettings.SetPreferredAudioLanguage(audioLanguage_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to set prefered audio language to " + audioLanguage_1.ToString());
                }

                // Play an asset with with several languages and subtitles
                res = CL.EA.VOD.PlayAsset(vodAsset);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to play the asset with several audio and subtitle languages");
                }

                // Check video is played with prefered language
                EnumLanguage audioLanguage = EnumLanguage.Off;
                CL.EA.GetCurrentAudioLanguage(ref audioLanguage);
                if (audioLanguage != audioLanguage_1)
                {
                    FailStep(CL, res, "Failed to check current audio language. Expected: " + audioLanguage_1.ToString() + ", Received: " + audioLanguage.ToString());
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

                // Switch to another audio language from action bar
                res = CL.EA.ChangeAudioTrack(audioLanguage_2,0, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to change audio language in action bar from " + audioLanguage_1.ToString() + " to " + audioLanguage_2.ToString());
                }

                // Check that selected audio language is played
                EnumLanguage audioLanguage = EnumLanguage.Off;
                CL.EA.GetCurrentAudioLanguage(ref audioLanguage);
                if (audioLanguage != audioLanguage_2)
                {
                    FailStep(CL, res, "Failed to check current audio language. Expected: " + audioLanguage_2.ToString() + ", Received: " + audioLanguage.ToString());
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

                // Switch to previous language from action bar
                res = CL.EA.ChangeAudioTrack(audioLanguage_1, 0, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to change audio language in action bar from " + audioLanguage_2.ToString() + " to " + audioLanguage_1.ToString());
                }

                // Check that selected audio language is played
                EnumLanguage audioLanguage = EnumLanguage.Off;
                CL.EA.GetCurrentAudioLanguage(ref audioLanguage);
                if (audioLanguage != audioLanguage_1)
                {
                    FailStep(CL, res, "Failed to check current audio language. Expected: " + audioLanguage_1.ToString() + ", Received: " + audioLanguage.ToString());
                }               

                PassStep();
            }
        }
        #endregion
    }

