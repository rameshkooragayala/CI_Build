/// <summary>
///  Script Name : REC_0091_CurrentEvent_DuringBodyFromTB.cs
///  Test Name   : REC-0091-Current event rec - during body - from TB - SGT& body included
///  TEST ID     : 20123
///  QC Version  : 2
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified : 28th OCT, 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

/**********************************************************************************************/
/**
* @class  REC_0091 
*
* @brief   Recording current event during body from Time based Recording
*
* @author  Madhu Kumar K
* @date    28th OCT, 2013
**************************************************************************************************/

[Test("REC_0091")]
public class REC_0091 : _Test
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

    private static Service recordableService;

    /**********************************************************************************************/
    /**
* @brief   The service1.
**************************************************************************************************/

    private static Service service;

    /**********************************************************************************************/
    /**
* @brief   End Guard time in min
**************************************************************************************************/

    private static int endGuardTimeInt = 0;

    /**********************************************************************************************/
    /**
* @brief   Start Guard time in Min
**************************************************************************************************/

    private static int startGuardTimeInt = 0;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File, Set SGT and EGT";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1:Book a Time Based Recording, Tune to another channel and Book an Event Based Recording overlapping with the TBR";

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
* @date    28th OCT, 2013
**************************************************************************************************/

    private static class Constants
    {
        /**
* @brief   Minimum variance in min
**************************************************************************************************/

        public const int minVariance = 1;

        /**********************************************************************************************/
        /**
* @brief   Event Based Recording Key
**************************************************************************************************/

        public const string eventBasedRecordingkey = "eventBasedRecording";

        /**********************************************************************************************/
        /**
* @brief   Event Based Recording Key
**************************************************************************************************/

        public const string timeBasedRecordingkey = "timeBasedRecording";

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
* @date    28th OCT, 2013
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
* @date    26th OCT, 2013
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
* @date   28th OCT, 2013
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
* @date   28th OCT, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Fetching a recordable service from Content XML
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True;IsConstantEventDuration=True", "ParentalRating=High");
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

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            //As Per Test Case SGT and EGT values should be Greater than or equal to 2 minutes
            if (startGuardTimeInt < 2 || endGuardTimeInt < 2)
            {
                FailStep(CL, "SGT and EGT values fetched from test ini are less then 2 min's");
            }


            LogComment(CL, "Setting the Start Guard Time to " + sgtFriendlyName);
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
* @brief   Step 1 : Book a Time Based Recording, Tune to another channel and Book an Event Based Recording overlapping with the TBR";
*
* @author Madhu Kumar K
* @date   28th OCT, 2013
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
* @date   28th OCT, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Tuning to a Recordable Service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failesd to Tune to Service:" + recordableService.LCN);
            }

            //UPC/VOO - Reqt - If a TBR is done for more than half of event duration then further event based recording is not possible.To handle the above case making sure the TBR booking is not more than half of the event body

            int Duration = Convert.ToInt32(((Convert.ToInt32(recordableService.EventDuration) - 4) * 60) / 2);
            LogComment(CL, "Duration into Next Event :" + Duration);
            int TimeLeftInSec = 0;
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
                    FailStep(CL, res, "Failed to Wait before SGT as event time left is less then required SGT");
                }

                //getting the Time left which is required for SGT
                res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To get Current Event Time left");
                }
            }
            //To calculate the exact wait time for the recording, it is needed to capture the time taken by EA as well.The logic is to use a stop watch to calculte the exact time taken by EA and subtracting it with the overall duration. 

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            res = CL.EA.PVR.RecordManualFromPlanner(Constants.timeBasedRecordingkey, recordableService.Name, DaysDelay:-1,DurationInMin: Convert.ToInt32((TimeLeftInSec + Duration) / 60), VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To book Manual Recording on " + recordableService.LCN);
            }

            //Tuning to some other service to make sure that recording is not from RB
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Tune to Channel " + service.LCN);
            }
            sw.Stop();
            LogCommentImportant(CL, "Time taken for EA's in sec is " + Convert.ToInt32(sw.Elapsed.TotalSeconds));

            LogComment(CL,"Waiting Few min into next Event of that TBR");
            res = CL.IEX.Wait(TimeLeftInSec + (Duration / 2) - Convert.ToInt32(sw.Elapsed.TotalSeconds));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait");
            }
            //Recording Current Event from Guide
            res = CL.EA.PVR.RecordCurrentEventFromGuide(Constants.eventBasedRecordingkey, recordableService.LCN,MinTimeBeforeEvEnd:-1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To book current Event From Guide");
            }
            LogComment(CL, "Waiting until the Recording is completed");
            res = CL.EA.WaitUntilEventEnds(Constants.eventBasedRecordingkey);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To wait until Event Ends");
            }
            LogComment(CL, "Waiting until the EGT is completed");
            res = CL.IEX.Wait(endGuardTimeInt*60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait until event EGT ends");
            }

            //If the event based recording has appended TBR recording's duration then the recording will be Complete. This is checked below
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
* @brief   Step 2 : Verify and Playback the recording till EOF
*
* @author  Madhu Kumar K
* @date    28th OCT, 2013
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
* @date    28th OCT, 2013
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

            LogComment(CL, "Actual Duration in Min is" + actualDurationInMin + " and Duration in Min is " + durationInMin);

            double variance = Math.Abs((startGuardTimeInt + endGuardTimeInt + durationInMin) - actualDurationInMin);

            LogComment(CL, "Difference between Actual duration and Expected Duration "+variance);

            if (variance > Constants.minVariance)
            {
                FailStep(CL, "Actual Duration retrieved from PCAT is different from expected");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventBasedRecordingkey,SecToPlay:0,FromBeginning:true,VerifyEOF:true);
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
* @date    28th OCT, 2013
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //Delete the Event Based Recoeding
        res = CL.EA.PVR.DeleteRecordFromArchive(Constants.eventBasedRecordingkey);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete Event based recording from Archive");
        }
        //Delete the Time Based Recoeding
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