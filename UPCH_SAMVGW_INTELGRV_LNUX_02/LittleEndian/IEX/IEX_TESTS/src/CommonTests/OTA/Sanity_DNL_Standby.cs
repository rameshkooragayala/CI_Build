/// <summary>
///  Script Name : Sanity_DNL_Standby.cs
///  Test Name   : Sanity-DNL-Standby+n on n
///  TEST ID     : 
///  QC Version  : 2
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : MadhuKumar K
/// </summary>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using IEX.Tests.Utils;

[Test(" Sanity_DNL_StandBy")]
public class Sanity_DNL_StandBy : _Test
{

    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps

    static string oldVersion = "";
    static string usageID = "";
    static string otaDownloadOption = "";
    static string rfFeed = "";
    static string isLastDelivery = "";
    static string nitTable = "";
    static string powerMode = "";
    static string defaultNitTable = "";
    static string defaultMaintenanceDelay = "";
    //private static string defaultPin;
    private static string project;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Navigate to Settings Menu>>Diagnostics and verify for the older sofware version and ensure the box is in standby.";
    private const string STEP1_DESCRIPTION = "Step 1: Broadcast the Binary On Air";
    private const string STEP2_DESCRIPTION = "Step 2: Verify the box is in stand by after download and also verify new sofware version after download";

    #region Create Structure


    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();


    }
    #endregion

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition


    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File

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

            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }
            else
            {
                LogCommentInfo(CL, "Powermode fetched from Test ini " + powerMode);
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
            defaultMaintenanceDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_DEALY");
            if (string.IsNullOrEmpty(defaultMaintenanceDelay))
            {
                FailStep(CL, res, "Unable to fetch the default maintenance delay value from project ini");
            }

            //defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "TEST PARAMS", "DefaultPIN");
            //if (string.IsNullOrEmpty(defaultPin))
            //{
            //    FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            //}
            project = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "Project");
            if (string.IsNullOrEmpty(project))
            {
                FailStep(CL, "Failed to fetch Project Name from Environment.ini");
            }
            else
            {
                LogCommentInfo(CL, "Project fetched from Envirnoment ini " + project);
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
            // powerMode = "HOT STANDBY";
            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }

            //put the box to standby
            res = CL.EA.StandBy(false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to enter stand by");
            }
            else
            {
                LogCommentInfo(CL, "Box is in standby");
            }
            PassStep();
        }
    }
    #endregion

    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Download binary On Air
            res = CL.EA.OtaDownload(oldVersion, usageID, nitTable, Convert.ToBoolean(isLastDelivery), rfFeed);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to download binary on air");
            }
            else
            {
                LogCommentInfo(CL, "Binary downloaded successfully on the box");
            }

            int defaultMaintenanceDelayInt = Convert.ToInt32(defaultMaintenanceDelay);
            defaultMaintenanceDelayInt = defaultMaintenanceDelayInt * 60;
            //Download time
            string downloadtime = CL.EA.GetValueFromINI(EnumINIFile.Project, "OTA", "DOWNLOADTIME");
            res = CL.IEX.Wait(seconds: Convert.ToDouble(downloadtime) + 120 + defaultMaintenanceDelayInt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until the download is complete");
            }

            //Mount the box
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to mount the Gateway");
            }

            PassStep();
        }
    }
    #endregion

    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Its not possible to verify the Stand by state as Live and Stand by will have the same state and Screen controller so this is a work around to verify the Stand by 
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to send main menu so assuming we are in Stand by");
            }
            else
            {
                FailStep(CL, "Able to launch Main menu so assuming we are not in stand by");
            }
            //Come out of Stand by

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                LogComment(CL, "Failed to exit Stand by so Selecting the Accept Screen");
                CL.IEX.Wait(60);
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
            }
  

            CL.IEX.Wait(15);
            //check for live state
            bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (!liveState)
            {
                FailStep(CL, res, "Unable to verify the live state after on air OTA download");

            }
            //Adding to wait of 5 seconds 
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            res = CL.EA.GetAndVerifySoftVersion(ref oldVersion, ref usageID, IsVerify: true, OldSoftVersion: oldVersion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the Software version from EPG");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
    #region PostExecute

    public override void PostExecute()
    {
        LogComment(CL, "Broadcasting Manual NIT");
        //Fetch the default Manual OTA NIT
        CL.EA.UI.OTA.NITBraodcast(defaultNitTable);
        CL.IEX.Wait(20);
    }

    #endregion PostExecute
}
