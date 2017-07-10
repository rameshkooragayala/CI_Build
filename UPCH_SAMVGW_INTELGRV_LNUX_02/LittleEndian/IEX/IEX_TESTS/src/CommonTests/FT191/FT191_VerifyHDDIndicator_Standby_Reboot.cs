/// <summary>
///  Script Name : FT191_VerifyHDDIndicator_Standby_Reboot.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using OpenQA.Selenium.Firefox;

[Test("FT191_VerifyHDDIndicator_Standby_Reboot")]
public class FT191_Standby_Reboot : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;

    //Test Duration
    static int testDuration = 0;
    static FirefoxDriver driver;
    //Shared members between steps
    static string timestamp;
    //Shared members between steps
    static string FTA_Channel;
    static Service recordableService;
    static string timeBasedRecordingSupported;
    static bool isHomeNetwork;
    static int hddPercentbeforeStandbyReboot=0;
    static int hddPercentInDiagnostics=0;
    static int hddPercentInRecording=0;
    static int hddPercentInPlanner=0;
    private static Service TmebasedRecordableService;
    static _helper helper = new _helper();
    static string cpeId;
    static string browserHardDiskTabId;
    static string HddUsagePath;
    static string hddUsagePercentInPanorama;
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Recordable service , Modify rmf.cfg";
    private const string STEP1_DESCRIPTION = "Step 1:Perform Event Based Recoring and wait for recording completes and check HDD Percentage";
    private const string STEP2_DESCRIPTION = "Step 2:Verify HDD Usage Percentage in Diagnostics, Recording ,Planner & Panorama after Reboot & Standby";
    private const string STEP3_DESCRIPTION = "Step 3:Perform Time based recording for 20 Min";
    private const string STEP4_DESCRIPTION = "Step 4:Verify HDD Usage Indicator in Diagnostics, Recording ,Planner after Reboot & Standby";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();

        try
        {
            isHomeNetwork = Convert.ToBoolean(CL.EA.GetTestParams("IS_HOME_NETWORK"));
        }
        catch
        {
            LogCommentInfo(CL, "Fail to fetch IsHomeNetwork from Test INI. Hence making the value as FALSE");
            isHomeNetwork = false;
        }
        if (isHomeNetwork)
        {
            GW = GetGateway();
        }
    }

    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion
    private static class Constants
    {
        public const int minDelayUntilBeginning = 2;

        public const int totalDurationInMin = 20;

    }
    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

           

            //Get Values From ini File
            FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch recordableService" + recordableService.LCN + "from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "RecordableService fetched from content xml is : " + recordableService.LCN);
            }
            TmebasedRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High;LCN=" + recordableService.LCN + "");

            if (TmebasedRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + TmebasedRecordableService.LCN);
            }

            timeBasedRecordingSupported = CL.EA.GetValueFromINI(EnumINIFile.Project, "MANUAL_RECORDING", "SUPPORTED");
            if (timeBasedRecordingSupported.Trim() == "" || string.IsNullOrEmpty(timeBasedRecordingSupported))
            {
                FailStep(CL, "Fail to fetch timeBasedRecordingSupported value from project ini");
            }

           helper.Modifyrmf_Reboot();


            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU/TOOLBOX/SETTINGS/RECORDINGS & REMINDERS/RECORDINGS/EXTRA TIME AFTER PROGRAMME");
            }

            res = CL.IEX.MilestonesEPG.Navigate("NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to NONE");
            }


            //For RMS

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
            browserHardDiskTabId = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "HardDisk_Id");
            if (browserHardDiskTabId == null)
            {
                FailStep(CL, "Failed to fetch  BrowserTabControlId from ini File.");
            }
            else
            {
                LogComment(CL, "BrowserTabControlId fetched is : " + browserHardDiskTabId);

            }


            HddUsagePath = CL.EA.GetValueFromINI(EnumINIFile.Browser, "BROWSER_ID", "HDD_Usage_Path");
            if (browserHardDiskTabId == null)
            {
                FailStep(CL, "Failed to fetch  HDD Usage Path from ini File.");
            }
            else
            {
                LogComment(CL, "HDD_Usage_Path is : " + browserHardDiskTabId);

            }
            //End For RMS

          
            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            if (!isHomeNetwork)
            {
                helper.Panorama_Login();

                helper.VerifyHddPercentInPanorama();
            }
           
            res = CL.EA.TuneToChannel(recordableService.LCN, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Tune to channel " + recordableService.LCN);
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventbasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record current event from Banner on " + recordableService);
            }
           

            res = CL.EA.WaitUntilEventEnds("EventbasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }

            

            CL.IEX.Wait(10);
           

            CL.EA.UI.Utils.NavigateToDiagnostics();

            CL.IEX.Wait(1);

            hddPercentbeforeStandbyReboot = CL.EA.UI.Utils.GetHDDUsagePercentage();

            LogCommentInfo(CL, "Hdd Percentage before standBy & Reboot is " + hddPercentbeforeStandbyReboot.ToString());

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res=CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to standby");
            }

            CL.IEX.Wait(5);

            res = CL.EA.StandBy(true);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wake up from standby");
            }

            helper.VerifyHDDPercentageAfterStandbyReboot(hddPercentbeforeStandbyReboot,false);

            CL.IEX.Wait(2);

            if (!isHomeNetwork)
            {
                helper.VerifyHddPercentInPanorama();
            }


            helper.Modifyrmf_Reboot();

            helper.VerifyHDDPercentageAfterStandbyReboot(hddPercentbeforeStandbyReboot, false);

            CL.IEX.Wait(3);

           

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.EA.ReturnToLiveViewing();

            CL.IEX.Wait(5);

            if (timeBasedRecordingSupported.ToUpper() == "TRUE")
            {
                res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording", TmebasedRecordableService.Name, -1, Constants.minDelayUntilBeginning, Constants.totalDurationInMin);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Book a Time Based Recording on " + TmebasedRecordableService.LCN);
                }
            }
            else
            {
                FailStep(CL, res, "Time Based Recording is not supported");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to standby");
            }

            CL.IEX.Wait(5);

            res = CL.EA.StandBy(true);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wake up from standby");
            }

            helper.VerifyHDDPercentageAfterStandbyReboot(hddPercentbeforeStandbyReboot, true);

            CL.IEX.Wait(2);

            helper.Modifyrmf_Reboot();

            helper.VerifyHDDPercentageAfterStandbyReboot(hddPercentbeforeStandbyReboot, true);

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        driver.Close();
    }
    #endregion

    #region helper Class

    public class _helper : _Step
    {
        public override void Execute()
        {
           
        }

        public void Modifyrmf_Reboot()
        {
            if (isHomeNetwork)
            {
                res = GW.IEX.SendPowerCycle("OFF");
                if (!res.CommandSucceeded)
                {
                    FailStep(GW, "Fail to do PoweCycle OFF");
                }

                GW.IEX.Wait(5);
                res = GW.IEX.SendPowerCycle("ON");
                if (!res.CommandSucceeded)
                {
                    FailStep(GW, "Fail to do PoweCycle ON");
                }
                GW.EA.UI.Mount.WaitForPrompt(false);
                GW.EA.UI.Mount.SendMountCommand(true, @"sed -i -e '/Remaining/s/Remaining/Fixed/' -e '/80000/s/80000/2/' -e '/max_size=2/a max_size_units=""GiB""' /NDS/config/rmf.cfg");

                GW.IEX.Wait(3);
                res = GW.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
                if (!res.CommandSucceeded)
                {
                    FailStep(GW, "Fail to Mount after modifying rmf.cfg");
                }
                GW.IEX.Wait(50);
                res = GW.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(GW, "Fail to wakeup from standby");
                }

                // reboot ipc

                res = CL.IEX.SendPowerCycle("OFF");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to do PoweCycle OFF");
                }

                CL.IEX.Wait(5);
                res = CL.IEX.SendPowerCycle("ON");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to do PoweCycle ON");
                }

                CL.IEX.Wait(3);
                res = CL.EA.MountClient(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to Mount client after modifying rmf.cfg");
                }
                CL.IEX.Wait(50);
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to wakeup from standby");
                }

            }
            else
            {
                ///////////////////////////////// Modifying rmf.cfg///////////////////////////////////////

                res = CL.IEX.SendPowerCycle("OFF");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to do PoweCycle OFF");
                }

                CL.IEX.Wait(5);
                res = CL.IEX.SendPowerCycle("ON");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to do PoweCycle ON");
                }
                CL.EA.UI.Mount.WaitForPrompt(false);
                CL.EA.UI.Mount.SendMountCommand(true, @"sed -i -e '/Remaining/s/Remaining/Fixed/' -e '/80000/s/80000/2/' -e '/max_size=2/a max_size_units=""GiB""' /NDS/config/rmf.cfg");

                CL.IEX.Wait(3);
                res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to Mount after modifying rmf.cfg");
                }
                CL.IEX.Wait(30);
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to wakeup from standby");
                }
            }
            /////////////////////////////////End Modifying rmf.cfg///////////////////////////////////////
        }

        public void VerifyHDDPercentageAfterStandbyReboot(int PercentageBeforestandbyReboot,bool isOngoingRec)
        {
            IEXGateway._IEXResult res;
            //HDD Usage percentage in Diagnostics
            CL.EA.UI.Utils.NavigateToDiagnostics();

            CL.IEX.Wait(4);

            hddPercentInDiagnostics = CL.EA.UI.Utils.GetHDDUsagePercentage();

            CL.IEX.Wait(2);

            LogCommentInfo(CL, "Hdd Percentage in Diagnostics is " + hddPercentInDiagnostics.ToString());

            if (isOngoingRec==true) // if ongoing rec percentage may be higher than in diagnostics else same
            {
                LogCommentInfo(CL, "Ongoing recording is true");
                if (hddPercentInDiagnostics >= PercentageBeforestandbyReboot)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage  in Diagnostics");
                }
                else
                {
                    FailStep(CL, "Fail to verify HDD Usage Percentage  in Diagnostics");
                }
            }
            else
            {
                LogCommentInfo(CL, "Ongoing recording is false");
                if (hddPercentInDiagnostics == PercentageBeforestandbyReboot)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage  in Diagnostics is same as before stand by /Reboot");
                }                   
                else
                {
                    FailStep(CL, "Fail to verify HDD Usage Percentage in Diagnostics is same as before stand by /Reboot");
                }
            }

           


           CL.IEX.SendIRCommand("RETOUR", -1, ref timestamp); // to move out from dignostics

            CL.IEX.Wait(2);

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            CL.IEX.Wait(2);

            //HDD Usage percentage in My recording

            CL.EA.UI.ArchiveRecordings.Navigate();

            CL.IEX.Wait(2);


            hddPercentInRecording = CL.EA.UI.Utils.GetHDDUsagePercentage(isClearEPG: false);
            LogCommentInfo(CL, "Hdd Percentage in My Recording is " + hddPercentInRecording.ToString());
            if (isOngoingRec == true) // if ongoing rec percentage may be higher than in diagnostics else same
            {
                if (hddPercentInDiagnostics <= hddPercentInRecording)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage  in My Recording  is greater than as before stand by /Reboot during ongoing recording");
                }
                else
                {
                    FailStep(CL, "Fail to verify HDD Usage Percentage  in My Recording is greater than as before stand by /Reboot during ongoing recording");
                }
            }
            else
            {
                if (hddPercentInDiagnostics == hddPercentInRecording)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage  in My Recording is same as before stand by /Reboot");
                }
                else
                {
                    FailStep(CL, "Fail to verify HDD Usage Percentage  in My Recording is same as before stand by /Reboot");
                }
            }

            //HDD Usage percentage in In planner
            CL.EA.UI.FutureRecordings.Navigate();

            CL.IEX.Wait(4);

            hddPercentInPlanner = CL.EA.UI.Utils.GetHDDUsagePercentage();
            LogCommentInfo(CL, "Hdd Percentage in My Planner is " + hddPercentInPlanner.ToString());
            if (isOngoingRec == true) // if ongoing rec percentage may be higher than in diagnostics else same
            {
                if (hddPercentInDiagnostics <= hddPercentInPlanner)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage  in My Planner is greater than as before stand by /Reboot during ongoing recording");
                }
                else
                {
                    FailStep(CL, "Fail to verify HDD Usage Percentage  in My Planner is greater than as before stand by /Reboot during ongoing recording");
                }
            }
            else
            {
                if (hddPercentInDiagnostics == hddPercentInPlanner)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage  in My Planner is same as before stand by /Reboot");
                }
                else
                {
                    FailStep(CL, "Fail to verify HDD Usage Percentage  in My Planner is same as before stand by /Reboot");
                }
            }
        }

        public void Panorama_Login()
        {
            driver = new FirefoxDriver();

            CL.IEX.Wait(5);
            //LOGIN TO PANORAMA PAGE AND ENTER BOXID AND NAVIGATE TO RESPECTIVE TAB 
            res = CL.EA.RMSLoginAndEnterBoxid(driver, cpeId, browserHardDiskTabId);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Unable To Login Enter CpeId and Select Tab On the WebPage");
            }
            else
            {
                LogComment(CL, "Successfully Logged into web Page and entered cpeid and navigated to hard Disk tab");
            }

           
        }

        public void VerifyHddPercentInPanorama()
        {
           
            
            try
            {
                driver.FindElementByClassName("retrieve-line").Click();


                CL.IEX.Wait(30);

                hddUsagePercentInPanorama = driver.FindElementByXPath(HddUsagePath).Text.ToString();
            }
            catch
            {


                driver.FindElementByClassName("retrieve-line").Click();


                CL.IEX.Wait(50);

                hddUsagePercentInPanorama = driver.FindElementByXPath(HddUsagePath).Text.ToString();

            }

            CL.IEX.Wait(2);

            LogComment(CL, "HDD Usage Percentage from Panorama Page is " + hddUsagePercentInPanorama);

            int percentRounoff = Convert.ToInt32(Math.Round(Convert.ToDecimal(hddUsagePercentInPanorama.Replace("%","").Trim())));
            
            LogComment(CL, "HDD Usage Percentage from Panorama Page after RoundOff is " + percentRounoff.ToString());

            if (hddPercentInDiagnostics == hddPercentInPlanner)
            {
                LogCommentInfo(CL, "Verified HDD Usage Percentage  in Panorama is same as in diagnostics");
            }
            else
            {
                FailStep(CL, "Fail to verify HDD Usage Percentage  in Panorama is same as in diagnostics");
            }

        }
    }
    #endregion
}