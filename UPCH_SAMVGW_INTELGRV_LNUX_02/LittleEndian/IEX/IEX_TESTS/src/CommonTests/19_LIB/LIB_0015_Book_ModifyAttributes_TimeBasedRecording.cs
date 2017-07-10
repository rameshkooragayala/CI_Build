/// <summary>
///  Script Name : LIB_0015_Book_ModifyAttributes_TimeBasedRecording.cs
///  Test Name   : LIB-BOOK-0015-Modify attributes on time-based recording
///  TEST ID     : 4469
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified :  3rd Dec, 2013
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
* @class  BOOK_0015
*
* @brief   From the (planner)record bookings list, User is able to modify some attributes on a time-based recording
*
* @author  Madhu Kumar K
* @date    3rd Dec, 2013
**************************************************************************************************/

[Test("BOOK_0015")]
public class BOOK_0015 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/

    /**
* @brief   The recordableService.
**************************************************************************************************/

    private static Service recordableService;

    /**********************************************************************************************/
    /**
* @brief   The recordableService 1.
**************************************************************************************************/

    private static Service recordableService1;

    /**********************************************************************************************/
    /**
* @brief   The recordableService 2.
**************************************************************************************************/

    private static Service recordableService2;

    /**********************************************************************************************/
    /**
* @brief   The recordableService 3.
**************************************************************************************************/

    private static Service recordableService3;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and Book three Time Based Recordings";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Modify Start Time, End Time and Keep attributes of Time Based Recording";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Modify the Frequency of Time Based Recording1";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: Modify Date and channel name attributes of the Time Based Recording2";

    /**********************************************************************************************/
    /**
  

* @class   Constants
*
* @brief   Constants.
*
* @author Madhu Kumar K
* @date    3rd Dec, 2013
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
* @date    3rd Dec, 2013
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
* @date    3rd Dec, 2013
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
* @brief   Get Channel Numbers From xml File and Book three Time Based Recordings
* @author  Madhu Kumar K
* @date   3rd Dec, 2013
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
* @date   3rd Dec, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService.LCN);
            }
            recordableService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN="+recordableService.LCN);
            if (recordableService1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService1.LCN);
            }
            recordableService2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService.LCN+","+recordableService1.LCN);
            if (recordableService2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService2.LCN);
            }
			recordableService3 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService.LCN+","+recordableService1.LCN+","+recordableService2.LCN);
            if (recordableService3 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService3.LCN);
            }
            //Days Delay -1 Will select Today for Date
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording", recordableService.Name, DaysDelay: -1, MinutesDelayUntilBegining: 30, DurationInMin: 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Record Manual from Planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording1", recordableService1.Name, DaysDelay: -1, MinutesDelayUntilBegining: 30, DurationInMin: 10,Frequency:EnumFrequency.DAILY);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Manual from Planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording2", recordableService2.Name, DaysDelay: 1, MinutesDelayUntilBegining: 15, DurationInMin: 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Manual from Planner");
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
* @brief   Step 1 : Modify start time, end time and keep attributes of the Time Based Recording
*
* @author Madhu Kumar K
* @date   3rd Dec, 2013
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
* @date   3rd Dec, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
          //Fetching Event Start time from Event key
            string evtStartTime = CL.EA.GetEventInfo("TimeBasedRecording", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved start time from event info is null");
            }
            LogComment(CL, "Event Start time is " + evtStartTime);
            string evtEndTime = CL.EA.GetEventInfo("TimeBasedRecording", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event End time is " + evtEndTime);
            //Including extra Start Guard Time
            TimeSpan startTime = TimeSpan.Parse(evtStartTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            //Including extra End Guard Time
            TimeSpan endTime = TimeSpan.Parse(evtEndTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            res = CL.EA.PVR.ModifyManualRecording("TimeBasedRecording", startTime.ToString("hh\\:mm"), endTime.ToString("hh\\:mm"));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
            }
            res = CL.EA.PVR.SetKeepFlag("TimeBasedRecording", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag");
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
* @brief   Step 2 :Modify frequency of theTime Based Recording1
*
* @author  Madhu Kumar K
* @date    3rd Dec, 2013
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
* @date    3rd Dec, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.ModifyManualRecording("TimeBasedRecording1", Frequency:EnumFrequency.ONE_TIME);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
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
* @brief   Step 3 :Modify channel name and date attributes of Time Based Recording2
*
* @author  Madhu Kumar K
* @date    3rd Dec, 2013
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
* @date    3rd Dec, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Days 0 will select todays date
            res = CL.EA.PVR.ModifyManualRecording("TimeBasedRecording2",ChannelName:recordableService3.Name,Days:0);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner("TimeBasedRecording2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify recurring booking in planner");
            }
            res = CL.EA.WaitUntilEventEnds("TimeBasedRecording2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Wait until Event ends");
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
* @date    3rd Dec, 2013
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete Record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete Record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording2");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete Record from Archive");
        }
    }
    #endregion


}