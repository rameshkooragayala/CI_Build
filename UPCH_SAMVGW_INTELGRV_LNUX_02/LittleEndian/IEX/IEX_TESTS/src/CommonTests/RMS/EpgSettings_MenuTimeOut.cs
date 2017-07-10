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


    public class EpgSettings_MenuTimeOut : _Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string browserParameterTabId;
        static string browserSettingTabId;
        static string cpeId;
        static string path1;
        static string path2;
        static string path3;
        static string path4;
        static string applyPath1;
        static string apply_Path;
        static string epgTimeOutPath;
        static string divEpg;
        static string browserEpgMenuTimeoutId;
        static string obtainedEpgMenuTimeout;
        static string expectedMenuTimeout;
        static string sendKeys_Box_TimeOut;
        static string sendKeys_Panorama_TimeOut;
        private static EnumChannelBarTimeout setTimeOut;
        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2:Set The menu TimeOut parameter and navigate to menu language state on the box And fetch the Value ");
            this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
            this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
            this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
            this.AddStep(new Step6(), "Step6:Compare the box and panorama Timeout values");


            CL = GetClient();
        }
        #endregion Create Structure
        #region steps
        #region PreCondition
        private class PreCondition : _Step
        {
            public override void Execute()
            {

                StartStep();
                //cpeid from environment ini   
                cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }
                //Fetch the ParamaterTabId from Browser ini
                browserSettingTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_Id");
                if (browserSettingTabId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserSettingTabId);

                }

                //Fetch the ParamaterTabId from Browser ini
                browserParameterTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (browserParameterTabId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

                }
                //Fetching values from browser ini
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

                //div epg Value
                divEpg = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Epg");
                if (divEpg == null)
                {
                    FailStep(CL, "Failed to fetch the epg div from browser ini");
                }
                else
                {
                    LogComment(CL, "epg div value fetched from browser ini is" + divEpg);
                }

                //browserMenuTimeout Id
                browserEpgMenuTimeoutId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "EPG_SETTINGS_PARAMS", "EPG_MENUTIMEOUT");
                if (browserEpgMenuTimeoutId == null)
                {
                    FailStep(CL, "Failed to Fetch the MenuLanguageId from Browser Ini");
                }
                else
                {
                    LogComment(CL, "MenuTimeOutId fetched from browser ini is " + browserEpgMenuTimeoutId);
                }


                //Time out value to be send to set on box 
                sendKeys_Box_TimeOut = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_Timeout");
                if (sendKeys_Box_TimeOut == null)
                {
                    FailStep(CL, "Failed to Fetch the value from test ini ");
                }
                else
                {
                    LogComment(CL, "Timeout Value to set over the box is " + sendKeys_Box_TimeOut);
                }
                //Timeout value to set on the panorama page
                sendKeys_Panorama_TimeOut = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_Timeout");
                if (sendKeys_Panorama_TimeOut == null)
                {
                    FailStep(CL, "Failed to fetch the Value to set on panorama from  test ini");
                }
                else
                {
                    LogComment(CL, "Timeout Value to be set on panorama page is" + sendKeys_Panorama_TimeOut);
                }
                //Path for apply button
                apply_Path = path1 + "div[2]" + applyPath1;
                //path to set epg value on panorama page
                epgTimeOutPath = path1 + "div[3]" + path2 + "div[2]" + path3 + "tr[2]" + path4;
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
                driver = new FirefoxDriver();
			
                //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserSettingTabId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
                }
                else
                {
                    LogComment(CL, "Successfully Loggedinto web Page and entered cpeid and navigated to settings tab");
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
                //set the epg TimeOut parameter value on panorama page            
                try
                {
                    CL.EA.UI.RMS.SetParameterValues(driver, epgTimeOutPath, apply_Path, divEpg, sendKeys_Panorama_TimeOut);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                LogComment(CL, "successfully set the parameter");

                CL.IEX.Wait(5);
                //navigate to menu Timeout

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Menu TimeOut");

                }
                else
                {
                    LogComment(CL, "Navigated to menu TimeOut");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMenuTimeout);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Menu TimeOut Expected");
                }
                else
                {

                    LogComment(CL, "Current Menu TimeOut " + expectedMenuTimeout);

                }
            }
        }
        #endregion Step2
        #region Step3
        private class Step3 : _Step
        {
            public override void Execute()
            {

                StartStep();


				CL.IEX.Wait(5);

                //navigating to parameters tab
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                //fetching the value from panorama after setting the value

			
                obtainedEpgMenuTimeout = CL.EA.UI.RMS.GetParameterValues(driver, browserEpgMenuTimeoutId);
                if (obtainedEpgMenuTimeout == null)
                {
                    FailStep(CL, "Failed to fetch the menu timeout value from panoram page");
                }

                else
                {
                    obtainedEpgMenuTimeout += " sec.";
                    LogComment(CL, "Obtained menu time out from panorama page is" + obtainedEpgMenuTimeout);

                }

                PassStep();

            }
        }
        #endregion Step3
        #region Step4
        private class Step4 : _Step
        {
            public override void Execute()
            {

                StartStep();
                //comparing the value with the box value after setting value on panorama page.
                if (expectedMenuTimeout != obtainedEpgMenuTimeout)
                {
                    FailStep(CL, "Both the expected and obtained menu time out values are not equal");
                }
                else
                {
                    LogComment(CL, "Both the expected and obtained menu time out values are  equal");
                }
                PassStep();

            }

        }

        #endregion Step4
        #region Step5
        private class Step5 : _Step
        {
            public override void Execute()
            {

                StartStep();

                // set the timeout value on the box 

				CL.IEX.Wait(5);
                setTimeOut = (EnumChannelBarTimeout)Enum.Parse(typeof(EnumChannelBarTimeout), sendKeys_Box_TimeOut, true);
                res = CL.EA.STBSettings.SetBannerDisplayTime(setTimeOut);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To set Menu TimeOut to " + setTimeOut);
                }
                else
                {

                    LogComment(CL, "Chanel bar timeout set on the box is  " + setTimeOut);

                }
                //navigate to menu Timeout

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Menu TimeOut");

                }
                else
                {
                    LogComment(CL, "Navigated to menu TimeOut After Setting Value on The Box");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMenuTimeout);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Menu TimeOut Expected");
                }
                else
                {

                    LogComment(CL, "Timeout Value after setting on the box " + expectedMenuTimeout);

                }
                obtainedEpgMenuTimeout = CL.EA.UI.RMS.GetParameterValues(driver, browserEpgMenuTimeoutId);
                if (obtainedEpgMenuTimeout == null)
                {
                    FailStep(CL, "Failed to fetch the Channel bar timeout value from panorama page after setting value on the box");
                }
                else
                {
                    obtainedEpgMenuTimeout += " sec.";
                    LogComment(CL, "Fetched value from panorama page after value set on the box is " + obtainedEpgMenuTimeout);
                }
                PassStep();

            }
        }
        #endregion Step5
        #region Step6
        private class Step6 : _Step
        {
            public override void Execute()
            {

                StartStep();
                if (obtainedEpgMenuTimeout != expectedMenuTimeout)
                {
                    FailStep(CL, "After Setting value on the box Both the values over The box and panorama are not same");

                }
                else
                {
                    LogComment(CL, "Both the Values After Setting Value on the box are same");
                }
                PassStep();
            }
        }

        #endregion Step6
        #endregion Steps
        #region PostExecute

        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute
    }




