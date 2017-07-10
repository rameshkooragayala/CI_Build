/// <summary>
///  Script Name : REC_CurrentEvent_DuringEGT_AfterEGT_FromRB.cs
///  Test Name   : REC-0092-Current event rec - during EGT - from RB - SGT, body, EGT included-----REC-0100-Past event rec from RB - SGT, body, EGT
///  TEST ID     : 20129,20125
///  QC Version  : 2
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified : 25th OCT, 2013
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
* @class  REC_Current_Event_FromRB
*
* @brief   Setting of the different subtitle languages
*
* @author  Madhu Kumar K
* @date    25th OCT, 2013
**************************************************************************************************/

[Test("REC_Current_Event_FromRB")]
public class REC_Current_Event_FromRB : _Test
{
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/

    /**
* @brief   The service.
**************************************************************************************************/

    private static Service service;

    /**********************************************************************************************/
    /**
* @brief   The end guard time 
**************************************************************************************************/

    private static int endGuardTimeInt = 0;

    /**********************************************************************************************/
    /**
* @brief   The start guard time
**************************************************************************************************/

    private static int startGuardTimeInt = 0;

    /**********************************************************************************************/
    /**
* @brief   The swait for EGT in sec
**************************************************************************************************/

    private static int waitForEGT = 0;

    /**********************************************************************************************/
    /**
* @brief   The start guard time
**************************************************************************************************/

    private static string recordPastEvent = "";
    /***********************************************************************************************
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
* @brief   The rewind max string value
**************************************************************************************************/

    private static int medRewTrickmode;

    /**********************************************************************************************/


    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and set SGT and EGT";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Wait until SGT and Event body completes and book the event from EGT/ After EGT";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Verify the Recording and play back till EOF";

    /**********************************************************************************************/
    /**
  

* @class   Constants
*
* @brief   Constants.
*
* @author Madhu Kumar K
* @date    25th OCT, 2013
**************************************************************************************************/

    private static class Constants
    {
        /**********************************************************************************************/

        /**
* @brief   wait in trick mode speed
**************************************************************************************************/

        public const int convertMintoSec = 60;

        /**********************************************************************************************/
        /**
* @brief   constant play
**************************************************************************************************/

        public const int play = 1;

        /**********************************************************************************************/
        /**
* @brief   Minimum variance in min
**************************************************************************************************/

        public const int minVariance = 1;

        /**********************************************************************************************/
        /**
* @brief   Minimum variance in min
**************************************************************************************************/

        public const bool isResuming = false;

        /**********************************************************************************************/
        /**
* @brief   Minimum variance in min
**************************************************************************************************/

        public const bool verifyIsRecordingInPcat = false;

        /**********************************************************************************************/
        /**
* @brief   Minimum variance in min
**************************************************************************************************/

        public const bool isConflict=false;

        /**********************************************************************************************/
        /**
* @brief   Minimum variance in min
**************************************************************************************************/

        public const bool isPastEvent=true;

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
* @date    25th OCT, 2013
**************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        //Get Client Platform
        CL = GetClient();

        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
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
* @date    25th OCT, 2013
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
* @brief  
* @author  Madhu Kumar K
* @date   25th OCT, 2013
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
* @date   25th OCT, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
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

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);
            if (startGuardTimeInt < 2 || endGuardTimeInt < 2)
            {
                FailStep(CL, "SGT and EGT values fetched from test ini are less then 2 min's");
            }

            recordPastEvent = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Record_Past_Event");

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
            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

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
* @brief   Step 1 : Wait until SGT and body completes and book the Event from EGT/ After EGT
*
* @author Madhu Kumar K
* @date   25th OCT, 2013
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
* @date   25th OCT, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failesd to Tune to Service:" + service.LCN);
            }

            int TimeLeftInSec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To get Current Event Time left");
            }
            LogCommentInfo(CL, "Current Event Time Left:" + TimeLeftInSec);

            if (TimeLeftInSec < startGuardTimeInt * Constants.convertMintoSec)
            {
                LogComment(CL, "Waiting until this event ends as it is less then SGT");
                CL.IEX.Wait(TimeLeftInSec);
                res = CL.EA.ReturnToLiveViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to return to live viewing");
                }
                res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To get Current Event Time left");
                }
            }
            LogComment(CL, "Waiting until SGT event ends");
            res = CL.IEX.Wait(TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait until SGT ends");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Action Bar");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out expectedEventtime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name");
            }
            LogCommentImportant(CL, "Expected event time is " + expectedEventtime);
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the current event time left");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            LogComment(CL, "Wait until Event body ends");
            res = CL.IEX.Wait(TimeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event body ends");
            }

            if (recordPastEvent.ToUpper() == "TRUE")
            {
                LogComment(CL, "Wait until EGT ends");
                waitForEGT = endGuardTimeInt * Constants.convertMintoSec;
                res = CL.IEX.Wait(waitForEGT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to wait until EGT ends");
                }
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToInt32(medRewTrickmode), false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            double waitInTrickModeSpeed = (waitForEGT + 3 * Constants.convertMintoSec) / Math.Abs(medRewTrickmode);
            LogComment(CL, "Wait in trickmode till we reach previous event in RB");
            res = CL.IEX.Wait(waitInTrickModeSpeed);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait in trick mode speed");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }

            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Action Bar");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventtime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name");
            }
            LogCommentImportant(CL, "obtained event time is" + obtainedEventtime);
            if (expectedEventtime == obtainedEventtime)
            {
                if (recordPastEvent.ToUpper() == "TRUE")
                {
                    res = CL.EA.PVR.RecordCurrentEventFromBanner("EventRecording", -1, Constants.isResuming,Constants.verifyIsRecordingInPcat,Constants.isConflict,Constants.isPastEvent);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed To Record Current Event From Banner");
                    }
                }
                else
                {
                    res = CL.EA.PVR.RecordCurrentEventFromBanner("EventRecording");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed To Record Current Event From Banner");
                    }
                    res = CL.EA.ReturnToLiveViewing();
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to return to live viewing");
                    }
                    LogComment(CL,"Wait until End Guard time is completed");
                    res = CL.IEX.Wait(endGuardTimeInt * Constants.convertMintoSec);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed to wait until EGT ends");
                    }
                }
            }
            else
            {
                FailStep(CL, res, "Failed to rewind to the same event in RB Expected Event time " + expectedEventtime + "obtained Event time" + obtainedEventtime);
            }
            res = CL.EA.PCAT.VerifyEventPartialStatus("EventRecording", "ALL");
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
* @brief   Step 2 :  Verify the Recording and play back till EOF
*
* @author  Madhu Kumar K
* @date    25th OCT, 2013
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
* @date    25th OCT, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            string duration = "";
            string actualDuration = "";

            CL.EA.PCAT.GetEventInfo("EventRecording", EnumPCATtables.FromRecordings, "ACTUAL_DURATION", ref actualDuration);
            if (String.IsNullOrEmpty(actualDuration))
            {
                FailStep(CL,"Failed to retrieve the Actual Duration from PCAT");
            }
            CL.EA.PCAT.GetEventInfo("EventRecording", EnumPCATtables.FromRecordings, "DURATION", ref duration);
            if (String.IsNullOrEmpty(duration))
            {
                FailStep(CL, "Failed to retrieve the Duration from PCAT");
            }
            //Converting Actual duration from milliseconds to min 
            double actualDurationInMin = Convert.ToInt32(actualDuration) / (1000 * 60);

            //Converting duration from milliseconds to min 
            double durationInMin = Convert.ToInt32(duration) / (1000 * 60);

            LogComment(CL,"Actual Duration in Min "+actualDurationInMin+" and Duration is"+durationInMin);

            double variance = Math.Abs((startGuardTimeInt + endGuardTimeInt + durationInMin) - actualDurationInMin);

            if (variance > Constants.minVariance)
            {
                FailStep(CL, "Actual Duration retrieved from PCAT is different from expected");
            }
            LogComment(CL, "Actual duration " + actualDurationInMin + "Duration " + durationInMin);

            res = CL.EA.PVR.PlaybackRecFromArchive("EventRecording", 0, true, true);
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
* @date    25th OCT, 2013
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive("EventRecording");
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete All the recordings from Archive");
        }

    }
    #endregion


}
