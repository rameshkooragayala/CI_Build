/// <summary>
///  Script Name : PVERR-0030-Event Status-Partial-more-2min-lost.cs
///  Test Name   : PVERR-0030-Event Status-Partial-more 2min lost
///  TEST ID     : startGuardTimeList
///  QC Version  : 1
///  Variations from QC: designed accept lost recording in middle
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 07.10.2010
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
 * Pvrerr 0030 a.
 *
 * @author Avinoba
 * @date 07-Oct-13
 */

[Test("PVRERR_0030_A")]
public class PVRERR_0030_A : _Test
{
    /**
     * The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * Shared members between steps.
     */

    static Service recordChannel_1;

    /**
     * The second record channel.
     */

    static Service recordChannel_2;

    /**
     * Duration of the event.
     */

    static string evtDuration;

    /**
     * The start time.
     */

    static string missingStartTime;

    /**
     * The end time.
     */

    static string missingEndTime;

    /**
     * The end guard time.
     */

    static string endGuardTime;

    /**
     * The second time left in channel.
     */

    static int timeLeftInChannel_1;

    /**
     * Information describing the precondition.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**
     * Event queue for all listeners interested in STEP1_DESCRIPTION events.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Record 2 event which has missing content more than 2 minutes at begining and other at the end";

    /**
     * Event queue for all listeners interested in STEP2_DESCRIPTION events.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Verify the Recorded Event Error Message in Action Bar and Partial Status in PCAT";

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

        public const int minTimeBeforeEvtEnds = -1;
        /**
         * The minimum time before event starts.
         */
        public const int minTimeBeforeEvtStarts = 2;

        /**
         * The is resuming.
         */

        public const bool isResuming = false;

        /**
         * The verify pcat.
         */

        public const bool verifyPCAT = false;

        /**
         * The number of presses.
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

            //Get Values from Test.ini
            missingStartTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MISSING_START_TIME");
            missingEndTime = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MISSING_END_TIME");

            //Get Values From xml File
            recordChannel_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High;IsDefault=True");

            recordChannel_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High;LCN=" + recordChannel_1.LCN);

            //Checking if the Channels required is null
            if (recordChannel_1 == null || recordChannel_2 == null)
            {
                FailStep(CL, "Failed to get Channel from Content.xml for the passed criterion");
            }

            LogCommentInfo(CL, "Channels fetched from Content.xml:" + recordChannel_1.LCN + "," + recordChannel_2.LCN);

            //get value from project.ini for EGT
            string endGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "LIST");
            if (string.IsNullOrEmpty(endGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }


            endGuardTime = endGuardTimeList.Split(',').First();

            //set EGT to first value of list
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

            int timeLeftForEventEnds = 0;

            //Tune to a recordable service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordChannel_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel:" + recordChannel_1.LCN);
            }

            //Get Current event left time for the event to end
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftInChannel_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Time left for event to end");
            }

            //Checking if the minimum Start time has elapsed
            if ((int.Parse(recordChannel_1.EventDuration) - (timeLeftInChannel_1 / 60.0)) <= int.Parse(missingStartTime))
            {
                //go to standby
                res = CL.EA.StandBy(false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to go to StandBy");
                }

                LogCommentInfo(CL, "Waiting in StandBy for event to elapse");
                //wait for missing time to elapse
                res = CL.IEX.Wait(Math.Abs(Convert.ToDouble(timeLeftInChannel_1) + (Convert.ToDouble(missingStartTime) * 60) - (Convert.ToDouble(recordChannel_1.EventDuration) * 60)));
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Fail to wait in StandBy");
                }

                //Exit from StandBy
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit to StandBy");
                }
            }

            //record the current event 
            res = CL.EA.PVR.RecordCurrentEventFromBanner("recEventMissingStart", Constant.minTimeBeforeEvtEnds, Constant.isResuming, Constant.verifyPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record current event on Channel: " + recordChannel_1.LCN);
            }

            //Record an event from starting of the event 
            res = CL.EA.PVR.BookFutureEventFromGuide("recEventMissingEnd", recordChannel_2.LCN, Constant.noOfPresses, Constant.minTimeBeforeEvtStarts, Constant.verifyPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book event on Channel: " + recordChannel_2.LCN);
            }

            //Tune to another recordable service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordChannel_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel:" + recordChannel_2.LCN);
            }

            //waiting till the booked event starts
            res = CL.EA.WaitUntilEventStarts("recEventMissingEnd");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for the booked event to start");
            }

            //Get Current event left time to end
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftForEventEnds);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Time left for recording event to end");
            }

            //waiting till desired time before the event ends
            LogCommentInfo(CL, "Waiting till " + missingEndTime + " mins before the event ends");
            res = CL.IEX.Wait((Convert.ToDouble(timeLeftForEventEnds) - (Convert.ToDouble(missingEndTime) * 60)) - 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till " + missingEndTime + " mins before the event ends");
            }

            //stop the recording of the current event
            res = CL.EA.PVR.StopRecordingFromBanner("recEventMissingEnd");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recordings From Banner");
            }

            //wait till the recorded event ends
            res = CL.EA.WaitUntilEventEnds("recEventMissingStart");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait till the event in Channel: " + recordChannel_1.LCN + " to end");
            }

            int endGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(endGuardTime, false);
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
            //verify the recorded error Information
            res = CL.EA.PVR.VerifyRecordErrorInfo("recEventMissingEnd",EnumRecordErr.Partial_UserStopped);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify the recording error Info in Archive");
            }

            LogCommentInfo(CL, "Verified the Recording Error Info of the event in Channel: " + recordChannel_2.LCN);

            //verify the recorded error Information
            res = CL.EA.PVR.VerifyRecordErrorInfo("recEventMissingStart",EnumRecordErr.Partial_RecordedAfterEvtStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify the recording error Info in Archive");
            }

            LogCommentInfo(CL, "Verified the Recording Error Info of the event in Channel: " + recordChannel_1.LCN);

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
        //Delete recordings from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recEventMissingStart");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to delete all events from Archive");
        }
        //Delete recordings from Archiv
        res = CL.EA.PVR.DeleteRecordFromArchive("recEventMissingEnd");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to delete all events from Archive");
        }

        //get default value from project.ini for EGT
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }

        //set SGT to default
        res = CL.EA.STBSettings.SetGuardTime(false, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }

    }
    #endregion

}