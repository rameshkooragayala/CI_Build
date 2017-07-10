/// <summary>
///  Script Name : LIVE_0907_FZ_RB.cs
///  Test Name   : LIVE-0907-Fast Zapping-RB
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0907 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_3;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S1 After Standby Ans Start RB PLB");
        this.AddStep(new Step2(), "Step 2: Tune To Channel S2, Using Upwards Direction");

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
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To Channel S1 After Standby AND Start RB PLB
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + FTA_1st_Mux_3 + " With DCA");
            }

            // Pause Live Viewing
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }

            // Wait 20 Seconds
            int Test_Wait = 20;
            CL.IEX.Wait(Test_Wait);

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 3: Tune To Channel S3, Using Upwards Direction

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, DoTune: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Next Perdicted Service");
            }
            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}