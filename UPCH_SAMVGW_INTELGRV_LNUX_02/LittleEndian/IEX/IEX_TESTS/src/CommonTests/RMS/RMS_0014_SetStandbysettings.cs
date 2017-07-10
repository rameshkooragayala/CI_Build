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


public class RMS_0014 : _Test
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
    static string sendKey_Box_Standby;
    static string sendKey_Panorama_Standby;
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
        this.AddStep(new Step2(), "Step2: Setting the standby setting value from Panorama and compaing the same with CPE  ");
        this.AddStep(new Step3(), "Step3:Setting the value in CPE and Comparing the same with Panorama page ");
        
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
            sendKey_Box_Standby = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_Stanby");
            if (sendKey_Box_Standby == null)
            {
                FailStep(CL, "Failed to Fetch the Standby state value from test ini ");
            }
            else
            {

                LogComment(CL, "Standby Value to be set on box is " + sendKey_Box_Standby);

            }

            //Standby setting value to be set on the panorama page
            sendKey_Panorama_Standby = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_Standby");
            if (sendKey_Panorama_Standby == null)
            {
                FailStep(CL, "Failed to fetch the standby Value to set on panorama from  test ini");
            }
            else
            {
                LogComment(CL, "Standby Value to be set on panorama page is" + sendKey_Panorama_Standby);
             }

            //Standby setting parameters

            browserStandbySettingId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "STANDBY_SETTINGS");

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
            //path to set Standby value on panorama page
            setStandbyPath = path1 + "div[3]" + path2 + "div[3]" + path3 + "tr[4]" + path4;
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
                CL.IEX.Wait(20);
                CL.EA.UI.RMS.SetParameterValues(driver, setStandbyPath, apply_Path, divStandby, sendKey_Panorama_Standby);
                
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameters of standby settings");

            CL.IEX.Wait(5);

//Check if the value is correctly set in Panorama page

            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }


            //fetching the value from panorama after setting the value
            CL.IEX.Wait(5);

            obtainedStandbySetting = CL.EA.UI.RMS.GetParameterValues(driver, browserStandbySettingId);
            if (obtainedStandbySetting == null)
            {
                FailStep(CL, "Failed to fetch the Standby setting value from panoram page");
            }
            else if (obtainedStandbySetting == "suspend cm on")
            {
                obtainedStandbySetting = "MEDIUM";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "active")
            {
                obtainedStandbySetting = "HIGH";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "suspend")
            {
                obtainedStandbySetting = "LOW";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "off")
            {
                obtainedStandbySetting = "ECO MODE";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }
            else
            {

                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
                            
            }

// Check the standby setting value set in CPE  

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
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

//Comparing the CPE and Panorama value

            if (expectedStandbySetting != obtainedStandbySetting)
            {
                FailStep(CL, "Both the expected and obtained  values Of Standby setting values are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Standby setting are  equal");
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

            // set the Standby setting value on the box 

            res = CL.EA.STBSettings.SetPowerMode(sendKey_Box_Standby);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + sendKey_Box_Standby);
            }
            else
            {
                LogCommentInfo(CL, "Succesfully set the power mode option on the box");
            }

            //navigate to state standby setting

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Standby SETTING State");

            }
            else
            {
                LogComment(CL, "Navigated to State -- Standby setting");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedStandbySetting);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Standby setting value Expected");
            }
            else
            {

                LogComment(CL, "Current Standby setting is " + expectedStandbySetting);

            }
                      
                
            //fetching the value from panorama after setting the value

           
            obtainedStandbySetting = CL.EA.UI.RMS.GetParameterValues(driver, browserStandbySettingId);
            if (obtainedStandbySetting == null)
            {
                FailStep(CL, "Failed to fetch the Standby setting value from panoram page");
            }
            else if (obtainedStandbySetting == "suspend cm on")
            {
                obtainedStandbySetting = "MEDIUM";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }
            else if (obtainedStandbySetting == "active")
            {
                obtainedStandbySetting = "HIGH";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }
                
            else if (obtainedStandbySetting == "suspend")
            {
                obtainedStandbySetting = "LOW";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }

            else if (obtainedStandbySetting == "off")
            {
                obtainedStandbySetting = "ECO MODE";
                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
            }
            else
            {

                LogComment(CL, "Standby setting Value Obtained From Panorama page is" + obtainedStandbySetting);
                
            }

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
    #endregion Step3
    
    #endregion Steps
    #region PostExecute
    public override void PostExecute()
    {
        driver.Close();
    }

    #endregion PostExecute


}

