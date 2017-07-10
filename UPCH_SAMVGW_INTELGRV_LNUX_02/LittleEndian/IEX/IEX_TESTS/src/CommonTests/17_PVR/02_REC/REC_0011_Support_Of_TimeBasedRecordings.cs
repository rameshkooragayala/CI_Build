/// <summary>
///  Script Name : REC_0011_Support_Of_TimeBasedRecordings.cs
///  Test Name   : REC-0011-Support of time-based recordings
///  TEST ID     : 73377
///  QC Version  : 2
///  Variations from QC : NONE
///  Repository  : Unified_ATP_For_HMD_Cable
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 14th Feb, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_0011 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    private static Service radioService; //The Radio Service
    

    //Constants used in the test
    private static class Constants
    {
        public const int expectedErrorCode = 300;
        public const string expectedErrorString = "Failed To Set Manual Recording Channel To";
        public const string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded
        public const string timeBasedeRecording = "TIMEBASED_RECORDING"; //Event to be Recorded on Radio channel
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Fetch the Channel Numbers From Content XML File");
        this.AddStep(new Step1(), "Step 1: Start recording on the fetched video service and Playback the recording");
        this.AddStep(new Step2(), "Step 2: Try to do a recording on the Radio service");

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
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
            }
            //Fetcing a Radio service
            radioService = CL.EA.GetServiceFromContentXML("Type=Radio;IsEITAvailable=True", "ParentalRating=High");
            if (radioService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Radio Service fetched is : " + radioService.LCN);
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

            res = CL.EA.PVR.RecordManualFromPlanner(Constants.eventToBeRecorded,recordableService.Name,DaysDelay:-1,MinutesDelayUntilBegining:5,DurationInMin:3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record manual from planner");
            }
            res = CL.EA.WaitUntilEventEnds(Constants.eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event ends");
            }
            //Verifying that it is a Complete Recording
            res = CL.EA.PCAT.VerifyEventPartialStatus(Constants.eventToBeRecorded,Expected:"ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Verify the event Partial Status");
            }
            //Playing back the recording from Archive and verifying EOF
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventToBeRecorded, SecToPlay: 0, FromBeginning: true, VerifyEOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to playback record from Archive");
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
            LogCommentImportant(CL,"Trying to do a recording on a Radio service which should not be allowed");
            res = CL.EA.PVR.RecordManualFromPlanner(Constants.timeBasedeRecording, radioService.Name, DaysDelay: -1, MinutesDelayUntilBegining: 5, DurationInMin: 3);
            if (!res.CommandSucceeded)
            {
                if (res.FailureCode != Constants.expectedErrorCode && (!res.FailureReason.Contains(Constants.expectedErrorString)))
                {
                    FailStep(CL, res, "Expected Failure code " + Constants.expectedErrorCode + " and recieved Failure code are different " + res.FailureCode);
                }
            }
            else
            {
                FailStep(CL, "Successful in recording the Radio channel");
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
        res = CL.EA.PVR.DeleteRecordFromArchive(Constants.eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete Time based recording from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(Constants.timeBasedeRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete Time based recording from Archive");
        }
    }

    #endregion PostExecute
}