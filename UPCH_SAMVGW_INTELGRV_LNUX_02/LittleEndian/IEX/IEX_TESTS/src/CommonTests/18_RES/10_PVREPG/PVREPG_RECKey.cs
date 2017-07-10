/// <summary>
///  Script Name : PVREPG_RECKey.cs
///  Test Name   : PVREPG-0820, 0821, 0830, 0831 and 0850
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 27/08/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_RECKey : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService;
    private static EnumRecordIn recordIn;
    private static String recordScreen = "";

    private static class Constants
    {
        public const int minTimeForEventToEnd = 3;
        public const int secToPlay = 30;
    }

    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync and Tune to service S1");
        this.AddStep(new Step1(), "Step 1: Record the current event of service S1 by preesing REC key from Live, ChannelBar and Guide");
        this.AddStep(new Step2(), "Step 2: Book the future event of service s1 from ChannelBar and Guide, by pressing REC key");
        this.AddStep(new Step3(), "Step 3: PlayBack the recording");

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
            recordingService = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video", "IsDefault=True;ParentalRating=High");
            if (recordingService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            recordScreen = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "RECORD_SCREEN");

            recordIn = (EnumRecordIn)Enum.Parse(typeof(EnumRecordIn), recordScreen, true);

            //Tune to channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + recordingService.LCN);
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

            //Record current event using REC key
            res = CL.EA.PVR.RecordUsingRECkey(recordIn, "RecEvent", recordingService.LCN, Constants.minTimeForEventToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record an event");
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

            //Book future event from ChannelBar and Guide
            if (recordIn.Equals(EnumRecordIn.ChannelBar) || recordIn.Equals(EnumRecordIn.Guide))
            {
                res = CL.EA.PVR.RecordUsingRECkey(recordIn, "BookEvent", recordingService.LCN, Constants.minTimeForEventToEnd, IsCurrent: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book future event");
                }

                res = CL.EA.WaitUntilEventEnds("BookEvent");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till event end time");
                }
            }

            else
            {
                LogCommentImportant(CL, "Booking future event is not possible from " + recordScreen);

                res = CL.EA.WaitUntilEventEnds("RecEvent");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till event end time");
                }                
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

            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to playback the recording because" + res.FailureReason);
            }

            if (recordIn.Equals(EnumRecordIn.ChannelBar) || recordIn.Equals(EnumRecordIn.Guide))
            {
                res = CL.EA.PVR.PlaybackRecFromArchive("BookEvent", Constants.secToPlay, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to playback the recording because" + res.FailureReason);
                }           
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Delete All Recording from Archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to Delete recording because" + res.FailureReason);
        }
    }

    #endregion PostExecute
}