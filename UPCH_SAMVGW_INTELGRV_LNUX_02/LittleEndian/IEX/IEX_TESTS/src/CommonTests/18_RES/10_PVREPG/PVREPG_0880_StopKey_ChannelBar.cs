/// <summary>
///  Script Name : PVREPG_0880_StopKey_ChannelBar.cs
///  Test Name   : PVREPG-0880-Remote Control Key Stop-Current Channel Bar
///  TEST ID     : 63844
///  JIRA ID     : FC-303
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 7/12/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0880 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();
    private static Service recordingService2 = new Service();

    private static class Constants
    {
        public const int minTimeForEventToEnd = 4;
        public const int timeToWaitForStateChange = 10;
        public const int timeToPressKey = -1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: To stop on-going recording through a specific remote control key from channel bar & programme grid.
        //Pre-conditions: There is currently a record on-going on channel S1 & S2
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1 & S2 and Record Event from Banner on S1 & S2");
        this.AddStep(new Step1(), "Step 1: On live viewing of service S1 display the channel bar and press STOP key on current event ");
        this.AddStep(new Step2(), "Step 2: Tune to service S2 and display the programme grid and press STOP key on current event");

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

            //Get Values From xml File
            recordingService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True", "IsDefault=True;ParentalRating=High");
            if (recordingService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            recordingService2 = CL.EA.GetServiceFromContentXML("IsRecordable=True", "IsDefault=True;LCN=" + recordingService1.LCN);
            if (recordingService2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
            }

            //Start recording an event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecordFromBanner", Constants.minTimeForEventToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Event");
            }

            //Tune to service2
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService2.LCN);
            }

            //Start recording an event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecordFromBanner1", Constants.minTimeForEventToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Event");
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
            string timeStamp = "";

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
            }

            //Navigate to Channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Channel Bar");
            }

            //Stop Recording
            res = CL.IEX.SendIRCommand("STOP", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send STOP key to stop recording");
            }

            //Take 2 second for changing the state from CHANNEL BAR to CONFIRM DELETE
            res = CL.IEX.Wait(Constants.timeToWaitForStateChange);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for changing state from CHANNEL BAR to CONFIRM DELETE");
            }

            //CONFIRM STOP RECORDING, by selecting YES
            res = CL.IEX.MilestonesEPG.SelectMenuItem("YES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording");
            }

            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording");
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
            string timeStamp = "";

            //Tune to service2
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService2.LCN);
            }

            // Navigate to TV GUIDE
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate TV GUIDE");
            }
			
			//Take 2 second for changing the state from MAIN MENU to TV GUIDE
            res = CL.IEX.Wait(Constants.timeToWaitForStateChange);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for changing state from MAIN MENU to TV GUIDE");
            }

            //Stop Recording
            res = CL.IEX.SendIRCommand("STOP", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send STOP key to stop recording");
            }

            //Take 2 second for changing the state from TV GUIDE to CONFIRM DELETE
            res = CL.IEX.Wait(Constants.timeToWaitForStateChange);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for changing state from TV GUIDE to CONFIRM DELETE");
            }

            //CONFIRM STOP RECORDING, by selecting YES
            res = CL.IEX.MilestonesEPG.SelectMenuItem("YES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording");
            }

            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording");
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

        //Delete All Recording from Archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to Delete recording because" + res.FailureReason);
        }
    }

    #endregion PostExecute
}