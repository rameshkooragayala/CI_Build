﻿/// <summary>
///  Script Name        : AMS_0002_No_SurfEvent
///  Test Name          : AMS_0002_No_SurfEvent
///  TEST ID            : 15576
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 31st March, 2015
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0002_No_SurfEvent : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static Service Service_2;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from content xml");
        this.AddStep(new Step1(), "Step 1: Do channel surf and wait for minimum duration and verify only Live viewing event is present");
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

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_2.LCN);
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_1.LCN);
            }
            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }
            CL.IEX.Wait(540);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Surfing Event");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SurfingEvent);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Verified Surfing Event after minimum Live duration which is not supposed to happen");
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
