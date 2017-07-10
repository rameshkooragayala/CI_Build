/// <summary>
///  Script Name : LIB_0058_Book_VideoRecordingList
///  Test Name   : LIB-0058-Book-Video Recording List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_0058 : _Test
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
        this.AddStep(new Step1(), "Step 1: Booking event and time based recordings, that are past, current,complete and partial");
        this.AddStep(new Step2(), "Step 2: Verify that all recording in my recording list");

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

            //verify event in my recording
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
                res = CL.EA.PCAT.VerifyEventPartialStatus("TimeBaseFromLive", "PARTIAL");
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

            res = CL.EA.PCAT.VerifyEventPartialStatus("EventBaseFromeBanner", "PARTIAL");
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

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}