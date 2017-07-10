/**
 * @file 18_RES\10_PVREPG\PVREPG_0875_RecordPastEvent.cs
 *
 * Implements the pvrepg 0875 record rb and playback class.
 * Script Name : PVREPG_0875_RecordPastEvent.cs
 * Test Name   : PVREPG-0875-Record-Past-Channel Bar 
 * JIRA ID      :FC-592
 * TEST ID      :68376
 * QC Version   :2
 * Variation from QC: Step 6 is not included as it is not part of requirement
 * --------------------------
 * Modified By  :Avinob Aich
 * Modified Date: 17/9/2013
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**
 * Pvrepg 0875.
 *
 * @author Avinoba
 * @date 10-Sep-13
 */

[Test("PVREPG_0875")]
public class PVREPG_0875 : _Test
{
    /**
     * The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * Shared members between steps.
     */

    static Service rbChannel;

    /**
     * The minimum start guard time.
     */

    static string minStartGuardTime;

    /**
     * The minimum end guard time.
     */

    static string minEndGuardTime;

    /**
     * The minimum time before event starts.
     */

    static int minTimeBeforeEvtStarts;

    /**
     * The minimum number end guard time.
     */

    static int minNumEndGuardTime;
    /**
     * Information describing the precondition.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**
     * Information describing the step 1.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Tune to a channel";

    /**
     * Event queue for all listeners interested in STEP2_DESCRIPTION events.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Wait for a Event for atleast  Start Guard Time mins before the Event Starts to Event Ends";

    /**
     * Information describing the step 3.
     */

    private const string STEP3_DESCRIPTION = "Step 3: Rewind back till the event which is present in RB";

    /**
     * Information describing the step 4.
     */

    private const string STEP4_DESCRIPTION = "Step 4: Record the RB Past Event";

    /**
     * Information describing the step 5.
     */

    private const string STEP5_DESCRIPTION = "Step 5: Verify the event is present in Archive with proper Guard time";

    /**
     * Constants.
     *
     * @author Avinoba
     * @date 10-Sep-13
     */

    static class Constants
    {
        /**
         * The play rb.
         */

        public const int playRb = 1;

        /**
         * The minimum time before event ends.
         */

        public const int minTimeBeforeEvtEnds = -1;

        /**
         * The is resuming.
         */

        public const bool isResuming = false;

        /**
         * The verify recording in pcat.
         */

        public const bool verifyRecordingInPCAT = true;

        /**
         * The is conflict.
         */

        public const bool isConflict = false;

        /**
         * The is past event.
         */

        public const bool isPastEvent = true;
    }

    #region Create Structure

    /**
     * Creates the structure.
     *
     * @author Avinoba
     * @date 10-Sep-13
     */

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute

    /**
     * Pre execute.
     *
     * @author Avinoba
     * @date 10-Sep-13
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
     * @date 10-Sep-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 10-Sep-13
         */

        public override void Execute()
        {
            StartStep();
            //Get Values From xml File
            rbChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsConstantEventDuration=True;IsMinEventDuration=True", "ParentalRating=High");
            if (rbChannel == null)
            {
                FailStep(CL, "Failed to fetch channel from Content.xml for passed criterion");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + rbChannel.LCN);

            //Get Value from project.ini for minimum sgt
            minStartGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (string.IsNullOrEmpty(minStartGuardTime))
            {
                FailStep(CL, res, "Failed to fetch Minimum Start Guardtime from Project.ini");
            }

            //Get Value from project.ini for minimum egt
            minEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MIN");
            if (string.IsNullOrEmpty(minEndGuardTime))
            {
                FailStep(CL, res, "Failed to fetch Minimum End Guardtime from Project.ini");
            }

            // Get integer values for the passed friendly names for guard time
            minTimeBeforeEvtStarts = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(minStartGuardTime);

            minNumEndGuardTime = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(minEndGuardTime, false);

            //Set SGT to minimum
            res = CL.EA.STBSettings.SetGuardTime(true, minStartGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Start Guard Time to " + minStartGuardTime);
            }

            //Set EGT to minimum
            res = CL.EA.STBSettings.SetGuardTime(false, minEndGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set End Guard Time to " + minEndGuardTime);
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
     * @date 10-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 10-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Tune to a channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, rbChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to channel: " + rbChannel.LCN);
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
     * @date 10-Sep-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 10-Sep-13
         */

        public override void Execute()
        {
            StartStep();
            int timeLeft = 0;
            double waitUntillPastEventEnds = 0;

            //Get time left for event to end
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get time left for event ends on channel: " + rbChannel.LCN);
            }

            //Navigate to live
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:LIVE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live in RB");
            }

            // Verifying if time left for event to end is more than minimum start time mins 
            if (timeLeft > ((minTimeBeforeEvtStarts + 1) * 60))
            {

                waitUntillPastEventEnds = Convert.ToDouble(timeLeft + (int.Parse(rbChannel.EventDuration) * 60) + (minNumEndGuardTime * 60) + 60);

                LogCommentInfo(CL, "Waiting for " + Convert.ToString(waitUntillPastEventEnds / 60) + " minutes for RB");

                //waiting for event to end
                res = CL.IEX.Wait(waitUntillPastEventEnds);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait for full event to be in RB");
                }
            }
            else
            {

                //waiting for 2 events to complete as timeleft is not enough for the 1st event
                waitUntillPastEventEnds = Convert.ToDouble(timeLeft + (int.Parse(rbChannel.EventDuration) * 120) + (minNumEndGuardTime * 60) + 60);

                LogCommentInfo(CL, "Not enough Time for RB Content, Waiting for " + Convert.ToString(waitUntillPastEventEnds / 60) + " minutes for RB");

                res = CL.IEX.Wait(waitUntillPastEventEnds); //waiting for for event having minimum start time mins before it starts
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait for full event to be in RB");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**
     * Step 3.
     *
     * @author Avinoba
     * @date 10-Sep-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 10-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            int waitInTrickMode = int.Parse(rbChannel.EventDuration)+ minNumEndGuardTime - 1;

            //Get value form Project.ini for min speed for TrickMode
            string rbSpeed = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            if (string.IsNullOrEmpty(rbSpeed))
            {
                FailStep(CL, "'REW_MIN' is empty in Project.ini in Section 'RB'");
            }

            double rbRewSpeed = Convert.ToDouble(rbSpeed);

            //Rewind Back at min speed
            res = CL.EA.PVR.SetTrickModeSpeed("RB", rbRewSpeed, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind at (" + rbSpeed + ") speed in RB");
            }

            //waiting for required event to be in RB playback
            LogCommentInfo(CL, "Rewinding back till the required Event");
            res = CL.IEX.Wait((waitInTrickMode * 60) / Math.Abs(rbRewSpeed));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait when the STB is in Rewind in RB");
            }

            //Playback the TrickMode
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.playRb, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select Play from RB");
            }


            PassStep();
        }
    }
    #endregion
    #region Step4

    /**
     * Step 4.
     *
     * @author Avinoba
     * @date 10-Sep-13
     */

    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 10-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Record the RB event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("recEvent", Constants.minTimeBeforeEvtEnds, Constants.isResuming, Constants.verifyRecordingInPCAT, Constants.isConflict, Constants.isPastEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record event from Action Bar while STB is in playback from RB");
            }

            PassStep();
        }
    }
    #endregion

    #region Step5

    /**
     * Step 5.
     *
     * @author Avinoba
     * @date 10-Sep-13
     */

    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 10-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Get start time from Event Info Collection
            string evtStartTime = CL.EA.GetEventInfo("recEvent", EnumEventInfo.EventStartTime);

            //Get end time from Event Info Collection
            string evtEndTime = CL.EA.GetEventInfo("recEvent", EnumEventInfo.EventEndTime);

            //Including extra Start Guard Time
            TimeSpan startTime = TimeSpan.Parse(evtStartTime).Subtract(TimeSpan.Parse("00:" + minTimeBeforeEvtStarts));

            //Including extra End Guard Time
            TimeSpan endTime = TimeSpan.Parse(evtEndTime).Add(TimeSpan.Parse("00:" + minNumEndGuardTime));

            //Verify if the RB Event is Recorded and present in Archive with Guard Time
            res = CL.EA.PVR.VerifyEventInArchive("recEvent", true, true, startTime.ToString("hh\\:mm"), endTime.ToString("hh\\:mm"));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the RB event id recorded and is present in Archieve with proper guard time");
            }

            PassStep();
        }
    }
    #endregion

    #region PostExecute
    #endregion
    /**
     * Posts the execute.
     *
     * @author Avinoba
     * @date 10-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Returning to live
        res = CL.EA.ReturnToLiveViewing();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to return to Live Viewing from RB");
        }

        //Get Value from Project.ini for default value for SGT
        string defaultStartGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultStartGuardTime))
        {
            LogCommentFailure(CL, "Default Start Guard Time is Empty in Project.ini");
        }
        else
        {
            //Set the SGT to default
            res = CL.EA.STBSettings.SetGuardTime(true, defaultStartGuardTime);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to set Start Guard Time to Default:" + defaultStartGuardTime + "; " + res.FailureReason);
            }
        }

        //Get Value from Project.ini for default value for EGT
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Default Start Guard Time is Empty in Project.ini");
        }
        else
        {
            //Set the EGT to default
            res = CL.EA.STBSettings.SetGuardTime(false, defaultEndGuardTime);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to set End Guard Time to Default:" + defaultEndGuardTime + "; " + res.FailureReason);
            }
        }

        //Delete the recorded event from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete All Recordings form Archive; " + res.FailureReason);
        }
    }
    #endregion
}