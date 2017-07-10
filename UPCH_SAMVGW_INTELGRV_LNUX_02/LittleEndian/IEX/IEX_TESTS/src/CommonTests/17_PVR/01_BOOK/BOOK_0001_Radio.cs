/// <summary>
///  Script Name : BOOK_0001_Radio.cs
///  Test Name   : BOOK-0001-Radio Service
///  TEST ID     : 60969
///  QC Version  : 5
///  Variations From QC:
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.Tests.Engine;

public class BOOK_0001 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Radio_Service_2;
    private static string Radio_Service_2_NAME;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Check That Can't Book Radio Channel From Live");
        this.AddStep(new Step2(), "Step 2: Check That Radio Channel Doesn't Appear in Planner Channel Line-Up");

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
            Radio_Service_2 = CL.EA.GetValue("Radio_Service_2");
            Radio_Service_2_NAME = CL.EA.GetValue("Radio_Service_2_NAME");

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

            res = CL.EA.TuneToRadioChannel(Radio_Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Radio Channel");
            }

            //Action menu shouldn't be displayed for Radio channel
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Action Bar Should Not Appear in Radio Channel");
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

            //Should not succeeded to booking MR from radio channel
            res = CL.EA.PVR.RecordManualFromPlanner("MR", Radio_Service_2_NAME);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Radio Channel " + Radio_Service_2_NAME + " Should Not Appear in Planner Channel List");
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