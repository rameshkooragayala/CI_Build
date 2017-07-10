/// <summary>
///  Script Name : LIVE_0921_PiP_Guide.cs
///  Test Name   : LIVE-0921-PiP-Guide
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0921 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_2;
    private static string FTA_1st_Mux_5;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Set Banner Display Time");
        this.AddStep(new Step1(), "Step 1: Tune to Channel S1 Using DCA");
        this.AddStep(new Step2(), "Step 2: Browse  Current Events in Service S4 in Guide");
        this.AddStep(new Step3(), "Step 3: Close Guide & Tune to a Predicted Channel ");
        // step 4 is commented , since the main goal is to check in logs that predicted channel is prepared, which can not be done in IEX
        //this.AddStep(new Step4(), "Step 4: Again Browse Current Events in Service S4 in Guide");
        this.AddStep(new Step5(), "Step 5: Browse Future Events in Guide and Tune to a Non Predicted Channel S3");

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
            FTA_1st_Mux_2 = CL.EA.GetValue("FTA_1st_Mux_2");
            FTA_1st_Mux_5 = CL.EA.GetValue("FTA_1st_Mux_5");

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

            //Tune to channel S1 with DCA
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_2, true, 1, EnumPredicted.Ignore);
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
            // browse events on S4
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, FTA_1st_Mux_5, false, 3, EnumPredicted.Ignore);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to browse events on Channel S4 in Guide");
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

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Guide Events Surfing");
            }
            // wait in order to prepare the predicted channel
            CL.IEX.Wait(10);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, FTA_1st_Mux_5, true, 2, EnumPredicted.Ignore, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to browse events on Channel S4 in Guide");
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
            // browse future events in S4
            res = CL.EA.BrowseGuideFuture(true, 3, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf Future Events in Channel Bar");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Channel Bar Future Events Surfing");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
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