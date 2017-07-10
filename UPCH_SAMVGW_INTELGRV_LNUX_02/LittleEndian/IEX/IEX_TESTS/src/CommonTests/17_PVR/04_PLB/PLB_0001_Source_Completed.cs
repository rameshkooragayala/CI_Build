/// <summary>
///  Script Name : PLB_0001_Source_Completed.cs, 
///  Test Name   : PLB-0001-Source - Completed, PLB-0002-Source - Partial, PLB-0003-Source - Record in Progress, PLB-0004 - Source- Review Buffer
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Anshul Upadhyay
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************//**
 * @class   PLB_0001
 *
 * @brief   Plb 0001.
 *
 * @author  Anshulu
 * @date    01/10/13
 **************************************************************************************************/

[Test("PLB_0001_Source_Completed")]
public class PLB_0001 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The service to be recorded.
     **************************************************************************************************/

    static Service serviceToBeRecorded;

    /**********************************************************************************************/
    /*** @brief   The service to be recorded.
    **************************************************************************************************/

    static Service partialServiceToBeRecorded;

    /**********************************************************************************************//**
     * @brief   The future event recording.
     **************************************************************************************************/

    static string futureEventRecording = "FUTURE_EVENT";

    /**********************************************************************************************/
    /*** @brief   The Partial event recording.
    **************************************************************************************************/

    static string eventToBeRecorded = "EVENT_RECORDING";

    /**********************************************************************************************//**
     * @brief   Duration of the event.
     **************************************************************************************************/

    public const int EventDuration = 600;


    /**********************************************************************************************//**
     * @brief   The start guard time name.
     **************************************************************************************************/

    static string StartGuardTimeName;

    /**********************************************************************************************/
    /*** @brief   The start guard time name.
    **************************************************************************************************/

    static string endGuardTimeName;

    /**********************************************************************************************/
    /*** @brief   The event record duration for partial recording.
    **************************************************************************************************/

    public const int partialEventRecordDuration = 120;

    /**********************************************************************************************/
    /*** @brief   The forward speed to reach end of file
    **************************************************************************************************/

    static int FWD_Speed;

    /**********************************************************************************************/
    /*** @brief   The rewind speed to reach beginning of file
    **************************************************************************************************/

    static int REW_Speed;

    /**********************************************************************************************/
    /*** @brief   The time to wait for review buffer
    **************************************************************************************************/

    static int RBTime = 150;

    static string[] TM_REW = { "" };
    static string[] TM_FF = { "" };

    /**********************************************************************************************//**
     * @brief   The resource.
     **************************************************************************************************/

    static IEXGateway._IEXResult res;
    
    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    static class Constants
    {
        /**********************************************************************************************//**
         * @brief   The play from beginning.
         **************************************************************************************************/

        public const bool playFromBeginning = true;

        /**********************************************************************************************//**
         * @brief   The verify EOF.
         **************************************************************************************************/

        public const bool verifyEOF = false;

        /**********************************************************************************************//**
         * @brief   Number of presses for next events.
         **************************************************************************************************/

        public const int numberOfPressesForNextEvent = 1;

        /**********************************************************************************************//**
         * @brief   The minimum time before event start.
         **************************************************************************************************/

        public const int minTimeBeforeEventStart = 1;

        /**********************************************************************************************/
        /*** @brief   The minimum time for event to end
        **************************************************************************************************/

        public static int minMinRequiredInEvent = 2;

        /**********************************************************************************************/
        /*** @brief   The minimum time to record.
        **************************************************************************************************/

        public const int secToWaitAfterStartOfRecord = 90;
    }

    /**********************************************************************************************//**
     * @brief   Have a recording with COMPLETED status on disk
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Schedule a future recording";

    /**********************************************************************************************//**
     * @brief   Step 1: Access the recordings in the Recordings list
     **************************************************************************************************/
    
    private const string STEP1_DESCRIPTION = "Step 1: Record a partial Event";
    
    /**********************************************************************************************//**
     * @brief   Step 2: Access the recordings in the Recordings list
     **************************************************************************************************/
    
    private const string STEP2_DESCRIPTION = "Step 2: Once partial recording completed, verify Recording is Partial and playback until end of playback";
    
    /**********************************************************************************************//**
     * @brief   Step 3: Playback the recording
     **************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: Once FUTURE recording starts, verify the recording status and playback and FFW until end of recording is reached";
    
    /**********************************************************************************************/
    /*** @brief   Step 4: Playback the recording
    **************************************************************************************************/

    private const string STEP4_DESCRIPTION = "Step 4: Verify recording is completed";

    /**********************************************************************************************/
    /*** @brief   Step 5: Playback the recording
    **************************************************************************************************/

    private const string STEP5_DESCRIPTION = "Step 5: Playback the recording until end of playback";

    /**********************************************************************************************/
    /*** @brief   Step 5: Playback the recording
    **************************************************************************************************/

    private const string STEP6_DESCRIPTION = "Step 6: Access the content of review buffer and check REW and FFW is possible";

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);
        this.AddStep(new Step6(), STEP6_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion
    
    #region PreExecute

    /**********************************************************************************************//**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Have a recording with COMPLETED status on disk
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/
        
        public override void Execute()
        {   
            StartStep();
            
            //Get Channel Values From xml File
            LogCommentInfo(CL, "Get Channel Values From xml File");
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=HIGH");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }

            partialServiceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=HIGH;LCN=" + serviceToBeRecorded.LCN);
            if (partialServiceToBeRecorded == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            
            //reading Guard Time from Project ini
            StartGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (StartGuardTimeName == null)
            {
                FailStep(CL, res, "Failed to read min value of start guard time from project.ini");
            }

            endGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MIN");
            if (endGuardTimeName == null)
            {
                FailStep(CL, res, "Failed to read min value of end guard time from project.ini");
            }

            // setting Start Guard time to min value
            LogCommentInfo(CL, "Setting Start Guard Time to None");
            res = CL.EA.STBSettings.SetGuardTime(true, StartGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to set start guard time to NONE");
            }

            //setting End Guard Time to min value
            LogCommentInfo(CL, "Setting End Guard Time to None");
            res = CL.EA.STBSettings.SetGuardTime(false, endGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set end guard time to NONE");
            }
            
            //Tune to the service to record the event
            LogCommentInfo(CL, "Tuning to the channel where the event has to be recorded");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + serviceToBeRecorded.LCN);
            }

            //Book the future event for recording
            LogCommentInfo(CL, "Booking future event from banner");
            res = CL.EA.PVR.BookFutureEventFromBanner(futureEventRecording, Constants.numberOfPressesForNextEvent, 
                    Constants.minTimeBeforeEventStart, false);                       
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event recording on channel "+serviceToBeRecorded.LCN);
            }

            //Tune to channel where partial event to be recorded
            LogCommentInfo(CL, "Tuning to the channel where the event has to be recorded partially");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, partialServiceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + partialServiceToBeRecorded.LCN);
            }
                           
            PassStep();
        }
    }
    #endregion  
    #region Step1

    /**********************************************************************************************/
    /*** @class   Step1
    *
    * @brief   Step 1: record a partial event
    *
    * @author  Anshulu
    * @date    01/10/13
**************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /*** @fn  public override void Execute()
        *
        * @brief   Executes this object.
        *
        * @author  Anshulu
        * @date    01/10/13
        **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            
            //Record the current event
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minMinRequiredInEvent, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service " + partialServiceToBeRecorded.LCN);
            }

            //Wait for some time for event to record
            res = CL.IEX.Wait(Constants.secToWaitAfterStartOfRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of record!");
            }

            //Stop recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from archive!");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************/
    /*** @class   Step2
    *
    * @brief   Step 2: Once partial recording completed, verify Recording is Partial and playback until end of playback
    *
    * @author  Anshulu
    * @date    01/10/13
    **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************/
        /*** @fn  public override void Execute()
        *
        * @brief   Executes this object.
        *
        * @author  Anshulu
        * @date    01/10/13
        **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            
            //Check whether event is partially recorded
            LogCommentInfo(CL, "Checking whether event is partially recorded");
            res = CL.EA.PCAT.VerifyEventPartialStatus(eventToBeRecorded, "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Event is not completely recorded!");
            }

            //Playback the event till the end 
            LogCommentInfo(CL, "Playback the event until end of playback is reached");
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording!");
            }


            PassStep();
        }
    }
    #endregion
    #region Step3

    /**********************************************************************************************//**
     * @class   Step3
     *
     * @brief   Step 3: Once FUTURE recording starts, verify the recording status and playback the currently recording event and FFW until end of recording is reached
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
                        
            TM_FF = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_FWD").Split(','); ;
            int.TryParse(TM_FF[2], out FWD_Speed);

            if (FWD_Speed == null)
            {
                LogCommentInfo(CL, "Fail to get forward speed or string to int conversion failed");
            }
            
            LogCommentInfo(CL, "Wait for event to start recording");
            res = CL.EA.WaitUntilEventStarts(futureEventRecording);
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "FAILED TO WAIT FOR RECORDING TO START OR RECORDING IS ALREADY STARTED!");
            }

            //Verifying the current recording status in PCAT
            LogCommentInfo(CL, "Verify Currently Recording status in PCAT");
            res = CL.EA.PCAT.VerifyEventIsRecording(futureEventRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to validate RECORD IN PROGRESS in PCAT!");
            }

            //Wait for some time after recording started
            res = CL.IEX.Wait(RBTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for " + RBTime + " sec");
            }


            LogCommentInfo(CL, "Playback the currently recording event");
            res = CL.EA.PVR.PlaybackRecFromArchive(futureEventRecording, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording!");
            }

            //Forward the event until end of recording is reached
            LogCommentInfo(CL, "Perform trickmodes to reach end of recording");
            res = CL.EA.PVR.SetTrickModeSpeed(futureEventRecording, FWD_Speed, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback until live is reached!");
            }

            res = CL.IEX.Wait(RBTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for " + RBTime + " sec");
            }

            //wait for recording to end
            LogCommentInfo(CL, "Waiting for recording to end");
            res = CL.EA.WaitUntilEventEnds(futureEventRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until recording completed!");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4

    /**********************************************************************************************//**
     * @class   Step4
     *
     * @brief   Step 4: Verify the recording status is completed
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Check whether event is fully recorded
            LogCommentInfo(CL, "Checking whether event is completely recorded");
            res = CL.EA.PCAT.VerifyEventPartialStatus(futureEventRecording, "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Event is not completely recorded!");
            }

            PassStep();
        }
    }
    #endregion
    #region Step5

    /**********************************************************************************************//**
     * @class   Step5
     *
     * @brief   Step 5: Playback the recording
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [Step(5, STEP5_DESCRIPTION)]
    private class Step5 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Playback the event
            LogCommentInfo(CL, "Playback the completely recorded event");
            res = CL.EA.PVR.PlaybackRecFromArchive(futureEventRecording, 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording!");
            }

            PassStep();
        }
    }
    #endregion */
    #region Step6

    /**********************************************************************************************//**
     * @class   Step6
     *
     * @brief   Step 6: Access the content of review buffer and check REW and FFW is possible
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [Step(6, STEP5_DESCRIPTION)]
    private class Step6 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            TM_REW = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW").Split(',');
            int.TryParse(TM_REW[1], out REW_Speed);
            
            if (REW_Speed == null)
            {
                FailStep(CL, res, "Fail to get rewind speed or string to int conversion failed");
            }

            //Navigate to standby to clear buffer
            LogCommentInfo(CL, "Navigating and coming out of standby to clear review buffer");

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enter standby");
            }

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present After Exiting From Standby");
            }

            //Wait for some time after navigating to live started
            res = CL.IEX.Wait(RBTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for " + RBTime + " sec");
            }

            //Rewind the event until beginning of RB
            LogCommentInfo(CL, "Rewind until beginning of review buffer is reached");
            res = CL.EA.PVR.SetTrickModeSpeed("RB", REW_Speed, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to rewind until beginning of Rewiew buffer is reached!");
            }


            //Forward the event until end of recording is reached
            LogCommentInfo(CL, "Forward until end of review buffer(LIVE) is reached");
            res = CL.EA.PVR.SetTrickModeSpeed("RB", FWD_Speed, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to forward until live or end of review buffer is reached!");
            }

            PassStep();
        }
    }
    #endregion 
    #endregion

    #region PostExecute

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   Delete all recordings
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        //deleting recording from planner

        LogCommentInfo(CL, "Delete the recording");
        res = CL.EA.PVR.DeleteRecordFromArchive(futureEventRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete recording because" + res.FailureReason);
        }

        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete recording because" + res.FailureReason);
        }

        //deleting any future recordings from planner
        LogCommentInfo(CL, "Delete future recordings from planner");
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to cancel bookings from planner" + res.FailureReason);
        }

    }
    #endregion
}