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

public class RMS_TrickModes_GET_SET:_Test
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
    static string div_Tricmode;
    static string browserSkipForwordPath;
    static string browserSkipBackwordPath;
    static string setSkipForword;
    static string setSkipBackword;
    static string send_Box_SkipForword;
    static string send_Box_SkipBackword;
    static string send_Panorama_Skipforword;
    static string send_Panorama_SkipBackground;
    static string obtainedSkipForword;
    static string obtainedSkipBackword;
    static string ExpectedSkipForword;
    static string ExpectedSkipBackword;


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
        this.AddStep(new Step1(), "Step1: Navigate on the box and set the TrickMode param values ");
        this.AddStep(new Step2(), "Step2: Login to panorama page and fetch the updated values");
        this.AddStep(new Step3(), "Step3: Compare The both box and panorama values ");
        this.AddStep(new Step4(), "Step4: Setting the trickmode values in Panorama ");
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
            //Fetch the settingstabid from Browser ini
            LogCommentInfo(CL, "fetching settingstabID from browser ini file");
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
            LogCommentInfo(CL, "fetching parameterstabID from browser ini file");
            browserParameterTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
            if (browserParameterTabId == null)
            {
                FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
            }
            else
            {
                LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

            }
            browserSkipForwordPath = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "Skip_Forword");
            if (browserSkipForwordPath == null)
            {
                FailStep(CL, "failed to Fetch the browserSkipForwordPath");
            }
            else
            {
                LogCommentInfo(CL, "Fetched browserSkipForwordPath is" + browserSkipForwordPath);
            }
            browserSkipBackwordPath = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "Skip_Backword");
            if (browserSkipBackwordPath == null)
            {
                FailStep(CL, "failed to Fetch the browserSkipBackwordPath");
            }
            else
            {
                LogCommentInfo(CL, "Fetched browserSkipBackwordPath is" + browserSkipBackwordPath);
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
            div_Tricmode = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_General");
            if (div_Tricmode == null)
            {
                FailStep(CL, "Failed to fetch the Trickmode div from browser ini");
            }
            else
            {
                LogComment(CL, "Trickmode div value fetched from browser ini is" + div_Tricmode);
            }
            send_Box_SkipForword = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Send_Box_SkipForword");
            if (send_Box_SkipForword == null)
            {
                FailStep(CL, "Failed to get the Skipforword value to be set on the box from Test Ini");
            }
            else
            {
                LogCommentInfo(CL, "Obtained skipforword value to be set on the box from Test Ini " + send_Box_SkipForword);
            }
            send_Box_SkipBackword = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Send_Box_SkipBackword");
            if (send_Box_SkipBackword == null)
            {
                FailStep(CL, "Failed to get the SkipBackword value to be set on the Box from Test Ini");
            }
            else
            {
                LogCommentInfo(CL, "Obtained SkipBackword value to be set on the Box from Test Ini " + send_Box_SkipBackword);
            }
            send_Panorama_Skipforword = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Send_Panorama_Skipforword");
            if (send_Panorama_Skipforword == null)
            {
                FailStep(CL, "Failed to get the SkipForword value to be set on the panorama from Test Ini");
            }
            else
            {
                LogCommentInfo(CL, "Obtained AudioDigitaldolby value to be set on the Panorama from Test Ini " + send_Panorama_Skipforword);
            }
            send_Panorama_SkipBackground = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "send_Panorama_SkipBackground");
            if (send_Panorama_SkipBackground == null)
            {
                FailStep(CL, "Failed to get the skipBackword value to be set on the panorama from Test Ini");
            }
            else
            {
                LogCommentInfo(CL, "Obtained SkipBackword value to be set on the panorama from Test Ini " + send_Panorama_SkipBackground);
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

            apply_Path = path1 + "div[3]" + applyPath1;
            setSkipForword = path1 + "div[3]" + path2 + "div[4]" + path3 + "tr[4]" + path4;
            setSkipBackword = path1 + "div[3]" + path2 + "div[4]" + path3 + "tr[5]" + path4;
            
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
            LogCommentInfo(CL, "Navigating To SkipForword state And Setting the Value to " + send_Box_SkipForword);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP FORWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To navigate SkipForword state");
            }
            else
            {

                LogComment(CL, "Succesfully navigated to SkipForword state ");
            }
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipForword);
            if (ExpectedSkipForword == send_Box_SkipForword)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipForword);
                        loopCount++;
                    }
                    while (ExpectedSkipForword != send_Box_SkipForword || loopCount < 5);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter SkipForword to " + ExpectedSkipForword);
            }

            //dolby
            LogCommentInfo(CL, "Navigating To SkipBackword state And Setting the Value to " + send_Box_SkipBackword);
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP BACKWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To navigate SkipBackword state");
            }
            else
            {

                LogComment(CL, "Succesfully navigated to SkipBackword state ");
            }
            CL.IEX.Wait(2);
            CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipBackword);
            if (ExpectedSkipBackword == send_Box_SkipBackword)
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
                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipBackword);
                        loopCount++;
                    }
                    while (ExpectedSkipBackword != send_Box_SkipBackword || loopCount < 5);
                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogComment(CL, "Successfully set the parameter SkipBackward to " + ExpectedSkipForword);
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
            obtainedSkipForword = CL.EA.UI.RMS.GetParameterValues(driver, browserSkipForwordPath);
            if (obtainedSkipForword == null)
            {
                FailStep(CL, "Failed to fetch the SkipForword value from panoram page");
            }
            else if(obtainedSkipForword=="30000" || obtainedSkipForword=="10000")
            {
                obtainedSkipForword =  Convert.ToInt32(obtainedSkipForword)/1000 + " sec";
                LogCommentInfo(CL, "SkipForword Value obtained from panorama is" + obtainedSkipForword);
            }
            else
            {
                obtainedSkipForword = Convert.ToInt32(obtainedSkipForword) / 60000 + " min";
                LogCommentInfo(CL, "Skipforword Value obtained from panorama is" + obtainedSkipForword);
            }

            obtainedSkipBackword = CL.EA.UI.RMS.GetParameterValues(driver, browserSkipBackwordPath);
            if (obtainedSkipBackword == null)
            {
                FailStep(CL, "Failed to fetch the Skipbackword value from panoram page");
            }
            else if (obtainedSkipBackword == "30000" || obtainedSkipBackword == "10000")
            {
                obtainedSkipBackword = Convert.ToInt32(obtainedSkipBackword) / 1000 + " sec";
                LogCommentInfo(CL, "SkipBackword Value obtained from panorama is" + obtainedSkipBackword );
            }
            else
            {
                obtainedSkipForword = Convert.ToInt32(obtainedSkipBackword) / 60000 + " min";
                LogCommentInfo(CL, "Skipbackword Value obtained from panorama is" + obtainedSkipBackword);
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
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP FORWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to SkipForword State");

            }
            else
            {
                LogComment(CL, "Navigated to SkipForword State");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipForword);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get skipforword Value Expected");
            }
            else
            {

                LogComment(CL, "Expected SkipForword Value is " + ExpectedSkipForword);

            }
            if (ExpectedSkipForword != obtainedSkipForword)
            {
                LogCommentInfo(CL, "Both the Obtained and expected values of SkipForword are not same after setting value on the box");
                isFail = true;
                failCount++;
            }
            else
            {
                LogCommentInfo(CL, "Both the OBtained and expected values of SkipForword are same after setting value on the box");
            }

            LogCommentInfo(CL, "Navigating to Skipbackword state to get the updated expected value from box");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP BACKWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to SkipBackward State");

            }
            else
            {
                LogComment(CL, "Navigated to SkipBackward State");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipBackword);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get skipBackward Value Expected");
            }
            else
            {

                LogComment(CL, "Expected SkipBackward Value is " + ExpectedSkipBackword);

            }
            if (ExpectedSkipBackword != obtainedSkipBackword)
            {
                LogCommentInfo(CL, "Both the Obtained and expected values of SkipBackward are not same after setting value on the box");
                isFail = true;
                failCount++;
            }
            else
            {
                LogCommentInfo(CL, "Both the OBtained and expected values of SkipBackward are same after setting value on the box");
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
            LogCommentInfo(CL, "Setting the SkipForward and backward  parameters on the panorama page and comparing with the updated value of box");
            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserSettingTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }

            CL.EA.UI.RMS.SetParameterValues(driver, setSkipForword, apply_Path, div_Tricmode, send_Panorama_Skipforword);
            CL.IEX.Wait(3);
            CL.EA.UI.RMS.SetParameterValues(driver, setSkipBackword, apply_Path, div_Tricmode, send_Panorama_SkipBackground);
            CL.IEX.Wait(3);
           try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }
           CL.IEX.Wait(2);
           obtainedSkipForword = CL.EA.UI.RMS.GetParameterValues(driver, browserSkipForwordPath);
           if (obtainedSkipForword == null)
           {
               FailStep(CL, "Failed to fetch the SkipForword value from panoram page");
           }
           else if (obtainedSkipForword == "30000" || obtainedSkipForword == "10000")
           {
               obtainedSkipForword = Convert.ToInt32(obtainedSkipForword) /1000 + " sec";
               LogCommentInfo(CL, "SkipForword Value obtained from panorama is" + obtainedSkipForword);
           }
           else
           {
               obtainedSkipForword = Convert.ToInt32(obtainedSkipForword) / 60000 + " min";
               LogCommentInfo(CL, "Skipforword Value obtained from panorama is" + obtainedSkipForword);
           }

           obtainedSkipBackword = CL.EA.UI.RMS.GetParameterValues(driver, browserSkipBackwordPath);
           if (obtainedSkipBackword == null)
           {
               FailStep(CL, "Failed to fetch the Skipbackword value from panoram page");
           }
           else if (obtainedSkipBackword == "30000" || obtainedSkipBackword == "10000")
           {
               obtainedSkipBackword = Convert.ToInt32(obtainedSkipBackword) / 1000 + " sec";
               LogCommentInfo(CL, "SkipBackword Value obtained from panorama is" + obtainedSkipBackword);
           }
           else
           {
               obtainedSkipForword = Convert.ToInt32(obtainedSkipBackword) / 60000 + " min";
               LogCommentInfo(CL, "Skipbackword Value obtained from panorama is" + obtainedSkipBackword);
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
            LogCommentInfo(CL, "Fetching the updated value from box and comparing with the panorama values");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP FORWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to SkipForword State");

            }
            else
            {
                LogComment(CL, "Navigated to SkipForword State");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipForword);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get skipforword Value Expected");
            }
            else
            {

                LogComment(CL, "Expected SkipForword Value is " + ExpectedSkipForword);

            }
            if (ExpectedSkipForword != obtainedSkipForword)
            {
                LogCommentInfo(CL, "Both the Obtained and expected values of SkipForword are not same after setting value on the box");
                isFail = true;
                failCount++;
            }
            else
            {
                LogCommentInfo(CL, "Both the OBtained and expected values of SkipForword are same after setting value on the box");
            }

            LogCommentInfo(CL, "Navigating to Skipbackword state to get the updated expected value from box");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP BACKWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Navigate to SkipBackward State");

            }
            else
            {
                LogComment(CL, "Navigated to SkipBackward State");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out ExpectedSkipBackword);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Get skipBackward Value Expected");
            }
            else
            {

                LogComment(CL, "Expected SkipBackward Value is " + ExpectedSkipBackword);

            }
            if (ExpectedSkipBackword != obtainedSkipBackword)
            {
                LogCommentInfo(CL, "Both the Obtained and expected values of SkipBackward are not same after setting value on the box");
                isFail = true;
                failCount++;
            }
            else
            {
                LogCommentInfo(CL, "Both the OBtained and expected values of SkipBackward are same after setting value on the box");
            }
            PassStep();
        }
    }

    #endregion Step5
    #endregion Steps
}
