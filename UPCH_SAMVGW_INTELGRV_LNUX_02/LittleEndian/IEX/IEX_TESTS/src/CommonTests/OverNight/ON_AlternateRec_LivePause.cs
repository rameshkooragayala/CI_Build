/// <summary>
///  Script Name : ON_AlternateRec_LivePause.
///  Test Name   : ON_AlternateRec_LivePause
///  TEST ID     : NA
///  QC Version  : NA
///  Variations From QC: NA
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 11th Feb, 2014
/// </summary>

using System;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using System.Collections;
using System.Globalization;

public class ON_AlternateRec_LivePause : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service recordableService;
    private static Service recordableService1;
    private static Service recordableService2;
    private static Service recordableService3;

    private static string RB_SIZE;
    private static string Milestone = "";
    private static double rbDepthInMin;
    //Constants Class
    private static class Constants
    {
        public const int waitForMilestones = 30;
    }
    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Do 4 parallel Recordings (2 EB and 2 TB) and wait in Service s5 for more then 7 hours");
        this.AddStep(new Step1(), "Step 1: Verify and Playback the Recordings");
        this.AddStep(new Step2(), "Step 2: Bring up the Box and verify the Recordings by Playing Back");

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

            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService.LCN);
            }
            recordableService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService.LCN);
            if (recordableService1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService1.LCN);
            }
            recordableService2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + recordableService.LCN + "," + recordableService1.LCN);
            if (recordableService2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + recordableService2.LCN);
            }
            recordableService3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + recordableService.LCN + "," + recordableService1.LCN + "," + recordableService2.LCN);
            if (recordableService3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + recordableService3.LCN);
            }

            RB_SIZE = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "SIZE");

            Milestone = CL.EA.UI.Utils.GetValueFromMilestones("GetReviewBufferCurrentDepth");
			
            //Setting the Auto Stand by
            res = CL.EA.STBSettings.SetAutoStandBy("OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Auto Stand by to OFF");
            }
			
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED", Convert.ToInt32(recordableService.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 15, DurationInMin: 60, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to record manual from Planner");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED1", Convert.ToInt32(recordableService1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 17, DurationInMin: 60, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to record manual from Planner");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED", recordableService2.LCN, NumberOfPresses: 3, VerifyBookingInPCAT: false,ReturnToLive:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to book future event from Guide");
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED1", recordableService2.LCN, NumberOfPresses: 1, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to book future event from Guide");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+recordableService3.LCN);
            }
            res = CL.EA.FlushRB();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to flush the RB");
            }
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to set the Trock mode speed to 0");
            }
            res = CL.IEX.Wait(7*60*60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait few hours");
            }
			
			string timeStamp="";
			
			CL.IEX.SendIRCommand("MENU", 1,ref timeStamp);
			
			CL.IEX.Wait(5);
			
            CL.IEX.SendIRCommand("RETOUR", 1, ref timeStamp);

			CL.IEX.Wait(25);
			
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
            //Verify FAS message for RB size
            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to rewind into RB");
            }

            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            CL.IEX.LogComment("obtained Milestone String :" + ActualLines[0].ToString());

            res = CL.EA.GetRBDepthInSec(ActualLines[0].ToString(), ref rbDepthInMin);
            CL.IEX.LogComment("RB Depth is = " + rbDepthInMin);

            if ((((Convert.ToDouble(RB_SIZE)-1) * 60)>Convert.ToDouble(rbDepthInMin))|| ((Convert.ToDouble(RB_SIZE)+1) * 60)<(Convert.ToDouble(rbDepthInMin)))
                FailStep(CL, res, "Failed: The Review Buffer Depth Is Bigger Than Max Depth");

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 30, Verify_EOF_BOF: true, IsReviewBufferFull: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to verify the EOF and BOF in the RB");
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
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to set the trick mode speed to 30");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to stop the Playback");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED1", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED1", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to 30");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to stop the Playback");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.PlaybackRecFromArchive("EVENT_BASED", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("EVENT_BASED", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to 30");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to stop the Playback");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.PlaybackRecFromArchive("EVENT_BASED1", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("EVENT_BASED1", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to 30");
            }
            CL.IEX.Wait(20);
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to stop the Playback");
            }
			
			  CL.IEX.Wait(10);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", NumberOfPresses: 1, DoTune: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf in live");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", NumberOfPresses: 1, DoTune: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf in live");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Guide");
            }
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Banner Display Time out to 10");
            }
			
            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}