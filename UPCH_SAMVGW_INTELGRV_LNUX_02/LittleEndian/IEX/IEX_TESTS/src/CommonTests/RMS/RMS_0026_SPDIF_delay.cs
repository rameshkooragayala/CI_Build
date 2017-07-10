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


class RMS_0026_SPDIFaudio_delay : _Test
{
    [ThreadStatic]
    static FirefoxDriver driver;
    private static _Platform CL;
    static string browserParameterTabId;
    static string browserSPDIFdelayId;
    static string browserSPDIFoutputId;
    static string browserSPDIFId;
    static string browserSettingTabId;
    static string cpeId;
    static string path1;
    static string path2;
    static string path3;
    static string path4;
    static string applyPath1;
    static string apply_Path;
    static string setSPDIFdelayPath;
    static string divAudio;
    static string expectedSPDIFaudiodelay;
    static string obtainedSPDIFaudiodelay;
    static string sendkeys_Box_SPDIFaudiodelay;
    static string sendkeys_Panorama_SPDIFaudiodelay;
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
        this.AddStep(new Step2(), "Step2:Set The SPDIF delay value and navigate to on the box And fetch the Value ");
        this.AddStep(new Step3(), "Step3:Navigte to parameters tab on the panorama and fetch the Value ");
        this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
        this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");
        this.AddStep(new Step6(), "Step6:Compare the box and panorama value");
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

            browserSPDIFdelayId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "SPDIFAUDIODELAY");

            if (browserSPDIFdelayId == null)
            {
                FailStep(CL, "Failed to Fetch the SPDIF delay id from Browser Ini");
            }
            else
            {
                LogComment(CL, "SPDIF delay id fetched from browser ini is " + browserSPDIFdelayId);
            }


            sendkeys_Box_SPDIFaudiodelay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Box_SPDIFAudioDelay");
            if (sendkeys_Box_SPDIFaudiodelay == null)
            {
                FailStep(CL, "Failed to Fetch the SPDIF audio delay value from test ini ");
            }
            else
            {

                LogComment(CL, "SPDIF audio delay Value to be set on box is " + sendkeys_Box_SPDIFaudiodelay);

            }

            sendkeys_Panorama_SPDIFaudiodelay = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_Panorama_SPDIFAudioDelay");
            if (sendkeys_Panorama_SPDIFaudiodelay == null)
            {
                FailStep(CL, "Failed to Fetch the SPDIF audio delay from test ini ");
            }
            else
            {

                LogComment(CL, "SPDIF audio delay Value to be set on Panorama is " + sendkeys_Panorama_SPDIFaudiodelay);

            }

            //Path for apply button
            apply_Path = path1 + "div[1]" + applyPath1;

            //path to set Series Booking value on panorama page
            setSPDIFdelayPath = path1 + "div[1]" + path2 + "div[2]" + path3 + "tr[1]" + path4;
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
                CL.IEX.Wait(25);

                CL.EA.UI.RMS.SetParameterValues(driver, setSPDIFdelayPath, apply_Path, divAudio, sendkeys_Panorama_SPDIFaudiodelay);
                CL.IEX.Wait(10);
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            LogComment(CL, "successfully set the parameter SPDIF audio delay");

            CL.IEX.Wait(5);

            //navigate to S//PDIF AUDIO DELAY State 
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF AUDIO DELAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to SPDIF audio delay State");
            }
            else
            {
                LogComment(CL, "Navigated to SPDIF audio delay state");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSPDIFaudiodelay);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get SPDIF delay Value Expected");
            }
            else
            {
                LogComment(CL, "Current SPDIF delay Value is " + expectedSPDIFaudiodelay);

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
            obtainedSPDIFaudiodelay = CL.EA.UI.RMS.GetParameterValues(driver, browserSPDIFdelayId);

            if (obtainedSPDIFaudiodelay == null)
            {
                LogComment(CL, "Failed to fetch the SPDIF delay value from Panorama");

            }
            else
            {
                obtainedSPDIFaudiodelay = "+" + obtainedSPDIFaudiodelay + " ms.";
                LogCommentImportant(CL, "obtained SPDIF delay value from panorama is " + obtainedSPDIFaudiodelay);
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

            if (expectedSPDIFaudiodelay != obtainedSPDIFaudiodelay)
            {
                FailStep(CL, "Both the Expected and Obtained Values of SPDIF delay are not equal");
            }
            else
            {
                LogComment(CL, "Both the expected and obtained  values Of SPDIF delay value are  equal");
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


            //set value of S//PDIF AUDIO DELAY value on box

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:S//PDIF AUDIO DELAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Set SPDIF delay Value On Box");
            }
            else
            {
                LogComment(CL, "SPDIF delay Value set on the box is" + sendkeys_Box_SPDIFaudiodelay);
            }
            CL.IEX.Wait(5);

            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSPDIFaudiodelay);
            if (expectedSPDIFaudiodelay == sendkeys_Box_SPDIFaudiodelay)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSPDIFaudiodelay);
                        loopCount++;
                    }
                    while (expectedSPDIFaudiodelay != sendkeys_Box_SPDIFaudiodelay || loopCount < 21);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the SPDIF audio delay to " + expectedSPDIFaudiodelay);
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

            obtainedSPDIFaudiodelay = CL.EA.UI.RMS.GetParameterValues(driver, browserSPDIFdelayId);

            if (obtainedSPDIFaudiodelay == null)
            {
                LogComment(CL, "Failed to fetch the SPDIF delay value from Panorama");

            }
            else
            {
                obtainedSPDIFaudiodelay = "+" + obtainedSPDIFaudiodelay + " ms.";
                LogCommentImportant(CL, "obtained SPDIF delay value from panorama is " + obtainedSPDIFaudiodelay);
            }

            //comparing the expected and obtained values after setting value on the box

            if (obtainedSPDIFaudiodelay != expectedSPDIFaudiodelay)
            {
                FailStep(CL, "After Setting value on the box Both the values Of SPDIF delay value over The box and panorama are not same");
            }
            else
            {
                LogComment(CL, "After Setting value on the box Both the values Of SPDIF delay value over The box and panorama are same");
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


