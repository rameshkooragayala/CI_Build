/// <summary>
///  Script Name : EPG_PLB_0010_Action_Menu_Simple_Program_Info.cs
///  Test Name   : EPG-PLB-0010-ActionMenu-Simple Program Info
///  TEST ID     : 35586
///  QC Version  : 10
/// -----------------------------------------------
///  Modified by : Anshul Upadhyay
///  Deviations from HPQC :Th Event Based Recording part is covered in Banner_1211_MultipleEvents.cs 
///  Program duration is not handled as it is not part of Milestone.
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
 * @class   ACTION_0014
 *
 * @brief   Action 0014.
 *
 * @author  Anshulu
 * @date    Fri-Sep-27
 **************************************************************************************************/

[Test("ACTION_0014_Action_Menu_Simple_Program_Info")]
public class ACTION_0014 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;


    /**********************************************************************************************//**
     * @brief   The service to be manual recorded.
     **************************************************************************************************/

    static Service serviceToBeManualRecorded;

    /**********************************************************************************************//**
     * @brief   Manual recording key.
     **************************************************************************************************/

    static string manualRecording = "MANUAL_RECORDING";

    

    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Anshulu
     * @date    Fri-Sep-27
     **************************************************************************************************/

    static class Constants
    {
       
        /**********************************************************************************************//**
         * @brief   The days delay.
         **************************************************************************************************/

        public const int daysDelay = -1;

        /**********************************************************************************************//**
         * @brief   The minute delay in beginning.
         **************************************************************************************************/

        public const int minuteDelayInBeginning = 2;

        /**********************************************************************************************//**
         * @brief   The duration in mins of manual record.
         **************************************************************************************************/

        public const int durationInMinsOfManualRecord = 4;

        /**********************************************************************************************//**
         * @brief   The value for full playback.
         **************************************************************************************************/

        public const int secToPlay = 0;

        /**********************************************************************************************//**
         * @brief   The play from beginning.
         **************************************************************************************************/

        public const bool playFromBeginning = true;

        /**********************************************************************************************//**
         * @brief   The verify EOF.
         **************************************************************************************************/

        public const bool verifyEOF = false;

        /**********************************************************************************************/
        /*** @brief   verify in PCAT TRUE/FALSE.
        **************************************************************************************************/

        public const bool verifyInPCAT = false;
        
    }
    
    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Anshulu
     * @date    Fri-Sep-27
     **************************************************************************************************/

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the channels.Have a time based recording  in the library.");
        this.AddStep(new Step1(), "Step 1: Playback the recorded event");        
        this.AddStep(new Step2(), "Step 2: Launch Action Menu During Playback of Manually Recorded Content & Verify the Program Name");

        //Get Client Platform
        CL = GetClient();

    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Pre condition.
     *
     * @author  Anshulu
     * @date    Fri-Sep-27
     **************************************************************************************************/

    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    Fri-Sep-27
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Channel Values From xml File
			LogCommentInfo(CL,"Get Channel Values From xml File");
            serviceToBeManualRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;IsRecordable=True","ParentalRating=High");
            if (serviceToBeManualRecorded == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
 

            //Tune to the service whose future event will be recorded
			LogCommentInfo(CL,"Tune to the service whose manual recording has to be set");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeManualRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeManualRecorded.LCN);
            }

            //Schedule a time based recording
			LogCommentInfo(CL,"Schedule a time based recording");
            res = CL.EA.PVR.RecordManualFromPlanner(manualRecording, serviceToBeManualRecorded.Name, Constants.daysDelay, Constants.minuteDelayInBeginning, Constants.durationInMinsOfManualRecord, EnumFrequency.ONE_TIME, Constants.verifyInPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule time based recording");
            }

            //Wait for sometime for the recording to happen
			LogCommentInfo(CL,"Wait for sometime for the recording to happen");
            res = CL.IEX.Wait((Constants.durationInMinsOfManualRecord + Constants.minuteDelayInBeginning) * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until recording is completed");
            }
           

            //Verify whether the manual recording is present in the library
			LogCommentInfo(CL,"Verify whether the manual recording is present in the library");
            res = CL.EA.PVR.VerifyEventInArchive(manualRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the manual recording in the library!");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Step 1.
     *
     * @author  Anshulu
     * @date    Fri-Sep-27
     **************************************************************************************************/

    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    Fri-Sep-27
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Playback the recording
            LogCommentInfo(CL, "Playback the recording");
            res = CL.EA.PVR.PlaybackRecFromArchive(manualRecording, Constants.secToPlay, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Playback failed!!");
            }
            
            PassStep();
             
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   Step 2.
     *
     * @author  Anshulu
     * @date    Fri-Sep-27
     **************************************************************************************************/

    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    Fri-Sep-27
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Clear  EPG INFO
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            //Launch Action Bar
            LogCommentInfo(CL, "Launch Action Bar");
            res = CL.EA.LaunchActionBar(true);  //true for Action bar of playback
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to ACTION BAR");
            }


            //get title
            LogCommentInfo(CL, "get Event Name during Playback");
            string title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to GetEPGInfo title");
            }

            if (title != serviceToBeManualRecorded.Name)
            {
                FailStep(CL, res, "Service name, Fetched during Playback of Time Based Recording:" + title + "not matching Channel name" + serviceToBeManualRecorded.Name);
            }

            //Dismiss Action Bar
            LogCommentInfo(CL, "Dismiss Action Bar");
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to dismiss Actiom Menu To Full Screen");
            }

            //Stop playback
            LogCommentInfo(CL, "Stop playback");
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback!!");
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
     * @author  Anshulu
     * @date    Fri-Sep-27
     **************************************************************************************************/

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //deleting recording from archieve

        res = CL.EA.PVR.DeleteRecordFromArchive(manualRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete recording because" + res.FailureReason);
        }
    }
    #endregion
}