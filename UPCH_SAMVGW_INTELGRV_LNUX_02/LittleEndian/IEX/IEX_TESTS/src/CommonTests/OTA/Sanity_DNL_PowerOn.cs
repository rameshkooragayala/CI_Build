/// <summary>
///  Script Name : Sanity_DNL_PowerOn.cs
///  Test Name   : Sanity-DNL-PowerOn+n on n
///  TEST ID     : 74292
///  QC Version  : 2
///  Variations from QC:none
///  QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Modified by : Madhu K
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using OpenQA.Selenium.Firefox;

[Test("Sanity_DNL_Live")]
public class Sanity_DNL_Live : _Test
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
    static string defaultNitTable = "";
	static FirefoxDriver driver;
   // private static string defaultPin;
    private static string project;


    private const string PRECONDITION_DESCRIPTION = "Precondition: Navigate to Settings Menu>>Diagnostics and verify for the older sofware version and ensure the box is in live.";
    private const string STEP1_DESCRIPTION = "Step 1: Broadcast the Binary On Air";
    private const string STEP2_DESCRIPTION = "Step 2: Verify the box is on Live after download and the new sofware version";

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
            //defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            //if (string.IsNullOrEmpty(defaultPin))
            //{
            //    FailStep(CL, "Failed to fetch DefaultPIN from Envirnoment.ini");
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

            res = CL.IEX.SendIRCommand("MENU", 1,ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to send IR Main Menu");
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
            res = CL.EA.OtaDownload(oldVersion, usageID, nitTable,Convert.ToBoolean(isLastDelivery),rfFeed,IsLive:true);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to download binary on air");
            }
            else
            {
                LogCommentInfo(CL, "Binary downloaded successfully on the box");
            }

            if (project == "IPC")
            {
                res = CL.IEX.Wait(120);
                if (!(res.CommandSucceeded))
                {
                    FailStep(CL, res, "Failed during wait");
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
			CL.IEX.Wait(120);
            //Mount the Gateway
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to mount the Gateway");
            }

            LogCommentInfo(CL,"Waiting fews seconds for the box to come to live");

            CL.IEX.Wait(100);
            string acceptScreen = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title",out acceptScreen);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL,"Failed to get the title from EPG");
            }
            if(acceptScreen == "ACCEPT")
            {
                LogCommentInfo(CL,"Selecting Accept Screen");
                CL.EA.UI.Utils.SendIR("SELECT");
            }
            CL.IEX.Wait(15);

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

            //check for live state
            bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (!liveState)
            {
                FailStep(CL, res, "Unable to verify the live state after on air OTA download");

            }
           //Verifying the Software version with the old version and comparing
            res = CL.EA.GetAndVerifySoftVersion(ref oldVersion, ref usageID, IsVerify: true, OldSoftVersion: oldVersion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the Software version from EPG");
            }
			            //Adding the code to verify the Download Status and time in the RMS after the Download success
            string verifyOTAValuesInRMS="";
            verifyOTAValuesInRMS=CL.EA.GetValueFromINI(EnumINIFile.Test,"TEST PARAMS","VERIFY_RMS");
            if (verifyOTAValuesInRMS.ToUpper() == "TRUE")
            {
                //Fetching the Box ID from Environment ini
                string cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }
                //Fetching the required ID's from Browser ini
                string deviceLastSWDownloadTimeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LAST_SOFTWAREDOWNLOAD_DATE");
                if (deviceLastSWDownloadTimeId == null)
                {
                    FailStep(CL, "Failed to fetch LAST_SOFTWAREDOWNLOAD_DATE from ini File.");
                }
                else
                {
                    LogComment(CL, "deviceLastSWDownloadTimeId fetched from Browser ini is : " + deviceLastSWDownloadTimeId);
                }
                string LastSoftwareDownloadStatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LAST_SOFTWAREDOWNLOAD_STATUS");
                if (LastSoftwareDownloadStatusId == null)
                {
                    FailStep(CL, "Failed to fetch LAST_SOFTWAREDOWNLOAD_STATUS from ini File.");
                }
                else
                {
                    LogComment(CL, "LastSoftwareDownloadStatusId fetched from Browser ini is : " + LastSoftwareDownloadStatusId);
                }
                //Fetching the Last update in DIagnostics from CI
                string lastUpdateDate = "";
                res = CL.IEX.MilestonesEPG.GetEPGInfo("last update", out lastUpdateDate);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to get the info from EPG");
                }
                //Getting the Software Download date from the Last Update by splitting
                string expectedRMSDateFromDiagnostics = "";
                string[] lastUpdateDateArr = lastUpdateDate.Split('/');
                expectedRMSDateFromDiagnostics = lastUpdateDateArr[0].Trim();
                LogCommentImportant(CL, "Expected RMS date fetced from Diagnostics is " + expectedRMSDateFromDiagnostics);
                string BrowserTabControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (BrowserTabControlId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + BrowserTabControlId);

                }
                driver = new FirefoxDriver();
                //Login to the RMS and enter the box id
                res = CL.EA.RMSLoginAndEnterBoxid(driver: driver, CPEId: cpeId, TabId: BrowserTabControlId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Login to the RMS and enter Box ID");
                }
                //FETCHING THE Software Download date & time VALUE FROM THE PANORAMA WEBPAGE
                string RMSDateTime = CL.EA.UI.RMS.GetParameterValues(driver, deviceLastSWDownloadTimeId);
                if (RMSDateTime == "")
                {
                    FailStep(CL, "Device summary value from Panorama is null");

                }

                LogCommentImportant(CL, "Expected SerialNumber from Panorama is " + RMSDateTime);
                //FETCHING  Software Download status from RMS VALUE FROM THE PANORAMA WEBPAGE
                string RMSStatus = CL.EA.UI.RMS.GetParameterValues(driver, LastSoftwareDownloadStatusId);
                if (RMSStatus == "")
                {
                    FailStep(CL, "Device summary value from Panorama is null");
                }

                LogCommentImportant(CL, "Expected RMS Date vale Fetched from Panorama is " + RMSDateTime);
                // string RMSDateTime = "12/4/14 03:34:14 AM";
                string RMSDate = "";
                string[] RMSDateTimearray;
                string parsedRMSDate;
                LogCommentInfo(CL, "Download date time fetched from RMS is " + RMSDateTime);
                RMSDateTimearray = RMSDateTime.Split(' ');
                RMSDate = RMSDateTimearray[0];
                RMSDateTimearray = RMSDate.Split('/');
                parsedRMSDate = (RMSDateTimearray[1].Length == 1 ? "0" + RMSDateTimearray[1] : RMSDateTimearray[1]) + "." + (RMSDateTimearray[0].Length == 1 ? "0" + RMSDateTimearray[0] : RMSDateTimearray[0]) + "." + RMSDateTimearray[2];
                DateTime mydate = new DateTime(Convert.ToInt32("20" + RMSDateTimearray[2]), Convert.ToInt32(RMSDateTimearray[0]), Convert.ToInt32(RMSDateTimearray[1]));
                string day = mydate.DayOfWeek.ToString().ToUpper().Substring(0, 2);
                RMSDate = day + " " + parsedRMSDate;
                LogCommentImportant(CL, "RMS Date after parsing and converting to a specific format required for verification is" + RMSDate);

                if (RMSStatus.ToUpper() != "SUCCESS")
                {
                    FailStep(CL, "Failed to verify that the RMS status fetched from RMS " + RMSStatus + " is not SUCCESS which is Expected");
                }
                LogCommentImportant(CL, "Verified that the RMS status fetched from RMS " + RMSStatus + " is SUCCESS which is Expected");

                if (expectedRMSDateFromDiagnostics != RMSDate)
                {
                    FailStep(CL, "Failed to verify that the Expected RMS date from Diagnostics " + expectedRMSDateFromDiagnostics + " is same as obtained value from RMS " + RMSDate);
                }
                LogCommentImportant(CL, "Verified that the Expected RMS date from Diagnostics " + expectedRMSDateFromDiagnostics + " is same as obtained value from RMS " + RMSDate);
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
		driver.Close();
    }

    #endregion PostExecute
}
