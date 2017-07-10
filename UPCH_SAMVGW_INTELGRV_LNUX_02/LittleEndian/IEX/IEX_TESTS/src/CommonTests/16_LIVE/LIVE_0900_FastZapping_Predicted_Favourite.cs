/// <summary>
///  Script Name : LIVE_0900_FastZapping_Predicted_Favourite.cs
///  Test Name   : LIVE-0900-Fast-Zapping-Predicted-Favourite
///  TEST ID     : 18675
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Madhukumar K
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0900 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service Service;

    private static Service Service1;

    private static Service Service2;

    private static Service Service3;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml file & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S1 After Standby");
        this.AddStep(new Step2(), "Step 2: Tune To Channel S2, Using Downwards Direction and verify the predicted milestone");
        this.AddStep(new Step3(), "Step 3: Tune To Channel S3, Using Downwards Direction and verify the predicted milestone");
        this.AddStep(new Step4(), "Step 4: Verify the same by tuning to up");

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

            //Get channels from Content XML
            Service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (Service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service.LCN);
            }
            Service1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + Service.LCN);
            if (Service1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service1.LCN);
            }
            Service2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + Service.LCN + "," + Service1.LCN);
            if (Service2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service2.LCN);
            }
            Service3 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + Service.LCN + "," + Service1.LCN + "," + Service2.LCN);
            if (Service3 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service3.LCN);
            }

            //Set the channel as Favourite List of fav channel
            res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + Service3.LCN + ", " + Service2.LCN + "," + Service1.LCN + "," + Service.LCN + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
            }

            //if DISABLE FAVOURITE MODE is present on the ACTION BAR,then no need to navigate to "STATE:ENABLE FAVOURITE MODE"
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
            if (res.CommandSucceeded)
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("DISABLE FAVOURITE MODE");
                if (res.CommandSucceeded)
                {
                    CL.IEX.LogComment("DISABLE FAVOURITE MODE is present on the ACTION BAR");
                }
                else
                {
                    res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ENABLE FAVOURITE MODE");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed to Set FAVOURITE mode");
                    }
                }
            }
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To Channel S4 After Standby
        public override void Execute()
        {
            StartStep();

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            // Tune to channel to start down surf from it
            CL.EA.ChannelSurf(EnumSurfIn.Live, "4");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Ignore, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Tune To Channel S3, Using Downwards Direction

        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        //Step 3: Tune To Channel S2, Using Downwards Direction

        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        //Step 4: Tune To Channel S1, Using Downwards Direction

        public override void Execute()
        {
            StartStep();
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            // Tune to channel to start down surf from it
            CL.EA.ChannelSurf(EnumSurfIn.Live, "4");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Ignore, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Pervious Perdicted Service");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        CL.IEX.Wait(5);

        res = CL.EA.STBSettings.UnsetAllFavChannels();
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to UnsetAllFavChannels");
        }
        CL.IEX.LogComment("Removed Favourite List");
    }

    #endregion PostExecute
}