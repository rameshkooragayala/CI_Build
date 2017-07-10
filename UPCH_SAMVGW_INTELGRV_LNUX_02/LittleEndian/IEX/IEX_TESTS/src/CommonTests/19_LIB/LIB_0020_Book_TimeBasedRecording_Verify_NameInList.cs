/// <summary>
///  Script Name : LIB_0021_Book_EventBased_To_TimeBasedRecording.cs
///  Test Name   : LIB-BOOK-0020-Time-based recording name in list
///  TEST ID     : 19484
///  QC Version  : 2
///  Variations from QC: None
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
* @class  BOOK_0020
*
* @brief   When presenting the title of a time-based recording, the Fusion-based Product shall present a composite name that may include one or more of the following record attributes: Channel name, Start/end time, Date

*
* @author  Madhu Kumar K
* @date    3rd Dec, 2013
**************************************************************************************************/

[Test("BOOK_0020")]
public class BOOK_0020 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/

    /**
* @brief   The Recordable service 1.
**************************************************************************************************/

    private static Service recordableService1;

    /**********************************************************************************************/
    /**
* @brief   The Recordable service 2.
**************************************************************************************************/

    private static Service recordableService2;

    /**********************************************************************************************/
    /**
* @brief   The Recordable service 3.
**************************************************************************************************/

    private static Service recordableService3;

    /**********************************************************************************************/
    /**
* @brief   The Date Format
**************************************************************************************************/

    private static string dictionaryDateFormat;
    /**********************************************************************************************/
    /**
* @brief   The EPG Date Format
**************************************************************************************************/

    private static string EPGDateFormat;
    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and have few Time Based Recordings";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Verify that the title of the Time Based Recordings contains the attributes";

    /**********************************************************************************************/


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
* @brief   Get Channel Numbers From xml File and have few Time Based Recording
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
            recordableService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (recordableService1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService1.LCN);
            }
            recordableService2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService1.LCN);
            if (recordableService2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService2.LCN);
            }
            recordableService3 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService1.LCN + "," + recordableService2.LCN);
            if (recordableService3 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService3.LCN);
            }
            dictionaryDateFormat = CL.EA.UI.Utils.GetValueFromProject("EPG", "DATE_FORMAT_FOR_EVENT_DIC");
            if (string.IsNullOrEmpty(dictionaryDateFormat))
            {
                FailStep(CL,"Retrived Date format from Project ini file empty");
            }
            EPGDateFormat = CL.EA.UI.Utils.GetEpgDateFormat();
            if (string.IsNullOrEmpty(EPGDateFormat))
            {
                FailStep(CL, "Retrived Date format for EPG is empty");
            }
            //Days Delay -1 will select todays as date
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording1",recordableService1.Name, DaysDelay: -1, MinutesDelayUntilBegining: 5, DurationInMin: 4, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record Manual from Planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording2", recordableService2.Name, DaysDelay: -1, MinutesDelayUntilBegining: 5, DurationInMin: 10, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record Manual from Planner");
            }
            res = CL.EA.WaitUntilEventStarts("TimeBasedRecording2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until Starts");
            }
            //Waiting one min after recording starts and Stopping the recording to make sure that it is a partial recording
            res = CL.IEX.Wait(seconds: 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait a min into Event");
            }
            res = CL.EA.PVR.StopRecordingFromArchive("TimeBasedRecording2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop record from Archive");
            }
            res = CL.EA.PCAT.VerifyEventPartialStatus("TimeBasedRecording2", "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event Partial Status");
            }
            //Days Delay -1 will select todays as date
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording3", recordableService3.Name, DaysDelay: -1, MinutesDelayUntilBegining: 5, DurationInMin: 15, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record Manual from Planner");
            }
            res = CL.EA.WaitUntilEventStarts("TimeBasedRecording3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event starts");
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
* @brief   Step 1 :  Verify that the title of the Time Based Recordings contains the attributes
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
            string title;
            string[] message = new string[]{"","Complete Recording","Partial Recording","On Going Recording"};
            for (int i = 1; i <= 3; i++)
            {
                LogCommentImportant(CL,"Verifying that the title of the "+message[i]+" contains any of the required attributes");
                //Fetching the Event Start Time and End Time from The Event collection and adding extra time and modifying this EBR
                string evtStartTime = CL.EA.GetEventInfo("TimeBasedRecording"+i, EnumEventInfo.EventStartTime);
                if (string.IsNullOrEmpty(evtStartTime))
                {
                    FailStep(CL, "Retrieved start time from event info is null");
                }
                LogComment(CL, "Event Start time is " + evtStartTime);
                string evtEndTime = CL.EA.GetEventInfo("TimeBasedRecording"+i, EnumEventInfo.EventEndTime);
                if (string.IsNullOrEmpty(evtEndTime))
                {
                    FailStep(CL, "Retrieved end time from event info is null");
                }
                LogComment(CL, "Event End time is " + evtEndTime);
                string evtChannelName = CL.EA.GetEventInfo("TimeBasedRecording"+i,EnumEventInfo.EventChannel);
                if (string.IsNullOrEmpty(evtChannelName))
                {
                    FailStep(CL, "Retrieved Channel name from event info is null");
                }
                LogComment(CL, "Channel Name is " + evtChannelName);
                string evtDate = CL.EA.GetEventInfo("TimeBasedRecording"+i, EnumEventInfo.EventDate);
                if (string.IsNullOrEmpty(evtDate))
                {
                    FailStep(CL, "Retrieved Channel name from event info is null");
                }
                evtDate = DateTime.ParseExact(evtDate, dictionaryDateFormat, System.Globalization.CultureInfo.InvariantCulture).ToString(EPGDateFormat);
                LogComment(CL, "Channel Name is " + evtDate);
                CL.IEX.MilestonesEPG.ClearEPGInfo();

                res = CL.EA.PVR.VerifyEventInArchive("TimeBasedRecording"+i);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify Event in Archive");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out title);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the EPG info from ");
                }
                
                if (!(title.Contains(evtChannelName)||(title.Contains(evtStartTime)&&title.Contains(evtEndTime))||title.Contains(evtDate)))
                {
                    FailStep(CL, res, "Title does not contain channel name or Event Start and End Time or date for the "+message[i]);
                }
                LogCommentInfo(CL, "The title of the " + message[i] + ": " + title + "contains required attribute");
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
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete the record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording2");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete the record from Archive");
        }
        res = CL.EA.PVR.StopRecordingFromArchive("TimeBasedRecording3");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to Stop the recording from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording3");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete the record from Archive");
        }
    }
    #endregion


}