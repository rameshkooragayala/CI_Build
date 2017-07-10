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


   public class RMS_RFTuner_Status : _Test
    {

        [ThreadStatic]
        static FirefoxDriver driver;
        private static _Platform CL;
        static string browserParameterTabId;
        static string cpeId;
        static string quickActionRebootId;
        static string quickActionConfirmId;
        static string ShutDownMilestone;
        static ArrayList list = new ArrayList();
        static bool isShutDownMilestoneRecieved;
        static int Time_In_Standby = 60;

       //tuner status
        static string browserRFTuner1StatusId;
        static string browserRFTuner2StatusId;
        static string browserRFTuner3StatusId;
        static string browserRFTuner4StatusId;
        static string browserRFTuner5StatusId;
        static string browserRFTuner6StatusId;
        static string obtainedRFTuner1Status;
        static string obtainedRFTuner2Status;
        static string obtainedRFTuner3Status;
        static string obtainedRFTuner4Status;
        static string obtainedRFTuner5Status;
        static string obtainedRFTuner6Status;
        static int isLocked = 0;
        static string eventName;
        static bool isFail = false;
        private static Service serviceToBeRecorded;
        private static Service service;
        static string eventToBeRecorded = "EVENT_TO_BE_RECORDED";

       //signal strength percent
      
       //signal quality percent
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
            this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
            this.AddStep(new Step1(), "Step1: Reboot the cpe and put in standby");
            this.AddStep(new Step2(), "Step2: While in stanby checking the value of rftuner staus from the panorama page ");
            this.AddStep(new Step3(), "Step3:coming out from the standby ");
            this.AddStep(new Step4(), "Step4:checking the rf tuner value from panorama page");
            this.AddStep(new Step5(), "Step5:record an event on a service by tuning to that service");
            this.AddStep(new Step6(), "Step6:check the tuner value from panorama page again");
            this.AddStep(new Step7(), "Step7:deleting the recorded event from the box");
            this.AddStep(new Step8(), "Step8:checking the rf tuner values again and comparing according to the expected result");
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
                browserRFTuner1StatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSTATUSID1");
                if (browserRFTuner1StatusId == null)
                {
                    FailStep(CL, "Failed to fetch  browserRFTuner1StatusId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserRFTuner1StatusId fetched is : " + browserRFTuner1StatusId);

                }
                browserRFTuner2StatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSTATUSID2");
                if (browserRFTuner2StatusId == null)
                {
                    FailStep(CL, "Failed to fetch  browserRFTuner2StatusId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserRFTuner2StatusId fetched is : " + browserRFTuner2StatusId);

                }

                browserRFTuner3StatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSTATUSID3");
                if (browserRFTuner3StatusId == null)
                {
                    FailStep(CL, "Failed to fetch  browserRFTuner3StatusId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserRFTuner3StatusId fetched is : " + browserRFTuner3StatusId);

                }
                browserRFTuner4StatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSTATUSID4");
                if (browserRFTuner4StatusId == null)
                {
                    FailStep(CL, "Failed to fetch  browserRFTuner4StatusId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserRFTuner4StatusId fetched is : " + browserRFTuner4StatusId);

                }
                browserRFTuner5StatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSTATUSID5");
                if (browserRFTuner5StatusId == null)
                {
                    FailStep(CL, "Failed to fetch  browserRFTuner5StatusId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserRFTuner5StatusId fetched is : " + browserRFTuner5StatusId);

                }
                browserRFTuner6StatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "RFTUNER_SETTINGS", "RFTUNERSTATUSID6");
                if (browserRFTuner6StatusId == null)
                {
                    FailStep(CL, "Failed to fetch  browserRFTuner6StatusId from ini File.");
                }
                else
                {
                    LogComment(CL, "browserRFTuner6StatusId fetched is : " + browserRFTuner6StatusId);

                }
				
                //Get Values From ini File
                string powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
                if (string.IsNullOrEmpty(powerMode))
                {
                    LogCommentFailure(CL, "Unable to fetch the power mode value from test ini file");
                }
                //set to any power mode
                res = CL.EA.STBSettings.SetPowerMode(powerMode);
                if (!(res.CommandSucceeded))
                {
                    LogCommentFailure(CL, "Failed to set the power mode option" + powerMode);
                }
                else
                {
                    LogCommentInfo(CL, "Power mode set Successfully");
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
                LogCommentInfo(CL, "Reboot the box and putting in light standby");
                //Powercycle the CPE           
                res = CL.EA.PowerCycle(0, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }
                //Stay in Standby for a few seconds
                int Time_In_Standby = 100;
                CL.IEX.Wait(Time_In_Standby);
               
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
                obtainedRFTuner1Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner1StatusId);
                if (obtainedRFTuner1Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner1Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner1Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner1Status value from panorama page is " + obtainedRFTuner1Status);
                    }


                }
                obtainedRFTuner2Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner2StatusId);
                if (obtainedRFTuner2Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner2Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner2Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner2Status value from panorama page is " + obtainedRFTuner2Status);
                    }
                }
                obtainedRFTuner3Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner3StatusId);
                if (obtainedRFTuner3Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner3Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner3Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner3Status value from panorama page is " + obtainedRFTuner3Status);
                    }
                }
                obtainedRFTuner4Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner4StatusId);
                if (obtainedRFTuner4Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner4Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner4Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner4Status value from panorama page is " + obtainedRFTuner4Status);
                    }
                }
                obtainedRFTuner5Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner5StatusId);
                if (obtainedRFTuner5Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner5Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner5Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner5Status value from panorama page is " + obtainedRFTuner5Status);
                    }
                }
                obtainedRFTuner6Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner6StatusId);
                if (obtainedRFTuner6Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner6Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner6Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner6Status value from panorama page is " + obtainedRFTuner6Status);
                    }
                }
                if (isLocked == 1)
                {
                    LogCommentInfo(CL, "only one tuner is locked state and rest all are in 'not in use' state");
                    isLocked = 0;
                }
                else
                {
                    isLocked = 0;
                    FailStep(CL, "Failed because more than one rf tuner is in 'locked' state");
                    
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
                LogCommentInfo(CL, "Coming out from standby");
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Exit Standby ");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully came out from standby");
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
                try
                {
                    CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                    CL.IEX.Wait(8);
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogCommentInfo(CL, "Fetching the RF tuner Status values from panorama page again");
                obtainedRFTuner1Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner1StatusId);
                if (obtainedRFTuner1Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner1Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner1Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner1Status value from panorama page is " + obtainedRFTuner1Status);
                    }


                }
                obtainedRFTuner2Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner2StatusId);
                if (obtainedRFTuner2Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner2Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner2Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner2Status value from panorama page is " + obtainedRFTuner2Status);
                    }
                }
                obtainedRFTuner3Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner3StatusId);
                if (obtainedRFTuner3Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner3Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner3Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner3Status value from panorama page is " + obtainedRFTuner3Status);
                    }
                }
                obtainedRFTuner4Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner4StatusId);
                if (obtainedRFTuner4Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner4Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner4Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner4Status value from panorama page is " + obtainedRFTuner4Status);
                    }
                }
                obtainedRFTuner5Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner5StatusId);
                if (obtainedRFTuner5Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner5Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner5Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner5Status value from panorama page is " + obtainedRFTuner5Status);
                    }
                }
                obtainedRFTuner6Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner6StatusId);
                if (obtainedRFTuner6Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner6Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner6Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner6Status value from panorama page is " + obtainedRFTuner6Status);
                    }
                }
                if (isLocked == 1)
                {
                    LogCommentInfo(CL, "only one tuner is locked state and rest all are in 'not in use' state");
                }
                else
                {
                    FailStep(CL, "Failed because more than one rf tuner is in locked state");
                }
                if (isLocked == 3)
                {
                    LogCommentInfo(CL, "only 3 tuners are in locked state and rest all are in 'not in use' state");
                    isLocked = 0;
                }
                else
                {
                    isLocked = 0;
                    FailStep(CL, "Failed because more than 3 rf tuners are in 'locked' state");

                }
                PassStep();
            }
        }
       #endregion Step4;
        #region Step5
        private class Step5 : _Step
        {
            public override void Execute()
            {
                StartStep();
                LogCommentInfo(CL, "Recording the episode on the live channel");
                res = CL.EA.TuneToChannel("720");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to service 720");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully tuned to service 720");
                }
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
                res = CL.EA.TuneToChannel("601");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to service 601");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully tuned to service 601 ");
                }
                PassStep();
            }
        }
        #endregion Step5
        #region Step6
        private class Step6 : _Step
        {
            public override void Execute()
            {
                StartStep();
                try
                {
                    CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                    CL.IEX.Wait(8);
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogCommentInfo(CL, "Fetching the RF tuner Status values from panorama page again");
                obtainedRFTuner1Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner1StatusId);
                if (obtainedRFTuner1Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner1Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner1Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner1Status value from panorama page is " + obtainedRFTuner1Status);
                    }


                }
                obtainedRFTuner2Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner2StatusId);
                if (obtainedRFTuner2Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner2Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner2Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner2Status value from panorama page is " + obtainedRFTuner2Status);
                    }
                }
                obtainedRFTuner3Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner3StatusId);
                if (obtainedRFTuner3Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner3Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner3Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner3Status value from panorama page is " + obtainedRFTuner3Status);
                    }
                }
                obtainedRFTuner4Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner4StatusId);
                if (obtainedRFTuner4Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner4Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner4Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner4Status value from panorama page is " + obtainedRFTuner4Status);
                    }
                }
                obtainedRFTuner5Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner5StatusId);
                if (obtainedRFTuner5Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner5Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner5Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner5Status value from panorama page is " + obtainedRFTuner5Status);
                    }
                }
                obtainedRFTuner6Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner6StatusId);
                if (obtainedRFTuner6Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner6Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner6Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner6Status value from panorama page is " + obtainedRFTuner6Status);
                    }
                }
                if (isLocked == 1)
                {
                    LogCommentInfo(CL, "only one tuner is locked state and rest all are in 'not in use' state");
                }
                else
                {
                    FailStep(CL, "Failed because more than one rf tuner is in locked state");
                }
                if (isLocked == 4)
                {
                    LogCommentInfo(CL, "only 4 tuners are in locked state and rest all are in 'not in use' state");
                    isLocked = 0;
                }
                else
                {
                    isLocked = 0;
                    FailStep(CL, "Failed because more than 4 rf tuners are in 'locked' state");

                }
                PassStep();
            }
        }
        #endregion Step6
        #region Step7
        private class Step7 : _Step
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
                res = CL.EA.TuneToChannel("720");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to service 720");
                }
                else
                {
                    LogCommentInfo(CL, "Successfully tuned to service 720");
                }
                PassStep();
            }
        }
        #endregion Step7
        #region Step8
        private class Step8 : _Step
        {
            public override void Execute()
            {
                StartStep();
                try
                {
                    CL.EA.UI.RMS.EnterCpeId(driver, cpeId);
                    CL.IEX.Wait(8);
                    CL.EA.UI.RMS.SelectTab(driver, browserParameterTabId);

                }
                catch (Exception ex)
                {
                    FailStep(CL, ex.Message);
                }
                LogCommentInfo(CL, "Fetching the RF tuner Status values from panorama page again");
                obtainedRFTuner1Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner1StatusId);
                if (obtainedRFTuner1Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner1Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner1Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner1Status value from panorama page is " + obtainedRFTuner1Status);
                    }


                }
                obtainedRFTuner2Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner2StatusId);
                if (obtainedRFTuner2Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner2Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner2Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner2Status value from panorama page is " + obtainedRFTuner2Status);
                    }
                }
                obtainedRFTuner3Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner3StatusId);
                if (obtainedRFTuner3Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner3Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner3Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner3Status value from panorama page is " + obtainedRFTuner3Status);
                    }
                }
                obtainedRFTuner4Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner4StatusId);
                if (obtainedRFTuner4Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner4Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner4Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner4Status value from panorama page is " + obtainedRFTuner4Status);
                    }
                }
                obtainedRFTuner5Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner5StatusId);
                if (obtainedRFTuner5Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner5Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner5Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner5Status value from panorama page is " + obtainedRFTuner5Status);
                    }
                }
                obtainedRFTuner6Status = CL.EA.UI.RMS.GetParameterValues(driver, browserRFTuner6StatusId);
                if (obtainedRFTuner6Status == null)
                {
                    FailStep(CL, "Failed to fetch the RFTuner6Status value from panorama page");
                }
                else
                {
                    if (obtainedRFTuner6Status == "Locked")
                    {
                        isLocked++;
                        LogCommentInfo(CL, "Obtained RFTuner6Status value from panorama page is " + obtainedRFTuner6Status);
                    }
                }
                if (isLocked == 1)
                {
                    LogCommentInfo(CL, "only one tuner is locked state and rest all are in 'not in use' state");
                }
                else
                {
                    FailStep(CL, "Failed because more than one rf tuner is in locked state");
                }
                if (isLocked == 3)
                {
                    LogCommentInfo(CL, "only 3 tuners are in locked state and rest all are in 'not in use' state");
                    isLocked = 0;
                }
                else
                {
                    isLocked = 0;
                    FailStep(CL, "Failed because more than 3 rf tuners are in 'locked' state");

                }
                PassStep();
            }
        }
        #endregion Step8
        #endregion Steps

        #region PostExecute
        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute
    }

