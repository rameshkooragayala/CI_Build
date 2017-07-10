/// <summary>
///  Script Name : LIB_0043_Book_RefreshVideoRecordingList
///  Test Name   : LIB-0043-Book-Refresh Video Recording List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0043 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string time_based_channel;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book a Future Time-Based Recording on LIVE");
        this.AddStep(new Step2(), "Step 2: Make Sure Recording Is Not In Archive, Wait Until Recording Ends");
        this.AddStep(new Step3(), "Step 3: Refresh Recording List And Make Sure Recording Is Found In Archive");

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
            time_based_channel = CL.EA.GetValue("NAME_FTA_1st_Mux_4");

            //In order Library will not be empty
            res = CL.EA.PVR.RecordCurrentEventFromBanner("ER1", -1, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

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

            //book future time based recording
            res = CL.EA.PVR.RecordManualFromPlanner("MR1", time_based_channel, -1, 5, 5, EnumFrequency.ONE_TIME, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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

            res = CL.EA.PVR.VerifyEventInArchive("MR1", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Found Event in Archive (Event Should Not be There)");
            }

            //Wait until recording ends
            CL.IEX.Wait(310);

            res = CL.EA.ReturnToLiveViewing(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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

            //re navigate to LIBRARY and check recording again
            res = CL.EA.PVR.VerifyEventInArchive("MR1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event in archive");
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