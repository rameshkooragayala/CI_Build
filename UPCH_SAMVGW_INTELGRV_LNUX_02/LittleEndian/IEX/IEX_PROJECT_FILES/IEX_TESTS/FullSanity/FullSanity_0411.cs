/// <summary>
///  Script Name : FullSanity_0411.cs
///  Test Name   : FullSanity-0411-SET-Diagnostics
///  TEST ID     : 26480
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Madhu Kumar k
/// </summary>

using System;
using IEX.Tests.Engine;
using System.Collections.Generic;
using System.Collections;
using IEX.ElementaryActions.Functionality;

public class FullSanity_0411 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    private static string[] diagnosticeDefaultArray;
    private static bool isFail=false;
    private static int noOfFailures=0;
    //Constants to be used in the test case

    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File & SyncStream");
        this.AddStep(new Step1(), "Step 1: Navigate to Diagnostics and verify the default value");

        //Get Client Platform
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
            //Fetching the key values for diagnostics of the key values from Test ini
            string diagnosticsDefault = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DIAGNOSTICS_DEFAULT");
            if (diagnosticsDefault == "")
            {
                FailStep(CL,"Failed to fetch the diagnostics default value from Test ini");
            }
            LogCommentImportant(CL,"Diagnostics defaut from Test ini is "+diagnosticsDefault);
            diagnosticeDefaultArray = diagnosticsDefault.Split(',');

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUTO STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AUTO STANDBY");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.MilestonesEPG.Navigate("AUTOMATIC");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AUTOMATIC");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:STANDBY POWER USAGE");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.MilestonesEPG.Navigate("HOT STANDBY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to HIGH");
            }
            CL.EA.ReturnToLiveViewing();
            string msg_utm_start = "IEX_UTM_STARTED";
            string actual_msg;
            string maginlines;

            // Put the box into standby mode
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            // Wait for 20 mins to complete the UT. Since after UT, box will go to the required standby mode
            res = CL.IEX.Debug.BeginWaitForMessage(msg_utm_start, 0, 20 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.IEX.Debug.EndWaitForMessage(msg_utm_start, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }

            CL.IEX.LogComment("MP Started!!!");
            string msg_utm_end = "IEX_UTM_COMPLETED";

            res = CL.IEX.Debug.BeginWaitForMessage(msg_utm_end, 0, 20 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.IEX.Debug.EndWaitForMessage(msg_utm_end, out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }


            // Check the next MP is scheduled at end time +22hours + Random value in the range [0...180min]
            CL.IEX.Wait(10);

            CL.IEX.LogComment("MP Completed!!!");

            // wakeup the box
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.IEX.Wait(seconds: 10);

            PassStep();
        }
    }
    #endregion
    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //res = CL.EA.NavigateAndHighlight("STATE:DIAGNOSTICS", dictionary);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to Navigate to dignostics");
            //}
			CL.IEX.MilestonesEPG.ClearEPGInfo();
			
            CL.EA.UI.Utils.NavigateToDiagnostics();

            res = CL.IEX.Wait(seconds: 5);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to wait few seconds after navigating to Diagnostics");
            }

            foreach (string diagnosticsDefaultValue in diagnosticeDefaultArray)
            {
                //Fetching the keys from Test ini and adding an _ to make them the Case values which are then used to redirect to that paticular case
                string diagnosticsCaseValue = diagnosticsDefaultValue.ToUpper().Replace(' ', '_');

                switch (diagnosticsCaseValue)
                {
                    case "HARDWARE_VERSION":
                        string expectedHardwareVersion = "";
                        //Fetching the Hardware version from the Environent ini file
                        expectedHardwareVersion = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "HARDWARE_VERSION");
                        if (expectedHardwareVersion == "")
                        {
                            LogCommentFailure(CL, "HARDWARE_VERSION is not defined in the Environment ini file");
                            isFail = true;
                        }

                        string obtainedHardwareVersion = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("hardware version", out obtainedHardwareVersion);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the softwareVrsion from EPG");
                            isFail = true;
                        }
                        LogCommentInfo(CL, "Hardware version fetched from EPG is " + obtainedHardwareVersion);
                        //Comparing the Expected Hardware Version from Env ini file with obtained Hardware Version from EPG
                        if (!obtainedHardwareVersion.Contains(expectedHardwareVersion))
                        {
                            LogCommentFailure(CL, "Expected hardware version " + expectedHardwareVersion + " is different from Obtained version " + obtainedHardwareVersion);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Expected hardware version " + expectedHardwareVersion + " is same as " + obtainedHardwareVersion);
                        }

                        break;
                    case "SOFTWARE_VERSION":

                        string currentEPGVersion = "";
                        //Fetching the current EPG Version Worker and comparing it with the value from EPG
                        currentEPGVersion = CL.EA.UI.Mount.GetCurrentEpgVersion();
                        if (currentEPGVersion == "")
                        {
                            LogCommentFailure(CL, "Failed to get the current EPG version");
                            isFail = true;
                        }

                        LogCommentInfo(CL, "Current Version is" + currentEPGVersion);
                        string softwareVersion = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("software version", out softwareVersion);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the softwareVersion from EPG");
                            isFail = true;
                        }
                        LogCommentInfo(CL, "Software version fetched from EPG is " + softwareVersion);
                        //Using contains as the Software version from EPG contains lot of other values like version and all along with the Sotware version
                        if (!softwareVersion.Contains(currentEPGVersion))
                        {
                            LogCommentFailure(CL, "Software version from EPG : " + softwareVersion + " does not match with current EPG version " + currentEPGVersion);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Software version from EPG : " + softwareVersion + " matches with current EPG version " + currentEPGVersion);
                        }
                        break;
                    case "BOX_SERIAL_NUMBER":
                        string expectedBoxSerialNumber = "";
                        string expectedBoxId = "";
                        //Fetching the Expected Box id from Environment ini file
                        expectedBoxSerialNumber = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "BOX_ID");
                        if (expectedBoxSerialNumber == "")
                        {
                            LogCommentFailure(CL, "expectedBoxSerialNumber is not defined in the Environment ini file");
                            isFail = true;
                        }
                        //The box id contains two extra charactes then Box Serial Number which are removed in order to verify
                        //expectedBoxSerialNumber = expectedBoxId.Substring(0, expectedBoxId.Length - 2);
                        LogCommentInfo(CL, "Expected Box serial number is " + expectedBoxSerialNumber);
                        string obtainedBoxSerialNumber = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("box serial number", out obtainedBoxSerialNumber);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Box Serial Number from EPG");
                            isFail = true;
                        }
                        LogCommentInfo(CL, "Box Serial Number from EPG is " + obtainedBoxSerialNumber);
                        if (expectedBoxSerialNumber != obtainedBoxSerialNumber)
                        {
                            LogCommentFailure(CL, "Expected Box Serial Number " + expectedBoxSerialNumber + " is different from " + obtainedBoxSerialNumber);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Expected Box Serial Number " + expectedBoxSerialNumber + " is same as " + obtainedBoxSerialNumber);
                        }

                        break;
                    case "CPE_USAGE_ID":
                        string CPEUsageId = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("cpe usage id", out CPEUsageId);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the CPE Usage ID from EPG");
                            isFail = true;
                            noOfFailures++;
                        }
                        //CPE Usage ID is given only 0 and 82 to all the boxes so verifying only those 2
                        if (CPEUsageId != "0" && CPEUsageId != "82")
                        {
                            LogCommentFailure(CL, "CPE Usage ID :" + CPEUsageId + "is different from Expected 0 or 82");
                            isFail = true;
                        }
                        break;
                    case "LAST_UPDATE":
                        string lastUpdate = "";
                        string currentDateTime = "";
                        string[] currentDateTimearray;
                        string parsedCurrentTime;
                        string parsedCurrentDate;
                        string[] lastUpdatedearray;
                        string lastUpdatedtTime;
                        string lastUpdatedDate;
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("date", out currentDateTime);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the current date from EPG");
                            isFail = true;
                            noOfFailures++;
                        }
                        LogCommentInfo(CL,"Current Date before parsing is "+currentDateTime);

                        res = CL.IEX.MilestonesEPG.GetEPGInfo("last update", out lastUpdate);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the last Update date from EPG");
                            isFail = true;
                            noOfFailures++;
                        }
                        currentDateTimearray = currentDateTime.Split('_');
                        parsedCurrentTime = currentDateTimearray[3] + ":" + currentDateTimearray[4];
                        parsedCurrentDate=currentDateTimearray[0]+"."+currentDateTimearray[1]+"."+currentDateTimearray[2].Substring(2,2);
                        LogCommentInfo(CL,"Current date is "+parsedCurrentDate+" and time is "+parsedCurrentTime);
                        lastUpdatedearray = lastUpdate.Split('/');
                        lastUpdatedearray=lastUpdatedearray[1].Trim().Split(' ');
                        lastUpdatedtTime = lastUpdatedearray[0];
                        lastUpdatedDate=lastUpdatedearray[2];
                        LogCommentInfo(CL, "Last date is " + lastUpdatedDate + " and time is " + lastUpdatedtTime);
                        //Including extra Start Guard Time
                        TimeSpan lastUpdatedTimeSpan = TimeSpan.Parse(lastUpdatedtTime);

                        //Including extra End Guard Time
                        TimeSpan parsedCurrentTimeSpan = TimeSpan.Parse(parsedCurrentTime);
                        int differenceInSec=(parsedCurrentTimeSpan-lastUpdatedTimeSpan).Minutes;
                        int differenceInHours = (parsedCurrentTimeSpan - lastUpdatedTimeSpan).Hours;
                        if (lastUpdatedDate != parsedCurrentDate)
                        {
                            LogCommentFailure(CL,"Last Updated Date is "+lastUpdatedDate+" is different from the current EPG date "+parsedCurrentDate);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentInfo(CL, "Last Updated Date is " + lastUpdatedDate + " is same as the current EPG date " + parsedCurrentDate);
                            LogCommentImportant(CL, "Last Updated Time Span " + lastUpdatedTimeSpan + " and parsed current Time Span " + parsedCurrentTimeSpan + " and difference" + (parsedCurrentTimeSpan - lastUpdatedTimeSpan).Minutes);
                            if (differenceInSec > 45&&differenceInHours>0)
                            {
                                LogCommentFailure(CL, "Last update time is more then Expected");
                                isFail = true;
                                noOfFailures++;
                            }
                            else
                            {
                                LogCommentInfo(CL,"Last updated time is as expected");
                            }
                        }

                        break;
                    case "SIGNAL_STRENGTH":
                        string signalStrength = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("signal strength", out signalStrength);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Signal Strength from EPG");
                            isFail = true;
                        }
                        LogCommentInfo(CL, "Signal Strength is " + signalStrength);
                        //Signal Strength should be in between 1 and 100
                        if (Convert.ToInt32(signalStrength) < 1 || Convert.ToInt32(signalStrength) > 100)
                        {
                            LogCommentFailure(CL, "Signal strength :" + signalStrength + " is not between 1 and 100 which is expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Signal strength :" + signalStrength + " is between 1 and 100 which is expected");
                        }
                        break;
                    case "SIGNAL_QUALITY":
                        string signalQuality = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("signal quality", out signalQuality);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Signal Quality from EPG");
                            isFail = true;
                        }
                        //Signal strength should be in between 1 and 100
                        if (Convert.ToInt32(signalQuality) < 1 || Convert.ToInt32(signalQuality) > 100)
                        {
                            LogCommentFailure(CL, "Signal strength :" + signalQuality + " is not between 1 and 100 which is expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Signal Quality :" + signalQuality + " is between 1 and 100 which is expected");
                        }
                        break;
                    case "DOCSIS_STATUS":
                        string docsisStatus = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("docsis status", out docsisStatus);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the DOCSIS status from EPG");
                            isFail = true;
                        }
                        //Docsis Status should always be OPERATIONAL
                        if (docsisStatus != "OPERATIONAL")
                        {
                            LogCommentFailure(CL, "DOCSIS STATUS is different " + docsisStatus + " from Expexcted OPERATIONAL");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "DOCSIS STATUS is " + docsisStatus + " same as Expexcted OPERATIONAL");
                        }
                        break;
                    case "WAN_STATUS":
                        string wanStatus = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("wan status", out wanStatus);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the WAN Status from EPG");
                            isFail = true;
                        }
                        if (wanStatus != "00.000.000.000"||wanStatus!="UNKNOWN")
                        {
                            //In order to verify the WAN Status we are taking the first two digits of the IP and verifying for the Junk Value which is 0's
                            string[] wanStatusArray = wanStatus.Split('.');
                            if (wanStatusArray[0] != "10")
                            {
                                LogCommentFailure(CL, "Getting junk value for WAN STATUS " + wanStatus);
                                isFail = true;
                                noOfFailures++;
                            }
                            else
                            {
                                LogCommentImportant(CL, "WAN status is " + wanStatus);
                            }
                        }
                        else
                        {
                            LogCommentFailure(CL, "Obtained Wan Status is " + wanStatus + " which is not expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        break;
                    case "HDMI_STATUS":
                        string hdmiStatus = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("hdmi status", out hdmiStatus);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the hdmi Status from EPG");
                            isFail = true;
                        }
                        //HDMI Status should always be connected if connected to a TV
                        if (hdmiStatus != "CONNECTED")
                        {
                            LogCommentFailure(CL, "HDMI STATUS " + hdmiStatus + " is different from Expected CONNECTED");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "HDMI STATUS " + hdmiStatus + " is same as expected CONNECTED");
                        }
                        break;
                    case "HDCP_STATE":
                        string hdcpState = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("hdcp state", out hdcpState);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the hdcpState from EPG");
                            isFail = true;
                        }
                        //HDCP State should always be AUTHENTICATED
                        if (hdcpState != "AUTHENTICATED")
                        {
                            LogCommentFailure(CL, "HDCP STATE " + hdcpState + " is different from Expected AUTHENTICATED");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "HDCP STATE " + hdcpState + " is same as expected AUTHENTICATED");
                        }
                        break;
                    case "PROJECT_NAME":
                        string projectName = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("project name", out projectName);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Project name from EPG");
                            isFail = true;
                        }
                        //Project name should always be 021101
                        if (projectName != "021101")
                        {
                            LogCommentFailure(CL, "Project name " + projectName + " is different from Expected 021101");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Project name " + projectName + " is same as expected 021101");
                        }
                        break;
                    case "TV_MANUFACTURER_ID":
                        string tvManufacturerid = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("tv manufacturer id", out tvManufacturerid);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the tv Manufacturer id from EPG");
                            isFail = true;
                        }
                        //Fetching the TV_MANUFACTURER_ID from Test ini and validating it with the EPG
                        string expectedTvManufacturerId = "";
                        expectedTvManufacturerId = CL.EA.GetValueFromINI(EnumINIFile.Test,"TEST PARAMS","TV_MANUFACTURER_ID");
                        if (expectedTvManufacturerId == "")
                        {
                            LogCommentFailure(CL, "TV_MANUFACTURER_ID is not defined in the Environment ini file");
                            isFail = true;
                        }
                        
                        if (tvManufacturerid != expectedTvManufacturerId)
                        {
                            LogCommentFailure(CL, "TV Manufacturer ID from EPG : " + tvManufacturerid + " is different from Expected "+expectedTvManufacturerId);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "TV Manufacturer ID from EPG : " + tvManufacturerid + " is same as Expected " + expectedTvManufacturerId);
                        }
                        break;
                    case "TV_MODEL_NAME":
                        string obtainedTvModelName = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("tv model name", out obtainedTvModelName);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the tv model name from EPG");
                            isFail = true;
                        }
                        string expectedTvModelName = "";
                        expectedTvModelName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TV_MODEL_NAME");
                        if (expectedTvModelName == "")
                        {
                            LogCommentFailure(CL, "TV_MODEL_NAME is not defined in the Environment ini file");
                            isFail = true;
                        }

                        if (obtainedTvModelName != expectedTvModelName)
                        {
                            LogCommentFailure(CL, "TV Model Name from EPG : " + obtainedTvModelName + " is different from Expected " + expectedTvModelName);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "TV Model Name from EPG : " + obtainedTvModelName + " is same as Expected " + expectedTvModelName);
                        }
                        break;
                    case "TV_SERIAL_NUMBER":
                        string obtainedTvSerialNumber = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("tv serial number", out obtainedTvSerialNumber);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the tv serial Number from EPG");
                            isFail = true;
                        }
                        string expectedTvSerialNumber = "";
                        expectedTvSerialNumber = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TV_SERIAL_NUMBER");
                        if (expectedTvSerialNumber == "")
                        {
                            LogCommentFailure(CL, "TV_SERIAL_NUMBER is not defined in the Environment ini file");
                            isFail = true;
                        }

                        if (obtainedTvSerialNumber != expectedTvSerialNumber)
                        {
                            LogCommentFailure(CL, "TV Serial Number from EPG : " + obtainedTvSerialNumber + " is different from Expected " + expectedTvSerialNumber);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "TV Serial Number from EPG : " + obtainedTvSerialNumber + " is same as Expected " + expectedTvSerialNumber);
                        }
                        break;
                    case "TV_WEEK_YEAR_OF_MANUFACTURE":
                        string obtainedTvYearOfManufacture = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("tv week/year of manufacture", out obtainedTvYearOfManufacture);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the tv week/year of manufacture from EPG");
                            isFail = true;
                        }
                        string expectedTvYearOfManufacture = "";
                        expectedTvYearOfManufacture = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TV_WEEK_YEAR_OF_MANUFACTURE");
                        if (expectedTvYearOfManufacture == "")
                        {
                            LogCommentFailure(CL, "TV_WEEK_YEAR_OF_MANUFACTURE is not defined in the Environment ini file");
                            isFail = true;
                        }

                        if (expectedTvYearOfManufacture != obtainedTvYearOfManufacture)
                        {
                            LogCommentFailure(CL, "TV Year of Manufacture from EPG : " + obtainedTvYearOfManufacture + " is different from Expected " + expectedTvYearOfManufacture);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "TV Year of Manufacture from EPG : " + obtainedTvYearOfManufacture + " is same as Expected " + expectedTvYearOfManufacture);
                        }
                        break;
                    case "PREFERRED_TV_HD_RESOLUTION":
                        string obtainedTvHdResolution = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("preferred tv hd resolution", out obtainedTvHdResolution);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the preferred tv hd resolution from EPG");
                            isFail = true;
                        }
                        string expectedTvHdResolution = "";
                        expectedTvHdResolution = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PREFERRED_TV_HD_RESOLUTION");
                        if (expectedTvHdResolution == "")
                        {
                            LogCommentFailure(CL, "PREFERRED_TV_HD_RESOLUTION is not defined in the Environment ini file");
                            isFail = true;
                        }

                        if (expectedTvHdResolution != obtainedTvHdResolution)
                        {
                            LogCommentFailure(CL, "Preferred TV HD resolution from EPG : " + obtainedTvHdResolution + " is different from Expected " + expectedTvHdResolution);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Preferred TV HD resolution from EPG : " + obtainedTvHdResolution + " is same as Expected " + expectedTvHdResolution);
                        }
                        break;
                    case "SW_VERS._-_CAK":
                        string SWVersion_CAK = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("sw vers. - cak", out SWVersion_CAK);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Software version CAK from EPG");
                            isFail = true;
                        }
                        if (SWVersion_CAK.ToUpper() == "UNKNOWN")
                        {
                            LogCommentFailure(CL, "The value of sw vers. - cak is " + SWVersion_CAK + " which is not expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Software version sw vers. - cak is not UNKNOWN which is " + SWVersion_CAK);
                        }
                        break;
                    case "SW_VERS._-_DVR/VOD":
                        string SWVersion_DVR = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("sw vers. - dvr/vod", out SWVersion_DVR);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Software version DVR/VOD from EPG");
                        }
                        if (SWVersion_DVR.ToUpper() == "UNKNOWN")
                        {
                            LogCommentFailure(CL, "The value of sw vers. - dvr/vod is " + SWVersion_DVR + " which is not expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Software version sw vers. - dvr/vod is not UNKNOWN which is " + SWVersion_DVR);
                        }
                        break;
                    case "BOX_ID":
                        expectedBoxId = CL.EA.GetValueFromINI(IEX.ElementaryActions.Functionality.EnumINIFile.Environment, "IEX", "BOX_ID");
                        if (expectedBoxId == "")
                        {
                            LogCommentFailure(CL, "expectedBoxSerialNumber is not defined in the Environment ini file");
                        }
                        LogCommentInfo(CL, "Expected Box serial number is " + expectedBoxId);
                        string obtainedBoxID = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("box id", out obtainedBoxID);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Box Serial Number from EPG");
                            isFail = true;
                        }
                        //Box ID which is obtained from EPG contains some spaces so removing them
                        obtainedBoxID = obtainedBoxID.Replace(" ", string.Empty);
                        obtainedBoxID = obtainedBoxID.Substring(0, obtainedBoxID.Length - 2);
                        LogCommentInfo(CL, "Box Serial Number from EPG is " + obtainedBoxID);
                        if (expectedBoxId != obtainedBoxID)
                        {
                            LogCommentFailure(CL, "Expected Box ID " + expectedBoxId + " is different from " + obtainedBoxID);
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Expected Box ID " + expectedBoxId + " is same as " + obtainedBoxID);
                        }

                        break;
                    case "CHIPSET_ID":
                        string chipset_ID = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("chipset id", out chipset_ID);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Chipset ID from EPG");
                            isFail = true;
                        }
                        if (chipset_ID.ToUpper() == "UNKNOWN")
                        {
                            LogCommentFailure(CL, "The value of Chipset ID is " + chipset_ID + " which is not expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "chipset_ID is not UNKNOWN which is " + chipset_ID);
                        }
                        break;
                    case "SECURED_CHIPSET_REVISION":
                        string securedChipsetVersion = "";
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("secured chipset revision", out securedChipsetVersion);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to get the Secured Chipset Version from EPG");
                        }
                        if (securedChipsetVersion.ToUpper() == "UNKNOWN")
                        {
                            LogCommentFailure(CL, "The value of Chipset ID is " + securedChipsetVersion + " which is not expected");
                            isFail = true;
                            noOfFailures++;
                        }
                        else
                        {
                            LogCommentImportant(CL, "chipset_ID is not UNKNOWN which is " + securedChipsetVersion);
                        }
                        break;
                    default:
                        LogCommentInfo(CL, "Few values are not defined in the case statement");
                        isFail = true;
                        noOfFailures++;
                        break;

                }
            }
            if (isFail == true)
            {
                FailStep(CL,"Number of validations failed "+noOfFailures+"...Please see above for Failure reasons");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}