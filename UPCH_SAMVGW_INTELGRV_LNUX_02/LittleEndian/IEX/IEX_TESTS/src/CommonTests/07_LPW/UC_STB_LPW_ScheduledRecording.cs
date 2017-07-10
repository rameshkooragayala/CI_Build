/// <summary>
///  Script Name : UC_STB_LPW_ScheduledRecording.cs
///  Test Name   : UC-STB-0011-Navigation,UC-STB-0012-Navigation
///  TEST ID     : 73820,73821
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Madhu Renukaradhya
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("UC_STB_LPW_ScheduledRecording")]
public class UC_STB_LPW_ScheduledRecording : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
   
    //Variables which are used in different steps
    private static string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded
    private static int startGuardTimeInt = 0;
    private static int endGuardTimeInt = 0;
    private static string sgtFriendlyName;
    private static string egtFriendlyName;

    static string powerMode = "";
    static string defaultPowerMode = "";
    static string period = "";
    static string defaultPeriod = "";
    static string standyBy = "";
    static Int32 noOfPresses = 0;
    static string minDelayForBegin = "";
    static Int32 minDelayForBeginInt = 0;

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch Service from Content XML, Set GT's,Power modes and Book a future Recording";
    private const string STEP1_DESCRIPTION = "Step 1: Schedule a recording ";
    private const string STEP2_DESCRIPTION = "Step 2: Switch the box to standby and Verify that box goes to stand by only after recording is completed ";
    
    
    private static class Constants
    {
         public const int totalDurationInMin = 10;
    }
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
           
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
            
            //Get Values From ini File
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch recordableService" + recordableService.LCN + "from content xml.");
            }
            else
            {
                LogCommentInfo(CL, "RecordableService fetched from content xml is : " + recordableService.LCN);
            }

            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch SGT Value from Test ini");
            }
            else
            {
                LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);
            }
            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");
            if (egtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch EGT Value from Test ini");
            }
            else
            {
                LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);
            }

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }


            ////Get power mode Value to be set From test ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

            //Get default power mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini", true);
            }

            //Get period to be set From test ini File
            period = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "PERIOD");
            if (string.IsNullOrEmpty(period))
            {
                FailStep(CL, res, "Unable to fetch the period from test ini file");
            }

            //Get minDelay to be set From test ini File
            minDelayForBegin = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MIN_DELAY_UNTIL_BEGINING");
            if (string.IsNullOrEmpty(minDelayForBegin))
            {
                FailStep(CL, res, "Unable to fetch the period from test ini file");
            }

            minDelayForBeginInt = Convert.ToInt32(minDelayForBegin);

            //Get default period from project ini
            defaultPeriod = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_PERIOD");
            if (string.IsNullOrEmpty(defaultPeriod))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            //Get standby value to be set from test ini File
            standyBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STANDBY");
            if (string.IsNullOrEmpty(standyBy))
            {
                FailStep(CL, res, "Unable to fetch the standBy value from test ini file");
            }
               
           
            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully");
            }

            //set default to any available period
            res = CL.EA.STBSettings.ActivateAutoStandByAfterTime(period);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + period);
            }
            else
            {
                LogCommentInfo(CL, "Default Period for Stand By set successfully to: " + period);
            }

            //set default to any available standBy
            res = CL.EA.STBSettings.SetAutoStandBy(standyBy);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the option" + standyBy);
            }
            else
            {
                LogCommentInfo(CL, "Default StandBy set successfully to: " + standyBy);
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

            noOfPresses = minDelayForBeginInt/Int32.Parse(recordableService.EventDuration);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + recordableService.LCN);
            }

            //Book a record from guide
            res = CL.EA.PVR.BookFutureEventFromGuide(eventToBeRecorded, recordableService.LCN, NumberOfPresses: noOfPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book future Event from Guide");
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

            string currentTime = "";
             //Refreeshing the EPG to get the Current time from EPG
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to main menu", exitTest: false);
            }
            //Get Current EPG Time
            CL.EA.UI.Live.GetEpgTime(ref currentTime);
            if (string.IsNullOrEmpty(currentTime))
            {
                FailStep(CL, "Failed to Get the Current time from LIVE");
            }
            else
            {
                LogComment(CL, "Event Current time is " + currentTime);
            }

            //Getting Event end time from the event collection

            string evtStartTime = CL.EA.GetEventInfo(eventToBeRecorded, EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved Start time from event info is null");
            }
            else
            {
                LogComment(CL, "Event Start time is " + evtStartTime);
            }

            //Getting Event start time from the event collection

            string evtEndTime = CL.EA.GetEventInfo(eventToBeRecorded, EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            else
            {
                LogComment(CL, "Event End time is " + evtEndTime);
            }


            //verify set power mode
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode, jobPresent:true,StartTime:evtStartTime,EndTime:evtEndTime,currEPGTime:currentTime);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to verify power mode option");
            }
            else
            {
                LogCommentInfo(CL, "Set Power mode verified Successfully");
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
        //Restore default settings
        IEXGateway._IEXResult res;

        //Restore the period
        res = CL.EA.STBSettings.ActivateAutoStandByAfterTime(defaultPeriod);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default PERIOD");
        }
        else
        {
            LogCommentInfo(CL, "Restored to default PERIOD SUCCESSFULLY");
        }

        //Restore the power mode
        res = CL.EA.STBSettings.SetPowerMode(defaultPowerMode);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default POWER MODE");
        }
        else
        {
            LogCommentInfo(CL, "Restored to default POWER MODE SUCCESSFULLY");
        }

     
        //Delete the recording
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Event based recording from Archive");
        }
        //Fetch the SGT and EGT default values
        String defSgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (String.IsNullOrEmpty(defSgtValueInStr))
        {
            LogCommentFailure(CL, "Default SGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default SGT value in minutes - " + defSgtValueInStr);

        String defEgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (String.IsNullOrEmpty(defEgtValueInStr))
        {
            LogCommentFailure(CL, "Default EGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default EGT value in minutes - " + defEgtValueInStr);

        //Set SGT & EGT to default
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: defSgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set Start Guide time - " + defSgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: defEgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set End Guide time - " + defEgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
        
    }
    #endregion
}