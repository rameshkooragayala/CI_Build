using System;
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


   public class RMS_0030:_Test
    {
        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string timeStamp = "";
        static string cpeId;
        static string browserParameterTabId;
        static string browserSignalStrengthPercentId1;
        static string browserSignalStrengthPercentId2;
        static string browserSignalStrengthPercentId3;
        static string browserSignalStrengthPercentId4;
        static string browserSignalStrengthPercentId5;
        static string browserSignalStrengthPercentId6;

        static string expectedSignalStrengthPercent;
        static string obtainedSignalStrengthPercent1;
        static string obtainedSignalStrengthPercent2;
        static string obtainedSignalStrengthPercent3;
        static string obtainedSignalStrengthPercent4;
        static string obtainedSignalStrengthPercent5;
        static string obtainedSignalStrengthPercent6;
        static string browserSignalQualityPercentID1;
        static string browserSignalQualityPercentID2;
        static string browserSignalQualityPercentID3;
        static string browserSignalQualityPercentID4;
        static string browserSignalQualityPercentID5;
        static string browserSignalQualityPercentID6;
        static string expectedSignalQualityPercent;
        static string obtainedSiganalQualityPercent1;
        static string obtainedSiganalQualityPercent2;
        static string obtainedSiganalQualityPercent3;
        static string obtainedSiganalQualityPercent4;
        static string obtainedSiganalQualityPercent5;
        static string obtainedSiganalQualityPercent6;

        #region CreateStructure
        public override void CreateStructure()
        {
            //Brief Description: 
            //Perform Going To panorama webpage.
            //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
            //Verify With the Box Values.
            this.AddStep(new PreCondition(), "Precondition: Get CPE ID and other parameters From ini File");
            this.AddStep(new Step1(), "Step1: Fetching signal and quality percent values from box and Go To Panorama Webpage Login And Enter Boxid And Search");
            this.AddStep(new Step2(), "Step2: Getting the signal strength percent and quality percent values from Panorama and compaing the same with CPE  ");
             CL = GetClient();
        }
        #endregion CreateStructure
        #region Steps
    #region PreCondition
        private class PreCondition : _Step
        {
            public override void Execute()
            {

                StartStep();

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
                
                browserSignalStrengthPercentId1 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALSTRENGTHPERCENTID1");
                if (browserSignalStrengthPercentId1 == null || browserSignalStrengthPercentId1 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalstrengthpercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalstrengthpercentid from browser ini is"+browserSignalStrengthPercentId1);
                }
                browserSignalStrengthPercentId2 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALSTRENGTHPERCENTID2");
                if (browserSignalStrengthPercentId2 == null || browserSignalStrengthPercentId2 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalstrengthpercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalstrengthpercentid from browser ini is" + browserSignalStrengthPercentId2);
                }
                browserSignalStrengthPercentId3 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALSTRENGTHPERCENTID3");
                if (browserSignalStrengthPercentId3 == null || browserSignalStrengthPercentId3 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalstrengthpercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalstrengthpercentid from browser ini is" + browserSignalStrengthPercentId3);
                }
                browserSignalStrengthPercentId4 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALSTRENGTHPERCENTID4");
                if (browserSignalStrengthPercentId4 == null || browserSignalStrengthPercentId4 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalstrengthpercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalstrengthpercentid from browser ini is" + browserSignalStrengthPercentId4);
                }
                browserSignalStrengthPercentId5 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALSTRENGTHPERCENTID5");
                if (browserSignalStrengthPercentId5 == null || browserSignalStrengthPercentId5 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalstrengthpercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalstrengthpercentid from browser ini is" + browserSignalStrengthPercentId5);
                }
                browserSignalStrengthPercentId6 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALSTRENGTHPERCENTID6");
                if (browserSignalStrengthPercentId6 == null || browserSignalStrengthPercentId6 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalstrengthpercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalstrengthpercentid from browser ini is" + browserSignalStrengthPercentId6);
                }

                browserSignalQualityPercentID1 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID1");
                if (browserSignalQualityPercentID1 == null || browserSignalQualityPercentID1 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID1);
                }
                browserSignalQualityPercentID2 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID2");
                if (browserSignalQualityPercentID2 == null || browserSignalQualityPercentID2 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID2);
                }
                browserSignalQualityPercentID3 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID3");
                if (browserSignalQualityPercentID3 == null || browserSignalQualityPercentID3 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID3);
                }
                browserSignalQualityPercentID3 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID3");
                if (browserSignalQualityPercentID3 == null || browserSignalQualityPercentID3 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID3);
                }
                browserSignalQualityPercentID4 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID4");
                if (browserSignalQualityPercentID4 == null || browserSignalQualityPercentID4 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID4);
                }
                browserSignalQualityPercentID5 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID5");
                if (browserSignalQualityPercentID5 == null || browserSignalQualityPercentID5 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID5);
                }
                browserSignalQualityPercentID6 = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSIGNALQUALITYPERCENTID6");
                if (browserSignalQualityPercentID6 == null || browserSignalQualityPercentID6 == "N/A")
                {
                    FailStep(CL, "Failed to fetcht the singalQualitypercent id fropm browser ini");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained signalqualitypercentid from browser ini is" + browserSignalQualityPercentID6);
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
               
                CL.EA.UI.Utils.NavigateToDiagnostics();
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("signal strength", out expectedSignalStrengthPercent);
                LogCommentInfo(CL, "Obtained signalStrengthPercent from panorama page is"+expectedSignalStrengthPercent);
                CL.IEX.Wait(2);
                CL.IEX.MilestonesEPG.GetEPGInfo("signal quality", out expectedSignalQualityPercent);
                LogCommentInfo(CL, "Obtained signalQualityPercent from panorama page is" + expectedSignalQualityPercent);
                
                driver = new FirefoxDriver();

                CL.IEX.Wait(5);
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
                obtainedSignalStrengthPercent1 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalStrengthPercentId1);
                if (obtainedSignalStrengthPercent1 == null || obtainedSignalStrengthPercent1 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalstrengthpercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signal strengthpercnet value from panorama page is"+obtainedSignalStrengthPercent1);
                }
                obtainedSignalStrengthPercent2 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalStrengthPercentId2);
                if (obtainedSignalStrengthPercent2 == null || obtainedSignalStrengthPercent2 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalstrengthpercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signal strengthpercnet value from panorama page is" + obtainedSignalStrengthPercent2);
                }
                obtainedSignalStrengthPercent3 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalStrengthPercentId3);
                if (obtainedSignalStrengthPercent3 == null || obtainedSignalStrengthPercent3 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalstrengthpercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signal strengthpercnet value from panorama page is" + obtainedSignalStrengthPercent3);
                }
                obtainedSignalStrengthPercent4 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalStrengthPercentId4);
                if (obtainedSignalStrengthPercent4 == null || obtainedSignalStrengthPercent4 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalstrengthpercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signal strengthpercnet value from panorama page is" + obtainedSignalStrengthPercent4);
                }
                obtainedSignalStrengthPercent5 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalStrengthPercentId5);
                if (obtainedSignalStrengthPercent5 == null || obtainedSignalStrengthPercent5 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalstrengthpercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signal strengthpercnet value from panorama page is" + obtainedSignalStrengthPercent5);
                }
                obtainedSignalStrengthPercent6 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalStrengthPercentId6);
                if (obtainedSignalStrengthPercent6 == null || obtainedSignalStrengthPercent6 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalstrengthpercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signal strengthpercnet value from panorama page is" + obtainedSignalStrengthPercent6);
                }
                obtainedSiganalQualityPercent1 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalQualityPercentID1);
                if (obtainedSiganalQualityPercent1 == null || obtainedSiganalQualityPercent1 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalQualitypercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signalQualitypercnet value from panorama page is" + obtainedSiganalQualityPercent1);
                }
                obtainedSiganalQualityPercent2 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalQualityPercentID2);
                if (obtainedSiganalQualityPercent2 == null || obtainedSiganalQualityPercent2 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalQualitypercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signalQualitypercnet value from panorama page is" + obtainedSiganalQualityPercent2);
                }
                obtainedSiganalQualityPercent3 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalQualityPercentID3);
                if (obtainedSiganalQualityPercent3 == null || obtainedSiganalQualityPercent3 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalQualitypercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signalQualitypercnet value from panorama page is" + obtainedSiganalQualityPercent3);
                }
                obtainedSiganalQualityPercent4 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalQualityPercentID4);
                if (obtainedSiganalQualityPercent4 == null || obtainedSiganalQualityPercent4 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalQualitypercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signalQualitypercnet value from panorama page is" + obtainedSiganalQualityPercent4);
                }
                obtainedSiganalQualityPercent5 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalQualityPercentID5);
                if (obtainedSiganalQualityPercent5 == null || obtainedSiganalQualityPercent5 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalQualitypercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signalQualitypercnet value from panorama page is" + obtainedSiganalQualityPercent5);
                }
                obtainedSiganalQualityPercent6 = CL.EA.UI.RMS.GetParameterValues(driver, browserSignalQualityPercentID6);
                if (obtainedSiganalQualityPercent6 == null || obtainedSiganalQualityPercent6 == "N/A")
                {
                    FailStep(CL, "Faild to fetch the signalQualitypercent value from panorama page");
                }
                else
                {
                    LogCommentInfo(CL, "obtained signalQualitypercnet value from panorama page is" + obtainedSiganalQualityPercent6);
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
                if (expectedSignalStrengthPercent == obtainedSignalStrengthPercent1)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 1 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 1 are not equal");
                }
                if (expectedSignalStrengthPercent == obtainedSignalStrengthPercent2)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 2 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 2 are not equal");
                }
                if (expectedSignalStrengthPercent == obtainedSignalStrengthPercent3)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 3 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 3 are not equal");
                }
                if (expectedSignalStrengthPercent == obtainedSignalStrengthPercent3)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 4 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 4 are not equal");
                }
                if (expectedSignalStrengthPercent == obtainedSignalStrengthPercent4)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 5 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 5 are not equal");
                }
                if (expectedSignalStrengthPercent == obtainedSignalStrengthPercent5)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 6 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalstrengthpercent values on tuner 6 are not equal");
                }
                if (expectedSignalQualityPercent == obtainedSiganalQualityPercent1)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 1 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 1 are not equal");
                }
                if (expectedSignalQualityPercent == obtainedSiganalQualityPercent2)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 2 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 2 are not equal");
                }
                if (expectedSignalQualityPercent == obtainedSiganalQualityPercent3)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 3 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 3 are not equal");
                }
                if (expectedSignalQualityPercent == obtainedSiganalQualityPercent4)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 4 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 4 are not equal");
                }
                if (expectedSignalQualityPercent == obtainedSiganalQualityPercent5)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 5 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 5 are not equal");
                }
                if (expectedSignalQualityPercent == obtainedSiganalQualityPercent6)
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 6 are equal");
                }
                else
                {
                    LogCommentInfo(CL, "Both expected and obtained signalQualitypercent values on tuner 6 are not equal");
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

