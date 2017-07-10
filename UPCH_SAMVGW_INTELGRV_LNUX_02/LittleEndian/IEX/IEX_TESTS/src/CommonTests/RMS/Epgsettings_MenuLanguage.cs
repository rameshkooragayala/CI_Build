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


public class Epgsettings_menuLanguage : _Test
{
    [ThreadStatic]
    static FirefoxDriver driver;
    private static _Platform CL;
    static string browserParameterTabId;
    static string browserSettingTabId;
    static string cpeId;
    static string browserEpgMenuLanguageId;
    static string obtainedEpgMenuLanguage;
    static string expectedMenuLanguage;
    static string path1;
    static string path2;
    static string path3;
    static string path4;
    static string applyPath1;
    static string apply_Path;
    static string epgLanguagePath;
    static string sendKeys_Box_EpgLanguage;
    static string sendKeys_Panorama_EpgLanguage;
    static string timeStamp = "";
    static int count = 0;
    static string divEpg;
    static string sendKeys_Language;



    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get Parameter browser ids and values From ini File & Sync");
        this.AddStep(new Step1(), "Step 1:LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB");
        this.AddStep(new Step2(), "Step2:Set The menu language parameter and navigate to menu language state on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
        this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
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
            //Fetch the SettingsTabId from browser ini

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

            applyPath1 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_Apply_Path");
            if (applyPath1 == null)
            {
                FailStep(CL, "Failed to fetch the Apply settings button path");
            }
            else
            {
                LogComment(CL, "ApplyPath1 fetched successfully");
            }

            //epg Div Value
            divEpg = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Epg");
            if (divEpg == null)
            {
                FailStep(CL, "Failed to fetch the epg div from browser ini");
            }
            else
            {
                LogComment(CL, "epg div value fetched from browser ini is" + divEpg);
            }
            //browser epg menu id

            browserEpgMenuLanguageId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "EPG_SETTINGS_PARAMS", "EPG_MENULANGUAGE");
            if (browserEpgMenuLanguageId == null)
            {
                FailStep(CL, "Failed to Fetch the MenuLanguageId from Browser Ini");
            }
            else
            {
                LogComment(CL, "MenuLanguageId fetched from browser ini is " + browserEpgMenuLanguageId);
            }


            //The language Value to be send to set on Box
            sendKeys_Box_EpgLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_Language");
            if (sendKeys_Box_EpgLanguage == null)
            {
                FailStep(CL, "Failed to Fetch the value from test ini ");
            }
            else
            {
                LogComment(CL, "Language Value fetched from test ini is" + sendKeys_Box_EpgLanguage);
            }
            //The Language Value To be send to set on the panorama page
            sendKeys_Panorama_EpgLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_Language");
            if (sendKeys_Panorama_EpgLanguage == null)
            {
                FailStep(CL, "Failed to Fetch the value from test ini ");
            }
            else
            {
                LogComment(CL, "Language Value fetched from test ini is" + sendKeys_Panorama_EpgLanguage);
            }


            //Path for apply button
            apply_Path = path1 + "div[2]" + applyPath1;
            //path to set epg value on panorama page
            epgLanguagePath = path1 + "div[3]" + path2 + "div[2]" + path3 + "tr[1]" + path4;
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
			CL.IEX.Wait(5);
            //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
            res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserSettingTabId);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Unable To Login Enter CpeId and Select Settings Tab On the WebPage");
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

            //set the epg language parameter value on panorama page    
            try
            {
                CL.IEX.Wait(10);
                CL.EA.UI.RMS.SetParameterValues(driver, epgLanguagePath, apply_Path, divEpg, sendKeys_Panorama_EpgLanguage);
               
            }

            catch (Exception ex)
            {
                FailStep(CL, "Failed to Set Value for Language" + ex.Message);
            }

            LogComment(CL, "successfully set the parameter");

            CL.IEX.Wait(5);

            //navigate to menu language
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MENU LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to menu language");

            }
            else
            {
                LogComment(CL, "Navigated to menu language");
            }
            //gettings value from box               
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMenuLanguage);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Get Current Menu language");
            }
            else
            {
                LogComment(CL, "Current Menu Language " + expectedMenuLanguage);
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

			CL.IEX.Wait(5);
            //fetching the value from panorama after setting the value
            obtainedEpgMenuLanguage = CL.EA.UI.RMS.GetParameterValues(driver, browserEpgMenuLanguageId);
            if (obtainedEpgMenuLanguage == null)
            {
                FailStep(CL, "Failed to fetch the menu language from panoram page");
            }
            else if (obtainedEpgMenuLanguage == "eng")
            {
                obtainedEpgMenuLanguage = "ENGLISH";
                LogComment(CL, "Obtained menu language from panorama page is" + obtainedEpgMenuLanguage);
            }
            else if (obtainedEpgMenuLanguage == "dut")
            {
                obtainedEpgMenuLanguage = "Nederlands";

                LogComment(CL, "Obtained menu language from panorama page is" + obtainedEpgMenuLanguage);
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
            if (expectedMenuLanguage != obtainedEpgMenuLanguage)
            {
                FailStep(CL, "Both the expected and obtained menulanguage values are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained menulanguage values are  equal");
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
            //set EPG language value on the box

			CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MENU LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Menu Language State");

            }
            else
            {
                LogComment(CL, "Navigated to State -- Menu Language");
            }
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMenuLanguage);
            if (expectedMenuLanguage == sendKeys_Box_EpgLanguage)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMenuLanguage);
                        count++;
                    }
                    while (expectedMenuLanguage != sendKeys_Box_EpgLanguage || count < 4);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }


                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter main video ouput to " + expectedMenuLanguage);
            }
            if (sendKeys_Box_EpgLanguage == "ENGLISH")
            {
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MENU LANGUAGE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Menu Language State After setting value on the box");

                }
                else
                {
                    LogComment(CL, "Navigated to State -- Menu Language");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMenuLanguage);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Epg Language Value from Box After Setting Value on The Box");
                }
                else
                {
                    LogComment(CL, "Expected EpgMenu Language After setting value on the box is" + expectedMenuLanguage);
                }
            }
            else
            {
                expectedMenuLanguage = sendKeys_Box_EpgLanguage;
                LogComment(CL, "Expected EpgMenu Language After setting value on the box is" + expectedMenuLanguage);
            }

            //Getting Menu Language from the panorama page after setting value on the box

            obtainedEpgMenuLanguage = CL.EA.UI.RMS.GetParameterValues(driver, browserEpgMenuLanguageId);
            if (obtainedEpgMenuLanguage == null)
            {
                FailStep(CL, "Failed to fetch the menu language from panoram page");
            }
            else if (obtainedEpgMenuLanguage == "eng")
            {
                obtainedEpgMenuLanguage = "ENGLISH";
                LogComment(CL, "Obtained menu language from panorama page is" + obtainedEpgMenuLanguage);
            }
            else if (obtainedEpgMenuLanguage == "dut")
            {
                obtainedEpgMenuLanguage = "Nederlands";

                LogComment(CL, "Obtained menu language from panorama page is" + obtainedEpgMenuLanguage);
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

            if (obtainedEpgMenuLanguage != expectedMenuLanguage)
            {
                FailStep(CL, "After Setting value on the box Both the values Of Menu Language over The box and panorama are not same");
            }
            else
            {
                LogComment(CL, "After Setting value on the box Both the values of  Menu Language over The box and panorama are  same");
            }
            PassStep();
        }
    }

    #endregion Step6
    #endregion steps
    #region PostExecute

    public override void PostExecute()
    {
        driver.Close();
    }

    #endregion PostExecute
}
