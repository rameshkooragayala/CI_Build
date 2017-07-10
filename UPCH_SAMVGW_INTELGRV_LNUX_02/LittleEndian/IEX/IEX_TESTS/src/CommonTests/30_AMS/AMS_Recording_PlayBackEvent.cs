/// <summary>
///  Script Name        : AMS_Recording_PlayBackEvent.cs
///  Test Name          : AMS-0426-recording value, AMS-0501-Start-Playback-Event, AMS-0502-Playback-Speed-Event
///  TEST ID            : 20675, 15580, 15807
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

public class AMS_Recording_PlayBackEvent : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static Service Service_2;
    private static string timeBasedRecording = "TIME_BASED";

    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch Service from content xml and record a time based recording");
        this.AddStep(new Step1(), "Step 1: Set the Personalized recommendation Activation to true");
        this.AddStep(new Step2(), "Step 2: Playback the Recording and Set different trick mode speeds and verify the AMS Playback tags");
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
                FailStep(CL,res, "Failed to get channel number from ContentXML");
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
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_2.LCN);
            }
			string timeStamp = "";
            CL.IEX.SendIRCommand("MENU", -1, ref timeStamp);
			CL.IEX.Wait(10);
			
            res = CL.EA.PVR.RecordManualFromPlanner(timeBasedRecording,Convert.ToInt32(Service_1.LCN),DaysDelay: -1, MinutesDelayUntilBegining: 5, DurationInMin: 10, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on " + Service_1.LCN);
            }
            res = CL.EA.WaitUntilEventStarts(timeBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event starts");

            }
             CL.IEX.Wait(600);
             res = CL.EA.VerifyAMSTags(EnumAMSEvent.BookingforTime, service: Service_1);
             if (!res.CommandSucceeded)
             {
                 FailStep(CL, res, "Failed to verify the Booking foe time tags");
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
            res = CL.EA.WaitUntilEventEnds(timeBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until Event Ends");
            }

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                FailStep(CL, res, "Failed to set the Personalization to No");
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
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
            res = CL.EA.PVR.PlaybackRecFromArchive(timeBasedRecording, SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Record From Archive");
            }

            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedRecording, Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 30");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedRecording, Speed: -30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to -30");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedRecording, Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 0");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed(timeBasedRecording, Speed: 1, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 1");
            }
            CL.IEX.Wait(10);

            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the Playback");
            }

            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: 0);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify 0");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: 1000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify 1");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: 2000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify 2");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: -2000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify -2");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: 6000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify 6");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: -6000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify -6");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: 12000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify 12");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: -12000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify -12000");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: 30000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify 30000");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "false", Speed: -30000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify -30000");
            }
			
            PassStep();
        }
    }

    #endregion Step2



    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all records from Archive");
        }
        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }

    }

    #endregion PostExecute
}
