/// <summary>
///  Script Name : FT191_001_Verify_Usage_Indicatorcs.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by :  Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT191_001_Verify_Usage_Indicatorcs")]
public class FT191_001 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;
    static string recordingType = "";
    static string view = "";
    static string timeStamp;
    static string title = "";
    static int hddPercentInDiagnostics;
    static int hddPercentInRecording;
    static int hddPercentInPlanner;
    static string  modifyRFMCommand;
    //Shared members between steps
    static string FTA_Channel;
    static Service recordableService;
    private const string PRECONDITION_DESCRIPTION = "Precondition:Fetch Value from Test INI, select view & Do Recording according to Test.INI";
    private const string STEP1_DESCRIPTION = "Step 1:GO to Diagnostics and check HDD Usage Percentage";
    private const string STEP2_DESCRIPTION = "Step 2:GO to MY Recording and Check HDD Usage Indicator & Verify Percentage";
    private const string STEP3_DESCRIPTION = "Step 3:GO to MY Planner and Check HDD Usage Indicator & Verify Percentage";
    

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
           



            // fetching recordable service from content XML
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch recordableService" + recordableService.LCN + "from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "RecordableService fetched from content xml is : " + recordableService.LCN);
            }


            // fetch view from Test INI
            view = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "VIEW");
            if (view == "" || string.IsNullOrEmpty(view))
            {
                FailStep(CL, "Failed to fetch VIEW value from TESTINI");
            }

            //Fetch record type from cintent XML
            recordingType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RECORDTYPE");
            if (recordingType == "" || string.IsNullOrEmpty(recordingType))
            {
                FailStep(CL, "Failed to fetch RECORD TYPE value from TESTINI");
            }



            res = CL.EA.TuneToChannel(recordableService.LCN, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Tune to channel " + recordableService.LCN);
            }


                      
            // Perform Recording & Change View according to Test INI 
            
            switch (recordingType)
            {
                case "NORECORDING":
                    {
                         res = CL.EA.ReturnToLiveViewing(false);
                        CL.IEX.Wait(1);
                        break;
                    }
                case "CURRENT":
                    {
                        // Record current event from banner
                        res = CL.EA.PVR.RecordCurrentEventFromBanner("SD_Event", 5, false, false);
                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, res, "Failed to Book the SD_Event From banner");
                        }


                    if(view.ToUpper()=="MOSAIC") // By default view will be LIST. changing view to mosaic according to Test INI
                    {
                        CL.EA.UI.ArchiveRecordings.Navigate();
                       

                        CL.IEX.Wait(2);

                        res=CL.IEX.SendIRCommand("SELECT_RIGHT", -1, ref timeStamp);
                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, "Failed To navigate to REFINE");
                        }

                        CL.IEX.Wait(2);

                        CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);

                        CL.IEX.Wait(2);
                        int count = 0;
                        while (title.ToUpper() != view.ToUpper())
                        {
                            res = CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
                            if (!res.CommandSucceeded)
                            {
                                FailStep(CL, "Failed To navigate to REFINE");
                            }
                            CL.IEX.Wait(1);
                            CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
                            count++;
                            if (count > 6)
                            {
                                break;
                            }

                        }

                        if (count > 6)
                        {
                            FailStep(CL,"Failed to select MOSAIC from Menu");
                        }
                        else
                        {
                            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                        }

                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, "Failed To select MOSIAC View");
                        }
                        
                    }

                        break;
                    }
                case "FUTURE":
                    {
                        res = CL.EA.PVR.BookFutureEventFromBanner("SD_Event", 2, 5, false);
                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, res, "Failed to Book the SD_Event From banner");
                        }

                        break;
                    }

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

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            CL.IEX.Wait(1);

            CL.EA.UI.Utils.NavigateToDiagnostics();

            CL.IEX.Wait(2);

            try
            {
              hddPercentInDiagnostics=  CL.EA.UI.Utils.GetHDDUsagePercentage(); //getting HDD usage Indocator

                CL.IEX.LogComment("HDD Usage Percentage is " + hddPercentInDiagnostics.ToString());
            }
            catch
            {
                FailStep(CL, "Failed to fetch HDD Usage Percentage from Diagnostics");

            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            CL.IEX.Wait(2);

            res = CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp); // going back to main menu
            CL.IEX.Wait(1);
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
            CL.IEX.Wait(2);
            // navigating to my recording
            CL.EA.UI.ArchiveRecordings.Navigate(); 


            CL.IEX.Wait(3);

            try
            {

                CL.EA.UI.Utils.VerifyHDDIndicator(true); // verify HDD Indicator
                LogCommentInfo(CL, "Verifed HDD Indicator is false in My Recording Screen");
            }
            catch(Exception ex)
            {
                FailStep(CL, "Failed to Verify HDD Indicator in My Recording Screen. " +ex.Message );
            }

            // Occupied disk usage milestone

            hddPercentInRecording = CL.EA.UI.Utils.GetHDDUsagePercentage(isClearEPG:false);

           
            if (recordingType == "CURRENT") // if current recording verify HDD indicator is false in refine screen
            {
                // Moving to REFINE Page

            if(view.ToUpper() == "MOSAIC") // if mosaic view  move down else move right
            {
                res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate to REFINE");
                }
            }
            else
            {
                res = CL.IEX.SendIRCommand("SELECT_RIGHT", -1, ref timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed To navigate to REFINE");
                }
            }

            CL.IEX.Wait(1);

                // checking HDD indicator is not visible in REFINE Page
            try
            {
               CL.EA.UI.Utils.VerifyHDDIndicator(false); // verify HDD Indicator
               LogCommentInfo(CL, "Verifed HDD Indicator is false in Refine Screen");
            }
            catch
            {
                FailStep(CL, "Failed to Verify HDD Indicator is false in Refine Screen");
            }

           
            //Checking hdd Percentage with diagnostics HDD Percentage. 

            if (hddPercentInDiagnostics <= hddPercentInRecording)
            {
                LogCommentInfo(CL, "Verifed HDD Usage Percentage in My Recordings");
            }
            else
            {
                FailStep(CL, "Failed to verify Percentage in My Recording");
            }
               

            }
            else
            {
                if (hddPercentInDiagnostics == hddPercentInRecording)
                {
                    LogCommentInfo(CL, "Verifed HDD Usage Percentage in My Recordings");
                }
                else
                {
                    FailStep(CL, "Failed to verify Percentage in My Recording");
                }
               
            }

           CL.IEX.Wait(1);

            CL.EA.ReturnToLiveViewing();

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
          
            CL.EA.UI.FutureRecordings.Navigate(); // Navigating to Planner
            CL.IEX.Wait(1);

            try
            {
                CL.EA.UI.Utils.VerifyHDDIndicator(true); // verify HDD Indicator
                LogCommentInfo(CL, "Verifed HDD Indicator is true in My Planner Screen");
            }
            catch
            {
                FailStep(CL, "Failed to Verify HDD Indicator is true in My Planner Screen");
            }


            hddPercentInPlanner = CL.EA.UI.Utils.GetHDDUsagePercentage();
            if (hddPercentInDiagnostics == hddPercentInPlanner)
            {
                LogCommentInfo(CL, "Verifed HDD Usage Percentage in My Planner");
            }

            else
            {
                FailStep(CL, "Failed to verify Percentage in My Planner");
            }


            // checking hdd inidcator is false in NEW RECORD Screen
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To navigate to NEW RECORD Screen in Planner");
            }
            CL.IEX.Wait(1);

            try
            {
                CL.EA.UI.Utils.VerifyHDDIndicator(false); // verify HDD Indicator
                LogCommentInfo(CL, "Verifed HDD Indicator is false in NEW RECORD");
            }
            catch
            {
                FailStep(CL, "Failed to Verify HDD Indicator in NEW RECORD");
            }

           
               


            PassStep();
        }
    }
    #endregion
   
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        if (view.ToUpper() == "MOSAIC")
        {

            CL.EA.UI.ArchiveRecordings.Navigate();


           
            CL.IEX.Wait(1);
            int count = 0;
            while (title.ToUpper() != "LIST")
            {
                 CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
               
                CL.IEX.Wait(1);
                CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
                count++;
                if (count > 6)
                {
                    break;
                }

            }

            CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            CL.IEX.Wait(1);
                        

        }
        switch (recordingType)
        {
           
            case "CURRENT":
                {

                    CL.EA.UI.ArchiveRecordings.DeleteAll();
                   
                    break;
                }
            case "FUTURE":
                {
                    CL.EA.UI.FutureRecordings.DeleteAll();

                    break;
                }

        }

    }
    #endregion
}