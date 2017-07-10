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


public class RMS_0020 : _Test
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
    static string divVideo;
 
    //hdmi color mode parameters
    static string browserHdmiColorModeId;
    static string obtainedHdmiColorMode;
    static string expectedHdmiColorMode;
    static string sendKeys_Box_HdmiColorMode;
    static string sendKeys_Panorama_HdmiColormode;
    private static string defaultTVColorOutput;
    private static string selectedTVColorOutput;
   
    static string setMainVedioOutputPath;
    static string setHdmiColorModePath;
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
        this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2:Set The hdmi Color mode on the box and navigate to Respective states on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Compare the box value and the panorama value ");
        this.AddStep(new Step4(), "Step4:Set The value on panorama And Navigte to parameters tab on the panorama and fetch the Value");
        this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page and compare Both the Values");



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
            divVideo = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Video");
            if (divVideo == null)
            {
                FailStep(CL, "Failed to fetch the Subtitles div from browser ini");
            }
            else
            {
                LogComment(CL, "Video div value fetched from browser ini is" + divVideo);
            }

            //Path for apply button
            apply_Path = path1 + "div[1]" + applyPath1;


            //Hdmi Color Modeparameters
            browserHdmiColorModeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "VIDEO_HDMICOLORMODE");
            if (browserHdmiColorModeId == null)
            {
                FailStep(CL, "Failed to Fetch the HdmiResolutionId from browser ini");
            }

            else
            {
                LogComment(CL, "Browser Hdmi Resolution Id from browser ini fetched is " + browserHdmiColorModeId);
            }

            sendKeys_Box_HdmiColorMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_HdmiColorMode");
            if (sendKeys_Box_HdmiColorMode == null)
            {
                FailStep(CL, "Failed to fetch the hdmi color mode value to be set on box from test ini");
            }
            else
            {
                LogComment(CL, "Hdmi Color mode Value to be set on box is fetched from test ini");
            }

            sendKeys_Panorama_HdmiColormode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_HdmiColorMode");
            if (sendKeys_Panorama_HdmiColormode == null)
            {
                FailStep(CL, "Failed to fetch the hdmi Color Mode value to be set on panorama from test ini");
            }
            else
            {
                LogComment(CL, "Hdmi Color mode Value to be set on panorama is fetched from test ini");
            }
            //Format Conversion parameters
            browserHdmiColorModeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "VIDEO_HDMICOLORMODE");
            if (browserHdmiColorModeId == null)
            {
                FailStep(CL, "Failed to Fetch the browserFormatConversionId from browser ini");
            }

            else
            {
                LogComment(CL, "Browser browserFormatConversionId from browser ini fetched ");
            }

            //path to set Main VIdeo Ouput value on panorama page
            setMainVedioOutputPath = path1 + "div[1]" + path2 + "div[1]" + path3 + "tr[1]" + path4;
            setHdmiColorModePath = path1 + "div[1]" + path2 + "div[1]" + path3 + "tr[6]" + path4;
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
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "Failed to navigate to state TV COLOR OUTPUT");
                res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:VIDEO SETTINGS MAINVIDEOCABLE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to main video cable");
                }
                res = CL.IEX.MilestonesEPG.Navigate("HDMI");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to select HDMI");
                }
                res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to state TV COLOR OUTPUT");
                }
            }
            CL.IEX.Wait(1);
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiColorMode);
            
            if (expectedHdmiColorMode == sendKeys_Box_HdmiColorMode)
            {
			    CL.IEX.Wait(3);
                CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            }
            else
            {
                try
                {
                    do
                    {
                        CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiColorMode);
                        count++;
                    }
                    while (expectedHdmiColorMode != sendKeys_Box_HdmiColorMode || count < 4);
                    CL.IEX.Wait(3);
					CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter main video ouput to " + expectedHdmiColorMode);
            }
            

            bool settingsConfirmationState = CL.EA.UI.Utils.VerifyState("SETTINGS CONFIRMATION STATE", 5);
            if (settingsConfirmationState)
            {
                LogCommentInfo(CL, "settings confirmation state verified sucessfully after few seconds");
            }
            else
            {
                FailStep(CL, res, "Unable to verify the settings confirmation state after few seconds");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select Down");
            }
            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select");
            }
            CL.IEX.Wait(5);

            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV COLOR OUTPUT");
            }

            CL.IEX.Wait(2);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiColorMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info");
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
                LogComment(CL, "Successfully Logged into web Page and entered cpeid and navigated to Parameters tab");
            }
            CL.IEX.Wait(5);
            
            obtainedHdmiColorMode = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiColorModeId);
            if (obtainedHdmiColorMode == null)
            {
                FailStep(CL, "Hdmi Resolution fetched from the panorama page is null");
            }
            else if (obtainedHdmiColorMode == "YCBCR422")
            {
                obtainedHdmiColorMode = "YCbCr-4:2:2";
                LogComment(CL, "HDMI ColorMode Value fetched from panorama page is" + obtainedHdmiColorMode);
            }
            else if (obtainedHdmiColorMode == "YCBCR444")
            {
                obtainedHdmiColorMode = "YCbCr-4:4:4";
                LogCommentInfo(CL, "HDMi Color mode value fetche from panorama page is" + obtainedHdmiColorMode);
            }
            else if (obtainedHdmiColorMode == "Auto")
            {
                obtainedHdmiColorMode = "AUTOMATIC";
                LogCommentInfo(CL, "HDMi Color mode value fetche from panorama page is" + obtainedHdmiColorMode);
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
            //comparing the value with the box value after setting value on Cpe
            if (expectedHdmiColorMode != obtainedHdmiColorMode)
            {
                FailStep(CL, "Both the expected and obtained  values Of Hdmi Color Output setting value are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Hdmi Color Output setting value are  equal");
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
            CL.IEX.Wait(2);
            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }
            CL.IEX.Wait(3);
            //set format conversion value
            CL.EA.UI.RMS.SetParameterValues(driver, setHdmiColorModePath, apply_Path, divVideo, sendKeys_Panorama_HdmiColormode);
            CL.IEX.Wait(5);

            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            CL.IEX.Wait(5);
            obtainedHdmiColorMode = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiColorModeId);
            if (obtainedHdmiColorMode == null)
            {
                FailStep(CL, "Hdmi ClorMode Value fetched from the panorama page is null");
            }
           
            else if (obtainedHdmiColorMode == "YCBCR422")
            {
                obtainedHdmiColorMode = "YCbCr-4:2:2";
                LogCommentInfo(CL, "HDMI ColorMode Value fetched from panorama page is" + obtainedHdmiColorMode);
            }
            else if (obtainedHdmiColorMode == "YCBCR444")
            {
                obtainedHdmiColorMode = "YCbCr-4:4:4";
                LogCommentInfo(CL, "HDMi Color mode value fetche from panorama page is" + obtainedHdmiColorMode);
            }
            else if (obtainedHdmiColorMode == "Auto")
            {
                obtainedHdmiColorMode = "AUTOMATIC";
                LogCommentInfo(CL, "HDMi Color mode value fetche from panorama page is" + obtainedHdmiColorMode);
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
            CL.IEX.Wait(1);
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV COLOR OUTPUT");
            }

            CL.IEX.Wait(2);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiColorMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info");
            }


            //comparing the value with the box value after setting value on panorama page.
            if (expectedHdmiColorMode != obtainedHdmiColorMode)
            {
                FailStep(CL, "Both the expected and obtained  values Of Hdmi Color Output setting value are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of Hdmi Color Output setting value are  equal");
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

    

