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

public class RMS_HDPARAM : _Test
{
    [ThreadStatic]

    private static _Platform CL;
    static FirefoxDriver driver;
    static string cpeId;
    static string browserTabControlId;
    static string hdModelNumberId;
    static string hdSerialNumberId;
    static string freeNetworkQuotaId;
    static string networkQuotaSizeId;
    static string hdStatusId;
    static string temperatureId;
    static string logLastDataId;
    static string hdManufacturerId;
    static string expectedHdModelNumber;
    static string expectedHdSerialNumber;
    static string expectedHdManufacturer;
    static string expectedHdStatus;
    static string expectedNetworkQuotaSize;
    static string expectedFreeNetworkQuota;
    static string expectedTemperature1;
    static string expectedTemperature2;
    static string expectedLogLastData;
    static string expectedLogLastDatafinal;
    static string obtainedNetworkQuotaSize;
    static string obtainedFreeNetworkQuota;
    static string obtainedHdManufacturer;
    static string eventToBeRecorded = "EVENT_TO_BE_RECORDED";
    static string ftpIpPath;
    static string obtainedHdModelNumber;
    static string obtainedHdSerialNumber;
    private static Service service;

    static string[] ValidateStrings;

    private static class Constants
    {
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
        public const int minTimeBeforeEventEnd = 5; //in minutes
    }

   
    
    static string inputPath = "";
   
    static string versionCmd;
   
    private static bool isFail = false;
    private static int noOfFailures = 0;

   

    #region Create Structure

    public override void CreateStructure()
    {
       
        this.AddStep(new PreCondition(), "Precondition: Telneting the CPE and fetching the HDPARM file and obtaining the HD serial number and HD model name");
        this.AddStep(new Step1(), "Step 1: Fethcing the ID's from browser INI file");
        this.AddStep(new Step2(), "Step 2: Launch firefox and enter the CPE ID");
        this.AddStep(new Step3(), "Step 3: Fetching the values from panorama and comparing the same with CPE values");
        this.AddStep(new Step4(), "Step 4: Fetching and comparing the Network quota size and Free network quota values");
        this.AddStep(new Step5(), "Step 5: Fetching and comparing the HD temperature before and after recording");
        this.AddStep(new Step6(), "Step 6: Fetching and comparing the log data value");
        this.AddStep(new Step7(), "Step 7: Fetching and comparing the HD manufacturer value");
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
			res = CL.EA.TuneToChannel("720", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to channel 720");
            }
            else
                LogCommentInfo(CL, "Succesfully tuned to channel 720");
           
            //fetching value from environment file
            ftpIpPath = CL.EA.UI.Utils.GetValueFromEnvironment("PcatCopyCommand");

            if (ftpIpPath == null)
            {
                FailStep(CL, "Failed to fetch PcatcopyCommand from environment ini file");  
            }
            else
            {
                LogComment(CL, "fetched Pcatcommand is" + ftpIpPath);
            }
           
            //Executing hdparm Command and storing values in file

           versionCmd = @"hdparm -i /dev/sda>/host/Process.txt;" + ftpIpPath + " C:/PCAT_Modifier/IEX" + CL.IEX.IEXServerNumber + "/Process.txt  /host/Process.txt";
            
            try
            {

                if (File.Exists(inputPath))
                {
                    try
                    {
                        File.Delete(inputPath);
                    }
                    catch (Exception ex)
                    {
                        FailStep(CL, "Exception Caught" + ex);

                    }

                }
            }
            catch (Exception ex)
            {
                FailStep(CL, "Exception Caught" + ex);
            }
            CL.EA.TelnetLogIn(false);
            try
            {

                CL.EA.SendCmd(versionCmd, false, ref ValidateStrings);


            }
            catch (Exception)
            {
                FailStep(CL, "WARNING : Retrying To Send Command : " + versionCmd);
            }

            CL.EA.SendCmd("sync", false, ref ValidateStrings);

            CL.EA.TelnetDisconnect(false);
            CL.IEX.Wait(5);
//reading the process.txt file 
            
            inputPath = @"C:/PCAT_Modifier/IEX" + CL.IEX.IEXServerNumber + "/Process.txt";
            DataTable temptable = new DataTable();
            temptable.Columns.Add("keys");
            temptable.Columns.Add("values");
            using (StreamReader str = new StreamReader(inputPath))
            {
                string[] lines = System.IO.File.ReadAllLines(inputPath);

                //Console.WriteLine("\t" + lines[3]+"\t before splitting");

                string[] temp = lines[3].Split(',');

                if (temp != null)
                    if (temp[0] != null || temp[2] != null)
                    {
                        string[] temp2 = temp[0].Split('=');
                        string[] temp3 = temp[2].Split('=');
                        if (temp2 != null || temp3 != null)
                        {
  //obtaining the model value and serial number from the process.txt file

                             obtainedHdModelNumber = temp2[1].Trim().ToString();
                             obtainedHdSerialNumber = temp3[1].Trim().ToString();

                        }
                    }

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

            //fetch values from browser ini and panorama

            //cpeid
            cpeId = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
            if (cpeId == null)
            {
                FailStep(CL, "Failed to fetch  cpeId from ini File.");
            }
            else
            {
                LogComment(CL, "cpeId fetched is : " + cpeId);

            }
            //browserTabcontrolId
            browserTabControlId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "PARAMETER_TAB_ID");
            if (browserTabControlId == null)
            {
                FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
            }
            else
            {
                LogComment(CL, "BrowserTabControlId fetched is : " + browserTabControlId);


            }


            // hdModelNumberId

            hdModelNumberId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "HDMODELNUMBER");
            if (hdModelNumberId == null)
            {
                FailStep(CL, "Failed to fetch  hdModelNumberId from ini File.");
            }
            else
            {
                LogComment(CL, "hdModelNumberId fetched is : " + hdModelNumberId);

            }

            // hdSerialNumberId

            hdSerialNumberId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "HDSERIALNUMBER");
            if (hdSerialNumberId == null)
            {
                FailStep(CL, "Failed to fetch  hdSerialNumberId from ini File.");
            }
            else
            {
                LogComment(CL, "hdSerialNumberId fetched is : " + hdSerialNumberId);

            }

            // hdStatusId

            hdStatusId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "HDSTATUS");
            if (hdStatusId == null)
            {
                FailStep(CL, "Failed to fetch  hdStatusId from ini File.");
            }
            else
            {
                LogComment(CL, "hdStatusId fetched is : " + hdStatusId);

            }


            // Free network quota Id

            freeNetworkQuotaId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "FREENETWORKQUOTA");
            if (freeNetworkQuotaId == null)
            {
                FailStep(CL, "Failed to fetch  freeNetworkQuotaId from ini File.");
            }
            else
            {
                LogComment(CL, "freeNetworkQuotaId fetched is : " + freeNetworkQuotaId);

            }

            //network quota size

           networkQuotaSizeId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "NETWORKQUOTASIZE");
            if (networkQuotaSizeId == null)
            {
                FailStep(CL, "Failed to fetch   networkQuotaSizeId from ini File.");
            }
            else
            {
                LogComment(CL, " networkQuotaSizeId fetched is : " + networkQuotaSizeId);

            }
            //HD temperature

            temperatureId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "TEMPERATURE");
            if (temperatureId == null)
            {
                FailStep(CL, "Failed to fetch temperatureId from ini File.");
            }
            else
            {
                LogComment(CL, " temperatureId fetched is : " + networkQuotaSizeId);

            }

            //Last log number 

            logLastDataId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "LOGLASTDATA");
            if (logLastDataId == null)
            {
                FailStep(CL, "Failed to fetch logLastDataId from ini File.");
            }
            else
            {
                LogComment(CL, " logLastDataId fetched is : " + logLastDataId);

            }

            //HD manufacturer 

            hdManufacturerId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "hdmanufacturer");
            if (hdManufacturerId == null)
            {
                FailStep(CL, "Failed to fetch hdManufacturerId from ini File.");
            }
            else
            {
                LogComment(CL, " hdManufacturerId fetched is : " + hdManufacturerId);

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
            driver = new FirefoxDriver();
            CL.IEX.Wait(5);
            //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 

            res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserTabControlId);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
            }

            PassStep();
        }

    }

    #endregion Step2

    #region step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            CL.IEX.Wait(5);
            //FETCHING THE VALUE FROM THE PANORAMA WEBPAGE

            //HD model number
            expectedHdModelNumber = CL.EA.UI.RMS.GetParameterValues(driver, hdModelNumberId);
            if (expectedHdModelNumber == "")
            {
                FailStep(CL, "Failed to fetch the expectedHdModelNumber value from Panorama");

            }

            LogCommentImportant(CL, "obtained expectedHdModelNumber from Panorama is " + expectedHdModelNumber);

            //verifing expected panorama value and obtained value

            if (expectedHdModelNumber != obtainedHdModelNumber)
            {

                LogComment(CL, "Fetched value From Panorama Page" + expectedHdModelNumber + "And Fetched value From HDPARM File" + obtainedHdModelNumber + "Are Not Equal");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + expectedHdModelNumber + "And Fetched value From VersionInfo File" + obtainedHdModelNumber + "Are Equal");
            }
            CL.IEX.Wait(10);

            // HD serial number

            expectedHdSerialNumber = CL.EA.UI.RMS.GetParameterValues(driver, hdSerialNumberId);
            if (expectedHdSerialNumber == "")
            {
                FailStep(CL, "Failed to fetch the expectedHdSerialNumber value from Panorama");

            }

            LogCommentImportant(CL, "obtained expectedHdSerialNumber from Panorama is " + expectedHdSerialNumber);

            //verifing expected panorama value and obtained value

            if (expectedHdSerialNumber != obtainedHdSerialNumber)
            {

                LogComment(CL, "Fetched value From Panorama Page" + expectedHdSerialNumber + "And Fetched value From HDPARM File" + obtainedHdSerialNumber + "Are Not Equal");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + expectedHdSerialNumber + "And Fetched value From VersionInfo File" + obtainedHdSerialNumber + "Are Equal");
            }

            CL.IEX.Wait(10);

            PassStep();

        }
    }
    #endregion Step3

    #region step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            
            expectedNetworkQuotaSize = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "networkquotasize");
            if (expectedNetworkQuotaSize == null)
            {
                FailStep(CL, "Failed to fetch NetworkQuotaSize from ini File.");
            }
            else
            {
                LogComment(CL, "expectedNetworkQuotaSize fetched is : " + expectedNetworkQuotaSize);

            }
            CL.IEX.Wait(3);

            obtainedFreeNetworkQuota = CL.EA.UI.RMS.GetParameterValues(driver, freeNetworkQuotaId);

            if (obtainedFreeNetworkQuota == "N/A")
            {
                FailStep(CL, "Failed to fetch the FreeNetworkQuota value from Panorama");

            }

            LogCommentImportant(CL, "obtained FreeNetworkQuota from panorama is " + obtainedFreeNetworkQuota);

            
            obtainedNetworkQuotaSize = CL.EA.UI.RMS.GetParameterValues(driver, networkQuotaSizeId);

            if (obtainedNetworkQuotaSize == "N/A")
            {
                FailStep(CL, "Failed to fetch the NetworkQuotaSize value from Panorama");

            }

            LogCommentImportant(CL, "obtained NetworkQuotaSize from panorama is " + obtainedNetworkQuotaSize);

            //Comparing the Panoramavalue with environment INI

            if (expectedNetworkQuotaSize != obtainedNetworkQuotaSize)
            {

                LogComment(CL, "Both The values  " + expectedNetworkQuotaSize + "And" + obtainedNetworkQuotaSize + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedNetworkQuotaSize + "And Fetched value From INI File" + expectedNetworkQuotaSize + "Are Equal");
            }
            CL.IEX.Wait(5);

            expectedFreeNetworkQuota = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "freenetworkquota");
            if (expectedFreeNetworkQuota == null)
            {
                FailStep(CL, "Failed to fetch FreeNetworkQuota from ini File.");
            }
            else
            {
                LogComment(CL, "expectedFreeNetworkQuota fetched is : " + expectedFreeNetworkQuota);

            }

           
            //Comparing the Panoramavalue with environment INI

            if (expectedFreeNetworkQuota != obtainedFreeNetworkQuota)
            {

                LogComment(CL, "Both The values  " + expectedFreeNetworkQuota + "And" + obtainedFreeNetworkQuota + " are not same");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + obtainedFreeNetworkQuota + "And Fetched value From INI File" + expectedFreeNetworkQuota + "Are Equal");
            }
            CL.IEX.Wait(5);



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
           
            expectedTemperature1 = CL.EA.UI.RMS.GetParameterValues(driver, temperatureId);
            if (expectedTemperature1 == "")
            {
                FailStep(CL, "Failed to fetch the expectedTemperature value from Panorama");

            }

            LogCommentImportant(CL, "obtained Temperature from Panorama is " + expectedTemperature1);

            //verifing expected panorama value and predefined values

            if (Convert.ToInt32(expectedTemperature1) < 40)
            {

                LogComment(CL, "Fetched value From Panorama Page" + expectedTemperature1 + "is not correct");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + expectedTemperature1 + "is correct");
            }
            CL.IEX.Wait(5);

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service - " + service.LCN);

            }

            //check the HD temperature after the recording.
            driver.Navigate().Refresh();
           
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
            
            driver.Navigate().Refresh();
            expectedTemperature2 = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();
            //expectedTemperature2 = CL.EA.UI.RMS.GetParameterValues(driver, temperatureId);
            if (expectedTemperature2 == "")
            {
                FailStep(CL, "Failed to fetch the expectedTemperature2 value from Panorama");

            }

            LogCommentImportant(CL, "obtained Temperature from Panorama after performing the recording is " + expectedTemperature2);

            //verifing expected panorama value and predefined values

            if (Convert.ToInt32(expectedTemperature2) < Convert.ToInt32(expectedTemperature1))
            {

                LogComment(CL, "Fetched value From Panorama Page after recording" + expectedTemperature2 + "is not correct");
                isFail = true;
                noOfFailures++;
            }
            else
            {
                LogComment(CL, "Fetched value From Panorama Page after recording" + expectedTemperature2 + "is correct");
            }
            CL.IEX.Wait(5);

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

            
            expectedLogLastData = CL.EA.UI.RMS.GetParameterValues(driver, logLastDataId);
            if (expectedLogLastData == "")
            {
                FailStep(CL, "Failed to fetch the expectedLogLastData value from Panorama");

            }

            LogCommentImportant(CL, "obtained expectedLogLastData from Panorama is " + expectedLogLastData);

            expectedLogLastData = "Device.X_NDS_COM_Obj.History.HardDisk.1.Log." + expectedLogLastData + ".Data";

            expectedLogLastDatafinal = CL.EA.UI.RMS.GetParameterValues(driver, expectedLogLastData);
            if (expectedLogLastDatafinal == "")
            {
                FailStep(CL, "Failed to fetch the expectedLogLastDatafinal value from Panorama");

            }

            LogCommentImportant(CL, "obtained expectedLogLastDatafinal from Panorama is " + expectedLogLastDatafinal);

			CL.IEX.Wait(3);
            expectedHdManufacturer = CL.EA.UI.RMS.GetParameterValues(driver, hdManufacturerId);
            if (expectedHdManufacturer == "")
            {
                FailStep(CL, "Failed to fetch the expectedHdManufacturer value from Panorama");

            }

            LogCommentImportant(CL, "obtained expectedHdManufacturer from Panorama is " + expectedHdManufacturer);

            

           

            CL.IEX.Wait(5);
            

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
            
            // HD Status

            expectedHdStatus = CL.EA.UI.RMS.GetParameterValues(driver, hdStatusId);
            if (expectedHdStatus == "Spinning")
            {
                LogComment(CL, "Fetched expectedHdStatus value from Panorama is correct");

            }

            //verifing expected panorama value and obtained value

            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + expectedHdStatus + "And CPE status" + "spinning" + "Are Equal");
            }
            CL.IEX.Wait(5);

            //Switch the box to stand by
            res = CL.EA.StandBy(false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to enter standby");
            }

            // Stay in stand by for 300 seconds
            int Time_In_Standby = 100;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }
           
            //HD status in stand by
            driver.Navigate().Refresh();
            
            
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click();
             driver.Navigate().Refresh();

            expectedHdStatus = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString();
            //CL.EA.UI.RMS.GetParameterValues(driver, hdStatusId);
            if (expectedHdStatus == "sleep")
            {
                LogComment(CL, "Fetched expectedHdStatus value from Panorama is correct");

            }

            //verifing expected panorama value and obtained value

            else
            {
                LogComment(CL, "Fetched value From Panorama Page" + expectedHdStatus + "And CPE status" + "sleep" + "Are Equal");
            }

            if (isFail)
            {
                FailStep(CL, "Number of validations failed " + noOfFailures + "...Please see above for Failure reasons");
            }


        }
    }

    #endregion Step7
    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        driver.Close();
    }

    #endregion PostExecute
}
