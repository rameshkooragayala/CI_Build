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


    public class RMS_0007 : _Test
    {

        [ThreadStatic]
        static FirefoxDriver driver;
        static string cpeId;
        static string BrowserTabControlId;
        static string expectedSubnetMaskIp;
        static string obtainedSubnetMaskIp;
        static string expectedDnsServerIp;
        static string obtainedDnsServerIp;
        static string expectedIpAddress;
        static string obtainedIpAddress;
        static string expectedMacAddress;
        static string dnsServersId;
        static string defaultGatewayId;
        static string ipAddressId;
        static string macAddreddId;
        static string subnetMaskID;
        static string ftpIpPath;
        static string versionCmd;
        static string inputPath = "";
        static string[] ValidateStrings;
        static string obtainedMacAddress;

        private static _Platform CL;
        private static bool isFail = false;
        private static int noOfFailures = 0;
        static bool ishomenet = false;

        //constants to be used

        #region Create Structure

        public override void CreateStructure()
        {
            this.AddStep(new PreCondition(), "Precondition: Get the CPE ID from the INI file and locale values from test INI");
            this.AddStep(new Step1(), "Step 1: GO TO PANORAMA WEBPAGE LOGIN AND ENTER BOXID AND SEARCH");
            this.AddStep(new Step2(), "Step 2: Fetching the LAN parameter values from Panorama webpage");
            this.AddStep(new Step3(), "Step 3: Fetching the DNS and subnetmask from the test INI file");
            this.AddStep(new Step4(), "Step 4: Fetching the IP address and Macaddress from ifconfig file and comparing");
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

                 string isHomeNetwork = CL.EA.GetValueFromINI(EnumINIFile.Test,"TEST_PARAMS","IsHomeNetwork");

                ////If Home network is true perform GetGateway
                try
                {
                    ishomenet = Convert.ToBoolean(isHomeNetwork);
                }
                catch
                {
                    ishomenet = false;
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

                dnsServersId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "DNSSERVERS");
                if (dnsServersId == null)
                {
                    FailStep(CL, "Failed to fetch dnsServersId from ini File.");
                }
                else
                {
                    LogComment(CL, "dnsServersId fetched is : " + dnsServersId);

                }

                ipAddressId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "IPADDRESS");
                if (ipAddressId == null)
                {
                    FailStep(CL, "Failed to fetch  ipAddressId from ini File.");
                }
                else
                {
                    LogComment(CL, "ipAddressId fetched is : " + ipAddressId);

                }

                macAddreddId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "MACADDRESS");
                if (macAddreddId == null)
                {
                    FailStep(CL, "Failed to fetch  macAddreddId  from ini File.");
                }
                else
                {
                    LogComment(CL, "macAddreddId fetched is : " + macAddreddId);

                }


                //FETCHING THE PARAMETER Id TO BE SEARCHED IN THE PANARAMA WEBPAGE
                subnetMaskID = CL.EA.GetValueFromINI(EnumINIFile.Browser, "DEVICE_INFO_PARAMS", "SUBNETMASK");
                if (subnetMaskID == null)
                {
                    FailStep(CL, "Failed to fetch subnetMaskID from ini File.");
                }
                else
                {
                    LogComment(CL, "subnetMaskID fetched is : " + subnetMaskID);

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
               
                //FETCHING THE DNS SERVER IP VALUE FROM THE PANORAMA WEBPAGE 

                expectedDnsServerIp = CL.EA.UI.RMS.GetParameterValues(driver, dnsServersId);

                if (expectedDnsServerIp == "N/A")
                {
                    FailStep(CL, "Failed to fetch  expectedDnsServerIp value from Panorama");

                }

                LogCommentImportant(CL, "obtained  expectedDnsServerIp from panorama is " + expectedDnsServerIp);

                
                //FETCHING THE START IP VALUE FROM THE PANORAMA
                
                expectedIpAddress = CL.EA.UI.RMS.GetParameterValues(driver, ipAddressId);

                if (expectedIpAddress == "N/A")
                {
                    FailStep(CL, "Failed to fetch expectedStartIpAddress value from Panorama");

                }

                LogCommentImportant(CL, "obtained expectedStartIpAddress from panorama is " + expectedIpAddress);

              
                //FETCHING THE SUBNETMASK VALUE FROM THE PANORAMA WEBPAGE
                expectedSubnetMaskIp = CL.EA.UI.RMS.GetParameterValues(driver, subnetMaskID);

                if (expectedSubnetMaskIp == "N/A" || expectedSubnetMaskIp == "")
                {
                    FailStep(CL, "Failed to fetch expectedSubnetMaskIp value from Panorama");

                }

                LogCommentImportant(CL, "obtained totalPacketssentValue1 from panorama is " + expectedSubnetMaskIp);

              
                //FETCHING THE MACADDRESS VALUE FROM THE PANORAMA WEBPAGE

                expectedMacAddress = CL.EA.UI.RMS.GetParameterValues(driver, macAddreddId);
                expectedMacAddress = expectedMacAddress.ToUpper();
                if (expectedMacAddress == "N/A")
                {
                    FailStep(CL, "Failed to fetch expectedMacAddress value from Panorama");

                }

                LogCommentImportant(CL, "obtained expectedMacAddress from panorama is " + expectedMacAddress);

            }
        }
        #endregion Step2

        #region Step3

        private class Step3 : _Step
        {
            public override void Execute()
            {
                StartStep();


                //Fethcing the LAN parameter values from Test INI.

                obtainedSubnetMaskIp = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST_PARAMS", "subnetMaskIp");
                if (obtainedSubnetMaskIp == null)
                {
                    FailStep(CL, "Failed to fetch obtainedSubnetMaskIp from ini File.");
                }
                else
                {
                    LogComment(CL, "obtainedSubnetMaskIp fetched is : " + obtainedSubnetMaskIp);

                }

                if (obtainedSubnetMaskIp != expectedSubnetMaskIp)
                {

                    LogComment(CL, "Both The values  " + obtainedSubnetMaskIp + "And" + expectedSubnetMaskIp + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedSubnetMaskIp + "And Fetched value From INI File" + obtainedSubnetMaskIp + "Are Equal");
                }
                CL.IEX.Wait(5);


            }

        }

        #endregion Step3

        #region Step4

        private class Step4 : _Step
        {
            public override void Execute()
            {
                StartStep();

               
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

                //Executing the ifconfig command and placing the ifconfig content into "process.txt" file

                versionCmd = @"ifconfig>/host/ifconfig.txt;" + ftpIpPath + " C:/PCAT_Modifier/IEX" + CL.IEX.IEXServerNumber + "/ifconfig.txt  /host/ifconfig.txt";

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
                //////////////////////////////changed Aswin

                if (ishomenet)
                {
                    CL.EA.TelnetLogIn(false, false);
                }
                else
                {
                    CL.EA.TelnetLogIn(false);
                }

               
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



                inputPath = @"C:/PCAT_Modifier/IEX" + CL.IEX.IEXServerNumber + "/ifconfig.txt";
                //////////////////////////////changed Aswin
                if (ishomenet)
                {

                    using (StreamReader str = new StreamReader(inputPath))
                    {
                        string[] lines = System.IO.File.ReadAllLines(inputPath);

                        string[] temp = lines[0].Split(' ', '\t');

                        if (temp != null)
                            if (temp[11] != null)
                            {
                                obtainedMacAddress = temp[10];
                                obtainedMacAddress = obtainedMacAddress.ToUpper();

                            }

                        string[] temp1 = lines[8].Split(' ', '\t');

                        if (temp1 != null)
                        {
                            string[] temp2 = temp1[11].Split(':');
                            obtainedIpAddress = temp2[1];

                        }

                    }
                }
                else
                {
                    using (StreamReader str = new StreamReader(inputPath))
                    {
                        string[] lines = System.IO.File.ReadAllLines(inputPath);

                        string[] temp = lines[0].Split(' ', '\t');

                        if (temp != null)
                            if (temp[10] != null)
                            {
                                obtainedMacAddress = temp[10];
                                obtainedMacAddress = obtainedMacAddress.ToUpper();

                            }

                        string[] temp1 = lines[1].Split(' ', '\t');

                        if (temp1 != null)
                        {
                            string[] temp2 = temp1[11].Split(':');
                            obtainedIpAddress = temp2[1];

                        }

                    }

                }

                //////////////////////////////changed Aswin
                if (obtainedMacAddress != expectedMacAddress)
                {

                    LogComment(CL, "Both The values  " + obtainedMacAddress + "And" + expectedMacAddress + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedMacAddress + "And Fetched value From INI File" + obtainedMacAddress + "Are Equal");
                }
                CL.IEX.Wait(5);

                if (obtainedIpAddress != expectedIpAddress)
                {

                    LogComment(CL, "Both The values  " + obtainedIpAddress + "And" + expectedIpAddress + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedIpAddress + "And Fetched value From INI File" + obtainedIpAddress + "Are Equal");
                }
                CL.IEX.Wait(5);

                if (isFail)
                {
                    FailStep(CL, "Number of validations failed " + noOfFailures + "...Please see above for Failure reasons");
                }



                //dns serverip 
                string[] Temp4 = obtainedIpAddress.Split('.');
                int temp5 =1;
                obtainedDnsServerIp = new StringBuilder(Temp4[0] + ".").Append(Temp4[1] + ".").Append(Temp4[2] + ".").Append(temp5.ToString()).ToString();


                if (obtainedDnsServerIp == null)
                {
                    FailStep(CL, "Failed to fetch obtainedDnsServerIp from ini File.");
                }
                else
                {
                    LogComment(CL, "obtainedDnsServerIp fetched is : " + obtainedDnsServerIp);

                }

                if (obtainedDnsServerIp != expectedDnsServerIp)
                {

                    LogComment(CL, "Both The values  " + obtainedDnsServerIp + "And" + expectedDnsServerIp + " are not same");
                    isFail = true;
                    noOfFailures++;
                }
                else
                {
                    LogComment(CL, "Fetched value From Panorama Page" + expectedDnsServerIp + "And Fetched value From INI File" + obtainedDnsServerIp + "Are Equal");
                }
                CL.IEX.Wait(5);

                if (isFail)
                {
                    FailStep(CL, "Number of validations failed " + noOfFailures + "...Please see above for Failure reasons");
                }

                PassStep();

            }


        }



        #endregion Step4


        #endregion Steps
        #region PostExecute

        public override void PostExecute()
        {
            driver.Close();
        }

        #endregion PostExecute
    }




