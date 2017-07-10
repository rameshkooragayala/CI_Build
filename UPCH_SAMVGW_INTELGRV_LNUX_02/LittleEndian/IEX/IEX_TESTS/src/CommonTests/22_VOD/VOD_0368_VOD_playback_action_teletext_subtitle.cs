/// <summary>
///  Script Name : VOD-0368-VOD playback action teletext subtitle
///  Test Name   : VOD-0368-VOD playback action teletext subtitle  74005
///  TEST ID     : 74005
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

[Test("VOD-0368")]
class VOD_0368 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset;
    static EnumLanguage subtitleLanguage_1;
    static EnumLanguage subtitleLanguage_2;
    static EnumSubtitleType subtitletype_1;
    static EnumSubtitleType subtitletype_2;



    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play an asset with teltext subtitle");
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

            // Get a VOD asset with teletext subtitle 
            vodAsset = CL.EA.GetVODAssetFromContentXML("NoOfSubtitleLanguages=2;SubtitleType=Teletext");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset with teletext subtitle");
            }

            // Get  subtitle languages to use
            res = CL.EA.GetSubtitleLanguage(vodAsset, 0, ref subtitleLanguage_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get language of subtitle");

            }
            // Get  subtitle type to use
            res = CL.EA.GetSubtitleType(vodAsset, 0, ref subtitletype_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get type of 1st subtitle");
            }

            // Get  subtitle languages to use
            res = CL.EA.GetSubtitleLanguage(vodAsset, 1, ref subtitleLanguage_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get type of 2nd subtitle");
            }
            // Get  subtitle type to use
            res = CL.EA.GetSubtitleType(vodAsset, 1, ref subtitletype_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get type of 1st subtitle");
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


            // Play an asset with with teletxt subtitle
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the asset with  teletext subtitle ");
            }

            // Select a subtitle language from action menu
            res = CL.EA.SubtitlesLanguageChange(subtitleLanguage_1, subtitletype_1);
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

            // Check subtitles type  is correct
            EnumSubtitleType subtitleType = EnumSubtitleType.NORMAL;
            CL.EA.GetCurrentSubtitleType(ref subtitleType);
            if (subtitleType != subtitletype_1)
            {
                FailStep(CL, res, "Failed to check current subtitle Type. Expected: " + subtitleLanguage_1.ToString() + ", Received: " + subtitleType.ToString());
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
            if (subtitleLanguage != EnumLanguage.Off)
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
            res = CL.EA.SubtitlesLanguageChange(subtitleLanguage_2, subtitletype_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change subtitle language in action bar from OFF to " + subtitleLanguage_1.ToString());
            }



            // Check that prefered subtitle language is displayed
            EnumLanguage subtitleLanguage = EnumLanguage.Off;
            CL.EA.GetCurrentSubtitleLanguage(ref subtitleLanguage);
            if (subtitleLanguage != subtitleLanguage_2)
            {
                FailStep(CL, res, "Failed to check current subtitle language. Expected: " + subtitleLanguage_2.ToString() + ", Received: " + subtitleLanguage.ToString());
            }

            // Check subtitles type  is correct
            EnumSubtitleType subtitleType = EnumSubtitleType.NORMAL;
            CL.EA.GetCurrentSubtitleType(ref subtitleType);
            if (subtitleType != subtitletype_2)
            {
                FailStep(CL, res, "Failed to check current subtitle Type. Expected: " + subtitleLanguage_2.ToString() + ", Received: " + subtitleType.ToString());
            }
            PassStep();
        }
    }
    #endregion

}

