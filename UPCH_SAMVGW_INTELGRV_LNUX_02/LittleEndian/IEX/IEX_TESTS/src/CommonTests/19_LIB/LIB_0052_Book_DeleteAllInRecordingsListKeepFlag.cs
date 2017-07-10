/// <summary>
///  Script Name : LIB_0052_Book_DeleteAllInRecordingsListKeepFlag
///  Test Name   : LIB-0052-Book-Delete All In Recordings List Keep Flag
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0052 : _Test
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
        this.AddStep(new Step1(), "Step 1: Booking event and time based recordings, that are past, current, complete and partial");
        this.AddStep(new Step2(), "Step 2: Verify that all recording in my recording list");
        this.AddStep(new Step3(), "Step 3: Set keep flag to event base recording from my recording list");
        this.AddStep(new Step4(), "Step 4: Delete all from my recording list");
        this.AddStep(new Step5(), "Step 5: Verify that all recoding delete from my recording list exept event base recording with keep flag");

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
        public override void Execute()
        {
            StartStep();

            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                res = CL.EA.PVR.RecordManualFromCurrent("TimeBaseFromLive", FTA_1st_Mux_1, 3, EnumFrequency.ONE_TIME, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TimeBaseFromPlanner", Int32.Parse(FTA_1st_Mux_3), -1, 2, 3, EnumFrequency.ONE_TIME, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBaseFromeBanner", 3, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.StopRecordingFromBanner("EventBaseFromeBanner");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //wait for event TimeBaseFromPlanner end for aovid conflict
            CL.IEX.Wait(180);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("OnGoingEventBaseFromBanner", 5, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_6);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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
                FailStep(CL, res);
            }

            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromLive", false, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventBaseFromeBanner", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingEventBaseFromBanner", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingTimeBaseFromLive", false, true);
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
            //set keep flag to one recording
            StartStep();

            res = CL.EA.PVR.SetKeepFlag("EventBaseFromeBanner", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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
            //delete all recording from my recording
            StartStep();

            res = CL.EA.PVR.DeleteAllRecordsFromArchive(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            PassStep();
        }
    }

    #endregion Step4

    #region Step5

    private class Step5 : _Step
    {
        public override void Execute()
        {
            //verify that all recording deleted exept the keep recording
            StartStep();

            res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromPlanner", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.EA.PVR.VerifyEventInArchive("TimeBaseFromLive", false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventBaseFromeBanner", false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingEventBaseFromBanner", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.PVR.VerifyEventInArchive("OnGoingTimeBaseFromLive", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            PassStep();
        }
    }

    #endregion Step5

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}