/// <summary>
///  Script Name        : AMS_MLT_ActioMenu_Next_NoInfo
///  Test Name          : AMS-0112-MLT-ACTION MENU-LIVE-NO PROG, AMS-0111-MLT-ACTION MENU-LIVE-NEXT
///  TEST ID            : 
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 11th March, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_MLT_ActioMenu_Next_NoInfo : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static Service Service_2;
    private static string evtName1 = "";
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

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=False", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_2.LCN);
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to tune to service " + Service_1.LCN);
            }



            CL.IEX.Wait(10);
            CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            CL.IEX.Wait(20);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtName1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info for event name");
            }

            res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to ACTION BAR MORE LIKE THIS from Channel bar Next");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL,"Failed to tune to Service "+Service_2.LCN);
            }
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to Launch Action Menu");
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

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.ACTION_MENU, service: Service_2, commonVariable: evtName1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Action Menu event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.MORE_LIKE_THIS, service: Service_2, commonVariable: evtName1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the MLT event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.ACTION_MENU, service: Service_2, commonVariable: "_UNKNOWN_TITLE_");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the MLT event");
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
