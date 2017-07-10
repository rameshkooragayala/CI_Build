/// <summary>
///  Script Name        : SET_DELPVR_0003_Delete_All_Recordings_OngoingREC_Playback_FT146.cs
///  Test Name          : SET-DELPVR-0003-Delete_All_Recordings_OngoingREC_Playback_FT146
///  TEST ID            : 74649
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 04th Sept, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class FT146_0003 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service ongoingKeepTimeBasedRecordingService;
    private static Service recordedKeepTimeBasedRecordingService;
    private static Service ongoingUnkeeptimeBasedRecordingService;
    private static Service recordedUnKeeptimeBasedRecordingService;
    private static Service playbacktimeBasedRecordingService;
    private static string ongoingKeepTimeBasedRecording = "TIME_BASED_RECORDING";
    private static string recordedKeepTimeBasedRecording = "TIME_BASED_RECORDING2";
    private static string ongoingUnkeeptimeBasedRecording = "TIME_BASED_RECORDING3";
    private static string recordedUnKeeptimeBasedRecording = "TIME_BASED_RECORDING4";
    private static string playbacktimeBasedRecording = "TIME_BASED_RECORDING5";



    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml file and Sync");
        this.AddStep(new Step1(), "Step 1: Playback Recording 5,Delete all the recordings and  Verify the Recordings");

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

            ongoingKeepTimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High");
            if (ongoingKeepTimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + ongoingKeepTimeBasedRecordingService.LCN);
            }

            recordedKeepTimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + ongoingKeepTimeBasedRecordingService.LCN);
            if (recordedKeepTimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedKeepTimeBasedRecordingService.LCN);
            }

            ongoingUnkeeptimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + ongoingKeepTimeBasedRecordingService.LCN + "," + recordedKeepTimeBasedRecordingService.LCN);
            if (ongoingUnkeeptimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + ongoingUnkeeptimeBasedRecordingService.LCN);
            }

            recordedUnKeeptimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + ongoingKeepTimeBasedRecordingService.LCN + "," + recordedKeepTimeBasedRecordingService.LCN + "," + ongoingUnkeeptimeBasedRecordingService.LCN);
            if (recordedUnKeeptimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedUnKeeptimeBasedRecordingService.LCN);
            }

            playbacktimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + ongoingKeepTimeBasedRecordingService.LCN + "," + recordedKeepTimeBasedRecordingService.LCN + "," + ongoingUnkeeptimeBasedRecordingService.LCN + "," + recordedUnKeeptimeBasedRecordingService.LCN);
            if (playbacktimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + playbacktimeBasedRecordingService.LCN);
            }

            res = CL.EA.PVR.RecordManualFromCurrent(recordedKeepTimeBasedRecording,recordedKeepTimeBasedRecordingService.LCN,DurationInMin:3,VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record manual from current");
            }

            res = CL.EA.PVR.RecordManualFromCurrent(recordedUnKeeptimeBasedRecording, recordedUnKeeptimeBasedRecordingService.LCN, DurationInMin: 3, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from current");
            }

            res = CL.EA.PVR.RecordManualFromCurrent(playbacktimeBasedRecording, playbacktimeBasedRecordingService.LCN, DurationInMin: 5, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from current");
            }

            res = CL.EA.WaitUntilEventEnds(playbacktimeBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }

            res = CL.EA.PVR.SetKeepFlag(recordedKeepTimeBasedRecording, SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Keep flag to true");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedKeepTimeBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to find event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedUnKeeptimeBasedRecording,Navigate:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(playbacktimeBasedRecording, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find event in Archive");
            }

            res = CL.EA.PVR.RecordManualFromCurrent(ongoingUnkeeptimeBasedRecording, ongoingUnkeeptimeBasedRecordingService.LCN, DurationInMin: 10, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record Manual from current");
            }

            res = CL.EA.PVR.RecordManualFromCurrent(ongoingKeepTimeBasedRecording, ongoingKeepTimeBasedRecordingService.LCN, DurationInMin: 10, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record Manual from current");
            }
            //Wait until the event starts
            res=CL.EA.WaitUntilEventStarts(ongoingKeepTimeBasedRecording);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event starts");
            }
            LogCommentImportant(CL,"Set the keep flag to true");
            res = CL.EA.PVR.SetKeepFlag(ongoingKeepTimeBasedRecording, SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the keep flag");
            }
            res = CL.EA.PVR.VerifyEventInArchive(ongoingUnkeeptimeBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(ongoingKeepTimeBasedRecording, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find event in Archive");
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
            LogCommentImportant(CL,"Starting the play back of the recording 5");
            res = CL.EA.PVR.PlaybackRecFromArchive(playbacktimeBasedRecording, SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            res = CL.EA.PVR.DeleteAllRecordsFromArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to delete all records from Archive");
            }
            //All the recording should be deleted except the keep recordings and the recording which is played
            res = CL.EA.PVR.VerifyEventInArchive(recordedKeepTimeBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.EA.PVR.VerifyEventInArchive(ongoingKeepTimeBasedRecording, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.EA.PVR.VerifyEventInArchive(playbacktimeBasedRecording, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.EA.PVR.VerifyEventInArchive(recordedUnKeeptimeBasedRecording, Navigate: false, SupposedToFindEvent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.EA.PVR.VerifyEventInArchive(ongoingUnkeeptimeBasedRecording, Navigate: false, SupposedToFindEvent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
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
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to stop the playback from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(recordedKeepTimeBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(ongoingKeepTimeBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(playbacktimeBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete record from Archive");
        }
    }

    #endregion PostExecute
}
