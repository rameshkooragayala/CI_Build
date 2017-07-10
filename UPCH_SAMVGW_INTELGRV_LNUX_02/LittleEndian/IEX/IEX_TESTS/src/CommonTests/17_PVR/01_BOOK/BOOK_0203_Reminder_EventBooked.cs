/// <summary>
///  Script Name : BOOK_0203_Reminder_EventBooked.cs
///  Test Name   : BOOK-0203 Booking Reminder-Event Booked
///  TEST ID     : 4308
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
* @class  BOOK_0203_REMINDER 
*
* @brief   Can't book a reminder on a single event if there is event booking for same event
*
* @author  Madhu Kumar K
* @date    25th OCT, 2013
**************************************************************************************************/

[Test("BOOK_0203_REMINDER")]
public class BOOK_0203_REMINDER : _Test
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
* @brief   The service.
**************************************************************************************************/

    private static Service service1;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Book a Reminder on the event";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Book a Future Event Based Recording";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 3.
**************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: Try to book the reminder on the same Event on which recording is booked";

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
* @brief   Number of presses for Next Event.
**************************************************************************************************/

        public const int numOfPressesForNextEvent = 2;

        /**********************************************************************************************/
        /**
* @brief   Expected Error Code for Navigation Failure
**************************************************************************************************/

        public const int expectedErrorCode = 162;

        /**********************************************************************************************/
        /**
* @brief   Minimum time required before event ends in seconds
**************************************************************************************************/

        public const int reqMinTimeBeforeEventEnds = 240;

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
* @brief   Gets the required service from xml.
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
* @date   10 OCT,13
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            service = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }
            service1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High;LCN=" + service.LCN);
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service1.LCN);
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
* @brief   Step 1 : Book a Reminder on the Event
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

            res= CL.EA.BookReminderFromGuide("ReminderEvent",service.LCN,Constants.numOfPressesForNextEvent);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed To book Future Event from Guide");
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
* @brief   Step 2 : Book a Future Time Based Recording
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
            res=CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Tune to Service "+service1.LCN);
            }
            int timeLeftInSec=0;
            CL.EA.GetCurrentEventLeftTime(ref timeLeftInSec);
            if (timeLeftInSec < Constants.reqMinTimeBeforeEventEnds)
            {
                res = CL.IEX.Wait(timeLeftInSec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait until Event Ends");
                }               
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("EventBasedRecording", service1.LCN, Constants.numOfPressesForNextEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To book Furture Event From Guide");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**********************************************************************************************/
    /**
* @class   Step3
*
* @brief   Step 3 : Try to book the reminder on the same Event on which recording is booked
*
* @author  Madhu Kumar K
* @date    25th OCT, 2013
**************************************************************************************************/

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
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
            LogCommentImportant(CL, "We have already booked an Event for Recording on the same event so it should not allow Reminder again");
            res = CL.EA.BookReminderFromGuide("ReminderEvent1", service1.LCN, Constants.numOfPressesForNextEvent);
            if (!res.CommandSucceeded)
            {
                if (res.FailureCode != Constants.expectedErrorCode)
                {
                    FailStep(CL, res,"Expected Failure code "+Constants.expectedErrorCode+" and recieved Failure code are different "+res.FailureCode);
                }
            }
            else
            {
                FailStep(CL,"Successful in booking the reminder from guide after booking recording the same event");
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
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL,"Failed to cancel Bookings from planner");
        }
        res = CL.EA.CancelReminderFromGuide("ReminderEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed To Cancel the Reminder From Guide");
        }
    }
    #endregion

   
}