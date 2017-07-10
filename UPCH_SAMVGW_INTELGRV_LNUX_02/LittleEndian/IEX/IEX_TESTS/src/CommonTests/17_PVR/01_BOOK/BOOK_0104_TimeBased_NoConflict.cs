/// <summary>
///  Script Name : BOOK_0104_TimeBased_NoConflict.cs
///  Test Name   : BOOK-0104-Time Based-No Conflict
///  TEST ID     : 61253
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0104 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string channelName;
    private static string[] channelsArray;
    private static string conflict;
    private static int conflictNumber;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Book Maximum Time Based Recordings From Planner");
        this.AddStep(new Step2(), "Verify Recordings in Planner");

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
            channelName = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "SERVICE_LIST");
            channelsArray = channelName.Split('_');
            conflict = CL.EA.GetValueFromINI(EnumINIFile.Project, "FEATURES", "SIMULTANEOUS_RECORDINGS");
            conflictNumber = Int32.Parse(conflict);

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
            for (int i = 0; i < conflictNumber; i++)
            {
                res = CL.EA.PVR.RecordManualFromPlanner("MR" + i, Int32.Parse(channelsArray[i]), "13:00", 60, 1, EnumFrequency.ONE_TIME, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book Manual Recording From Planner");
                }
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

            for (int i = 0; i < conflictNumber; i++)
            {
                res = CL.EA.PVR.VerifyEventInPlanner("MR" + i, true, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Find Manual Recording in Planner");
                }
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