using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;

public class AMS_NON_IMI_REMINDER : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    static string ATLs;
    private static Service Service_1;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Fetch Channel Numbers from xml file & Sync");
        this.AddStep(new Step1(), "Step 1: Select next and select add reminder");
        this.AddStep(new Step2(), "Step 2: Wait for 10 minutes and varify AMS tags for CRID and IMI value for Non IMI Reminder");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion Create Structure

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to launch TV GUIDE");
            }

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, "Failed to set the Personalization to YES");
            }
            
            // No IMI and CRID event without IMI value
            string SINGLEEVENT_SERVICE_WITH_IMI = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_WITHOUTIMI");

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN=" + SINGLEEVENT_SERVICE_WITH_IMI, "ParentalRating=High");
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
                LogCommentWarning(CL, "Failed to tune to service " + Service_1.LCN);
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to set channel bar");
            }

            res = CL.IEX.MilestonesEPG.Navigate("ADD REMINDER");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select ADD REMINDER");
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

            CL.IEX.Wait(600);

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.REMINDER, commonVariable: "Reminder for without IMI");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify IMI, CRID and PROFILE ID without IMI");
            }
            else
            {
                LogCommentInfo(CL, "Verified AMS Tag for IMI, CRID and PROFILE ID without IMI");
            }

            PassStep();
        }
    }

    #endregion Step2

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

