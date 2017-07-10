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

    public class RMS_0001_GetParameterValues : _Test
    {

        [ThreadStatic]
        static FirefoxDriver driver;
        static string paramId;
        static string cpeId;
        static string BrowserTabControlId;
        static string deviceCountryBrowserId;
        static string manufacturerOuiBrowserId;
        static string productClassBrowserId;
        static string serialNumberBrowserId;
        static string fosProfileBrowserId;
        static string deviceSummaryBrowserId;
        static string descriptionBrowserId;
        static string expectedDescription;
        static string expectedDeviceCountry;
        static string expectedSerialNumber;
        static string expectedManufacturerOui;
        static string obtainedSerialNumber;
        static string expectedFosProfile;
        static string expectedDeviceSummary;
        static string expectedProductClass;
        static string obtainedDeviceCountry;
        static string obtainedManufacturerOui;
        static string obtainedDescription;
        static string obtainedFosProfile;
        static string obtainedProductClass;
        static string obtainedDeviceSummary;


        private static _Platform CL;
        private static bool isFail = false;
        private static int noOfFailures = 0;
        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama Webpage
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step 1: GO TO PANORAMA WEBPAGE LOGIN AND ENTER BOXID AND SEARCH");
            this.AddStep(new Step2(), "Step 2: PASSING THE PARAMETER VALUE ON TO THE PANORAM PAGE AND FETCH THE VALUE");

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

                //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMO WEBPAGE
                deviceCountryBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "DEVICE_COUNTRY");
                if (deviceCountryBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch deviceCountryBrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "deviceCountryBrowserId fetched is : " + deviceCountryBrowserId);

                }
                manufacturerOuiBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "MANUFACTUREROUI");
                if (manufacturerOuiBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch  manufacturerOuiBrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "Service fetched is : " + manufacturerOuiBrowserId);

                }
                productClassBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "PRODUCT_CLASS");
                if (productClassBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch  productClassBrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "productClassBrowserId fetched is : " + productClassBrowserId);

                }

                descriptionBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "DESCRIPTION");
                if (descriptionBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch  descriptionBrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "descriptionBrowserId fetched is : " + descriptionBrowserId);

                }

                fosProfileBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "COM_FOSPROFILE");
                if (fosProfileBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch  fosProfileBrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "fosProfileBrowserId fetched is : " + fosProfileBrowserId);

                }

                deviceSummaryBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "DEVICE_SUMMARY");
                if (deviceSummaryBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch  deviceSummaryBrowserId  from ini File.");
                }
                else
                {
                    LogComment(CL, "deviceSummaryBrowserId  fetched is : " + deviceSummaryBrowserId);

                }

                serialNumberBrowserId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "SERIALNUMBER");
                if (serialNumberBrowserId == null)
                {
                    FailStep(CL, "Failed to fetch serialNumberBrowserId from ini File.");
                }
                else
                {
                    LogComment(CL, "serialNumberBrowserId fetched is : " + serialNumberBrowserId);

                }

                BrowserTabControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (BrowserTabControlId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + BrowserTabControlId);

                }

                cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }

                //fetching values from test and environment ini files
                obtainedProductClass = CL.EA.UI.Utils.GetValueFromEnvironment("ProductClass");
                if (obtainedProductClass == null)
                {
                    FailStep(CL, "Failed to fetch  productClass from ini File.");
                }
                else
                {
                    LogComment(CL, "productClass fetched is : " + obtainedProductClass);

                }
                obtainedManufacturerOui = CL.EA.UI.Utils.GetValueFromEnvironment("ManufacturerOui");
                if (obtainedManufacturerOui == null)
                {
                    FailStep(CL, "Failed to fetch  manufacturerOui from ini File.");
                }
                else
                {
                    LogComment(CL, "manufacturerOui fetched is : " + obtainedManufacturerOui);

                }
                obtainedFosProfile = CL.EA.UI.Utils.GetValueFromEnvironment("FosProfile");
                if (obtainedFosProfile == null)
                {
                    FailStep(CL, "Failed to fetch  FOS_PROFILE from ini File.");
                }
                else
                {
                    LogComment(CL, "FOS_PROFILE fetched is : " + obtainedFosProfile);

                }
                obtainedDeviceSummary = CL.EA.UI.Utils.GetValueFromEnvironment("DeviceSummary");
                if (obtainedDeviceSummary == null)
                {
                    FailStep(CL, "Failed to fetch  deviceSummary from ini File.");
                }
                else
                {
                    LogComment(CL, "deviceSummary fetched is : " + obtainedDeviceSummary);

                }
                obtainedDescription = CL.EA.UI.Utils.GetValueFromEnvironment("Description");
                if (obtainedDescription == null)
                {
                    FailStep(CL, "Failed to fetch  description from ini File.");
                }
                else
                {
                    LogComment(CL, "description fetched is : " + obtainedDescription);

                }
                obtainedSerialNumber = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (obtainedSerialNumber == null)
                {
                    FailStep(CL, "Failed to fetch  serialNumber from ini File.");
                }
                else
                {
                    LogComment(CL, "serialNumber fetched is : " + obtainedSerialNumber);

                }
                obtainedDeviceCountry = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEVICECOUNTRY");

                if (obtainedDeviceCountry == null)
                {
                    FailStep(CL, "Failed to fetch  deviceCountry from ini File.");
                }
                else
                {
                    LogComment(CL, "deviceCountry fetched is : " + obtainedDeviceCountry);

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
                CL.IEX.Wait(5);
                //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, BrowserTabControlId);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
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
                //DEVICE COUNTRY

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
               
                expectedDeviceCountry = CL.EA.UI.RMS.GetParameterValues(driver, deviceCountryBrowserId);
               
                if (expectedDeviceCountry == "")
                {
                    FailStep(CL, "Failed due to fetched DeviceCountry value from Panorama is null");

                }

                LogCommentImportant(CL, "Expected device country from Pa page is " + expectedDeviceCountry);

                //comparing box and panorama device country values
                

                if (expectedDeviceCountry != obtainedDeviceCountry)
                {

                    LogComment(CL, "Both The values  " + expectedDeviceCountry + "And" + obtainedDeviceCountry + "are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedDeviceCountry + "And Fetched value From INI File" + obtainedDeviceCountry + "Are Equal");
                }
                CL.IEX.Wait(5);


                //MANUFACTURER OUI          

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
                expectedManufacturerOui = CL.EA.UI.RMS.GetParameterValues(driver, manufacturerOuiBrowserId);

                //FETCHING THE PARAMETER VALUE FROM THE INI FILE
                if (expectedManufacturerOui == "")
                {
                    FailStep(CL, "Failed due to fetched  manufacturerOui value from Panorama is null");

                }

                LogCommentImportant(CL, "Expected manufacturerOui from Panorama is " + expectedManufacturerOui);

                //verifying expected panorama value and obtained value
                if (expectedManufacturerOui != obtainedManufacturerOui)
                {

                    LogComment(CL, "Fetched value From Panorama Page" + expectedManufacturerOui + "And Fetched value From INI File" + obtainedManufacturerOui + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedManufacturerOui + "And Fetched value From INI File" + obtainedManufacturerOui + "Are Equal");
                }
                CL.IEX.Wait(5);


                //FOSPROFILE
               


                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
                expectedFosProfile = CL.EA.UI.RMS.GetParameterValues(driver, fosProfileBrowserId);

                if (expectedFosProfile == "")
                {
                    FailStep(CL, "Failed due to fetched  FosProfile value from Panorama is null");

                }
                else if (expectedFosProfile == "FOSP_UPC_Cable_Gateway_HD_PVR_INTEL-Groveland_FOS21.1.0:P1.12")
                {
                    expectedFosProfile = "FOSP_UPC_Cable_Gateway_HD_PVR_INTEL-Groveland_FOS21.1.0:P...";

                }
                else
                {
                    LogComment(CL, "Succesfully fetched the fos Profile Value from panorama page");
                }


                LogCommentImportant(CL, "Expected FosProfile from Panorama is " + expectedFosProfile);

                
           
                //verifying expected panorama value and obtained value
                if (expectedFosProfile == obtainedFosProfile)
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedFosProfile + "And Fetched value From INI File" + obtainedFosProfile + "Are Equal");

                }

                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedFosProfile + "And Fetched value From INI File" + obtainedFosProfile + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }

                CL.IEX.Wait(4);


                //product class

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
                expectedProductClass = CL.EA.UI.RMS.GetParameterValues(driver, productClassBrowserId);

                if (expectedProductClass == "")
                {
                    FailStep(CL, "Failed due to fetched  product calss value from Panorama is null");

                }
                LogCommentImportant(CL, "Expected ProductClass from Panorama is " + expectedProductClass);

                //verifying expected panorama value and obtained value
                if (expectedProductClass == obtainedProductClass)
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedProductClass + "And Fetched value From INI File" + obtainedProductClass + "Are Equal");

                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedProductClass + "And Fetched value From INI File" + obtainedProductClass + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                CL.IEX.Wait(5);

                //DESCRIPTION           

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
                expectedDescription = CL.EA.UI.RMS.GetParameterValues(driver, descriptionBrowserId);
                if (expectedDescription == "")
                {
                    FailStep(CL, "Failed due to fetched  description value from Panorama is null");

                }
                LogCommentImportant(CL, "Expected Description from Panorama is " + expectedDescription);

                //verifying expected panorama value and obtained value

                if (expectedDescription == obtainedDescription)
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedDescription + "And Fetched value From INI File" + obtainedDescription + "Are Equal");

                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedDescription + "And Fetched value From INI File" + obtainedDescription + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }
                CL.IEX.Wait(5);
                //DEVICE SUMMARY     

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
                expectedDeviceSummary = CL.EA.UI.RMS.GetParameterValues(driver, deviceSummaryBrowserId);

                if (expectedDeviceSummary == "")
                {
                    FailStep(CL, "Failed due to fetched  device summary value from Panorama is null");
                }
                LogCommentImportant(CL, "Expected DeviceSummary from Panorama is " + expectedDeviceSummary);

                //verifying expected panorama value and obtained value
                if (expectedDeviceSummary == obtainedDeviceSummary)
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedDeviceSummary + "And Fetched value From INI File" + obtainedDeviceSummary + "Are Equal");

                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedDeviceSummary + "And Fetched value From INI File" + obtainedDeviceSummary + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }

                CL.IEX.Wait(5);

                //SERIAL  NUMBER    

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE
                expectedSerialNumber = CL.EA.UI.RMS.GetParameterValues(driver, serialNumberBrowserId);

                if (expectedSerialNumber == "")
                {
                    FailStep(CL, "Failed due  to fetched  device summary value from Panorama is null");

                }
                LogCommentImportant(CL, "Expected SerialNumber from Panorama is " + expectedSerialNumber);

                //verifying expected panorama value and obtained value
                if (expectedSerialNumber == obtainedSerialNumber)
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedSerialNumber + "And Fetched value From INI File" + obtainedSerialNumber + "Are Equal");

                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedSerialNumber + "And Fetched value From INI File" + obtainedSerialNumber + "Are Not Equal");
                    isFail = true;
                    noOfFailures++;
                }

                CL.IEX.Wait(6);

                //Finally display the number of Sections Failed If any
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

