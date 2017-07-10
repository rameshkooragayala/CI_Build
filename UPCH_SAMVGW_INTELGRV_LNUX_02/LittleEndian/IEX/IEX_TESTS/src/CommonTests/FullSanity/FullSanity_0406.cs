using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;


public class FullSanity_0406 : _Test
{
    [ThreadStatic]

    static _Platform CL, GW;

    //Channels used by the tes
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    private static Service Service_5;
    static bool ishomenet = false;
    static string selectedSGTValue = "";
    static string selectedInfoMenuTimeOut = "";
    static string selectedDiskSpaceManagement = "";
    static string selectedSeriesRecording = "";
    private static string defaultPin;
    private static string factoryResetoptions = "";
    private static bool keepCurrentSetting = false;
    private static bool keepRecording = false;
	static FirefoxDriver driver;
    //Shared members between steps
    private static class Constants
    {
        public const string newPIN = "1111";
    }
    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Preconditions: Get Services from Content xml");
        this.AddStep(new Step1(), "Step 1: Modify some settings like Pin Code, Favourite Channels, Age rating etc");
        this.AddStep(new Step2(), "Step 2: Book a future Time based recording");
        this.AddStep(new Step3(), "Step 3: Schedule Some Event-Based Recordings");
        this.AddStep(new Step4(), "Step 4: Do Factory Reset and bring up the box");
        this.AddStep(new Step5(), "Step 5: Verify All the settings");


        //Get Client Platform
        CL = GetClient();
        string isHomeNetwork = CL.EA.GetTestParams("IsHomeNetwork");

        ////If Home network is true perform GetGateway
        ishomenet = Convert.ToBoolean(isHomeNetwork);
        if (ishomenet)
        {
            //Get gateway platform
            GW = GetGateway();
        }
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
           

            //Get different services from Content xml
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_2.LCN);
            }
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_3.LCN);
            }
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_4.LCN);
            }
            string lockableService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LOCKABLE_SERVICE");
            if (lockableService == "")
            {
                FailStep(CL, "Failed to get the LOCKABLE_SERVICE from test ini");
            }
            Service_5 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN="+lockableService, "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN + "," + Service_4.LCN);
            if (Service_5 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_5.LCN);
            }

            factoryResetoptions = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FACTORYRESET_OPTIONS");
            if (factoryResetoptions == "")
            {
                FailStep(CL, "Failed to fetch the FACTORYRESET_OPTIONS from test ini");
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
            //Changing the Default pin to new pin
            res = CL.EA.STBSettings.ChangePinCode(defaultPin, Constants.newPIN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to change pin code");
            }
            //Changing the Channel bar time out by selecting the next option and storing the title
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:CHANNEL BAR TIME OUT");
            }
            CL.IEX.Wait(2);
            string timeStamp = "";
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select Down");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedInfoMenuTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select");
            }
            CL.IEX.Wait(5);
            //Changing the SGT value by selecting the next one and storing the title
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:EXTRA TIME BEFORE PROGRAMME");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select Down");
            }

            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedSGTValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select");
            }
            CL.IEX.Wait(2);
            //Setting the DiskSpace Management to the Next value
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:DISK SPACE MANAGEMENT");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select Down");
            }

            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedDiskSpaceManagement);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select");
            }
            CL.IEX.Wait(2);

            //Setting the Series Recording to the Next Value
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SERIES RECORDINGT");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select Down");
            }

            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedSeriesRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to send the IR command Select");
            }
            CL.IEX.Wait(2);

            //Unsetting all the Favourites and setting few services as favourites
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unset all the services from the Favourite");
            }

            res = CL.EA.STBSettings.SetFavoriteChannelNumList(Service_3.LCN + "," + Service_4.LCN, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set services as Favourites");
            }
            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }
            //Setting the Parental control age limit to FSK 12
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.FSK_12);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Parental control Age limit to 12");
            }
            //LOcking the Service 5
            res = CL.EA.STBSettings.SetLockChannel(Service_5.Name);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to lock the service");
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

            //Booking a time based recording for 3 hours from now
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED",Convert.ToInt32(Service_3.LCN), DaysDelay: 0, MinutesDelayUntilBegining: 180, DurationInMin: 10,VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from Planner");
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
            //Recording two event based and stopping them after few seconds
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to servcie " + Service_1.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED", MinTimeBeforeEvEnd: 2, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from Banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to servcie " + Service_2.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED_2", MinTimeBeforeEvEnd: 2, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from Banner");
            }
            CL.IEX.Wait(60);

            res = CL.EA.PVR.StopRecordingFromArchive("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Stop the recording from Archive");
            }
            CL.IEX.Wait(2);
            res = CL.EA.PVR.StopRecordingFromArchive("EVENT_BASED_2");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Stop the recording from Archive");
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

            string quickOtionFactoryResetId = "";
            switch (factoryResetoptions)
            {
                case "KEEPRECORDINGS_KEEPSETTINGS":
                    keepRecording = true;
                    keepCurrentSetting = true;
                    quickOtionFactoryResetId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QucikActin_KeepRecordings_KeepSettings_Id");
                    break;
                case "KEEPRECORDINGS_UNKEEPSETTINGS":
                    keepRecording = true;
                    keepCurrentSetting = false;
                    quickOtionFactoryResetId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QucikActin_KeepRecordings_UnkeepSettings_Id");
                    break;
                case "UNKEEPRECORDINGS_KEEPSETTINGS":
                    keepRecording = false;
                    keepCurrentSetting = true;
                    quickOtionFactoryResetId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QucikActin_UnkeepRecording_KeepSettings_Id");
                    break;
                case "UNKEEPRECORDINGS_UNKEEPSETTINGS":
                    keepRecording = false;
                    keepCurrentSetting = false;
                    quickOtionFactoryResetId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QucikActin_UnkeepRecording_UnkeepSettings_Id");
                    break;
                default:
                    FailStep(CL,"Obtained option "+factoryResetoptions+" is not related to any of the cases");
                    break;
            }
            //Depending on this Factory Reset Option we will be setting the Factory Reset Through Main Menu or Through RMS
            string factoryResetRMS = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FACTORYRESET_RMS");
            if (factoryResetRMS.ToUpper() == "TRUE")
            {
                if (quickOtionFactoryResetId == "")
                {
                    FailStep(CL,"Failed to get the Quick Action Factory Reset ID's from browser ini");
                }
                //cpeid from environment ini   
               string cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }

                // Quick Action ConfirmId from Browser ini
               string quickActionConfirmId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QuickAction_Confirm_Id");
                if (quickActionConfirmId == null)
                    FailStep(CL, "Failed to Fetch quickActionConfirmation Id from ini file");
                else
                    LogComment(CL, "quickActionConfirmation Id fetched is" + quickActionConfirmId);
                driver = new FirefoxDriver();
                CL.IEX.Wait(20);
                //Login To panorama Page and enter Boxid and perform Factory Reset
                res = CL.EA.RMSLoginAndQuickActions(driver, cpeId, quickOtionFactoryResetId, quickActionConfirmId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login and perform "+quickOtionFactoryResetId);
                }

                ArrayList list = new ArrayList();
                //getting fas shutdown milestones
                string ShutDownMilestone = CL.EA.UI.Utils.GetValueFromMilestones("ShutDown");
                if (ShutDownMilestone == null)
                {
                    FailStep(CL, "Failed to get ShutDown Milestone");
                }
                CL.EA.UI.Utils.BeginWaitForDebugMessages(ShutDownMilestone, 120);

                bool isShutDownMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(ShutDownMilestone, ref list);

                //Checking Whether fas milestones are coming or not
                if (!isShutDownMilestoneRecieved)
                {
                    FailStep(CL, "Failed to get FAS ShutDown Milestone");
                }
                 driver.Quit();
                CL.IEX.Wait(100);
            }
            else
            {
                //Doing a factory reset with Keep Current settings as true and save recordings as true
                res = CL.EA.STBSettings.FactoryReset(SaveRecordings: keepRecording, KeepCurrentSettings: keepCurrentSetting, PinCode: Constants.newPIN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to do the Factory reset");
                }
            }
            //Bringing up the box after the Factory reset handled for both sigle STB and a homne network
            if (ishomenet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount client");
                }
            }
            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount cleint");
                }
            }
            CL.IEX.Wait(30);
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

                //Entering the new pin and changing it back to the default pin
            res = CL.EA.STBSettings.ChangePinCode(Constants.newPIN, defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify that the change pin codeis not reverted back to the default value");
            }
            LogCommentImportant(CL, "We are able to enter new pin and change it to default pin which suggests that the settings were retained after Factory Reset");
           

            //Verifyng that the Channel bar time out is the one which is selected previously after Factory reset
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:CHANNEL BAR TIME OUT");
            }
            CL.IEX.Wait(2);
            string obtainedInfoMenuTimeOut = "";
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedInfoMenuTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            //Depending on the keepsettings flag we are verifying
            if (keepCurrentSetting)
            {
                if (obtainedInfoMenuTimeOut != selectedInfoMenuTimeOut)
                {
                    FailStep(CL, "Failed to verify that the selected Selected Info Time out " + selectedInfoMenuTimeOut + " is same as expected " + obtainedInfoMenuTimeOut);
                }
                LogCommentImportant(CL, "Verified that the selected Selected Info Time out " + selectedInfoMenuTimeOut + " is same as expected " + obtainedInfoMenuTimeOut);

            }
            else
            {
                string DefaultInfoTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
                if (DefaultInfoTimeOut == "")
                {
                    FailStep(CL, "Failed to fetch the Default value for CHANNEL_BAR_TIMEOUT from Project ini");
                }
                if (!obtainedInfoMenuTimeOut.StartsWith(DefaultInfoTimeOut))
                {
                    FailStep(CL, "Failed to verify that the obtained Info Time out " + obtainedInfoMenuTimeOut + " is same as expected " + DefaultInfoTimeOut);
                }
                LogCommentImportant(CL, "Verified that the obtained Info Time out " + obtainedInfoMenuTimeOut + " is same as expected " + DefaultInfoTimeOut);

            }
               string obtainedSGTValue = "";
            CL.IEX.Wait(5);
            //Verifying that the SGT is the one which is selected previously after Factory reset
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:EXTRA TIME BEFORE PROGRAMME");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSGTValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            //Depending on the keepsettings flag we are verifying the SGT value
            if (keepCurrentSetting)
            {
                if (obtainedSGTValue != selectedSGTValue)
                {
                    FailStep(CL, "Failed to verify that the selected SGT value " + selectedSGTValue + " is same as expected " + obtainedSGTValue);
                }
                LogCommentImportant(CL, "Verified that the selected SGT value " + selectedSGTValue + " is same as expected " + obtainedSGTValue);
            }
            else
            {
                string DefaultSGTValue = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
                if (DefaultSGTValue == "")
                {
                    FailStep(CL, "Failed to fetch the Default value for SGT from Project ini");
                }
                if (!obtainedSGTValue.StartsWith(DefaultSGTValue))
                {
                    FailStep(CL, "Failed to verify that the obtained SGT value " + obtainedSGTValue + " is same as expected " + DefaultSGTValue);
                }
                LogCommentImportant(CL, "Verified that the obtained SGT value " + obtainedSGTValue + " is same as expected " + DefaultSGTValue);
            }
            //Depending on the keepsettings flag we are verifying the Disk Space Management
            string obtainedDiskSpaceManagement = "";
            CL.IEX.Wait(5);
            //Verifying that the DISK SPACE MANAGEMENT is the one which is selected previously after Factory reset
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:DISK SPACE MANAGEMENT");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDiskSpaceManagement);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            if (keepCurrentSetting)
            {
                if (obtainedDiskSpaceManagement != selectedDiskSpaceManagement)
                {
                    FailStep(CL, "Failed to verify that the selected Disk Space Management value " + selectedDiskSpaceManagement + " is same as expected " + obtainedDiskSpaceManagement);
                }
                LogCommentImportant(CL, "Verified that the selected Disk Space Management value " + selectedDiskSpaceManagement + " is same as expected " + obtainedDiskSpaceManagement);
            }
            else
            {
                string DefaultDiskSpaceManagement = CL.EA.GetValueFromINI(EnumINIFile.Project, "DISK_SPACE_MANAGEMENT", "DEFAULT");
                if (DefaultDiskSpaceManagement == "")
                {
                    FailStep(CL, "Failed to fetch the Default value for DISK_SPACE_MANAGEMENT from Project ini");
                }
                if (obtainedDiskSpaceManagement != DefaultDiskSpaceManagement)
                {
                    FailStep(CL, "Failed to verify that the obtained Disk Space Management " + obtainedDiskSpaceManagement + " is same as expected " + DefaultDiskSpaceManagement);
                }
                LogCommentImportant(CL, "Verified that the obtained Disk Space Management value " + obtainedDiskSpaceManagement + " is same as expected " + DefaultDiskSpaceManagement);
            }
            //Verifying that the SERSERIES RECORDING the one which is selected previously after Factory reset
            string obtainedSeriesRecording = "";
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SERIES RECORDING");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSeriesRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the title from EPG");
            }
            CL.IEX.Wait(1);
            if (keepCurrentSetting)
            {
                if (obtainedSeriesRecording != selectedSeriesRecording)
                {
                    FailStep(CL, "Failed to verify that the selected Series Recording " + selectedSeriesRecording + " is same as expected " + obtainedSeriesRecording);
                }
                LogCommentImportant(CL, "Verified that the selected Series Recording " + selectedSeriesRecording + " is same as expected " + obtainedSeriesRecording);
            }
            else
            {
                string DefaultSeriesRecordingValue = CL.EA.GetValueFromINI(EnumINIFile.Project, "SERIES_RECORDING", "DEFAULT");
                if (DefaultSeriesRecordingValue == "")
                {
                    FailStep(CL, "Failed to fetch the Default value for Series Recording from Project ini");
                }
                if (obtainedSeriesRecording != DefaultSeriesRecordingValue)
                {
                    FailStep(CL, "Failed to verify that the obtained Series Recording value " + obtainedSeriesRecording + " is same as expected " + DefaultSeriesRecordingValue);
                }
                LogCommentImportant(CL, "Verified that the obtained Series Recording value " + obtainedSeriesRecording + " is same as expected " + DefaultSeriesRecordingValue);
            }
            String Milestones = "";
            if (keepCurrentSetting)
            {
                Milestones = "FSK 12";
            }
            else
            {
                Milestones = CL.EA.GetValueFromINI(EnumINIFile.Project, "PARENTAL_CONTROL_AGE_LIMIT", "DEFAULT");
            }

            //Verifying the Parental control age limit retaind after the Factory Reset
            CL.EA.UI.Settings.NavigateToParentalControlAgeLimit();
            //WORKAROUD: As there is an issue with the title milestone (Issue: We are getting title milestone 1 after the proper milestone which is masking the correct milestone) so using the begin wait and end wait

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
                FailStep(CL, "Failed to verify title milestone for "+Milestones);
            }
            LogCommentImportant(CL, "Verifed that the Parental control age limit is "+Milestones+" after Facotory Reset which is expected");
			CL.IEX.Wait(10);
            CL.EA.UI.Utils.SendIR("MENU");
            CL.EA.UI.Utils.SendIR("RETOUR");
            CL.IEX.Wait(20);
            if (keepCurrentSetting)
            {
                //Verifying that the service which is locked previously is still locked after the Factory Reset
                res = CL.EA.TuneToLockedChannel(Service_5.LCN, CheckForVideo: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to lock service " + Service_5.LCN);
                }
                LogCommentImportant(CL, "We are able to tune to the service which we locked " + Service_5.LCN + " by entering pin which is expected");
            }
            else
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_5.LCN);
                if(!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to tune to service "+Service_5.LCN);
                }
                LogCommentImportant(CL, "We are able to tune to the service which we locked " + Service_5.LCN + " without entering pin which is expected");
            }

            //Verrifying that the Favourite services are retained after the Factory reset
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_3.LCN);
            }
            CL.IEX.MilestonesEPG.ClearEPGInfo();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to Channel bar");
            }
            string isFavourite="";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("isfavourite", out isFavourite);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the EPG info for is Favourite");
            }

            //Verifying that the service 4 is a Favourite Service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_4.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_4.LCN);
            }
            CL.IEX.MilestonesEPG.ClearEPGInfo();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Channel bar");
            }
            string isFavourite1 = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("isfavourite", out isFavourite1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info for is Favourite");
            }

            if (keepCurrentSetting)
            {
                if (isFavourite.ToUpper() != "TRUE")
                {
                    FailStep(CL, "Failed to verify that the service " + Service_3.LCN + " is a favourite service");
                }
                LogCommentImportant(CL, "Verified that the service " + Service_3.LCN + " is a Favourite service which is expected");
                if (isFavourite1.ToUpper() != "TRUE")
                {
                    FailStep(CL, "Failed to verify that the service " + Service_4.LCN + " is a favourite service");
                }
                LogCommentImportant(CL, "Verified that the service " + Service_4.LCN + " is a Favourite service which is expected");
            }
            else
            {
                if (isFavourite.ToUpper() != "FALSE")
                {
                    FailStep(CL, "Failed to verify that the service " + Service_3.LCN + " is not a favourite service");
                }
                LogCommentImportant(CL, "Verified that the service " + Service_3.LCN + " is not a Favourite service which is expected");
                if (isFavourite1.ToUpper() != "FALSE")
                {
                    FailStep(CL, "Failed to verify that the service " + Service_4.LCN + " is not a favourite service");
                }
                LogCommentImportant(CL, "Verified that the service " + Service_4.LCN + " is not a Favourite service which is expected");
            }
            //Verifying that the Recordings and planner are retained after the factory reset
            res = CL.EA.PVR.VerifyEventInArchive("EVENT_BASED",SupposedToFindEvent:keepRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.EA.PVR.VerifyEventInArchive("EVENT_BASED_2", SupposedToFindEvent: keepRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("TIME_BASED", SupposedToFindEvent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Planner");
            }
            LogCommentImportant(CL, "Verified that the recordings are not deleted and planner is Deleted after the Factory Reset");
            PassStep();
        }

    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //Setting the Device pin back to true
        res = CL.EA.STBSettings.ChangePinCode(Constants.newPIN, defaultPin);
        if (!res.CommandSucceeded)
        {
            try
            {
                LogCommentFailure(CL, "Failed to verify that the change pin codeis not reverted back to the default value");
                string boxID = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID").ToString();
                if (boxID == "")
                {
                    LogCommentFailure(CL, "Failed to get the BOX_ID from Environment ini");
                }
                else
                {
                    LogCommentImportant(CL, "Box ID fetched from Environment ini file is " + boxID);
                }
                string remotePSURL = CL.EA.UI.Utils.GetValueFromEnvironment("RemotePSServerURL").ToString();
                if (remotePSURL == "")
                {
                    LogCommentFailure(CL, "Failed to get the RemotePSServerURL from Environment ini");
                }
                else
                {
                    LogCommentImportant(CL, "RemotePSServerURL fetched from Environment ini file is " + remotePSURL);
                }
                FirefoxDriver driver = new FirefoxDriver();
                driver.Navigate().GoToUrl(remotePSURL);
                LogCommentImportant(CL, "Navigating to the Remote PS server which is 10.201.96.19");
                driver.Manage().Window.Maximize();
                driver.FindElement(By.Id("element_1")).Click();
                driver.FindElement(By.Id("element_1")).Clear();
                driver.FindElement(By.Id("element_1")).SendKeys(boxID);
                LogCommentImportant(CL, "Entering the BOX ID");
                CL.IEX.Wait(2);
                LogCommentImportant(CL, "Selecting the API in this case Reset MPIN");
                SelectElement APIselector = new SelectElement(driver.FindElementById("element_2"));
                APIselector.SelectByIndex(3);
                CL.IEX.Wait(2);
                LogCommentImportant(CL, "Selecting the Preferred Language which is en");
                SelectElement Languageselector = new SelectElement(driver.FindElementById("language"));
                Languageselector.SelectByValue("en");
                CL.IEX.Wait(2);
                LogCommentImportant(CL, "Selecting the LAB which is UM");
                SelectElement Labselector = new SelectElement(driver.FindElementById("lab"));
                Labselector.SelectByIndex(3);
                CL.IEX.Wait(5);
                driver.FindElementById("submit_form").Click();
                CL.IEX.Wait(10);
                driver.Quit();
            }
            catch (Exception ex)
            {
                LogCommentFailure(CL, "Failed to reset the Pin from Remote PS server. Reason :" + ex.Message);
				CL.IEX.Wait(10);
                driver.Quit();
            }
        }
		    CL.IEX.Wait(10);
            CL.EA.UI.Utils.SendIR("MENU");
			CL.IEX.Wait(3);
            CL.EA.UI.Utils.SendIR("RETOUR");
			CL.IEX.Wait(2);
			CL.EA.UI.Utils.SendIR("MENU");
            CL.IEX.Wait(20);
        //Setting the Channel bar display timout to default
        //string defaultChannelBarTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
        //if (string.IsNullOrEmpty(defaultChannelBarTimeOut))
        //{
        //    LogCommentFailure(CL, "CHANNEL_BAR_TIMEOUT, DEFAULT fetched from Project.ini is null or empty");
        //}
        //EnumChannelBarTimeout defaultChannelBarTimeOutVal;
        //Enum.TryParse<EnumChannelBarTimeout>(defaultChannelBarTimeOut, out defaultChannelBarTimeOutVal);
        //res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to set the Banner Display timeout to 5");
        //}
        ////Setting the Parental control age limit to default
        //string defaultPCAgeLimit = CL.EA.GetValueFromINI(EnumINIFile.Project, "PARENTAL_CONTROL_AGE_LIMIT", "DEFAULT");
        //EnumParentalControlAge EnumParentalControlAgeValue;
        //Enum.TryParse<EnumParentalControlAge>(defaultPCAgeLimit, out EnumParentalControlAgeValue);
        //res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAgeValue);
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to set the Parental control Age limit to Default");
        //}
        ////Setting the start Guard time to default
        //string DefaultSGTValue = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        //res = CL.EA.STBSettings.SetGuardTime(true, DefaultSGTValue);
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL,"Failed to set the Guard time to true");
        //}
        ////Setting the Disk Space ManageMent to the Default value
        //res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to navigate to STATE:DISK SPACE MANAGEMENT");
        //}
        //CL.IEX.Wait(2);
        //string DefaultDiskSpaceManagement = CL.EA.GetValueFromINI(EnumINIFile.Project, "DISK_SPACE_MANAGEMENT", "DEFAULT");
        //res= CL.IEX.MilestonesEPG.Navigate(DefaultDiskSpaceManagement);
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL,"Failed to set the Disk space management to Default value");
        //}
        ////Setting the Series recording to the Default value
        //res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to navigate to STATE:SERIES RECORDING");
        //}
        //CL.IEX.Wait(2);
        //string DefaultSeriesRecordingValue = CL.EA.GetValueFromINI(EnumINIFile.Project, "SERIES_RECORDING", "DEFAULT");
        //res = CL.IEX.MilestonesEPG.Navigate(DefaultSeriesRecordingValue);
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to set the SeriesRecordingValue to Default value");
        //}
        ////Unlocking the locked service
        //res = CL.EA.STBSettings.SetUnLockChannel(Service_5.Name);
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to lock the service");
        //}
        ////Unsetting all the Favourites and setting few services as favourites
        //res = CL.EA.STBSettings.UnsetAllFavChannels();
        //if (!res.CommandSucceeded)
        //{
        //    LogCommentFailure(CL, "Failed to unset all the services from the Favourite");
        //}
        //Deleting all the recordings
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete all records from Archive");
        }
        //Cancelling all bookings from planner
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to cancel all bookings from planner");
        }
        res = CL.EA.STBSettings.FactoryReset(SaveRecordings: false, KeepCurrentSettings: false, PinCode: defaultPin);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to do the Factory reset");
        }
		CL.IEX.Wait(60);
    }
    #endregion
}