/// <summary>
///  Script Name : PLB_0100_StartPosition_Options.cs
///  Test Name   : PLB_0100_StartPosition_beginning, PLB_0100_StartPosition_LastPosition, 
///                PLB_0100_StartPosition_LastPosition_EndOfRecording, PLB_0100_StartPosition_LastPosition_StoppedByPowerCut
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Anshul Upadhyay
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("PLB_0100_StartPosition_PlaybackOptions")]
public class PLB_0100 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Service to be recorded
    static Service serviceToBeRecorded;

    //future event recording
    static string eventToRecord = "EVENT_RECORDING";

    static class Constants
    {
        public const bool playFromBeginning = true;

        public const bool playFromLastPosition = false;

        public const int secToPlay = 0;
        
        public const int minTimeBeforeEventEnd = 3;

        public const int eventRecordTime = 90;

        public const bool exitStandBy = true;
        
        public const int timeToPressKey = -1;

    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From content XML File and record an event";
    private const string STEP1_DESCRIPTION = "Step 1: Playback the recorded event and reboot STB in middle of playback";
    private const string STEP2_DESCRIPTION = "Step 2: Verify that Last Viewed Position or Start From Beginning option is not displayed";
    private const string STEP3_DESCRIPTION = "Step 3: Playback from beginning";
    private const string STEP4_DESCRIPTION = "Step 4: Stop the playback event, Verify the playback options";
    private const string STEP5_DESCRIPTION = "Step 5: Playback the event from Beginning";
    private const string STEP6_DESCRIPTION = "Step 6: Stop the playback before end of playback and verify the playback options";
    private const string STEP7_DESCRIPTION = "Step 7: Playback the event from the last viewed position until end of playback";
    private const string STEP8_DESCRIPTION = "Step 8: No option to playback from last viewed position";
    private const string STEP9_DESCRIPTION = "Step 9: Playback the event from beginning";

    #region Create Structure
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
        this.AddStep(new Step9(), STEP9_DESCRIPTION);
        
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function

    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            
            StartStep();
            
            //Get channels from content.XML file
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (serviceToBeRecorded == null)
            {
                FailStep(CL, res, "Failed to get service from content XML");
            }

            //Tune to the service
            LogCommentInfo(CL, "Tuning to channel where event has to be recorded");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to the service where event has to be recorded");
            }

            LogCommentInfo(CL, "Book a current event for recording");
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToRecord, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book current event from banner on channel " + serviceToBeRecorded.LCN);
            }

            LogCommentInfo(CL, "Wait for even to record for some time");
            res = CL.IEX.Wait(Constants.eventRecordTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait to record event");
            }

            LogCommentInfo(CL, "Stop recording");
            res = CL.EA.PVR.StopRecordingFromBanner(eventToRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to stop recording from Banner");
            }
            
            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Verifying script PLB_0100_StartPosition_LastPosition_StoppedByPowerCut

            //Playback the event from My recordings
            LogComment(CL, "Playback event from My recordings");
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToRecord, Constants.secToPlay, Constants.playFromBeginning, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback event from My recordings");
            }

            //reboot the STB while event is playing back
            string imageLoadDelay;
            int delaytime;
            string standbyAfterBoot;

            standbyAfterBoot = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "STANDBY_AFTER_REBOOT");

            imageLoadDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "IMAGE_LOAD_DELAY_SEC");
            if (imageLoadDelay == null)
            {
                FailStep(CL, res, "Failed to load image load delay time from Project INI file");
            }

            int.TryParse(imageLoadDelay, out delaytime);    //converting the image load time to int for Wait
            
            LogCommentInfo(CL, "mounting the image to STB");
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT);            
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to power cycle STB to live");
            }
            
            //Wait for some time for STB to come to standby mode
            res = CL.IEX.Wait(delaytime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for image to load ");
            }

            //Checking if STB enters standby after reboot. For few projects STB goes to LIVE. This flag STANDBY_AFTER_REBOOT should be in Project.ini
            if (Convert.ToBoolean(standbyAfterBoot))
            {
                //Navigate out of standby
                res = CL.EA.StandBy(Constants.exitStandBy);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit out of standby");
                }
            }
            else
            {
                res = CL.EA.ReturnToLiveViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "STB is not in LIVE mode");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.VerifyEventInArchive(eventToRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event in my recordings");
            }
            
            LogCommentInfo(CL, "Navigate to Action bar of recorded event");
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Action Bar");
            }
           
            LogCommentInfo(CL, "Verify Play from Beginning is not listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM BEGINNING");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from beginning or Restart option is displayed in the Action bar of Recorded event");
            }

            LogCommentInfo(CL, "Verify Play from last viewed position is not listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM LAST VIEWED");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from last viewed position or Resume option is displayed in the Action bar of Recorded event");
            }
            
            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            LogCommentInfo(CL, "Verify and select Play option to play from beginning");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Play options is not highlighted");
            }

            LogCommentInfo(CL, "Playback the event from beginning");
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToRecord, Constants.secToPlay, Constants.playFromBeginning, false); //false will not verify end of playback
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Fail to playback event from beginning");
            }
            
            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            //PLB_0100_StartPosition_beginning

            LogCommentInfo(CL, "Stopping the playback");
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            res = CL.EA.PVR.VerifyEventInArchive(eventToRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event in my recordings");
            }

            LogCommentInfo(CL, "Navigate to Action bar of recorded event");
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Action Bar");
            }
            
            //Verifying the playback options
            LogCommentInfo(CL, "Verify Play option is not listed in action bar");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Play options is not selected");
            }

            LogCommentInfo(CL, "Verify Play from Beginning or Restart is listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM BEGINNING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from beginning or Restart option is not displayed in the Action bar of Recorded event");
            }

            LogCommentInfo(CL, "Verify Play from last viewed position or Resume is listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM LAST VIEWED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from last viewed position or Resume option is not displayed in the Action bar of Recorded event");
            }
            
            PassStep();
        }
    }
    #endregion
    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            LogCommentInfo(CL, "Playback the event from beginning");
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToRecord, Constants.secToPlay, Constants.playFromBeginning, false); //false will not verify end of playback
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Fail to playback event from beginning");
            }

            PassStep();
        }
    }
    #endregion
    #region Step6
    [Step(6, STEP6_DESCRIPTION)]
    public class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //PLB_0100_StartPosition_LastPosition

            LogCommentInfo(CL, "Stopping the playback");
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            res = CL.EA.PVR.VerifyEventInArchive(eventToRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event in my recordings");
            }

            LogCommentInfo(CL, "Navigate to Action bar of recorded event");
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Action Bar");
            }

            //Verifying the playback options
            LogCommentInfo(CL, "Verify Play option is not listed in action bar");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "PLAY options is listed in action bar when event is stopped in middle of playback");
            }

            LogCommentInfo(CL, "Verify Play from Beginning or Restart is listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM BEGINNING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from beginning or Restart option is not displayed in the Action bar of Recorded event");
            } 
            
            LogCommentInfo(CL, "Verify Play from last viewed position or Resume is listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM LAST VIEWED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from last viewed position or Resume option is not displayed in the Action bar of Recorded event");
            }
            
            PassStep();
        }
    }
    #endregion
    #region Step7
    [Step(7, STEP7_DESCRIPTION)]
    public class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();
                        
            LogCommentInfo(CL, "Playback the event from last viewed position until end of playback");
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToRecord, Constants.secToPlay, Constants.playFromLastPosition, true); //True will playback until end of file
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the event until end of playback");
            }

            PassStep();
        }
    }
    #endregion
    #region Step8
    [Step(8, STEP8_DESCRIPTION)]
    public class Step8 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //PLB_0100_StartPosition_LastPosition_EndOfRecording

            res = CL.EA.PVR.VerifyEventInArchive(eventToRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event in my recordings");
            }

            LogCommentInfo(CL, "Navigate to Action bar of recorded event");
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Action Bar");
            }

            LogCommentInfo(CL, "Verify Play from Beginning is not listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM BEGINNING");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from beginning or Restart option is displayed in the Action bar of Recorded event");
            }

            LogCommentInfo(CL, "Verify Play from last viewed position is not listed");
            res = CL.IEX.MilestonesEPG.SelectMenuItem("PLAY FROM LAST VIEWED");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Play from last viewed position or Resume option is displayed in the Action bar of Recorded event");
            }

            PassStep();
        }
    }
    #endregion
    #region Step9
    [Step(9, STEP9_DESCRIPTION)]
    public class Step9 : _Step
    {
        public override void Execute()
        {
            StartStep();

            LogCommentInfo(CL, "Playback the event from beginning");
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToRecord, Constants.secToPlay, Constants.playFromBeginning, false); //false will not verify end of playback
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the event until end of playback");
            }

            LogCommentInfo(CL, "Stopping the playback");
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

        //Delete all events from Archieve
        IEXGateway._IEXResult res;
        LogCommentInfo(CL, "Delete the recordings");
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to Delete recording because" + res.FailureReason);
        }

    }
    #endregion
}