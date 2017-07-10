/// <summary>
///  Script Name : LIB_0051_Book_DeleteAllInRecordingsList.cs
///  Test Name   : LIB-0051-Book-Delete All In Recordings List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0051 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_1;
    private static string FTA_1st_Mux_3;
    private static string FTA_1st_Mux_4;
    private static string FTA_1st_Mux_5;
    private static string FTA_1st_Mux_6;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Booking event and time based recordings, that are past and current and complete and partial");
        this.AddStep(new Step2(), "Step 2: Verify that all recording in my recording list");
        this.AddStep(new Step3(), "Step 3: Delete all from my recording list");
        this.AddStep(new Step4(), "Step 4: Verify that all recoding delete from my recording list");

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
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            FTA_1st_Mux_4 = CL.EA.GetValue("FTA_1st_Mux_4");
            FTA_1st_Mux_5 = CL.EA.GetValue("FTA_1st_Mux_5");
            FTA_1st_Mux_6 = CL.EA.GetValue("FTA_1st_Mux_6");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //step 1 book multiple kind of booking
        public override void Execute()
        {
            StartStep();

            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel With DCA");
                }

                res = CL.EA.PVR.RecordManualFromCurrent("TimeBaseFromLive", FTA_1st_Mux_1, 3, EnumFrequency.ONE_TIME, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to book time base from Live");
                }
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TimeBaseFromPlanner", Int32.Parse(FTA_1st_Mux_3), -1, 2, 3, EnumFrequency.ONE_TIME, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book time base from planner");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBaseFromeBanner", 3, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book event from banner");
            }

            res = CL.EA.PVR.StopRecordingFromBanner("EventBaseFromeBanner");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop event from banner");
            }

            //wait for event TimeBaseFromPlanner end for aovid to getting conflict
            CL.IEX.Wait(180);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("OnGoingEventBaseFromBanner", 5, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book event from banner");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("OnGoingTimeBaseFromLive", Int32.Parse(FTA_1st_Mux_6), -1, -1, 30, EnumFrequency.ONE_TIME, false, false);
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

            //verify that all recorded event in my recording
            res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromPlanner", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify event Time Base From Planner in my recording");
            }
            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromLive", false, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify event Time Base From Live in my recording");
                }
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventBaseFromeBanner", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Event Base Frome Banner in my recording");
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingEventBaseFromBanner", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify OnGoing Event Base From Banner in my recording");
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingTimeBaseFromLive", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify OnGoing event Time Base From Live in my recording");
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
            //delete all recording from my recording
            StartStep();

            res = CL.EA.PVR.DeleteAllRecordsFromArchive(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to delete all recording from my recording");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to Live viewing");
            }
            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //verify that all recording deleted
            res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromPlanner", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Found Time Based Recording (From Planner) In Archive After Deletion");
            }
            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromLive", false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed: Found Time Based Recording (From Live) In Archive After Deletion");
                }
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventBaseFromeBanner", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Found Event Based Recording (From Banner) In Archive After Deletion");
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingEventBaseFromBanner", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Found Event Based Recording In Archive After Deletion");
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingTimeBaseFromLive", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Found Time Based Recording In Archive After Deletion");
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