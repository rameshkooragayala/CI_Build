/// <summary>
///  Script Name : LIB_0010_Book_ModifyKeepStatus_EventBasedRecording.cs
///  Test Name   : LIB-BOOK-0010-Modify keep status of event-based recording
///  TEST ID     : 4468
///  QC Version  : 2
///  Variations from QC: None
///  
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
* @class  BOOK_0010 
*
* @brief   From the (planner) bookings list, User is able to modify the keep status of an event-based recording
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

[Test("BOOK_0010")]
public class BOOK_0010 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/

    /**
* @brief   The recordable service.
**************************************************************************************************/

    private static Service recordableService;

    /**********************************************************************************************/
    /**
* @brief   The recordable service.
**************************************************************************************************/

    private static Service recordableService1;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and Book two Event Based Recordings and set Keep flag";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Select one Event Based Recording and change the Keep flag";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Select the other Recording and change the Keep flag";

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
 * @brief   Extra Time Which is added to the Start and End times in Minutes
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
* @brief   Precondition: Get Channel Numbers From xml File and Book two Event Based Recordings and set Keep flag
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
            res = CL.EA.PVR.BookFutureEventFromGuide("EventBasedRecording",recordableService.LCN,NumberOfPresses:3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to book future event from guide");
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("EventBasedRecording1", recordableService1.LCN, NumberOfPresses:3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event from guide");
            }

            //Setting the keep flag to true and verification of PCAT is taken care inside this EA itself
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording1",SetKeep:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag");
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
* @brief   Step 1 : Select one Event Based Recording and change the Keep flag
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
            //Setting the keep flag for the Event Based Recording which is initially false
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording", SetKeep:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag");
            }

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

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   Step 2 : Select the other Recording and change the Keep flag
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
            //Seting the keep flag of an other EBR to false
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording1", SetKeep:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag");
            }
            //Fetching the Event Start Time and End Time from The Event collection and adding extra time and modifying this EBR
            string evtStartTime = CL.EA.GetEventInfo("EventBasedRecording1", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved start time from event info is null");
            }
            LogComment(CL, "Event Start time is " + evtStartTime);
            string evtEndTime = CL.EA.GetEventInfo("EventBasedRecording1", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event End time is " + evtEndTime);
            //Including extra Start Guard Time
            TimeSpan startTime = TimeSpan.Parse(evtStartTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            //Including extra End Guard Time
            TimeSpan endTime = TimeSpan.Parse(evtEndTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            res = CL.EA.PVR.ModifyManualRecording("EventBasedRecording1", startTime.ToString("hh\\:mm"), endTime.ToString("hh\\:mm"));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
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
        res = CL.EA.PVR.CancelBookingFromPlanner("EventBasedRecording");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to cancel the Booking from planner");
        }
        res = CL.EA.PVR.CancelBookingFromPlanner("EventBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to cancel the Booking from planner");
        }

    }
    #endregion


}