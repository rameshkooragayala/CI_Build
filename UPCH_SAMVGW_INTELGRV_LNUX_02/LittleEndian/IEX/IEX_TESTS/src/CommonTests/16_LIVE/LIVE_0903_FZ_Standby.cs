/// <summary>
///  Script Name : LIVE_0903_FZ_Standby.cs
///  Test Name   : LIVE-0903-Fast Zapping-Standby
///  TEST ID     : 59039
///  QC Version  : 2
///  Variation From QC : performance of fast channel zap is not tested
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0903 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_4;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to a Service");
        this.AddStep(new Step2(), "Step 2: Tune to previous service by CH down");
        this.AddStep(new Step3(), "Step 3: Go to Standby and exit from Standby");
        this.AddStep(new Step4(), "Step 4: Tune to a Next service and validate fast zap");

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
            FTA_1st_Mux_4 = CL.EA.GetValue("FTA_1st_Mux_4");

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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + FTA_1st_Mux_4 + " With DCA");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.NotPredicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel Using Channel Down");
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

            //Enter Standy
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
            }

            CL.IEX.Wait(10);

            //Exit Standby
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
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

            //Tune to next service and validate fast zapping
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Next Service Using Channel Up and Validate Fast Zapping");
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