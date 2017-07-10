/// <summary>
///  Script Name : BOOK_0205_SingleRecording.cs
///  Test Name   : BOOK-0205 Booking a single recording current according to usage rules
///  TEST ID     : 20801
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
using FailuresHandler;

/**********************************************************************************************/
/**
* @class  BOOK_0205 
*
* @brief   
*
* @author  Madhu Kumar K
* @date    25th OCT, 2013
**************************************************************************************************/

[Test("BOOK_0205")]
public class BOOK_0205 : _Test
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
* @brief   The service.
**************************************************************************************************/

    private static Service nonRecordableService;

    /**********************************************************************************************/
    /**
* @brief   The service.
**************************************************************************************************/

    private static string timeBasedRecordingSupported;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Book an Event Based and Time Based Recordings on the Recordable Service";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Try to Book an Event Based and Time Based Recordings on the Non-Recordable Service";

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
* @brief   Minutes Delay until Beginning
**************************************************************************************************/

        public const int minDelayUntilBeginning = 2;

        /**********************************************************************************************/
        /**
* @brief   Minutes Delay until Beginning
**************************************************************************************************/

        public const int totalDurationInMin = 10;

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
* @date   25th OCT, 2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;Encryption=Scrambled;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService.LCN);
            }
            nonRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=False;Encryption=Scrambled;IsEITAvailable=True", "ParentalRating=High");
            if (nonRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + nonRecordableService.LCN);
            }
            timeBasedRecordingSupported = CL.EA.GetValueFromINI(EnumINIFile.Project, "MANUAL_RECORDING", "SUPPORTED");
            LogComment(CL,"Fetched value for time based recording supported is "+timeBasedRecordingSupported);
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   Step 1 : Book an Event Based and Time Based Recordings on the Recordable Service
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + recordableService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventbasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record current event from Banner on " + recordableService);
            }
            if (timeBasedRecordingSupported.ToUpper() == "TRUE")
            {
                res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording", recordableService.Name, -1, Constants.minDelayUntilBeginning, Constants.totalDurationInMin);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Book a Time Based Recording on " + recordableService.LCN);
                }
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
* @brief   Step 2 : Try to Book an Event Based and Time Based Recordings on the Non-Recordable Service
*
* @author  Madhu Kumar K
* @date    10 OCT,13
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, nonRecordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + nonRecordableService.LCN);
            }
            LogCommentImportant(CL, "The below EA's RecordCurrentEventFromBanner and RecordManualFromPlanner are supposed to fail as we are trying to book on Non-recordable service");
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording1");
            if (!res.CommandSucceeded)
            {
                if (res.FailureCode != ExitCodes.RecordEventFailure.GetHashCode())
                {
                    FailStep(CL, res, "Failed to Record current event from Banner on " + nonRecordableService);
                }
            }
            else
            {
                FailStep(CL,"Successfull in booking the recording on a Non-Recordable service");
            }
            if (timeBasedRecordingSupported.ToUpper() == "TRUE")
            {
                res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording1", nonRecordableService.Name, -1, Constants.minDelayUntilBeginning, Constants.totalDurationInMin);
                if (!res.CommandSucceeded)
                {
                    if (res.FailureCode != ExitCodes.RecordEventFailure.GetHashCode())
                    {
                        FailStep(CL, res, "Failed To Book a Time Based Recording on " + nonRecordableService.LCN);
                    }
                }
                else
                {
                    FailStep(CL, "Successfull in booking the recording on a Non-Recordable service");
                }
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
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete All the recordings from Archive");
        }
    }
    #endregion


}