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

    public class RMS_0023:_Test
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
        static string obtainedAudioLanguage;
        static string expectedAudioLanguage;
        static string sendKey_BoxAudioLanguage;
        static string sendKey_PanoramaAudioLanguage;
        static string browserAudioLanguageId;
        static string setAudioLanguagePath;

        static string obtainedAnalogOutput;
        static string expectedAnalogOutput;
        static string sendKey_Box_AnalogOuput;
        static string sendKey_Panorama_AnalogOutput;
        static string browserAnalogOutputId;
        static string setAnalogOuputPath;
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
            this.AddStep(new Step1(), "Step1: Setting the audio param values on the box and fetching the expected values");
            this.AddStep(new Step2(), "Step2: Login into the panorama page and fetch the updated value and compare with expected values");
            this.AddStep(new Step3(), "Step3:Setting the value again on panorama page ");
            this.AddStep(new Step4(), "Step4:Navigating to the box and comparing the values with panorama page values ");

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
                browserAudioLanguageId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "AUDIO_AUDIOLANGUAGE");
                if (browserAudioLanguageId == null)
                {
                    FailStep(CL, "failed to Fetch the BrowserAudioLanguageId");
                }
                else
                {
                    LogCommentInfo(CL, "Fetched BrowserAudioLanguageId is" + browserAudioLanguageId);
                }
                browserAnalogOutputId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "AV_SETTINGS_PARAMS", "AUDIO_ANALOGOUTPUT");
                if (browserAnalogOutputId == null)
                {
                    FailStep(CL, "failed to Fetch the browserAnalogOutputId");
                }
                else
                {
                    LogCommentInfo(CL, "Fetched browserAnalogOutputId is" + browserAnalogOutputId);
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
                div_audioPath = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Audio");
                if (div_audioPath == null)
                {
                    FailStep(CL, "Failed to fetch the Audio div from browser ini");
                }
                else
                {
                    LogComment(CL, "Audio div value fetched from browser ini is" + div_audioPath);
                }
                sendKey_BoxAudioLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Box_AudioLanguage");
                if (sendKey_BoxAudioLanguage == null)
                {
                    FailStep(CL, "Failed to get the Audio Language value to be set on the box from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Audio Language value to be set on the box from Test Ini "+sendKey_BoxAudioLanguage);
                }
                sendKey_PanoramaAudioLanguage = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS","SendKey_panorama_AudioLanguage");
                if (sendKey_PanoramaAudioLanguage == null)
                {
                    FailStep(CL, "Failed to get the Audio Language value to be set on the panorama from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Audio Language value to be set on the panorama from Test Ini " + sendKey_PanoramaAudioLanguage);
                }
                sendKey_Box_AnalogOuput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Box_AnalogOuput");
                if (sendKey_Box_AnalogOuput == null)
                {
                    FailStep(CL, "Failed to get the Analog Ouput value to be set on the box from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Analog Ouput value to be set on the box from Test Ini " + sendKey_Box_AnalogOuput);
                }
                sendKey_Panorama_AnalogOutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_panorama_AnalogOutput");
                if (sendKey_Panorama_AnalogOutput == null)
                {
                    FailStep(CL, "Failed to get the Analog Ouput value to be set on the panorama from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Analog Ouput value to be set on the panorama from Test Ini " + sendKey_Panorama_AnalogOutput);
                }
                apply_Path = path1 + "div[1]" + applyPath1;
                setAudioLanguagePath = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[4]" + path4;
                setAnalogOuputPath = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[5]" + path4;
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

                
                LogCommentInfo(CL, "Navigating To Audio Language And Setting the Value to ");
               
                try
                {
                    CL.IEX.Wait(3);
                    CL.EA.UI.Settings.SetAudioLanguage(sendKey_BoxAudioLanguage);
                    CL.IEX.Wait(3);
                }
                catch (Exception ex)
                {
                    FailStep(CL,"Failed To Set The Audio Language Value On The Box because"+ex.Message);
                }
                CL.IEX.Wait(3);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUDIO LANGUAGE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Audio Language state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Audio language state ");
                }
                //CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedAudioLanguage);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Audio Language setting Value Expected");
                }
                else
                {

                    LogComment(CL, "Current Audio Language setting  Value is " + expectedAudioLanguage);

                }
                LogCommentInfo(CL, "Setting Analog Audio Output Value on the box");
				CL.IEX.Wait(5);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ANALOG");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Analog Output state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Analog Output state ");
                }
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedAnalogOutput);
                if (expectedAnalogOutput == sendKey_Box_AnalogOuput)
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
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedAnalogOutput);
                            loopCount++;
                        }
                        while (expectedAnalogOutput != sendKey_Box_AnalogOuput || loopCount < 2);
                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, ex.Message);
                    }
                    LogComment(CL, "Successfully set the parameter Analog ouput to " + expectedAnalogOutput);
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
                obtainedAudioLanguage = CL.EA.UI.RMS.GetParameterValues(driver, browserAudioLanguageId);
                if (obtainedAudioLanguage == null)
                {
                    FailStep(CL, "Failed to fetch the Audio Language value from panoram page");
                }
                else
                {
                switch (obtainedAudioLanguage)
                {
                    case "eng":
                        obtainedAudioLanguage = "ENGLISH";
                            break;
                    case "dut":
                            obtainedAudioLanguage = "NEDERLANDS";
                            break;
                     
                }
                LogCommentInfo(CL, "Audio Language Value obtained from panorama is"+obtainedAudioLanguage);
                }
                obtainedAnalogOutput =CL.EA.UI.RMS.GetParameterValues(driver, browserAnalogOutputId);
                if(obtainedAnalogOutput==null)
                {
                  FailStep(CL, "Failed to fetch the Analog Ouput setting value from panoram page");
                }
                else
                {
                    switch(obtainedAnalogOutput)
                    {
                        case "On":
                            {
                                obtainedAnalogOutput = "ENABLE";
                                LogCommentInfo(CL, "Obtained Analog Output Value from panorama is"+obtainedAnalogOutput);
                            break;
                            }
                        case "Off":
                            {
                                obtainedAnalogOutput = "DISABLED";
                                LogCommentInfo(CL, "Obtained Analog Output Value from panorama is" + obtainedAnalogOutput);
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
                if (expectedAudioLanguage != obtainedAudioLanguage)
                {
                    FailStep(CL, "Both the expected and obtained values of audio language are not euqal");
                }
                else
                {
                    LogCommentInfo(CL, "Both The obtained and expected values of audio language are equal");
                }
                if (obtainedAnalogOutput != expectedAnalogOutput)
                {
                    FailStep(CL, "Both the Expected and Obtained Values of Audio Analog Output Values are not equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both the Expected and Obtained Values of Audio Analog Output Values are  equal");
                }
                CL.IEX.Wait(2);
                LogCommentInfo(CL, "Setting value on the panorama page and checkin on the box");
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                CL.EA.UI.RMS.SetParameterValues(driver, setAudioLanguagePath, apply_Path, div_audioPath, sendKey_PanoramaAudioLanguage);
                CL.IEX.Wait(3);
                CL.EA.UI.RMS.SetParameterValues(driver, setAnalogOuputPath, apply_Path, div_audioPath, sendKey_Panorama_AnalogOutput);
                CL.IEX.Wait(2);

                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                CL.IEX.Wait(5);
                obtainedAudioLanguage = CL.EA.UI.RMS.GetParameterValues(driver, browserAudioLanguageId);
                if (obtainedAudioLanguage == null)
                {
                    FailStep(CL, "Obtained Audio language from panorama page is null");
                }
                else
                {
                    switch (obtainedAudioLanguage)
                    {
                        case "eng":
                            obtainedAudioLanguage = "ENGLISH";
                            break;
                        case "dut":
                            obtainedAudioLanguage = "NEDERLANDS";
                            break;

                    }
                    LogCommentInfo(CL, "Audio Language Value obtained from panorama is" + obtainedAudioLanguage);
                }
                CL.IEX.Wait(2);
                obtainedAnalogOutput = CL.EA.UI.RMS.GetParameterValues(driver, browserAnalogOutputId);
                if (obtainedAnalogOutput == null)
                {
                    FailStep(CL, "Failed to fetch the Analog Ouput setting value from panoram page");
                }
                else
                {
                    switch (obtainedAnalogOutput)
                    {
                        case "On":
                            {
                                obtainedAnalogOutput = "ENABLE";
                                LogCommentInfo(CL, "Obtained Analog Output Value from panorama is" + obtainedAnalogOutput);
                                break;
                            }
                        case "Off":
                            {
                                obtainedAnalogOutput = "DISABLED";
                                LogCommentInfo(CL, "Obtained Analog Output Value from panorama is" + obtainedAnalogOutput);
                                break;
                            }
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
                LogCommentInfo(CL, "Fetching the audio output and audio language values from the box");
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUDIO LANGUAGE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Audio Language state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Audio language state ");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedAudioLanguage);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Get Audio Language setting Value Expected");
                }
                else
                {

                    LogComment(CL, "Current Audio Language setting  Value is " + expectedAudioLanguage);

                }
				CL.IEX.Wait(5);
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ANALOG");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate Analog Output state");
                }
                else
                {

                    LogComment(CL, "Succesfully navigated to Analog Output state ");
                }
                CL.IEX.Wait(2);
                res=CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedAnalogOutput);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Fetch the Expected Analog Ouput Value from Box");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Analog Output Value from Box is"+expectedAnalogOutput);
                }

                LogCommentInfo(CL,"Comaparing the values of panorama with box values after setting value on the panorama page");
                if (expectedAudioLanguage != obtainedAudioLanguage)
                {
                    FailStep(CL, "Both the expected and obtained values of audio language are not euqal");
                }
                else
                {
                    LogCommentInfo(CL, "Both The obtained and expected values of audio language are equal");
                }
                CL.IEX.Wait(2);
                if (obtainedAnalogOutput != expectedAnalogOutput)
                {
                    FailStep(CL, "Both the Expected and Obtained Values of Audio Analog Output Values are not equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both the Expected and Obtained Values of Audio Analog Output Values are  equal");
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

