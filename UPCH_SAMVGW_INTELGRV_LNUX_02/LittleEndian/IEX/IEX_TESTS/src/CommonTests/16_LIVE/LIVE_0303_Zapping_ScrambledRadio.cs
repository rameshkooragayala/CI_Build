/// <summary>
///  Script Name : LIVE_0303_Zapping_ScrambledRadio.cs
///  Test Name   : LIVE-0303-Scrambled Radio Service
///  TEST ID     : 61181
///  QC Version  : 1
///  Variation From QC : Req 177667 - performance measurement of channel zap is not tested
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>
///

using System;
using IEX.Tests.Engine;

public class LIVE_0303 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string ScrambledRadioService;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to a Scrambled Radio Service");

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
            ScrambledRadioService = CL.EA.GetValue("RADIO_SCRAMBLED");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Zap to a Radio Channel
        public override void Execute()
        {
            StartStep();
            res = CL.EA.TuneToRadioChannel(ScrambledRadioService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Radio Channel : " + ScrambledRadioService + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}