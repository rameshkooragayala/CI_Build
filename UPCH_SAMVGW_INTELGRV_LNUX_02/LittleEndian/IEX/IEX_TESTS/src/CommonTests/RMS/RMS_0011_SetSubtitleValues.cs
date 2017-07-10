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
using IEX.ElementaryActions.Functionality;
using OpenQA.Selenium.IE;


class RMS_0011 : _Test
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
    static string setSubtitlesLanguagePath;
    static string divSubtitles;
    static string browserSubtitlesLanguageId;
    static string obtainedSubtitlesLanguage;
    static string expectedSubtitlesLanguage;
    static string sendKeys_Box_SubtitlesLanguage;
    static string sendKeys_Panorama_SubtitlesLanguage;
    private static EnumLanguage setSubtitleLanguage;
    //subtitle display params
    static string browserSubtitleDisplayId;
    static string obtainedSubtitleDisplay;
    static string expectedSubtitleDisplay;
    static string sendKeys_Box_SubtitlesDisplay;
    static string sendKeys_Panorama_SubtitlesDisplay;
    static string setSubtitleDisplayPath;
   
    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2:Set The Subtitle Display parameter and navigate to menu language state on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
        this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
        this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
        this.AddStep(new Step6(), "Step6:Compare the box and panorama  Subtitle Language values");


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

            //div Subtitle Display Value
            divSubtitles = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Subtitles");
            if (divSubtitles == null)
            {
                FailStep(CL, "Failed to fetch the Subtitles div from browser ini");
            }
            else
            {
                LogComment(CL, "epg div value fetched from browser ini is" + divSubtitles);
            }

            //browserSubtitlesLanguageId
            browserSubtitlesLanguageId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "SUBTITLES_LANGUAGE");
            if (browserSubtitlesLanguageId == null)
            {
                FailStep(CL, "Failed to Fetch the Subtitle language id from Browser Ini");
            }
            else
            {
                LogComment(CL, "Subtitle language id fetched from browser ini is " + browserSubtitlesLanguageId);
            }


            //Subtitle Language value to be  set on box 
            sendKeys_Box_SubtitlesLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_SubtitleLanguage");
            if (sendKeys_Box_SubtitlesLanguage == null)
            {
                FailStep(CL, "Failed to Fetch the Subtitle Language value from test ini ");
            }
            else
            {

                LogComment(CL, "Subtitle Language Value to be set on box is " + sendKeys_Box_SubtitlesLanguage);

            }

            //Subtitle Language value to be set on the panorama page
            sendKeys_Panorama_SubtitlesLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_SubtitleLanguage");
            if (sendKeys_Panorama_SubtitlesLanguage == null)
            {
                FailStep(CL, "Failed to fetch the Subtitle Display Value to set on panorama from  test ini");
            }
            else
            {
                LogComment(CL, "Subtitle Display Value to be set on panorama page is" + sendKeys_Panorama_SubtitlesLanguage);
            }
            //Subtitle Display parameters

            browserSubtitleDisplayId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "SUBTITLES_DISPLAY");

            if (browserSubtitleDisplayId == null)
            {
                FailStep(CL, "Failed to Fetch the Subtitle Display id from Browser Ini");
            }
            else
            {
                LogComment(CL, "Subtitle Display id fetched from browser ini is " + browserSubtitleDisplayId);
            }




            sendKeys_Box_SubtitlesDisplay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_SubtitlesDisplay");
            if (sendKeys_Box_SubtitlesDisplay == null)
            {
                FailStep(CL, "Failed to Fetch the Subtitle Display value from test ini ");
            }
            else
            {

                LogComment(CL, "Subtitle Display Value to be set on box is " + sendKeys_Box_SubtitlesDisplay);

            }

            sendKeys_Panorama_SubtitlesDisplay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_SubtitlesDisplay");
            if (sendKeys_Panorama_SubtitlesDisplay == null)
            {
                FailStep(CL, "Failed to Fetch the Subtitle Display value from test ini ");
            }
            else
            {

                LogComment(CL, "Subtitle Display Value to be set on box is " + sendKeys_Panorama_SubtitlesDisplay);

            }

            //Path for apply button
            apply_Path = path1 + "div[3]" + applyPath1;
            //path to set Series Booking value on panorama page
            setSubtitlesLanguagePath = path1 + "div[3]" + path2 + "div[1]" + path3 + "tr[2]" + path4;
            setSubtitleDisplayPath = path1 + "div[3]" + path2 + "div[1]" + path3 + "tr[1]" + path4;
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
                LogComment(CL, "Successfully Logged into web Page and entered cpeid and navigated to settings tab");
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


            //set the Subtitle Display parameter value on panorama page            
            try
            {
                CL.IEX.Wait(10);
                CL.EA.UI.RMS.SetParameterValues(driver, setSubtitlesLanguagePath, apply_Path, divSubtitles, sendKeys_Panorama_SubtitlesLanguage);
                CL.IEX.Wait(5);
                CL.EA.UI.RMS.SetParameterValues(driver, setSubtitleDisplayPath, apply_Path, divSubtitles, sendKeys_Panorama_SubtitlesDisplay);
                CL.IEX.Wait(10);
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameters of subtitle language and subtitle display");

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
            
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            //getting value from panorama page after setting value over box
            driver.Navigate().Refresh();
            CL.IEX.Wait(11);


            try
            {

                CL.IEX.Wait(2);
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));

                driver.FindElement(By.ClassName("v-filterselect-input")).Click();
                CL.IEX.Wait(2);
                driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + browserSubtitleDisplayId + "");

                CL.IEX.Wait(2);
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")));
                driver.FindElement(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")).Click();
                CL.IEX.Wait(1);
               
            }
            catch
            { 
              CL.EA.UI.RMS.EnterCpeId(driver,cpeId);
              CL.IEX.Wait(1);
              CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);
              
              wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
              CL.IEX.Wait(4);
              driver.FindElement(By.ClassName("v-filterselect-input")).Click();

              wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
              CL.IEX.Wait(2);
              driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + browserSubtitleDisplayId + "");

              CL.IEX.Wait(2);
              wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")));
              driver.FindElement(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")).Click();
            }
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = 'Retrieve']")));
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            CL.IEX.Wait(2);
            driver.Navigate().Refresh();
            CL.IEX.Wait(11);
            wait.Until(ExpectedConditions.ElementIsVisible((By.XPath("//input[@type='checkbox']"))));

            IWebElement element = driver.FindElement(By.XPath("//input[@type='checkbox']"));
            if (!element.Selected)
            {
                obtainedSubtitleDisplay = "OFF";
                LogComment(CL, "obtained subtitle display retrieved from panorama is " + obtainedSubtitleDisplay);

            }
            else
            {
                obtainedSubtitleDisplay = "ON";
                LogComment(CL, "Obtained subtitle display retrieved from panorama page is" + obtainedSubtitleDisplay);
            }


            //fetching the value from panorama after setting the value
            CL.IEX.Wait(5);
            obtainedSubtitlesLanguage = CL.EA.UI.RMS.GetParameterValues(driver, browserSubtitlesLanguageId);
            if (obtainedSubtitlesLanguage == null)
            {
                FailStep(CL, "Failed to fetch the Subtitle Language value from panoram page");
            }

            else
            {
                if (obtainedSubtitlesLanguage == "eng")
                {
                    obtainedSubtitlesLanguage = "ENGLISH";
                    LogComment(CL, "Subtitle Language Value Obtained From Panorama page is" + obtainedSubtitlesLanguage);
                }
                else if (obtainedSubtitlesLanguage == "dut")
                {
                    obtainedSubtitlesLanguage = "NEDERLANDS";
                    LogComment(CL, "Subtitle Language Value Obtained From Panorama page is" + obtainedSubtitlesLanguage);
                }
                else if (obtainedSubtitlesLanguage == "cze")
                {
                    obtainedSubtitlesLanguage = "CESKY";
                }

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

            //navigate and get Subtitle Language from box

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Subtitle Language Setting State");

            }
            else
            {
                LogComment(CL, "Succesfully Navigated to Subtitle Language State");

            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitlesLanguage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Subtitle Language Value Expected");
            }
            else
            {
                if (expectedSubtitlesLanguage == "OFF")
                {
                    expectedSubtitlesLanguage = sendKeys_Panorama_SubtitlesLanguage;
					
                   LogComment(CL, "Current Subtitle LANGUAGE  Value is " + expectedSubtitlesLanguage);
                }
                else
                {
				
                    LogComment(CL, "Current Subtitle Language  Value is " + expectedSubtitlesLanguage);
                                       
                }

            }


            //navigate and get the subtitle display value from box
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Subtitle Display Setting State");
            }
            else
            {
                LogComment(CL, "Navigated to subtitle display setting state");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitleDisplay);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Subtitle Display Value Expected");
            }
            else
            {

                if (expectedSubtitleDisplay == "OFF")
                {
                   LogComment(CL, "Current Subtitle Display  Value is " + expectedSubtitleDisplay);
                }
               else
                {
                    expectedSubtitleDisplay = "ON";
                    LogComment(CL, "Current Subtitle Display Is ON");
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
            if (expectedSubtitlesLanguage != obtainedSubtitlesLanguage)
            {
                FailStep(CL, "Both the expected and obtained  values Of Subtitle Language are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Subtitle Language are  equal");
            }
            //comparing the subtitle display obtained and expected values
            if (expectedSubtitleDisplay != obtainedSubtitleDisplay)
            {
                FailStep(CL, "Both the expected and obtained  values Of Subtitle Display are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Subtitle display are  equal");
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

            // set the Subtitle Language value on the box 
            setSubtitleLanguage = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), sendKeys_Box_SubtitlesLanguage, true);
            res = CL.EA.STBSettings.SetSubtitlesPrefs(true, setSubtitleLanguage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Set the Subtitle Language on The box");
            }
            else
            {
                LogComment(CL, "Subtitle Language Set on the box");
            }

            //navigate to state Preferred subtitle Values

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Subtitle SETTING State");

            }
            else
            {
                LogComment(CL, "Navigated to State -- SETTINGS SUBTITLE");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitlesLanguage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Subtitle Language Value Expected");
            }
            else
            {

                LogComment(CL, "Current Subtitle Language is " + expectedSubtitlesLanguage);

            }

            //setting subtitle display value on the box
            if (sendKeys_Box_SubtitlesDisplay == "OFF")
            {
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE OFF");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Set Subtitle Display Value On Box");
                }
                else
                {
                    LogComment(CL, "Subtitle display Value set on the box is" + sendKeys_Box_SubtitlesDisplay);
                }
                CL.IEX.Wait(5);
                //navigate to subtitles display state 
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Subtitle Display Setting State");
                }
                else
                {
                    LogComment(CL, "Navigated to subtitle display setting state");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitleDisplay);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Subtitle Display Value Expected");
                }
                else
                {
                    LogComment(CL, "Current Subtitle Display Value is " + expectedSubtitleDisplay);

                }

            }
            else
            {
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLE DISPLAY ON");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Set Subtitle Display Value On Box");
                }
                else
                {

                    LogComment(CL, "Subtitle display Value set on the box is" + sendKeys_Box_SubtitlesDisplay);
                }
                CL.IEX.Wait(5);
                //navigating to subtitle display state
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Subtitle Display Setting State");
                }
                else
                {
                    LogComment(CL, "Navigated to subtitle display setting state");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitleDisplay);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Subtitle Display Value Expected");
                }
                else
                {
                    expectedSubtitleDisplay = "ON";
                    LogComment(CL, "Current Subtitle Display Value is " + expectedSubtitleDisplay);

                }

            }
           

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            //getting value from panorama page after setting value over box
            driver.Navigate().Refresh();
            CL.IEX.Wait(11);

            try
            {
               
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                CL.IEX.Wait(4);
                driver.FindElement(By.ClassName("v-filterselect-input")).Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                CL.IEX.Wait(2);
                driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + browserSubtitleDisplayId + "");

                CL.IEX.Wait(2);
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")));
                driver.FindElement(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")).Click();
            }
            catch 
            {
                CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                CL.IEX.Wait(3);
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                CL.IEX.Wait(4);
                driver.FindElement(By.ClassName("v-filterselect-input")).Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                CL.IEX.Wait(2);
                driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + browserSubtitleDisplayId + "");

                CL.IEX.Wait(2);
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")));
                driver.FindElement(By.XPath("//span[text() = '" + browserSubtitleDisplayId + "']")).Click();
            }
            CL.IEX.Wait(1);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = 'Retrieve']")));
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            CL.IEX.Wait(2);
            driver.Navigate().Refresh();
            CL.IEX.Wait(11);
            wait.Until(ExpectedConditions.ElementIsVisible((By.XPath("//input[@type='checkbox']"))));

            IWebElement element = driver.FindElement(By.XPath("//input[@type='checkbox']"));
       
            if (!element.Selected)
            {
                obtainedSubtitleDisplay = "OFF";
                LogComment(CL, "obtained subtitle display retrieved from panorama is " + obtainedSubtitleDisplay);

            }
            else
            {
                obtainedSubtitleDisplay = "ON";
                LogComment(CL, "Obtained subtitle display retrieved from panorama page is" + obtainedSubtitleDisplay);
            }
                       

            //fetching the value from panorama after setting the value
           
            CL.IEX.Wait(5);
            obtainedSubtitlesLanguage = CL.EA.UI.RMS.GetParameterValues(driver, browserSubtitlesLanguageId);
            if (obtainedSubtitlesLanguage == null)
            {
                FailStep(CL, "Failed to fetch the Subtitle Language value from panoram page");
            }

            else
            {
                if (obtainedSubtitlesLanguage == "eng")
                {
                    obtainedSubtitlesLanguage = "ENGLISH";
                    LogComment(CL, "Subtitle Language Value Obtained From Panorama page is" + obtainedSubtitlesLanguage);
                }
                else if (obtainedSubtitlesLanguage == "dut")
                {
                    obtainedSubtitlesLanguage = "NEDERLANDS";
                    LogComment(CL, "Subtitle Language Value Obtained From Panorama page is" + obtainedSubtitlesLanguage);
                }
                else if (obtainedSubtitlesLanguage == "cze")
                {
                    obtainedSubtitlesLanguage = "CESKY";
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
            //comparing the expected and obtained values after setting value on the box
            if (obtainedSubtitlesLanguage != expectedSubtitlesLanguage)
            {
                FailStep(CL, "After Setting value on the box Both the values over The box and panorama are not same");

            }
            else
            {
                LogComment(CL, "Both the Values After Setting Value on the box are same");
            }

            //comparing the expected and obtained values after setting value on the box
            if (obtainedSubtitleDisplay != expectedSubtitleDisplay)
            {
                FailStep(CL, "Both the Values Of Subtitle Display values After Setting  on the box are not same");

            }
            else
            {
                LogComment(CL, "Both the Values Of Subtitle Display values After Setting  on the box are same");
            }

            PassStep();
        }
    }

    #endregion Step6

    #endregion Steps
    #region PostExecute
    public override void PostExecute()
    {
        driver.Quit();
    }

    #endregion PostExecute


}

