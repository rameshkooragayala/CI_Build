/// <summary>
///  Script Name : ON_ParalellBooking_EventBased.
///  Test Name   : ON_ParalellBooking_EventBased
///  TEST ID     : NA
///  QC Version  : NA
///  Variations From QC: NA
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 023rd Jan, 2014
/// </summary>

using System;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

public class ON_ParalellBooking_EventBased : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service recordableService;
    private static Service recordableService1;
    private static Service recordableService2;
    private static Service recordableService3;
    //static string eventBased11="EVENT_BASED11";
    //static string eventBased12="EVENT_BASED12";
    //static string eventBased13="EVENT_BASED13";
    //static string eventBased14="EVENT_BASED14";
    //static string eventBased15="EVENT_BASED15";
    //static string eventBased16="EVENT_BASED16";
    //static string eventBased21="EVENT_BASED21";
    //static string eventBased22="EVENT_BASED22";
    //static string eventBased23="EVENT_BASED23";
    //static string eventBased24="EVENT_BASED24";
    //static string eventBased25="EVENT_BASED25";
    //static string eventBased26="EVENT_BASED26";
    //static string eventBased31="EVENT_BASED31";
    //static string eventBased32="EVENT_BASED32";
    //static string eventBased33="EVENT_BASED33";
    //static string eventBased34="EVENT_BASED34";
    //static string eventBased35="EVENT_BASED35";
    //static string eventBased36="EVENT_BASED36";
    //static string eventBased41="EVENT_BASED41";
    //static string eventBased42="EVENT_BASED42";
    //static string eventBased43="EVENT_BASED43";
    //static string eventBased44="EVENT_BASED44";
    //static string eventBased45="EVENT_BASED45";
    //static string eventBased46="EVENT_BASED46";
    private static string powerMode;
    static string mountCommand = "";
    static string prompt = "";
    static string defaultMaintenanceDelay = "";
    static string MaintenanceDuration = "";
    static string defaultSGT = "";
    static string defaultEGT = "";
    static Helper helper = new Helper();
    private static double timeToWait = 0;//Total time to wait in Stand By


    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Do some Event Based recordings and keep the box in Stand By state");
        this.AddStep(new Step1(), "Step 1: Wait in the Standby State till the recordings were completed and verify MP");
        this.AddStep(new Step2(), "Step 2: Wait for few hours in Stand by & bring up the box, Playback and Verify the Recordings with Trick mode");

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
            defaultSGT = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFAULT_SGT");
            if (defaultSGT == "")
            {
                FailStep(CL,"Default SGT is not mentioned in the test ini");
            }
            defaultEGT = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFAULT_EGT");
            if (defaultEGT == "")
            {
                FailStep(CL, "Default EGT is not mentioned in the test ini");
            }

            prompt = CL.EA.UI.Utils.GetValueFromEnvironment("prompt");
            if (prompt == "")
            {
                FailStep(CL, "Failed to get the prompt value from environment ini file");
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

            //Navigate to Active Stand by after to 30 min
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTIVATE STANDBY AFTER");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to settings ACTIVATE STANDBY AFTER");
            }
            res = CL.IEX.MilestonesEPG.Navigate("30 min.");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to 30 min. and select");
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
           
            for (int i = 1; i <= 6; i++)
            {
                if (i == 1)
                {
                    res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED1" + i, recordableService.LCN, NumberOfPresses: 6, VerifyBookingInPCAT: false, ReturnToLive: false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to book future event from Guide");
                    }
                }
                else
                {
                    res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED1" + i, recordableService.LCN, NumberOfPresses: 1, VerifyBookingInPCAT: false, ReturnToLive: false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to book future event from Guide");
                    }
                }
                //Getting Event end time from the event collection

                string evtStartTime1 = CL.EA.GetEventInfo("EVENT_BASED1" + i, EnumEventInfo.EventStartTime);
                if (string.IsNullOrEmpty(evtStartTime1))
                {
                    FailStep(CL, "Retrieved Start time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event Start time is " + evtStartTime1);
                }

                //Getting Event start time from the event collection

                string evtEndTime1 = CL.EA.GetEventInfo("EVENT_BASED1" + i, EnumEventInfo.EventEndTime);
                if (string.IsNullOrEmpty(evtEndTime1))
                {
                    FailStep(CL, "Retrieved end time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event End time is " + evtEndTime1);
                }
                res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED2" + i, recordableService1.LCN, NumberOfPresses: 0, VerifyBookingInPCAT: false, ReturnToLive: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book future event from Guide");
                }
                //Getting Event end time from the event collection

                string evtStartTime2 = CL.EA.GetEventInfo("EVENT_BASED2" + i, EnumEventInfo.EventStartTime);
                if (string.IsNullOrEmpty(evtStartTime2))
                {
                    FailStep(CL, "Retrieved Start time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event Start time is " + evtStartTime2);
                }

                //Getting Event start time from the event collection

                string evtEndTime2 = CL.EA.GetEventInfo("EVENT_BASED2" + i, EnumEventInfo.EventEndTime);
                if (string.IsNullOrEmpty(evtEndTime2))
                {
                    FailStep(CL, "Retrieved end time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event End time is " + evtEndTime2);
                }
                res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED3" + i, recordableService2.LCN, NumberOfPresses: 0, VerifyBookingInPCAT: false, ReturnToLive: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book future event from Guide");
                }
                //Getting Event end time from the event collection

                string evtStartTime3 = CL.EA.GetEventInfo("EVENT_BASED3" + i, EnumEventInfo.EventStartTime);
                if (string.IsNullOrEmpty(evtStartTime3))
                {
                    FailStep(CL, "Retrieved Start time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event Start time is " + evtStartTime3);
                }

                //Getting Event start time from the event collection

                string evtEndTime3 = CL.EA.GetEventInfo("EVENT_BASED3" + i, EnumEventInfo.EventEndTime);
                if (string.IsNullOrEmpty(evtEndTime3))
                {
                    FailStep(CL, "Retrieved end time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event End time is " + evtEndTime3);
                }

                //Booking the Fourth Recording
                //Booking Future Event from Guide
                res = CL.EA.PVR.BookFutureEventFromGuide("EVENT_BASED4" + i, recordableService3.LCN, NumberOfPresses: 0, VerifyBookingInPCAT: false, ReturnToLive: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book future event from Guide");
                }
                //Getting Event end time from the event collection

                string evtStartTime4 = CL.EA.GetEventInfo("EVENT_BASED4" + i, EnumEventInfo.EventStartTime);
                if (string.IsNullOrEmpty(evtStartTime4))
                {
                    FailStep(CL, "Retrieved Start time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event Start time is " + evtStartTime4);
                }

                //Getting Event start time from the event collection

                string evtEndTime4 = CL.EA.GetEventInfo("EVENT_BASED4" + i, EnumEventInfo.EventEndTime);
                if (string.IsNullOrEmpty(evtEndTime4))
                {
                    FailStep(CL, "Retrieved end time from event info is null");
                }
                else
                {
                    LogComment(CL, "Event End time is " + evtEndTime4);
                }
                if ((evtStartTime1 == evtStartTime2) && (evtStartTime1 == evtStartTime3) && (evtStartTime1 == evtStartTime4))
                {
                    LogCommentImportant(CL, "Event start times of all the recordings are same " + evtStartTime1);
                }
                else
                {
                    FailStep(CL, "Event start times of all the recordings are not same ");
                }
                if ((evtEndTime1 == evtEndTime2) && (evtEndTime1 == evtEndTime3) && (evtEndTime1 == evtEndTime4))
                {
                    LogCommentImportant(CL, "Event start times of all the recordings are same " + evtEndTime1);
                }
                else
                {
                    FailStep(CL, "Event start times of all the recordings are not same ");
                }
            }

            //res = CL.EA.StandBy(false);
            //if (!res.CommandSucceeded)
            //{
            //    LogCommentFailure(CL,"Failed to keep the box into Standy By");
            //}
            //Wait until the box goes to Standy 
            res = CL.IEX.Wait(29 * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to wait until few minutes");
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
            LogCommentInfo(CL, "Waiting 3 min till the box goes to Stand by");
            res = CL.IEX.Wait(190);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until the box enters stand by");
            }
            //Calculating the time to wait for all the recordings to complete
            string lastevtEndTime = CL.EA.GetEventInfo("EVENT_BASED46", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(lastevtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            else
            {
                LogComment(CL, "Event End time is " + lastevtEndTime);
            }
            string currentTime = "";
            //Get Current EPG Time
            CL.EA.UI.Live.GetEpgTime(ref currentTime);
            if (string.IsNullOrEmpty(currentTime))
            {
                FailStep(CL, "Failed to Get the EPG time from LIVE");
            }

            LogComment(CL, "Current time is " + currentTime);
            //calculating time to wait in Stand By as we cant use Wait until Event Ends in Stand By including EGT
            timeToWait = ((Convert.ToDateTime(lastevtEndTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds);
            LogComment(CL, "Time to wait till all the recordings were completed is " + timeToWait);
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
            LogCommentImportant(CL, "Waiting for all the recordings to be completed");
            CL.IEX.Wait(timeToWait);

            //After more then one hour wait in standby before doing Maintenanace phase the box will do Reboot and will come to shell prompt
            string ActualLine = "";
            string timeStamp = "";
            res = CL.IEX.Debug.BeginWaitForMessage(prompt, 0, 60 * 60, debugDevice: IEXGateway.DebugDevice.Serial);
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

            LogCommentWarning(CL, "Wait for maintenance to start after 10 mins in hot standby.");
            int defaultMaintenanceDelayInt = Convert.ToInt32(defaultMaintenanceDelay);
            defaultMaintenanceDelayInt = (defaultMaintenanceDelayInt * 60) + (Convert.ToInt32(defaultEGT) * 60);

            //Fetch the maintenaceStart milestone from milestones.ini
            String maintenanceStartMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceStart");

            //Begin wait for maintenaceStart milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceStartMilestone, defaultMaintenanceDelayInt);
            ArrayList arrayList = new ArrayList();

            bool isMaintenanceMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceStartMilestone, ref arrayList);
            if (!isMaintenanceMilestoneRecieved)
            {
                FailStep(CL, res, "Failed to start maintenance ");
            }

            LogCommentWarning(CL, "Wait for maintenance to completes.");

            //Fetch the maintenaceComplete milestone from milestones.ini
            String maintenanceCompleteMilestone = CL.EA.UI.Utils.GetValueFromMilestones("MaintenanceComplete");

            int MaintenanceDurationInt = Convert.ToInt32(MaintenanceDuration) * 60;

            //Begin wait for maintenaceComplete milestone
            CL.EA.UI.Utils.BeginWaitForDebugMessages(maintenanceCompleteMilestone, MaintenanceDurationInt);


            bool maintenanceCompleteMilestoneRecieved = CL.EA.UI.Utils.EndWaitForDebugMessages(maintenanceCompleteMilestone, ref arrayList);
            if (!maintenanceCompleteMilestoneRecieved)
            {
                FailStep(CL, res, "Failed to complete the maintenance");
            }

            LogCommentImportant(CL, "Waiting for Six hours in Stand by after the recordinga are completed");
            CL.IEX.Wait(6 * 60 * 60);
            LogCommentImportant(CL, "Bring up the box");
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                //Exit Stand By Milestone will be begin in the Ea so stopping it here
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
            CL.IEX.Wait(10);

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

          
            //Looping through all the recordings and verifying the Duration and Playing back
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    //Getting Event end time from the event collection

                    string evtStartTime = CL.EA.GetEventInfo("EVENT_BASED" + i + j, EnumEventInfo.EventStartTime);
                    if (string.IsNullOrEmpty(evtStartTime))
                    {
                        FailStep(CL, "Retrieved Start time from event info is null");
                    }
                    else
                    {
                        LogComment(CL, "Event Start time is " + evtStartTime);
                    }
                    string evtStartTimeSGT = Convert.ToDateTime(evtStartTime).AddMinutes(-5).ToString("HH:mm");
                    //Getting Event start time from the event collection

                    string evtEndTime = CL.EA.GetEventInfo("EVENT_BASED" + i + j, EnumEventInfo.EventEndTime);
                    if (string.IsNullOrEmpty(evtEndTime))
                    {
                        FailStep(CL, "Retrieved end time from event info is null");
                    }
                    else
                    {
                        LogComment(CL, "Event End time is " + evtEndTime);
                    }
                    string evtDate = CL.EA.GetEventInfo("EVENT_BASED" + i + j, EnumEventInfo.EventConvertedDate);
                    if (string.IsNullOrEmpty(evtDate))
                    {
                        FailStep(CL, "Retrieved evtDate from event info is null");
                    }
                    else
                    {
                        LogComment(CL, "Event Date is " + evtDate);
                    }
                    //string EPGDateFormat = CL.EA.UI.Utils.GetEpgDateFormat();
                    //LogCommentInfo(CL, "Expected Date fetched from the event collection is " + evtDate);
                    //string eventDictionaryDateFormat = CL.EA.UI.Utils.GetValueFromProject("EPG", "DATE_FORMAT_FOR_EVENT_DIC");
                    //string EPGEventdate = DateTime.ParseExact(evtDate, eventDictionaryDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);
                    //LogCommentImportant(CL, "Event Date after Parsing is" + EPGEventdate);

                    CL.IEX.Wait(10);
                    LogCommentImportant(CL, "Verifying the Details of the recording: EVENT_BASED" + i + j);
                    CL.IEX.MilestonesEPG.ClearEPGInfo();
                    res = CL.EA.PVR.VerifyEventInArchive("EVENT_BASED" + i + j, StartTime: evtStartTimeSGT, EndTime: evtEndTime, EvDate: evtDate);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to verify the Event in Archive");
                    }
                    if (!helper.VerifyDuration())
                    {
                        LogCommentFailure(CL, "Failed to verify the Duration of the recording");
                    }
                    res = CL.EA.PVR.PlaybackRecFromArchive("EVENT_BASED" + i + j, SecToPlay: 0, FromBeginning: true, VerifyEOF: false, StartTime: evtStartTimeSGT, EndTime: evtEndTime, EvDate: evtDate);
                    if (!res.CommandSucceeded)
                    {
                        LogCommentFailure(CL, "Failed to playback record from Archive");
                    }
                    CL.IEX.Wait(10);
                    //Set the Trick mode speed to 30
                    res = CL.EA.PVR.SetTrickModeSpeed("EVENT_BASED" + i + j, Speed: 30, Verify_EOF_BOF: false);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set the trick mode speed to 30");
                    }
                    CL.IEX.Wait(2);
                    //Stop the Playback of the Recording
                    res = CL.EA.PVR.StopPlayback();
                    if (!res.CommandSucceeded)
                    {
                        LogCommentFailure(CL, "Failed to stop the Playback of the Recording");
                    }
                    CL.IEX.Wait(10);
                    res=CL.EA.ReturnToLiveViewing();
                    if (!res.CommandSucceeded)
                    {
                        LogCommentFailure(CL, "Failed to return to live viewing");
                    }
                }
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
    public class Helper : _Step
    {

        public override void Execute() { }

        /// <summary>
        /// Verifies the Duration of the Recordings
        /// </summary>
        /// <returns>bool</returns>


        #region Reorder Favourites

        public bool VerifyDuration()
        {
            bool isPass = true;
            string duration = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("duration", out duration);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to get Duration");
                isPass= false;
            }
            int eventDuration = Convert.ToInt32(Convert.ToInt32(defaultSGT) + Convert.ToInt32(defaultEGT) + 10);
            int durationInt = Convert.ToInt32(Regex.Match(duration, @"\d+").Value);
            if(durationInt!=eventDuration)
            {
                LogCommentFailure(CL, "Failed to get Duration");
                isPass = false;
            }

            return isPass;
        }

        #endregion
    }
}