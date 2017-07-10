/// <summary>
///  Script Name        : AMS_0415_BookingRecurring_TimeBased.cs
///  Test Name          : AMS-0415-booking-recurring-time-base-reacurring
///  TEST ID            : 19481
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 7th NOV, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0415 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file and Book future Recurring event");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the booking for time event");

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
            CL.IEX.Wait(10);
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }
			
			string timeStamp = "";
            CL.IEX.SendIRCommand("MENU", -1, ref timeStamp);
			CL.IEX.Wait(10);
            string frequency=CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "FREQUENCY");
            if (frequency == "")
            {
                FailStep(CL,"Failed to get the Frequency from test ini");
            }
            EnumFrequency enumfrequency = (EnumFrequency)Enum.Parse(typeof(EnumFrequency), frequency, true);
            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED", Convert.ToInt32(Service_1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 10, DurationInMin: 1, VerifyBookingInPCAT: false, Frequency:enumfrequency);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
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

            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.BookingforTime, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags for booking for recording");
            }
            PassStep();
        }
    }

    #endregion Step1




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
