/// <summary>
///  Script Name : PVREPG_0879_StopKey_Live.cs
///  Test Name   : PVREPG-0879-Remote Control Key Stop-Current Live
///  TEST ID     : 63843
///  JIRA ID     : FC-302
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 7/12/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0879 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService;

    private static class Constants
    {
        public const int minTimeForEventToEnd = 1;
        public const int timeToWaitForStateChange = 2;
        public const int timeToPressKey = -1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: To stop on-going recording through a specific remote control key.
        //Pre-conditions:  There is currently a record on-going on this channel S1
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync,Tune to service S1 and Record Event from Banner");
        this.AddStep(new Step1(), "Step 1: Stop Recording, By pressing STOP key");

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
            recordingService = CL.EA.GetServiceFromContentXML("IsRecordable=True", "IsDefault=True;ParentalRating=High");
            if (recordingService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            //Tune to channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + recordingService.LCN);
            }

            // Start recording an event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecordFromBanner", Constants.minTimeForEventToEnd);
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

            //Stop Recording
            res = CL.IEX.SendIRCommand("STOP", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send STOP key to stop recording");
            }

            //Take 2 second for changing the state from LIVE to CONFIRM DELETE
            res = CL.IEX.Wait(Constants.timeToWaitForStateChange);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for changing state from LIVE to CONFIRM DELETE");
            }

            //CONFIRM STOP RECORDING, by selecting YES
            res = CL.IEX.MilestonesEPG.SelectMenuItem("YES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to highlight menu item YES");
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