/// <summary>
///  Script Name : FullSanity_2008.cs
///  Test Name   : FullSanity-2008-VOD-Audio & Subtitles
///  TEST ID     : 24530
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

// FullSanity-2008-VOD-Audio & Subtitles
public class FullSanity_2008 : _Test

{
    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset;
    static EnumLanguage audioLanguage_1;  
    static EnumLanguage audioLanguage_2;
    static EnumAudioType audioType_1;
    static EnumAudioType audioType_2;
    static EnumLanguage subtitleLanguage_1;
    static EnumLanguage subtitleLanguage_2;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Set default audio language and disable subtitle display");
        this.AddStep(new Step2(), "Step 2: Play an asset with several languages and subtitles");
        this.AddStep(new Step3(), "Step 3: Switch to another language from action menu");
        this.AddStep(new Step4(), "Step 4: Switch to another subtitle from action menu");
        this.AddStep(new Step5(), "Step 5: Stop playback and resume it");
        this.AddStep(new Step6(), "Step 6: Change default audio language and enable subtitle display");
        this.AddStep(new Step7(), "Step 7: Playback the same asset");

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

            // Get a VOD asset with several audio and subtitle languages
            vodAsset = CL.EA.GetVODAssetFromContentXML("NoOfAudioLanguages=2;NoOfSubtitleLanguages=2");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset with several audio and subtitle languages from ini file");
            }

            // Get audio & subtitle languages to use
            res = CL.EA.GetAudioType(vodAsset, 0, ref audioType_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get type of 1st audio");
            }

            res = CL.EA.GetAudioType(vodAsset, 1, ref audioType_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get type of 2nd audio");
            }

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

            res = CL.EA.GetSubtitleLanguage(vodAsset, 0, ref subtitleLanguage_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get language of 1st subtitle");
            }

            res = CL.EA.GetSubtitleLanguage(vodAsset, 1, ref subtitleLanguage_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get type of 2nd subtitle");
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

            //  Disable subtitle display
            res = CL.EA.STBSettings.SetSubtitlesPrefs(true, EnumLanguage.Off);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to disable subtitles");
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

            // Play an asset with with several languages and subtitles
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if(! res.CommandSucceeded )
            {
                FailStep(CL, res, "Failed to play the asset with several audio and subtitle languages");
            }

            // Check video is played with prefered language
            EnumLanguage audioLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentAudioLanguage(ref audioLanguage);
            if(audioLanguage != audioLanguage_1)
            {
                FailStep(CL, res, "Failed to check current audio language. Expected: " + audioLanguage_1.ToString() + ", Received: " + audioLanguage.ToString());
            }
            
            // Check subtitles are not displayed
            EnumLanguage subtitleLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
            if (subtitleLanguage != EnumLanguage.Off)
            {
                FailStep(CL, res, "Failed to check current subtitle language. Expected: OFF, Received: " + subtitleLanguage.ToString());
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

            // Switch to another audio language from action bar
            res = CL.EA.ChangeAudioTrack(audioLanguage_2, audioType_2, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change audio language in action bar from " + audioLanguage_1.ToString () + " to " + audioLanguage_2.ToString());
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

    #region Step4
    private class Step4 : _Step
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
                FailStep(CL, res, "Failed to stop the playback");
            }

            //  Resume the playback
            res = CL.EA.VOD.PlayAsset(vodAsset, false);
            if(! res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resume playback");
            }

            // Check video is played with prefered language
            EnumLanguage audioLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentAudioLanguage(ref audioLanguage);
            if (audioLanguage != audioLanguage_1)
            {
                FailStep(CL, res, "Failed to check current audio language. Expected: " + audioLanguage_1.ToString() + ", Received: " + audioLanguage.ToString());
            }

            // Check subtitles are not displayed
            EnumLanguage subtitleLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
            if (subtitleLanguage != EnumLanguage.Off)
            {
                FailStep(CL, res, "Failed to check current subtitle language. Expected: OFF, Received: " + subtitleLanguage.ToString());
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

            // Select another prefered audio language in settings
            res = CL.EA.STBSettings.SetPreferredAudioLanguage(audioLanguage_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set prefered audio language to " + audioLanguage_2.ToString());
            }

            // Select another prefered subtitle language in settings
            res = CL.EA.STBSettings.SetSubtitlesPrefs(true, subtitleLanguage_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set subtitle language to " + subtitleLanguage_2.ToString() + " in Settings");
            }   

            PassStep();
        }
    }
    #endregion

    #region Step7
    private class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Playback the same VOD asset
            res = CL.EA.VOD.PlayAsset(vodAsset, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the playback");
            }

            // Check that prefered audio language is played
            EnumLanguage audioLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentAudioLanguage(ref audioLanguage);
            if (audioLanguage != audioLanguage_2)
            {
                FailStep(CL, res, "Failed to check current audio language. Expected: " + audioLanguage_2.ToString() + ", Received: " + audioLanguage.ToString());
            }

            // Check that prefered subtitle language is displayed
            EnumLanguage subtitleLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
            if (subtitleLanguage != subtitleLanguage_2)
            {
                FailStep(CL, res, "Failed to check current subtitle language. Expected: " + subtitleLanguage_2.ToString() + ", Received: " + subtitleLanguage.ToString());
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}





