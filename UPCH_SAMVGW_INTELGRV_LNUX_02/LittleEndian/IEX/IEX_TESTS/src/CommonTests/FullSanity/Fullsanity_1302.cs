using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_1302 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    //Channels used by the test
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    private static Service Service_5;
    private static string timeBasedRecording1 = "timeBasedRecording1";
    private static string timeBasedRecording2 = "timeBasedRecording2";
    private static string timeBasedRecording3 = "timeBasedRecording3";
    private static string timeBasedRecording4 = "timeBasedRecording4";
    private static string timeBasedRecording5 = "timeBasedRecording5";
    private static string timeBasedRecording6 = "timeBasedRecording6";
    private static string timeBasedRecording7 = "timeBasedRecording7";
    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Preconditions: Get Values from ini File & Book a few Recordings and raise a conflict");
        this.AddStep(new Step1(), "Step 1: Resolve the Conflict by cancelling the booking and verify other Bookings");
        this.AddStep(new Step2(), "Step 2: Again Raise conflict and choose to Automatically and verify the other Bookings");
        this.AddStep(new Step3(), "Step 3: Again Raise Conflict and choose manually and verify the other Bookings");

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

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }
            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_2.LCN);
            }
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_3.LCN);
            }
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_4.LCN);
            }
            Service_5 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN + "," + Service_4.LCN);
            if (Service_5 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_5.LCN);
            }
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording1, Convert.ToInt32(Service_1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 60, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording2, Convert.ToInt32(Service_2.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 58, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording3, Convert.ToInt32(Service_3.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 56, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording4, Convert.ToInt32(Service_4.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 54, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording5, Convert.ToInt32(Service_5.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 52, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false, IsConflict: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }

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
            res = CL.EA.PVR.ResolveConflict(timeBasedRecording5, "CANCEL RECORDING", VerifyInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resolve the conflict by selecting the Cancel recording");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording1, Navigate: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording2, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording3, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording4, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
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
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording6, Convert.ToInt32(Service_5.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 45, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false, IsConflict: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }
            res = CL.EA.PVR.ResolveConflict(timeBasedRecording6, "AUTOMATICALLY", VerifyInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resolve conflict");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording1, Navigate: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording2, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording3, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording6, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording4, Navigate: false, SupposedToFindEvent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
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
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording7, Convert.ToInt32(Service_4.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 40, DurationInMin: 60, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false, IsConflict: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }
            res = CL.EA.PVR.ResolveConflict(timeBasedRecording1, "MANUALLY", VerifyInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to resolve conflict");
            }

            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording2, Navigate: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording3, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording6, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording7, Navigate: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(timeBasedRecording1, Navigate: false, SupposedToFindEvent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the event in Planner");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to cancel all bookings from planner");
        }
    }
    #endregion
}