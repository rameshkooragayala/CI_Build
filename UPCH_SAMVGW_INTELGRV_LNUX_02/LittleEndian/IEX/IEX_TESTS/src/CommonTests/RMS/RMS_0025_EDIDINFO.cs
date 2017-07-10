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


public class RMS_0025 : _Test
{

    [ThreadStatic]

    static string cpeId;
    static string browserTabControlId;
    static string TvmodelId;
    static string ObtainedTvmodel;
    static string ExpectedTvmodel;
    static string VendorId;
    static string ObtainedVendorId;
    static string ExpectedVendorId;
    static string SupportedResolutionsId;
    static string ObtainedSupportedResolutions;
    static string ExpectedSupportedResolutions;
    static string CurrentResolutionsId;
    static string ObtainedCurrentResolution;
    static string ExpectedCurrentResoltion;
    static string HdmiStatusId;
    static string ObtainedHdmistatus;
    static string ExpectedHdmistatus;
    static string HdmiSupportedId;
    static string ObtainedHdmiSupported;
    static string ExpectedHdmiSupported;
    static string HdmiSuportedAudioModesId;
    static string ObtainedHdmisupportedaudiomodes;
    static string ExpectedHdmisupportedaudiomodes;
    static string HdcpstatusId;
    static string ExpectedHdcpstatus;
    static string ObtainedHdcpstatus;
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
        this.AddStep(new Step2(), "Step 2: Fetching the HDmi related parameter values from panorama and comparing with expected values");
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

            TvmodelId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "TVMODEL");
            if (TvmodelId == null)
            {
                FailStep(CL, "Failed to fetch TvmodelId from ini File.");
            }
            else
            {
                LogComment(CL, "TvmodelId fetched is : " + TvmodelId);

            }

            VendorId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "VENDORID");
            if (VendorId == null)
            {
                FailStep(CL, "Failed to fetch VendorID from ini File.");
            }
            else
            {
                LogComment(CL, "VendorID fetched is : " + VendorId);

            }

            SupportedResolutionsId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "SUPPORTEDRESOLUTIONS");
            if (SupportedResolutionsId == null)
            {
                FailStep(CL, "Failed to fetch Supported resolution from ini File.");
            }
            else
            {
                LogComment(CL, "VendorID fetched is : " + SupportedResolutionsId);

            }

            CurrentResolutionsId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "CURRENTRESOLUTION");
            if (CurrentResolutionsId == null)
            {
                FailStep(CL, "Failed to fetch Current resolution from ini File.");
            }
            else
            {
                LogComment(CL, "Current resolution fetched is : " + CurrentResolutionsId);

            }

            HdmiStatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "HDMISTATUS");
            if (HdmiStatusId == null)
            {
                FailStep(CL, "Failed to fetch HDMI status from ini File.");
            }
            else
            {
                LogComment(CL, "HDMI status fetched is : " + HdmiStatusId);

            }

            HdmiSupportedId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "HDMISUPPORTED");
            if (HdmiSupportedId == null)
            {
                FailStep(CL, "Failed to fetch HDMI supported ID from ini File.");
            }
            else
            {
                LogComment(CL, "HDMI status fetched is : " + HdmiSupportedId);

            }


            HdmiSuportedAudioModesId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "HDMISUPPORTEDAUDIOMODES");
            if (HdmiSuportedAudioModesId == null)
            {
                FailStep(CL, "Failed to fetch HDMI supported audio mode ID from ini File.");
            }
            else
            {
                LogComment(CL, "HDMI status fetched is : " + HdmiSuportedAudioModesId);

            }

            HdcpstatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "USER_SETTINGS_PARAMS", "HDCPSTATUS");
            if (HdmiSuportedAudioModesId == null)
            {
                FailStep(CL, "Failed to fetch HDMI supported audio mode ID from ini File.");
            }
            else
            {
                LogComment(CL, "HDMI status fetched is : " + HdmiSuportedAudioModesId);

            }
            //fetching values from environment ini files

            ExpectedTvmodel = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "TVMODEL");
            if (ExpectedTvmodel == null)
            {
                FailStep(CL, "Failed to fetch TV model from ini File.");
            }
            else
            {
                LogComment(CL, "TV model fetched is : " + ExpectedTvmodel);

            }

            ExpectedVendorId = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "VENDORID");
            if (ExpectedVendorId == null)
            {
                FailStep(CL, "Failed to fetch Vendor ID from ini File.");
            }
            else
            {
                LogComment(CL, "countrycode fetched is : " + ExpectedVendorId);

            }

            ExpectedSupportedResolutions = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "SUPPORTEDRESOLUTIONS");
            if (ExpectedSupportedResolutions == null)
            {
                FailStep(CL, "Failed to fetch expected resolutions from ini File.");
            }
            else
            {
                LogComment(CL, "Supported resolution fetched is : " + ExpectedSupportedResolutions);

            }

            ExpectedCurrentResoltion = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "CURRENTRESOLUTION");
            if (ExpectedCurrentResoltion == null)
            {
                FailStep(CL, "Failed to fetch current resolutions from ini File.");
            }
            else
            {
                LogComment(CL, "Current resolution fetched is : " + ExpectedCurrentResoltion);

            }

            ExpectedHdmistatus = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "HDMISTATUS");
            if (ExpectedHdmistatus == null)
            {
                FailStep(CL, "Failed to fetch HDMI status from ini File.");
            }
            else
            {
                LogComment(CL, "HDMI status fetched is : " + ExpectedHdmistatus);

            }

            ExpectedHdmisupportedaudiomodes = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "HDMISUPPORTEDAUDIOMODES");
            if (ExpectedHdmisupportedaudiomodes == null)
            {
                FailStep(CL, "Failed to fetch HDMI supported audio modes from ini File.");
            }
            else
            {
                LogComment(CL, "HDMI supported status fetched is : " + ExpectedHdmisupportedaudiomodes);

            }

            ExpectedHdcpstatus = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "HDCPSTATUS");
            if (ExpectedHdcpstatus == null)
            {
                FailStep(CL, "Failed to fetch HDCP status from ini File.");
            }
            else
            {
                LogComment(CL, "HDCP status fetched is : " + ExpectedHdcpstatus);

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

            //TV model

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            ObtainedTvmodel = CL.EA.UI.RMS.GetParameterValues(driver, TvmodelId);

            if (ObtainedTvmodel == null)
            {
                LogComment(CL, "Failed to fetch the TV model value from Panorama");

            }

            LogCommentImportant(CL, "obtained TV model from panorama is " + ObtainedTvmodel);

            //Comparing the Panorama value with Expected Value

            if (ExpectedTvmodel != ObtainedTvmodel)
            {

                LogComment(CL, "Both The values  " + ExpectedTvmodel + "And" + ObtainedTvmodel + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + ObtainedTvmodel + "And Fetched value From INI File" + ExpectedTvmodel + "Are Equal");
            }
            CL.IEX.Wait(5);

            //Vendor ID

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            ObtainedVendorId = CL.EA.UI.RMS.GetParameterValues(driver, VendorId);

            if (ObtainedVendorId == null)
            {
                LogComment(CL, "Failed to fetch the vendor ID value from Panorama");

            }

            LogCommentImportant(CL, "obtained vendor ID from panorama is " + ObtainedVendorId);

            //Comparing the Panorama value with Expected Value

            if (ExpectedVendorId != ObtainedVendorId)
            {

                LogComment(CL, "Both The values Expected " + ExpectedVendorId + "And Obtained " + ObtainedVendorId + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page " + ObtainedVendorId + " And Fetched value From INI File" + ExpectedVendorId + " Are Equal");
            }
            CL.IEX.Wait(5);

            //Supported resolution

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            ObtainedSupportedResolutions = CL.EA.UI.RMS.GetParameterValues(driver, SupportedResolutionsId);

            if (ObtainedSupportedResolutions == null)
            {
                LogComment(CL, "Failed to fetch the Supported resolution value from Panorama");

            }

            LogCommentImportant(CL, "obtained supported resolution from panorama is " + ObtainedSupportedResolutions);

            //Comparing the Panoramavalue with environment INI

            if (ExpectedSupportedResolutions != ObtainedSupportedResolutions)
            {

                LogComment(CL, "Both The values  Expected supported reolutions Value" + ExpectedSupportedResolutions + " And Obtained supported resolutions " + ObtainedSupportedResolutions + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + ObtainedSupportedResolutions + " And Fetched value From INI File " + ExpectedSupportedResolutions + "Are Equal");
            }

            CL.IEX.Wait(5);

            driver.Navigate().Refresh();


            //Current resoltution


            ObtainedCurrentResolution = CL.EA.UI.RMS.GetParameterValues(driver, CurrentResolutionsId);

            if (ObtainedCurrentResolution == null)
            {
                LogComment(CL, "Failed to fetch the current resolution value from Panorama");

            }

            LogCommentImportant(CL, "obtained current resolution value from panorama is " + ObtainedCurrentResolution);

            //Comparing the Panoramavalue with environment INI

            if (ExpectedCurrentResoltion != ObtainedCurrentResolution)
            {

                LogComment(CL, "Both The values  Expected current resolution" + ExpectedCurrentResoltion + " And Obtained current resolution " + ObtainedCurrentResolution + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + ObtainedCurrentResolution + " And Fetched value From INI File " + ExpectedCurrentResoltion + "Are Equal");
            }
            CL.IEX.Wait(5);

            //HDMI status

            ObtainedHdmistatus = CL.EA.UI.RMS.GetParameterValues(driver, HdmiStatusId);

            if (ObtainedHdmistatus == null)
            {
                LogComment(CL, "Failed to fetch the HDMI status value from Panorama");

            }

            LogCommentImportant(CL, "obtained HDMI status value from panorama is " + ObtainedHdmistatus);

            //Comparing the Panoramavalue with environment INI

            if (ExpectedHdmistatus != ObtainedHdmistatus)
            {

                LogComment(CL, "Both The values  Expected Region Value" + ExpectedHdmistatus + " And Obtained Region Value " + ObtainedHdmistatus + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + ObtainedHdmistatus + " And Fetched value From INI File " + ExpectedHdmistatus + "Are Equal");
            }
            CL.IEX.Wait(5);


            //HDMI supported audio modes
            ObtainedHdmisupportedaudiomodes = CL.EA.UI.RMS.GetParameterValues(driver, HdmiSuportedAudioModesId);

            if (ObtainedHdmisupportedaudiomodes == null)
            {
                LogComment(CL, "Failed to fetch the HDMI supported audio modes value from Panorama");

            }

            LogCommentImportant(CL, "obtained HDMI audio supported modes from panorama is " + ObtainedHdmisupportedaudiomodes);

            //Comparing the Panoramavalue with environment INI

            if (ExpectedHdmisupportedaudiomodes != ObtainedHdmisupportedaudiomodes)
            {

                LogComment(CL, "Both The values  Expected Region Value" + ExpectedHdmisupportedaudiomodes + " And Obtained Region Value " + ObtainedHdmisupportedaudiomodes + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + ObtainedHdmisupportedaudiomodes + " And Fetched value From INI File " + ExpectedHdmisupportedaudiomodes + "Are Equal");
            }
            CL.IEX.Wait(5);


            //HDCP status

            ObtainedHdmiSupported = CL.EA.UI.RMS.GetParameterValues(driver, HdmiSupportedId);

            if (ObtainedHdmiSupported != "YES")
            {
                LogComment(CL, "Failed to fetch HDCP status value from Panorama" + ObtainedHdmiSupported);

            }

            else
            {
                LogComment(CL, "Fetched values from panorama is correct" + ObtainedHdmiSupported);
            }
            CL.IEX.Wait(5);


            //HDCP status 
            ObtainedHdcpstatus = CL.EA.UI.RMS.GetParameterValues(driver, HdcpstatusId);

            if (ObtainedHdcpstatus == null)
            {
                LogComment(CL, "Failed to fetch the HDCP value from Panorama");

            }

            LogCommentImportant(CL, "obtained HDCP status value from panorama is " + ObtainedHdcpstatus);

            //Comparing the Panoramavalue with environment INI

            if (ExpectedHdcpstatus != ObtainedHdcpstatus)
            {

                LogComment(CL, "Both The values  Expected HDCP status Value" + ExpectedHdcpstatus + " And Obtained HDCP status Value " + ObtainedHdcpstatus + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + ObtainedHdcpstatus + " And Fetched value From INI File " + ExpectedHdcpstatus + "Are Equal");
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

