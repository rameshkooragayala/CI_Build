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


public class RMS_0015 : _Test
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
    static string setStandbyPath;
    static string divStandby;
    static string obtainedStandbySetting;
    static string expectedStandbySetting;
    static string Sendkey_Box_AutoStanby;
    static string Sendkey_panorama_AutoStandby;
    static string browserStandbySettingId;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2: Setting the standby setting value on box and compaing the same with panorama  ");
        this.AddStep(new Step3(), "Step3:Setting the value in panorama and Comparing the same with cpe value ");
        this.AddStep(new Step4(),"Step4:Comparing Both the Box and panorama values after setting value on the panorama");

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

            //div standby Display Value
            divStandby = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Standby");
            if (divStandby == null)
            {
                FailStep(CL, "Failed to fetch the Subtitles div from browser ini");
            }
            else
            {
                LogComment(CL, "epg div value fetched from browser ini is" + divStandby);
            }

            //Stand by value to be set on box 
            Sendkey_Box_AutoStanby = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_AutoStanby");
            if (Sendkey_Box_AutoStanby == null)
            {
                FailStep(CL, "Failed to Fetch the Standby state value from test ini ");
            }
            else
            {

                LogComment(CL, "Standby Value to be set on box is " + Sendkey_Box_AutoStanby);

            }

            //Standby setting value to be set on the panorama page
            Sendkey_panorama_AutoStandby = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_AutoStandby");
            if (Sendkey_panorama_AutoStandby == null)
            {
                FailStep(CL, "Failed to fetch the standby Value to set on panorama from  test ini");
            }
            else
            {
                LogComment(CL, "Standby Value to be set on panorama page is" + Sendkey_panorama_AutoStandby);
            }

            //Standby setting parameters

            browserStandbySettingId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "AUTOSTANDBY_SETTINGS");

            if (browserStandbySettingId == null)
            {
                FailStep(CL, "Failed to Fetch the Standby setting id from Browser Ini");
            }
            else
            {
                LogComment(CL, "Standby setting Display id fetched from browser ini is " + browserStandbySettingId);
            }


            //Path for apply button
            apply_Path = path1 + "div[3]" + applyPath1;
            //path to set Series Booking value on panorama page
            setStandbyPath = path1 + "div[3]" + path2 + "div[3]" + path3 + "tr[1]" + path4;
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
            CL.IEX.Wait(3);
            // set the Standby setting value on the box 

            res = CL.EA.STBSettings.SetAutoStandBy(Sendkey_Box_AutoStanby);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + Sendkey_Box_AutoStanby);
            }


            //navigate to state standby setting

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Auto Standby SETTING State");

            }
            else
            {
                LogComment(CL, "Navigated to State -- Auto Standby setting");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedStandbySetting);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Standby setting value Expected");
            }
            else
            {

                LogComment(CL, "Current Standby setting is " + expectedStandbySetting);

            }


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


            //fetching the value from panorama after setting the value on box
            CL.IEX.Wait(5);

            obtainedStandbySetting = CL.EA.UI.RMS.GetParameterValues(driver, browserStandbySettingId);
            if (obtainedStandbySetting == null)
            {
                FailStep(CL, "Failed to fetch the Standby setting value from panoram page");
            }
            else if (obtainedStandbySetting == "Always On")
            {
                obtainedStandbySetting = "AUTOMATIC";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "Off")
            {
                obtainedStandbySetting = "OFF";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "Night Time")
            {
                obtainedStandbySetting = "AT NIGHT";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else
            {

                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);

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


            //Check if the value is correctly set in Panorama page
            //Comparing the CPE and Panorama value

            if (expectedStandbySetting != obtainedStandbySetting)
            {
                FailStep(CL, "Both the expected and obtained  values Of Standby setting values are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Standby setting are  equal");
            }
            CL.IEX.Wait(4);
          



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

            //select the settings tab on the panorama
            try
            {
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
                CL.EA.UI.RMS.SetParameterValues(driver, setStandbyPath, apply_Path, divStandby, Sendkey_panorama_AutoStandby);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameters of standby settings on panorama");

            CL.IEX.Wait(5);
            //select the paramaters tab on the panorama
            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }
            CL.IEX.Wait(5);
            //check the value set on the panorama by fetching the value from panorama

            //obtainedStandbySetting = CL.EA.UI.RMS.GetParameterValues(driver, browserStandbySettingId);
            driver.Navigate().Refresh();
            CL.IEX.Wait(7);
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            driver.Navigate().Refresh();
            CL.IEX.Wait(7);
            obtainedStandbySetting = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();

            if (obtainedStandbySetting == null)
            {
                FailStep(CL, "Failed to fetch the Standby setting value from panoram page");
            }
            else if (obtainedStandbySetting == "Always On")
            {
                obtainedStandbySetting = "AUTOMATIC";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "Off")
            {
                obtainedStandbySetting = "OFF";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "Night Time")
            {
                obtainedStandbySetting = "AT NIGHT";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else
            {

                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);

            }

            // Check the standby setting value set in CPE  

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Standby Setting State");

            }
            else
            {
                LogComment(CL, "Navigated to Standby setting State");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedStandbySetting);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Standby setting Value Expected");
            }
            else
            {

                LogComment(CL, "Current standby setting  Value is " + expectedStandbySetting);

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
      //Comparing the CPE and Panorama values

            if (obtainedStandbySetting != expectedStandbySetting)
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
#endregion Step4
    #endregion Steps
    #region PostExecute
    public override void PostExecute()
    {
        driver.Close();
    }

    #endregion PostExecute


}

