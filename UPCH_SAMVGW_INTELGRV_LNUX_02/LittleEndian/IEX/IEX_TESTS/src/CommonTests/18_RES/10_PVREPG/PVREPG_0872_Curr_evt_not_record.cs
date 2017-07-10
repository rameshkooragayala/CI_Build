/// <summary>
///  Script Name : PVREPG_0872_Curr_evt_not_record
///  Test Name   : PVREPG-0872-Current_event_not_recordable
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 10/09/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0872 : _Test
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
    }

    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync and Tune to service S1");
        this.AddStep(new Step1(), "Step 1: Try to record the current event by preesing REC key from ChannelBar and Guide, And verify that recording is not possible");

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
            recordingService = CL.EA.GetServiceFromContentXML("IsRecordable=False;IsEITAvailable=True;Type=Video", "ParentalRating=High");
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
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Recording started on non recordable service");
            }
            else
            {
                LogCommentImportant(CL, "Unable to record the event, hence PRM descriptor of this service is no recording");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {

    }

    #endregion PostExecute
}