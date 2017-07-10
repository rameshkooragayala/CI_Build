/// <summary>
///  Script Name : ON_MP_OverlapRecordings.
///  Test Name   : Overnight overlap Recordings
///  TEST ID     : NA
///  QC Version  : NA
///  Variations From QC: NA
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 09th Jan, 2014
/// </summary>

using System;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using System.Collections;
using System.Globalization;

public class ON_MP_OverlapRecordings : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service recordableService;
    private static Service recordableService1;
    private static string powerMode;


    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Do some recording such a way that box wont go to Stand by due to Inactivity");
        this.AddStep(new Step1(), "Step 1: Wait until the Recordings are completed");
        this.AddStep(new Step2(), "Step 2: Once the Recordings are completed , Playback and Verify the Recordings with Trick mode");

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
            //Setting the Auto Stand by
            res = CL.EA.STBSettings.SetAutoStandBy("OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Faile to set the Auto Stand by to ");
            }
            //1st Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED", Convert.ToInt32(recordableService.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 180, DurationInMin: 95, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService.LCN);
            }
            //2nd Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED1", Convert.ToInt32(recordableService1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 240, DurationInMin: 60, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService1.LCN);
            }
            //3rd Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED2", Convert.ToInt32(recordableService1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 481, DurationInMin: 60, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService1.LCN);
            }
            //4th Manual Recording
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED3", Convert.ToInt32(recordableService1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 510, DurationInMin: 60, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on service " + recordableService1.LCN);
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
           
            res = CL.EA.WaitUntilEventEnds("TIME_BASED3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until Event Ends");
            }
            LogCommentImportant(CL, "Sucessfully waited until all the recordings were completed");
            //Waiting for Three hours after the recordings rea completed
            res = CL.IEX.Wait(180*60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait few hours after the recordings were completed");
            }
            CL.EA.UI.Utils.SendIR("MENU");

            bool menuState = CL.EA.UI.Utils.VerifyState("MAIN MENU", 20);
            if (menuState)
            {
                LogCommentInfo(CL, "Main Menu state verified sucessfully after idle wait time since the stand by mode is OFF");
            }
            else
            {
                FailStep(CL, res, "Unable to verify Main Menu after idle wait time");
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
            //Getting Event end time from the event collection

            string evtStartTime = CL.EA.GetEventInfo("TIME_BASED", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved Start time from event info is null");
            }
            else
            {
                LogComment(CL, "Event Start time is " + evtStartTime);
            }

            //Getting Event start time from the event collection

            string evtEndTime = CL.EA.GetEventInfo("TIME_BASED", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            else
            {
                LogComment(CL, "Event End time is " + evtEndTime);
            }
            string evtDate = CL.EA.GetEventInfo("TIME_BASED", EnumEventInfo.EventDate);
            if (string.IsNullOrEmpty(evtDate))
            {
                FailStep(CL, "Retrieved evtDate from event info is null");
            }
            else
            {
                LogComment(CL, "Event Date is " + evtDate);
            }
           string EPGDateFormat= CL.EA.UI.Utils.GetEpgDateFormat();
           LogCommentInfo(CL, "Expected Date fetched from the event collection is " + evtDate);
           string eventDictionaryDateFormat = CL.EA.UI.Utils.GetValueFromProject("EPG", "DATE_FORMAT_FOR_EVENT_DIC");
           string EPGEventdate = DateTime.ParseExact(evtDate, eventDictionaryDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);
           LogCommentImportant(CL, "Event Date after Parsing is" + EPGEventdate);
          
            CL.IEX.Wait(10);
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED", SecToPlay: 0, FromBeginning: true, VerifyEOF: false, StartTime: evtStartTime, EndTime: evtEndTime, EvDate: EPGEventdate);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
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
                LogCommentFailure(CL, "Failed to stop the Playback of the Recording");
            }
            CL.IEX.Wait(10);
            //Getting Event end time from the event collection
           res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to return to live viewing");
            }
            string evtStartTime1 = CL.EA.GetEventInfo("TIME_BASED1", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime1))
            {
                FailStep(CL, "Retrieved Start time from event info is null");
            }
            else
            {
                LogComment(CL, "Event Start time is " + evtStartTime1);
            }

            //Getting Event start time from the event collection

            string evtEndTime1 = CL.EA.GetEventInfo("TIME_BASED1", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime1))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            else
            {
                LogComment(CL, "Event End time is " + evtEndTime1);
            }
            string evtDate1 = CL.EA.GetEventInfo("TIME_BASED1", EnumEventInfo.EventDate);
            if (string.IsNullOrEmpty(evtDate1))
            {
                FailStep(CL, "Retrieved evtDate from event info is null");
            }
            else
            {
                LogComment(CL, "Event Date is " + evtDate1);
            }
            string EPGEventdate1 = DateTime.ParseExact(evtDate1, eventDictionaryDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);
            LogCommentImportant(CL, "Event Date after Parsing is" + EPGEventdate1);
            CL.IEX.Wait(10);
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED1", SecToPlay: 0, FromBeginning: true, VerifyEOF: false, StartTime: evtStartTime1, EndTime: evtEndTime1, EvDate: EPGEventdate1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED1", Speed: 30, Verify_EOF_BOF: false);
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
            //Getting Event end time from the event collection
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to return to live viewing");
            }
            string evtStartTime2 = CL.EA.GetEventInfo("TIME_BASED2", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime2))
            {
                FailStep(CL, "Retrieved Start time from event info is null");
            }
            else
            {
                LogComment(CL, "Event Start time is " + evtStartTime2);
            }

            //Getting Event start time from the event collection

            string evtEndTime2 = CL.EA.GetEventInfo("TIME_BASED2", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime2))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            else
            {
                LogComment(CL, "Event End time is " + evtEndTime2);
            }
            string evtDate2 = CL.EA.GetEventInfo("TIME_BASED2", EnumEventInfo.EventDate);
            if (string.IsNullOrEmpty(evtDate2))
            {
                FailStep(CL, "Retrieved evtDate from event info is null");
            }
            else
            {
                LogComment(CL, "Event Date is " + evtDate2);
            }
            string EPGEventdate2 = DateTime.ParseExact(evtDate2, eventDictionaryDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);
            LogCommentImportant(CL, "Event Date after Parsing is" + EPGEventdate2);
            CL.IEX.Wait(60);

            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED2", SecToPlay: 0, FromBeginning: true, VerifyEOF: false,StartTime:evtStartTime2,EndTime:evtEndTime2,EvDate: EPGEventdate2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
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
            //Getting Event end time from the event collection

            string evtStartTime3 = CL.EA.GetEventInfo("TIME_BASED3", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime3))
            {
                FailStep(CL, "Retrieved Start time from event info is null");
            }
            else
            {
                LogComment(CL, "Event Start time is " + evtStartTime3);
            }

            //Getting Event start time from the event collection

            string evtEndTime3 = CL.EA.GetEventInfo("TIME_BASED3", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime3))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            else
            {
                LogComment(CL, "Event End time is " + evtEndTime3);
            }
            string evtDate3 = CL.EA.GetEventInfo("TIME_BASED3", EnumEventInfo.EventDate);
            if (string.IsNullOrEmpty(evtDate3))
            {
                FailStep(CL, "Retrieved evtDate from event info is null");
            }
            else
            {
                LogComment(CL, "Event Date is " + evtDate3);
            }
            string EPGEventdate3 = DateTime.ParseExact(evtDate3, eventDictionaryDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);
            LogCommentImportant(CL, "Event Date after Parsing is" + EPGEventdate3);
            res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED3", SecToPlay: 0, FromBeginning: true, VerifyEOF: false, StartTime: evtStartTime3, EndTime: evtEndTime3,EvDate:EPGEventdate3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback record from Archive");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED3", Speed: 30, Verify_EOF_BOF: false);
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
            LogCommentImportant(CL,"Successfully completed the verification of trickmodes on all the recordings");
           
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