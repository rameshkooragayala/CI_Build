/// <summary>
///  Script Name : ACTION_0011_Action_Menu_Simple_Program_Info_Not_Available.cs
///  Test Name   : ACTION-0011-Action_Menu-Simple Program Info-Info Not Available
///  TEST ID     : 35646
///  QC Version  : 10
/// -----------------------------------------------
///  Modified by : Madhu Thomas/Anshul Upadhyay
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Reflections;

/**********************************************************************************************//**
 * @class   ACTION_0013
 *
 * @brief   Action 0013.
 *
 * @author  Anshulu
 * @date    01/10/13
 **************************************************************************************************/

[Test("ACTION_0013_Action_Menu_Simple_Program_Info_Not_Available")]
public class ACTION_0013 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The service to be recorded.
     **************************************************************************************************/

    static Service serviceToBeRecorded;

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
     * @date    01/10/13
     **************************************************************************************************/

    static class Constants
    {
        /**********************************************************************************************//**
         * @brief   -1 in order to record today.
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
         * @brief   The security to wait after start of record.
         **************************************************************************************************/

        public const int secToWaitAfterStartOfRecord = 60;

        /**********************************************************************************************//**
         * @brief   The value for full playback.
         **************************************************************************************************/

        public const int valueForFullPlayback = 0;

        /**********************************************************************************************//**
         * @brief   The play from beginning.
         **************************************************************************************************/

        public const bool playFromBeginning = true;

        /**********************************************************************************************//**
         * @brief   The verify EOF.
         **************************************************************************************************/

        public const bool verifyEOF = false;

        /**********************************************************************************************/
        /*** @brief   The trick mode for play.
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
     * @date    01/10/13
     **************************************************************************************************/

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the channels. Have a event based and a time based recording each in the library.");
        this.AddStep(new Step1(), "Step 1: Launch Action Menu During Playback of Manually Recorded Content");

        //Get Client Platform
        CL = GetClient();

    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Get the channels. Have a event based and a time based recording each in the library.
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Get Channel Values From XML File
			LogCommentInfo(CL,"Get Channel Values From XML File");
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video", "IsEITAvailable=True;ParentalRating=High");

            //Tune to the service whose event will be recorded
			LogCommentInfo(CL,"Tune to the service whose event will be recorded");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded.LCN);
            }

            //Schedule a time based recording
			LogCommentInfo(CL,"Schedule a time based recording");
            res = CL.EA.PVR.RecordManualFromPlanner(manualRecording, serviceToBeRecorded.Name, Constants.daysDelay, Constants.minuteDelayInBeginning, Constants.durationInMinsOfManualRecord, EnumFrequency.ONE_TIME, Constants.verifyInPCAT);           
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule time based recording");
            }

            //Wait for sometime for the recording to happen
			LogCommentInfo(CL,"Wait for sometime for the recording to happen");
            res = CL.IEX.Wait(Constants.secToWaitAfterStartOfRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of record!");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Launch Action Menu During Playback of Manually Recorded Content
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Playback the recording
			LogCommentInfo(CL,"Playback the recording");
            res = CL.EA.PVR.PlaybackRecFromArchive(manualRecording, Constants.valueForFullPlayback, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Playback failed!!");
            }

            //Clear EPG Info
            LogCommentInfo(CL, "Clear EPG Info");
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Clear EPG Info");
            }

            //Launch Action Bar
			LogCommentInfo(CL,"Launch Action Bar");
            res = CL.EA.LaunchActionBar(true);  //true for Action bar of playback
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to ACTION BAR");
            }
            

            //get title of the event
			LogCommentInfo(CL,"get title");
            string title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to GetEPGInfo title");
            }

            LogCommentInfo(CL, "Verifying the channel name is same on which recording was set");
            if(title != serviceToBeRecorded.Name)
            {
                FailStep(CL, res, "Channel name is different on which manual recording was set");
            }

            //Dismiss Action Bar
			LogCommentInfo(CL,"Dismiss Action Bar");            
            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send RETOUR Key To Go Back From Actiom Menu To Full Screen");
            }

            //Stop playback
			LogCommentInfo(CL,"Stop playback");
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
     * @date    01/10/13
     **************************************************************************************************/

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //deleting recording from planner

        LogCommentInfo(CL, "Deleting recording from archieve");
        res = CL.EA.PVR.DeleteRecordFromArchive(manualRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete recording because" + res.FailureReason);
        }
    }
    #endregion
}