﻿/// <summary>
///  Script Name        : LIB_1004_AdultSession_Terminates_StartingPlayback.cs
///  Test Name          : LIB-1004-Adult session Terminates on starting playback
///  TEST ID            : 23620
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 22nd Sept. 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_1004 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service lockedService;
    private static Service Service1;
    private static string defaultPIN;
    private static string selectKeyName;
    private static string lockedEventName;
    private static string timeStamp = "";
    private static string eventName;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Locked Service From xml and unlock the service and record two Events");
        this.AddStep(new Step1(), "Step 1: Playback one of the recording by entering proper pin and while playing back navigate to Library and verify that the Adult session is terminated");
        this.AddStep(new Step2(), "Step 2: Stop Playback and verify that the Adult session is terminated and Pin screen is prompted");

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
            //Get PIN code
            defaultPIN = CL.EA.UI.Utils.GetValueFromEnvironment("DefaultPIN");

            //Get key names from Project.ini
            selectKeyName = CL.EA.UI.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY");
            //Get locked adult item name in 'My recordings'
            lockedEventName = CL.EA.UI.Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT");
            //Get the Locked service from Content XML
            lockedService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;ParentalRating=High", "");
            if (lockedService == null)
            {
                FailStep(CL, "Failed to get service with high Parental Rating from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + lockedService.LCN);
            }

            Service1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service1 == null)
            {
                FailStep(CL, "Failed to get service with high Parental Rating from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service1.LCN);
            }
            //Tune to the locked service
            res = CL.EA.TuneToLockedChannel(lockedService.LCN, CheckForVideo: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tuned to Locked Service");
            }
            //Record Current event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording", MinTimeBeforeEvEnd: 2, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner on " + lockedService);
            }
            //Navigating to channel bar next
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Expected to Fail as it is locked service so we need to enter pin so it will fail");
            }

            CL.IEX.Wait(2);

            //Check the PIN is asked
            if (!CL.EA.UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL"))
            {
                FailStep(CL, res, " PIN must be asked to open an adult session");
            }

            //Enter PIN
            CL.EA.UI.Utils.EnterPin(defaultPIN);
            res = CL.EA.PVR.BookFutureEventFromBanner("FutureEventBasedRecording", VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event from Banner");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service1.LCN);
            }
            //Stop the recording from Archive
            res = CL.EA.PVR.StopRecordingFromArchive("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop the record from Archive");
            }

            res = CL.EA.WaitUntilEventStarts("FutureEventBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event starts");
            }
            res = CL.IEX.Wait(seconds: 300);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds after the recording starts");
            }
            res = CL.EA.PVR.StopRecordingFromArchive("FutureEventBasedRecording");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop the record from Archive");
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
            //playback the record from Archive
            res = CL.EA.PVR.PlaybackRecFromArchive_LockedEvent("FutureEventBasedRecording", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the locked event from Archive");
            }
            CL.IEX.Wait(10);
            //Navigate to 'My recordings'
            res = CL.EA.PVR.NavigateToArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Could not navigate to 'My recordings'");
            }
       
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get selected Event Name from 'My recordings' list");
            }
            if (eventName != lockedEventName)
            {
                FailStep(CL, "Adult Session is not terminated after playing the recording");
            }
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select down in My Recordings");
            }
            //Check selected item is no more locked           
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get selected Event Name from 'My recordings' list");
            }
            if (eventName != lockedEventName)
            {
                FailStep(CL, "Adult session is not terminated after playing the recording");
            }
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to play back record from Archive");
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
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop the playback");
            }
            CL.IEX.Wait(10);
            //Navigate to 'My recordings'
            res = CL.EA.PVR.NavigateToArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Could not navigate to 'My recordings'");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get selected Event Name from 'My recordings' list");
            }
            if (eventName != lockedEventName)
            {
                FailStep(CL, "Adult Session is not terminated after stopping the play back of  the recording");
            }
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select down in My Recordings");
            }
            //Check selected item is no more locked           
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get selected Event Name from 'My recordings' list");
            }
            if (eventName != lockedEventName)
            {
                FailStep(CL, "Adult session is not terminated after stopping the playback of the recording");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to Live Veiwing");
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
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete all the records from Archive");
        }
    }

    #endregion PostExecute
}
