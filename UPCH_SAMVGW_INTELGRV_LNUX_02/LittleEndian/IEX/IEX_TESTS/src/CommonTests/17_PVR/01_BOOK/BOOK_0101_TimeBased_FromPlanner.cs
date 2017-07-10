/// <summary>
///  Script Name : BOOK_0101_TimeBased_FromPlanner.cs
///  Test Name   : BOOK-0101-Time Based-From Planner
///  TEST ID     : 60964
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0101 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string ChannelName;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Book a Time-Based Recording From Planner");
        this.AddStep(new Step2(), "Step 2: Verify Event in Planner");

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
            ChannelName = CL.EA.GetTestParams("SERVICE_TYPE");

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

            //Book a time-based recording on planner
            res = CL.EA.PVR.RecordManualFromPlanner("MR1", ChannelName, 1, -1, 60, EnumFrequency.ONE_TIME, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording From planner");
            }
            res = CL.EA.ReturnToLiveViewing();
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

            res = CL.EA.PVR.VerifyEventInPlanner("MR1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Event in Planner");
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