using System;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using FailuresHandler;
using System.Data;
using System.Linq;

    public class RMS_0002 : _Test
    {
        [ThreadStatic]

       
        static FirefoxDriver driver;
        static string cpeId;
        static string browserTabControlId;
        static string browserModelNameId;
        static string browserBootLoaderId;
        static string browserManufacturerOuiId;
        static string browserHardwareVersionId;
        static string browserOperator_snId;
        static string browserSoftImgId;


        static string expectedModelName;
        static string expectedBootLoader;
        static string expectedManufacturerOui;
        static string expectedHardwareVersion;
        static string expectedOperator_sn;
        static string expectedSoftImgId;

        static string[] ValidateStrings;
        static string versionPath = @"/dev/nds/versioninformation";


        static string obtainedModelName;
        static string obtainedBootLoader;
        static string obtainedManufacturerOui;
        static string obtainedHardwareVersion;
        static string obtainedOperator_sn;
        static string obtainedSoftImgId;
        static string inputPath = "";

        static string catCommandPath;
        static bool ishomenet;

        static string versionCmd;

        private static _Platform CL;
        private static bool isFail = false;
        private static int noOfFailures = 0;


        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step 2: Passing The Parameter Value On To The Panorama Page And Fetch The Value And verify");

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
                 string isHomeNetwork = CL.EA.GetValueFromINI(EnumINIFile.Test,"TEST_PARAMS","IsHomeNetwork");

                ////If Home network is true perform GetGateway
                try
                {
                    ishomenet = Convert.ToBoolean(isHomeNetwork);
                }
                catch
                {
                    ishomenet = false;
                }

                //Executing VersionInfo Command and storing values in file

                catCommandPath = CL.EA.UI.Utils.GetValueFromEnvironment("PcatCopyCommand");

                if (catCommandPath == null)
                {
                    FailStep(CL, "Failed to fetch PcatcopyCommand from environment ini file");
                }
                else
                {
                    LogComment(CL, "fetched Pcatcommand is" + catCommandPath);
                }
                inputPath = @"C:/PCAT_Modifier/IEX" + CL.IEX.IEXServerNumber + "/versionInfo";


                versionCmd = catCommandPath + " C:/PCAT_Modifier/IEX" + CL.IEX.IEXServerNumber + "/versionInfo " + versionPath;

                try
                {

                    if (File.Exists(inputPath))
                    {
                        try
                        {
                            File.Delete(inputPath);
                        }
                        catch (Exception ex)
                        {
                            FailStep(CL, "Exception Caught" + ex);

                        }

                    }
                }
                catch (Exception ex)
                {
                    FailStep(CL, "Exception Caught" + ex);
                }

                //////////////////////////////changed Aswin

                if (ishomenet)
                {
                    CL.EA.TelnetLogIn(false, false);
                }
                else
                {
                    CL.EA.TelnetLogIn(false);
                }
                try
                {

                    CL.EA.SendCmd(versionCmd, false, ref ValidateStrings);


                }
                catch (Exception)
                {
                    FailStep(CL, "WARNING : Retrying To Send Command : " + versionCmd);
                }

                CL.EA.SendCmd("sync", false, ref ValidateStrings);

                CL.EA.TelnetDisconnect(false);

                //Fetching values from versionInfo file

                //expectedSoftImgId
                expectedSoftImgId = getvalueFromVersion(inputPath, "SSU_SW_VER");

                if (expectedSoftImgId == null)
                {
                    FailStep(CL, "Failed to fetch  SoftwareImgid from version File.");
                }
                else
                {
                    LogComment(CL, "SoftwareImgId fetched is : " + expectedSoftImgId);

                }

                //expectedModelName
                expectedModelName = getvalueFromVersion(inputPath, "MODEL_NAME");

                if (expectedModelName == null)
                {
                    FailStep(CL, "Failed to fetch  model name from version File.");
                }
                else
                {
                    LogComment(CL, "modelname fetched is : " + expectedModelName);

                }
                //expectedBootLoader
                expectedBootLoader = getvalueFromVersion(inputPath, "BSL_SW_VER");


                if (expectedBootLoader == null)
                {
                    FailStep(CL, "Failed to fetch  Bootloader from version File.");
                }
                else
                {
                    LogComment(CL, "Boot Loader Value fetched is : " + expectedBootLoader);

                }
                //expectedHardwareVersion
                string MODEL_ID = getvalueFromVersion(inputPath, "MODEL_ID");
                string MODEL_VER = getvalueFromVersion(inputPath, "MODEL_VER");
                expectedHardwareVersion = MODEL_ID + ":" + MODEL_VER;

                if (expectedHardwareVersion == null)
                {
                    FailStep(CL, "Failed to fetch  hardware Version from version File.");
                }
                else
                {
                    LogComment(CL, "Hardware Version fetched is : " + expectedHardwareVersion);

                }
                //expectedManufacturerOui
                expectedManufacturerOui = getvalueFromVersion(inputPath, "SSU_OUI_ID");

                if (expectedManufacturerOui == null)
                {
                    FailStep(CL, "Failed to fetch  manufacturerOui from version File.");
                }
                else
                {
                    LogComment(CL, "ManufacturerOui fetched is : " + expectedManufacturerOui);

                }

                //expectedOperator_sn
                expectedOperator_sn = getvalueFromVersion(inputPath, "OPERATOR_SN");

                if (expectedOperator_sn == null)
                {
                    FailStep(CL, "Failed to fetch  operatorSn from version File.");
                }
                else
                {
                    LogComment(CL, "OperatorSn fetched is : " + expectedOperator_sn);

                }


                //fetch values from browser ini 

                //cpeid
                cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }
                //browserTabcontrolId
                browserTabControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (browserTabControlId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserTabControlId);

                }
                //browserSoftImgId
                browserSoftImgId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "SoftwareImageId");
                if (browserSoftImgId == null)
                {
                    FailStep(CL, "Failed to fetch SoftwareImg BrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "SoftwareImageId fetched is : " + browserSoftImgId);

                }


                //browserHardwareVersionId
                browserHardwareVersionId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "HARDWAREVERSION");
                if (browserHardwareVersionId == null)
                {
                    FailStep(CL, "Failed to fetch browserHardwareVersionId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserHardwareVersionId fetched is : " + browserHardwareVersionId);

                }
                //browserBootLoaderId
                browserBootLoaderId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "BOOTLOADER_VERSION");
                if (browserBootLoaderId == null)
                {
                    FailStep(CL, "Failed to fetch browserBootLoaderId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserBootLoaderId fetched is : " + browserBootLoaderId);

                }


                //BrowsermanufacturerId
                browserManufacturerOuiId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "MANUFACTUREROUI");

                if (browserManufacturerOuiId == null)
                {
                    FailStep(CL, "Failed to fetch browserManufacturerOuiId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserManufacturerOuiId fetched is : " + browserManufacturerOuiId);

                }
                //browserModelNameId
                browserModelNameId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "MODELNAME");

                if (browserModelNameId == null)
                {
                    FailStep(CL, "Failed to fetch browserModelNameId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserModelNameId fetched is : " + browserModelNameId);

                }

                //browseroperator_snId
                browserOperator_snId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "OPERATOR_SN");

                if (browserOperator_snId == null)
                {
                    FailStep(CL, "Failed to fetch browserOperator_snId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserOperator_snId fetched is : " + browserOperator_snId);

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
                driver = new FirefoxDriver();

                //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 

                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserTabControlId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
                }

                PassStep();
            }

        }

        #endregion Step1

        #region step2

        private class Step2 : _Step
        {
            public override void Execute()
            {
                StartStep();

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

                //SoftwareImgid
                obtainedSoftImgId = CL.EA.UI.RMS.GetParameterValues(driver, browserSoftImgId);
                if (obtainedSoftImgId == "")
                {
                    LogComment(CL, "Failed to fetch the bootLoader value from Panorama");

                }

                LogCommentImportant(CL, "obtained SoftwareImgId from Panorama is " + obtainedSoftImgId);

                //verifing obtained panorama value and expected value
                if (expectedSoftImgId != obtainedSoftImgId)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + obtainedSoftImgId + "And Fetched value From VersionInfo File" + expectedSoftImgId + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedSoftImgId + "And Fetched value From VersionInfo File" + expectedSoftImgId + "Are Equal");
                }
                CL.IEX.Wait(5);

                //BootLoader
                obtainedBootLoader = CL.EA.UI.RMS.GetParameterValues(driver, browserBootLoaderId);
                if (obtainedBootLoader == "")
                {
                    LogComment(CL, "Failed to fetch the bootLoader value from Panorama");

                }

                LogCommentImportant(CL, "obtained bootLoader from Panorama is " + obtainedBootLoader);

                //verifing obtained panorama value and expected value
                if (expectedBootLoader != obtainedBootLoader)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + obtainedBootLoader + "And Fetched value From VersionInfo File" + expectedBootLoader + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedBootLoader + "And Fetched value From VersionInfo File" + expectedBootLoader + "Are Equal");
                }
                CL.IEX.Wait(5);

                //Hardware version
                obtainedHardwareVersion = CL.EA.UI.RMS.GetParameterValues(driver, browserHardwareVersionId);

                if (obtainedHardwareVersion == "")
                {
                    LogComment(CL, "Failed to fetch the Hardware Version value from Panorama");

                }

                LogCommentImportant(CL, "obtained Hardware Version from Panorama is " + obtainedHardwareVersion);

                //verifing obtained panorama value and expected value
                if (expectedHardwareVersion != obtainedHardwareVersion)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + obtainedHardwareVersion + "And Fetched value From VersionInfo File" + expectedHardwareVersion + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedHardwareVersion + "And Fetched value From VersionInfo File" + expectedHardwareVersion + "Are Equal");
                }
                CL.IEX.Wait(5);

                //manufactureroui

                obtainedManufacturerOui = CL.EA.UI.RMS.GetParameterValues(driver, browserManufacturerOuiId);

                if (obtainedManufacturerOui == "")
                {
                    LogComment(CL, "Failed to fetch the manufacturerOui value from Panorama");

                }

                LogCommentImportant(CL, "obtained manufacturerOui from Panorama is " + obtainedManufacturerOui);

                //verifing obtained panorama value and expected value
                if (expectedManufacturerOui != obtainedManufacturerOui)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + obtainedManufacturerOui + "And Fetched value From VersionInfo File" + expectedManufacturerOui + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedManufacturerOui + "And Fetched value From VersionInfo File" + expectedManufacturerOui + "Are Equal");
                }
                CL.IEX.Wait(5);
                //operator_sn
                obtainedOperator_sn = CL.EA.UI.RMS.GetParameterValues(driver, browserOperator_snId);

                if (obtainedOperator_sn == "")
                {
                    LogComment(CL, "Failed to fetch the operaor_sn value from Panorama");

                }

                LogCommentImportant(CL, "obtained operator_sn from Panorama is " + obtainedOperator_sn);

                //verifing obtained panorama value and expected value
                if (expectedOperator_sn != obtainedOperator_sn)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + obtainedOperator_sn + "And Fetched value From VersionInfo File" + expectedOperator_sn + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedOperator_sn + "And Fetched value From VersionInfo File" + expectedOperator_sn + "Are Equal");
                }
                CL.IEX.Wait(5);

                //obtainedModelName
                obtainedModelName = CL.EA.UI.RMS.GetParameterValues(driver, browserModelNameId);

                if (obtainedModelName == "")
                {
                    LogComment(CL, "Failed to fetch the model name value from Panorama");

                }

                LogCommentImportant(CL, "obtained model name from Panorama is " + obtainedModelName);

                //verifing obtained panorama value and expected value
                if (expectedModelName != obtainedModelName)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + obtainedModelName + "And Fetched value From VersionInfo File" + expectedModelName + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedModelName + "And Fetched value From VersionInfo File" + expectedModelName + "Are Equal");
                }
                CL.IEX.Wait(5);



                //Displays Number of Verifications Failed In the Final Step
                if (isFail)
                {
                    FailStep(CL, "Number of validations failed " + noOfFailures + "...Please see above for Failure reasons");
                }
                PassStep();

            }
        }
        #endregion Step2

        #endregion Steps

        #region PostExecute

        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute

        public static string getvalueFromVersion(string inputPath, string key)
        {
            DataTable temptable = new DataTable();
            temptable.Columns.Add("keys");
            temptable.Columns.Add("values");
            using (StreamReader str = new StreamReader(inputPath))
            {
                string line;
                while ((line = str.ReadLine()) != null)
                {
                    string[] temp = line.Split(':');
                    if (temp != null)
                        if (temp.Count() == 3)
                            temptable.Rows.Add(temp[0].ToString(), (temp[1].ToString() + ":" + temp[2].ToString()).ToString());
                        else if (temp.Count() == 2)
                            temptable.Rows.Add(temp[0].ToString(), temp[1].ToString());

                }
            }

            return temptable.Select("keys='" + key + "'")[0][1].ToString();

        }
    }

