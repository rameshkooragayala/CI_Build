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
using System.Xml;

    public class RMS_PurchaseProtection:_Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string browserParameterTabId;
        static string browserSettingTabId;
        static string browserPurchageProtectionId;
        static string cpeId;
        static string existingValue;
        static string sendkeys_box_PurchageProtection;
        static string sendkeys_Panorma_PurchageProtection;
        static string expectedPurchageProtection;
        static string obtainedPurchageProtection;
        
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
            this.AddStep(new Step1(), "Step1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2: Setting the standby setting value from Panorama and compaing the same with CPE  ");
            this.AddStep(new Step3(), "Step3:Setting the value in CPE and Comparing the same with Panorama page ");

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
                browserPurchageProtectionId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "PURCHAGEPROTECTION");
                if (browserPurchageProtectionId == null)
                {
                    FailStep(CL, "BrowserPurchageId fetched from ini file is "+browserPurchageProtectionId);
                }
                sendkeys_box_PurchageProtection = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SendKey_Box_PurchageProtection");
                if (sendkeys_box_PurchageProtection == null)
                {
                    FailStep(CL, "Failed to get the Purchage Protection value to be set on the box from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Purchage Protection value to be set on the box from Test Ini " + sendkeys_box_PurchageProtection);
                }
                sendkeys_Panorma_PurchageProtection = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SenKey_Panorma_PurchageProtection");
                if (sendkeys_Panorma_PurchageProtection == null)
                {
                    FailStep(CL, "Failed to get the Purchage Protection value to be set on the Panorama from Test Ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Purchage Protection value to be set on the Panorama from Test Ini " + sendkeys_Panorma_PurchageProtection);
                }
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
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PURCHASE PROTECTION");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to navigate to purchase protection state");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully navigated to purchase protection state");
                }
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out existingValue);
                try
                {
                    switch (sendkeys_box_PurchageProtection)
                    {
                        case "OFF":
                            if (existingValue == sendkeys_box_PurchageProtection)
                            {
                                CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                                expectedPurchageProtection = existingValue;
                                LogCommentInfo(CL, "Successfully set the parameter PurchageProtection setting to " + expectedPurchageProtection);
                            }

                            else
                            {
                                try
                                {
                                    do
                                    {
                                        CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
                                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedPurchageProtection);
                                        loopCount++;
                                    }
                                    while (expectedPurchageProtection != sendkeys_box_PurchageProtection || loopCount < 2);
                                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        CL.IEX.SendIRCommand("0", -1, ref timeStamp);
                                    }
                                    CL.EA.EnterDeafultPIN("PIN & PARENTAL CONTROL");
                                    CL.IEX.Wait(1);
                                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                                    CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedPurchageProtection);

                                }

                                catch (Exception ex)
                                {
                                    FailStep(CL, ex.Message);
                                }
                                LogCommentInfo(CL, "Successfully set the parameter PurchageProtection setting to " + expectedPurchageProtection);
                            }
                            break;
                        case "ON":
                            {
                                if (existingValue == sendkeys_box_PurchageProtection)
                                {
                                    CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                                    expectedPurchageProtection = existingValue;
                                    LogCommentInfo(CL, "Successfully set the parameter PurchageProtection setting to " + expectedPurchageProtection);
                                }
                                else
                                {
                                    try
                                    {
                                        do
                                        {
                                            CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
                                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedPurchageProtection);
                                            loopCount++;
                                        }
                                        while (expectedPurchageProtection != sendkeys_box_PurchageProtection || loopCount < 2);
                                        CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                                    }
                                    catch (Exception ex)
                                    {
                                        FailStep(CL, ex.Message);
                                    }
                                    LogCommentInfo(CL, "Successfully set the parameter PurchageProtection setting to " + expectedPurchageProtection);
                                }
                            }
                            break;

                    }
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
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

                obtainedPurchageProtection = CL.EA.UI.RMS.GetParameterValues(driver,browserPurchageProtectionId);
                IWebElement element = driver.FindElement(By.XPath("//input[@type='checkbox']"));

                if (!element.Selected)
                {
                    obtainedPurchageProtection = "OFF";
                    LogComment(CL, "obtained Purchage Protection retrieved from panorama is " + obtainedPurchageProtection);

                }
                else
                {
                    obtainedPurchageProtection = "ON";
                    LogComment(CL, "Obtained  Purchage Protection retrieved from panorama page is" + obtainedPurchageProtection);
                }

                
                if (expectedPurchageProtection != obtainedPurchageProtection)
                {
                    FailStep(CL, "Both the Expected and obtained Purchage protection values are not same");
                   
                }
                else
                {
                    LogCommentInfo(CL, "Both the OBtained and expected values of purchage protection are same");
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
                    
                    driver.Navigate().Refresh();
                    driver.FindElement(By.XPath("//span[text() = 'Edit']")).Click();

                    IWebElement element = driver.FindElement(By.XPath("//input[@type='checkbox']"));
                   
                    switch(sendkeys_Panorma_PurchageProtection)
                    {
                        case "OFF":
                            if (!element.Selected)
                            {
                               
                                driver.FindElement(By.XPath("//span[text() = 'Apply']")).Click();
                                obtainedPurchageProtection = "OFF";
                                LogCommentInfo(CL, "Successfully set the purchage protection value on the panoram page to" + obtainedPurchageProtection);
                            }
                            else
                            {
                                element.Click();
                                driver.FindElement(By.XPath("//span[text() = 'Apply']")).Click();
                                obtainedPurchageProtection = "OFF";
                                LogCommentInfo(CL, "Successfully set the purchage protection value on the panoram page to" + obtainedPurchageProtection);
                            }
                            break;
                        case "ON":
                            if (element.Selected)
                            {
                                driver.FindElement(By.XPath("//span[text() = 'Apply']")).Click();
                                obtainedPurchageProtection = "ON";
                                LogCommentInfo(CL, "Successfully set the purchage protection value on the panoram page to" + obtainedPurchageProtection);
                            }
                            else
                            {
                                element.Click();
                                driver.FindElement(By.XPath("//span[text() = 'Apply']")).Click();
                                obtainedPurchageProtection = "ON";
                                LogCommentInfo(CL, "Successfully set the purchage protection value on the panoram page to" + obtainedPurchageProtection);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                CL.IEX.Wait(2);
                LogCommentInfo(CL, "Navigating to the purchage protection state and fetching the value");
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PURCHASE PROTECTION");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to navigate to purchase protection state");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully navigated to purchase protection state");
                }
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedPurchageProtection);
                if (expectedPurchageProtection == null)
                {
                    FailStep(CL, "Failed to fetch the expected Purchage PRotection value from box");
                }
                else
                {
                    LogCommentInfo(CL, "Expected Purchage protection value obtained from box is "+expectedPurchageProtection);
                }

                LogCommentInfo(CL, "Comparing the both obtained and expected values of purchase protection");
                if (expectedPurchageProtection != obtainedPurchageProtection)
                {
                    FailStep(CL, "Both the Expected and obtained Purchage protection values are not same");
                   
                }
                else
                {
                    LogCommentInfo(CL, "Both the OBtained and expected values of purchage protection are same");
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



