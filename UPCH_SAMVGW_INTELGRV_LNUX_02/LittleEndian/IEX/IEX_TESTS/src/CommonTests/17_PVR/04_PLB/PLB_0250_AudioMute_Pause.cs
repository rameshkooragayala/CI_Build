/// <summary>
///  Script Name : PLB_0250_AudioMute_Pause
///  Test Name   : PLB-0250-Audio mute in pause
///  TEST ID     : 68952
///  Repository  :STB_DIVISION
/// Jira ID     : FC-705
///  QC Version  : 1
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by :Scripted by : Appanna Kangira
/// Last modified : 03 Oct 2013
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
 * @class   PLB_0250
 *
 * @brief   A plb 0250.
 *
 * @author  Appannak
 * @date    01-Oct-13
 **************************************************************************************************/

[Test("PLB_0250_AudioMute_Pause")]
public class PLB_0250 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    

   
    /**********************************************************************************************//**
     * @brief   Service to be recorded.
     **************************************************************************************************/

    private static Service serviceToBeRecorded;

    /**********************************************************************************************//**
     * @brief   The event to be recorded.
     **************************************************************************************************/

    private static string eventToBeRecorded="EVENT_TO_BE_RECORDED";

    /**********************************************************************************************//**
     * @brief   The default audio check timeout.
     **************************************************************************************************/

    private static int defaultAudioCheckTimeout;

    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   All the Values which doesn't change are put in Constants Class
     *
     * @author  Appannak
     * @date    01-Oct-13
     **************************************************************************************************/

    private static class Constants
    {
 

        /**********************************************************************************************//**
         * @brief   The trick mode for pause.
         **************************************************************************************************/

        public const double trickModeForPause = 0;

        /**********************************************************************************************//**
         * @brief   The seconds to play.
         **************************************************************************************************/

        public const int secsToPlay = 0;

        /**********************************************************************************************//**
         * @brief   In minutes.
         **************************************************************************************************/

        public const int minTimeBeforeEventEnd = 4;

        /**********************************************************************************************//**
         * @brief   In Seconds
         **************************************************************************************************/


        public const int secToWaitForRecord = 60;  

    }

    /**********************************************************************************************//**
     * @brief   Event queue for all listeners interested in PRECONDITION_DESCRIPTION events.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition:Fetch a service required for the test case.Initiate an event based recording";

    /**********************************************************************************************//**
     * @brief   Information describing the step 1.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Playback the recording ";

    /**********************************************************************************************//**
     * @brief   Information describing the step 2.
     **************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Pause the recording Playback and Check for Audio mute ";
   

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Appannak
     * @date    01-Oct-13
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
     * @date    01-Oct-13
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
     * @date    01-Oct-13
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
         * @date    01-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            
            //Get Channel Values From ini File
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True","ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            //fetch the Default Audio Check Time Out of respective Project from Project.ini
            String defaultAudioCheckTimeoutInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultAudioCheckTimeoutInStr))
            {
                FailStep(CL, res, "DEFAULT_AUDIO_CHECK_SEC value is not present in the Project.ini");
            }

            defaultAudioCheckTimeout = int.Parse(defaultAudioCheckTimeoutInStr);

            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded.LCN);
            }

           

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            LogCommentInfo(CL, "Waiting for " + Constants.secToWaitForRecord + " secs to ensure sufficient duration of recording");
            res = CL.IEX.Wait(Constants.secToWaitForRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required");
            }

        
            //Stop Recording 
            res = CL.EA.PVR.StopRecordingFromBanner(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Event From Live");
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
     * @date    01-Oct-13
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
         * @date    01-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
           
            //Play the recording from Archive
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.secsToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + eventToBeRecorded + " From Archive");
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
     * @date    01-Oct-13
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
         * @date    01-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();           
            //Check for Audio Present before PAUSE
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio is missing on recording Playback");

            }
            
            // Pause the Playback
            res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, Constants.trickModeForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause the recording Playback");
            }

            
            //Check for Audio is muted on Pause mode
            res = CL.EA.CheckForAudio(false, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio is not muted on recording Pause");

            }
            else
            {
                LogCommentInfo(CL, "Audio mute on recording PAUSE succeessful");

            }
           
            //Stop the recording Playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback");
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
     * @author  Appannak
     * @date    01-Oct-13
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