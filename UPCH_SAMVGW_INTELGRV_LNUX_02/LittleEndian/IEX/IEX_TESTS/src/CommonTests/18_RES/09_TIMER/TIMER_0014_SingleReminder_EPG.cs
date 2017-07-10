/// <summary>
///  Script Name        : TIMER_0014_SingleReminder_EPG.cs
///  Test Name          : IMER-0014 Single reminder notification EPG
///  TEST ID            : 74673
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

public class TIMER_0014 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static string verifyReminderInState;
    static Helper helper = new Helper();

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and Set Reminders on Services S1 and S2 ");
        this.AddStep(new Step1(), "Step 1: Wait Until the Reminder on S1 in any EPG Screen");
        this.AddStep(new Step2(), "Step 2: Reject the reminder");
        this.AddStep(new Step3(), "Step 3: Wait until the reminder S2 in any EPG Screen");
        this.AddStep(new Step4(), "Step 4: Accept the Reminder and verify that you have tuned to that particular Service S2");

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

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_2.LCN);
            }

            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_3.LCN);
            }
            verifyReminderInState = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "VERIFY_REMINDER_IN_STATE");
            if (verifyReminderInState == "")
            {
                FailStep(CL, "Failed to fetch VERIFY_REMINDER_IN_STATE from test ini");
            }
            if (verifyReminderInState == "STATE:TV GUIDE")
            {
                res = CL.EA.STBSettings.SetTvGuideBackgroundAsSolid();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to set the TV Back ground to solid");
                }
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_3.LCN);
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

            if (!helper.WaitUntilReminderInEPG("ReminderEvent"))
            {
                FailStep(CL, "Failed ToString wait until reminerd in RB");
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
                FailStep(CL,res,"Failed to Reject the reminder");
            }
            CL.IEX.Wait(10);
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
            if (!helper.WaitUntilReminderInEPG("ReminderEvent1"))
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
                FailStep(CL, res, "Failed to Accept the reminder");
            }



            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        if (verifyReminderInState == "STATE:TV GUIDE")
        {
            res = CL.EA.STBSettings.SetTvGuideBackgroundAsTransparent();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to set the TV Back ground to solid");
            }
        }
    }

    #endregion PostExecute
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Waits until the reminder and navigates to menu screen 
        /// </summary>
        /// <returns>bool</returns>
        public bool  WaitUntilReminderInEPG(string reminderEventKeyName)
        {
            bool isPass = true;
            string EpgTime = "";
            int timeToWait = 0;
            string evtStartTime = CL.EA.GetEventInfo(reminderEventKeyName, EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                LogCommentFailure(CL, "Retrieved start time from event info is null");
                isPass = false;
            }
            LogComment(CL, "Event Start time is " + evtStartTime);

            CL.EA.UI.Banner.Navigate();
            CL.EA.UI.Live.GetEpgTime(ref EpgTime);
            res = CL.IEX.MilestonesEPG.NavigateByName(verifyReminderInState);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to navigate to Main Menu");
                isPass = false;
            }
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
