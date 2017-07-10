/// <summary>
///  Script Name : PVERR_0202_Fully_Recorded_less_Than_2min_MissingStart.cs
///  Test Name   : PVERR_0202_Fully_Recorded_less_Than_2min_MissingStart
///  TEST ID     : 71889
///  QC Version  : 2
///  Variations from QC:Split into two. This script will verify for missing in start
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified date: 25.11.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("PVERR_0202A")]
public class PVERR_0202A : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service recordChannel;
    static String endGuardTime;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Record a Channel withib less than 2 min after the event starts";
    private const string STEP2_DESCRIPTION = "Step 2: Wait till the end of the event and verify if the recording is Full";

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
        public const int minTimeBeforeEvtEnds = -1;
        public const bool isResuming = false;
        public const bool verifyRecordingInPCAT = false;
        public const bool copyPCAT = true;
        public const bool EGTtoSet = false;
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

            //get value from project.ini for EGT
            string endGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "LIST");
            if (string.IsNullOrEmpty(endGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }


            endGuardTime = endGuardTimeList.Split(',').First();

            //set EGT to first value of list
            res = CL.EA.STBSettings.SetGuardTime(Constant.EGTtoSet, endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set End Guard Time to " + endGuardTime);
            }
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
            //tune to a recordable channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel: " + recordChannel.LCN);
            }

            //Get time left for event to end
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftForEvtToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Event Left Time");
            }

            double waitTimeInStandBy = Convert.ToDouble(timeLeftForEvtToEnd);

            LogCommentInfo(CL, "Waiting for " + waitTimeInStandBy + " secs in StandBy so that event is not present in RB");
            //wait in standby till the event starts
            res = CL.EA.FlushRB(waitTimeInStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in standBy untill the event starts");
            }
            //Record current event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("recEvent", Constant.minTimeBeforeEvtEnds, Constant.isResuming, Constant.verifyRecordingInPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record Current Event from Banner");
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

            //wait till the event recording ends
            res = CL.EA.WaitUntilEventEnds("recEvent",endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait till the recording is complete");
            }
            //verify that the status of the recording is full
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

        //delete event from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + " ; Failed to Delete Recording from Archive");
        }

        //get value for default EGT from project.ini
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }
        //set EGT to default
        res = CL.EA.STBSettings.SetGuardTime(Constant.EGTtoSet, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }
    }
    #endregion
}