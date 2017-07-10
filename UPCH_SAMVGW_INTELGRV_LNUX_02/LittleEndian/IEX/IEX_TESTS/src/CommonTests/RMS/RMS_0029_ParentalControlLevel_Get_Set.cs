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

    public class RMS_0029:_Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string browserParameterTabId;
        static string browserSettingTabId;
        static string cpeId;
        static string browserParentalControlId;
        static string expectedParentalControl;
        static string obtainedParentalControl;
        static string sendKeys_Box_ParentalControl;
        static string sendKeys_Panorama_ParentalControl;
        static string setParentalControlPath;
        static string path1;
        static string path2;
        static string path3;
        static string path4;
        static string applyPath1;
        static string apply_Path;
        static string div_General;
        static string timeStamp = "";
        static int loopCount;
        private static EnumParentalControlAge setParentalControl;
        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step1:Setting the value in CPE and Comparing the same with Panorama page");
            this.AddStep(new Step2(), "Step2:Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step3(), "Step3:comparing the both expected and obtained values of parental cotrol level");
            this.AddStep(new Step4(), "Step4: Setting the parental control setting value from Panorama");
            this.AddStep(new Step5(), "Step5: Comparing the values after setting on panorama page ");

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
                LogCommentInfo(CL, "Fetching all the required values from ini files");
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
                browserParentalControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "PARENTALCONTROL");
                if (browserParentalControlId == null)
                {
                    FailStep(CL, "Fetched value of parental control browser id from browser ini is null");
                }
                else
                    LogCommentInfo(CL, "Fetched BrowserParentalControlId is "+browserParentalControlId);
                sendKeys_Box_ParentalControl = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_ParentalControl");
                if (sendKeys_Box_ParentalControl == null)
                {
                    FailStep(CL, "Failed to fetch  sendKeys_Box_ParentalControl from ini File.");
                }
                else
                {
                    LogComment(CL, "sendKeys_Box_ParentalControl fetched is : " + sendKeys_Box_ParentalControl);

                }
                sendKeys_Panorama_ParentalControl = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Panorama_ParentalControl");
                if (sendKeys_Panorama_ParentalControl == null)
                {
                    FailStep(CL, "Failed to fetch  sendKeys_Panorama_ParentalControl from ini File.");
                }
                else
                {
                    LogComment(CL, "sendKeys_Panorama_ParentalControl fetched is : " + sendKeys_Panorama_ParentalControl);

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
                setParentalControlPath = path1 + "div[3]" + path2 + "div[4]" + path3 + "tr[2]" + path4;
                
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
                LogCommentInfo(CL, "Setting the Parental Control Value on the box to"+sendKeys_Box_ParentalControl);
             

                setParentalControl = (EnumParentalControlAge)Enum.Parse(typeof(EnumParentalControlAge), sendKeys_Box_ParentalControl);
                res = CL.EA.STBSettings.SetParentalControlAgeLimit(setParentalControl);
                if(!res.CommandSucceeded)
                {
                FailStep(CL,"Failed to set the parental control level value on the box");
                }
                else
                {
                 LogCommentInfo(CL,"Succesfully set the parental control value on the box to"+setParentalControl);
                 expectedParentalControl=setParentalControl.ToString();
                 switch (expectedParentalControl)
                 { 
                     case "FSK_12":
                         expectedParentalControl = "FSK 12";
                         break;
                     case "FSK_16":
                         expectedParentalControl = "FSK 16";
                         break;
                     case "FSK_18":
                         expectedParentalControl = "FSK 18";
                         break;
                     case "FSK_6":
                         expectedParentalControl = "FSK 6";
                         break;
                     case "UNLOCK_ALL":
                         expectedParentalControl = "UNLOCK ALL";
                         break;
                 }
                 LogCommentInfo(CL, "Expected parental control on the box is" + expectedParentalControl);
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
                obtainedParentalControl = CL.EA.UI.RMS.GetParameterValues(driver, browserParentalControlId);
                if (obtainedParentalControl == null)
                {
                    FailStep(CL, "Failed to fetch the obtained parental control value from panrorama");
                }
                else
                {
                    switch (obtainedParentalControl)
                    {
                        case "3":
                            obtainedParentalControl = "FSK 6";
                            break;
                        case "9":
                            obtainedParentalControl = "FSK 12";
                            break;
                        case "13":
                            obtainedParentalControl = "FSK 16";
                            break;
                        case "15":
                            obtainedParentalControl = "FSK 18";
                            break;
                        case "255":
                            obtainedParentalControl = "UNLOCK ALL";
                            break;
                    }
                    LogCommentInfo(CL, "Obtained parental control value from panorama is"+obtainedParentalControl);
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
                if (expectedParentalControl != obtainedParentalControl)
                {
                    FailStep(CL, "Both the Expected and Obtained Parental control values are not same");

                }

                else
                {
                    LogCommentInfo(CL, "Both the expected and obtained parental control  are same");
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
                LogCommentInfo(CL, "setting the parental control value on the panorama page to "+sendKeys_Panorama_ParentalControl);
                try
                {
                    CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                    CL.IEX.Wait(10);
                    CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                //set the Subtitle Display parameter value on panorama page            
                try
                {
                    CL.IEX.Wait(20);
                    CL.EA.UI.RMS.SetParameterValues(driver, setParentalControlPath, apply_Path, div_General, sendKeys_Panorama_ParentalControl);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                LogComment(CL, "successfully set the parameters of parental control level settings on panorama");
                CL.IEX.Wait(2);
                try
                {
                    CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                    CL.IEX.Wait(10);
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogCommentInfo(CL, "Fetching the parental control level value from panorama page");
                obtainedParentalControl = CL.EA.UI.RMS.GetParameterValues(driver, browserParentalControlId);
                if (obtainedParentalControl == null)
                {
                    FailStep(CL, "Failed to fetch the obtained parental control value from panrorama");
                }
                else
                {
                    switch (obtainedParentalControl)
                    {
                        case "4":
                            obtainedParentalControl = "FSK 6";
                            break;
                        case "10":
                            obtainedParentalControl = "FSK 12";
                            break;
                        case "14":
                            obtainedParentalControl = "FSK 16";
                            break;
                        case "255":
                            obtainedParentalControl = "UNLOCK ALL";
                            break;
                    }
                    LogCommentInfo(CL, "Obtained parental control value from panorama is" + obtainedParentalControl);
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
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:LOCK PROGRAMMES BY AGE RATING");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to navigate to Parental control state");
                }
                else
                {
                    res = CL.EA.EnterDeafultPIN("LOCK PROGRAMMES BY AGE RATING");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed to enter default pin ");
                    }
                    else
                    {
                        LogCommentInfo(CL, "Successfully entered the default pin");
                    }
                    CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
                    CL.IEX.Wait(2);
                    CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedParentalControl);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed To Get Parental control  Value Expected");
                    }
                    else
                    {
                        LogComment(CL, "Current Parental control  Value is " + expectedParentalControl);

                    }
                
                }
                LogCommentInfo(CL, "Comapring the both expected and obtained values of parental control values");
                if (expectedParentalControl != obtainedParentalControl)
                {
                    LogCommentInfo(CL, "Both the Expected and Obtained Parental control values are not same after setting on the panorama page");
                }

                else
                {
                    LogCommentInfo(CL, "Both the expected and obtained parental control  are same");
                }
                PassStep();
            }
        }
        #endregion Step5
        #endregion Steps

        #region PostExecute
        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute
    }

