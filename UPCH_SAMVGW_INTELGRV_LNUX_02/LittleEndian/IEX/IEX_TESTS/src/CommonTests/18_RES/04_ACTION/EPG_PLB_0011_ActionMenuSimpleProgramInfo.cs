/// <summary>
///  Script Name : EPG_PLB_0011_ActionMenuSimpleProgramInfo.cs
///  Test Name   : EPG-PLB-0011-Actionmenu - simple program info - info not available
///               ,EPG-PLB-3101-Simple information playback banner
///  TEST ID     : 17915,26498
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Madhu Renukaradhya
///  Deviations from HPQC :step 1 and 2 of EPG-PLB-3101 covered in script EPG_PLB_3810_ActionmenuInfo_Playback.cs

/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;


[Test("EPG_PLB_0011")]
public class EPG_PLB_0011 : _Test
{

    [ThreadStatic]

        static _Platform CL;
        static Service serviceToBeManualRecorded;
        static Service serviceToBeEventRecorded;
        static string manualRecording = "MANUAL_RECORDING";

    
 static class Constants
    {
       
        public const int daysDelay = -1;

        public const int minuteDelayInBeginning = 1;

        public const int durationInMinsOfManualRecord = 2;

        public const int secToPlay = 0;

        public const bool playFromBeginning = true;

        public const bool verifyEOF = false;

        public const bool verifyInPCAT = true;
        
    }
    
    #region Create Structure

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

    private class PreCondition : _Step
    {


        public override void Execute()
        {
            StartStep();

            //Get Channel Values From xml File
			LogCommentInfo(CL,"Get Channel Values From xml File");
            serviceToBeManualRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=False;IsRecordable=True","ParentalRating=High");
            if (serviceToBeManualRecorded == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }

            serviceToBeEventRecorded = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;IsRecordable=True", "ParentalRating=High");
            if (serviceToBeEventRecorded == null)
            {
                FailStep(CL, "Failed to get serviceToBeEventRecorded from ContentXML");
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
            res = CL.IEX.Wait((Constants.durationInMinsOfManualRecord + Constants.minuteDelayInBeginning +2) * 60);
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

    private class Step1 : _Step
    {

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

    private class Step2 : _Step
    {

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
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to GetEPGInfo title");
            }

            if (title != serviceToBeManualRecorded.Name)
            {
                FailStep(CL, res, "Event name, Fetched during Playback of Time Based Recording:" + title + "not matching Channel name" + serviceToBeManualRecorded.Name);
            }

            //get duration
            LogCommentInfo(CL, "get Duration during Playback");
            string duration = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("duration", out duration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Duration");
            }
            duration = duration.Split(' ')[0];
            string manualRecordDuration = Convert.ToString(Constants.durationInMinsOfManualRecord);
            if (!(duration.Contains(manualRecordDuration)))
            {
                FailStep(CL, res, "duration, Fetched during Playback of Time Based Recording:" + duration + "not matching the duration defined " + manualRecordDuration);
            }
          
		    res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait");
            }
            //get Progressbar time
            LogCommentInfo(CL, "get Progressbar during Playback");
            string progressTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("ProgressBarTime", out progressTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get ProgressBarTime");
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


    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //deleting recording from archieve

        res = CL.EA.PVR.DeleteRecordFromArchive(manualRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to Delete recording because");
        }
    }
    #endregion
}