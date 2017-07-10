using System;
using Microsoft.VisualBasic;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
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


    public class RMS_0005_Reboot :_Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
       
        private static _Platform CL;

        static string cpeId;
        static string quickActionRebootId;
        static string quickActionConfirmId;
        static string browserParameterTabId;
        static int obtainedDeviceUpTime1;
        static int obtainedDeviceUpTime2;
        static string browserDeviceUpTimeId;
        static string output;
        static string DeviceHomeId;
        static string ShutDownMilestone;
        static ArrayList list = new ArrayList();
        static bool isShutDownMilestoneRecieved;
        static int Time_In_Standby = 60;
        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step 1: Perform reboot from panorama page and check the device uptime value");
            

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
                //Quick ActionRebootId from Browser ini
                quickActionRebootId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QucikActin_Reboot_Id");
                if (quickActionRebootId == null)
                    FailStep(CL, "Failed to Fetch quickActionRebootId from ini file");
                else
                    LogComment(CL, "quickActionRebootId fetched is" + quickActionRebootId);

                // Quick Action ConfirmId from Browser ini
                quickActionConfirmId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "QuickAction_Confirm_Id");
                if (quickActionConfirmId == null)
                    FailStep(CL, "Failed to Fetch quickActionConfirmation Id from ini file");
                else
                    LogComment(CL, "quickActionConfirmation Id fetched is" + quickActionConfirmId);
                browserParameterTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (browserParameterTabId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserParameterTabId);

                }
                browserDeviceUpTimeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "DEVICE_UPTIME");
                if (browserDeviceUpTimeId == null)
                {
                    FailStep(CL, "browserDeviceUpTimeId obtained from ini is null");
                }
                else
                {
                    LogCommentInfo(CL, "browserDeviceUpTimeId obtained from ini is "+browserDeviceUpTimeId);
                }
                DeviceHomeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Device_Home");
                if (DeviceHomeId == null)
                {
                    FailStep(CL, "Failed to fetch the Device HomeID from Browser ini file");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained DeviceHomeID from ini file is"+DeviceHomeId);
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
               
                //Login To panorama Page and enter Boxid and perform Reboot
                res = CL.EA.RMSLoginAndQuickActions(driver, cpeId, quickActionRebootId, quickActionConfirmId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login and perform Reboot");
                }
          
                
                //getting fas shutdown milestones
                ShutDownMilestone = CL.EA.UI.Utils.GetValueFromMilestones("ShutDown");
                
                 CL.EA.UI.Utils.BeginWaitForDebugMessages(ShutDownMilestone, 120);
               
                isShutDownMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(ShutDownMilestone, ref list);

               //Checking Whether fas milestones are coming or not
                if (!isShutDownMilestoneRecieved)
                {
                    FailStep(CL, "Failed to get FAS ShutDown Milestone");
                }

                CL.IEX.Wait(60);

                //mount the box after reboot
                res=CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
               if (!res.CommandSucceeded)
                {
                FailStep(CL, res);
                }

            //Stay in Standby for a few seconds
            
            CL.IEX.Wait(Time_In_Standby);
            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }
            //fetching the device uptime after reboot
            
            output= CL.EA.UI.RMS.GetParameterValues(driver, browserDeviceUpTimeId);
            int.TryParse(output, out obtainedDeviceUpTime1);
            if (obtainedDeviceUpTime1.ToString() == null)
            {
                FailStep(CL, "Obtained Device Uptime from panorama page is null");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Device Uptime from panorama page is"+obtainedDeviceUpTime1);
            }

            CL.IEX.Wait(60);
            output = CL.EA.UI.RMS.GetParameterValues(driver, browserDeviceUpTimeId);
            int.TryParse(output, out obtainedDeviceUpTime2);
            if (obtainedDeviceUpTime2.ToString() == null)
            {
                FailStep(CL, "Obtained Device Uptime from panorama page is null");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Device Uptime from panorama page is" + obtainedDeviceUpTime2);
            }
            if (obtainedDeviceUpTime2 > obtainedDeviceUpTime1)
            {
                LogCommentInfo(CL, "The Uptime2 is greater than the uptime1");

            }
            else
            {
                FailStep(CL, "The Uptime2 is not greater than the uptime1");
               
            }
            
            try
            {
                //CL.EA.UI.RMS.SelectTab(driver, DeviceHomeId);
              CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@title='"+quickActionRebootId+"']")));
            driver.FindElement(By.XPath("//span[@title='" + quickActionRebootId + "']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='" + quickActionConfirmId + "']")));
            driver.FindElement(By.XPath("//span[text()= '" + quickActionConfirmId + "']")).Click();
           
            //getting fas shutdown milestones
            ShutDownMilestone = CL.EA.UI.Utils.GetValueFromMilestones("ShutDown");

            CL.EA.UI.Utils.BeginWaitForDebugMessages(ShutDownMilestone, 120);

            isShutDownMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(ShutDownMilestone, ref list);

            //Checking Whether fas milestones are coming or not
            if (!isShutDownMilestoneRecieved)
            {
                FailStep(CL, "Failed to get FAS ShutDown Milestone");
            }

            CL.IEX.Wait(60);

            //mount the box after reboot
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Stay in Standby for a few seconds
            CL.IEX.Wait(Time_In_Standby);
            try
            {
                CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

            }
            catch (Exception ex)
            {
                FailStep(CL, ex.Message);
            }
            //fetching the device uptime after reboot

            output = CL.EA.UI.RMS.GetParameterValues(driver, browserDeviceUpTimeId);
            int.TryParse(output, out obtainedDeviceUpTime1);
            if (obtainedDeviceUpTime1.ToString() == null)
            {
                FailStep(CL, "Obtained Device Uptime from panorama page is null");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Device Uptime from panorama page is" + obtainedDeviceUpTime1);
            }
            if (obtainedDeviceUpTime1 >= 0 && obtainedDeviceUpTime1 < obtainedDeviceUpTime2)
            {
                LogCommentInfo(CL, "ObtainedDeviceUpTime is" + obtainedDeviceUpTime1);
            }
            else
            {
                FailStep(CL, "Obtaineddevice uptime is not begun from 0");
            }
            PassStep();
            }
        }
    #endregion Step1

          #endregion steps
        #region PostExecute

        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute
    }

          