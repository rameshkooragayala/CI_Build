/// <summary>
///  Script Name : LIVE_0904_FZ_Radio.cs
///  Test Name   : LIVE-0904-Fast Zapping-Radio
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0904 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_4;
    private static string Radio_Service_2;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S1");
        this.AddStep(new Step2(), "Step 2: Tune To A Radio Channel ");
        this.AddStep(new Step3(), "Step 3: Tune To Another Radio Channel , Using Upwards Direction");

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
            Radio_Service_2 = CL.EA.GetValue("Radio_Service_2");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To Channel S1
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + FTA_1st_Mux_4 + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Tune To A Radio Channel

        public override void Execute()
        {
            StartStep();
            res = CL.EA.TuneToRadioChannel(Radio_Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The A Radio Channel");
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        //Step 3: Tune To Another Radio Channel , Using Upwards Direction

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Next Perdicted Radio Channel");
            }
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}