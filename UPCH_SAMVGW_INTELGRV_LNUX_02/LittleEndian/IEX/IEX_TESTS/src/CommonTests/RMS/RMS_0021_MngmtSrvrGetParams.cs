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


public class RMS_0021 : _Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string browserParameterTabId;
        static string browserUpgradeMangedId;
        static string cpeId;
        static string ErrorStatementMsg;
        static IWebElement checkBox1;
        static string obtainedUpgradeManged;
        static string browserReadErrorCountId;
        static string expectedReadErrorCount;
        static string obtainedReadErrorCount;
        static string obtainedUrl;
        static string expectedUrl;
        static string browserUrlId;
    
        
        static string widgetnotificationflagID;
        static string obtainedwidgetnotificationflag;
        private static bool isFail = false;
        private static int noOfFailures = 0;
#region Create Structure
public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get CPE ID and other parameters From ini File");
        this.AddStep(new Step1(), "Step1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2: Trying To Set the Upgrade Manged Value on Panorama and Getting the Error Message Succesfully And Getting the Parameter Value of HArd Disk Read Error Count Value ");
        
        
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
            browserParameterTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
            if (browserParameterTabId == null)
            {
                FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
            }
            else
            {
                LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

            }
            browserUpgradeMangedId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "MANAGEMENT_SERVER_SETTINGS", "UPGRADE_MANAGED_ID");
            if(browserUpgradeMangedId==null)
            {
            FailStep(CL,"Failed to fetch the browserUpgradeMangedId from browser ini");
            }
            else
            {
             LogCommentInfo(CL,"Successfullly Fetched the browserUpgradeMangedId from browser ini is"+browserUpgradeMangedId);
            }
            browserReadErrorCountId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "MANAGEMENT_SERVER_SETTINGS", "READ_ERROR_COUNT_ID");
            if (browserReadErrorCountId == null)
            {
                FailStep(CL, "Failed to fetch the browserReadErrorCountId from browser ini");
            }
            else
            {
                LogCommentInfo(CL, "Successfullly Fetched the browserReadErrorCountId from browser ini is" + browserReadErrorCountId);
            }
            expectedReadErrorCount = CL.EA.UI.Utils.GetValueFromEnvironment("ReadError_Count");
            if (expectedReadErrorCount == null)
            {
                FailStep(CL, "Failed to fetch  expectedReadErrorCount from ini File.");
            }
            else
            {
                LogComment(CL, "expectedReadErrorCount fetched is : " + expectedReadErrorCount);

            }
            expectedUrl = CL.EA.UI.Utils.GetValueFromEnvironment("Managment_Url");
            if (expectedUrl == null)
            {
                FailStep(CL, "Failed to fetch  expectedUrl from ini File.");
            }
            else
            {
                LogComment(CL, "expectedUrl fetched from Environment ini is : " + expectedUrl);

            }
            browserUrlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "MANAGEMENT_SERVER_SETTINGS", "MANAGEMENT_URL_ID");
            if (browserUrlId == null)
            {
                FailStep(CL, "Failed to fetch the browserUrlId from browser ini");
            }
            else
            {
                LogCommentInfo(CL, "Successfullly Fetched the browserUrlId from browser ini is" + browserUrlId);
            }
            widgetnotificationflagID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "WIDGETNOTIFICATIONFLAG");
            if (widgetnotificationflagID == null)
            {
                FailStep(CL, "Failed to fetch widgetnotificationflagID from ini File.");
            }
            else
            {
                LogComment(CL, "ID  fetched is : " + widgetnotificationflagID);

            }
        PassStep();
        }
    }
#endregion PreCondition
#region Step1
     private class Step1 : _Step
    {
        public override void Execute()
        {

            StartStep();
             driver = new FirefoxDriver();
            
            //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
            res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserParameterTabId);
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
              obtainedUrl = CL.EA.UI.RMS.GetParameterValues(driver, browserUrlId);
              if (obtainedUrl == null)
              {
                  FailStep(CL, "Failed To Fetch the Management server Url value from Panorama");

              }
              else
              {
                  LogCommentInfo(CL, "Obtained Management server Url from panorama is " + obtainedUrl);
              }

              CL.IEX.Wait(1);
              obtainedReadErrorCount = CL.EA.UI.RMS.GetParameterValues(driver, browserReadErrorCountId);
              if (obtainedReadErrorCount == null)
              {
                  FailStep(CL, "Failed To Fetch the HD ReadErrorCount value from Panorama");

              }
              else
              {
                  LogCommentInfo(CL, "Obtained HD ReadErrorCount from panorama is " + obtainedReadErrorCount);
              }
              obtainedwidgetnotificationflag = CL.EA.UI.RMS.GetParameterValues(driver, widgetnotificationflagID);
              driver.Navigate().Refresh();
              CL.IEX.Wait(5);
              IWebElement element = driver.FindElement(By.XPath("//input[@type='checkbox']"));
              if (!element.Selected)
              {
                  obtainedwidgetnotificationflag = "OFF";
                  LogCommentInfo(CL, "Failed because obtained widget notification flag from panorama is" + obtainedwidgetnotificationflag);
                  isFail = true;
                  noOfFailures++;
              }
              else
              {
                  obtainedwidgetnotificationflag = "ON";
                  LogCommentImportant(CL, "obtained widget notification flag from panorama is correct " + obtainedwidgetnotificationflag);
              }

              try
              {
                  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                  LogCommentInfo(CL, "Clicking on the input box to send keys");
                  wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                  CL.IEX.Wait(2);
                  LogCommentInfo(CL, "Clicking on v-filterselect-input");
                  driver.FindElement(By.ClassName("v-filterselect-input")).Click();

                  wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                  CL.IEX.Wait(2);
                  LogCommentInfo(CL, "Sending the keys " + browserUpgradeMangedId);
                  driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + browserUpgradeMangedId + "");
                  CL.IEX.Wait(2);
                  wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + browserUpgradeMangedId + "']")));
                  CL.IEX.Wait(2);
                  LogCommentInfo(CL, "Clicking on span text " + browserUpgradeMangedId);
                  driver.FindElement(By.XPath("//span[text() = '" + browserUpgradeMangedId + "']")).Click();
                  wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='Edit']")));
                  CL.IEX.Wait(3);
                  driver.FindElement(By.XPath("//span[text()='Edit']")).Click();
                  wait.Until(ExpectedConditions.ElementIsVisible(By.Id("gwt-uid-1")));
                  CL.IEX.Wait(2);
                  checkBox1 = driver.FindElementById("gwt-uid-1");
                  
                  if (!checkBox1.Selected)
                  {
                      CL.IEX.Wait(2);
                      checkBox1.Click();
                      CL.IEX.Wait(2);
                      driver.FindElement(By.XPath("//span[text()='Apply']")).Click();
                      CL.IEX.Wait(10);
                      ErrorStatementMsg = driver.FindElementByXPath("//div[@id='notification-msg-text']").Text;
                      LogCommentInfo(CL, "ErrorMessage We received whilesetting value to true is " + ErrorStatementMsg);
                     
                      if (ErrorStatementMsg == "TR browser: Failed to update device. Connection to device may be down.")
                      {
                          LogComment(CL, "Succesfully unable to set the value to true for UpgradeManaged value");
                      }
                      else 
                      {
                          LogCommentInfo(CL, "Failed As no error message received while setting value to true");
                          isFail = true;
                          noOfFailures++;
                      }
                      
                  }
                  else
                  {
                      FailStep(CL, "Failed Because Unable to Set the Falg true as it is already in true mode");
                  }



              }
              catch
              {
                  CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                  CL.IEX.Wait(3);
                  CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);
                  CL.IEX.Wait(5);
                  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                  LogCommentInfo(CL, "Clicking on the input box to send keys");
                  wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                  CL.IEX.Wait(2);
                  LogCommentInfo(CL, "Clicking on v-filterselect-input");
                  driver.FindElement(By.ClassName("v-filterselect-input")).Click();

                  wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")));
                  CL.IEX.Wait(2);
                  LogCommentInfo(CL, "Sending the keys " + browserUpgradeMangedId);
                  driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + browserUpgradeMangedId + "");
                  CL.IEX.Wait(2);
                  wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + browserUpgradeMangedId + "']")));
                  CL.IEX.Wait(2);
                  LogCommentInfo(CL, "Clicking on span text " + browserUpgradeMangedId);
                  driver.FindElement(By.XPath("//span[text() = '" + browserUpgradeMangedId + "']")).Click();
                  wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='Edit']")));
                  CL.IEX.Wait(3);
                  driver.FindElement(By.XPath("//span[text()='Edit']")).Click();
                  wait.Until(ExpectedConditions.ElementIsVisible(By.Id("gwt-uid-1")));
                  CL.IEX.Wait(2);
                  checkBox1 = driver.FindElementById("gwt-uid-1");
                  if (!checkBox1.Selected)
                  {
                      checkBox1.Click();
                      wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='Apply']")));
                      driver.FindElement(By.XPath("//span[text()='Apply']")).Click();
                      CL.IEX.Wait(10);
                      ErrorStatementMsg = driver.FindElementByXPath("//div[@id='notification-msg-text']").Text;
                      LogCommentInfo(CL, "ErrorMessage We received whilesetting value to true is " + ErrorStatementMsg);
                      if (ErrorStatementMsg == "TR browser: Failed to update device. Connection to device may be down.")
                      {
                          LogComment(CL, "Succesfully unable to set the value to true for UpgradeManaged value");
                      }
                      else
                      {
                          LogCommentInfo(CL, "Failed As no error message received while setting value to true");
                          isFail = true;
                          noOfFailures++;
                      }
                  }
                  else
                  {
                      FailStep(CL, "Failed Because Unable to Set the Falg true as it is already in true mode");
                  }

              }
              checkBox1 = driver.FindElement(By.XPath("//input[@type='checkbox']"));
              if (checkBox1.Selected)
              {
                  obtainedUpgradeManged = "True";
                  FailStep(CL, "Failed Because Upgrade Manged value is true");
              }
              else
              {
                  obtainedUpgradeManged = "False";
                  LogCommentInfo(CL, "UpgradeManged Value is false");
              }

             


              if (expectedReadErrorCount != obtainedReadErrorCount)
              {
                  LogCommentInfo(CL, "Obtained and expected ReadErrorCount values are not equal");
                  isFail = true;
                  noOfFailures++;
              }
              else
              {
                  LogCommentInfo(CL, "Obtained and expected ReadErrorCount values are equal");
              
              }
              
              if (expectedUrl != obtainedUrl)
              {
                  LogCommentInfo(CL, "Obtained and expected Url values are not equal");
                  isFail = true;
                  noOfFailures++;
              }
              else
              {
                  LogCommentInfo(CL, "Obtained and expected Url values are equal");

              }

              if (isFail)
              {
                  FailStep(CL, "Number of validations failed " + noOfFailures + "...Please Check above Steps for Failure reasons");
              }
              PassStep();
          }
      }

#endregion Step2
#endregion Steps
#region PostExecute
      public override void PostExecute()
      {
          driver.Close();
      }
#endregion PostExecute     
}

