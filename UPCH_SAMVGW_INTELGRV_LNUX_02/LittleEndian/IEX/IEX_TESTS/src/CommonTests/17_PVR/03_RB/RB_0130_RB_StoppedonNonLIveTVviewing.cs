/// <summary>
///  Script Name : RB_0130_RB_InitializationAutoStart.cs
///  Test Name   : RB-0130-Review Buffer initialization auto start
///  TEST ID     : 16024
///  JIRA ID     : 
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.ElementaryActions.FunctionalityCS;

public class RB_0130 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static Service clearChannel1;
    private static Service clearChannel2;
    private static string rbDepth;
    private static double rbDepthInMin;
    private static double rbDepthInMinWhilePlbSample1;
    private static double rbDepthInMinWhilePlbSample2;
    private static double rbDepthInLive;
    private static double rbDepthInLiveBeforePlb;
    private static string timeStamp;
    private static string timeStampMarginLine="";
    private static string Milestone = "";
    
    private static class Constants
    {
        public const int timeInStby = 10;
        public const int waitInLive = 90;
        public const int minTimeForEventToEnd = 5;
        public const int recordingDuration = 120;
        public const int speedForPause = 0;
        public const int speedForPlay = 1;
        public const int timeInPlb = 60;
        public const int waitForMilestones = 30;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync & record an event");
        this.AddStep(new Step1(), "Step 1: Clean RB by entering stby. Wakeup the box and wait for the RB to start again");
        this.AddStep(new Step2(), "Step 2: Playback the recording and caluclating RB depth while playback & stop PLayback");
        this.AddStep(new Step3(), "Step 3: Retrun to live and chek if the live content is appended to previous RB");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Service information Values From  Content XML File
            clearChannel1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;Type=Radio");
            if (clearChannel1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            clearChannel2 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + clearChannel1.LCN);
            if (clearChannel2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            Milestone = CL.EA.UI.Utils.GetValueFromMilestones("GetReviewBufferCurrentDepth");
  
            //tuning to a clear channel 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel1.LCN);
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            // Cleaning RB . Stby Wakeup
            res = CL.EA.FlushRB();

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to standy and wakeup the STB");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel2.LCN);
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventToBeRecorded", 5, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
            }

            CL.IEX.Wait(Constants.recordingDuration);
            CL.IEX.Wait(60);

            res = CL.EA.PVR.StopRecordingFromBanner("EventToBeRecorded");

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop a ongoing recording ");
            }

            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones );
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }
            
            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            CL.IEX.LogComment("obtained Milestone String :" + ActualLines[0].ToString());
            res = CL.EA.GetRBDepthInSec(ActualLines[0].ToString(), ref rbDepthInMin);
            rbDepthInLiveBeforePlb = rbDepthInMin;
            CL.IEX.LogComment("RB Depth = " + rbDepthInLiveBeforePlb.ToString());

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Playback the Already Recorded Event from Archive
            res = CL.EA.PVR.PlaybackRecFromArchive("EventToBeRecorded", 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The Event to EOF ");
            }

            //waiting for the reviw buffer depth milestones while recording playback 
            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }
            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            CL.IEX.LogComment("obtained Milestone String :" + Milestone);
            res = CL.EA.GetRBDepthInSec(Milestone, ref rbDepthInMin);
            rbDepthInMinWhilePlbSample1 = rbDepthInMin;
            CL.IEX.LogComment("RB Depth = " + rbDepthInMinWhilePlbSample1.ToString());

            res = CL.EA.PVR.SetTrickModeSpeed("EventToBeRecorded", Constants.speedForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            //PLayback the recording for a while 
            CL.IEX.Wait(Constants.timeInPlb);

            verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }
            
            endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.GetRBDepthInSec(timeStampMarginLine, ref rbDepthInMin);
            rbDepthInMinWhilePlbSample2 = rbDepthInMin;
            CL.IEX.LogComment("RB Depth = " + rbDepthInMinWhilePlbSample2.ToString());

            res = CL.EA.PVR.SetTrickModeSpeed("EventToBeRecorded", Constants.speedForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            
            if (rbDepthInMinWhilePlbSample1 != rbDepthInMinWhilePlbSample2  )
            {
                FailStep(CL, res, "Failed: Review Buffer Depth size is growing while recoring playback" );
            }

            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording playback");
            }
            
            //tuning to a different channel
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after Playback from Last Viewed Position ");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //waiting for the reviw buffer depth milestones
            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }
            
            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.GetRBDepthInSec(timeStampMarginLine, ref rbDepthInMin);
            rbDepthInLive = rbDepthInMin;
            CL.IEX.LogComment("RB Depth = " + rbDepthInLive.ToString());

            if (rbDepthInLive < rbDepthInLiveBeforePlb)
            {
                FailStep(CL, res, "Review Buffer is Appended to the previous RB" );
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {

      IEXGateway._IEXResult res;
      res = CL.EA.PVR.StopPlayback(true);
      if (!res.CommandSucceeded)
      {
            CL.IEX.LogComment("Failed to Return to Live Viewing From RB");
      }

      res = CL.EA.PVR.DeleteRecordFromArchive("EventToBeRecorded");
      if (!res.CommandSucceeded)
      {
           CL.IEX.LogComment("Failed to Return to Live Viewing From RB");
      }

    }

    #endregion PostExecute
}