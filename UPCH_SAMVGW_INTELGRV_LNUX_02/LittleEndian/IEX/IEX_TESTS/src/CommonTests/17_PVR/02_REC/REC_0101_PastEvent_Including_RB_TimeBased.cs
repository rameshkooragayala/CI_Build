/// <summary>
///  Script Name : REC_0101_PastEvent_Including_RB_TimeBased.cs
///  Test Name   : REC-0101-Past event rec from RB + time-based - SGT, body, EGT
///  TEST ID     : 20126
///  QC Version  : 2
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified : 12th Nov, 2013
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************/
/**
* @class  REC_0101
*
* @brief   Recording current event from Time based Recording and RB
*
* @author  Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

[Test("REC_0101")]
public class REC_0101 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/

    /**
* @brief   The Recordable service.
**************************************************************************************************/

    private static Service recordableService;

    /**********************************************************************************************/
    /**
* @brief   Any service
**************************************************************************************************/

    private static Service service;

    /**********************************************************************************************/
    /**
* @brief   The rewind string value
**************************************************************************************************/

    private static int medRewTrickmode;

    /**********************************************************************************************/
    /**
* @brief   End Guard Time in Minutes
**************************************************************************************************/

    private static int endGuardTimeInt = 0;

    /**********************************************************************************************/
    /**
* @brief   Start Guard time in Minutes
**************************************************************************************************/

    private static int startGuardTimeInt = 0;

    /**********************************************************************************************/
    /**
* @brief   The obtained event time
**************************************************************************************************/

    private static string obtainedEventtime = "";

    /**********************************************************************************************/
    /**
* @brief   The expected event time
**************************************************************************************************/

    private static string expectedEventtime = "";

    /**********************************************************************************************/
    /**
* @brief   The obtained event name
**************************************************************************************************/

    private static string obtainedEventname = "";

    /**********************************************************************************************/
    /**
* @brief   The expected event name
**************************************************************************************************/

    private static string expectedEventname = "";

    /**********************************************************************************************/

    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File, Set SGT and EGT";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1:Stay in RB few min into Event body and book a Time based recording until EGT is completed and Book that event from RB";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Verify and Playback the Recording from Archive";

    /**********************************************************************************************/
    /**
  

* @class   Constants
*
* @brief   Constants.
*
* @author Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

    private static class Constants
    {
        /**
* @brief   Constant used for converting minutes to seconds
**************************************************************************************************/

        public const int convertMintoSec = 60;

        /**********************************************************************************************/
        /**
* @brief   constant play
**************************************************************************************************/

        public const int play = 1;

        /**********************************************************************************************/
        /**
* @brief   Minimum variance in minutes
**************************************************************************************************/

        public const int minVariance = 1;

        /**********************************************************************************************/
        /**
* @brief   duration into Event Body in seconds
**************************************************************************************************/

        public const int durationIntoEventBody = 180;

        /**********************************************************************************************/
        /**
* @brief   duration into Event Body in seconds
**************************************************************************************************/

        public const int minTimeRequiredAfterEGT = 6;

        /**********************************************************************************************/
        /**
* @brief   Event Based Recording Key
**************************************************************************************************/

        public const string eventBasedRecordingkey = "eventBasedRecording";

        /**********************************************************************************************/
        /**
* @brief   Event Based Recording Key
**************************************************************************************************/

        public const string timeBasedRecordingkey = "TimeBasedRecording";

        /**********************************************************************************************/
    }


    #region Create Structure

    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

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

    #region PreExecute

    /**********************************************************************************************/
    /**
* @fn  public override void PreExecute()
*
* @brief  Pre execute
*
* @author Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************/
    /**
* @class   PreCondition
*
* @brief  Get Channel Numbers From xml File, Set SGT and EGT 
* @author  Madhu Kumar K
* @date   12th Nov, 2013
**************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author Madhu Kumar K
* @date   12th Nov, 2013
**************************************************************************************************/

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
                LogCommentInfo(CL, "Service fetched is : " + recordableService.LCN);
            }

            //Any service which is just used for tuning
            service = CL.EA.GetServiceFromContentXML("Type=Video", "LCN=" + recordableService.LCN);
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }

            string sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");

            string egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");

            LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);

            LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);

            //Fetch the medium REW trickmode
            String rewTrickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            if (String.IsNullOrEmpty(rewTrickModeArrayInStr))
            {
                FailStep(CL, "Rewind Trick mode list not present in Project.ini file.");
            }
            String[] rewTrickModeArray = rewTrickModeArrayInStr.Split(',');
            String rewTrickModeInStr = rewTrickModeArray[(rewTrickModeArray.Length) / 2];
            medRewTrickmode = int.Parse(rewTrickModeInStr);
            LogComment(CL, "Medium Trick mode speed fetched" + medRewTrickmode);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);
            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);
            //SGT and EGT values should be Greater than or equal to 2 minutes
            if (startGuardTimeInt < 2 || endGuardTimeInt < 2)
            {
                FailStep(CL, "SGT and EGT values fetched from test ini are less then 2 min's");
            }
            LogComment(CL,"Setting the Start Guard Time to "+sgtFriendlyName);
            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            LogComment(CL, "Setting the End Guard Time to " + egtFriendlyName);
            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }


            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   Step 1 : Stay in RB few min into Event body and book a Time based recording until EGT is completed and Book that event from RB
*
* @author Madhu Kumar K
* @date   12th Nov, 2013
**************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief  Executes this object.
*
* @author Madhu Kumar K
* @date   12th Nov, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Tune to a Recordable Service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failesd to Tune to Service:" + recordableService.LCN);
            }

            int TimeLeftInSec = 0;
            //Getting the Current Event Time Left 
            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To get Current Event Time left");
            }

            LogCommentInfo(CL, "Current Event Time Left:" + TimeLeftInSec);

            //If Time left is less then the SGT then we are waiting till that event completes
            if (TimeLeftInSec < startGuardTimeInt * 60)
            {
                LogComment(CL, "Returning to Live viewing from Action Bar Launched During GetCurrentEventLeftTime");
                res = CL.EA.ReturnToLiveViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Return to Live Viewing");
                }
                LogComment(CL, "Waiting until Event is completed as time left is less then then required SGT");
                res = CL.IEX.Wait(TimeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Wait before SGT");
                }
                //Getting the Current Event time left which is required for SGT
                res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To get Current Event Time left");
                }
            }
            LogComment(CL, "Waiting until SGT is completed");
            res = CL.IEX.Wait(TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To till SGT is completed");
            }
            //Returing to live viewing from Action Bar to get current event info
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            //Launching Action Bar to get the expected EPG info
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Action Bar");
            }
            //Getting the Expected Event time
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out expectedEventtime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time");
            }
            //Getting the Expected Event Name
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out expectedEventname);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name");
            }
            LogCommentImportant(CL, "Expected event time is " + expectedEventtime);
            LogCommentImportant(CL, "Expected event name is " + expectedEventname);

            LogComment(CL, "Waiting 3 min into the Event body");
            res = CL.IEX.Wait(Constants.durationIntoEventBody);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until 3 min into Event body");
            }
            //Returing to Live viewing to refresh the Event info
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Return to Live Viewing");
            }
            //Getting the Event Time left
            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get current event left time");
            }

            //Doing a Manual Recording from Planner till EGT is completed
            res = CL.EA.PVR.RecordManualFromPlanner(Constants.timeBasedRecordingkey, recordableService.Name, DaysDelay: -1, DurationInMin: ((TimeLeftInSec / 60) + (endGuardTimeInt + 6)), VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Manual from Planner");
            }
            //Tuning to an other Service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + service.LCN);
            }
            LogComment(CL,"Waiting until the EGT of that Event is completed");
            double waitUntilEGT = (TimeLeftInSec + (endGuardTimeInt * 60));
            res = CL.IEX.Wait(waitUntilEGT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until EGT ends");
            }
            //Rewinding Back into the Event Body
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToInt32(medRewTrickmode),false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            double waitInTrickModeSpeed = (waitUntilEGT / Math.Abs(medRewTrickmode));
            LogComment(CL, "Wait in trickmode till we reach previous event in RB");
            res = CL.IEX.Wait(waitInTrickModeSpeed);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait in trick mode speed");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play,false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }
            //Launching Action Bar after Trick Mode to get the EGPinfo
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Action Bar");
            }
            //Fetching the Event time from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventtime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time");
            }
            //Fetching the Event Name from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventname);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name");
            }

            LogCommentImportant(CL, "Obtained event time is " + obtainedEventtime);

            LogCommentImportant(CL, "Obtained event name is " + obtainedEventname);

            //Verifying the Expected Event time and Event name are same as Obtained Event time and Event Name
            if (expectedEventtime == obtainedEventtime && expectedEventname == obtainedEventname)
            {
                //Recording a current event from banner from RB
                res = CL.EA.PVR.RecordCurrentEventFromBanner(Constants.eventBasedRecordingkey,VerifyIsRecordingInPCAT:false,IsPastEvent:true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Record current event from Banner");
                }
            }
            else
            {
                FailStep(CL, "Obtained Event info is different from Expected Event info after trick mode");
            }
            //Coming back to live from RB
            res = CL.EA.PVR.StopPlayback(IsReviewBuffer:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }
           // If the event based recording has appended TBR recording's duration then the recording will be Complete. This is checked below
            res = CL.EA.PCAT.VerifyEventPartialStatus(Constants.eventBasedRecordingkey, "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Verify the event Partial Status");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   Step 2 : Playback the recording till EOF
*
* @author  Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief  Executes this object.
*
* @author  Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            string duration = "";
            string actualDuration = "";

            CL.EA.PCAT.GetEventInfo(Constants.eventBasedRecordingkey, EnumPCATtables.FromRecordings, "ACTUAL_DURATION", ref actualDuration);
            if (String.IsNullOrEmpty(actualDuration))
            {
                FailStep(CL, "Failed to retrieve the Actual Duration from PCAT");
            }
            CL.EA.PCAT.GetEventInfo(Constants.eventBasedRecordingkey, EnumPCATtables.FromRecordings, "DURATION", ref duration);
            if (String.IsNullOrEmpty(duration))
            {
                FailStep(CL, "Failed to retrieve the Duration from PCAT");
            }
            //Converting Actual duration from milliseconds to min 
            double actualDurationInMin = Convert.ToInt32(actualDuration) / (1000 * 60);

            //Converting duration from milliseconds to min 
            double durationInMin = Convert.ToInt32(duration) / (1000 * 60);

            LogComment(CL, "Actual Duration in Min " + actualDurationInMin + " and Duration is" + durationInMin);

            double variance = Math.Abs((startGuardTimeInt + endGuardTimeInt + durationInMin) - actualDurationInMin);

            LogComment(CL, "Difference between Actual duration and Expected Duration " + variance);
            //If the Variance is more then the Expected it should fail
            if (variance > Constants.minVariance)
            {
                FailStep(CL, "Actual Duration retrieved from PCAT is different from expected");
            }
           
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventBasedRecordingkey, SecToPlay: 0, FromBeginning: true, VerifyEOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To PlayBack the Record From Archive");
            }
            PassStep();
        }
    }
    #endregion



    #endregion

    #region PostExecute

    /**********************************************************************************************/
    /**
* @fn  public override void PostExecute()
*
* @brief   Executes this object.
*
* @author  Madhu Kumar K
* @date    12th Nov, 2013
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive(Constants.eventBasedRecordingkey);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete Event based recording from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(Constants.timeBasedRecordingkey);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete the Time Based recording from Archive");
        }
        //Fetch the SGT and EGT default values
        String defSgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (String.IsNullOrEmpty(defSgtValueInStr))
        {
            CL.IEX.FailStep("Default SGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default SGT value in minutes - " + defSgtValueInStr);

        String defEgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (String.IsNullOrEmpty(defEgtValueInStr))
        {
            CL.IEX.FailStep("Default EGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default EGT value in minutes - " + defEgtValueInStr);

        //Set SGT & EGT to default
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: defSgtValueInStr);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to set Start Guide time - " + defSgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: defEgtValueInStr);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to set End Guide time - " + defEgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
    }
    #endregion


}