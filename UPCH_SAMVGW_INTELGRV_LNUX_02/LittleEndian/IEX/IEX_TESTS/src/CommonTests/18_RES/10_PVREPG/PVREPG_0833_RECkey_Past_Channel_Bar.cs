/// <summary>
///  Script Name : PVREPG_0833_RECkey_Past_Channel_Bar
///  Test Name   : PVREPG-0833-REC key record-Past-Channel Bar
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 09/09/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0833 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();
    private static String obtainedEventtime = "";
    private static String expectedEventtime = "";
    private static String obtainedEventname = "";
    private static String expectedEventname = "";
    
    private static class Constants
    {
        public const int minTimeForEventToEnd = 3;
        public const int trickModeSpeed = -2;
        public const int play = 1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        
        //Pre-conditions: There is currently a record on-going on channel S1 and also future event is booked
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1");
        this.AddStep(new Step1(), "Step 1:Tune to srvice, wait till end of next event and Rewind Review buffer and play event A from Review Buffer. ");
        this.AddStep(new Step2(), "Step 2: Raise Action Bar and start recording using REC key");

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

            //Get Values From xml File, +ve criteria as IsEIT, video, IsSeries as True
            recordingService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video", "IsDefault=True;ParentalRating=High");
            if (recordingService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
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

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
            }

            int TimeLeftInSec = 0;
            //Getting the Current Event Time Left 
            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To get Current Event Time left");
            }

            if (TimeLeftInSec < 180)
            {

                LogComment(CL, "Returning to Live viewing from Action Bar Launched During GetCurrentEventLeftTime");
                res = CL.EA.ReturnToLiveViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Return to Live Viewing");
                }

                res = CL.IEX.Wait(TimeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Wait till evt end time");
                }

                res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To get Current Event Time left");
                }
            }
            
            res = CL.IEX.Wait(TimeLeftInSec + 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait till evt end time");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Return to Live Viewing");
            }

            //Fetching the Event time from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out expectedEventtime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time");
            }
            //Fetching the Event Name from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out expectedEventname);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name");
            }

            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To get Current Event Time left");
            }

            res = CL.IEX.Wait(TimeLeftInSec + 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait till evt end time");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Return to Live Viewing");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.trickModeSpeed, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To rewind with speed: " + Constants.trickModeSpeed);
            }

            res = CL.IEX.Wait(TimeLeftInSec/4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait till evt end time");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
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

            //Launching Action Bar after Trick Mode to get the EGPinfo
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Action Bar");
            }
            //Fetching the Event time from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventtime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time");
            }
            //Fetching the Event Name from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventname);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name");
            }

            if ((expectedEventtime.Equals(obtainedEventtime)) && (expectedEventname.Equals(obtainedEventname)))
            {

                //Record current event using REC key
                res = CL.EA.PVR.RecordUsingRECkey(EnumRecordIn.ActionBar, "RecEvent", recordingService1.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to record an event");
                }
            }
            else
            {
                FailStep(CL, "Obtained Event info is different from Expected Event info after trick mode");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent", 30, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recorded event");
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

        //Cancel all booking from Planner
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all recordings because" + res.FailureReason);
        }
    }

    #endregion PostExecute
}