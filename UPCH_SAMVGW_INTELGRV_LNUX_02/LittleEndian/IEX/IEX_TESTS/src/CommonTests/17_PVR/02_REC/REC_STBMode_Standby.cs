/// <summary>
///  Script Name : REC_STBMode_Standby.cs
///  Test Name   : REC-0060-STB mode - hot standby,REC-0062-STB mode - lukewarm standby,REC-0063-STB mode - cold standby,REC-0064-STB mode - standby delay
///  TEST ID     : 73388,73389,73390,73391
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : Unified_ATP_For_HMD_Cable
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 10th Feb, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_STB_Standby : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    private static string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded
    private static string standByMode;//STB Standby mode which we will be fetching from Test ini
    private static string timeInStandBy;//Time to Stanby which we will be fetching from Test ini
    private static double timeToWait = 0;//Total time to wait in Stand By

    //Constants used in the test
    private static class Constants
    {
        public const int startTimeDelayInMin = 5;//Delay which is added to the Start time for EA in Minutes
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Fetch the Channel Numbers From Content XML File");
        this.AddStep(new Step1(), "Step 1: Book a future Time based recording");
        this.AddStep(new Step2(), "Step 2: Enter Standby and wait till the recording is completed");
        this.AddStep(new Step3(), "Step 3: Verify the Status and playback the recording");

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
            //Fetcing a recordable service
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
            }

            //Fetching the Stand By mode and Time in Standby from Test ini
            standByMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STANYBYMODE");
            if (string.IsNullOrEmpty(standByMode))
            {
                FailStep(CL,"Failed to fetch the Stand by mode from test ini");
            }

            timeInStandBy = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TIME_IN_STANDBY");
            if (string.IsNullOrEmpty(timeInStandBy))
            {
                FailStep(CL, "Failed to fetch the Time in Stand by from test ini");
            }

            res = CL.EA.STBSettings.SetPowerMode(standByMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the power mode to " + standByMode);
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
            string currentTime = "";
            //Booking a recording after the Delay which is fetched from test ini
            res = CL.EA.PVR.RecordManualFromPlanner(eventToBeRecorded,recordableService.Name,DaysDelay:-1,MinutesDelayUntilBegining:(Convert.ToInt32(timeInStandBy)+Constants.startTimeDelayInMin) ,DurationInMin:5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record manual from planner");
            }
            //Getting Event info from the event collection
            string evtEndTime = CL.EA.GetEventInfo(eventToBeRecorded, EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event End time is " + evtEndTime);

            //Get Current EPG Time
            CL.EA.UI.Live.GetEpgTime(ref currentTime);
            if (string.IsNullOrEmpty(currentTime))
            {
                FailStep(CL,"Failed to Get the EPG time from LIVE");
            }

            LogComment(CL, "Current time is " + currentTime);
            //calculating time to wait in Stand By as we cant use Wait until Event Ends in Stand By
            timeToWait = (Convert.ToDateTime(evtEndTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds;
            LogComment(CL, "Time to wait is " + timeToWait);


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
            res = CL.EA.StandBy(IsOn:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter stand by");
            }
            //Entering the Stanby and Waiting till the recording is completed and exiting the standby
            res = CL.IEX.Wait(seconds:timeToWait);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait in Standby Until recording is completed");
            }

            res = CL.EA.StandBy(IsOn: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to exit stand by");
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
            //Verifying that the Event which is recorded is a complete recording
            res = CL.EA.PCAT.VerifyEventPartialStatus(eventToBeRecorded, Expected:"ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event partail status");
            }
            //Verifying the recorded event is played back properly with proper AV
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from archive");
            }
            res = CL.EA.CheckForVideo(IsPresent: true, CheckFullArea: false, Timeout: 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for Video present while playBack");
            }
            res = CL.EA.CheckForAudio(IsPresent: true, Timeout: 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check Audio is present while PlayBack");
            }
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop Playback from Archive");
            }

            PassStep();
        }
    }

    #endregion Step2


    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete Record from Archive");
        }
        //Setting the Default power mode
        string defaultPowerMode=CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
        res = CL.EA.STBSettings.SetPowerMode(PowerModeOption: defaultPowerMode);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Set the Power mode to Default one"); 
        }
    }

    #endregion PostExecute
}