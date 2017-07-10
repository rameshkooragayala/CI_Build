/// <summary>
///  Script Name        : TIMER_0020_Multiple_ReminderNotification_Order.cs
///  Test Name          : TIMER-0020 Multiple reminder notification order
///  TEST ID            : 
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
using System.Collections;

public class TIMER_0020: _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    static Helper helper = new Helper();
    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and set three Reminders on the same time on 2 occasions");
        this.AddStep(new Step1(), "Step 1: Wait Until the first 3 Reminders and reject them and verify the order");
        this.AddStep(new Step2(), "Step 2: Accept the Next 3 reminders booked at the same time and verify that it is tuning");


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

            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_4.LCN);
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_4.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_4.LCN);
            }

            int timeLeftInSec = 0;
            res=CL.EA.GetCurrentEventLeftTime(ref timeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the current event time left");
            }

            if (timeLeftInSec < 540)
            {
                CL.IEX.Wait(timeLeftInSec);
            }
            //Booking the reminder on service 1 for 2nd event and 3rd event at the same time without returning to live
            res = CL.EA.BookReminderFromGuide("ReminderEvent11", Service_1.LCN, NumberOfPresses: 2, VerifyBookingInPCAT: false,ReturnToLive:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to book reminder from Guide on "+Service_1.LCN);
            }
            res = CL.EA.BookReminderFromGuide("ReminderEvent21", Service_1.LCN, NumberOfPresses: 1, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_1.LCN);
            }
            //Booking the reminder on service 2 for 2nd event and 3rd event at the same time without returning to live
            res = CL.EA.BookReminderFromGuide("ReminderEvent12", Service_2.LCN, NumberOfPresses: 2, VerifyBookingInPCAT: false,ReturnToLive:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_2.LCN);
            }
            res = CL.EA.BookReminderFromGuide("ReminderEvent22", Service_2.LCN, NumberOfPresses: 1, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_2.LCN);
            }
            //Booking the reminder on service 3 for 2nd event and 3rd event at the same time without returning to live
            res = CL.EA.BookReminderFromGuide("ReminderEvent13", Service_3.LCN, NumberOfPresses: 2, VerifyBookingInPCAT: false,ReturnToLive:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_3.LCN);
            }

            res = CL.EA.BookReminderFromGuide("ReminderEvent23", Service_3.LCN, NumberOfPresses: 1, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_3.LCN);
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
            res = CL.EA.WaitUntilReminder("ReminderEvent11");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until reminder");
            }

            if (!helper.VerifyMultipleReminders("ReminderEvent11"))
            {
                FailStep(CL,"Failed to verify the remiders...Please check the above failures");
            }
            if (!helper.VerifyMultipleReminders("ReminderEvent12"))
            {
                FailStep(CL, "Failed to verify the remiders...Please check the above failures");
            }
            if (!helper.VerifyMultipleReminders("ReminderEvent13"))
            {
                FailStep(CL, "Failed to verify the remiders...Please check the above failures");
            }
            string obtainedChannelNumber = "";
            CL.EA.UI.Menu.GetChannelNumber(ref obtainedChannelNumber);
            if (obtainedChannelNumber!=Service_4.LCN)
            {
                FailStep(CL, "Tuned to the reminded service " + obtainedChannelNumber + " after rejecting the reminder");
            }
            else
            {
                LogCommentImportant(CL, "Not Tuned to any of the reminded service after rejecting the reminder");
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
            res = CL.EA.WaitUntilReminder("ReminderEvent21");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until reminder");
            }
            if (!helper.VerifyMultipleReminders("ReminderEvent21"))
            {
                FailStep(CL, "Failed to verify the remiders...Please check the above failures");
            }
            if (!helper.VerifyMultipleReminders("ReminderEvent22"))
            {
                FailStep(CL, "Failed to verify the remiders...Please check the above failures");
            }
            if (!helper.VerifyMultipleReminders("ReminderEvent23"))
            {
                FailStep(CL, "Failed to verify the remiders...Please check the above failures");
            }
            string obtainedChannelNumber = "";
            CL.EA.UI.Menu.GetChannelNumber(ref obtainedChannelNumber);
            if (obtainedChannelNumber != Service_3.LCN)
            {
                FailStep(CL, "Failed to tuned to Service 3" + Service_3.LCN + " after accepting the reminder");
            }
            else
            {
                LogCommentImportant(CL, "Tuned to service " + Service_3.LCN + " after accepting the reminder");
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
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Verifies the Multiple reminders at the same time
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifyMultipleReminders(string reminderEventKeyName)
        {
            bool isPass = true;
            string ExpectedEventName;
            string obtainedEventName;
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed To fetch the evtName from EPG");
                isPass = false;
            }

            ExpectedEventName = CL.EA.GetEventInfo(reminderEventKeyName, EnumEventInfo.EventName);
            if (string.IsNullOrEmpty(ExpectedEventName))
            {
                LogCommentFailure(CL, "Retrieved event name from event info is null");
                isPass = false;
            }

            if (obtainedEventName != ExpectedEventName)
            {
                LogCommentFailure(CL, "Obtained Event name from EPG " + obtainedEventName + " is different from Expected " + ExpectedEventName);
                isPass = false;
            }
            else
            {
                LogCommentImportant(CL, "Obtained Event name from EPG " + obtainedEventName + " is same as Expected " + ExpectedEventName);
            }
            if (reminderEventKeyName.Contains("ReminderEvent2"))
            {
                String Milestones = "IEX_Zapping_Type:";

                CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, 20);
                CL.EA.UI.OSD_Reminder.AcceptReminder();
                ArrayList arraylist = new ArrayList();
                if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arraylist))
                {
                    FailStep(CL, "Failed to Verify zapping milestone after verifying accepting the reminder");
                }
            }
            else
            {
                CL.EA.UI.OSD_Reminder.RejectReminder();
            }
            CL.IEX.Wait(2);


            return isPass;
        }
    }

    #endregion
}
