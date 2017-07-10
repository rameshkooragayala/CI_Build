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


public class RMS_0022 : _Test
{
    [ThreadStatic]
    static FirefoxDriver driver;
    private static _Platform CL;
    static string timeStamp = "";
    static string cpeId;
    static string pvrRecCount;
    static string expectedRecordCount;
    static int obtainedRecordCount1;
    static int obtainedRecordCount2;
    static int obtainedRecordCount3;
    static string expectedPvrStatus;
    static string obtainedPvrStatus;
    static string browserParameterTabId;
    static string browserPvrRecordCountId;
    static string browserPvrrecordStatusId;
    static int count = 0;
    static string eventName;
    static bool isFail = false;
    private static Service serviceToBeRecorded;
    private static Service service;
    static string eventToBeRecorded = "EVENT_TO_BE_RECORDED";
    #region Create Structure
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
        public const int minTimeBeforeEventEnd = 10; //in minutes
    }
    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get CPE ID and other parameters From ini File");
        this.AddStep(new Step1(), "Step1: Go To Panorama Webpage Login And Enter Boxid And Search");
        this.AddStep(new Step2(), "Step2: Setting the standby setting value from Panorama and compaing the same with CPE  ");
        this.AddStep(new Step3(), "Step3: Comparing the Pvr Record Count And Pvr Status Values Of box and Panorama ");
        this.AddStep(new Step4(), "Step4: Perorm Record and Check the Value of Pvr Record Count Value from panorama Increased");
        this.AddStep(new Step5(), "Step5: Perform Delete Record and Verify the Panorama record event value updated ");
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
            service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }
            res = CL.EA.TuneToChannel(service.LCN,true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service"+service.LCN);
            }
            else
            {
                LogCommentInfo(CL, "Successfully tuned to service "+service.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd,VerifyIsRecordingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service - " + service.LCN);
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Return to Live Viewing");
            }
            else
            {
                LogCommentInfo(CL, "Returned to Live Viewing");
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
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY RECORDINGS");
            //CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            CL.IEX.MilestonesEPG.GetEPGInfo("recordposition", out expectedRecordCount);
             if (expectedRecordCount == "")
            {
                LogComment(CL, "There Are no Recorded Content in the box");
                expectedRecordCount = "0";
            }
            else
            {
                string[] output = expectedRecordCount.Split('/');
                expectedRecordCount = output[1];
                LogCommentInfo(CL, "Expected Record Count Value is" + expectedRecordCount);
            }

            expectedPvrStatus = CL.EA.UI.Utils.GetValueFromEnvironment("PVR_STATUS");
            if (expectedPvrStatus == null)
            {
                FailStep(CL, "Failed to get PvrStatus value from enivironment ");
            }
            else
            {
                LogCommentInfo(CL, "PvrStatus Value fetched from environment file is " + expectedPvrStatus);
            }
            browserPvrRecordCountId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "PVR_SETTINGS_PARAMS", "PVR_RECORDCOUNTID");
            if (browserPvrRecordCountId == null)
            {
                FailStep(CL, "Failed to Get the PvrRecording Browser Id from test ini");
            }
            else
            {
                LogCommentInfo(CL, "BrowserPvrRecordCountId fetched from test ini is" + browserPvrRecordCountId);
            }
            browserPvrrecordStatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "PVR_SETTINGS_PARAMS", "PVR_RECORDSTATUS");
            if (browserPvrrecordStatusId == null)
            {
                FailStep(CL, "Failed to Get the PvrRecordStatus Browser Id from test ini");
            }
            else
            {
                LogCommentInfo(CL, "BrowserPvrRecordStatus fetched from test ini is" + browserPvrrecordStatusId);
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
            LogCommentInfo(CL, "Fetching the PvrRecord Count value from panorama");

            pvrRecCount = CL.EA.UI.RMS.GetParameterValues(driver, browserPvrRecordCountId);
            int.TryParse(pvrRecCount, out obtainedRecordCount1);
            if (obtainedRecordCount1.ToString() == null)
            {
                FailStep(CL, "Failed to fethc the Record Count value from panorama page");
            }
            else
            {
                LogCommentInfo(CL, "Obtained RecordCount value from panorama page is " + obtainedRecordCount1);
            }
            obtainedPvrStatus = CL.EA.UI.RMS.GetParameterValues(driver, browserPvrrecordStatusId);
            if (obtainedPvrStatus == null)
            {
                FailStep(CL, "Failed to fetch the Pvr Status value from panorama page");
            }
            else
            {
                LogCommentInfo(CL, "Obtained pvr status value from panorama page is " + obtainedPvrStatus);
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
            if (expectedRecordCount != obtainedRecordCount1.ToString())
            {
                LogCommentInfo(CL, "Both the Expected and Obtained Pvr Recording count Values Are Not matching");
                count++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both the Expected and Obtained Pvr Recordings count Are equal");
            }
            if (expectedPvrStatus != obtainedPvrStatus)
            {
                LogComment(CL, "Both the expected and obtained pvr Status Values are Not matching");
                count++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both the Expected and Obtained Pvr status  Are equal");
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

            CL.IEX.Wait(4);
            res = CL.EA.TuneToChannel("721", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service 721");
            }
            else
            {
                LogCommentInfo(CL, "Successfully tuned to service 721 ");
            }
            CL.IEX.Wait(2);
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service - " + service.LCN);
            }
          
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Return to Live Viewing");
            }
            else
            {
                LogCommentInfo(CL, "Returned to Live Viewing");
            }

            pvrRecCount = CL.EA.UI.RMS.GetParameterValues(driver, browserPvrRecordCountId);
            int.TryParse(pvrRecCount, out obtainedRecordCount2);
            if (obtainedRecordCount2.ToString() == null)
            {
                FailStep(CL, "Failed to Retrive the Updated RecordIng Count Value from Panorama page");
            }
            else
            {
                LogCommentInfo(CL, "Obtained RecordCount value from panorama page is " + obtainedRecordCount2);
            }


            LogComment(CL, "Comapring the Updated values of Recording count With previous Record Count value");
            if (obtainedRecordCount2 <= obtainedRecordCount1)
            {
                LogCommentInfo(CL, "Both the Expected and Obtained Pvr Recording count Values Are Not matching");
                count++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Both the Expected and Obtained Pvr Recordings count Are equal");
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
            LogCommentInfo(CL, "Deleting a recorded event from myrecording");
            CL.IEX.Wait(2);
            res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded, InReviewBuffer: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Delete Recorded Event");
            }
            else
            {
                LogCommentInfo(CL, "Successfully Deleted the Recorded event");
            }
           
            pvrRecCount = CL.EA.UI.RMS.GetParameterValues(driver, browserPvrRecordCountId);
            int.TryParse(pvrRecCount, out obtainedRecordCount3);
            if (obtainedRecordCount3.ToString() == null)
            {
                FailStep(CL, "Failed to Retrive the Updated RecordIng Count Value from Panorama page");
            }
            else
            {
                LogCommentInfo(CL, "Obtained RecordCount value from panorama page is " + obtainedRecordCount2);
            }


            LogComment(CL, "Comapring the Updated values of Recording count With previous Record Count value");
            if (obtainedRecordCount3 >= obtainedRecordCount2)
            {
                LogCommentInfo(CL, "Obtained Pvr Record Count value is not Decremented by previous value after deleting the recorded event");
                count++;
                isFail = true;
            }
            else
            {
                LogComment(CL, "Obtained Pvr Record Count is Decremented by one from previous value after deleting a recorded event");
            }
            if (isFail)
            {
                FailStep(CL, "Number of validations failed " + count + "...Please see above for Failure reasons");
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
        IEXGateway._IEXResult res;

        //Delete all recordings in archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive(Navigate:true);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }
    }

    #endregion PostExecute

}

