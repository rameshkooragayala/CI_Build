/// <summary>
///  Script Name : PVERR-0075-Restarted by user.cs
///  Test Name   : PVERR_0075_Restarted_by_user
///  TEST ID     : 71572
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified date: 11.11.2013
/// </summary>

using System;
using System.Linq;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("PVERR_0075")]
public class PVERR_0075 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service recordChannel;
    static string endGuardTime;
    static string evtDuration;
    static bool isResuming = false;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Record an Event 2 mins after the Event Starts and Stop and record again";
    private const string STEP2_DESCRIPTION = "Step 2: Verify if the Partial Icon is displayed with proper error description";

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
        public const double waitTimeAfterEventStarts = 180; //in secs
        public const int minTimeBeforeEvtEnd = -1;
        public const bool verifyInPCAT = false;
        public const double timeToRecord = 30; // in secs
        public const double waitTimeInStandby = 10; //in secs
        public const bool egtToSet = false;
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

            //get the values from Test.ini
            evtDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EVENT_DURATION");
            if (int.Parse(evtDuration) < 10)
            {
                FailStep(CL, "Event Duration fetched from Test.ini should be more or equal to 10 mins");
            }

            //Get Values From xml File
            recordChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;IsRecordable=True;EventDuration=" + evtDuration, "ParentalRating=High");
            if (recordChannel == null)
            {
                FailStep(CL, "Failed to get Channel from Content.xml for the passed criterion");
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
            res = CL.EA.STBSettings.SetGuardTime(Constant.egtToSet, endGuardTime);
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

            int timeLeftForEventToEnd = 0;

            //Surf to Recordable Channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel" + recordChannel.LCN);
            }

            //get current event left time
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftForEventToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Left Time");
            }

            //Go to standBy
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to StandBy");
            }

            //Wait till some starting part is missing from recording.
            res = CL.IEX.Wait(Convert.ToDouble(timeLeftForEventToEnd) + Constant.waitTimeAfterEventStarts);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the event start & " + Constant.waitTimeAfterEventStarts.ToString() + " secs to elapsed");
            }

            //Exit from StandBy
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to exit from StandBy");
            }

            //Record Current Event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("recEvent", Constant.minTimeBeforeEvtEnd, isResuming, Constant.verifyInPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from Banner");
            }

            //Record for 30 secs
            res = CL.IEX.Wait(Constant.timeToRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait for "+Constant.timeToRecord+" secs to record");
            }

            //stop the recording
            res = CL.EA.PVR.StopRecordingFromBanner("recEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording in Channel: " + recordChannel.LCN);
            }

            //going to standby to ensure that no content is available in RB
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to StandBy");
            }

            //waiting for 3 min in Standby
            LogCommentInfo(CL, "Waiting for " + Constant.waitTimeInStandby + " secs in StandBy");
            res = CL.IEX.Wait(Constant.waitTimeInStandby);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in StandBy");
            }

            //Exiting from standby
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to exit from StandBy");
            }

            isResuming = true;

            //Record the same event after exiting from standby
            res = CL.EA.PVR.RecordCurrentEventFromBanner("recordSameEvent", Constant.minTimeBeforeEvtEnd, isResuming, Constant.verifyInPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Same Event after exiting from StandBy in Channel: " + recordChannel.LCN);
            }

            //wait for event to complete
            res = CL.EA.WaitUntilEventEnds("recordSameEvent",endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait untill the event recording is completed");
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

            //Verify The Record Error Info
            res = CL.EA.PVR.VerifyRecordErrorInfo("recEvent", EnumRecordErr.Partial_ResumedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Recording Error Information");
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

        //delete the recorded event from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to Delete Recording from Archive");
        }
        //get value for default EGT from project.ini
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }
        //set EGT to default
        res = CL.EA.STBSettings.SetGuardTime(Constant.egtToSet, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }
    }
    #endregion
}