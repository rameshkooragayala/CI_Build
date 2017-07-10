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
using System;
using IEX.Tests.Engine;
using System.Collections.Generic;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using IEX.ElementaryActions.Functionality;
using System.Xml;

    public class RMS_0027:_Test
    {
       [ThreadStatic]
       static FirefoxDriver driver;
       private static _Platform CL;
       static string browserParameterTabId;
       static string browserSettingTabId;
       static string cpeId;
       static string  browserEpgVersionId;
       static string expectedEpgVersion;
       static string obtainedEpgVersion;
       static string browserLuminosityId;
       static string expectedLuminosity;
       static string obtainedLuminosity;
       static string sendKeys_Box_Luminosity;
       static string sendKeys_Panorama_Luminosity;
       static string setLuminosityPath;
       static string path1;
       static string path2;
       static string path3;
       static string path4;
       static string applyPath1;
       static string apply_Path;
       static string div_General;
       static string timeStamp = "";
       static int loopCount;

       #region Create Structure

       public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2:Fetch the EpgVersion from xml file and compare with panorama value");
            this.AddStep(new Step3(), "Step3:Setting the Luminosity value in CPE and Comparing the same with Panorama page ");

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
                browserParameterTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (browserParameterTabId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

                }
                browserSettingTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_Id");
                if (browserSettingTabId == null)
                {
                    FailStep(CL, "Failed to fetch  browserSettingTabId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserSettingTabId fetched is : " + browserSettingTabId);

                }

                cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }
                browserEpgVersionId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "EPG_VERSION_ID");
                if (browserParameterTabId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

                }
                browserLuminosityId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "LUMINOSITYID");
                if (browserLuminosityId == null)
                {
                    FailStep(CL, "Failed to fetch  browserLuminosityId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserLuminosityId fetched is : " + browserLuminosityId);

                }
                sendKeys_Box_Luminosity = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_Luminosity");
                if (sendKeys_Box_Luminosity == null)
                {
                FailStep(CL, "Failed to Fetch the Luminosity value to be set on the box from test ini ");
                 }
                else
                {

                LogComment(CL, "Luminosity Value to be set on box is " + sendKeys_Box_Luminosity);

                }
                sendKeys_Panorama_Luminosity = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Panorama_Luminosity");
                if (sendKeys_Panorama_Luminosity == null)
                {
                    FailStep(CL, "Failed to Fetch the Luminosity value to be set on panorama from test ini ");
                }
                else
                {

                    LogComment(CL, "Luminosity Value to be set on Panorama is " + sendKeys_Panorama_Luminosity);

                }
                div_General = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_General");
                if (div_General == null)
                {
                    FailStep(CL, "Failed to fetch the  div Value from browser ini");
                }
                else
                {
                    LogComment(CL, "div value fetched from browser ini is" + div_General);
                }

                path1 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_path1");
                if (path1 == null)
                {
                    FailStep(CL, "Failed to fetch  path1  from browser ini");
                }
                else
                {
                    LogComment(CL, "Path1 Fetched from browser");
                }

                path2 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_path2");
                if (path2 == null)
                {
                    FailStep(CL, "Failed to fetch path2  from browser ini");
                }
                else
                {
                    LogComment(CL, "Path2 Fetched from browser");
                }

                path3 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_path3");
                if (path3 == null)
                {
                    FailStep(CL, "Failed to fetch path3  from browser ini");
                }
                else
                {
                    LogComment(CL, "Path3 Fetched from browser");
                }

                path4 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_path4");
                if (path4 == null)
                {
                    FailStep(CL, "Failed to fetch path4  from browser ini");
                }
                else
                {
                    LogComment(CL, "Path4 Fetched from browser");
                }
                //apply button path from browser ini
                applyPath1 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_Apply_Path");
                if (applyPath1 == null)
                {
                    FailStep(CL, "Failed to fetch the Apply settings button path");
                }
                else
                {
                    LogComment(CL, "ApplyPath1 fetched successfully");
                }
                //Path for apply button
                apply_Path = path1 + "div[2]" + applyPath1;
                //path to set Series Booking value on panorama page
                setLuminosityPath = path1 + "div[3]" + path2 + "div[4]" + path3 + "tr[3]" + path4;
                
                PassStep();
            }
        }
       #endregion Precondition
        #region Step1
        private class Step1 : _Step
        {
            public override void Execute()
            {
                StartStep();
                CL.IEX.Wait(5);
                driver = new FirefoxDriver();
                CL.IEX.Wait(5);
                //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserParameterTabId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully Executed the login EA");
                }
                CL.IEX.Wait(2);
                obtainedEpgVersion = CL.EA.UI.RMS.GetParameterValues(driver, browserEpgVersionId);

                if (obtainedEpgVersion == "")
                {
                    FailStep(CL, "Failed due to fetched obtainedEpg version value from Panorama is null");

                }
                else
                LogCommentImportant(CL, "obtained Epg Version from Panorama page is " + obtainedEpgVersion);
               
                XmlDocument xmlDoc = new XmlDocument();
                string xmlpath = "C:\\Program Files\\IEX\\Tests\\TestsINI\\IEX"+CL.IEX.IEXServerNumber+"\\Dictionary\\EPG_properties.xml";
                xmlDoc.Load(xmlpath);
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("prop");
                string attrVal = nodeList[0].Attributes["name"].Value;
                expectedEpgVersion = nodeList[0].InnerXml.ToString();
                if (expectedEpgVersion == null)
                {
                    FailStep(CL, "Expected Epg Version is null");
                }
                else
                    LogCommentInfo(CL, "Expected EpgVErsion is"+expectedEpgVersion);


                LogCommentInfo(CL, "Setting the luminosity value on the panorama page to"+sendKeys_Panorama_Luminosity);
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                CL.EA.UI.RMS.SetParameterValues(driver, setLuminosityPath, apply_Path, div_General, sendKeys_Panorama_Luminosity);
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                obtainedLuminosity = CL.EA.UI.RMS.GetParameterValues(driver, browserLuminosityId);
                if (obtainedLuminosity == null)
                {
                    FailStep(CL, "Failed to fetch the luminosity value from panrorama");

                }
                else
                {
                    switch (obtainedLuminosity)
                    { 
                        case "0":
                            obtainedLuminosity = "OFF";
                            break;
                        case "4":
                            obtainedLuminosity = "LOW";
                            break;
                        case "8":
                            obtainedLuminosity = "MEDIUM";
                            break;
                        case "15":
                            obtainedLuminosity = "HIGH";
                            break;
                    }
                    LogCommentInfo(CL, "Obtained Luminosity value from panorama is " + obtainedLuminosity);
                }

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:FRONT PANEL BRIGHTNESS");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Navigate to Front panel Display state");

                }
                else
                {
                    LogCommentInfo(CL, "Successfully navigated to Front panel display state");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedLuminosity);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Fetch the Luminosity value from box");
                }
                else
                {
                    LogCommentInfo(CL, "OBtained Luminosityvalue from box is"+expectedLuminosity);
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
                if (expectedEpgVersion != obtainedEpgVersion)
                {
                    FailStep(CL, "Both the Values of Epg Versions are not same");
                }
                else
                {
                    LogCommentInfo(CL, "Both the Values of Epg Version are same");
                }
                if (expectedLuminosity != obtainedLuminosity)
                {
                    FailStep(CL, "Both the values of luminosity are not equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both the values of luminosity are equal");
                }
                PassStep();
            
            
            }

        }
        #endregion Step2
        #region Step3
        private class Step3 : _Step
        {
            public override void Execute()
            {
                StartStep();
                LogCommentInfo(CL, "Setting the value of luminosity on the box");
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:FRONT PANEL BRIGHTNESS");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Navigate to Front panel Display state");

                }
                else
                {
                    LogCommentInfo(CL, "Successfully navigated to Front panel display state");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedLuminosity);
                if (expectedLuminosity == sendKeys_Box_Luminosity)
                {
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                else
                {
                    try
                    {
                        do
                        {
                            CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedLuminosity);
                            loopCount++;
                        }
                        while (expectedLuminosity != sendKeys_Box_Luminosity || loopCount < 4);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, ex.Message);
                    }
                    LogComment(CL, "Successfully set the Luminosity to " + expectedLuminosity);
                }
                obtainedLuminosity = CL.EA.UI.RMS.GetParameterValues(driver, browserLuminosityId);
                if (obtainedLuminosity == null)
                {
                    FailStep(CL, "Failed to fetch the luminosity value from panrorama");

                }
                else
                {
                    switch (obtainedLuminosity)
                    {
                        case "0":
                            obtainedLuminosity = "OFF";
                            break;
                        case "4":
                            obtainedLuminosity = "LOW";
                            break;
                        case "8":
                            obtainedLuminosity = "MEDIUM";
                            break;
                        case "15":
                            obtainedLuminosity = "HIGH";
                            break;
                    }
                    LogCommentInfo(CL, "Obtained Luminosity value from panorama is " + obtainedLuminosity);
                }

                if (expectedLuminosity != obtainedLuminosity)
                {
                    FailStep(CL, "Both the values of luminosity are not equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both the values of luminosity are equal");
                }
                PassStep();


            }

        }
        #endregion Step3
        #endregion Steps

        #region PostExecute
        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute
    }

