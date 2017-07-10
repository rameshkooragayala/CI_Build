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
using System;
using IEX.Tests.Engine;
using System.Collections.Generic;
using OpenQA.Selenium.IE;


public class RMS_0009_SetSeriesBooking : _Test
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
    static string setSeriesRecordingPath;
    static string divSeriesRecording;
    static string browserSeriesRecordingId;
    static string obtainedSeriesRecording;
    static string expectedSeriesRecording;
    static string sendKeys_Box_SeriesRecording;
    static string sendKeys_Panorama_SeriesRecording;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2:Set The Series Recording parameter and navigate to menu language state on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
        this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
        this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
        this.AddStep(new Step6(), "Step6:Compare the box and panorama  Series Recording values");


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

            //div Series Recording Value
            divSeriesRecording = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_SeriesRecord");
            if (divSeriesRecording == null)
            {
                FailStep(CL, "Failed to fetch the series recording div from browser ini");
            }
            else
            {
                LogComment(CL, "epg div value fetched from browser ini is" + divSeriesRecording);
            }

            //browserSeriesBookingId
            browserSeriesRecordingId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "PVR_SETTINGS_PARAMS", "PVR_SERIESBOOKING");
            if (browserSeriesRecordingId == null)
            {
                FailStep(CL, "Failed to Fetch the Series recording id from Browser Ini");
            }
            else
            {
                LogComment(CL, "Series recording id fetched from browser ini is " + browserSeriesRecordingId);
            }


            //Series Recording value to be  set on box 
            sendKeys_Box_SeriesRecording = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_SeriesRecording");
            if (sendKeys_Box_SeriesRecording == null)
            {
                FailStep(CL, "Failed to Fetch the Series Recording value from test ini ");
            }
            else
            {

                LogComment(CL, "Series Recording Value to be set on box is " + sendKeys_Box_SeriesRecording);

            }

            //Series Recording value to be set on the panorama page
            sendKeys_Panorama_SeriesRecording = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_SeriesRecording");
            if (sendKeys_Panorama_SeriesRecording == null)
            {
                FailStep(CL, "Failed to fetch the Series Recording Value to set on panorama from  test ini");
            }
            else
            {
                LogComment(CL, "Series Recording Value to be set on panorama page is" + sendKeys_Panorama_SeriesRecording);
            }
            //Path for apply button
            apply_Path = path1 + "div[3]" + applyPath1;
            //path to set Series Booking value on panorama page
            setSeriesRecordingPath = path1 + "div[2]" + path2 + "div[3]" + path3 + "tr" + path4;
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
            
            CL.IEX.Wait(5);
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
            //set the Series Recording parameter value on panorama page            
            try
            {
                CL.IEX.Wait(20);
                CL.EA.UI.RMS.SetParameterValues(driver, setSeriesRecordingPath, apply_Path, divSeriesRecording, sendKeys_Panorama_SeriesRecording);
                CL.IEX.Wait(5);
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameter");

            CL.IEX.Wait(5);

            //navigate to Series Recording State 

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Series Recording State");

            }
            else
            {
                LogComment(CL, "Navigated to Series Recording State");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSeriesRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Series Recording Value Expected");
            }
            else
            {

                LogComment(CL, "Current Series Recording  Value is " + expectedSeriesRecording);

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
            CL.IEX.Wait(5);
            obtainedSeriesRecording = CL.EA.UI.RMS.GetParameterValues(driver, browserSeriesRecordingId);
            if (obtainedSeriesRecording == null)
            {
                FailStep(CL, "Failed to fetch the Series Recording value from panoram page");
            }

            else
            {
                if (obtainedSeriesRecording == "Single Channel")
                {
                    obtainedSeriesRecording = "FROM SINGLE CHANNEL";
                    LogComment(CL, "Series Recording Value Obtained From Panorama page is" + obtainedSeriesRecording);
                }
                else if (obtainedSeriesRecording == "Across Channels")
                {
                    obtainedSeriesRecording = "FROM ALL CHANNELS";
                    LogComment(CL, "Series Recording Value Obtained From Panorama page is" + obtainedSeriesRecording);
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
            if (expectedSeriesRecording != obtainedSeriesRecording)
            {
                FailStep(CL, "Both the expected and obtained  values Of Series Recording are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Series Recording are  equal");
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
          
            // set the SERIES RECORDING value on the box 
            if (sendKeys_Box_SeriesRecording == "Record from single channel")
            {
                CL.IEX.Wait(5);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING SINGLECHANNEL");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to set Series Recording Value to FROM SINGLE CHANNEL");
                }
                else
                {
                    LogComment(CL, "Successfully set the Value of Series Recording on box to FROM SINGLE CHANNEL");
                }
            }
            else if (sendKeys_Box_SeriesRecording == "Record from all channels")
            {
                CL.IEX.Wait(5);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING ALLCHANNELS");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to set Series Recording Value to FROM ALL CHANNELS");
                }
                else
                {
                    LogComment(CL, "Successfully set the Value of Series Recording on box to FROM ALL CHANNELS");
                }
            }
            else
            {
                FailStep(CL, "No Such Value Is There To Set On Series Recording State");
            }



            //navigate to state Series Recording
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SERIES RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to Series Recording State");

            }
            else
            {
                LogComment(CL, "Navigated to State -- Series Recording");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSeriesRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Series recording Value Expected");
            }
            else
            {

                LogComment(CL, "Current Series Recording Mode is " + expectedSeriesRecording);

            }



            LogCommentInfo(CL, "perform retrive and fetch the updted the value after setting value on the box");
            //getting value from panorama page after setting value over box
            driver.Navigate().Refresh();
            CL.IEX.Wait(11);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = 'Retrieve']")));
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            CL.IEX.Wait(3);
            driver.Navigate().Refresh();
            CL.IEX.Wait(11);

            LogCommentInfo(CL,"Getting text from //input[@type='text'])[2] ");

            obtainedSeriesRecording = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();
            obtainedSeriesRecording = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();
            //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(90));
            if (obtainedSeriesRecording == null)
            {
                FailStep(CL, "Failed to fetch the  Series Recording value from panorama page after setting value on the box");
            }
            else
            {
                if (obtainedSeriesRecording == "Single Channel")
                {
                    obtainedSeriesRecording = "FROM SINGLE CHANNEL";
                    LogComment(CL, "Obtained Series Recording value from panorama page is" + obtainedSeriesRecording);
                }
                else if (obtainedSeriesRecording == "Across Channels")
                {
                    obtainedSeriesRecording = "FROM ALL CHANNELS";
                    LogComment(CL, "Obtained Series Recording value from panorama page is" + obtainedSeriesRecording);
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
            if (obtainedSeriesRecording != expectedSeriesRecording)
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
