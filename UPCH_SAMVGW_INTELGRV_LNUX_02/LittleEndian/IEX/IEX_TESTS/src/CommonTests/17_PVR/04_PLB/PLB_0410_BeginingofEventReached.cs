/// <summary>
///  Script Name : PLB_0410_BeginingofEventReached
///  Test Name   : PLB-0410-Begining of Event Reached
///  Repository  :STB_DIVISION
///  TEST ID     : 71348
///  QC Version  : 1
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Appanna Kangira
/// Last modified : 29 Oct 2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************//**
 * @class   PLB_0410
 *
 * @brief   When the begining of the recording is reached, the fusion product shall automatically go in Play mode.
 * @author  Appannak
 * @date    24-Oct-13
 **************************************************************************************************/

[Test("PLB_0410_BeginingofEventReached")]
public class PLB_0410 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The service to be recorded.
     **************************************************************************************************/

    private static Service serviceToBeRecorded;

    /**********************************************************************************************//**
     * @brief   The event to be recorded.
     **************************************************************************************************/

    private static string eventToBeRecorded = "EVENT1";

  
    


    private static string TM_RWStr;
    private static double TM_RW;

    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   A constants.
     *
     * @author  Appannak
     * @date    24-Oct-13
     **************************************************************************************************/

    private static class Constants
    {
        /**********************************************************************************************//**
         * @brief   The seconds to play.
         **************************************************************************************************/

        public const int secsToPlay = 0;

        /**********************************************************************************************//**
         * @brief   in secs.
         **************************************************************************************************/

        public const int minTimeBeforeEventEnd = 5;

        /**********************************************************************************************//**
         * @brief   in secs.
         **************************************************************************************************/

        public const int secToWaitForRecord = 120;

        /**********************************************************************************************//**
         * @brief   in secs.
         **************************************************************************************************/



        public const int waitforPlaybackbeforeRWBOF = 60;
    }

    /**********************************************************************************************//**
     * @brief   Event queue for all listeners interested in PRECONDITION_DESCRIPTION events.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition:Fetch service required for the test case.Initiate Single event based recordings on a service";

    /**********************************************************************************************//**
     * @brief   Information describing the step 1.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Stop the Initiated Recording ";

    /**********************************************************************************************//**
     * @brief   Information describing the step 2.
     **************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Playback the recording till BOF and after Verify the recording is automatically Played in Playmode ";
    
    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Appannak
     * @date    24-Oct-13
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

    /**********************************************************************************************//**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Appannak
     * @date    24-Oct-13
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
     * @brief   A pre condition.
     *
     * @author  Appannak
     * @date    24-Oct-13
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    24-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Channel Values From XML File
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True","ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            //Fetch the trick Mode RW MIN Speed from PROJECT INI.
            TM_RWStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "REW_MIN");
            TM_RW = double.Parse(TM_RWStr);


            //Tune to the service whose event will be recorded.
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded.LCN);
            }

            ////Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule event  recording");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   A step 1.
     *
     * @author  Appannak
     * @date    24-Oct-13
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    24-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            LogCommentInfo(CL, "Waiting for " + Constants.secToWaitForRecord + " secs to ensure sufficient duration of recording");
            res = CL.IEX.Wait(Constants.secToWaitForRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required for the recording to go on ");
            }

            res = CL.EA.PVR.StopRecordingFromBanner(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop the eventRecorded From Banner");
            }
            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   A step 2.
     *
     * @author  Appannak
     * @date    24-Oct-13
     **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    24-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
                //Play the recording from Archive.
                res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.secsToPlay, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Playback the Event " + eventToBeRecorded + " From Archive");
                }

                LogCommentInfo(CL, "Waiting for " + Constants.waitforPlaybackbeforeRWBOF + " secs to some Play duration to do rewind");
                CL.IEX.Wait(Constants.waitforPlaybackbeforeRWBOF);

                

                //Set the Trick Mode Speed to RW_MIN Value & Verify BOF & Playback speed 1(Play mode)
                res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, TM_RW, true);
                LogCommentInfo(CL, "The RW speed is " + TM_RW);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set the RW_MIN  Speed :" + TM_RW);
                }

                LogCommentInfo(CL, "The Verification of BOF and there after Play From Begining in Play mode is successful");       
             
                
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
     * @author  Appannak
     * @date    24-Oct-13
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {

        IEXGateway._IEXResult res;


        //Delete the recorded event  from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to delete the recorded event  from archive!");
        }
    }
    #endregion
}