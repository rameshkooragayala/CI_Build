/// <summary>
///  Script Name : MigrationTest
///  Test Name   : Migration tests
///  TEST ID     : NA
///  QC Version  : NA
///  Variations From QC: NA
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date:12th Jan, 2014
/// </summary>

using System;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using System.Collections;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

public class MigrationTest : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service service;
    private static Service service1;
    private static Service service2;
    private static Service lockedService;
    private static string powerMode;
    static string mainVideoCable;
    static string hdResolution;
    static string tvColorOutput;
    static string formatConversion;
    static string hdmiOutput;
    static string spdifOutput;
    static string dolbyDigitalOutput;
    static string spdifAudioDelay;
    static string hdmiAudioDelay;
    static string sgtFriendlyName = "";
    static string egtFriendlyName = "";
    static string diskSpaceManagement;
    static string seriesRecording;
    static string transparency;
    static string infoMenuTimeout;
    static string tvGuideBackground;
    static string preferredSubtitleLanguage;
    static string hardOfHearingSubtitles;
    static string autoStandBy;
    static string nightTimeStartTime;
    static string nightTimeEndTime;
    static string activateStandbyAfterTime;
    static string purchaseProtection;
    static string frontPanelBrightness;
    static string iniSoftwareVersion;
    static string isFavouriteMode;
    private static string defaultPin;
    static string oldVersion = "";
    static string usageID = "";
    static string otaDownloadOption = "";
    static string rfFeed = "";
    static string isLastDelivery = "";
    static string nitTable = "";
    static string defaultNitTable = "";
    static string isMigration = "";
    static Helper helper = new Helper();


    //Shared members between steps
    private static class Constants
    {
        public const string newPIN = "1111";
        public const string currentid = "2"; //id of Service 2 before reordering. 
        public const string reorderid = "1"; //id of Service 2 after reordering.
    }

    #region Create Structure

    public override void CreateStructure()
    {
          this.AddStep(new PreCondition(), "Precondition: Change all the required settings");
          this.AddStep(new Step1(), "Step 1: Broadcast and Download the SSU and bring up the box");
          this.AddStep(new Step2(), "Step 2: Verify that all the settings were same as before the SSU Download");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }
            service1 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + service.LCN);
            if (service1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service1.LCN);
            }
            service2 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + service.LCN + "," + service1.LCN);
            if (service2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service2.LCN);
            }

            //Fetch the Default values from the  ini files
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (powerMode == "")
            {
                FailStep(CL, "Failed to get the POWER_MODE value from test ini");
            }


            mainVideoCable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MAIN_VIDEO_CABLE");
            if (mainVideoCable == "")
            {
                FailStep(CL, "Failed to get the MAIN_VIDEO_CABLE value from test ini");
            }


            hdResolution = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "HD_RESOLUTION");
            if (hdResolution == "")
            {
                FailStep(CL, "Failed to get the HD_RESOLUTION value from test ini");
            }


            tvColorOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TV_COLOR_OUTPUT");
            if (tvColorOutput == "")
            {
                FailStep(CL, "Failed to get the TV_COLOR_OUTPUT value from test ini");
            }


            formatConversion = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FORMAT_CONVERSION");
            if (formatConversion == "")
            {
                FailStep(CL, res, "Failed to get the FORMAT_CONVERSION value from test ini");
            }



            hdmiOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "HDMI_OUTPUT");
            if (hdmiOutput == "")
            {
                FailStep(CL, "Failed to get the HDMI_OUTPUT value from test ini");
            }


            spdifOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SPDIF_OUTPUT");
            if (spdifOutput == "")
            {
                FailStep(CL, res, "Failed to get the SPDIF_OUTPUT value from test ini");
            }


            dolbyDigitalOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DOLBY_DIGITAL_OUTPUT");
            if (dolbyDigitalOutput == "")
            {
                FailStep(CL, "Failed to get the DOLBY_DIGITAL_OUTPUT value from test ini");
            }


            spdifAudioDelay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SPDIF_AUDIO_DELAY");
            if (spdifAudioDelay == "")
            {
                FailStep(CL, "Failed to get the SPDIF_AUDIO_DELAY value from test ini");
            }


            hdmiAudioDelay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "HDMI_AUDIO_DELAY");
            if (hdmiAudioDelay == "")
            {
                FailStep(CL, "Failed to get the HDMI_AUDIO_DELAY value from test ini");
            }


            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, "Failed to get the SGT value from test ini");
            }


            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT");
            if (egtFriendlyName == "")
            {
                FailStep(CL, "Failed to get the EGT value from test ini");
            }


            diskSpaceManagement = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DISK_SPACE_MANAGEMENT");
            if (diskSpaceManagement == "")
            {
                FailStep(CL, "Failed to get the DISK_SPACE_MANAGEMENT value from test ini");
            }


            seriesRecording = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERIES_RECORDING");
            if (seriesRecording == "")
            {
                FailStep(CL, "Failed to get the SERIES_RECORDING value from test ini");
            }


            transparency = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TRANSPARENCY");
            if (transparency == "")
            {
                FailStep(CL, "Failed to get the TRANSPARENCY value from test ini");
            }



            infoMenuTimeout = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "INFO_MENU_TIMEOUT");
            if (infoMenuTimeout == "")
            {
                FailStep(CL, "Failed to get the INFO_MENU_TIMEOUT value from test ini");
            }


            tvGuideBackground = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TV_GUIDE_BACKGROUND");
            if (tvGuideBackground == "")
            {
                FailStep(CL, "Failed to get the TV_GUIDE_BACKGROUND value from test ini");
            }

            preferredSubtitleLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SUBTITLE_LANGUAGE");
            if (preferredSubtitleLanguage == "")
            {
                FailStep(CL, "Failed to get the SUBTITLE_LANGUAGE value from test ini");
            }


            hardOfHearingSubtitles = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "HARD_OF_HEARING_SUBTITLE");
            if (hardOfHearingSubtitles == "")
            {
                FailStep(CL, "Failed to get the hardOfHearingSubtitles value from test ini");
            }

            autoStandBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "AUTO_STANDBY");
            if (autoStandBy == "")
            {
                FailStep(CL, "Failed to get the autoStandBy value from test ini");
            }

            activateStandbyAfterTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ACTIVATE_STANDBY_AFTER");
            if (activateStandbyAfterTime == "")
            {
                FailStep(CL, "Failed to get the activateStandbyAfterTime value from test ini");
            }

            frontPanelBrightness = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FRONT_PANEL_BRIGHTNESS");
            if (frontPanelBrightness == "")
            {
                FailStep(CL, "Failed to get the frontPanelBrightness value from test ini");
            }

            purchaseProtection = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PURCHASE_PROTECTION");
            if (purchaseProtection == "")
            {
                FailStep(CL, "Failed to get the purchaseProtection value from test ini");
            }

            string lockableService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LOCKABLE_SERVICE");
            if (lockableService == "")
            {
                FailStep(CL, "Failed to get the LOCKABLE_SERVICE from test ini");
            }


            lockedService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN=" + lockableService, "ParentalRating=High;LCN=" + service.LCN + "," + service1.LCN + "," + service2.LCN);
            if (lockedService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + lockedService.LCN);
            }
            isMigration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_MIGRATION");
            if (isMigration == "")
            {
                FailStep(CL, "Failed to get the IS_MIGRATION from Test ini");
            }

            iniSoftwareVersion = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SOFTWARE_VERSION");
            if (iniSoftwareVersion == "")
            {
                FailStep(CL, "Failed to get the iniSoftwareVersion from Test ini");
            }

            isFavouriteMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_FAVOURITEMODE");
            if (isFavouriteMode == "")
            {
                FailStep(CL, "Failed to get the IS_FAVOURITEMODE from Test ini");
            }
            res = CL.EA.STBSettings.SetUnLockChannel(lockedService.Name);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to unlock channel");
            }

            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unset all the services from the Favourite");
            }
			
		res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all records from Achive");
        }
		
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to cancel all bookings from Planner");
        }
		
            if (isMigration.ToUpper() == "TRUE")
            {
                //Mounting the Box with the Old Binary
                res = CL.EA.MountGw(EnumMountAs.NOFORMAT, IsLastDelivery: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Mount the last delivery image");
                }
                CL.IEX.Wait(120);
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to come out of stand by after mouting last delivery binary");
                }
                CL.IEX.Wait(20);
            }


           // Personal Suggestion

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }
            //Changing the Default pin to new pin
            // res = CL.EA.STBSettings.ChangePinCode(defaultPin, Constants.newPIN);
            // if (!res.CommandSucceeded)
            //  {
            //     FailStep(CL, "Failed to change pin code");
            // }

            // Changing the Video Settings
            // Changing the Main video cable
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS MAINVIDEOCABLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:VIDEO SETTINGS MAINVIDEOCABLE");
            }
            res = CL.IEX.MilestonesEPG.Navigate(mainVideoCable);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + mainVideoCable);
            }
            //Chaning the Video Settings HD resolution
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS HDRESOLUTION");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:VIDEO SETTINGS HDRESOLUTION");
            }
            res = CL.IEX.MilestonesEPG.Navigate(hdResolution);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + hdResolution);
            }

            //Verify for Live State
            bool ConfirmatioState = CL.EA.UI.Utils.VerifyState("SETTINGS CONFIRMATION STATE", 120);
            if (!ConfirmatioState)
            {
                FailStep(CL, res, "Failed to verify ConfirmatioState");

            }
            else
            {
                LogCommentInfo(CL, "Box is in ConfirmatioState");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.Navigate("YES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to YES");
            }
            //Changing the Video settings TV color Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:TV COLOR OUTPUT");
            }
            res = CL.IEX.MilestonesEPG.Navigate(tvColorOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + tvColorOutput);
            }
            //Verify for Live State
            bool ConfirmatioState1 = CL.EA.UI.Utils.VerifyState("SETTINGS CONFIRMATION STATE", 120);
            if (!ConfirmatioState1)
            {
                FailStep(CL, res, "Failed to verify ConfirmatioState");

            }
            else
            {
                LogCommentInfo(CL, "Box is in ConfirmatioState");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.Navigate("YES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to YES");
            }
            //Chaning the Video settings for Format Conversion
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:4:3 VIDEO DISPLAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:4:3 VIDEO DISPLAY");
            }
            res = CL.IEX.MilestonesEPG.Navigate(formatConversion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + formatConversion);
            }

            //Change the Audio Settings
            //Changing the Audio settings for HDMI Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:HDMI OUTPUT");
            }
            res = CL.IEX.MilestonesEPG.Navigate(hdmiOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + hdmiOutput);
            }
            //Changing the Audio settings for SPDIF Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:S//PDIF OUTPUT");
            }
            res = CL.IEX.MilestonesEPG.Navigate(spdifOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + spdifOutput);
            }
            //Changing the Audio Settings for Dolby Digital Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI DIGITAL DOLBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:HDMI DIGITAL DOLBY");
            }
            res = CL.IEX.MilestonesEPG.Navigate(dolbyDigitalOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + dolbyDigitalOutput);
            }
            //Changing the Audio Settings for SPDIF Audio Delay
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF AUDIO DELAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:S//PDIF AUDIO DELAY");
            }
            res = CL.IEX.MilestonesEPG.Navigate(spdifAudioDelay);
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "Due to the Migration Rollback there is a change in EPG Navigation which is hardcoded and handled");
                string title = "";
                string firsttitle = "";
                string timeStamp = "";
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out firsttitle);
                if (firsttitle == spdifAudioDelay)
                {
                    CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
                }
                else
                {
                    while (title != spdifAudioDelay || title == firsttitle)
                    {
                        CL.IEX.Wait(2);
                        CL.IEX.SendIRCommand("SELECT_LEFT", -1, ref timeStamp);
                        CL.IEX.Wait(2);
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
                    }
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
            }
            //Changing the HDMI Audio Delay
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI AUDIO DELAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:HDMI AUDIO DELAY");
            }
            res = CL.IEX.MilestonesEPG.Navigate(hdmiAudioDelay);
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "Due to the Migration Rolllback there is a change in EPG Navigation which is hardcoded and handled");
                string title = "";
                string firsttitle = "";
                string timeStamp = "";
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out firsttitle);
                if (firsttitle == hdmiAudioDelay)
                {
                    CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
                }
                else
                {
                    while (title != hdmiAudioDelay || title == firsttitle)
                    {
                        CL.IEX.Wait(2);
                        CL.IEX.SendIRCommand("SELECT_LEFT", -1, ref timeStamp);
                        CL.IEX.Wait(2);
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
                    }
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
            }
            //Recordings
            //Changing the SGT
            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the SGT to " + sgtFriendlyName);
            }
            LogCommentInfo(CL, "Successfully set the SGT to " + sgtFriendlyName);

            //Changing the EGT
            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the EGT to " + egtFriendlyName);
            }
            LogCommentInfo(CL, "Successfully set the EGT to " + egtFriendlyName);

            //Changing theDisk Space Management

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:DISK SPACE MANAGEMENT");
            }
            res = CL.IEX.MilestonesEPG.Navigate(diskSpaceManagement);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + diskSpaceManagement);
            }
            //Changing the series Recording
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SERIES RECORDINGT");
            }
            res = CL.IEX.MilestonesEPG.Navigate(seriesRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + seriesRecording);
            }
            //Changes the settings for Preferences
            //Changing the Menu Language
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MENU LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:MENU LANGUAGE");
            }
            //Changing the Transparency
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TRANSPARENCY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:TRANSPARENCY");
            }
            res = CL.IEX.MilestonesEPG.Navigate(transparency);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + transparency);
            }
            //Changing the Banner Display Time out
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Banner display time to 10");
            }
            else
            {
                LogCommentInfo(CL, "Successfully set the Banner Display time out to 10 sec");
            }
            //Changing the TV Guide Back ground to Solid
            res = CL.EA.STBSettings.SetTvGuideBackgroundAsSolid();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the TV Guide Back ground to Solid");
            }

            //Favourites
            //Unsetting all the Favourites and setting few services as favourites
            // res = CL.EA.STBSettings.UnsetAllFavChannels();
            //  if (!res.CommandSucceeded)
            //  {
            //      FailStep(CL, res, "Failed to unset all the services from the Favourite");
            //  }

            res = CL.EA.STBSettings.SetFavoriteChannelNumList(service.LCN + "," + service1.LCN + "," + service2.LCN, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set services as Favourites");
            }
            //Change the ordering of the Favourite Services
            if (!helper.ReorderFavourites())
            {
                FailStep(CL, "Failed to reorder the Favourites");
            }

            //Preferred Audio Language
            res = CL.EA.STBSettings.SetPreferredAudioLanguage(EnumLanguage.Dutch);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Preferred Audio Language to Dutch");
            }

            //Preferred Subtitle Language

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLES SETTING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SUBTITLES SETTING");
            }
            res = CL.IEX.MilestonesEPG.Navigate(preferredSubtitleLanguage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + preferredSubtitleLanguage);
            }


            // Hard Of Hearing Subtitle Language
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLE HARD OF HEARING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SUBTITLE HARD OF HEARING");
            }

            //Power Management
            //Navigate to Active Stand by after to 30 min
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully to : " + powerMode);
            }

            //Setting the Auto Stand by
            res = CL.EA.STBSettings.SetAutoStandBy(autoStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Faile to set the Auto Stand by to ");
            }

            //Setting the Night time

            //Refreeshing the EPG to get the Current time from EPG
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to main menu", exitTest: false);
            }
            string EPG_Time = "";
            CL.EA.UI.Live.GetEpgTime(ref EPG_Time);
            LogCommentInfo(CL, "Current EPG Time: " + EPG_Time);

            DateTime startTime = DateTime.Parse(EPG_Time);
            startTime = startTime.AddMinutes(2);
            DateTime endTime = startTime.AddHours(1);

            nightTimeStartTime = Convert.ToString(startTime);
            LogCommentInfo(CL, "Start Time: " + nightTimeStartTime);

            nightTimeEndTime = Convert.ToString(endTime);
            LogCommentInfo(CL, "End Time: " + nightTimeEndTime);
            res = CL.EA.STBSettings.SetNightTime(nightTimeStartTime, nightTimeEndTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set night time.");
            }

            //Changing the Active Stand by After 
            res = CL.EA.STBSettings.ActivateAutoStandByAfterTime(activateStandbyAfterTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Activate the Standby after time to ");
            }

            //Fron Panel Brightness
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:FRONT PANEL BRIGHTNESS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:FRONT PANEL BRIGHTNESS");
            }

            res = CL.IEX.MilestonesEPG.Navigate(frontPanelBrightness);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to FRONT_PANEL_BRIGHTNESS");
            }
            //Parental Control

            //Setting the Parental control age limit to FSK 12
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.FSK_12);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Parental control Age limit to 12");
            }
            //Locking the Service 5
            res = CL.EA.STBSettings.SetLockChannel(lockedService.Name);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to lock the service");
            }

            //Pin Management

            //Setting the Purchase Protection to On
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Purchase Protection to True");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service");
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED", MinTimeBeforeEvEnd: 3, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from Banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service");
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED1", MinTimeBeforeEvEnd: 3, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from Banner");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED", service1.Name, DaysDelay: -1, MinutesDelayUntilBegining: 120, DurationInMin: 10,VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED1", service2.Name, DaysDelay: -1, MinutesDelayUntilBegining: 120, DurationInMin: 10,VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }

            res = CL.EA.WaitUntilEventEnds("EVENT_BASED1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until Event ends");
            }
            if (isFavouriteMode.ToUpper() == "TRUE")
            {
                //if DISABLE FAVOURITE MODE is present on the ACTION BAR,then no need to navigate to "STATE:ENABLE FAVOURITE MODE"
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
                if (res.CommandSucceeded)
                {
                    res = CL.IEX.MilestonesEPG.SelectMenuItem("DISABLE FAVOURITE MODE");
                    if (res.CommandSucceeded)
                    {
                        CL.IEX.LogComment("DISABLE FAVOURITE MODE is present on the ACTION BAR");
                    }
                    else
                    {
                        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ENABLE FAVOURITE MODE");
                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, "Failed to Set FAVOURITE mode");
                        }
                    }
                }

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, "1");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to Favourite service in Favourite Mode");
                }
            }
            else
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to Favourite service in Favourite Mode");
                }
            }


            
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Navigate to Diagnostics and get the Software version and Usage ID
            res = CL.EA.GetAndVerifySoftVersion(ref oldVersion, ref usageID);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the Software version from EPG");
            }
            else
            {
                LogCommentInfo(CL, "Fetched values from Diagnostics : Diagnostics - " + oldVersion + " UsageID - " + usageID);
            }

            //Fetching the Last delivery from Test Ini using which we will decide the Decide the download on either Last delivery or current
            isLastDelivery = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_LASTDELIVERY");
            if (isLastDelivery == "")
            {
                FailStep(CL, "Failed to get the is last delivery from Project ini");
            }
            else
            {
                LogCommentInfo(CL, "Is LastDelivery fetched from Test ini " + isLastDelivery);
            }
            //Fetching the RF feed from Test ini
            rfFeed = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RF_FEED");
            if (rfFeed == "")
            {
                FailStep(CL, "Failed to get the RF feed from Test ini");
            }
            else
            {
                LogCommentInfo(CL, "RF Feed fetched from Test ini " + rfFeed);
            }
            //Fetching the OTA download option Forced, Manual or Automatic
            otaDownloadOption = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "OTA_DOWNLOAD_OPTION");
            if (otaDownloadOption == "")
            {
                FailStep(CL, "Failed to fetch the otaDownloadOption from Project ini");
            }
            else
            {
                LogCommentInfo(CL, "OTA Download option fetched from Test ini " + otaDownloadOption);
            }
            //NIT table from Test ini
            nitTable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NIT_TABLE");
            if (nitTable == "")
            {
                FailStep(CL, "Failed to get the nit Table value from test ini file");
            }
            else
            {
                LogCommentInfo(CL, "Nit Table fetched from test ini " + nitTable);
            }
            defaultNitTable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFAULT_NIT_TABLE");
            if (defaultNitTable == "")
            {
                FailStep(CL, "Failed to get the Default nit Table value from test ini file");
            }
            else
            {
                LogCommentInfo(CL, "Default Nit Table fetched from test ini " + defaultNitTable);
            }
            string timeStamp = "";

            res = CL.IEX.SendIRCommand("MENU", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.SendIRCommand("RETOUR", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }
            //Verify for Live State
            bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 120);
            if (!liveState)
            {
                FailStep(CL, res, "Failed to verify Live");

            }
            else
            {
                LogCommentInfo(CL, "Box is in LIVE");
            }
            if (isMigration.ToUpper() == "TRUE")
            {
                //MIgration script will use the existing EA as we have flashed the old binary and changed the Settings
                //Download binary On Air
                res = CL.EA.OtaDownload(oldVersion, usageID, nitTable, Convert.ToBoolean(isLastDelivery), rfFeed, IsLive: true);
                if (!(res.CommandSucceeded))
                {
                    FailStep(CL, res, "Failed to download binary on air");
                }
                else
                {
                    LogCommentInfo(CL, "Binary downloaded successfully on the box");
                }
            }
            else
            {
                try
                {
                    //Rollback we have changed the Settings on the current build and broadcasting old binary
                    CL.EA.UI.OTA.CopyOldBinary();
                    //Modify Imgae version
                    CL.EA.UI.OTA.ModifyImageVersion(oldVersion);

                    //Create carousel
                    CL.EA.UI.OTA.Create_Carousel(oldVersion, usageID, rfFeed);

                    //Broadcast and rest carousel
                    CL.EA.UI.OTA.BroadcastCarousel(rfFeed);

                    //Broadcast NIT
                    CL.EA.UI.OTA.NITBraodcast(nitTable);
                }
                catch
                {
                    FailStep(CL, "Failed to broadcast the Old binary on air");
                }
            }

            //Verify the Download option
            if (otaDownloadOption.ToUpper() == "FORCED")
            {
                res = CL.EA.OtaDownloadOption(EnumOTADownloadOption.FORCED);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Download option Forced");
                }
            }
            else
            {
                res = CL.EA.OtaDownloadOption(EnumOTADownloadOption.AUTOMATIC);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Download option Automatic");
                }
            }

            //Mount the Gateway
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to mount the Gateway");
            }

            LogCommentInfo(CL, "Waiting fews seconds for the box to come to live");

            CL.IEX.Wait(100);
            string acceptScreen = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out acceptScreen);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to get the title from EPG");
            }
            if (acceptScreen == "ACCEPT")
            {
                LogCommentInfo(CL, "Selecting Accept Screen");
                CL.EA.UI.Utils.SendIR("SELECT");
            }
            CL.IEX.Wait(15);
            //check for live state
            bool liveState1 = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (!liveState1)
            {
                FailStep(CL, res, "Unable to verify the live state after on air OTA download");

            }
            //Verifying the Software version with the old version and comparing
            res = CL.EA.GetAndVerifySoftVersion(ref oldVersion, ref usageID, IsVerify: true, OldSoftVersion: oldVersion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the Software version from EPG");
            }
            string softwareVersion="";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("software version", out softwareVersion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to fetch the Software version from the EPG");
            }
            string[] softwareVersionarr = softwareVersion.Split('-');
            softwareVersion = softwareVersionarr[1].Substring(0, 2);
            if (softwareVersion == iniSoftwareVersion)
            {
                LogCommentImportant(CL, "Verified that the Software Version " + softwareVersion + " is same as expected " + iniSoftwareVersion);
            }
            else
            {
                FailStep(CL,"Failed to Verify that the Software Version " + softwareVersion + " is same as expected " + iniSoftwareVersion);
            }

            res = CL.IEX.SendIRCommand("RETOUR", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.SendIRCommand("RETOUR", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }
            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Verifying the Video Settings after Migration
            //Changing the Default pin to new pin
            //Entering the new pin and changing it back to the default pin
            //  res = CL.EA.STBSettings.ChangePinCode(Constants.newPIN, defaultPin);
            // if (!res.CommandSucceeded)
            //  {
            //     FailStep(CL, "Failed to verify that the change pin codeis not reverted back to the default value");
            // }
            // LogCommentImportant(CL, "We are able to enter new pin and change it to default pin which suggests that the settings were retained after Factory Reset");

            //Verifying the Main Video Cable
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS MAINVIDEOCABLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:VIDEO SETTINGS MAINVIDEOCABLE");
            }
            string obtainedMainVideoCable = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedMainVideoCable);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for MainVideoCable");
            }
            if (obtainedMainVideoCable == mainVideoCable)
            {
                LogCommentImportant(CL, "Verified the Main Video Cable after Migration " + obtainedMainVideoCable + " is same as expected " + mainVideoCable);
            }
            else
            {
                FailStep(CL, "Failed to Verify the Main Video Cable after Migration " + obtainedMainVideoCable + " is same as expected " + mainVideoCable);
            }
            //Verifing the HD resolution

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS HDRESOLUTION");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:VIDEO SETTINGS HDRESOLUTION");
            }
            string obtainedHDResolution = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedHDResolution);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for HDResolution");
            }
            if (obtainedHDResolution == hdResolution)
            {
                LogCommentImportant(CL, "Verified the HD Resolution after Migration " + obtainedHDResolution + " is same as expected " + hdResolution);
            }
            else
            {
                FailStep(CL, "Failed to Verify the HD Resolution after Migration " + obtainedHDResolution + " is same as expected " + hdResolution);
            }
            //Verifying the TV Color Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:TV COLOR OUTPUT");
            }
            string obtainedTVColorOutput = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedTVColorOutput);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for oTVColorOutput");
            }
            if (obtainedHDResolution == hdResolution)
            {
                LogCommentImportant(CL, "Verified the TV Color Output after Migration " + obtainedTVColorOutput + " is same as expected " + tvColorOutput);
            }
            else
            {
                FailStep(CL, "Failed to Verify the TV Color Output after Migration " + obtainedTVColorOutput + " is same as expected " + tvColorOutput);
            }
            //Verifying the Format Conversion
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:4:3 VIDEO DISPLAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:4:3 VIDEO DISPLAY");
            }
            string obtainedFormatConversion = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedFormatConversion);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for formatConversion");
            }
            if (obtainedFormatConversion == formatConversion)
            {
                LogCommentImportant(CL, "Verified the formatConversion after Migration " + obtainedFormatConversion + " is same as expected " + formatConversion);
            }
            else
            {
                FailStep(CL, "Failed to Verify the formatConversion after Migration " + obtainedFormatConversion + " is same as expected " + formatConversion);
            }

            //Change the Audio Settings
            //Verifying the HDMI Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:HDMI OUTPUT");
            }
            string obtainedHDMIOutput = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedHDMIOutput);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for hdmiOutput");
            }
            if (obtainedHDMIOutput == hdmiOutput)
            {
                LogCommentImportant(CL, "Verified the hdmiOutput after Migration " + obtainedHDMIOutput + " is same as expected " + hdmiOutput);
            }
            else
            {
                FailStep(CL, "Failed to Verify the hdmiOutput after Migration " + obtainedHDMIOutput + " is same as expected " + hdmiOutput);
            }
            //Verifying the SPDIF Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:S//PDIF OUTPUT");
            }
            string obtainedSPDIFOutput = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSPDIFOutput);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for spdifOutput");
            }
            if (obtainedSPDIFOutput == spdifOutput)
            {
                LogCommentImportant(CL, "Verified the spdifOutput after Migration " + obtainedSPDIFOutput + " is same as expected " + spdifOutput);
            }
            else
            {
                FailStep(CL, "Failed to Verify the spdifOutput after Migration " + obtainedSPDIFOutput + " is same as expected " + spdifOutput);
            }

            //Verifying the Dolby Digital Output
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI DIGITAL DOLBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:HDMI DIGITAL DOLBY");
            }
            string obtainedDolbyDigitalOutput = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDolbyDigitalOutput);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for dolbyDigitalOutput");
            }
            if (obtainedDolbyDigitalOutput == dolbyDigitalOutput)
            {
                LogCommentImportant(CL, "Verified the dolbyDigitalOutput after Migration " + obtainedDolbyDigitalOutput + " is same as expected " + dolbyDigitalOutput);
            }
            else
            {
                FailStep(CL, "Failed to Verify the dolbyDigitalOutput after Migration " + obtainedDolbyDigitalOutput + " is same as expected " + dolbyDigitalOutput);
            }

            //Changing the SPDIF Audio Delay
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF AUDIO DELAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:S//PDIF AUDIO DELAY");
            }
            string obtainedSPDIFAudioDelay = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSPDIFAudioDelay);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for spdifAudioDelay");
            }
            if (obtainedSPDIFAudioDelay == spdifAudioDelay)
            {
                LogCommentImportant(CL, "Verified the spdifAudioDelay after Migration " + obtainedSPDIFAudioDelay + " is same as expected " + spdifAudioDelay);
            }
            else
            {
                FailStep(CL, "Failed to Verify the spdifAudioDelay after Migration " + obtainedSPDIFAudioDelay + " is same as expected " + spdifAudioDelay);
            }
            //Changing the HDMI Audio Delay
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI AUDIO DELAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:HDMI AUDIO DELAY");
            }
            string obtainedHDMIAudioDelay = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedHDMIAudioDelay);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for hdmiAudioDelay");
            }
            if (obtainedHDMIAudioDelay == hdmiAudioDelay)
            {
                LogCommentImportant(CL, "Verified the hdmiAudioDelay after Migration " + obtainedHDMIAudioDelay + " is same as expected " + hdmiAudioDelay);
            }
            else
            {
                FailStep(CL, "Failed to Verify the hdmiAudioDelay after Migration " + obtainedHDMIAudioDelay + " is same as expected " + hdmiAudioDelay);
            }
            //Recordings
            //Verifying the SGT
            string expectedSGT = "";
            expectedSGT = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EXPECTED_SGT");
            if (expectedSGT == "")
            {
                FailStep(CL, "Failed to get the EXPECTED_SGT from test ini");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:EXTRA TIME BEFORE PROGRAMME");
            }
            string obtainedSGT = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSGT);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for SGT");
            }
            if (obtainedSGT == expectedSGT)
            {
                LogCommentImportant(CL, "Verified the SGT after Migration " + obtainedSGT + " is same as expected " + expectedSGT);
            }
            else
            {
                FailStep(CL, "Failed to Verify the SGT after Migration " + obtainedSGT + " is same as expected " + expectedSGT);
            }

            //Verifying the EGT
            string expectedEGT = "";
            expectedEGT = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EXPECTED_EGT");
            if (expectedEGT == "")
            {
                FailStep(CL, "Failed to get the EXPECTED_EGT from test ini");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:EXTRA TIME AFTER PROGRAMME");
            }
            string obtainedEGT = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedEGT);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for EGT");
            }
            if (obtainedEGT == expectedEGT)
            {
                LogCommentImportant(CL, "Verified the EGT after Migration " + obtainedEGT + " is same as expected " + expectedEGT);
            }
            else
            {
                FailStep(CL, "Failed to Verify the EGT after Migration " + obtainedEGT + " is same as expected " + expectedEGT);
            }

            //Verifying the Disk Space Management
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:DISK SPACE MANAGEMENT");
            }
            string obtainedDiskSpaceManagement = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDiskSpaceManagement);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for diskSpaceManagement");
            }
            if (obtainedDiskSpaceManagement == diskSpaceManagement)
            {
                LogCommentImportant(CL, "Verified the diskSpaceManagement after Migration " + obtainedDiskSpaceManagement + " is same as expected " + diskSpaceManagement);
            }
            else
            {
                FailStep(CL, "Failed to Verify the diskSpaceManagement after Migration " + obtainedDiskSpaceManagement + " is same as expected " + diskSpaceManagement);
            }

            //Verifying the Series Recording
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SERIES RECORDINGT");
            }
            string obtainedSeriesRecording = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSeriesRecording);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for seriesRecording");
            }
            if (obtainedSeriesRecording == seriesRecording)
            {
                LogCommentImportant(CL, "Verified the seriesRecording after Migration " + obtainedSeriesRecording + " is same as expected " + seriesRecording);
            }
            else
            {
                FailStep(CL, "Failed to Verify the seriesRecording after Migration " + obtainedSeriesRecording + " is same as expected " + seriesRecording);
            }
            //Changes the settings for Preferences

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MENU LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:MENU LANGUAGE");
            }
            //Verifying the Transparency
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TRANSPARENCY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:TRANSPARENCY");
            }
            string obtainedTransparency = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedTransparency);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for transparency");
            }
            if (obtainedTransparency == transparency)
            {
                LogCommentImportant(CL, "Verified the transparency after Migration " + obtainedTransparency + " is same as expected " + transparency);
            }
            else
            {
                FailStep(CL, "Failed to Verify the transparency after Migration " + obtainedTransparency + " is same as expected " + transparency);
            }
            //Verifying the Channel Bar time out
            string expectedChannelBarTimOut = "";
            expectedChannelBarTimOut = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "INFO_MENU_TIMEOUT");
            if (expectedChannelBarTimOut == "")
            {
                FailStep(CL, "Failed to fetch the EXPECTED_CHANNELBAR_TIMEOUT from Test ini");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:CHANNEL BAR TIME OUT");
            }
            string obtainedChannelBarTimeOut = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedChannelBarTimeOut);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for ChannelBarTimOut");
            }
            if (obtainedChannelBarTimeOut == expectedChannelBarTimOut)
            {
                LogCommentImportant(CL, "Verified the ChannelBarTimOut after Migration " + obtainedChannelBarTimeOut + " is same as expected " + expectedChannelBarTimOut);
            }
            else
            {
                FailStep(CL, "Failed to Verify the ChannelBarTimOut after Migration " + obtainedChannelBarTimeOut + " is same as expected " + expectedChannelBarTimOut);
            }
            //Verifying the TV Guide BackGround Settings
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE BACKGROUND SETTINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:TV GUIDE BACKGROUND SETTINGS");
            }
            string obtainedTVGuideBackground = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedTVGuideBackground);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for tvGuideBackground");
            }
            if (obtainedTVGuideBackground == tvGuideBackground)
            {
                LogCommentImportant(CL, "Verified the tvGuideBackground after Migration " + obtainedTVGuideBackground + " is same as expected " + tvGuideBackground);
            }
            else
            {
                FailStep(CL, "Failed to Verify the tvGuideBackground after Migration " + obtainedTVGuideBackground + " is same as expected " + tvGuideBackground);
            }
            //Verifying the order of the Favourites
            if (!helper.VerifyFavouritesOrder())
            {
                FailStep(CL, "Failed to verify the Order of the Favourites after the Download");
            }
            LogCommentImportant(CL, "Succcessfully verified the Order of the Favourites after the Download");

            //Verification of Preferred Audio Language
            string expectedAudioLanguage = "";
            expectedAudioLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "AUDIO_LANGUAGE");
            if (expectedAudioLanguage == "")
            {
                FailStep(CL, "Failed to fetch the AUDIO_LANGUAGE from Test ini");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUDIO LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:AUDIO LANGUAGE");
            }
            string obtainedAudioLanguage = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedAudioLanguage);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for AudioLanguage");
            }
            if (obtainedAudioLanguage == expectedAudioLanguage)
            {
                LogCommentImportant(CL, "Verified the AudioLanguage after Migration " + obtainedAudioLanguage + " is same as expected " + expectedAudioLanguage);
            }
            else
            {
                FailStep(CL, "Failed to Verify the AudioLanguage after Migration " + obtainedAudioLanguage + " is same as expected " + expectedAudioLanguage);
            }
            //Verification of Preferred Subtitle Language
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLES SETTING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SUBTITLES SETTING");
            }
            string obtainedSubtitleLanguage = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSubtitleLanguage);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for preferredSubtitleLanguage");
            }
            if (obtainedSubtitleLanguage == preferredSubtitleLanguage)
            {
                LogCommentImportant(CL, "Verified the preferredSubtitleLanguage after Migration " + obtainedSubtitleLanguage + " is same as expected " + preferredSubtitleLanguage);
            }
            else
            {
                FailStep(CL, "Failed to Verify the preferredSubtitleLanguage after Migration " + obtainedSubtitleLanguage + " is same as expected " + preferredSubtitleLanguage);
            }
            //Verification of Personal Suggestion
            string settingsAfterPinState = "";
            if (isMigration.ToUpper() == "TRUE")
            {
                //settingsAfterPinState = "PERSONALIZED RECOMMENDATION ACTIVATION";
				settingsAfterPinState = "PREFERRED SUBTITLE LANGUAGE";
            }
            else
            {
                settingsAfterPinState = "PREFERRED SUBTITLE LANGUAGE";
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUGGESTED SETTINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:SUGGESTED SETTINGS");
            }
            string Milestones1 = "YES";
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones1, 240);
            res = CL.EA.EnterDeafultPIN(settingsAfterPinState);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enter Pin");
            }

            //End wait for Milestones arrival
            ArrayList arraylist1 = new ArrayList();
            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones1, ref arraylist1))
            {
                FailStep(CL, "Failed to verify title milestone for " + Milestones1);
            }
            LogCommentImportant(CL, "Verified Milestone YES in Personalized Recommendation Activation");
            //Hard Of Hearing Subtitle Language
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HARD OF HEARING SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SUBTITLE HARD OF HEARING");
            }
            string obtainedHardOfHearingSubtitle = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedHardOfHearingSubtitle);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for hardOfHearingSubtitles");
            }
            if (obtainedHardOfHearingSubtitle == hardOfHearingSubtitles)
            {
                LogCommentImportant(CL, "Verified the HardOfHearingSubtitle after Migration " + obtainedHardOfHearingSubtitle + " is same as expected " + hardOfHearingSubtitles);
            }
            else
            {
                FailStep(CL, "Failed to Verify the HardOfHearingSubtitle after Migration " + obtainedHardOfHearingSubtitle + " is same as expected " + hardOfHearingSubtitles);
            }
            //Power Management
            //Navigate to power mode and Verify
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:STANDBY POWER USAGE");
            }
            string obtainedStandByPowerUsage = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedStandByPowerUsage);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for powerMode");
            }
            if (obtainedStandByPowerUsage == powerMode)
            {
                LogCommentImportant(CL, "Verified the powerMode after Migration " + obtainedStandByPowerUsage + " is same as expected " + powerMode);
            }
            else
            {
                FailStep(CL, "Failed to Verify the powerMode after Migration " + obtainedStandByPowerUsage + " is same as expected " + powerMode);
            }

            //Setting the Auto Stand by
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:AUTO STANDBY");
            }

            string obtainedAutoStandBy = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedAutoStandBy);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for autoStandBy");
            }
            if (obtainedAutoStandBy == autoStandBy)
            {
                LogCommentImportant(CL, "Verified the autoStandBy after Migration " + obtainedAutoStandBy + " is same as expected " + autoStandBy);
            }
            else
            {
                FailStep(CL, "Failed to Verify the autoStandBy after Migration " + obtainedAutoStandBy + " is same as expected " + autoStandBy);
            }

            //Verifying the Night Time           

            //Changing the Active Stand by After 

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DEFINE AUTO STANDBY TIME NIGHT START TIME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:AUTO STANDBY");
            }
            string obtainedNightStartTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("data", out obtainedNightStartTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch data milestone for nightTimeStartTime");
            }
            if (obtainedNightStartTime == Convert.ToDateTime(nightTimeStartTime).ToString("HH:mm"))
            {
                LogCommentImportant(CL, "Verified the nightTimeStartTime after Migration " + obtainedNightStartTime + " is same as expected " + nightTimeStartTime);
            }
            else
            {
                FailStep(CL, "Failed to Verify the nightTimeStartTime after Migration " + obtainedNightStartTime + " is same as expected " + nightTimeStartTime);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DEFINE AUTO STANDBY TIME NIGHT END TIME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:DEFINE AUTO STANDBY TIME NIGHT END TIME");
            }

            string obtainedNightEndTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("data", out obtainedNightEndTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch data milestone for nightTimeEndTime");
            }

            if (obtainedNightEndTime == Convert.ToDateTime(nightTimeEndTime).ToString("HH:mm"))
            {
                LogCommentImportant(CL, "Verified the nightTimeEndTime after Migration " + obtainedNightEndTime + " is same as expected " + nightTimeEndTime);
            }
            else
            {
                FailStep(CL, "Failed to Verify the nightTimeEndTime after Migration " + obtainedNightEndTime + " is same as expected " + nightTimeEndTime);
            }

            //Verifying the Active StandBy After
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTIVATE STANDBY AFTER");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to settings ACTIVATE STANDBY AFTER");
            }
            string obtainedActivateStandByAfter = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedActivateStandByAfter);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for activateStandbyAfterTime");
            }
            if (obtainedActivateStandByAfter == activateStandbyAfterTime)
            {
                LogCommentImportant(CL, "Verified the activateStandbyAfterTime after Migration " + obtainedActivateStandByAfter + " is same as expected " + activateStandbyAfterTime);
            }
            else
            {
                FailStep(CL, "Failed to Verify the activateStandbyAfterTime after Migration " + obtainedActivateStandByAfter + " is same as expected " + activateStandbyAfterTime);
            }

            //Fron Panel Brightness
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:FRONT PANEL BRIGHTNESS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:FRONT PANEL BRIGHTNESS");
            }
            string obtainedFrontPanelBrightness = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedFrontPanelBrightness);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for frontPanelBrightness");
            }
            if (frontPanelBrightness == obtainedFrontPanelBrightness)
            {
                LogCommentImportant(CL, "Verified the frontPanelBrightness after Migration " + obtainedFrontPanelBrightness + " is same as expected " + frontPanelBrightness);
            }
            else
            {
                FailStep(CL, "Failed to Verify the frontPanelBrightness after Migration " + obtainedFrontPanelBrightness + " is same as expected " + frontPanelBrightness);
            }
            //Parental Control

            //Setting the Parental control age limit to FSK 12
            //Verifying the Parental control age limit retaind after the Factory Reset
            CL.EA.UI.Settings.NavigateToParentalControlAgeLimit();
            string Milestones = "FSK 12";
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, 240);

            res = CL.EA.EnterDeafultPIN("LOCK PROGRAMMES BY AGE RATING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enter Defualt pin");
            }

            CL.IEX.Wait(2);
            //End wait for Milestones arrival
            ArrayList arraylist = new ArrayList();
            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arraylist))
            {
                FailStep(CL, "Failed to verify title milestone for " + Milestones);
            }
            LogCommentImportant(CL, "Verifed that the Parental control age limit is " + Milestones + " after Facotory Reset which is expected");
            CL.IEX.Wait(10);
            CL.EA.UI.Utils.SendIR("MENU");
            CL.EA.UI.Utils.SendIR("RETOUR");
            CL.IEX.Wait(20);

            //Pin Management

            //Verifying the Purchase Protection
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PURCHASE PROTECTION");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:PURCHASE PROTECTION");
            }
            string obtainedPurchaseProtection = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedPurchaseProtection);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to fetch title milestone for purchaseProtection");
            }
            if (purchaseProtection == obtainedPurchaseProtection)
            {
                LogCommentImportant(CL, "Verified the purchaseProtection after Migration " + obtainedPurchaseProtection + " is same as expected " + purchaseProtection);
            }
            else
            {
                FailStep(CL, "Failed to Verify the purchaseProtection after Migration " + obtainedPurchaseProtection + " is same as expected " + purchaseProtection);
            }

            res = CL.EA.PVR.VerifyEventInArchive("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("EVENT_BASED1", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Record from Archive");
            }
            // Stop Recording
            res = CL.EA.PVR.StopRecordingFromArchive("EVENT_BASED1");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop recording from Archive");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to return to Live Viewing");
            }
            res = CL.EA.PVR.VerifyEventInPlanner("TIME_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Time based recording in Planner");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("TIME_BASED1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Time based recording in Planner");
            }
            string serviceNumber = "";
            CL.IEX.MilestonesEPG.ClearEPGInfo();
            CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out serviceNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the service no from EPG");
            }

            if (isFavouriteMode.ToUpper() == "TRUE")
            {
                if (serviceNumber == "1")
                {
                    LogCommentImportant(CL, "Sucessfully Verified that we are in Favourite mode after SSU as the service "+serviceNumber+" is same as expected 1");
                }
                else
                {
                    FailStep(CL,"Failed to verify that we are in Favourite mode after SSU");
                }
            }
            else
            {
                if (serviceNumber == service1.LCN)
                {
                    LogCommentImportant(CL, "Sucessfully Verified that we are in normal mode after SSU as the "+serviceNumber+" is same as expected "+service1.LCN);
                }
                else
                {
                    FailStep(CL, "Failed to verify that we are in normal mode after SSU");
                }
            }
            
			 //Verifying that the service which is locked previously is still locked after the software Update
            res = CL.EA.TuneToLockedChannel(lockedService.LCN, CheckForVideo: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to lock service " + lockedService.LCN);
            }
            LogCommentImportant(CL, "We are able to tune to the service which we locked " + lockedService.LCN + " by entering pin which is expected");

			
            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.STBSettings.SetUnLockChannel(lockedService.Name);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to unlock channel");
        }
        LogComment(CL, "Broadcasting Manual NIT");
        //Fetch the default Manual OTA NIT
        CL.EA.UI.OTA.NITBraodcast(defaultNitTable);
        CL.IEX.Wait(20);
        //Setting the Device pin back to true
        //  res = CL.EA.STBSettings.ChangePinCode(Constants.newPIN, defaultPin);
        // if (!res.CommandSucceeded)
        // {
        //   try
        //   {
        //  LogCommentFailure(CL, "Failed to verify that the change pin codeis not reverted back to the default value");
        //  string boxID = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID").ToString();
        //  if (boxID == "")
        //  {
        //      LogCommentFailure(CL, "Failed to get the BOX_ID from Environment ini");
        //  }
        //  else
        //   {
        //      LogCommentImportant(CL, "Box ID fetched from Environment ini file is " + boxID);
        //  }
        //  string remotePSURL = CL.EA.UI.Utils.GetValueFromEnvironment("RemotePSServerURL").ToString();
        //  if (remotePSURL == "")
        //  {
        //      LogCommentFailure(CL, "Failed to get the RemotePSServerURL from Environment ini");
        //  }
        // else
        //  {
        //     LogCommentImportant(CL, "RemotePSServerURL fetched from Environment ini file is " + remotePSURL);
        // }
        // FirefoxDriver driver = new FirefoxDriver();
        //  driver.Navigate().GoToUrl(remotePSURL);
        //  LogCommentImportant(CL, "Navigating to the Remote PS server which is 10.201.96.19");
        //  driver.Manage().Window.Maximize();
        // driver.FindElement(By.Id("element_1")).Click();
        // driver.FindElement(By.Id("element_1")).Clear();
        //  driver.FindElement(By.Id("element_1")).SendKeys(boxID);
        // LogCommentImportant(CL, "Entering the BOX ID");
        // CL.IEX.Wait(2);
        // LogCommentImportant(CL, "Selecting the API in this case Reset MPIN");
        // SelectElement APIselector = new SelectElement(driver.FindElementById("element_2"));
        // APIselector.SelectByIndex(3);
        // CL.IEX.Wait(2);
        // LogCommentImportant(CL, "Selecting the Preferred Language which is en");
        // SelectElement Languageselector = new SelectElement(driver.FindElementById("language"));
        // Languageselector.SelectByValue("en");
        // CL.IEX.Wait(2);
        // LogCommentImportant(CL, "Selecting the LAB which is UM");
        // SelectElement Labselector = new SelectElement(driver.FindElementById("lab"));
        // Labselector.SelectByIndex(3);
        // CL.IEX.Wait(5);
        // driver.FindElementById("submit_form").Click();
        // CL.IEX.Wait(10);
        //  driver.Close();
        // }
        // catch (Exception ex)
        // {
        //     LogCommentFailure(CL, "Failed to reset the Pin from Remote PS server. Reason :" + ex.Message);
        // }

        // }
        // CL.IEX.Wait(10);
        // CL.EA.UI.Utils.SendIR("MENU");
        // CL.IEX.Wait(3);
        // CL.EA.UI.Utils.SendIR("RETOUR");
        // CL.IEX.Wait(2);
        // CL.EA.UI.Utils.SendIR("MENU");
        // CL.IEX.Wait(20);
        // res = CL.EA.STBSettings.FactoryReset(SaveRecordings: false, KeepCurrentSettings: false, PinCode: defaultPin);
        // if (!res.CommandSucceeded)
        //  {
        //      LogCommentInfo(CL, "Failed to do the Factory reset");
        //  }
        //  CL.IEX.Wait(60);
		res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all records from Achive");
        }
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to cancel all bookings from Planner");
        }
    }

    #endregion PostExecute
    public class Helper : _Step
    {

        static string ChannelID = "";
        static string ChannelName = "";

        public override void Execute() { }

        /// <summary>
        /// Tunes to Channel with subtitles and verifies the defualt subtitle language in Action bar.
        /// </summary>
        /// <returns>bool</returns>


        #region Reorder Favourites

        public bool ReorderFavourites()
        {



            string EpgText = "";

            try
            {
                // Navigate to Renumber Favorites
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:RENUMBER FAVOURITES");

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Select RENUMBER FAVOURITES");
                }



                //get the current channel is

                CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);

                // select id to be swapped

                int i = 0;

                while (ChannelID != Constants.currentid || i == 15)
                {
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_NEXT_SELECTION"));
                    CL.IEX.Wait(4);
                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    i++;
                }

                // moving to new positopn


                CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));

                while (ChannelID != Constants.reorderid || i == 15)
                {
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_PREV_SELECTION"));
                    CL.IEX.Wait(4);
                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    i++;
                }

                //moving to confirm

                //Press back button

                CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "CONFIRM_FAVOURITE"));
                CL.IEX.Wait(2);
                string title = "";
                CL.EA.UI.Utils.GetEpgInfo("title", ref title);
                if (title.ToUpper() == "DEFAULT ORDER")
                {
                    CL.EA.UI.Utils.SendIR("RETOUR");
                }
                else
                {
                    //send the "SELECT" key to confirm reorder Favourist List
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));
                }

                CL.IEX.Wait(20);
                CL.EA.UI.Utils.GetEpgInfo("title", ref EpgText);

                if (EpgText.Trim() == "RENUMBER FAVOURITES")
                {
                    // Confirm reorder by checking id is 1 and chname is service 2 channel name
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));

                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    CL.EA.UI.Utils.GetEpgInfo("title", ref ChannelName);

                    if (ChannelID.Trim() == Constants.reorderid && ChannelName.Trim() == service1.Name)
                    {
                        CL.IEX.LogComment("Reorder Favourites Passed");
                    }
                    else
                    {
                        FailStep(CL, "Fail to CONFIRM Reorder Favourite");
                    }

                }
                else
                {
                    FailStep(CL, "Fail to CONFIRM Reorder Favourite");
                }

                return true;

            }
            catch (Exception ex)
            {

                FailStep(CL, "Failed to  Reorder Favourites." + ex.Message);
                return false;
            }
        }

        #endregion

        #region Verify Favourites Order

        public bool VerifyFavouritesOrder()
        {


            try
            {
                // Navigate to Renumber Favorites
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:RENUMBER FAVOURITES");

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Select RENUMBER FAVOURITES");
                }



                //get the current channel is
                CL.EA.UI.Utils.GetEpgInfo("title", ref ChannelName);
                CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                if (ChannelName == service1.Name && ChannelID == "1")
                {
                    LogCommentImportant(CL, "Verified that the Favourite Services 1 is same as expected after the Download");
                }
                else
                {
                    FailStep(CL, "Failed to verify that the Favourite Services 1 is same as expected after the Download");
                }

                CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_NEXT_SELECTION"));
                CL.IEX.Wait(4);
                CL.EA.UI.Utils.GetEpgInfo("title", ref ChannelName);
                CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                if (ChannelName == service.Name && ChannelID == "2")
                {
                    LogCommentImportant(CL, "Verified that the Favourite Services 2 is same as expected after the Download");
                }
                else
                {
                    FailStep(CL, "Failed to verify that the Favourite Services 2 is same as expected after the Download");
                }
                CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_NEXT_SELECTION"));
                CL.IEX.Wait(4);
                CL.EA.UI.Utils.GetEpgInfo("title", ref ChannelName);
                CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                if (ChannelName == service2.Name && ChannelID == "3")
                {
                    LogCommentImportant(CL, "Verified that the Favourite Services 3 is same as expected after the Download");
                }
                else
                {
                    FailStep(CL, "Failed to verify that the Favourite Services 3 is same as expected after the Download");
                }


                return true;

            }
            catch (Exception ex)
            {

                FailStep(CL, "Failed to  Reorder Favourites." + ex.Message);
                return false;
            }
        }

        #endregion
    }
}