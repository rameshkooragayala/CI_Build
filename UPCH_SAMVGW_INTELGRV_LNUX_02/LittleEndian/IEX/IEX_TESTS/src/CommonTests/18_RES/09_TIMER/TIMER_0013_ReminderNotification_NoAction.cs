/// <summary>
///  Script Name : TIMER_0013_ReminderNotification_NoAction
///  Test Name   : TIMER-0013-No Action at Reminder Notification
///  TEST ID     : 64526
///  JIRA ID     : FC-506
///  QC Version  : 1
///  Variations from QC:None
/// -----------------------------------------------
///  Modified by : Appanna Kangira
///  Modified Date:09/19/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("TIMER_0013_ReminderNotification_NoAction")]
public class TIMER_0013 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service serviceForReminder;
    private static Service serviceForLive;

    private static class Constants
    {
        public const int numberofRightKeyPressinGrid = 1;
        public const int minTimeBeforeEventStart = 3;
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Set Reminders Settings";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to Service S1 and Set a Reminder";
    private const string STEP2_DESCRIPTION = "Step 2: Wait for the Reminder Notification and Ignore the Reminder";

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From XML File
            serviceForReminder = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=High");
            serviceForLive = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + serviceForReminder.LCN);

            if (serviceForReminder.Equals(null))
            {
                FailStep(CL, "Returned serviceForReminder NULL");
            }
            if (serviceForLive.Equals(null))
            {
                FailStep(CL, "Returned serviceForLive NULL");
            }

            //Fetching the Reminder Settings SUPPORTED Flag from INI file

            string is_SettingsSupported = CL.EA.GetValueFromINI(EnumINIFile.Project, "REMINDER_NOTIFICATION_SETTINGS", "SUPPORTED");
            if (string.IsNullOrEmpty(is_SettingsSupported))
            {
                FailStep(CL, "Failed To fetch  the Settings Flag Value from Project INI file");

            }

            //Checking the SettingsSupported flag for True/False.
            if (Convert.ToBoolean(is_SettingsSupported))
            {
                res = CL.EA.STBSettings.SetReminderNotifications(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Turn Reminder Notifications On");
                }
            }
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to Live AV Service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceForLive.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Live Service:" + serviceForLive.LCN);
            }

            //  Set Reminder on Future Event on Reminder Service
            res = CL.EA.BookReminderFromGuide("Event1", serviceForReminder.LCN, Constants.numberofRightKeyPressinGrid, Constants.minTimeBeforeEventStart);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Reminder from Guide on future event of Service:" + serviceForReminder.LCN);
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Wait for the Reminder Time of a Reminded Event [Start Time - 60 sec]
            res = CL.EA.WaitUntilReminder("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait for the Event Reminder popup ");
            }

            //Ignore the Reminder. Verify Ignoring the reminder doesn't tune away from the currently tuned channel.
            res = CL.EA.HandleReminder("Event1", EnumReminderActions.Wait);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Ignore the Reminder Popup for Event1");
            }
            LogCommentInfo(CL, "Reminder Notification is ignored Successfully and currently tuned channel is maintained" + serviceForLive.LCN);

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}