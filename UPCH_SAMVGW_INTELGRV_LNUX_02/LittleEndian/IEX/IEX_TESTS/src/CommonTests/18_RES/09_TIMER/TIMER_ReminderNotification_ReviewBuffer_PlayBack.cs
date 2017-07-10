/// <summary>
///  Script Name        : TIMER_ReminderNotification_ReviewBuffer_PlayBack.cs
///  Test Name          : TIMER-0011 Single reminder notification from playback from review buffer, TIMER-0012 Single reminder notification from playback from recording
///  TEST ID            : 74672, 71198
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 16th Sept, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class TIMER_RB_Playback : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static string isReviewBuffer;
    static Helper helper = new Helper();
    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and set two Reminders, (Pause and Play in RB)/(Record and Playback)");
        this.AddStep(new Step1(), "Step 1: Wait Until the first Reminder on Service S1");
        this.AddStep(new Step2(), "Step 2: Reject the Reminder on Service S1");
        this.AddStep(new Step3(), "Step 3: Wait until the Second Reminder S2 when still in RB/Playback");
        this.AddStep(new Step4(), "Step 4: Accept the reminder and verify that we have tuned to that particular service");

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

            //Get Values From ini File

           

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN="+ Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_2.LCN);
            }

            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN+","+Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_3.LCN);
            }

            isReviewBuffer = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_REVIEWBUFFER");
            if (isReviewBuffer == "")
            {
                FailStep(CL, res, "Failed to fetch IS_REVIEWBUFFER from Test ini");
            }
            else
            {
                LogCommentImportant(CL, "IS_REVIEWBUFFER fetched from Test ini is "+isReviewBuffer);
            }


            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_3.LCN);
            }
            if (isReviewBuffer.ToUpper()=="TRUE")
            {
                res = CL.EA.BookReminderFromGuide("ReminderEvent", Service_1.LCN, NumberOfPresses: 2, MinTimeBeforeEvStart: 4, VerifyBookingInPCAT: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book reminder from Guide");
                }
                res = CL.EA.BookReminderFromGuide("ReminderEvent1", Service_2.LCN, NumberOfPresses: 3, MinTimeBeforeEvStart: 3, VerifyBookingInPCAT: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book reminder from Guide");
                }
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to set the trick mode speed to 0");
                }
                res = CL.IEX.Wait(seconds: 120);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait few seconds in Pause mode");
                }
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 1, Verify_EOF_BOF: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to set the trick mode speed to 0");
                }
            }
            else 
            {
                res = CL.EA.PVR.RecordManualFromCurrent("ManualRecording", Service_3.LCN, DurationInMin: 20, VerifyBookingInPCAT: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to record Manual from Current on "+Service_3.LCN);
                }
                res = CL.IEX.Wait(600);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to wait few minutes into the recording");
                }
                res = CL.EA.BookReminderFromGuide("ReminderEvent", Service_1.LCN, NumberOfPresses: 2, MinTimeBeforeEvStart: 4, VerifyBookingInPCAT: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book reminder from Guide");
                }
                res = CL.EA.BookReminderFromGuide("ReminderEvent1", Service_2.LCN, NumberOfPresses: 3, MinTimeBeforeEvStart: 3, VerifyBookingInPCAT: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book reminder from Guide");
                }
                res = CL.EA.WaitUntilEventEnds("ManualRecording");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to wait until event ends");
                }
                res = CL.EA.PVR.PlaybackRecFromArchive("ManualRecording", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to playback record from Archive");
                }
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
            if (!helper.WaitUntilReminderInRB("ReminderEvent"))
            {
                FailStep(CL,"Failed To wait until reminerd in RB");
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
            res = CL.EA.HandleReminder("ReminderEvent", EnumReminderActions.Reject);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to handle reminder");
            }

            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG Info from EPG");
            }
            if (chNum != Service_3.LCN)
            {
                FailStep(CL, "Tuned to service " + chNum + "after rejecting the reminder which is not expected");
            }
            else
            {
                LogComment(CL,"We are still in service "+Service_3.LCN+" after rejecting the reminder");
            }

            if (isReviewBuffer.ToUpper() == "FALSE")
            {
                bool playBackState = CL.EA.UI.Utils.VerifyState("PLAYBACK", 10);
                if (playBackState)
                {
                    LogCommentInfo(CL, "Playback state verified sucessfully after cancelling the reminder");
                }
                else
                {
                    FailStep(CL, res, "Unable to verify the Playback state");
                }

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

            if (!helper.WaitUntilReminderInRB("ReminderEvent1"))
            {
                FailStep(CL, "Failed ToString wait until reminerd in RB");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.HandleReminder("ReminderEvent1", EnumReminderActions.Accept);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to handle reminder");
            }

            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG Info from EPG");
            }
            if (chNum != Service_2.LCN)
            {
                FailStep(CL, "After Accepting the reminder we are still in "+chNum);
            }
            else
            {
                LogComment(CL, "Tuned to service "+Service_2.LCN+" after accepting the reminder");
            }
            if (isReviewBuffer.ToUpper() == "FALSE")
            {
                bool playBackState = CL.EA.UI.Utils.VerifyState("LIVE", 10);
                if (playBackState)
                {
                    LogCommentInfo(CL, "Live state verified sucessfully after cancelling the reminder");
                }
                else
                {
                    FailStep(CL, res, "Unable to verify the Live state");
                }

            }
            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        CL.EA.PVR.DeleteAllRecordsFromArchive();
    }

    #endregion PostExecute
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Waits until the Reminder
        /// </summary>
        /// <returns>bool</returns>
        public bool WaitUntilReminderInRB(string reminderEventKeyName)
        {
            bool isPass = true;
            string EpgTime = "";
            int timeToWait = 0;
            string evtStartTime = CL.EA.GetEventInfo(reminderEventKeyName, EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved start time from event info is null");
            }
            LogComment(CL, "Event Start time is " + evtStartTime);
			CL.IEX.Wait(5);
            if (isReviewBuffer.ToUpper() == "TRUE")
            {
                CL.EA.UI.Banner.Navigate();
            }
            else
            {
                CL.EA.UI.Banner.Navigate(FromPlayback:true);
            }
            CL.IEX.Wait(5);
            CL.EA.UI.Live.GetEpgTime(ref EpgTime);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to navigate to Main Menu");
            }
            CL.IEX.Wait(10);
            timeToWait = CL.EA.UI.Utils._DateTime.SubtractInSec(Convert.ToDateTime(evtStartTime).AddMinutes(-1), Convert.ToDateTime(EpgTime));
            CL.IEX.Wait(timeToWait - 90);
            LogCommentImportant(CL, "Time To Wait Until Reminder : 90 Seconds");
            LogCommentImportant(CL, "Waiting For Reminder Until : " + Convert.ToDateTime(EpgTime).AddSeconds(timeToWait).ToString("HH:mm:ss"));
            CL.EA.UI.OSD_Reminder.VerifyReminderAppeared();
            return isPass;
        }
    }

    #endregion
}
