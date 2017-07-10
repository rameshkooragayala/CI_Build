using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class LightSanity_200 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration 
    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string FTA_1st_Mux_2;
    static string FTA_1st_Mux_3;

    //Shared parameters between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-0200-TIMER-Reminders Bookings, popup and Disable
        //Brief Description: Test Book Reminder for an Event & Reminder Triggering
        //Events S2 and S3 are future events on other channels Then S1 - channels S2 and S3
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not checking Audio.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Set Reminders Settings");
        this.AddStep(new Step1(), "Step 1: Tune to S1 and set Reminders on Future Events on S2 and S3");
        this.AddStep(new Step2(), "Step 2: Wait for the Reminder Time of the First Reminded Event");
        this.AddStep(new Step3(), "Step 3: Accept the Reminder");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            FTA_1st_Mux_2 = CL.EA.GetValue("FTA_1st_Mux_2");


            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to S1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel S1");
            }

            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }

            if (timeToEventEnd_sec < 180)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            //  Reminders on Future Events on S2 and S3
            res = CL.EA.BookReminderFromGuide("Event2", FTA_1st_Mux_2, 1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Reminder from Guide on S2");
            }

            //Retuning to make sure exiting from guide didn't take us to focused channel S3
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel S1");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Wait for the Reminder Time of a Reminded Event [Start Time - 60 sec]
            res = CL.EA.WaitUntilReminder("Event2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait for Event2 Reminder Time");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Accept the Reminder. Verify Tuning to the Correct Channel Performed
            res = CL.EA.HandleReminder("Event2", EnumReminderActions.Accept);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Accept the Reminder for Event2");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Reminded Event2 Channel");
            }

            PassStep();

        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}
