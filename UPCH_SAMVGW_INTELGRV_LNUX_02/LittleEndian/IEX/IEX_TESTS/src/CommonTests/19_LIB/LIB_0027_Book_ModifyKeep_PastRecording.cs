/// <summary>
///  Script Name : LIB_0027_Book_ModifyKeep_PastRecording.cs
///  Test Name   : LIB-BOOK-0027-Modify keep attribute of past recording
///  TEST ID     : 4473
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified :  4th Dec, 2013
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
* @class  BOOK_0027
*
* @brief   From the video library, the user shall be able to modify the Keep attribute of a recording.
*
* @author  Madhu Kumar K
* @date    4th Dec, 2013
**************************************************************************************************/

[Test("BOOK_0027")]
public class BOOK_0027 : _Test
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
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and have Recordings and set the keep status to True and False";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Modify Keep status of Past Event Based Recording to Keep and to Not Keep";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Modify Keep status of Past Time Based Recording to keep and to Not Keep";

    /**********************************************************************************************/


    #region Create Structure

    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author Madhu Kumar K
* @date    4th Dec, 2013
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
* @date    4th Dec, 2013
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
* @brief   Precondition: Get Channel Numbers From xml File and have Recording and set the keep status
* @author  Madhu Kumar K
* @date   4th Dec, 2013
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
* @date   4th Dec, 2013
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+recordableService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording1",MinTimeBeforeEvEnd:5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record current event from Banner");
            }
            //Modifying the Keep status of the ongoing Event Based recording to True
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording1",SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag True");
            }
            //Modifying the Keep status of the ongoing Event Based recording to False
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording1",SetKeep: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag False");
            }
            //Days Delay -1 will select todays date
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording1", recordableService.Name, DaysDelay: -1, MinutesDelayUntilBegining:5,DurationInMin: 5, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record Manual from Planner");
            }
            res = CL.EA.WaitUntilEventStarts("TimeBasedRecording1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event starts");
            }
            //Modifying the Keep status of the ongoing Time Based recording to True
            res = CL.EA.PVR.SetKeepFlag("TimeBasedRecording1",SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag True");
            }
            //Modifying the Keep status of the ongoing Time Based recording to False
            res = CL.EA.PVR.SetKeepFlag("TimeBasedRecording1", SetKeep:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag to False");
            }
            //Waiting until the Events are completed so that we have a past Event Based and Time Based Recording
            res = CL.EA.WaitUntilEventEnds("EventBasedRecording1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait until Event Ends");
            }
            res = CL.EA.WaitUntilEventEnds("TimeBasedRecording1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event ends");
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
* @brief   Step 1 : Modify Keep status of Past Event Based Recording to Keep and to Not Keep
*
* @author Madhu Kumar K
* @date   4th Dec, 2013
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
* @date   4th Dec, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Modifying the Keep status of the Past Event Based Recording to True and False
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording1",SetKeep:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag to true");
            }
            res = CL.EA.PVR.SetKeepFlag("EventBasedRecording1",SetKeep: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag False");
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
* @brief   Step 2 : Modify Keep status of Past Time Based Recording to Keep and to Not Keep
*
* @author  Madhu Kumar K
* @date    4th Dec, 2013
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
* @date    4th Dec, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Modifying the Keep status of the Past Time Based Recording to True and False
            res = CL.EA.PVR.SetKeepFlag("TimeBasedRecording1",SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag to True");
            }
            res = CL.EA.PVR.SetKeepFlag("TimeBasedRecording1",SetKeep: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set keep flag to Flase");
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
* @date    4th Dec, 2013
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed To delete Record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("TimeBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed To delete Record from Archive");
        }

    }
    #endregion


}