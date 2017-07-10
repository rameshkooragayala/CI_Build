/// <summary>
///  Script Name        : AMS_0509_RBPlayback_MinChangeSpeed.cs
///  Test Name          : AMS-0509-Start-RB-Playback-Event-minimum-change speed, AMS-0102-MLT-CHAN-BAR-RB, AMS-0115-MLT-ACTION MENU-RB
///  TEST ID            : 20744
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 11th NOV, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0509 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static string evtName = "";
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file, collect RB and set the speed to 30");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the playback event with speed 30");

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

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN=9", "ParentalRating=High");
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
                FailStep(CL,res,"Failed to tune to service "+Service_1.LCN);
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info for event name");
            }
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Trick mode speed to 0");
            }
            CL.IEX.Wait(180);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 1, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Trick mode speed to 0");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to navigate to CHANNEL BAR MORE LIKE THIS");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to navigate to ACTION BAR MORE LIKE THIS");
            }
            CL.IEX.Wait(10);
            string timeStamp = "";
            CL.IEX.SendIRCommand("MENU", -1, ref timeStamp);
            CL.IEX.Wait(5);
            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Trick mode speed to 0");
            }

            CL.IEX.Wait(10);
            CL.EA.UI.Utils.SendIR("FF", WaitAfterIR: 0);
            CL.EA.UI.Utils.SendIR("FF", WaitAfterIR: 0);
            CL.EA.UI.Utils.SendIR("FF", WaitAfterIR: 0);
            CL.EA.UI.Utils.SendIR("FF", WaitAfterIR: 0);
            CL.EA.UI.Utils.SendIR("FF", WaitAfterIR: 0);

            CL.IEX.Wait(3);
            res = CL.EA.PVR.StopPlayback(IsReviewBuffer: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop the Playback fromRB");
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

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 2000);
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to verify 2 which is expected");
            }
            else
            {
                FailStep(CL,"Found Playback event with speed 2 which is not expected");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 6000);
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to verify 6 which is expected");
            }
            else
            {
                FailStep(CL, "Found Playback event with speed 6 which is not expected");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 12000);
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to verify 12 which is expected");
            }
            else
            {
                FailStep(CL, "Found Playback event with speed 12 which is not expected");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "true", Speed: 30000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify the AMS tag for Playback speed event with speed 30");
            }
            else
            {
                LogCommentImportant(CL, "Found Playback event with speed 30");
            }
            CL.IEX.Wait(10);
			//According to the new FT change we will be getting the LTV on LIVE
            evtName = "LTV " + evtName;
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.CHANNEL_BAR, service: Service_1, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.ACTION_MENU, service: Service_1, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.MORE_LIKE_THIS, service: Service_1, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            PassStep();
        }
    }

    #endregion Step1




    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {


    }

    #endregion PostExecute
}
