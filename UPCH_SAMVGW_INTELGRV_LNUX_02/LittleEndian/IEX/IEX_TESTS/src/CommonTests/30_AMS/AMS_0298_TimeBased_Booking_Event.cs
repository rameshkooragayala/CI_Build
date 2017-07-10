/// <summary>
///  Script Name        : AMS_0298_TimeBased_Booking_Event.cs
///  Test Name          : AMS-0298-Time-Based-Booking-Event
///  TEST ID            : 23466
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 27th OCT, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0298 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers from XML file and Book a future Time Based Recording");
        this.AddStep(new Step1(), "Step 1: Verify the AMS log file for Booking For Time Event");
        this.AddStep(new Step2(), "Step 2: Wait until the event starts and wait until the recording is completed");
        this.AddStep(new Step3(), "Step 3: Verify the AMS log file for Recording Event");
        this.AddStep(new Step4(), "Step 4: Delete record from Archive and verify the AMS tags");
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
           // if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
           // {
           //     FailStep(CL, res, "Failed to set the Personalization to YES");
           // }

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_1.LCN);
            }
            res = CL.EA.PVR.RecordManualFromPlanner("TIMEBASED",Convert.ToInt32(Service_1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 15, DurationInMin: 10, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record Manual from Planner");
            }
            CL.IEX.Wait(600);

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
           res= CL.EA.VerifyAMSTags(EnumAMSEvent.BookingforTime, service: Service_1);
           if (!res.CommandSucceeded)
           {
               FailStep(CL,res,"Failed to verify the AMS tags for Booking for time");
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
            res = CL.EA.WaitUntilEventEnds("TIMEBASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event starts");
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
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.RecordingEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags for Booking for time");
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
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                FailStep(CL, res, "Failed to set the Personalization to NO");
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }
           res= CL.EA.PVR.DeleteRecordFromArchive("TIMEBASED",VerifyDeletedInPCAT:false);
           if (!res.CommandSucceeded)
           {
               FailStep(CL,res,"Failed to delete record from Archive");
           }
           CL.IEX.Wait(600);
           res = CL.EA.VerifyAMSTags(EnumAMSEvent.DeletionEvent, service: Service_1);
           if (!res.CommandSucceeded)
           {
               FailStep(CL, res, "Failed to verify the AMS tags for Booking for time");
           }
            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }

    }

    #endregion PostExecute
}
