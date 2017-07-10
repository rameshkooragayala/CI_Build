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


public class RMS_0012 : _Test
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
    //mainvideo output parameters
    static string browserMainVedioOutputId;
    static string obtainedMainVedioOutput;
    static string expectedMainVedioOutput;
    static string sendKeys_Box_MainVedioOutput;
    static string sendKeys_Panorama_MainVedioOutput;

    //hdmi resolution parameters
    static string browserHdmiResolutionId;
    static string obtainedHdmiResolution;
    static string expectedHdmiResolution;
    static string sendKeys_Box_HdmiResolution;
    static string sendkeys_Panorama_Hdmiresolution;
    static string confirmSelectionMilestone;
   
    //Format conversion
    static string browserFormatConversionId;
    static string obtainedFormatConversion;
    static string expectedFormatConversion;
    static string sendKeys_Box_FormatConversion;
  


    static string setMainVedioOutputPath;
    static string setHdResolutionPath;
   
    static int loopCount = 0;
    static string timeStamp = "";
    static int i = 0;
    static int j = 0;
    static int failCount = 0;
    static bool isFail = false;


    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2:Set The Video parameter values and navigate to Respective states on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
        this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
        this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
        this.AddStep(new Step6(), "Step6:Compare the box and panorama  Subtitle Language values");


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


            //main vedio output parameters
            browserMainVedioOutputId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "VIDEO_MAINVIDEOCONNCETOR");
            if (browserMainVedioOutputId == null)
            {
                FailStep(CL, "Failed to Fetch the MainVedioOutout id from Browser Ini");
            }
            else
            {
                LogComment(CL, "Main Video Ouput id fetched from browser ini is " + browserMainVedioOutputId);
            }

            sendKeys_Box_MainVedioOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_MainVideoOutput");
            if (sendKeys_Box_MainVedioOutput == null)
            {
                FailStep(CL, "Failed to Fetch the Main Video Ouput  value from test ini ");
            }
            else
            {

                LogComment(CL, "Main Video Ouput  Value to be set on box is " + sendKeys_Box_MainVedioOutput);

            }


            sendKeys_Panorama_MainVedioOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_MainVideoOutput");
            if (sendKeys_Panorama_MainVedioOutput == null)
            {
                FailStep(CL, "Failed to fetch the Main Video Ouput  Value to set on panorama from  test ini");
            }
            else
            {
                LogComment(CL, "Main Video Ouput  Value to be set on panorama page is" + sendKeys_Panorama_MainVedioOutput);
            }


            //Hdmi resolution parameters
            browserHdmiResolutionId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "VIDEO_HDMIRESOLUTION");
            if (browserHdmiResolutionId == null)
            {
                FailStep(CL, "Failed to Fetch the HdmiResolutionId from browser ini");
            }

            else
            {
                LogComment(CL, "Browser Hdmi Resolution Id from browser ini fetched ");
            }

            sendKeys_Box_HdmiResolution = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_HdmiResolution");
            if (sendKeys_Box_HdmiResolution == null)
            {
                FailStep(CL, "Failed to fetch the hdmi resolution value to be set on box from test ini");
            }
            else
            {
                LogComment(CL, "Hdmi resolution Value to be set on box is fetched from test ini");
            }

            sendkeys_Panorama_Hdmiresolution = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_HdmiResolution");
            if (sendkeys_Panorama_Hdmiresolution == null)
            {
                FailStep(CL, "Failed to fetch the hdmi resolution value to be set on panorama from test ini");
            }
            else
            {
                LogComment(CL, "Hdmi resolution Value to be set on panorama is fetched from test ini"+sendkeys_Panorama_Hdmiresolution);
            }

           
             //Format Conversion parameters
            browserFormatConversionId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "VIDEO_FORMATCONVERSION");
             if (browserFormatConversionId == null)
             {
                 FailStep(CL, "Failed to Fetch the browserFormatConversionId from browser ini");
             }

             else
             {
                 LogComment(CL, "Browser browserFormatConversionId from browser ini fetched ");
             }

             sendKeys_Box_FormatConversion = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_PictureFromat");
             if (sendKeys_Box_FormatConversion == null)
             {
                 FailStep(CL, "Failed to fetch the Picture format value to be set on box from test ini");
             }
             else
             {
                 LogComment(CL, "Picture Format Value to be set on box is fetched from test ini");
             }
            //path to set Main VIdeo Ouput value on panorama page
            setMainVedioOutputPath = path1 + "div[1]" + path2 + "div[1]" + path3 + "tr[1]" + path4;
            setHdResolutionPath = path1 + "div[1]" + path2 + "div[1]" + path3 + "tr[2]" + path4; ;
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
                     
            try
            {

                CL.IEX.Wait(10);
                //set main vedio output
                CL.EA.UI.RMS.SetParameterValues(driver, setMainVedioOutputPath, apply_Path, divVideo, sendKeys_Panorama_MainVedioOutput);
                CL.IEX.Wait(10);
                //set hdresolution value
                CL.EA.UI.RMS.SetParameterValues(driver, setHdResolutionPath, apply_Path, divVideo, sendkeys_Panorama_Hdmiresolution);
                CL.IEX.Wait(10);
               
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameter Main VedioOutPut Value");


            //navigate to Main Vedio Ouput State
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS MAINVIDEOCABLE");

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To navigate Main VedioOutPut state");
            }
            else
            {

                LogComment(CL, "Succesfully navigated to main video output state " + expectedMainVedioOutput);
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMainVedioOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Main VedioOutPut Value Expected");
            }
            else
            {

                LogComment(CL, "Current Main Vedio OutPut Value is " + expectedMainVedioOutput);

            }
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS HDRESOLUTION");

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to HdmiResolution state");
            }
            else
            {

                LogComment(CL, "Successfully navigated to Hdmi Resolution state");

            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiResolution);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get HdmiResolution Value Expected");
            }
            else
            {

                LogComment(CL, "Hdmi Resolution value from box is " + expectedHdmiResolution);

            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS PICTUREFORMAT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Navigate to Picture Format state on epg");

            }
            else
            {
                LogComment(CL, "Succesfully Navigated to Picture Format State");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedFormatConversion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get Picture Format Value Expected");
            }
            else
            {
                string[] temp = expectedFormatConversion.Split();
                expectedFormatConversion = temp[0];
                LogComment(CL, "picture format value from box is " + expectedFormatConversion);

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

            obtainedMainVedioOutput = CL.EA.UI.RMS.GetParameterValues(driver, browserMainVedioOutputId);
            if (obtainedMainVedioOutput == null)
            {
                FailStep(CL, "Failed to fetch the MainOutput Video value from panoram page");
            }

            else
            {

                LogComment(CL, "MainOutput Video Value Obtained From Panorama page is" + obtainedMainVedioOutput);

            }
            CL.IEX.Wait(5);

            
            obtainedHdmiResolution = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiResolutionId);
            if (obtainedHdmiResolution == null)
            {
                FailStep(CL, "Hdmi Resolution fetched from the panorama page is null");
            }
            else
            {
                LogComment(CL, "HDMI Resolution value fetched from panorama page is" + obtainedHdmiResolution);
            }
            CL.IEX.Wait(2);
            obtainedFormatConversion = CL.EA.UI.RMS.GetParameterValues(driver, browserFormatConversionId);
            if (obtainedFormatConversion == null)
            {
                FailStep(CL, "Failed to Get the picture format value from panorama");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Picture Format is " + obtainedFormatConversion);
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
            CL.IEX.Wait(5);
            //comparing the value with the box value after setting value on panorama page.
            if (expectedMainVedioOutput != obtainedMainVedioOutput)
            {
                LogCommentInfo(CL, "Both the expected and obtained  values Of MainVedio Output setting value are not equal");
                failCount++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of MainVedio Output setting value are  equal");
            }

            if (expectedHdmiResolution != obtainedHdmiResolution)
            {
                LogCommentInfo(CL, "Both The Expected and obtained values of HD Resolution setting value are not equal");
                failCount++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both The Expected and Obtained Values of HD Resolution settings values are equal");
            }
            
            switch(expectedFormatConversion)
            {

                case "FULLSCREEN (STRETCH)":
                    {
                        expectedFormatConversion = "FullScreen";
                        if (expectedFormatConversion != obtainedFormatConversion)
                        {
                           LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are not same");

                           failCount++;
                           isFail = true;
                         
                        }
                        else
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are same");
                        }
                        break;
                    }
                case "FULLSCREEN (ZOOM)":
                    {
                        expectedFormatConversion = "Zoom";
                        if (expectedFormatConversion != obtainedFormatConversion)
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are not same");
                            failCount++;
                            isFail = true;
                        }
                        else
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are  same");
                        }
                        break;
                        
                    }
                case "PILLARBOX":
                    {
                        expectedFormatConversion = "Pillarbox";
                        if (expectedFormatConversion != obtainedFormatConversion)
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are not same");
                            failCount++;
                            isFail = true;
                        }
                        else
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are  same");
                        }
                        break;
                    }
                    
             
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

            CL.IEX.Wait(10);
            //navigate and set Main Vedio Cable parameter

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS MAINVIDEOCABLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To navigate Main VedioOutPut state");
            }
            else
            {

                LogComment(CL, "Succesfully navigated to main video output state " + expectedMainVedioOutput);
            }

            CL.IEX.Wait(1);
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMainVedioOutput);

            if (expectedMainVedioOutput == sendKeys_Box_MainVedioOutput)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedMainVedioOutput);
                        loopCount++;
                    }
                    while (expectedMainVedioOutput != sendKeys_Box_MainVedioOutput || loopCount < 3);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter main video ouput to " + expectedMainVedioOutput);

            }

            //getting MainVideo Output from panorama page
            obtainedMainVedioOutput = CL.EA.UI.RMS.GetParameterValues(driver, browserMainVedioOutputId);
            if (obtainedMainVedioOutput == null)
            {
                FailStep(CL, "Failed to fetch the MainOutput Video value from panoram page");
            }

            else
            {

                LogComment(CL, "MainOutput Video Value Obtained From Panorama page is" + obtainedMainVedioOutput);

            }
            //setting HD Resolution value on the box
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS HDRESOLUTION");

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to HD Resolution State");

            }
            else
            {
                LogComment(CL, "Navigated to State -- HDResolution State");
            }
            CL.IEX.Wait(2);
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiResolution);
            if (expectedHdmiResolution == sendKeys_Box_HdmiResolution)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiResolution);
                        loopCount++;
                    }
                    while (expectedHdmiResolution != sendKeys_Box_HdmiResolution || loopCount < 5);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    CL.IEX.Wait(3);
                    CL.IEX.MilestonesEPG.GetEPGInfo("title", out confirmSelectionMilestone);
                    if (confirmSelectionMilestone == "NO")
                    {
                        CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiResolution);
                    }
                    else
                    {
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiResolution);
                    }
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter Hdmiresolution to " + expectedHdmiResolution);
            }
            CL.IEX.Wait(5);
            //getting the hdmi Resolution value from panorama
            obtainedHdmiResolution = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiResolutionId);
            if (obtainedHdmiResolution == null)
            {
                FailStep(CL, "Hdmi Resolution fetched from the panorama page is null");
            }
            else
            {
                LogComment(CL, "HDMI Resolution value fetched from panorama page is" + obtainedHdmiResolution);
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SETTINGS PICTUREFORMAT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Navigate to picture format state");
            }
            else
            {
                LogCommentInfo(CL, "Navigated to picture format state");
            }
            CL.IEX.Wait(2);
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedFormatConversion);
            if (expectedFormatConversion == sendKeys_Box_FormatConversion)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedFormatConversion);
                        loopCount++;
                    }
                    while (expectedFormatConversion != sendKeys_Box_FormatConversion || loopCount < 3);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter Picture Format to " + expectedFormatConversion);
            }
            CL.IEX.Wait(5);

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
            if (obtainedMainVedioOutput != expectedMainVedioOutput)
            {
                LogCommentInfo(CL, "After Setting value on the box Both the Main video Output values over The box and panorama are not same");
                failCount++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both the Main Video Output Values After Setting Value on the box are same");
            }
            //comparing the expected and obtained values of HDMIResolution after setting value on the box
            if (obtainedHdmiResolution != expectedHdmiResolution)
            {
                LogCommentInfo(CL, "After Setting value on the box Both the values over The box and panorama are not same");
                failCount++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both the Values After Setting Value on the box are same");
            }
            switch (expectedFormatConversion)
            {

                case "FULLSCREEN (STRETCH)":
                    {
                        expectedFormatConversion = "FullScreen";
                        if (expectedFormatConversion != obtainedFormatConversion)
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are not same");

                            failCount++;
                            isFail = true;

                        }
                        else
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are  same");
                        }
                        break;
                    }
                case "FULLSCREEN (ZOOM)":
                    {
                        expectedFormatConversion = "Zoom";
                        if (expectedFormatConversion != obtainedFormatConversion)
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are not same");
                            failCount++;
                            isFail = true;
                        }
                        else
                        {
                            LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are  same");
                        }
                        break;
                    }
                case "PILLARBOX":
                        {
                            expectedFormatConversion="Pillarbox";
                            if (expectedFormatConversion != obtainedFormatConversion)
                            {
                                LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are not same");
                                failCount++;
                                isFail = true;
                            }
                            else
                            {
                                LogCommentInfo(CL, "Obtained FormatConversion and Expected Format Conversion Are  same");
                            }
                          break;
                        }
                    

            }
            if (isFail)
            {
                FailStep(CL, "Number of validations failed " + failCount + "...Please Check above Steps for Failure reasons");
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

