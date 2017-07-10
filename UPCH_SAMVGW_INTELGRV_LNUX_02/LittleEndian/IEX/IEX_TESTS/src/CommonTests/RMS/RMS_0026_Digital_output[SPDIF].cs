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


class RMS_0026_SPDIF_Digital_Output : _Test
{
    [ThreadStatic]
    static FirefoxDriver driver;
    private static _Platform CL;
    static string browserParameterTabId;
    static string browserSPDIFaudiooutputId;
    static string browserSettingTabId;
    static string cpeId;
    static string path1;
    static string path2;
    static string path3;
    static string path4;
    static string applyPath1;
    static string apply_Path;
    static string setSPDIFaudiooutput;
    static string divAudio;
    static string expectedSPDIFaudiooutput;
    static string obtainedSPDIFaudiooutput;
    static string sendkeys_Panorama_SPDIFaudiooutput;
    static string sendkeys_Box_SPDIFaudiooutput;
    static string timeStamp = "";
    static int loopCount;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Getting the parameter ID and values from ini File");
        this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2:Set The SPDIF output and navigate to on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
        this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
        this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
        this.AddStep(new Step6(), "Step6:Compare the box and panorama  Subtitle Display values");
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

            //Fetch the settingId from Browser ini
            browserParameterTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
            if (browserParameterTabId == null)
            {
                FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
            }
            else
            {
                LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

            }

            //Fetching the path values from browser ini
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

            divAudio = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_Audio");
            if (divAudio == null)
            {
                FailStep(CL, "Failed to fetch the Audio div from browser ini");
            }
            else
            {
                LogComment(CL, "epg div value fetched from browser ini is" + divAudio);
            }

            browserSPDIFaudiooutputId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "SPDIFAUDIOOUTPUT");

            if (browserSPDIFaudiooutputId == null)
            {
                FailStep(CL, "Failed to Fetch the SPDIF audio output id from Browser Ini");
            }
            else
            {
                LogComment(CL, "SPDIF audio output id fetched from browser ini is " + browserSPDIFaudiooutputId);
            }


            sendkeys_Box_SPDIFaudiooutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_SPDIFaudiooutput");
            if (sendkeys_Box_SPDIFaudiooutput == null)
            {
                FailStep(CL, "Failed to Fetch the SPDIF audio output value from test ini ");
            }
            else
            {

                LogComment(CL, "SPDIF audio output Value to be set on box is " + sendkeys_Box_SPDIFaudiooutput);

            }

            sendkeys_Panorama_SPDIFaudiooutput = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Panorama_SPDIFaudiooutput");
            if (sendkeys_Panorama_SPDIFaudiooutput == null)
            {
                FailStep(CL, "Failed to Fetch the SPDIF audio output from test ini ");
            }
            else
            {

                LogComment(CL, "SPDIF audio output Value to be set on Panorama is " + sendkeys_Panorama_SPDIFaudiooutput);

            }

            //Path for apply button
            apply_Path = path1 + "div[1]" + applyPath1;

            //path to set Series Booking value on panorama page
            setSPDIFaudiooutput = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[8]" + path4;
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
                CL.IEX.Wait(5);

                CL.EA.UI.RMS.SetParameterValues(driver, setSPDIFaudiooutput, apply_Path, divAudio, sendkeys_Panorama_SPDIFaudiooutput);
                CL.IEX.Wait(10);
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameter SPDIF audio output");

            CL.IEX.Wait(5);

            //navigate to Subtitle Display State 

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to SPDIF audio outout State");
            }
            else
            {
                LogComment(CL, "Navigated to SPDIF audio output state");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSPDIFaudiooutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get SPDIF output Value Expected");
            }
            else
            {
                LogComment(CL, "Current SPDIF audio output Value is " + expectedSPDIFaudiooutput);

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
            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            obtainedSPDIFaudiooutput = CL.EA.UI.RMS.GetParameterValues(driver, browserSPDIFaudiooutputId);

            if (obtainedSPDIFaudiooutput == null)
            {
                LogComment(CL, "Failed to fetch the SPDIF output value from Panorama");

            }
            else 
            {
                switch (obtainedSPDIFaudiooutput)
                {
                    case "Enabled":
                        {
                            obtainedSPDIFaudiooutput = "ENABLE";
                            LogCommentInfo(CL, "Obtained SPDIF DigitaL Output Value from panorama is" + obtainedSPDIFaudiooutput);
                            break;
                        }
                    case "Disabled":
                        {
                            obtainedSPDIFaudiooutput = "DISABLED";
                            LogCommentInfo(CL, "Obtained SPDIF DigitaL Output Value from panorama is" + obtainedSPDIFaudiooutput);
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
            //comparing the value with the box value after setting value on panorama page.

            if (expectedSPDIFaudiooutput != obtainedSPDIFaudiooutput)
            {
                FailStep(CL, "Both the Expected and Obtained Values of SPDIF output are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of SPDIF output value are  equal");
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


            //set value of subtitles display value on box

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Set SPDIF output Value On Box");
            }
            else
            {
                LogComment(CL, "SPDIF output Value set on the box is" + sendkeys_Box_SPDIFaudiooutput);
            }
            CL.IEX.Wait(5);

            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSPDIFaudiooutput);
            if (expectedSPDIFaudiooutput == sendkeys_Box_SPDIFaudiooutput)
            {
                CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            }
            else
            {
                try
                {
                    do
                    {
                        CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSPDIFaudiooutput);
                        loopCount++;
                    }
                    while (expectedSPDIFaudiooutput != sendkeys_Box_SPDIFaudiooutput || loopCount < 2);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter SPDIF audio Delay to " + expectedSPDIFaudiooutput);
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

            obtainedSPDIFaudiooutput = CL.EA.UI.RMS.GetParameterValues(driver, browserSPDIFaudiooutputId);

            if (obtainedSPDIFaudiooutput == null)
            {
                LogComment(CL, "Failed to fetch the SPDIF output value from Panorama");

            }
            else
            {
                switch (obtainedSPDIFaudiooutput)
                {
                    case "Enabled":
                        {
                            obtainedSPDIFaudiooutput = "ENABLE";
                            LogCommentInfo(CL, "Obtained SPDIF DigitaL Output Value from panorama is" + obtainedSPDIFaudiooutput);
                            break;
                        }
                    case "Disabled":
                        {
                            obtainedSPDIFaudiooutput = "DISABLED";
                            LogCommentInfo(CL, "Obtained SPDIF DigitaL Output Value from panorama is" + obtainedSPDIFaudiooutput);
                            break;
                        }
                }
                
            }

            //comparing the expected and obtained values after setting value on the box

            if (obtainedSPDIFaudiooutput != expectedSPDIFaudiooutput)
            {
                FailStep(CL, "After Setting value on the box Both the values Of SPDIF output value over The box and panorama are not same");
            }
            else
            {
                LogComment(CL, "After Setting value on the box Both the values Of SPDIF ouput value over The box and panorama are same");
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


