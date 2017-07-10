/// <summary>
///  Script Name        : AMS_MLT_CHbar_PIP_PCEvent.cs
///  Test Name          : AMS-0105-MLT-CHAN-BAR-PIP
///  TEST ID            : 
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 20th March, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0105 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static Service Service_2;
    private static string evtName = "";
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file and Sync");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the events");

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

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_2.LCN);
            }
            
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._15);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to set the Banner Display Timeout to 15");
            }
            //Tune to a service and wait for few seconds
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_1.LCN);
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", DoTune: true,NumberOfPresses:2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", IsNext: false, NumberOfPresses: 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to Service ");
            }
            res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to Navigate to MORE LIKE THIS");
            }
            CL.IEX.Wait(5);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info for event name");
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

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.CHANNEL_BAR, service: Service_2, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Channel bar event");
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
