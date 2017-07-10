/// <summary>
///  Script Name : LIB_0041_Book_Delete_Ongoing_CompletedRecording.cs
///  Test Name   : LIB-BOOK-0041-Delete ongoing completed recording_FT146
///  TEST ID     : 24615
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
* @class  BOOK_0041
*
* @brief   From the video library (My Recording), the user shall be able to delete an ongoing / completed recording 
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

[Test("BOOK_0041")]
public class BOOK_0041 : _Test
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
* @brief   The recordableService.
**************************************************************************************************/

    private static Service recordableService1;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and have Event Based Recordings";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Delete one of the Past Event Based Recordings";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2:Verify that the Deleted Event Based Recording is not Present in the Archive";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 3.
**************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: Navigate to Delete option to another Event Based Recording and select No while confirming";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 4.
**************************************************************************************************/

    private const string STEP4_DESCRIPTION = "Step 4:Verify that the recording is present in the Archive";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 5.
**************************************************************************************************/

    private const string STEP5_DESCRIPTION = "Step 5:Stop the ongoing Event Based Recording from Archive";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 6.
**************************************************************************************************/

    private const string STEP6_DESCRIPTION = "Step 6:Verify that the recording is Stopped and present in Archive";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 7.
*************************************************************************************************/

    private const string STEP7_DESCRIPTION = "Step 7:Delete the Ongoing record from Archive";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 8.
**************************************************************************************************/

    private const string STEP8_DESCRIPTION = "Step 8:Verify the Deleted Recording is not present in Archive";

    /**********************************************************************************************/

    #region Create Structure
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
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);
        this.AddStep(new Step6(), STEP6_DESCRIPTION);
        this.AddStep(new Step7(), STEP7_DESCRIPTION);
        this.AddStep(new Step8(), STEP8_DESCRIPTION);
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
* @brief   Precondition: Get Channel Numbers From xml File and have Event Based Recordings
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live,recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+recordableService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording1",MinTimeBeforeEvEnd:2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record to Current Event from banner");
            }
            res = CL.EA.PVR.StopRecordingFromBanner("EventBasedRecording1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop the record from Banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService1.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording2",MinTimeBeforeEvEnd:2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record to Current Event from Banner");
            }
            res = CL.EA.PVR.StopRecordingFromBanner("EventBasedRecording2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop the record from Banner");
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
* @brief   Step 1: Delete one of the Past Event Based Recordings
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
            res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to delete record from Archive");
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
* @brief   Step 2:Verify that the Deleted Event Based Recording is not Present in the Archive";
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
            //Verifying that the Event is not present in Archive 
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording1",SupposedToFindEvent:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Found the deleted Event in Archive");
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
* @brief   Step 3: Navigate to Delete option to another Event Based Recording and select No while confirming
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
            //Navigating to that Event in Archive and Selecting Delete No from Action Bar
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording2", Navigate:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event in Archive");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CONFIRM DELETE NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to Confirm Delete No");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4

    /**********************************************************************************************/
    /**
* @class   Step4
*
* @brief   Step 4:Verify that the recording is present in the Archive
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
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
            //Verifying that the Event is Present in Archive as we have selected Delete No
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to find event in Archive");
            }


            PassStep();
        }
    }
    #endregion
    #region Step5

    /**********************************************************************************************/
    /**
* @class   Step5
*
* @brief   Step 5:Stop the ongoing Event Based Recording from Archive
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    [Step(5, STEP5_DESCRIPTION)]
    private class Step5 : _Step
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
            //As per the test case this recording should be started in Pre-Condition but to make sure that the Recording is still going on i am recording it now
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording3",MinTimeBeforeEvEnd:5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record current event from Banner");
            }
            res = CL.EA.PVR.StopRecordingFromArchive("EventBasedRecording3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop recording from Archive");
            }


            PassStep();
        }
    }
    #endregion
    #region Step6

    /**********************************************************************************************/
    /**
* @class   Step6
*
* @brief   Step 6:Verify that the recording is Stopped and present in Archive
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    [Step(6, STEP6_DESCRIPTION)]
    private class Step6 : _Step
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
            // verfying that the event is present in Archive
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify Event in Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step7

    /**********************************************************************************************/
    /**
* @class   Step7
*
* @brief   Step 7:Delete the Ongoing record from Archive
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    [Step(7, STEP7_DESCRIPTION)]
    private class Step7 : _Step
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
            //Deleting the on going record from Archive 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService1.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording4", MinTimeBeforeEvEnd: 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from Banner");
            }
            res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording4");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to delete the on going record fro Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step8

    /**********************************************************************************************/
    /**
* @class   Step8
*
* @brief  Step 8:Verify the Deleted Recording is not present in Archive
*
* @author  Madhu Kumar K
* @date    25th Nov, 2013
**************************************************************************************************/

    [Step(8, STEP8_DESCRIPTION)]
    private class Step8 : _Step
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
            //Verifying that this event is not present in Archive
            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRecording4",SupposedToFindEvent:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Found the event in archive after deleting");
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
        //Deleting all the events from Arhive
        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording1");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed To delete Record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording2");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed To delete Record from Archive");
        }

        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording3");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed To delete Record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive("EventBasedRecording4");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed To delete Record from Archive");
        }
    }
    #endregion


}