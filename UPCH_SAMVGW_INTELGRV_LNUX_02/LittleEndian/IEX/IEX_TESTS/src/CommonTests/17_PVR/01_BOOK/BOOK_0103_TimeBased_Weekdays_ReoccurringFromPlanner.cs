/// <summary>
///  Script Name : BOOK_0103_TimeBased_Weekdays_ReoccurringFromPlanner.cs
///  Test Name   : BOOK-0103-Time Based-Weekdays-Reoccurring From Planner
///  TEST ID     : 60967
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0103_Weekdays : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string ChannelName;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book a Daily Time-Based Recording on Planner");
       

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
            ChannelName = CL.EA.GetValue("Name_Overnight_EB1");

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
            res = CL.EA.PVR.RecordManualFromPlanner("MR1", ChannelName, "13:00", 60, 1, EnumFrequency.WEEKDAY, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording from planner");
            }
            res = CL.EA.ReturnToLiveViewing();
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