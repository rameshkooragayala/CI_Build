/// <summary>
///  Script Name : TIMER_0030_CancelReminder
///  Test Name   : TIMER-0030-Cancel Booking Of Reminder
///  TEST ID     : 63990
///  JIRA ID     : FC-324
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh/Appanna
///  Modified Date : 9/19/2013
/// </summary>

using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class TIMER_0030 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service reminderService;

    private static class Constants
    {
        public const int minTimeForEventStart = 4;
        public const int numOfPresses = 1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: User can cancel booking of a reminder, before the reminder is triggered
        //Pre-conditions: Profile is set to receive reminder notifications and There is reminder booking for future event
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync, Set reminder notification ON and Add reminder for future event");
        this.AddStep(new Step1(), "Step 1: Before Booked reminder was triggered, navigate to reminder and cancel booking");
        this.AddStep(new Step2(), "Step 2: Wait till end of event and verify that reminder is deleted");

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

            //Get Values From xml File
            reminderService = CL.EA.GetServiceFromContentXML("IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (reminderService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }



            //Fetching the Reminder Settings SUPPORTED Flag from INI file

            string is_SettingsSupported = CL.EA.GetValueFromINI(EnumINIFile.Project, "REMINDER_NOTIFICATION_SETTINGS", "SUPPORTED");
            if (string.IsNullOrEmpty(is_SettingsSupported))
            {
                FailStep(CL, "Failed To fetch  the Settings Flag Value from Project INI file");

            }

            //Checking the Reminder SettingsSupported flag for True/False.
            if (Convert.ToBoolean(is_SettingsSupported))
            {
                res = CL.EA.STBSettings.SetReminderNotifications(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Turn Reminder Notifications On");
                }
            }

            //Set Reminder from Guide
            res = CL.EA.BookReminderFromGuide("ReminderEvent", reminderService.LCN, Constants.numOfPresses, Constants.minTimeForEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Reminder for event on service " + reminderService.LCN);
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

            //Delete Reminder from Guide Before notification
            res = CL.EA.CancelReminderFromGuide("ReminderEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to cancel Reminder for event on service " + reminderService.LCN);
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

            //Verify that reminder is deleted
            res = CL.EA.WaitUntilReminder("ReminderEvent");
            int failureCode = res.FailureCode;
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Got Reminder for event");
            }

            //Check for failure reason
            if (!failureCode.Equals(ExitCodes.ReminderFailure.GetHashCode()))
            {
                FailStep(CL, res, "Got Reminder for event");
            }

            LogCommentInfo(CL, "Successfully verified that Reminder is deleted");

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Reset the default setting of Reminder notification to OFF
        String defaultReminder = CL.EA.GetValueFromINI(EnumINIFile.Project, "REMINDER NOTIFICATION", "DEFAULT");
        if (defaultReminder.Equals("OFF"))
        {
            res = CL.EA.STBSettings.SetReminderNotifications(false);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Set Reminder Notification setting OFF because :" + res.FailureReason);
            }
        }
    }

    #endregion PostExecute
}