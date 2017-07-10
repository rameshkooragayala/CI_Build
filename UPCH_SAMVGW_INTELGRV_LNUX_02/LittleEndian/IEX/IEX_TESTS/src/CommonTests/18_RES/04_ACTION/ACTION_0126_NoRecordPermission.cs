/// <summary>
///  Script Name : ACTION_0126_NoRecordPermission
///  Test Name   : ACTION-0126-NoRecordPermission-LiveEvent
///  TEST ID     : 64410
///  QC Version  : 1
//   Jira ID     : FC-248
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Renukaradhya 
///  Last modified : 1 AUGUST 2013 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************//**
 * @class   ACTION_0126
 *
 * @brief    Verify on non recordable service there is no permision to record a live channel ";
 *
 * @author  Madhu R
 * @date    9/12/2013
 **************************************************************************************************/

[Test("ACTION_0126")]
public class ACTION_0126 : _Test
{
    /**********************************************************************************************//**
     * @brief   Platform cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The service not recordable.
     **************************************************************************************************/

    static Service serviceNotRecordable;
   /**********************************************************************************************//**
     * @brief   The default not recordable message.
     **************************************************************************************************/
    static string notrecordableMessage = "";
    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    static class Constants
    {

         /**********************************************************************************************//**
         * @brief   The time for Milestone Arrival.
         **************************************************************************************************/

        public const int timeoutForMilestoneArrival = 30;
        
    }

    /**********************************************************************************************//**
     * @brief    Get Channel Numbers From xml File.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**********************************************************************************************//**
     * @brief   Verify for non recordable message when there is no permision to record on a live channel
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Verify for non recordable message when there is no permision to record on a live channel ";


    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Madhu R
     * @date    9/12/2013
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

    /**********************************************************************************************//**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Fetches a non recorable service from content xml.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhu R
         * @date    9/12/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Values From xml file File
            serviceNotRecordable = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=False;IsEITAvailable=True","ParentalRating=High");
            if (serviceNotRecordable == null)
            {
                FailStep(CL, "Unable to fetch a Service which is NotRecordable from content xml. ");
            }
            else
            {
                LogCommentInfo(CL, "Non Recordable Service is: " + serviceNotRecordable.LCN);

            }
            notrecordableMessage = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "NO_RECORD_MESSAGE");
            if (String.IsNullOrEmpty(notrecordableMessage))
            {
                FailStep(CL, res, "DEFAULT value No Record Message is not present in the Project.ini");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief  Tune to service S1 and try to book a recording on current event through action menu and Verify for non recordable when there is no permision to record a live channel.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhu R
         * @date    9/12/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceNotRecordable.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel Non Recordable Service " + serviceNotRecordable);
            }

            //Record a service which is non recordable and verify for nonrecordable message
            String Milestones = CL.EA.UI.Utils.GetValueFromMilestones("NonRecordableMessage");

            //Begin wait for nonrecordable message milestones arrival
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, Constants.timeoutForMilestoneArrival);

            //Record the non recordable service
                res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent");
                if (!res.CommandSucceeded)
                {

                    LogCommentInfo(CL, "Exception occured while recording as the service has no permission to record");

                    //verify for FAS milestones when service is non recordable
                    //End wait for Milestones arrival

                    ArrayList arrayList = new ArrayList();

                    if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arrayList))
                    {
                        FailStep(CL, "Unable to get NO_DRM_RECORDING_PERMISSION milestone ");
                    }
                    //UI verification of NoRecord Message
                    //String obtainedNoRecordMessage = "";
                    //res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedNoRecordMessage);
                    //if (!res.CommandSucceeded)
                    //{
                    //    FailStep(CL, res, "Failed to get the no record message");
                    //}

                    //LogCommentInfo(CL, "obtainedNoRecordMessage is : " + obtainedNoRecordMessage);
                    //LogCommentInfo(CL, "expectedNorecordMessage  is : " + notrecordableMessage);
                    //if (notrecordableMessage.Equals(obtainedNoRecordMessage.Trim()))
                    //{
                    //    LogCommentInfo(CL, "No Record Message displayed successfully");
                    //}
                    //else
                    //{
                    //    FailStep(CL, res, "Expected No Record Message is not displayed ");
                    //}
                }
            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   Posts the execute.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}