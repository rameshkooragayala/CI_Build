/// <summary>
///  Script Name        : AMS_0297_Programme_BookingEvent.cs
///  Test Name          : AMS-0297-Programme-Booking-Event
///  TEST ID            : 23464
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 31st OCT, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0297 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static string StartGuardTimeName;
    //Channels used by the test

    private static Service Service_1;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers from XML file and Book a future Event Based Recording");
        this.AddStep(new Step1(), "Step 1: Verify the AMS log file for Booking For Recording Event");
        this.AddStep(new Step2(), "Step 2: Wait until the event starts and stop recording");
        this.AddStep(new Step3(), "Step 3: Verify the AMS log file for Recording Event");
        this.AddStep(new Step4(), "Step 4: Delete the Recording and Verify the Deletion AMS event");
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

             StartGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (StartGuardTimeName == null)
            {
                LogCommentFailure(CL, "Failed to get SGT MIN value from Project INI file");
            }

            res = CL.EA.STBSettings.SetGuardTime(true, StartGuardTimeName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to Set Guard time");
            }

           string endGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MIN");
            if (endGuardTimeName == null)
            {
                LogCommentFailure(CL, "Failed to get EGT MIN value from Project INI file");
            }

            res = CL.EA.STBSettings.SetGuardTime(false, endGuardTimeName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Set Guard time");
            }

            string serviceLCN = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_NUMBER");
            if (serviceLCN == "")
            {
                FailStep(CL, "Failed to get the SERVICE_NUMBER from Test ini");
            }

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN="+serviceLCN, "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to tune to sercice "+Service_1.LCN);
            }
            res = CL.EA.PVR.BookFutureEventFromBanner("EVENT_RECORDING", MinTimeBeforeEvStart: 4, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to book Future event from banner");
            }
            CL.IEX.Wait(420);

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
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.BookingforRecording, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags for Booking for time");
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
            res = CL.EA.WaitUntilEventStarts("EVENT_RECORDING",StartGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event Ends");
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                FailStep(CL, res, "Failed to set the Personalization to NO");
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }

            CL.IEX.Wait(120);
            res = CL.EA.PVR.StopRecordingFromArchive("EVENT_RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop the recording from Archive");
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
            CL.IEX.Wait(500);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.RecordingEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags for Recording Event");
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
            res = CL.EA.PVR.DeleteRecordFromArchive("EVENT_RECORDING",VerifyDeletedInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to delete record from Archive");
            }
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.DeletionEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags for Deletion Event");
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
