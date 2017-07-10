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


    public class SetGaurdTimes : _Test
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
        static string startGaurdTimePath;
        static string divGaurdTime;
        static string browserStartGaurdTimeId;
        static string obtainedStartGaurdTime;
        static string expectedStartGaurdTime;
        static string sendKeys_Box_StartGaurdTime;
        static string sendKeys_Panorama_StartGaurdTime;
        //end Gaurd time params
        static string endGaurdTimePath;
        static string browserEndGaurdTimeId;
        static string obtainedEndGaurdTime;
        static string expectedEndGaurdTime;
        static string sendKeys_Box_EndGaurdTime;
        static string sendKeys_Panorama_EndGaurdTime;

        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2:Set The Extra Time Before Programme parameter and navigate to menu language state on the box And fetch the Value ");
            this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
            this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
            this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
            this.AddStep(new Step6(), "Step6:Compare the box and panorama Extra Time Before Programme values");


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
                //Fetch the SettingTabId from Browser ini
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

                //div GaurdTime Value from browser ini
                divGaurdTime = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Gaurdtime");
                if (divGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the Gaurd Time div from browser ini");
                }
                else
                {
                    LogComment(CL, "epg div value fetched from browser ini is" + divGaurdTime);
                }

                //browserStartGaurdTime Id from Browser Ini
                browserStartGaurdTimeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "PVR_SETTINGS_PARAMS", "PVR_STARTGAURDTIME");
                if (browserStartGaurdTimeId == null)
                {
                    FailStep(CL, "Failed to Fetch the MenuLanguageId from Browser Ini");
                }
                else
                {
                    LogComment(CL, "Extra Time Before Programme id fetched from browser ini is " + browserStartGaurdTimeId);
                }


                //StartGaurdTime value to be send to set on box 
                sendKeys_Box_StartGaurdTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_StartGaurdTime");
                if (sendKeys_Box_StartGaurdTime == null)
                {
                    FailStep(CL, "Failed to Fetch the value from test ini ");
                }
                else
                {
                    if (sendKeys_Box_StartGaurdTime == "0")
                    {
                        sendKeys_Box_StartGaurdTime = "NONE";
                        LogComment(CL, "Start Gaurd time to be set on box is" + sendKeys_Box_StartGaurdTime);
                    }
                    else if (sendKeys_Box_StartGaurdTime == "AUTOMATIC")
                    {
                        LogComment(CL, "Start Gaurd time to be set on box is" + sendKeys_Box_StartGaurdTime);
                    }
                    else
                    {
                        LogComment(CL, "Start Gaurd time to be set on box is " + sendKeys_Box_StartGaurdTime);
                    }
                }
                //Start GaurdTime value to set on the panorama page
                sendKeys_Panorama_StartGaurdTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_StartGaurdTime");
                if (sendKeys_Panorama_StartGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the Value to set on panorama from  test ini");
                }
                else
                {
                    LogComment(CL, "Extra Time Before Programme Value to be set on panorama page is" + sendKeys_Panorama_StartGaurdTime + "minutes");
                }


                //end Gaurd Time params
                browserEndGaurdTimeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "PVR_SETTINGS_PARAMS", "PVR_ENDGAURDTIME");
                if (browserEndGaurdTimeId == null)
                {
                    FailStep(CL, "Failed to Fetch the MenuLanguageId from Browser Ini");
                }
                else
                {
                    LogComment(CL, "MenuExtra Gaurd Time After Programme fetched from browser ini is " + browserEndGaurdTimeId);
                }


                //EndGaurdTime value to be  set on box 
                sendKeys_Box_EndGaurdTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_EndGaurdTime");
                if (sendKeys_Box_EndGaurdTime == null)
                {
                    FailStep(CL, "Failed to Fetch the value from test ini ");
                }
                else
                {

                    if (sendKeys_Box_EndGaurdTime == "0")
                    {
                        sendKeys_Box_EndGaurdTime = "NONE";
                        LogComment(CL, "Start Gaurd time to be set on box is" + sendKeys_Box_EndGaurdTime);
                    }
                    else if (sendKeys_Box_EndGaurdTime == "AUTOMATIC")
                    {
                        LogComment(CL, "Start Gaurd time to be set on box is" + sendKeys_Box_EndGaurdTime);
                    }
                    else
                    {
                        LogComment(CL, "Start Gaurd time to be set on box is " + sendKeys_Box_EndGaurdTime);
                    }
                }

                //End GaurdTime value to be set on the panorama page
                sendKeys_Panorama_EndGaurdTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_EndGaurdTime");
                if (sendKeys_Panorama_EndGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the Value to set on panorama from  test ini");
                }
                else
                {
                    sendKeys_Panorama_EndGaurdTime += " min";
                    LogComment(CL, "Extra Gaurd Time After Programme Value to be set on panorama page is" + sendKeys_Panorama_EndGaurdTime);
                }
                //Path for apply button
                apply_Path = path1 + "div[3]" + applyPath1;
                //path to set epg value on panorama page
                startGaurdTimePath = path1 + "div[2]" + path2 + "div[1]" + path3 + "tr[1]" + path4;
                endGaurdTimePath = path1 + "div[2]" + path2 + "div[1]" + path3 + "tr[2]" + path4;
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
				CL.IEX.Wait(20);
                //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserSettingTabId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
                }
                else
                {
                    LogComment(CL, "Successfully Logged into Web Page and Entered cpeid and Navigated To Settings Tab");
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
                //set the Start gaurd Time value on panorama page     
                CL.IEX.Wait(5);
                try
                {
				    CL.IEX.Wait(5);
                    CL.EA.UI.RMS.SetParameterValues(driver, startGaurdTimePath, apply_Path, divGaurdTime, sendKeys_Panorama_StartGaurdTime);
                    CL.IEX.Wait(5);
                    CL.EA.UI.RMS.SetParameterValues(driver, endGaurdTimePath, apply_Path, divGaurdTime, sendKeys_Panorama_EndGaurdTime);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                LogComment(CL, "successfully set the parameters start gaurd time and end guard time on the panorama page");

                CL.IEX.Wait(2);
                //navigate to Extra Time Before Programme

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Extra Time Before Programme");

                }
                else
                {
                    LogComment(CL, "Navigated to Extra Time Before Programme");
                }
                //Fetch the value of start gaurd time from box
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedStartGaurdTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Start Gaurd Time Expected");
                }
                else
                {

                    LogComment(CL, " StartGaurd Time Setting Value After setting on the panorama page is " + expectedStartGaurdTime);

                }
                //getting end gaurd time value from the box after setting value on panorama page

                CL.IEX.Wait(2);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Extra Time After Programme");

                }
                else
                {
                    LogComment(CL, "Navigated to Extra Time After Programme");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedEndGaurdTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Extra Time After Programme Expected");
                }
                else
                {

                    LogComment(CL, "EndGaurd Time Setting Value on the box after setting value on panorama page is " + expectedEndGaurdTime);

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


               CL.IEX.Wait(2);
                //navigating to parameters tab
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, "Failed to Select the Parameter tab on the panorama page" + ex.Message);
                }
                LogComment(CL, "Succesfully selected the parameter tab on the panorama page");

                //fetching the value from panorama after setting the value
                 
                obtainedStartGaurdTime = CL.EA.UI.RMS.GetParameterValues(driver, browserStartGaurdTimeId);
                if (obtainedStartGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the Start gaurd value from panoram page");
                }

                else
                {
                    if (obtainedStartGaurdTime == "Automatic")
                    {

                        LogComment(CL, "Obtained Start Gaurd time from panorama page is" + obtainedStartGaurdTime);
                    }
                    else if (obtainedStartGaurdTime == "0")
                    {
                        obtainedStartGaurdTime = "NONE";
                        LogComment(CL, "Obtained Start Gaurd Time from panorama page is" + obtainedStartGaurdTime);
                    }
                    else
                    {
                        obtainedStartGaurdTime = (Convert.ToInt32(obtainedStartGaurdTime) / 60).ToString() + " min.";
                        LogComment(CL, "Obtainted Start gaurd time from panorama page is " + obtainedStartGaurdTime);

                    }
                }
				
                //Fetching the end gaurd time value from panorama after setting value on box
                obtainedEndGaurdTime = CL.EA.UI.RMS.GetParameterValues(driver, browserEndGaurdTimeId);
                if (obtainedEndGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the Extra Time After Programme value from panoram page");
                }

                else
                {
                    if (obtainedEndGaurdTime == "Automatic")
                    {

                        LogComment(CL, "Obtained End Gaurd time from panorama page is" + obtainedEndGaurdTime);
                    }
                    else if (obtainedEndGaurdTime == "0")
                    {
                        obtainedEndGaurdTime = "NONE";
                        LogComment(CL, "Obtained End Gaurd Time from panorama page is" + obtainedEndGaurdTime);
                    }
                    else
                    {
                        obtainedEndGaurdTime = (Convert.ToInt32(obtainedEndGaurdTime) / 60).ToString() + " min.";
                        LogComment(CL, "Obtainted End gaurd time from panorama page is " + obtainedEndGaurdTime);

                    }
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
                if (expectedStartGaurdTime != obtainedStartGaurdTime)
                {
                    FailStep(CL, "Both the values After setting on panorama page are not equal");
                }
                else
                {
                    LogComment(CL, "Both the values After setting on panorama page are  equal");
                }
                //comparing end gaurd time values
                if (expectedEndGaurdTime != obtainedEndGaurdTime)
                {
                    FailStep(CL, "Both the expected and obtained  values are not equal");
                }
                else
                {
                    LogComment(CL, "Both the expected and obtained  values are  equal");
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
                CL.IEX.Wait(2);
                //set the Start Gaurd time value on the box 

                res = CL.EA.STBSettings.SetGuardTime(true, sendKeys_Box_StartGaurdTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To set Start Gaurd Time to " + sendKeys_Box_StartGaurdTime);
                }
                else
                {

                    LogComment(CL, "Current Start Gaurd Time Before Setting " + sendKeys_Box_StartGaurdTime);

                }

                // set the EndGaurdTime value on the box 

                res = CL.EA.STBSettings.SetGuardTime(false, sendKeys_Box_EndGaurdTime);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To set Extra Gaurd Time After Programme to " + sendKeys_Box_EndGaurdTime);
                }
                else
                {

                    LogComment(CL, "Successfully set the End Gaurd Time to " + sendKeys_Box_EndGaurdTime);

                }

                //navigate to Extra time After programme State
				CL.IEX.Wait(5);

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to State Extra time Before Programme");

                }
                else
                {
                    LogComment(CL, "Navigated to Extra time Before Programme");
                }
                //Get the Start gaurd time value from box
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedStartGaurdTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Expected Gaurd time from Box ");
                }
                else
                {

                    LogComment(CL, "Current Start Gaurd Time is " + expectedStartGaurdTime);

                }
				CL.IEX.Wait(5);

                //Navigate and Fetch the End gaurd time value from the box
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Extra Gaurd Time After Programme");

                }
                else
                {
                    LogComment(CL, "Navigated to State EXTRA TIME AFTER PROGRAMME");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedEndGaurdTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get EXTRA TIME AFTER PROGRAMME Expected");
                }
                else
                {

                    LogComment(CL, "Current EXTRA TIME AFTER PROGRAMME " + expectedEndGaurdTime);

                }
                //Fetching the Start Gaurd time value from panorama after setting Value over box

				CL.IEX.Wait(2);
                obtainedStartGaurdTime = CL.EA.UI.RMS.GetParameterValues(driver, browserStartGaurdTimeId);
                if (obtainedStartGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the Extra Time Before Programme value from panorama page after setting value on the box");
                }
                else
                {
                    if (obtainedStartGaurdTime == "Automatic")
                    {

                        LogComment(CL, "Obtained Start Gaurd time from panorama page is" + obtainedStartGaurdTime);
                    }
                    else if (obtainedStartGaurdTime == "0")
                    {
                        obtainedStartGaurdTime = "NONE";
                        LogComment(CL, "Obtained Start Gaurd Time from panorama page is" + obtainedStartGaurdTime);
                    }
                    else
                    {
                        obtainedStartGaurdTime = (Convert.ToInt32(obtainedStartGaurdTime) / 60).ToString() + " min.";
                        LogComment(CL, "Obtainted Start gaurd time from panorama page is " + obtainedStartGaurdTime);

                    }
                }
                //Fetching ecnd gaurd time value from the panorama after setting value on the box
                CL.IEX.Wait(2);
                obtainedEndGaurdTime = CL.EA.UI.RMS.GetParameterValues(driver, browserEndGaurdTimeId);
                if (obtainedEndGaurdTime == null)
                {
                    FailStep(CL, "Failed to fetch the  Extra Gaurd Time After Programme value from panorama page after setting value on the box");
                }
                else
                {
                    if (obtainedEndGaurdTime == "Automatic")
                    {

                        LogComment(CL, "Obtained Start Gaurd time from panorama page is" + obtainedEndGaurdTime);
                    }
                    else if (obtainedEndGaurdTime == "0")
                    {
                        obtainedEndGaurdTime = "NONE";
                        LogComment(CL, "Obtained Start Gaurd Time from panorama page is" + obtainedEndGaurdTime);
                    }
                    else
                    {
                        obtainedEndGaurdTime = (Convert.ToInt32(obtainedEndGaurdTime) / 60).ToString() + " min.";
                        LogComment(CL, "Obtainted Start gaurd time from panorama page is " + obtainedEndGaurdTime);

                    }
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
                //comparing the panorama and box values after setting value on the box
                if (obtainedStartGaurdTime != expectedStartGaurdTime)
                {
                    FailStep(CL, "After Setting value on the box Both the values over The box and panorama are not same");

                }
                else
                {
                    LogComment(CL, "Both the Values After Setting Value on the box are same");
                }

                //comparing the expected and obtained values after setting value on the box
                if (obtainedEndGaurdTime != expectedEndGaurdTime)
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




