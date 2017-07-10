/// <summary>
///  Script Name : BOOK_0806_Modify_EventBaseToTimeBaseRecurring.cs
///  Test Name   : BOOK-0806-Modify-Event Base To Time Base Recurring
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0806 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_1;

    //CQ#2247606:Scheduling the Manual recording after 30min of EventBase recording
    private static class Constants
    {
        public const int extraTimeAdded = 30;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Book Future Event Base Recording");
        this.AddStep(new Step2(), "Step 2: Modify Event Base To Time Base Recurring (Frequency Daily)");
        this.AddStep(new Step3(), "Step 3: Verify That Modification of Event Base To Time Base Recurring Succeed");

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

            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");

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

            res = CL.EA.PVR.BookFutureEventFromGuide("eR1", FTA_1st_Mux_1, 3, 10, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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

            //CQ#2247606: While updating EventBase recording to Manual Recording, Start & End Time was hardcoded which was failing to schedule current days recording.
            //Which was failing in Step 3 while verifying the number of Recordings.
            //Fetching the Event Start Time and End Time from The Event collection and adding extra time and modifying this EBR
            string evtStartTime = CL.EA.GetEventInfo("eR1", EnumEventInfo.EventStartTime);
            if (string.IsNullOrEmpty(evtStartTime))
            {
                FailStep(CL, "Retrieved start time from event info is null");
            }
            LogComment(CL, "Event Start time is " + evtStartTime);
            string evtEndTime = CL.EA.GetEventInfo("eR1", EnumEventInfo.EventEndTime);
            if (string.IsNullOrEmpty(evtEndTime))
            {
                FailStep(CL, "Retrieved end time from event info is null");
            }
            LogComment(CL, "Event End time is " + evtEndTime);
            //Including extra Start Guard Time
            TimeSpan startTime = TimeSpan.Parse(evtStartTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            //Including extra End Guard Time
            TimeSpan endTime = TimeSpan.Parse(evtEndTime).Add(TimeSpan.Parse("00:" + Constants.extraTimeAdded));

            res = CL.EA.PVR.ModifyManualRecording("eR1", startTime.ToString("hh\\:mm"), endTime.ToString("hh\\:mm"), "", 0, EnumFrequency.DAILY, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
            }

            res = CL.EA.PVR.ModifyManualRecording("eR1", startTime.ToString("hh\\:mm"), endTime.ToString("hh\\:mm"), "", 0, EnumFrequency.DAILY, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to modify manual recording from planner");
            }
            //res = CL.EA.PVR.ModifyManualRecording("eR1", "00:10", "00:40", "", 0, EnumFrequency.DAILY);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res);
            //}
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.VerifyRecurringBookingInPlanner("eR1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}