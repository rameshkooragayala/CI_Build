/// <summary>
///  Script Name        : AMS_0002_RB_SurfEvent.cs
///  Test Name          : AMS-0002-Surf-Event
///  TEST ID            : 15576
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

public class AMS_0002_RB_SurfEvent : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from content xml and Verify the tags");
        this.AddStep(new Step1(), "Step 1: Stop Playback and channel change before Minimum duration and verify the tag");
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

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_2.LCN);
            }
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_3.LCN);
            }
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_2.LCN + "," + Service_3.LCN);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_4.LCN);
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

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to return to Live viewing");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 0");
            }
            CL.IEX.Wait(60);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 1, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the trick mode speed to 0");
            }
            res = CL.EA.PVR.StopPlayback(IsReviewBuffer: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback from Review buffer");
            }
            CL.IEX.Wait(5);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }
            CL.IEX.Wait(5);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_3.LCN);
            }
            CL.IEX.Wait(5);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_4.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_4.LCN);
            }
            CL.IEX.Wait(540);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SurfingEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Surfing Event");
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
