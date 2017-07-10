using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;

public class AMS_IMI_REMINDER : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    static string ATLs;
    private static Service Service_1;
    static int COUNT_ActionBar;
    static int COUNT_EPISODE;

    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch Channel Numbers from xml file & Sync");
        this.AddStep(new Step1(), "Step 1: Select next and select add reminder");
        this.AddStep(new Step2(), "Step 2: Wait for 10 minutes and varify AMS tags for CRID and IMI value");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion Create Structure

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
            string SINGLEEVENT_SERVICE_WITH_IMI = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_WITH_IMI");

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

            COUNT_ActionBar = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "COUNT_ACTION_BAR_COUNT"));

            COUNT_EPISODE = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "COUNT_EPISODE"));

            PassStep();
        }
    }

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Reminder for single channel with IMI

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to set channel bar");
            }

            string testReminderwithIMI = string.Empty;
            int cntActionBar = 0;
            
            while (testReminderwithIMI != "ADD REMINDER" && cntActionBar < COUNT_ActionBar)
            {
                CL.EA.UI.Utils.SendIR("SELECT_DOWN");
                CL.IEX.Wait(2);
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out testReminderwithIMI);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Get the title");
                }
                cntActionBar++;
            }
            if (testReminderwithIMI == "ADD REMINDER")
            {
                CL.EA.UI.Utils.SendIR("SELECT");
                CL.IEX.Wait(2);

                string testSingleEpisode = string.Empty;
                int cntEpisode = 0;
                
                while (testSingleEpisode != "THIS EPISODE" && cntEpisode < COUNT_EPISODE)
                {
                    CL.EA.UI.Utils.SendIR("SELECT_DOWN");
                    CL.IEX.Wait(2);
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out testSingleEpisode);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set reminder for this episode");
                    }
                    cntEpisode++;
                }

                if (testSingleEpisode == "THIS EPISODE")
                {
                    CL.EA.UI.Utils.SendIR("SELECT");
                    CL.IEX.Wait(2);
                }
            }
            else
            {
                FailStep(CL, res, "Failed to Get the ADD REMINDER");
            }

            // Reminder for series channel with IMI

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to set channel bar");
            }

            string testReminderwitIMI = string.Empty;
            int cntActBar = 0;
            while (testReminderwitIMI != "ADD REMINDER" && cntActBar < COUNT_ActionBar)
            {
                CL.EA.UI.Utils.SendIR("SELECT_DOWN");
                CL.IEX.Wait(2);
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out testReminderwitIMI);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Get the title");
                }
                cntActBar++;
            }
            if (testReminderwitIMI == "ADD REMINDER")
            {
                CL.EA.UI.Utils.SendIR("SELECT");
                CL.IEX.Wait(2);

                string testSeriesEpisode = string.Empty;
                int cntsEpisode = 0;
                while (testSeriesEpisode != "ENTIRE SERIES" && cntsEpisode < COUNT_EPISODE)
                {
                    CL.EA.UI.Utils.SendIR("SELECT_DOWN");
                    CL.IEX.Wait(2);
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out testSeriesEpisode);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to set reminder for this episode");
                    }
                    cntsEpisode++;
                }

                if (testSeriesEpisode == "ENTIRE SERIES")
                {
                    CL.EA.UI.Utils.SendIR("SELECT");
                    CL.IEX.Wait(2);
                }
            }
            else
            {
                LogCommentInfo(CL, "Failed to Get the ADD REMINDER for reminder for series");
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

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.REMINDER, commonVariable: "Reminder with IMI");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify IMI, CRID and PROFILE ID with IMI");
            }
            else
            {
                LogCommentInfo(CL, "Verified AMS Tag for IMI, CRID and PROFILE ID with IMI");
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

