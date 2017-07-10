/// <summary>
///  Script Name : LIB_0031_Book_DeletePastRec.cs
///  Test Name   : LIB-0031-Book-Delete Past Recording
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0031 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Name_For_Finished_Booking;
    private static string Name_For_OnGoing_Booking;
    private static string Channel_For_Finished_Booking;
    private static string Channel_For_OnGoing_Booking;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File , Sync And Book Events");
        this.AddStep(new Step1(), "Step 1: Delete Finished EB Recording");
        this.AddStep(new Step2(), "Step 2: Go To Delete Finished TB Recording And Dont Delete");
        this.AddStep(new Step3(), "Step 3: Stop on-going  TB Recording");
        this.AddStep(new Step4(), "Step 4: Delete Stoped TB Recording");

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
            CL.IEX.LogComment("Retrieving : Name_For_Finished_Booking ");
            Name_For_Finished_Booking = CL.EA.GetValue("Name_FTA_1st_Mux_1");

            CL.IEX.LogComment("Retrieving : Name_For_OnGoing_Booking ");
            Name_For_OnGoing_Booking = CL.EA.GetValue("Name_FTA_1st_Mux_2");

            CL.IEX.LogComment("Retrieving : Channel_For_Finished_Booking");
            Channel_For_Finished_Booking = CL.EA.GetValue("Short_SD_1");

            CL.IEX.LogComment("Retrieving : Channel_For_OnGoing_Booking ");
            Channel_For_OnGoing_Booking = CL.EA.GetValue("Long_SD_2");

            string egtDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
            if (string.IsNullOrEmpty(egtDefault))
            {
                FailStep(CL, res, "Failed to fetch the Default EGT value from project ini");
            }


            // Time Based Recordings
            res = CL.EA.PVR.RecordManualFromPlanner("Finished_MR", Name_For_Finished_Booking, -1, 3, 1, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book First Manual Recording");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("OnGoing_MR", Name_For_OnGoing_Booking, -1, 4, 30, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Secend Manual Recording");
            }

            // Event Based Recordings
            res = CL.EA.TuneToChannel(Channel_For_Finished_Booking);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune To Channel");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("Finished_EB", -1, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event From Banner");
            }

            res = CL.EA.TuneToChannel(Channel_For_OnGoing_Booking);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune To Channel");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("OnGoing_EB", 7, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event From Banner");
            }

            res = CL.EA.WaitUntilEventEnds("Finished_EB",egtDefault);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Finished_EB Ends");
            }

            res = CL.EA.WaitUntilEventEnds("Finished_MR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Finished_MR Ends");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Delete Finished EB Recording
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.DeleteRecordFromArchive("Finished_EB", false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Delete Record From Archive Finished_EB");
            }

            res = CL.EA.PVR.VerifyEventInArchive("Finished_EB", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Delete Record From Archive Finished_EB");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        // Navigate and select  Finished TB
        //Go to action menu -> delete and select no
        //verify that event still present on recording list
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.VerifyEventInArchive("Finished_MR", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording Finished_TB on Archive");
            }

            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR/DELETE/NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Delete Menu And Select NO");
            }

            res = CL.EA.PVR.VerifyEventInArchive("Finished_MR", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Recording Finished_TB on Archive After Not Deleting it");
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    public class Step3 : _Step
    {
        //stop on going time baseed recording
        // verify partial state on pcat (on failing if ont due to pcat problens )
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.StopRecordingFromArchive("OnGoing_MR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording On Going Time Based");
            }

            res = CL.EA.VerifyEventDuration("OnGoing_MR", 30, false);
            //res = CL.EA.PCAT.VerifyEventPartialStatus("OnGoing_MR", "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Verify Partial State Of recording on PCAT", false);
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

            res = CL.EA.PVR.DeleteRecordFromArchive("OnGoing_MR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Delete Recording From Archive ");
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoing_MR", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Delete Recording From Archive (OnGoing_MR) Was Faund In Archive ");
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