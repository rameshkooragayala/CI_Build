/// <summary>
///  Script Name : LIVE_0902_FZ_Down.cs
///  Test Name   : LIVE-0902-Fast Zapping-Down
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0902 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_3;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S4 After Standby");
        this.AddStep(new Step2(), "Step 2: Tune To Channel S3, Using Downwards Direction");
        this.AddStep(new Step3(), "Step 3: Tune To Channel S2, Using Downwards Direction");
        this.AddStep(new Step4(), "Step 4: Tune To Channel S1, Using Downwards Direction");

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
            FTA_1st_Mux_3 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Service_num");
            if (String.IsNullOrEmpty(FTA_1st_Mux_3))
            {
                FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
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

            // Check that ther is a present video after standby
            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video Is Present After Standby");
            }
            // Tune to channel to start down surf from it
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + FTA_1st_Mux_3 + " With DCA");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Ignore, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Previous Not Perdicted Service");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Previous Perdicted Service");
            }
            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}