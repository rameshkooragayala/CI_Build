/// <summary>
///  Script Name : FT191_Verify_HDDIndicator_Ongoing_Delete_Recording.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT191_Verify_HDDIndicator_Ongoing_Delete_Recording")]
public class FT191_Recording : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;

    //Test Duration
    static int testDuration = 0;
    static string timestamp;
    //Shared members between steps
    static string FTA_Channel;
    static Service recordableService;
    static string timeBasedRecordingSupported;
    static int hddPercentInDiagnostics;
    static int hddPercentInRecording;
     static int hddPercentInPlanner;
     static int hddPercentBeforeEventDelete;
     static int hddPercentBeforeTimeBasedDelete;
     private static Service TmebasedRecordableService;
     static _helper helper = new _helper();
     static Boolean isHomeNetwork;
    private const string PRECONDITION_DESCRIPTION = "Precondition: Modify rmf.cfg and Do Manual Recording";
    private const string STEP1_DESCRIPTION = "Step 1: Check HDD Usage Percentage While recording is going on";
    private const string STEP2_DESCRIPTION = "Step 2: Check HDD Usage Percentage after deleting event based recording";
    private const string STEP3_DESCRIPTION = "Step 3: CHeck HDD Usage Percentage after deleting time based recoeding";
   
    private static class Constants
    {
        /**********************************************************************************************/
        /**
* @brief   Minutes Delay until Beginning
**************************************************************************************************/

        public const int minDelayUntilBeginning = 2;

        /**********************************************************************************************/
        /**
* @brief   Minutes Delay until Beginning
**************************************************************************************************/

        public const int totalDurationInMin = 5;

        /**********************************************************************************************/

    }
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
       

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

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

           

            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch recordableService" + recordableService.LCN + "from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "RecordableService fetched from content xml is : " + recordableService.LCN);
            }
            TmebasedRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High;LCN="+recordableService.LCN+"");

            if (TmebasedRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + TmebasedRecordableService.LCN);
            }

            timeBasedRecordingSupported = CL.EA.GetValueFromINI(EnumINIFile.Project, "MANUAL_RECORDING", "SUPPORTED");
            if(timeBasedRecordingSupported.Trim()==""||string.IsNullOrEmpty(timeBasedRecordingSupported))
            {
                FailStep(CL, "Fail to fetch timeBasedRecordingSupported value from project ini");
            }



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
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // navigating to my recording
            CL.EA.UI.ArchiveRecordings.Navigate();
            helper.VerifyHDDPercentageDuringOngoingRec();
            CL.IEX.Wait(3);
            CL.EA.UI.FutureRecordings.Navigate(); // Navigating to Planner
            helper.VerifyHDDPercentageDuringOngoingRec();
                      
            res=CL.EA.WaitUntilEventEnds("EventbasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait until Event based recording ends ");
            }

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

             //HDD Usage percentage in Diagnostics
            CL.EA.UI.Utils.NavigateToDiagnostics();

            CL.IEX.Wait(2);

           hddPercentBeforeEventDelete = CL.EA.UI.Utils.GetHDDUsagePercentage();

           CL.IEX.SendIRCommand("RETOUR", -1, ref timestamp);

           CL.IEX.Wait(2);
                       
            res = CL.EA.PVR.DeleteRecordFromArchive("EventbasedRecording", VerifyDeletedInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to delete Event based recording  ");
            }

            CL.IEX.Wait(4);

          

           helper.VerifyHDDPercentageAfterDeletion(hddPercentBeforeEventDelete);
          


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
            CL.IEX.Wait(2);

            hddPercentBeforeTimeBasedDelete = hddPercentInDiagnostics;

           

            res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording", VerifyDeletedInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to delete Time based recording  ");
            }

            CL.IEX.Wait(4);
            
            helper.VerifyHDDPercentageAfterDeletion(hddPercentBeforeTimeBasedDelete);
           
            PassStep();
        }
    }
    #endregion
   
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion

    #region helper Class

    public class _helper : _Step
    {
        public override void Execute()
        {
        }

        public void VerifyHDDPercentageDuringOngoingRec()
        {
            CL.IEX.Wait(1);
            try
            {

                CL.EA.UI.Utils.VerifyHDDIndicator(true); // verify HDD Indicator
                LogCommentInfo(CL, "Verifed HDD Indicator is true");
            }
            catch
            {
                FailStep(CL, "Failed to Verify HDD Indicator is true");
            }

            // Occupied disk usage milestone

            hddPercentInPlanner = CL.EA.UI.Utils.GetHDDUsagePercentage();

            for (int i = 0; i < 4; i++)
            {
                int hddPercentPlanner = 0;
                CL.IEX.Wait(15);
                hddPercentPlanner = CL.EA.UI.Utils.GetHDDUsagePercentage();
                if (hddPercentPlanner > hddPercentInPlanner)
                {
                    LogCommentInfo(CL, "Verified HDD Usage Percentage is increasing during on going recording in " + i.ToString() + " Check . Percentage is " + hddPercentPlanner.ToString() + "");
                }
                else
                {
                    LogCommentWarning(CL, "HDD Usage percentage is not increased during on going recording in " + i.ToString() + " Check");
                }
                if (i == 4)
                {
                    if (hddPercentPlanner <= hddPercentInPlanner)
                    {
                        FailStep(CL, "Failed to Verify HDD Usage Percentage is increasing during ongoing recording");
                    }

                }
            }


        }

        public void VerifyHDDPercentageAfterDeletion(int PercnetageBeforeDelete)
        {
            //HDD Usage percentage in Diagnostics
            CL.EA.UI.Utils.NavigateToDiagnostics();

            CL.IEX.Wait(4);
            hddPercentInDiagnostics = CL.EA.UI.Utils.GetHDDUsagePercentage();

            if (hddPercentInDiagnostics < PercnetageBeforeDelete)
            {
                LogCommentInfo(CL, "Verified HDD Usage Percentage after deleting event based recording in Diagnoistics");
            }
            else
            {
                FailStep(CL, "Fail to verify HDD Usage Percentage after deleting event based recording in Diagnoistics");
            }


            CL.IEX.SendIRCommand("RETOUR", -1, ref timestamp);
            
            CL.IEX.Wait(2);
           
            CL.IEX.MilestonesEPG.ClearEPGInfo();

            CL.IEX.Wait(2);

            //HDD Usage percentage in My recording

            CL.EA.UI.ArchiveRecordings.Navigate();

            CL.IEX.Wait(2);
                          

            hddPercentInRecording=CL.EA.UI.Utils.GetHDDUsagePercentage(isClearEPG:false);

            if (hddPercentInDiagnostics == hddPercentInRecording)
            {
                LogCommentInfo(CL, "Verified HDD Usage Percentage after deleting event based recording in My Recording");
            }
            else
            {
                FailStep(CL, "Fail to verify HDD Usage Percentage after deleting event based recording in My Recording");
            }

            //HDD Usage percentage in In planner
            CL.EA.UI.FutureRecordings.Navigate();

            CL.IEX.Wait(4);

            hddPercentInPlanner = CL.EA.UI.Utils.GetHDDUsagePercentage();

            if (hddPercentInDiagnostics == hddPercentInPlanner)
            {
                LogCommentInfo(CL, "Verified HDD Usage Percentage after deleting event based recording in My Planner");
            }
            else
            {
                FailStep(CL, "Fail to verify HDD Usage Percentage after deleting event based recording in My Planner");
            }
        }
    }
    #endregion

}