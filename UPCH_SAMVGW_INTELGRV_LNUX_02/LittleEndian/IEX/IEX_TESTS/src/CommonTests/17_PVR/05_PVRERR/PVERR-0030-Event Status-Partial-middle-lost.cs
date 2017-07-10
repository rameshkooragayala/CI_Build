/// <summary>
///  Script Name : PVERR-0030-Event Status-Partial-middle-lost.cs
///  Test Name   : PVERR-0030-Event Status-Partial-more 2min lost
///  TEST ID     : startGuardTimeList
///  QC Version  : 1
///  Variations from QC: Designed for lost recording in middle
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 07.10.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**
 * Pvrerr 0030 b.
 *
 * @author Avinoba
 * @date 07-Oct-13
 */

[Test("PVRERR_0030_B")]
public class PVRERR_0030_B : _Test
{
    /**
     * The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * Shared members between steps.
     */

    static Service recordChannel;

    /**
     * Duration of the event.
     */

    static string evtDuration;

    /**
     * The end guard time.
     */

    static string endGuardTime;
    /**
     * Information describing the precondition.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**
     * Event queue for all listeners interested in STEP1_DESCRIPTION events.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Record an Event which has missing content from middle of the Event";

    /**
     * Information describing the step 2.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Verify the Recording Status Error Info is Partial";
    #region Create Structure

    /**
     * Creates the structure.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

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

    /**
     * Constant.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    private static class Constant
    {
        /**
         * The minimum time before event ends.
         */

        public const int minTimeBeforeEvtEnds = 1;

        /**
         * The minimum time before event starts.
         */
        public const int minTimeBeforeEvtStart = 3;

        /**
         * The wait time in event.
         */

        public const double waitTimeInEvent = 3;

        /**
         * The wait time in standby.
         */

        public const double waitTimeInStandby = 3;

        /**
         * The verify pcat.
         */

        public const bool verifyPCAT = false;

        /**
        * The no Of Presses.
         */
        public const int noOfPresses = 1;
    }

    #region PreExecute

    /**
     * Pre execute.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**
     * Pre condition.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 07-Oct-13
         */

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
            recordChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;EventDuration=" + evtDuration, "ParentalRating=High");
            if (recordChannel == null)
            {
                FailStep(CL, "Failed to get Channel from Content.xml for the passed criterion");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + recordChannel.LCN);

            //get value from project.ini
            string endGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "LIST");

            if (string.IsNullOrEmpty(endGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }

            endGuardTime = endGuardTimeList.Split(',').First();

            //set EGT to fist value of the list
            res = CL.EA.STBSettings.SetGuardTime(false, endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set End Guard Time to " + endGuardTime);
            }
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * Step 1.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 07-Oct-13
         */

        public override void Execute()
        {
            StartStep();
            bool isResuming = false;

            //Book the event from Guide 
            res = CL.EA.PVR.BookFutureEventFromGuide("recordEvent", recordChannel.LCN, Constant.noOfPresses, Constant.minTimeBeforeEvtStart, Constant.verifyPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Event from Guide in Channel: " + recordChannel.LCN);
            }

            //tune to the recordable channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel: " + recordChannel.LCN);
            }

            //wait till the event starts
            res = CL.EA.WaitUntilEventStarts("recordEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for the booked event to start");
            }

            //waiting for 3 mins for event to elapse
            LogCommentInfo(CL, "Waiting for " + Constant.waitTimeInEvent + " mins for event begining to elapse");
            res = CL.IEX.Wait(Constant.waitTimeInEvent * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait");
            }

            //stop the recording
            res = CL.EA.PVR.StopRecordingFromBanner("recordEvent");
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
            LogCommentInfo(CL, "Waiting for " + Constant.waitTimeInStandby + " mins in StandBy");
            res = CL.IEX.Wait(Constant.waitTimeInStandby * 60);
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
            res = CL.EA.PVR.RecordCurrentEventFromBanner("recordSameEvent", Constant.minTimeBeforeEvtEnds, isResuming, Constant.verifyPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Same Event after exiting from StandBy in Channel: " + recordChannel.LCN);
            }

            //wait for event to complete
            res = CL.EA.WaitUntilEventEnds("recordSameEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait untill the event recording is completed");
            }

            int endGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(endGuardTime, false);

            LogCommentInfo(CL, "Waiting for " + endGuardTime + " mins");
            //waiting for end Guard time to end
            res = CL.IEX.Wait(Convert.ToDouble(endGuardTimeNum) * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the end guard time is completed");
            }
            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * Step 2.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 07-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //verify record error information
            res = CL.EA.PVR.VerifyRecordErrorInfo("recordEvent",EnumRecordErr.Partial_ResumedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify record error information");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute

    /**
     * Posts the execute.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //delete the recorded event from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recordEvent");
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
        res = CL.EA.STBSettings.SetGuardTime(false, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }

    }
    #endregion
}