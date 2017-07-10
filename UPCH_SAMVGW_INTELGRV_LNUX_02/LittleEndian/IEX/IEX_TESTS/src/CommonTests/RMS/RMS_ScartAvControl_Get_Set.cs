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

 public class RMS_ScartAvControl_Get_Set : _Test
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
        static string divScart;
      //ScartAvcontrol Value
        static string obtainedScartAvControl;
        static string expectedScartAvControl;
        static string browserScartAvControlId;
        static string sendKey_Box_ScartAvControl;
        static string sendKeY_Panorama_ScartAvControl;
        static string setScartAvcontrolPath;
        


        static int count = 0;
        static string timeStamp = "";
        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2: Setting the ScartAvcontrol setting value in CPE and Comparing the same with Panorama page");
            this.AddStep(new Step3(), "Step3: Setting the ScartAvcontrol setting value from Panorama and compaing the same with CPE  ");

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


                //div video Display Value
                divScart = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Scart");
                if (divScart == null)
                {
                    FailStep(CL, "Failed to fetch the Subtitles div from browser ini");
                }
                else
                {
                    LogComment(CL, "Video div value fetched from browser ini is" + divScart);
                }

                //Path for apply button
                apply_Path = path1 + "div[1]" + applyPath1;


                //Hdmi Color Modeparameters
                browserScartAvControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "SCART_AVCONTROL");
                if (browserScartAvControlId == null)
                {
                    FailStep(CL, "Failed to Fetch the browserScartAvControlId from browser ini");
                }

                else
                {
                    LogComment(CL, "browserScartAvControlId from browser ini fetched is " + browserScartAvControlId);
                }

                sendKey_Box_ScartAvControl = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_ScartAvControl");
                if (sendKey_Box_ScartAvControl == null)
                {
                    FailStep(CL, "Failed to fetch the ScartAvControl value to be set on box from test ini");
                }
                else
                {
                    LogComment(CL, "ScartAvControl Value to be set on box is fetched from test ini");
                }

                sendKeY_Panorama_ScartAvControl = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Panorama_ScartAvControl");
                if (sendKeY_Panorama_ScartAvControl == null)
                {
                    FailStep(CL, "Failed to fetch the ScartAvControl value to be set on panorama from test ini");
                }
                else
                {
                    LogComment(CL, "ScartAvControl to be set on panorama is fetched from test ini");
                }
               

                //path to set Main VIdeo Ouput value on panorama page
                setScartAvcontrolPath = path1 + "div[1]" + path2 + "div[3]" + path3 + "tr[1]" + path4;
                //setHdmiColorModePath = path1 + "div[1]" + path2 + "div[1]" + path3 + "tr[6]" + path4;
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
                res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:AUTOMATIC A//V SWITCHING (SCART)");
                if (!res.CommandSucceeded)
                {
                    LogCommentInfo(CL, "Failed to navigate to state ScartAv Control ");
                    res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:VIDEO SETTINGS MAINVIDEOCABLE");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to navigate to main video cable");
                    }
                    res = CL.IEX.MilestonesEPG.Navigate("SCART");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to select SCART");
                    }
                    res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:AUTOMATIC A//V SWITCHING (SCART)");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to navigate to state ScartAv Control");
                    }
                }

                CL.IEX.Wait(1);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedScartAvControl);

                if (expectedScartAvControl == sendKey_Box_ScartAvControl)
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
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedScartAvControl);
                            count++;
                        }
                        while (expectedScartAvControl != sendKey_Box_ScartAvControl || count < 2);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, ex.Message);
                    }
                    LogComment(CL, "Successfully set the parameter ScartAvControl Value to " + expectedScartAvControl);
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
                LogCommentInfo(CL, "Perform Login Panorama and getting the values");
                driver = new FirefoxDriver();

                //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserParameterTabId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
                }
                else
                {
                    LogComment(CL, "Successfully Logged into web Page and entered cpeid and navigated to parameters tab");
                }
                obtainedScartAvControl = CL.EA.UI.RMS.GetParameterValues(driver, browserScartAvControlId);
                if (obtainedScartAvControl == null)
                {
                    FailStep(CL, "Failed to fetch the Scart Av control value from panoram page");
                }
                else
                {
                    switch (obtainedScartAvControl)
                    {
                        case "Enabled":
                            obtainedScartAvControl = "ENABLE";
                            break;
                        case "Disabled":
                            obtainedScartAvControl = "DISABLED";
                            break;
                       

                    }
                    LogCommentInfo(CL, "Scart Av control Value obtained from panorama is" + obtainedScartAvControl);
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
                if (obtainedScartAvControl != expectedScartAvControl)
                {
                    FailStep(CL, "Both the Expected and Obtained ScartAvControl Are not Same");
                }
                else
                {
                    LogCommentInfo(CL, "Both the Expected and Obtained ScartAv control Values are same");
                }
                LogCommentInfo(CL, "Navigating to settings tab on panorama to set the scartAvcontrol value on the panorama");
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                CL.IEX.Wait(2);
                CL.EA.UI.RMS.SetParameterValues(driver, setScartAvcontrolPath, apply_Path, divScart, sendKeY_Panorama_ScartAvControl);
                CL.IEX.Wait(2);
                LogCommentInfo(CL, "Getting the ScartAvControl value after setting the value on panorama page");
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                obtainedScartAvControl = CL.EA.UI.RMS.GetParameterValues(driver, browserScartAvControlId);
                if (obtainedScartAvControl == null)
                {
                    FailStep(CL, "Failed to fetch the Scart Av control value from panoram page");
                }
                else
                {
                    switch (obtainedScartAvControl)
                    {
                        case "Enabled":
                            obtainedScartAvControl = "ENABLE";
                            break;
                        case "Disabled":
                            obtainedScartAvControl = "DISABLED";
                            break;


                    }
                    LogCommentInfo(CL, "Scart Av control Value obtained from panorama is" + obtainedScartAvControl);
                }
                LogCommentInfo(CL, "Navigating to scartavcontrol state on the box to get the updated value");
                res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:AUTOMATIC A//V SWITCHING (SCART)");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to ScartAvControl State");
                }
                else
                {
                    CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedScartAvControl);
                    LogCommentInfo(CL, "Expected Scart Av Control value is"+expectedScartAvControl);
                }

                LogCommentInfo(CL, "Comparing the expected and obtained values of scartAvcontrol values");
                if (obtainedScartAvControl != expectedScartAvControl)
                {
                    FailStep(CL, "Both the Expected and Obtained ScartAvControl Are not Same");
                }
                else
                {
                    LogCommentInfo(CL, "Both the Expected and Obtained ScartAv control Values are same");
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

