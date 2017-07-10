/// <summary>
///  Script Name : ON_MP_ParalellBooking.
///  Test Name   : Overnight Paralell Booking
///  TEST ID     : NA
///  QC Version  : NA
///  Variations From QC: NA
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 08th Jan, 2014
/// </summary>

using System;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using System.Collections;

public class ON_MP_ParalellBooking : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service recordableService;
    private static Service recordableService1;
    private static string powerMode;
    static string defaultMaintenanceDelay = "";
    static string MaintenanceDuration = "";
    static string mountCommand = "";
    static string prompt = "";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Do some Future bookings and wait for the box to go to Standy state");
        this.AddStep(new Step1(), "Step 1: Wait until the Recordings are completed");
        this.AddStep(new Step2(), "Step 2: Wake up the box from Standy State and verify all the recordings");

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

            prompt = CL.EA.UI.Utils.GetValueFromEnvironment("prompt");
            if (prompt == "")
            {
                FailStep(CL,"Failed to get the prompt value from environment ini file");
            }

            mountCommand = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "MountCommand");
            if (string.IsNullOrEmpty(mountCommand))
            {
                FailStep(CL, "Failed to fetch mountCommand from Environment.ini");
            }

            //Get default maintenance delay from project ini
            defaultMaintenanceDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_DEALY");
            if (string.IsNullOrEmpty(defaultMaintenanceDelay))
            {
                FailStep(CL, res, "Unable to fetch the default maintenance delay value from project ini");
            }

            //Get default maintenance duration from project ini
            MaintenanceDuration = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "MAINTENANCE_DURATION");
            if (string.IsNullOrEmpty(MaintenanceDuration))
            {
                FailStep(CL, res, "Unable to fetch the default MaintenanceDuration value from project ini");
            }


            //1st Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED", Convert.ToInt32(recordableService.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 200, DurationInMin: 60,VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService.LCN);
            }
            //2nd Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED1", Convert.ToInt32(recordableService1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 440, DurationInMin: 60, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService1.LCN);
            }
            //3rd Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED2", Convert.ToInt32(recordableService.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 650, DurationInMin: 30, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService.LCN);
            }
            //Navigate to Active Stand by after to 30 min
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTIVATE STANDBY AFTER");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to settings ACTIVATE STANDBY AFTER");
            }
            res = CL.IEX.MilestonesEPG.Navigate("30 min.");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to navigate to 30 min. and select");
            }
            //Get the power mode vale to be set from the test ini
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully to : " + powerMode);
            }
            //Wait until the box goes to Standy 
            res = CL.IEX.Wait(29*60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"failed to wait until few minutes");
            }
            //Before box goes to Stand by we will be getting the Notification message so waiting for that
            bool notificationState = CL.EA.UI.Utils.VerifyState("NOTIFICATION MESSAGE", 120);
            if (notificationState)
            {
                LogCommentInfo(CL, "Notification state verified sucessfully after idle wait time");
            }
            else
            {
                FailStep(CL, res, "Unable to verify the Notification state after wait time");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();
            LogCommentInfo(CL,"Waiting 3 min till the box goes to Stand by");
            res = CL.IEX.Wait(190);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until the box enters stand by");
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
            //Verify the power mode
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode: powerMode, jobPresent: false, isStandBy: false, isWakeUp: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that the power mode is " + powerMode);
            }

            //1st Recording Waiting for the shell prompt which will come when we try to reach hot stand by from other stand by states for the 1st recording
            string ActualLine = "";
            string timeStamp = "";
            res = CL.IEX.Debug.BeginWaitForMessage(prompt, 0, 200*60, debugDevice: IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to start the begin debug message for shell prompt");
            }

            res = CL.IEX.Debug.EndWaitForMessage(prompt, out ActualLine, out timeStamp, debugDevice: IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify the shell prompt");
            }
            else
            {
                //mounting STB
                try
                {
                    CL.EA.UI.Mount.SendMountCommand(true, mountCommand: mountCommand);
                }
                catch (Exception ex)
                {
                    LogCommentInfo(CL, "Failed to mount the box as we are not able to send mount command during mount:" + ex.Message);
                }
            }


            //2nd Recording: Waiting for the shell prompt which will come when we try to reach hot stand by from other stand by states for the 2nd recording
            res = CL.IEX.Debug.BeginWaitForMessage(prompt, 0, 250*60, debugDevice: IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to start the begin debug message for shell prompt");
            }

            res = CL.IEX.Debug.EndWaitForMessage(prompt, out ActualLine, out timeStamp, debugDevice: IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify the shell prompt");
            }
            else
            {
                //mounting STB
                try
                {
                    CL.EA.UI.Mount.SendMountCommand(true, mountCommand: mountCommand);
                }
                catch (Exception ex)
                {
                    LogCommentInfo(CL, "Failed to mount the box as download detected during mount:" + ex.Message);
                }
            }
            //3rd Recording: Waiting for the shell prompt which will come when we try to reach hot stand by from other stand by states for the 3rd recording
            res = CL.IEX.Debug.BeginWaitForMessage(prompt, 0, 230*60, debugDevice: IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to start the begin debug message for shell prompt");
            }

            res = CL.IEX.Debug.EndWaitForMessage(prompt, out ActualLine, out timeStamp, debugDevice: IEXGateway.DebugDevice.Serial);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify the shell prompt");
            }
            else
            {
                //mounting STB
                try
                {
                    CL.EA.UI.Mount.SendMountCommand(true, mountCommand: mountCommand);
                }
                catch (Exception ex)
                {
                    LogCommentInfo(CL, "Failed to mount the box as download detected during mount:" + ex.Message);
                }
            }
            //Wait till the end of the recordings
            //res = CL.IEX.Wait(710*60);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,res,"Failed to wait until the recordings are completed");
            //}
            //Waiting 30 min 
			LogCommentInfo(CL, "Waiting for the Recording to Complete");
            CL.IEX.Wait(40 * 60);

            //wait for maintenance to start
          //  LogCommentWarning(CL, "Wait for maintenance to start after 10 mins in hot standby.");
          //  int defaultMaintenanceDelayInt = Convert.ToInt32(defaultMaintenanceDelay);
         //   defaultMaintenanceDelayInt = defaultMaintenanceDelayInt * 60;

            //Fetch the maintenaceStart milestone from milestones.ini
           // String maintenanceStartMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceStart");

            //Begin wait for maintenaceStart milestone
          //  CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceStartMilestone, defaultMaintenanceDelayInt);

          
          //  bool isMaintenanceMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceStartMilestone, ref arrayList);
          //  if (!isMaintenanceMilestoneRecieved)
          //  {
           //     FailStep(CL, res, "Failed to start maintenance ");
           // }

          //  LogCommentWarning(CL, "Wait for maintenance to completes.");

            //Fetch the maintenaceComplete milestone from milestones.ini
           // String maintenanceCompleteMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceComplete");

          //  int MaintenanceDurationInt = Convert.ToInt32(MaintenanceDuration) * 60;

            //Begin wait for maintenaceComplete milestone
          //  CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceCompleteMilestone, MaintenanceDurationInt);


          //  bool maintenanceCompleteMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceCompleteMilestone, ref arrayList);
         //   if (!maintenanceCompleteMilestoneRecieved)
           // {
           //     FailStep(CL, res, "Failed to complete the maintenance");
          //  }

            //Wait for one hour after the Maintenance phase is completed
			
            LogCommentInfo(CL, "Waiting for one hour after the Recording is Completed");
            CL.IEX.Wait(180*60);

            //try to come out of Stand by which will take us to shell prompt then mount and bring up the box
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
			    //Exit Stand By Milestone will be begin in the Ea so stopping it here
				ArrayList arrayList = new ArrayList();
			    String exitStandByMilestone = CL.EA.UI.Utils.GetValueFromMilestones("ExitStandby");
                CL.EA.UI.Utils.EndWaitForDebugMessages(exitStandByMilestone, ref arrayList);
                CL.IEX.Wait(180);
                Boolean standbyAfterBoot = Boolean.Parse(CL.EA.UI.Utils.GetValueFromProject("BOOTUP", "STANDBY_AFTER_REBOOT"));
                Boolean isHomeNetwork = Boolean.Parse(CL.EA.UI.Utils.GetValueFromProject("BOOTUP", "IsHomeNetwork"));
                Int32 imageLoadDelay = Int32.Parse(CL.EA.UI.Utils.GetValueFromProject("BOOTUP", "IMAGE_LOAD_DELAY_SEC"));
                //if the box supports home network mount client
                if (isHomeNetwork)
                {
                    CL.EA.MountClient(EnumMountAs.NOFORMAT);
                }
                //if the box does not support home network mount only gateway
                else
                {
                    CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);

                    //Wait for some time for STB to come to standby mode 
                    CL.IEX.Wait(imageLoadDelay);
                }
                //Verify state LIVE
                CL.EA.UI.Utils.VerifyState("LIVE", imageLoadDelay);
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

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            //Playback the 1st Recording
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            //Set the Trick mode speed to 30
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 6");
            }
            CL.IEX.Wait(60);
            //Stop the Playback of the Recording
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to stop the Playback of the Recording");
            }
            CL.IEX.Wait(10);
            //Playback the 2nd recording
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED1", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            //Set the Trick mode speed to 30
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED1", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 6");
            }
            CL.IEX.Wait(60);
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop the Playback of the Recording");
            }
            CL.IEX.Wait(10);
            //Playback the 3rd Recording
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED2", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            //Set the trick mode speed to 30
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED2", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 6");
            }
            CL.IEX.Wait(60);
            //Stop the Playback of the Recording
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop the Playback of the Recording");
            }
            CL.IEX.Wait(10);
			
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf in live");
            }
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