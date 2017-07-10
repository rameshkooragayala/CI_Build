/// <summary>
///  Script Name : TIMER_0002_OverlappingReminder.cs
///  Test Name   : TIMER-0002-Booking Overlapping Reminders
///  TEST ID     : 63821
///  QC Version  : 1
///  JIRA ID     : FC-379
/// ----------------------------------------------
///  Modified by : Avinob Aich
///  Modified Date: 11/07/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("TIMER-0002-Booking Overlapping Reminders")]
public class TIMER_0002 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service ReminderChannel_1;
    private static Service ReminderChannel_2;
    private static Service ReminderChannel_3;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File & book reminder for an Event R1";
    private const string STEP1_DESCRIPTION = "Step 1: Book reminders for event which are overlapping R1";
    private const string STEP2_DESCRIPTION = "Step 2: Check that all reminder have been booked";

    private static class Constants
    {
        public const int noOfPresses = 2;
        public const int minTimeBeforeEvtStart = 2; //in min
        public const int checkTime = 5; //in min
    }

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

            int timeLeft = 0;
            //Get Values From channel.xml File
            ReminderChannel_1 = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=High");
            ReminderChannel_2 = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "LCN=" + ReminderChannel_1.LCN + ";ParentalRating=High");
            ReminderChannel_3 = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "LCN=" + ReminderChannel_1.LCN + "," + ReminderChannel_2.LCN + ";ParentalRating=High");

            //Tune to a channel
            res = CL.EA.TuneToChannel(ReminderChannel_1.LCN);
            LogCommentInfo(CL, "Tuning to channel: " + ReminderChannel_1.LCN + " to get time left for next event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel");
            }

            //Get Event Left time to Start next Event
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get left time for next event to start");
            }

            //Checking if left time is less than 5 mins to ensure overlapping
            if (timeLeft < (Constants.checkTime * 60))
            {
                CL.IEX.Wait(Convert.ToDouble(timeLeft));
            }

            //Add reminder to an next event from Guide on Channel 1
            res = CL.EA.BookReminderFromGuide("EventName_1", ReminderChannel_1.LCN, Constants.noOfPresses, Constants.minTimeBeforeEvtStart, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Add Reminder to event from Banner in Channel: " + ReminderChannel_1.LCN);
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

            //Add reminder to next event in from Guide on channel 2 which overlaps event in channel 1
            res = CL.EA.BookReminderFromGuide("EventName_2", ReminderChannel_2.LCN, Constants.noOfPresses, Constants.minTimeBeforeEvtStart, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Add Reminder to event from Banner in channel: " + ReminderChannel_2.LCN);
            }
            //Add reminder to next event in from Guide on channel 2 which overlaps event in channel 1
            res = CL.EA.BookReminderFromGuide("EventName_3", ReminderChannel_3.LCN, Constants.noOfPresses, Constants.minTimeBeforeEvtStart, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Add Reminder to event from Banner in channel: " + ReminderChannel_3.LCN);
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

            bool isPass = true;
            //Verifying reminder for channel 1
            res = CL.EA.PCAT.VerifyEventBooked("EventName_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify reminder of event in channel: " + ReminderChannel_1.LCN, false);
                isPass = false;
            }

            //verifying reminder for channel 2
            res = CL.EA.PCAT.VerifyEventBooked("EventName_2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify reminder of event in channel: " + ReminderChannel_2.LCN, false);
                isPass = false;
            }

            //verifying reminder for channel 3
            res = CL.EA.PCAT.VerifyEventBooked("EventName_3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify reminder of event in channel: " + ReminderChannel_3.LCN);
            }

            if (!isPass)
            {
                FailStep(CL, "Captured in Previous");
            }

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