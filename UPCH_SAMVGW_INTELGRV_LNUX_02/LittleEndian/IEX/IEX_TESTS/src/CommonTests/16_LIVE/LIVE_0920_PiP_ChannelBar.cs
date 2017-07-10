/// <summary>
///  Script Name : LIVE_0920_PiP_ChannelBar.cs
///  Test Name   : LIVE-0920-PiP-Channel Bar
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0920 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Same_Schedule_1;
    private static string Same_Schedule_5;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Set Banner Display Time");
        this.AddStep(new Step1(), "Step 1: Tune to Channel S1 Using DCA");
        this.AddStep(new Step2(), "Step 2: Browse Current Events in Channel Bar  ");
        this.AddStep(new Step3(), "Step 3: Tune to Predicted Channel S2 ");
        this.AddStep(new Step4(), "Step 4: Again Browse Current Events in Channel Bar");
        this.AddStep(new Step5(), "Step 5: Browse Future Events in the Channel Bar and Tune to Predicted Channel S3");

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

            //Get Values From ini File
            Same_Schedule_1 = CL.EA.GetValue("Same_Schedule_1");
            Same_Schedule_5 = CL.EA.GetValue("Same_Schedule_5");

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

            //Tune to channel S5 with DCA in order to have next not predicted channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Same_Schedule_5, true, 1, EnumPredicted.Ignore);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            //Tune to channel S1 with DCA
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Same_Schedule_1, true, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on S1");
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

            //Open the Channel List and browse current events. Focus channel to S4.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 3, EnumPredicted.Predicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Channel Bar");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present in Background of Channel Bar Surfing");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on S1");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Open the Channel List and browse current events. Focused channel to S4.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 2, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Channel Bar a Second Time");
            }

            res  = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live");
            }

            PassStep();
        }
    }

    #endregion Step4

    #region Step5

    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelBarSurfFuture(true, 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Future Event in Channel Bar");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            PassStep();
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present in Background of Channel Bar Surfing");
            }
            PassStep();
        }
    }

    #endregion Step5

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}