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

    public class RMS_024:_Test
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
        static string div_audioPath;
        static string browserHdmiDelayId;
        static string browserHdmiDigitalAudioDolbyId;
        static string browserHdmiAudioOutputId;
        static string setHdmiAudioDelayPath;
        static string setHdmiDigitalAudioDolbyPath;
        static string setHdmiAudioOutputPath;
        static string obtainedHdmiDelay;
        static string expectedHdmiDelay;
        static string obtainedHdmiDigitalAudioDolby;
        static string expectedHdmiDigitalAudioDolby;
        static string obtainedHdmiAudioOutput;
        static string expectedHdmiAudioOutput;
        static string send_Box_HdmiAudioDelay;
        static string send_Panorama_HdmiAudioDelay;
        static string send_Box_HdmiAudioDigitalDolby;
        static string send_Panorama_HdmiAudioDigitalDolby;
        static string send_Box_HdmiAudioOutput;
        static string send_Panorama_HdmiAudioOuput;
        
        static string obtianedSpdifDigitalDolby;
        static string browserSpdifDolbyId;
        static string expectedSpdifDigitalDolby;
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
            this.AddStep(new Step1(), "Step1: Navigate on the box and set the hdmi param values ");
            this.AddStep(new Step2(), "Step2: Login to panorama page and fetch the updated values");
            this.AddStep(new Step3(), "Step3: Compare The both box and panorama values ");
            this.AddStep(new Step4(), "Step4: Setting the value in Panorama ");
            this.AddStep(new Step5(), "Step5: Compare the box and panorama values again");

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
                browserHdmiDelayId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "AUDIO_HDMIDELAY");
                if (browserHdmiDelayId == null)
                {
                    FailStep(CL, "failed to Fetch the browserHdmiDelayId");
                }
                else
                {
                    LogCommentInfo(CL, "Fetched browserHdmiDelayId is" + browserHdmiDelayId);
                }
                browserHdmiDigitalAudioDolbyId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "AUDIO_HDMIDIGITALDOLBAY");
                if (browserHdmiDigitalAudioDolbyId == null)
                {
                    FailStep(CL, "failed to Fetch the browserHdmiDigitalAudioDolbyId");
                }
                else
                {
                    LogCommentInfo(CL, "Fetched browserHdmiDigitalAudioDolbyId is" + browserHdmiDigitalAudioDolbyId);
                }
                browserHdmiAudioOutputId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "AUDIO_HDMIAUDIOOUTPUT");
                if (browserHdmiAudioOutputId == null)
                {
                    FailStep(CL, "failed to Fetch the browserHdmiAudioOutputId");
                }
                else
                {
                    LogCommentInfo(CL, "Fetched browserHdmiAudioOutputId is" + browserHdmiAudioOutputId);
                }
                browserSpdifDolbyId=CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "AUDIO_SPDIF_DOLBY");
                if (browserSpdifDolbyId == null)
                {
                    FailStep(CL, "failed to Fetch the browserSpdifDolbyId");
                }
                else
                {
                    LogCommentInfo(CL, "Fetched browserSpdifDolbyId is" + browserSpdifDolbyId);
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
                //div Value
                div_audioPath = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Audio");
                if (div_audioPath == null)
                {
                    FailStep(CL, "Failed to fetch the Audio div from browser ini");
                }
                else
                {
                    LogComment(CL, "Audio div value fetched from browser ini is" + div_audioPath);
                }
                send_Box_HdmiAudioDelay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Box_AudioDelay");
                if (send_Box_HdmiAudioDelay == null)
                {
                    FailStep(CL, "Failed to get the HdmiAudioDelay value to be set on the box from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained HdmiAudioDelay value to be set on the box from Test Ini " + send_Box_HdmiAudioDelay);
                }
                send_Panorama_HdmiAudioDelay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_panorama_AudioDelay");
                if (send_Panorama_HdmiAudioDelay == null)
                {
                    FailStep(CL, "Failed to get the Hdmi delay value to be set on the panorama from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Hdmi delay value to be set on the panorama from Test Ini " + send_Panorama_HdmiAudioDelay);
                }
                send_Box_HdmiAudioDigitalDolby = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Box_AudioDigitalDolby");
                if (send_Box_HdmiAudioDigitalDolby == null)
                {
                    FailStep(CL, "Failed to get the AudioDigitalDolby value to be set on the box from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained AudioDigitaldolby value to be set on the box from Test Ini " + send_Box_HdmiAudioDigitalDolby);
                }
                send_Panorama_HdmiAudioDigitalDolby = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Panorama_AudioDigitalDolby");
                if (send_Panorama_HdmiAudioDigitalDolby == null)
                {
                    FailStep(CL, "Failed to get the AudioDigitalDolby value to be set on the panorama from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained AudioDigitalDolby value to be set on the panorama from Test Ini " + send_Panorama_HdmiAudioDigitalDolby);
                }
                send_Box_HdmiAudioOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Box_AudioOutput");
                if (send_Box_HdmiAudioOutput == null)
                {
                    FailStep(CL, "Failed to get the Audio Ouput value to be set on the box from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Audio Ouput value to be set on the box from Test Ini " + send_Box_HdmiAudioOutput);
                }
                send_Panorama_HdmiAudioOuput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Panorama_AudioOutput");
                if (send_Panorama_HdmiAudioOuput == null)
                {
                    FailStep(CL, "Failed to Fetch the AudioOutput Value to be set on the panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "The Audio Output value fetched to set on the panorama page is"+send_Panorama_HdmiAudioOuput);
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

                apply_Path = path1 + "div[1]" + applyPath1;
                setHdmiAudioDelayPath = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[2]" + path4;
                setHdmiDigitalAudioDolbyPath = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[7]" + path4;
                setHdmiAudioOutputPath = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[6]" + path4;
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
                //audio delay
                LogCommentInfo(CL, "Navigating To Hdmi Audio Delay And Setting the Value to " + send_Box_HdmiAudioDelay);

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI AUDIO DELAY");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Hdmi Audio Delay state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Hdmi Audio Delay state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiDelay);
                if (expectedHdmiDelay == send_Box_HdmiAudioDelay)
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
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiDelay);
                            loopCount++;
                        }
                        while (expectedHdmiDelay != send_Box_HdmiAudioDelay || loopCount < 20);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, ex.Message);
                    }
                    LogComment(CL, "Successfully set the parameter Hdmi Audio Delay to " + expectedHdmiDelay);
                }
                
                //dolby
                LogCommentInfo(CL, "Navigating To Hdmi Digital Audio Dolby And Setting the Value to " + send_Box_HdmiAudioDigitalDolby);
                CL.IEX.Wait(2);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI DIGITAL DOLBY");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Hdmi Digital Dolby state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Hdmi Digital Dolby state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiDigitalAudioDolby);
                if (expectedHdmiDigitalAudioDolby == send_Box_HdmiAudioDigitalDolby)
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
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiDigitalAudioDolby);
                            loopCount++;
                        }
                        while (expectedHdmiDigitalAudioDolby != send_Box_HdmiAudioDigitalDolby || loopCount < 2);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, ex.Message);
                    }
                    LogComment(CL, "Successfully set the parameter Hdmi Digital Audio Dolby setting to " + expectedHdmiDigitalAudioDolby);

                }
                //hdmi audio output
                LogCommentInfo(CL, "Navigating To Hdmi Digital Audio output And Setting the Value to " + send_Box_HdmiAudioOutput);
                CL.IEX.Wait(2);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI OUTPUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Hdmi Audio Output state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Hdmi Audio Output state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiAudioOutput);
                if (expectedHdmiAudioOutput == send_Box_HdmiAudioOutput)
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
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiAudioOutput);
                            loopCount++;
                        }
                        while (expectedHdmiAudioOutput != send_Box_HdmiAudioOutput || loopCount < 2);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, ex.Message);
                    }
                    LogComment(CL, "Successfully set the parameter Hdmi  Audio Output setting to " + expectedHdmiAudioOutput);
                }
                 CL.IEX.Wait(2);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI DIGITAL DOLBY");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate  Digital Dolby state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Digital Dolby state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSpdifDigitalDolby);
                if (expectedSpdifDigitalDolby == null)
                {
                    FailStep(CL, "unable to set the ExpectedSpdifDigitalDolby Value on the box");
                }
                else
                {
                    LogCommentInfo(CL, "ExpectedSpdifDigitalDolby Value set on the box is"+expectedSpdifDigitalDolby);
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
                obtainedHdmiDelay = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiDelayId);
                if (obtainedHdmiDelay == null)
                {
                    FailStep(CL, "Failed to fetch the Hdmi Audio Delay value from panoram page");
                }
                else
                {
                    obtainedHdmiDelay="+"+obtainedHdmiDelay+" ms.";
                   LogCommentInfo(CL, "Hdmi Audio Delay Value obtained from panorama is" + obtainedHdmiDelay);
                }
                   
               
                obtainedHdmiDigitalAudioDolby = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiDigitalAudioDolbyId);
                if (obtainedHdmiDigitalAudioDolby == null)
                {
                    FailStep(CL, "Failed to fetch the Analog Ouput setting value from panoram page");
                }
                else
                {
                    switch (obtainedHdmiDigitalAudioDolby)
                    {
                        case "Enabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "ENABLE";
                                LogCommentInfo(CL, "Obtained Digital Audio Dolby Output Value from panorama is" + obtainedHdmiDigitalAudioDolby);
                                break;
                            }
                        case "Disabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "DISABLED";
                                LogCommentInfo(CL, "Obtained Digital Audio Dolby Value from panorama is" + obtainedHdmiDigitalAudioDolby);
                                break;
                            }
                    }

                }
                obtainedHdmiAudioOutput = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiAudioOutputId);
                if (obtainedHdmiAudioOutput == null)
                {
                    FailStep(CL, "Failed to fetch the Hdmi Audio Output setting value from panoram page");
                }
                else
                {
                    switch (obtainedHdmiAudioOutput)
                    {
                        case "Enabled":
                            {
                                obtainedHdmiAudioOutput = "ENABLE";
                                LogCommentInfo(CL, "Obtained Hdmi Audio Output Value from panorama is" + obtainedHdmiAudioOutput);
                                break;
                            }
                        case "Disabled":
                            {
                                obtainedHdmiAudioOutput = "DISABLED";
                                LogCommentInfo(CL, "Obtained Hdmi Audio Output Value from panorama is" + obtainedHdmiAudioOutput);
                                break;
                            }
                    }

                }

                obtianedSpdifDigitalDolby = CL.EA.UI.RMS.GetParameterValues(driver, browserSpdifDolbyId);
                if (obtianedSpdifDigitalDolby == null)
                {
                    FailStep(CL, "Failed to fetch the SPDIF DIGITAL DOLBY setting value from panoram page");
                }
                else
                {
                    switch (obtainedHdmiDigitalAudioDolby)
                    {
                        case "Enabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "ENABLE";
                                LogCommentInfo(CL, "Obtained SPDIF Digital Audio Dolby Output Value from panorama is" + obtianedSpdifDigitalDolby);
                                break;
                            }
                        case "Disabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "DISABLED";
                                LogCommentInfo(CL, "Obtained SPDIF Digital Audio Dolby Value from panorama is" + obtianedSpdifDigitalDolby);
                                break;
                            }
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
                if (expectedHdmiDelay != obtainedHdmiDelay)
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of Hdmi Audio Delay values are not same after setting value on the box");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the OBtained and expected values of Hdmi Audio Delay Values are same after setting value on the box");
                }
                if (expectedHdmiDigitalAudioDolby != obtainedHdmiDigitalAudioDolby)
                {
                    LogCommentInfo(CL, "Both the Obtained and Expected values of Hdmi Audio Dolby values are not same after setting value on the box");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of Hdmi Auido Dolby Values are same after setting value on the box");
                }
                if (expectedHdmiAudioOutput != obtainedHdmiAudioOutput)
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of Hdmi Audio Output values are not same after setting value on the box");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of HDMI Audio Output values are same after setting value on the box");
                }
                if (expectedSpdifDigitalDolby != obtianedSpdifDigitalDolby)
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of SPDIF DIGITAL DOLBY values are not same after setting value on the box");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of SPDIF DIGITAL DOLBY values are same after setting value on the box");
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
                LogCommentInfo(CL, "Setting the hdmi parameters on the panorama page and comparing with the updated value of box");
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                CL.EA.UI.RMS.SetParameterValues(driver, setHdmiAudioDelayPath, apply_Path, div_audioPath, send_Panorama_HdmiAudioDelay);
                CL.IEX.Wait(3);
                CL.EA.UI.RMS.SetParameterValues(driver, setHdmiDigitalAudioDolbyPath, apply_Path, div_audioPath, send_Panorama_HdmiAudioDigitalDolby);
                CL.IEX.Wait(3);
                CL.EA.UI.RMS.SetParameterValues(driver, setHdmiAudioOutputPath, apply_Path, div_audioPath, send_Panorama_HdmiAudioOuput);

                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                obtainedHdmiDelay = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiDelayId);
                if (obtainedHdmiDelay == null)
                {
                    FailStep(CL, "Failed to fetch the Hdmi Audio Delay value from panoram page");
                }
                else
                {
                    obtainedHdmiDelay = "+" + obtainedHdmiDelay + " ms.";
                    LogCommentInfo(CL, "Hdmi Audio Delay Value obtained from panorama is" + obtainedHdmiDelay);
                }


                obtainedHdmiDigitalAudioDolby = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiDigitalAudioDolbyId);
                if (obtainedHdmiDigitalAudioDolby == null)
                {
                    FailStep(CL, "Failed to fetch the Analog Ouput setting value from panoram page");
                }
                else
                {
                    switch (obtainedHdmiDigitalAudioDolby)
                    {
                        case "Enabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "ENABLE";
                                LogCommentInfo(CL, "Obtained Digital Audio Dolby Output Value from panorama is" + obtainedHdmiDigitalAudioDolby);
                                break;
                            }
                        case "Disabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "DISABLED";
                                LogCommentInfo(CL, "Obtained Digital Audio Delay Value from panorama is" + obtainedHdmiDigitalAudioDolby);
                                break;
                            }
                    }

                }
                obtainedHdmiAudioOutput = CL.EA.UI.RMS.GetParameterValues(driver, browserHdmiAudioOutputId);
                if (obtainedHdmiAudioOutput == null)
                {
                    FailStep(CL, "Failed to fetch the Hdmi Audio Output setting value from panoram page");
                }
                else
                {
                    switch (obtainedHdmiAudioOutput)
                    {
                        case "Enabled":
                            {
                                obtainedHdmiAudioOutput = "ENABLE";
                                LogCommentInfo(CL, "Obtained Hdmi Audio Output Value from panorama is" + obtainedHdmiAudioOutput);
                                break;
                            }
                        case "Disabled":
                            {
                                obtainedHdmiAudioOutput = "DISABLED";
                                LogCommentInfo(CL, "Obtained Hdmi Audio Output Value from panorama is" + obtainedHdmiAudioOutput);
                                break;
                            }
                    }

                }

                obtianedSpdifDigitalDolby = CL.EA.UI.RMS.GetParameterValues(driver, browserSpdifDolbyId);
                if (obtianedSpdifDigitalDolby == null)
                {
                    FailStep(CL, "Failed to fetch the SPDIF DIGITAL DOLBY setting value from panoram page");
                }
                else
                {
                    switch (obtainedHdmiDigitalAudioDolby)
                    {
                        case "Enabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "ENABLE";
                                LogCommentInfo(CL, "Obtained SPDIF Digital Audio Dolby Output Value from panorama is" + obtianedSpdifDigitalDolby);
                                break;
                            }
                        case "Disabled":
                            {
                                obtainedHdmiDigitalAudioDolby = "DISABLED";
                                LogCommentInfo(CL, "Obtained SPDIF Digital Audio Dolby Value from panorama is" + obtianedSpdifDigitalDolby);
                                break;
                            }
                    }

                }

                CL.IEX.Wait(5);
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
                LogCommentInfo(CL, "Fetching the updated value from box and comparing with the panorama values");
                //hdmi delay
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI AUDIO DELAY");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Hdmi Audio Delay state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Hdmi Audio Delay state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiDelay);
                LogCommentInfo(CL, "Navigating and fetching the HDMi Digital Dolby value");
                //hdmi digital dolby
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI DIGITAL DOLBY");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Hdmi Digital Dolby state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Hdmi Digital Dolby state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiDigitalAudioDolby);
                CL.IEX.Wait(2);
                LogCommentInfo(CL,"Navigating and fetching the HDMi output value");
                //hdmi output
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI OUTPUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Hdmi Audio Output state");
                }
                else
                {

                    LogComment(CL, "Successfully navigated to Hdmi Audio Output state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedHdmiAudioOutput);
                CL.IEX.Wait(2);
                LogCommentInfo(CL, "Navigating and fetching the  Digital Dolby value");
                //spdif dolby
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:HDMI DIGITAL DOLBY");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate  Digital Dolby state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Digital Dolby state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSpdifDigitalDolby);
                if (expectedSpdifDigitalDolby == null)
                {
                    FailStep(CL, "Failed to Fetcht the Expected digital dolby value from box");
                }
                else
                {
                    LogCommentInfo(CL, "Expected Digital Dolby value from box is"+expectedSpdifDigitalDolby);
                }
                //comparing with panorama values
                if (expectedHdmiDelay != obtainedHdmiDelay)
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of Hdmi Audio Delay values are not same after setting value on the panorama");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the OBtained and expected values of Hdmi Audio Delay Values are same after setting value on the panorama");
                }
                if (expectedHdmiDigitalAudioDolby != obtainedHdmiDigitalAudioDolby)
                {
                    LogCommentInfo(CL, "Both the Obtained and Expected values of Hdmi Audio Dolby values are not same after setting value on the panorama ");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of Hdmi Auido Dolby Values are same after setting value on the panorama");
                }
                if (expectedHdmiAudioOutput != obtainedHdmiAudioOutput)
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of Hdmi Audio Output values are not same after setting value on the panorama");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of HDMI Audio Output values are same After setting value on panorama");
                }
                if (expectedSpdifDigitalDolby != obtianedSpdifDigitalDolby)
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of SPDIF DIGITAL DOLBY values are not same after setting value on the box");
                    isFail = true;
                    failCount++;
                }
                else
                {
                    LogCommentInfo(CL, "Both the Obtained and expected values of SPDIF DIGITAL DOLBY values are same after setting value on the box");
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


