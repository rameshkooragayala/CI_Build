/// <summary>
///  Script Name : LIB_0021_Book_EventBased_To_TimeBasedRecording.cs
///  Test Name   : LIB-BOOK-0021-Event-based converted to time-based recording name in list
///  TEST ID     : 19485
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified :  25th Nov, 2013
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
* @class  BOOK_0021
*
* @brief   When presenting the title of a time-based recording that was generated from an event-based recording (see PSS-FP-LIB-0170), the title will be the original title of the event-based recording at the time the event-based recording was converted to a time-based recording.
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

[Test("BOOK_0021")]
public class BOOK_0021 : _Test
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
* @brief   The Recordable service.
**************************************************************************************************/

    private static Service recordableService1;

    /**********************************************************************************************/
    /**
* @brief   The Expected Event Name.
**************************************************************************************************/

    private static string expectedEventName;

    /**********************************************************************************************/
    /**
* @brief   The Obtained Event Name.
**************************************************************************************************/

    private static string obtainedEventName;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and Book an Event Based Recording";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Convert this Event Based Recording to an Time Based Recording and verify the title";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2:Wait until the Event starts and verify the title";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: wait until the event ends and verify the title";

    /**********************************************************************************************/
    /**
  

* @class   Constants
*
* @brief   Constants.
*
* @author Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    private static class Constants
    {
        /**
  * @brief   Extra Time Which is added to the Start and End times
  **************************************************************************************************/

        public const int extraTimeAdded = 2;

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
* @date    25th Nov, 2013
**************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

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
* @date    25th Nov, 2013
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
* @brief   Precondition: Get Channel Numbers From xml File and Book an Event Based Recording
* @author  Madhu Kumar K
* @date   25th Nov, 2013
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
* @date   25th Nov, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService.LCN);
            }
            recordableService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High;LCN="+recordableService.LCN);
            if (recordableService1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService1.LCN);
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("EventBasedRecording", recordableService.LCN, NumberOfPresses: 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event from guide");
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("EventBasedRecording1", recordableService1.LCN, NumberOfPresses: 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event from guide");
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
* @brief   Step 1 : Convert this Event Based Recording to an Time Based Recording and verify the title
*
* @author Madhu Kumar K
* @date   25th Nov, 2013
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
* @date   25th Nov, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Fetching the Event Start Time and End Time from The Event collection and adding extra time and modifying this EBR
            string evtStartTime = CL.EA.GetEventInfo("EventBasedRecording", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved start time from event info is null");
            }
            LogComment(CL, "Event Start time is " + evtStartTime);
            string evtEndTime = CL.EA.GetEventInfo("EventBasedRecording", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event End time is " + evtEndTime);
            //Including extra Start Guard Time
            TimeSpan startTime = TimeSpan.Parse(evtStartTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            //Including extra End Guard Time
            TimeSpan endTime = TimeSpan.Parse(evtEndTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            res = CL.EA.PVR.ModifyManualRecording("EventBasedRecording", startTime.ToString("hh\\:mm"), endTime.ToString("hh\\:mm"));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
            }
            //Fetching the expected Event title from the collection
            expectedEventName = CL.EA.GetEventInfo("EventBasedRecording", EnumEventInfo.EventName);
            if (string.IsNullOrEmpty(expectedEventName))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Expected Event Name " + expectedEventName);
            res = CL.EA.PVR.VerifyEventInPlanner("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify Event in Planner");
            }
            //Fetching the Event title from the EBR from Planner once EBR is converted to TBR
            res=CL.IEX.MilestonesEPG.GetEPGInfo("evtname",out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to fetch event name from EPG");
            }
            LogComment(CL,"Obtained Event Name "+obtainedEventName);
            //Verifying the Expected Event title with the obtained Event title--Checking that the title is not changed
            if (obtainedEventName != expectedEventName)
            {
                FailStep(CL,res,"Event title was modified after converting Event Based Recording to Time Based Recording");
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
* @brief   Step 2 :Wait until the Event starts and verify the title
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
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
* @date    25th Nov, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Waiting until the recording is started and verifying the title with the previous title. it is not expected to change
            res = CL.EA.WaitUntilEventStarts("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event starts");
            }
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event name from EPG");
            }
            LogComment(CL, "Obtained Event Name " + obtainedEventName);
            if (obtainedEventName != expectedEventName)
            {
                FailStep(CL, res, "Event title was modified after converting Event Based Recording to Time Based Recording and once Recording is started");
            }

            PassStep();
        }
    }
    #endregion

    #region Step2

    /**********************************************************************************************/
    /**
* @class   Step3
*
* @brief   Step 3 :Wait until the Event Ends and verify the title
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief  Executes this object.
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Waiting until the recording is completed and verifying the title with the previous title. it is not expected to change
            res = CL.EA.WaitUntilEventEnds("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event Ends");
            }
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch event name from EPG");
            }
            LogComment(CL, "Obtained Event Name " + obtainedEventName);
            if (obtainedEventName != expectedEventName)
            {
                FailStep(CL, res, "Event title was modified after converting Event Based Recording to Time Based Recording and once Recording is complete");
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
* @date    25th Nov, 2013
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete the record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete the record from Archive");
        }

    }
    #endregion


}