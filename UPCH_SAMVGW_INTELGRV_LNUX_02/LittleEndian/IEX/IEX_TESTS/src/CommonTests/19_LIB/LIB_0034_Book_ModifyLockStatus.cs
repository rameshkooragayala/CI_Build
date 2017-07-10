/// <summary>
///  Script Name : LIB_0034_Book_ModifyLockStatus.cs
///  Test Name   : LIB-0034-Book-Modify Lock Status
///  TEST ID     : 61970
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.Tests.Engine;

//Common - LIB-0034-Modify lock status of event-based recording
public class LIB_0034 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string _1st_RecordingChannel;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book future event, Verify Event On Planner");
        this.AddStep(new Step2(), "Step 2: Perform lock, verify event is lock in pcat");
        this.AddStep(new Step3(), "Step 3: Perform unlock, verify event is unlock in pcat");
        this.AddStep(new Step4(), "Step 4: Perform keep, verify event is keep in pcat, verify there is only one event on planner ");

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
            CL.IEX.LogComment("Retrieving : _1st_RecordingChannel");
            _1st_RecordingChannel = CL.EA.GetValue("Long_SD_1");

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

            res = CL.EA.TuneToChannel(_1st_RecordingChannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune To Channel");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide("EB_1", _1st_RecordingChannel, 3, 5, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EB_1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_1");
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

            res = CL.EA.PVR.LockEventFromPlanner("EB_1", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Lock Event From Planner EB_1");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.UnLockEventFromPlanner("EB_1", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  UnLock Event From Planner EB_1");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.LockEventFromPlanner("EB_1", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Lock Event From Planner EB_1");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EB_1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  Verify Event In Planner: EB_1");
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