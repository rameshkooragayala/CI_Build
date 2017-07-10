/// <summary>
///  Script Name : TIMER_0010_SingleReminder_Live.cs
///  Test Name   : TIMER-0010-Single Reminder-Notification From Live Viewing
///  TEST ID     : 64000
///  QC Version  : 1
///  JIRA ID     :FC-297
/// -----------------------------------------------
///  Modified by : Appanna Kangira
///  Last Modified: 09/19/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("TIMER_0010_Single_Reminder_Live")]
public class TIMER_0010 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service service1ForFirstReminder;
    private static Service service2ForSecondReminder;
    private static Service service3ForLive;

    private static class Constants
    {
        public const int numberofRightKeyPressinGrid = 1;
        public const int minTimeBeforeEventStart = 3;
        public const int videoCheckTimeout = 10;
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File & Set Reminders Settings";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to S3 & Set First Reminder on Service S1";
    private const string STEP2_DESCRIPTION = "Step 2: Wait for the Reminder Time of the First Event booked in Live & reject the reminder";
    private const string STEP3_DESCRIPTION = "Step 3: Tune to S3 & Set second Reminder on Service S2";
    private const string STEP4_DESCRIPTION = "Step 4: Wait for the Reminder Time of the second  Event booked in Live & Accept the reminder";

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);

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
            service1ForFirstReminder = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=High");
            if (service1ForFirstReminder.Equals(null))
            {
                FailStep(CL, "Failed to retrieve  Service1 for Reminder");
            }

            service2ForSecondReminder = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "LCN=" + service1ForFirstReminder.LCN);
            if (service2ForSecondReminder.Equals(null))
            {
                FailStep(CL, "Failed to retrieve Service2 for Reminder");
            }
            service3ForLive = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + service1ForFirstReminder.LCN + "," + service2ForSecondReminder.LCN);

            if (service3ForLive.Equals(null))
            {
                FailStep(CL, "Failed to retrieve Live Service");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service3ForLive.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to AV Service: " + service3ForLive.LCN);
            }

            //  Reminders on Future Event1 on S1
            res = CL.EA.BookReminderFromGuide("Event1", service1ForFirstReminder.LCN, Constants.numberofRightKeyPressinGrid, Constants.minTimeBeforeEventStart);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Reminder from Guide on future event of service 1" + service1ForFirstReminder.LCN);
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
                FailStep(CL, res, "Failed to Wait for Event2 Reminder Time");
            }

            //Reject the Reminder. Verify Tuning to the Correct Channel Performed
            res = CL.EA.HandleReminder("Event1", EnumReminderActions.Reject);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Reject the Reminder for Event2");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //  Set Reminder on Future Event1 on S2
            res = CL.EA.BookReminderFromGuide("Event2", service2ForSecondReminder.LCN, Constants.numberofRightKeyPressinGrid, Constants.minTimeBeforeEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Reminder from Guide on future event of  S2" + service2ForSecondReminder.LCN);
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Wait for the Reminder Time of a Reminded Event [Start Time - 60 sec]
            res = CL.EA.WaitUntilReminder("Event2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait for Event3 Reminder Time");
            }

            //Accept the Reminder. Verify Tuning to the Correct Channel Performed
            res = CL.EA.HandleReminder("Event2", EnumReminderActions.Accept);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Accept the Reminder for Event3 on S2");
            }

            //checking whether video is present on tuning to the reminder event.
            res = CL.EA.CheckForVideo(true, false, Constants.videoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Verify Video is Present on Reminded Event on Channel S2");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}