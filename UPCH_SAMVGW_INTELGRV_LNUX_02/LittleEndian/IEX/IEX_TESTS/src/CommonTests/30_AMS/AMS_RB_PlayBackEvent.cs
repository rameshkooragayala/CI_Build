/// <summary>
///  Script Name        : AMS_RB_PlayBackEvent.cs
///  Test Name          : AMS-0505-Start-RB-Playback-Event,AMS-0510-RB-Playback-Speed-Event
///  TEST ID            : 15984,15985
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 28th OCT, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_RB_PlayBackEvent : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;

    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition:Fetch Service from content xml and collect RB");
        this.AddStep(new Step1(), "Step 1: Set the Personalized recommendation Activation to true");
        this.AddStep(new Step2(), "Step 2: Playback the RB and Set different trick mode speeds and verify the AMS RB tags");
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
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
            }
            CL.IEX.Wait(15);
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
            }
            CL.IEX.Wait(15);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service " + Service_1.LCN);
            }
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to 0");
            }
            //Collecting RB
            CL.IEX.Wait(600);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 1, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to 0");
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

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                FailStep(CL, "Failed to set the Personalization to No");
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, "Failed to set the Personalization to YES");
            }
			
			string timeStamp="";
			
            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);

            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);

            CL.IEX.Wait(10);
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

            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to 30");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: -30, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set the trick mode speed to -30");
            }
            CL.IEX.Wait(4);
            res = CL.EA.PVR.StopPlayback(IsReviewBuffer: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop the Playback");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 0");
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 1, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 1");
            }
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 0);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Verify 0");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 1000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify 1");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 2000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify 2");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: -2000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify -2");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 6000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify 6");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: -6000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify -6");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 12000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify 12");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: -12000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify -12000");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 30000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify 30000");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: -30000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify -30000");
            }


            PassStep();
        }
    }

    #endregion Step2



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
