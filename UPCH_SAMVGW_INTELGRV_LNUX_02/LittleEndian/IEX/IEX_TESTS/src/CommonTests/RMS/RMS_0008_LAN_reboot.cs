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


public class RMS_0008 : _Test
{

    [ThreadStatic]
    static FirefoxDriver driver;
    static string cpeId;
    static string BrowserTabControlId;
    static string totalBytesReceivedId;
    static string totalBytesSentId;
    static string totalPacketsReceivedId;
    static string totalPacketsSentId;
    static string totalBytesReceivedValue1;
    static string totalBytesSentValue1;
    static string totalPacketsReceivedValue1;
    static string totalPacketsSentValue1;
    static string totalBytesReceivedValue2;
    static string totalBytesSentValue2;
    static string totalPacketsReceivedValue2;
    static string totalPacketsSentValue2;
    static string totalBytesReceivedValue3;
    static string totalBytesSentValue3;
    static string totalPacketsReceivedValue3;
    static string totalPacketsSentValue3;
    static string obtainedLedName1;
    static string obtainedLedStatus1;
    static string obtainedLedName2;
    static string obtainedLedStatus2;
    static string ledName1ID;
    static string ledName2ID;
    static string ledStatus1ID;
    static string ledStatus2ID;
    private static _Platform CL;
    private static bool isFail = false;
    private static int noOfFailures = 0;
    private static Service service;
    static string eventToBeRecorded = "EVENT_TO_BE_RECORDED"; //The event to be recorded


    //constants to be used

    private static class Constants
    {
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
        public const int minTimeBeforeEventEnd = 5; //in minutes
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: 
        //Perform Going To panorama webpage.
        //Login To The Website And Enter the CPEId From Environment ini file And Get the Values of Parameters.
        //Verify With the Box Values.
        this.AddStep(new PreCondition(), "Precondition: Get the CPE ID from the INI file and locale values from test INI");
        this.AddStep(new Step1(), "Step 1: GO TO PANORAMA WEBPAGE LOGIN AND ENTER BOXID AND SEARCH");
        this.AddStep(new Step2(), "Step 2: Fetching the LAN values from Panorama and comparing the same with previous value");
        this.AddStep(new Step3(), "Step 3: Fetching the Power LED staus values from Panorama and comparing the same with expected value");
        this.AddStep(new Step4(), "Step 4: Fetching the LAN values from Panorama and comparing the same with previous value");
        this.AddStep(new Step5(), "Step 5: Fetching the REC LED staus values from Panorama and comparing the same with expected value");
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

            //Fetch service to be recorded

            service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }


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

            //Fetching the Panorama navigation tab ID's

            BrowserTabControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
            if (BrowserTabControlId == null)
            {
                FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
            }
            else
            {
                LogComment(CL, "BrowserTabControlId fetched is : " + BrowserTabControlId);

            }


            //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMA WEBPAGE
            totalBytesReceivedId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "TOTALBYTESRECEIVED");
            if (totalBytesReceivedId == null)
            {
                FailStep(CL, "Failed to fetch totalBytesReceivedId from ini File.");
            }
            else
            {
                LogComment(CL, "totalBytesReceivedId fetched is : " + totalBytesReceivedId);

            }
            totalBytesSentId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "TOTALBYTESSENT");
            if (totalBytesSentId == null)
            {
                FailStep(CL, "Failed to fetch  totalBytesSentId from ini File.");
            }
            else
            {
                LogComment(CL, "totalBytesSentId fetched is : " + totalBytesSentId);

            }
            totalPacketsReceivedId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "TOTALPACKETSRECEIVED");
            if (totalPacketsReceivedId == null)
            {
                FailStep(CL, "Failed to fetch  totalPacketsReceivedId from ini File.");
            }
            else
            {
                LogComment(CL, "totalPacketsReceivedId fetched is : " + totalPacketsReceivedId);

            }
            totalPacketsSentId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "TOTALPACKETSSENT");
            if (totalPacketsSentId == null)
            {
                FailStep(CL, "Failed to fetch  totalPacketsSentId from ini File.");
            }
            else
            {
                LogComment(CL, "totalPacketsSentId fetched is : " + totalPacketsSentId);

            }



            //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMA WEBPAGE
            ledName1ID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LEDNAME1");
            if (ledName1ID == null)
            {
                FailStep(CL, "Failed to fetch LED Name 1 from ini File.");
            }
            else
            {
                LogComment(CL, "LED Name 1 fetched is : " + ledName1ID);

            }



            //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMA WEBPAGE
            ledStatus1ID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LEDSTATUS1");
            if (ledStatus1ID == null)
            {
                FailStep(CL, "Failed to fetch LED status 1 from ini File.");
            }
            else
            {
                LogComment(CL, "LED status1 fetched is : " + ledStatus1ID);

            }



            //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMA WEBPAGE
            ledName2ID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LEDNAME2");
            if (ledName2ID == null)
            {
                FailStep(CL, "Failed to fetch LED NAME 2 from ini File.");
            }
            else
            {
                LogComment(CL, "LED Name 2 fetched is : " + ledName2ID);

            }



            //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMA WEBPAGE
            ledStatus2ID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LEDSTATUS2");
            if (ledStatus2ID == null)
            {
                FailStep(CL, "Failed to fetch LED status 2 from ini File.");
            }
            else
            {
                LogComment(CL, "LED STATUS 2 fetched is : " + ledStatus2ID);

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

            //Total Bytes Recevied 

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            totalBytesReceivedValue1 = CL.EA.UI.RMS.GetParameterValues(driver, totalBytesReceivedId);

            if (totalBytesReceivedValue1 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalBytesReceivedValue1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalBytesReceivedValue1 from panorama is " + totalBytesReceivedValue1);

            //Total Bytes Sent

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            totalBytesSentValue1 = CL.EA.UI.RMS.GetParameterValues(driver, totalBytesSentId);

            if (totalBytesSentValue1 == "N/A")
            {
                FailStep(CL, "Failed to fetch the totalBytesSentValue1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalBytesSentValue1 from panorama is " + totalBytesSentValue1);


            //Total Bytes Recevied Value 2

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            totalBytesReceivedValue2 = CL.EA.UI.RMS.GetParameterValues(driver, totalBytesReceivedId);

            if (totalBytesReceivedValue2 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalBytesReceivedValue2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalBytesReceived Value2 from panorama is " + totalBytesReceivedValue2);


            //Total Bytes Sent Value 2

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            totalBytesSentValue2 = CL.EA.UI.RMS.GetParameterValues(driver, totalBytesSentId);

            if (totalBytesSentValue2 == "N/A")
            {
                FailStep(CL, "Failed to fetch the totalBytesSentValue2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalBytesSentValue2 from panorama is " + totalBytesSentValue2 + "is greater than or equla to value 1" + totalBytesSentValue1);



            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalBytesReceivedValue2) < Convert.ToInt32(totalBytesReceivedValue1))
            {

                LogComment(CL, "Total bytes recevied is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total bytes received From Panorama Page" + totalBytesReceivedValue2 + "is greater than or equla to value 1" + totalBytesReceivedValue1);
            }



            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalBytesSentValue2) < Convert.ToInt32(totalBytesSentValue1))
            {

                LogComment(CL, "Total bytes sent is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total bytes sent From Panorama Page" + totalBytesSentValue2);
            }
            CL.IEX.Wait(10);

            //Total Packets Received

            //FETCHING THE VALUE FROM THE PANORAMA

            totalPacketsReceivedValue1 = CL.EA.UI.RMS.GetParameterValues(driver, totalPacketsReceivedId);

            if (totalPacketsReceivedValue1 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalPacketsReceivedValue1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalPacketsReceivedValue1 from panorama is " + totalPacketsReceivedValue1);


            //Total Packets sent

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            totalPacketsSentValue1 = CL.EA.UI.RMS.GetParameterValues(driver, totalPacketsSentId);

            if (totalPacketsSentValue1 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalPacketssentvalue1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalPacketssentValue1 from panorama is " + totalPacketsSentValue1);


            //Total Packets Received Value 2

            //FETCHING THE VALUE FROM THE PANORAMA

            totalPacketsReceivedValue2 = CL.EA.UI.RMS.GetParameterValues(driver, totalPacketsReceivedId);

            if (totalPacketsReceivedValue2 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalPacketsReceivedValue2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalPacketsReceivedValue2 from panorama is " + totalPacketsReceivedValue2);


            //Total Packets sent Value 2

            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            totalPacketsSentValue2 = CL.EA.UI.RMS.GetParameterValues(driver, totalPacketsSentId);

            if (totalPacketsSentValue2 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalPacketssentvalue2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalPacketssentValue2 from panorama is " + totalPacketsSentValue2);

            
            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalPacketsReceivedValue2) < Convert.ToInt32(totalPacketsReceivedValue1))
            {

                LogComment(CL, "Total Packets recevied is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total packets received value 2 From Panorama Page" + totalBytesReceivedValue2 + "is greater or equal to value 1 " + totalBytesReceivedValue1);
            }



            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalPacketsSentValue2) < Convert.ToInt32(totalPacketsSentValue1))
            {

                LogComment(CL, "Total Packets sent is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total Packets sent From Panorama Page value2 " + totalPacketsSentValue2 + "greater or equal to value 1" + totalPacketsSentValue1);
            }
            CL.IEX.Wait(10);


        }
    }
    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.IEX.Wait(5);
            //Powercycle the CPE           
            res = CL.EA.PowerCycle(0, true, false);
            if (!res.CommandSucceeded)
            {
               FailStep(CL, res);
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 120;
            CL.IEX.Wait(Time_In_Standby);
            driver.Navigate().Refresh();
            CL.IEX.Wait(7);
            //FETCHING THE VALUE FROM THE PANORAMA

            obtainedLedName1 = CL.EA.UI.RMS.GetParameterValues(driver, ledName1ID);

            if (obtainedLedName1 == "N/A")
            {
                FailStep(CL, "Failed to fetch LED Name 1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained obtainedLedName1 from panorama is " + obtainedLedName1);

            if (obtainedLedName1 != "Power")
            {

                LogComment(CL, "Expected LED Name 1: Power and obtained ledName 1: " + obtainedLedName1 + "Are not same");
                isFail = true;
                noOfFailures++;
                obtainedLedName1 = CL.EA.UI.RMS.GetParameterValues(driver, ledName1ID);
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedLedName1 + " And value Power Are Equal");
            }
            CL.IEX.Wait(5);

            obtainedLedStatus1 = CL.EA.UI.RMS.GetParameterValues(driver, ledStatus1ID);

            if (obtainedLedStatus1 == "N/A")
            {
                FailStep(CL, "Failed to fetch LED Name 1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained obtainedLedStatus1 from panorama is " + obtainedLedStatus1);

            if (obtainedLedStatus1 != "Orange")
            {

                LogComment(CL, "Expected LED Name 1: Power and obtained ledName 1: " + obtainedLedStatus1 + "Are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedLedStatus1 + " And value Power Are Equal");
            }
            CL.IEX.Wait(5);


            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }
            LogComment(CL, "Refreshing the panorama web page");
            /*driver.Navigate().Refresh();
            CL.IEX.Wait(2);
            LogComment(CL, "Clicking on retrive button");
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            CL.IEX.Wait(3);
            LogComment(CL, "Retriving the value from panorama page");
            obtainedLedStatus1 = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();
            CL.IEX.Wait(2);*/
            driver.Navigate().Refresh();
            CL.IEX.Wait(5);
           obtainedLedStatus1 = CL.EA.UI.RMS.GetParameterValues(driver, ledStatus1ID);

            if (obtainedLedStatus1 == "N/A")
            {
                FailStep(CL, "Failed to fetch LED Stauts 1 value from Panorama");

            }

            LogCommentImportant(CL, "obtained obtainedLedStatus1 from panorama is " + obtainedLedStatus1);

            if (obtainedLedStatus1 != "Blue")
            {

                LogComment(CL, "Expected LED Status 1: Power and obtained ledName 1: " + obtainedLedStatus1 + "Are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedLedStatus1 + " And value Blue Are Equal");
            }

        }
    }
    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            totalBytesReceivedValue3 = CL.EA.UI.RMS.GetParameterValues(driver, totalBytesReceivedId);


            if (totalBytesReceivedValue3 == "N/A")
            {
                LogComment(CL, "Failed to fetch totalBytesReceivedValue3 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalBytesReceivedValue3 from panorama is " + totalBytesReceivedValue3);

            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalBytesReceivedValue3) > Convert.ToInt32(totalBytesReceivedValue1))
            {

                LogComment(CL, "Total bytes recevied is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total bytes received From Panorama Page" + totalBytesReceivedValue3);
            }

            totalBytesSentValue3 = CL.EA.UI.RMS.GetParameterValues(driver, totalBytesSentId);

            if (totalBytesSentValue3 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalBytesSentValue3 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalBytesSentValue3 from panorama is " + totalBytesSentValue3);

            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalBytesSentValue3) > Convert.ToInt32(totalBytesSentValue1))
            {

                LogComment(CL, "Total bytes sent is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total bytes sent From Panorama Page" + totalBytesSentValue3);
            }
            CL.IEX.Wait(3);

            totalPacketsReceivedValue3 = CL.EA.UI.RMS.GetParameterValues(driver, totalPacketsReceivedId);

            if (totalPacketsReceivedValue3 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalPacketsReceivedValue3 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalPacketsReceivedValue3 from panorama is " + totalPacketsReceivedValue3);

            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalPacketsReceivedValue3) > Convert.ToInt32(totalPacketsReceivedValue1))
            {

                LogComment(CL, "Total bytes recevied is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total Packets received From Panorama Page" + totalPacketsReceivedValue3);
            }
            CL.IEX.Wait(3);

            totalPacketsSentValue3 = CL.EA.UI.RMS.GetParameterValues(driver, totalPacketsSentId);

            if (totalPacketsSentValue3 == "N/A")
            {
                FailStep(CL, "Failed to fetch totalPacketsSentValue2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained totalPacketsSentValue2 from panorama is " + totalPacketsSentValue3);

            //Comparing both the values retreived from Panorama

            if (Convert.ToInt32(totalPacketsSentValue3) > Convert.ToInt32(totalPacketsSentValue1))
            {

                LogComment(CL, "Total bytes recevied is incorrect");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Total Packets sent From Panorama Page" + totalPacketsSentValue3);
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
            driver.Navigate().Refresh();
            CL.IEX.Wait(5);
            //FETCHING THE VALUE FROM THE PANORAMA

            obtainedLedName2 = CL.EA.UI.RMS.GetParameterValues(driver, ledName2ID);

            if (obtainedLedName2 == "N/A" || obtainedLedName2 == null)
            {
                FailStep(CL, "Failed to fetch LED Name 2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained obtainedLedName2 from panorama is " + obtainedLedName1);

            if (obtainedLedName2 != "REC_LED")
            {

                LogComment(CL, "Expected LED Name 2: REC_LED and obtained ledName 2: " + obtainedLedName2 + "Are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedLedName2 + " And value REC_LED Are Equal");
            }
            CL.IEX.Wait(3);

            obtainedLedStatus2 = CL.EA.UI.RMS.GetParameterValues(driver, ledStatus2ID);

            if (obtainedLedStatus2 == "N/A")
            {
                FailStep(CL, "Failed to fetch LED Status 2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained obtainedLedStatus2 from panorama is " + obtainedLedStatus2);

            if (obtainedLedStatus2 != "Off")
            {

                LogComment(CL, "Expected LED Status 2: OFF and obtained ledName 1: " + obtainedLedStatus2 + "Are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedLedStatus2 + " And value OFF Are Equal");
            }
            CL.IEX.Wait(3);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Service:" + service.LCN);
            }

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service - " + service.LCN);
            }
            //driver.Navigate().Refresh();
            //CL.IEX.Wait(6);
            //driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            //CL.IEX.Wait(5);
            //obtainedLedStatus2 = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();
            obtainedLedStatus2 = CL.EA.UI.RMS.GetParameterValues(driver, ledStatus2ID);

            if (obtainedLedStatus2 == "N/A" || obtainedLedStatus2 == null)
            {
                FailStep(CL, "Failed to fetch LED Status 2 value from Panorama");

            }
            
            else if(obtainedLedStatus2 == "On")
            {

                LogComment(CL, "Fetched value From Panorama Page" + obtainedLedStatus2 + " And Status Record Are Equal");
            }
            else 
            {
                
                LogComment(CL, "Expected LED Status 2: Record and obtained LED status 2: " + obtainedLedStatus2 + "Are not same");
                isFail = true;
                noOfFailures++;
            }

            LogCommentImportant(CL, " obtainedLedStatus2 from panorama is " + obtainedLedStatus2);
            CL.IEX.Wait(3);

            // Stop Recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from Archive");
            }
            if (isFail)
            {
                FailStep(CL, "Number of validations failed " + noOfFailures + "...Please see above for Failure reasons");
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
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }


    }

    #endregion PostExecute
}

