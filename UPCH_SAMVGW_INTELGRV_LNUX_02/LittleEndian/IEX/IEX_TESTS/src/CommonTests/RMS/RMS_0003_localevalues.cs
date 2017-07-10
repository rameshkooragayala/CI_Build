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


    public class RMS_0003_LocaleValues : _Test
    {

        [ThreadStatic]

        static string cpeId;
        static string browserTabControlId;
        static string cityCodeBrowserID;
        static string countryCodeBrowserID;
        static string regionCodeBrowserID;
        static string expectedCityCode;
        static string expectedCountryCode;
        static string expectedRegionCode;
        static string obtainedCityCode;
        static string obtainedCountryCode;
        static string obtainedRegionCode;
        static FirefoxDriver driver;



        private static _Platform CL;
        private static bool isFail = false;
        private static int noOfFailures = 0;
        #region Create Structure

        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get the CPE ID from the INI file and locale values from test INI");
            this.AddStep(new Step1(), "Step 1: GO TO PANORAMA WEBPAGE LOGIN AND ENTER BOXID AND SEARCH");
            this.AddStep(new Step2(), "Step 2: Fetching the locale values from Panorama and comparing the same with CPE value");
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

                //Fetching the CPE ID from environment INI file

                cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
                if (cpeId == null)
                {
                    FailStep(CL, "Failed to fetch  cpeId from ini File.");
                }
                else
                {
                    LogComment(CL, "cpeId fetched is : " + cpeId);

                }

                //Fetching the Panorama navigation tab ID

                browserTabControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
                if (browserTabControlId == null)
                {
                    FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
                }
                else
                {
                    LogComment(CL, "BrowserTabControlId fetched is : " + browserTabControlId);

                }


                //FETCHING THE PARAMETER NAME TO BE SEARCHED IN THE PANARAMA WEBPAGE

                cityCodeBrowserID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "CITY_CODE");
                if (cityCodeBrowserID == null)
                {
                    FailStep(CL, "Failed to fetch citycodeId from ini File.");
                }
                else
                {
                    LogComment(CL, "citycodeId fetched is : " + cityCodeBrowserID);

                }
                countryCodeBrowserID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "COUNTRY_CODE");
                if (countryCodeBrowserID == null)
                {
                    FailStep(CL, "Failed to fetch  countrycodeId from ini File.");
                }
                else
                {
                    LogComment(CL, "CountrycodeID fetched is : " + countryCodeBrowserID);

                }
                regionCodeBrowserID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "REGION_CODE");
                if (regionCodeBrowserID == null)
                {
                    FailStep(CL, "Failed to fetch  regioncodeId from ini File.");
                }
                else
                {
                    LogComment(CL, "regioncodeID fetched is : " + regionCodeBrowserID);

                }


                //fetching values from environment ini files
                expectedCityCode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "citycode");
                if (expectedCityCode == null)
                {
                    FailStep(CL, "Failed to fetch citycode from ini File.");
                }
                else
                {
                    LogComment(CL, "citycode fetched is : " + expectedCityCode);

                }

                expectedCountryCode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "countrycode");
                if (expectedCountryCode == null)
                {
                    FailStep(CL, "Failed to fetch  countrycode from ini File.");
                }
                else
                {
                    LogComment(CL, "countrycode fetched is : " + expectedCountryCode);

                }
                expectedRegionCode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "regioncode");
                if (expectedRegionCode == null)
                {
                    FailStep(CL, "Failed to fetch regioncode from ini File.");
                }
                else
                {
                    LogComment(CL, "regioncode fetched is : " + expectedRegionCode);

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
                res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserTabControlId);
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

                //City code

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

                obtainedCityCode = CL.EA.UI.RMS.GetParameterValues(driver, cityCodeBrowserID);

                if (obtainedCityCode == null)
                {
                    FailStep(CL, "Failed to fetch the citycode value from Panorama");

                }

                LogCommentImportant(CL, "obtained citycode from panorama is " + obtainedCityCode);

                //Comparing the Panorama value with Expected Value

                if (expectedCityCode != obtainedCityCode)
                {

                    LogComment(CL, "Both The values  " + expectedCityCode + "And" + obtainedCityCode + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedCityCode + "And Fetched value From INI File" + expectedCityCode + "Are Equal");
                }
                CL.IEX.Wait(5);

                //Country code

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

                obtainedCountryCode = CL.EA.UI.RMS.GetParameterValues(driver, countryCodeBrowserID);

                if (obtainedCountryCode == null)
                {
                    FailStep(CL, "Failed to fetch the countrycode value from Panorama");

                }

                LogCommentImportant(CL, "obtained countrycode from panorama is " + obtainedCountryCode);

                //Comparing the Panorama value with Expected Value

                if (expectedCountryCode != obtainedCountryCode)
                {

                    LogComment(CL, "Both The values Expected " + expectedCountryCode + "And Obtained " + obtainedCountryCode + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page " + obtainedCountryCode + " And Fetched value From INI File" + expectedCountryCode + " Are Equal");
                }
                CL.IEX.Wait(5);

                //region code

                //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

                obtainedRegionCode = CL.EA.UI.RMS.GetParameterValues(driver, regionCodeBrowserID);

                if (obtainedRegionCode == null)
                {
                    FailStep(CL, "Failed to fetch the regioncode value from Panorama");

                }

                LogCommentImportant(CL, "obtained regioncode from panorama is " + obtainedRegionCode);

                //Comparing the Panoramavalue with environment INI

                if (expectedRegionCode != obtainedRegionCode)
                {

                    LogComment(CL, "Both The values  Expected Region Value" + expectedRegionCode + " And Obtained Region Value " + obtainedRegionCode + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + obtainedRegionCode + " And Fetched value From INI File " + expectedCountryCode + "Are Equal");
                }
                CL.IEX.Wait(5);

                //Displays in final The number of verification failed If any
                if (isFail)
                {
                    FailStep(CL, "Number of validations failed " + noOfFailures + "...Please see above for Failure reasons");
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


