/// <summary>
///  Script Name : LIB_BOOK_SeriesCancelCompleteCurrent.cs
///  Test Name   : LIB-BOOK-0031-Planner_Series_Recording_Cancel_Current_episode_FT146 
///                LIB-BOOK-0033-Planner_Series_Recording_Cancel_Whole_Series_FT146
///  TEST ID     : 73883, 73884
///  Variations from QC:none
///  QC Repository : UPC/FR_FUSION
/// -----------------------------------------------
///  Modified by : Ganpat Singh
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("LIB_BOOK_SeriesCancelCompleteCurrent")]
public class LIB_BOOK_SeriesCancelCompleteCurrent : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service seriesService_1;
    private static Service seriesService_2;
	private static string StartGuardTimeName = "";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml file , Sync And Book Series on two different services");
        this.AddStep(new Step1(), "Step 1: Cancel Single Event of Series from Planner");
        this.AddStep(new Step2(), "Step 2: Cancel complete Series from Planner");

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

            //Fetcing a Series service
            seriesService_1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsSeries=True", "ParentalRating=High");
            if (seriesService_1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            seriesService_2 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsSeries=True", "ParentalRating=High;LCN="+seriesService_1.LCN);
            if (seriesService_2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

			StartGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (StartGuardTimeName == null)
            {
               FailStep(CL, "Failed to get SGT MIN value from Project INI file");
            }

            res = CL.EA.STBSettings.SetGuardTime(true, StartGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Guard time");
            }
			
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, seriesService_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to channel");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("series", NumOfPresses: 1, MinTimeBeforeEvStart: 3, VerifyBookingInPCAT: true, ReturnToLive: true, IsConflict: false, IsSeries: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Series Event From Banner");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, seriesService_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to channel");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("series_1",NumOfPresses: 1,MinTimeBeforeEvStart: 3,VerifyBookingInPCAT: true,ReturnToLive: true,IsConflict: false, IsSeries: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Series Event From Banner");
            }


            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Cancle Single episode of Series From Planner
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.CancelBookingFromPlanner("series",VerifyCancelInPCAT: false, IsSeries: true, IsComplete: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to cancel Single episode of Series From Planner");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {

        //Cancel All episodes of Series From Planner
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.CancelBookingFromPlanner("series_1",VerifyCancelInPCAT: false,IsSeries: true, IsComplete: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to cancel All episodes of Series From Planner");
            }
            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        //Clean up the planner
        IEXGateway._IEXResult res;

        res = CL.EA.PVR.CancelBookingFromPlanner("series", VerifyCancelInPCAT: false, IsSeries: true, IsComplete: true);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to cancel All episodes of Series From Planner");
        }
		
		res = CL.EA.PVR.CancelBookingFromPlanner("series_1", VerifyCancelInPCAT: false, IsSeries: true, IsComplete: true);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to cancel All episodes of Series From Planner");
        }
    }
    #endregion PostExecute
}