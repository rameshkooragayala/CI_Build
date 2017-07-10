/// <summary>
///  Script Name : FT191_Verify_HDDIndicator_AfterSCRSVR_Timeout.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT191_Verify_HDDIndicator_AfterSCRSVR_Timeout")]
public class FT191_SCRSVR_Timeout : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static string screenSaverWait;
    static string IsWithRecording;
    static string IsPlanner_Recording;
    static int screenSaverWaitInSecs;
    static Service recordableService1;
    static string ScreenSaver;
    static string timeStamp;
    static string hddUsagePercent;


    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch values from Test INI";
    private const string STEP1_DESCRIPTION = "Step 1:Navidate to my planner or My recording & Wait for screen saver & verify polling is not happening";
    private const string STEP3_DESCRIPTION = "Step 2:Verify Polling is not happening during screen saver ";
    private const string STEP2_DESCRIPTION = "Step 3:Verify Disk usage iducator & Percentage after came out from screen saver ";

    private static class Constants
    {
        public const string scrnSaverActive = "ON";
        public const string scrnSaverInActive = "OFF";
    }

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step2(), STEP3_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
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


            try
            {
                screenSaverWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "EPG_INACTIVE_SCREENSAVER_WAIT");
                screenSaverWaitInSecs = Convert.ToInt32(screenSaverWait);
            }
            catch (Exception ex)
            {

                FailStep(CL, "SCREENSAVER_WAIT not defined in project INI");
            }

            recordableService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService1 == null)
            {
                FailStep(CL, "Failed to fetch recordableService" + recordableService1.LCN + "from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "RecordableService fetched from content xml is : " + recordableService1.LCN);
            }

           

            // fetch navigate to My Recording / My Planner  from Test INI
            IsPlanner_Recording = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ISRECORD_PLANNER");
            if (IsPlanner_Recording == "" || string.IsNullOrEmpty(IsPlanner_Recording))
            {
                FailStep(CL, "Failed to fetch ISRECORD_PLANNER value from TESTINI");
            }

            //Fetch with ongoing recording or with out on going recording from TestINI
            IsWithRecording = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ISWITHRECORDING");
            if (IsWithRecording == "" || string.IsNullOrEmpty(IsWithRecording))
            {
                FailStep(CL, "Failed to fetch ISWITHRECORDING  value from TESTINI");
            }

            //Tune to recordable service
            res = CL.EA.TuneToChannel(recordableService1.LCN, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Tune to channel " + recordableService1.LCN);
            }

            CL.IEX.Wait(1);

            if (IsWithRecording.ToUpper() == "YES")
            {
                res = CL.EA.PVR.RecordManualFromCurrent("ManualRecord", recordableService1.LCN, ((screenSaverWaitInSecs/60)+10));
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To record from  " + recordableService1.LCN);
                }

               
            }

            CL.IEX.Wait(1);

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

            // navigating to MyPlanner / My recording  according to Test INI

            if (IsPlanner_Recording.ToUpper() == "MYRECORDING")
            {
                // navigating to my recording
                CL.EA.UI.ArchiveRecordings.Navigate();
            }
            else if (IsPlanner_Recording.ToUpper() == "MYPLANNER")
            {
                CL.EA.UI.FutureRecordings.Navigate(); // Navigating to Planner
            }
            else
            {
                FailStep(CL, "ISRECORD_PLANNER value from Test INI is invalid");
            }

            CL.IEX.Wait(screenSaverWaitInSecs);

            CL.IEX.Wait(65);

            // geting screen saver milestone
            res = CL.IEX.MilestonesEPG.GetEPGInfo("ScreenSaver", out ScreenSaver);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get ScreenSaver Milestone from EPGINFO");
            }

            CL.IEX.Wait(1);

            if (ScreenSaver == Constants.scrnSaverActive) // verifying screen saver milestone is ON
            {
                LogCommentInfo(CL, "Screen Saver is  appeared  after " + (screenSaverWaitInSecs / 60).ToString() + " Mins");
            }
            else
            {

                FailStep(CL, "Failed to verify Screen Saver is  appeared  after " + (screenSaverWaitInSecs / 60).ToString() + " Mins");

               
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

            CL.IEX.MilestonesEPG.ClearEPGInfo();
            // verifying polling is happening or not in every 10 seconds 4 times
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    CL.IEX.MilestonesEPG.GetEPGInfo("Occupied disk space", out  hddUsagePercent);
                    if (hddUsagePercent.Trim() == "" || string.IsNullOrEmpty(hddUsagePercent))
                    {
                        FailStep(CL, "failed to Verify Polling is not happening in " + i.ToString() + " try");

                    }

                }
                catch
                {
                    LogCommentInfo(CL, "Verified Polling is not happening for " + i.ToString() + " Time");
                }

                CL.IEX.Wait(10);

               
            }
            PassStep();
        }
    #endregion

        #region Step3
        [Step(3, STEP3_DESCRIPTION)]
        private class Step3 : _Step
        {
            public override void Execute()
            {
                StartStep();
                res = CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp); // came out of screen saver
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to send RETOUR Key to come out fro screen saver");
                }

                CL.IEX.Wait(1);

                try
                {

                    CL.EA.UI.Utils.VerifyHDDIndicator(true); // verify HDD Indicator
                    LogCommentInfo(CL, "Verifed HDD Indicator after coming out from screen saver");
                }
                catch (Exception ex)
                {
                    FailStep(CL, "Failed to Verify HDD Indicator after coming out from screen saver." + ex.Message);
                }


                int hddUsagePercentInInt = 0;
                try
                {
                hddUsagePercentInInt=CL.EA.UI.Utils.GetHDDUsagePercentage();
                }
                catch(Exception ex)
                {
                    FailStep(CL,"Failed to get HDD Usage Percentage. "+ex.Message);
                }

                // verifying hdd usage  percentage
                if (IsWithRecording.ToUpper() == "YES")
                {
                   
                    if(hddUsagePercentInInt>0)
                    {
                        LogCommentInfo(CL,"Verified Hdd Usage Percentage after came out from screen saver");
                    }
                    else
                    {
                        FailStep(CL, "faile to Verify Hdd Usage Percentage after came out from screen saver");
                    }

                }
                else
                {
                    if (hddUsagePercentInInt ==0)
                    {
                        LogCommentInfo(CL, "Verified Hdd Usage Percentage after came out from screen saver");
                    }
                    else
                    {
                        FailStep(CL, "faile to Verify Hdd Usage Percentage after came out from screen saver");
                    }
                }


                PassStep();
            }
        }
        #endregion

    #endregion
    }
        #region PostExecute
        [PostExecute()]
        public override void PostExecute()
        {
            CL.EA.PVR.DeleteAllRecordsFromArchive();
        }
        #endregion
    }
