/// <summary>
///  Script Name : PVERR_0202_Fully_Recorded_less_Than_2min_MissingEnd.cs
///  Test Name   : PVERR_0202_Fully_Recorded_less_Than_2min_MissingEnd
///  TEST ID     : 71889
///  QC Version  : 2
///  Variations from QC: Split into two. This script will verify for missing in end
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 25.11.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("PVERR_0202B")]
public class PVERR_0202B : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service recordChannel;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Book a future Recording and Stop recording less than 2 mins from the event ends";
    private const string STEP2_DESCRIPTION = "Step 2: Verify if the recorded event is complete";

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

    private static class Constant
    {
        public const double timeBeforeEvtEnds = 60; //in secs
        public const int minTimeBeforeEvtStart = 2;
        public const bool isResuming = false;
        public const bool verifyInPCAT = false;
        public const int noOfPresses = 1;
        public const bool copyPCAT = true;
    }

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
            recordChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
            if (recordChannel == null)
            {
                FailStep(CL, res, "Fail to fetched channel for the passed criterion");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + recordChannel.LCN);
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
            int timeLeftForEvtToEnd = 0;

            
            //Book future event from Guide
            res = CL.EA.PVR.BookFutureEventFromGuide("recEvent", recordChannel.LCN, Constant.noOfPresses, Constant.minTimeBeforeEvtStart, Constant.verifyInPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book future Event from Guide");
            }

            //Tune to the Booked Channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel: " + recordChannel.LCN);
            }

            //waiting till the recording event starts
            res = CL.EA.WaitUntilEventStarts("recEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the Event Starts");
            }
            //get current event time left
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftForEvtToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Event Left Time");
            }

            //waiting 1 min before the recording completes
            LogCommentInfo(CL, "Waiting 1 min before the event ends");
            res = CL.IEX.Wait(Convert.ToDouble(timeLeftForEvtToEnd) - Constant.timeBeforeEvtEnds);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till " + Constant.timeBeforeEvtEnds + " secs before the event ends");
            }

            //stop the recording
            res = CL.EA.PVR.StopRecordingFromBanner("recEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop recording from Banner");
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
            //verify if the recording status is full
            res = CL.EA.PCAT.VerifyEventPartialStatus("recEvent", "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify if the Recording is complete");
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
        IEXGateway._IEXResult res;
        //delete the recording from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + " ; Failed to Delete Recording from Archive");
        }
    }
    #endregion
}