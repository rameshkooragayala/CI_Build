/// <summary>
///  Script Name : RMS_0603_PvrSettingsDiskmgmt.cs
///  Test Name   : RMS_0603_PvrSettingsDiskmgmt
///  TEST ID     : 26073
///  QC Version  : 2
///  Variations from QC:none
///  QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Modified by : Appanna
/// </summary>

using System;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using OpenQA.Selenium.Firefox;
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

    public class PvrSettingsDiskmgmt : _Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string browserParameterTabId;
        static string browserSettingTabId;
        static string browserDiskSpaceValue;
        static string cpeId;
        static string path1;
        static string path2;
        static string path3;
        static string path4;
        static string applyPath1;
        static string apply_Path;
        static string SetDiskSpacePath;
        static string diskSpaceOptionInPanoroma;
        static string obtainedDiskSpaceValue;
        static string expectedDiskSpaceOption;
        static string sendKeys_Panorama_SetDiskSpaceValue;
        static string setDiskSpaceValue_fromPanroma;
        static string setDiskSpaceValue_fromBox;
        static string actualDiskSpaceOptionSetOnbox;

        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step 1: Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2:Set the Disk management settings Value from Panoroma->Settings and validate the changes in box ");
            this.AddStep(new Step3(), "Step3:Compare the DiskSpace option value chosen in Panorama matches in the BOX");
            this.AddStep(new Step4(), "Step4:Compare the box value and the panorama value");
            this.AddStep(new Step5(), "Step5:Set Value Over box and Get The same over panorama page");



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
                //Fetch the SettingTabId from Browser ini
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
                //apply button path from Settings browser ini
                applyPath1 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Settings_Apply_Path");
                if (applyPath1 == null)
                {
                    FailStep(CL, "Failed to fetch the Apply settings button path");
                }
                else
                {
                    LogComment(CL, "ApplyPath1 fetched successfully");
                }

                //Disk Space tab Value from Settings-browser ini.
                diskSpaceOptionInPanoroma = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "Div_DiskSpace");
                if (diskSpaceOptionInPanoroma == null)
                {
                    FailStep(CL, "Failed to fetch the Disk_Space from browser ini");
                }
                else
                {
                    LogComment(CL, "epg div value fetched from browser ini is" + diskSpaceOptionInPanoroma);
                }

                //Fetch diskSpace Value to be set from Browser Ini

                //browserpvrDiskManagementid  from Parameters-Browser Ini.
                browserDiskSpaceValue = CL.EA.GetValueFromINI(EnumINIFile.Browser, "PVR_SETTINGS_PARAMS", "PVR_DISKMANAGEMENT");
                if (browserDiskSpaceValue == null)
                {
                    FailStep(CL, "Failed to Fetch thebrowserpvrDiskManagementid from Browser Ini");
                }
                else
                {
                    LogComment(CL, "MenuTimeOutId fetched from browser ini is " + browserDiskSpaceValue);
                }


                //Disk Space Management value to set on the panorama page
                sendKeys_Panorama_SetDiskSpaceValue = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "Sendkey_panorama_SetDiskSpaceValue");
                if (sendKeys_Panorama_SetDiskSpaceValue == null)
                {
                    FailStep(CL, "Failed to fetch the Value to set on panorama from  test ini");
                }
                else
                {
                    if (sendKeys_Panorama_SetDiskSpaceValue.ToUpper() == "AUTOMATIC")
                    {
                        setDiskSpaceValue_fromPanroma = "AUTO DELETE RECORDINGS";
                        setDiskSpaceValue_fromBox = "NEVER DELETE RECORDINGS";
                        //setDiskSpaceValue_fromPanroma = "DISK SPACE MANAGEMENT";
                        LogComment(CL, "Disk Management Value set on box is" + sendKeys_Panorama_SetDiskSpaceValue);
                    }
                    else if (sendKeys_Panorama_SetDiskSpaceValue.ToUpper() == "MANUAL")
                    {
                        setDiskSpaceValue_fromPanroma = "NEVER DELETE RECORDINGS";
                        setDiskSpaceValue_fromBox = "AUTO DELETE RECORDINGS";
                        //setDiskSpaceValue_fromPanroma = "DISK SPACE MANAGEMENT";
                        LogComment(CL, "Disk Management Value set on box is" + sendKeys_Panorama_SetDiskSpaceValue);
                    }
                    else
                    {
                        setDiskSpaceValue_fromPanroma = sendKeys_Panorama_SetDiskSpaceValue;
                        setDiskSpaceValue_fromBox = sendKeys_Panorama_SetDiskSpaceValue;
                        LogComment(CL, "sendKeys_Panorama_SetDiskSpaceValue is: " + sendKeys_Panorama_SetDiskSpaceValue);
                    }
                }

                //Path for apply button
                apply_Path = path1 + "div[3]" + applyPath1;
                //path to set epg value on panorama page
                SetDiskSpacePath = path1 + "div[2]" + path2 + "div[2]" + path3 + "tr" + path4;
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
                    LogComment(CL, "Successfully Logged into Web Page and Entered cpeid and Navigated To Settings Tab");
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
                CL.IEX.Wait(5);
                //set the respective DiskSpace Value  on panorama page            
                try
                {
                    CL.EA.UI.RMS.SetParameterValues(driver, SetDiskSpacePath, apply_Path, diskSpaceOptionInPanoroma, sendKeys_Panorama_SetDiskSpaceValue);
                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }

                LogComment(CL, "successfully set the parameter");

                
                //navigate to Extra Time Before Programme

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:" + setDiskSpaceValue_fromPanroma);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to Extra Time Before Programme");

                }
                else
                {
                    LogComment(CL, "Navigated to DISK SPACE MANAGEMENT value:" + setDiskSpaceValue_fromPanroma);
                    CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
                }

                //Fetch the value of the Disk Space Setting Value  from box
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out actualDiskSpaceOptionSetOnbox);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To get info of DISK SPACE MANAGEMENT");
                }
                else
                {

                    LogComment(CL, "Current DISK SPACE MANAGEMENT Setting Value is " + actualDiskSpaceOptionSetOnbox);

                }

                //navigating to parameters tab in Panaroma Page to fetch the Disk Space Option Value to check whether it is same as in BOX
                try
                {
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, "Failed to Select the Parameter tab on the panorama page" + ex.Message);
                }
                LogComment(CL, "Succesfully selected the parameter tab on the panorama page");

                //fetching the value from panorama Parameters page after setting the value
                CL.IEX.Wait(5);
                obtainedDiskSpaceValue = CL.EA.UI.RMS.GetParameterValues(driver, browserDiskSpaceValue);
                if (obtainedDiskSpaceValue == null)
                {
                    FailStep(CL, "Failed to fetch the Disk Space Value from panorama page");
                }

                else
                {
                    if (obtainedDiskSpaceValue.ToUpper() == "AUTOMATIC")
                    {
                        obtainedDiskSpaceValue = "AUTO DELETE RECORDINGS";

                        LogComment(CL, "Obtained Disk Space Value from panorama page is" + obtainedDiskSpaceValue);
                    }
                    else if (obtainedDiskSpaceValue.ToUpper() == "MANUAL")
                    {
                        obtainedDiskSpaceValue = "NEVER DELETE RECORDINGS";
                        LogComment(CL, "Obtained Disk Space Value from panorama page is" + obtainedDiskSpaceValue);
                    }
                    else
                    {
                        obtainedDiskSpaceValue = "WARN ME ABOUT CONFLICTS";
                        LogComment(CL, "Obtained Disk Space Value from panorama page is " + obtainedDiskSpaceValue);

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


                if ((setDiskSpaceValue_fromPanroma != actualDiskSpaceOptionSetOnbox) && (setDiskSpaceValue_fromPanroma != obtainedDiskSpaceValue))
                {
                    FailStep(CL, "Both the values setting on panorama web page and set on box  are not equal");
                }
                else
                {
                    LogComment(CL, "Values set on panorama:" + setDiskSpaceValue_fromPanroma + "=" + "Value fetched from Box" + actualDiskSpaceOptionSetOnbox + "=" + "Value fetched from Panroma Get Parameters " + obtainedDiskSpaceValue + "are equal");
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


                //Change the current DiskSpace Management option to other value from box and Validate the changes are reflected in Panorama Page

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:" + setDiskSpaceValue_fromBox);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To Navigate to DISK SPACE MANAGEMENT");

                }

                else
                {
                    LogComment(CL, "Navigated to DISK SPACE MANAGEMENT" + setDiskSpaceValue_fromBox);
                    res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");

                }
                //Fetch the value set in the Box and store in variable for comparsion with Get Parameters in Panoroma page

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedDiskSpaceOption);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To get info of DISK SPACE MANAGEMENT");
                }
                else
                {

                    LogComment(CL, "Current DISK SPACE MANAGEMENT Setting Value is " + expectedDiskSpaceOption);

                }

                CL.IEX.Wait(20);


                LogComment(CL, "Succesfully selected the parameter tab on the panorama page");

                
				obtainedDiskSpaceValue = CL.EA.UI.RMS.GetParameterValues(driver, browserDiskSpaceValue);
				if (obtainedDiskSpaceValue == null)
                {
                    FailStep(CL, "Failed to fetch the Disk Space Value from panorama page");
                }

                else
                {
                    if (obtainedDiskSpaceValue.ToUpper() == "AUTOMATIC")
                    {
                        obtainedDiskSpaceValue = "AUTO DELETE RECORDINGS";

                        LogComment(CL, "Obtained Disk Space Value from panorama page is" + obtainedDiskSpaceValue);
                    }
                    else if (obtainedDiskSpaceValue.ToUpper() == "MANUAL")
                    {
                        obtainedDiskSpaceValue = "NEVER DELETE RECORDINGS";
                        LogComment(CL, "Obtained Disk Space Value from panorama page is" + obtainedDiskSpaceValue);
                    }
                    else
                    {
                        obtainedDiskSpaceValue = "WARN ME ABOUT CONFLICTS";
                        LogComment(CL, "Obtained Disk Space Value from panorama page is " + obtainedDiskSpaceValue);

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

                //comparing the value set in box is same in panorama page.
                if (expectedDiskSpaceOption != obtainedDiskSpaceValue)
                {
                    FailStep(CL, "values set From Box and Values retrieved from Panroama Get Parameters are not equal");
                }
                else
                {
                    LogComment(CL, "Both the values After setting on panorama page are  equal");
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




