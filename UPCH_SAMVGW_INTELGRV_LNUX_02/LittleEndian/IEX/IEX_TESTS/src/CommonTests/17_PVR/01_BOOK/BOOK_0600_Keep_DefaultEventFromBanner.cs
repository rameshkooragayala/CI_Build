/// <summary>
///  Script Name : BOOK_0600_Keep_DefaultEventFromBanner.cs
///  Test Name   : BOOK-0600-Keep Default Event From Banner
///  TEST ID     : 59838
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0600 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string ChannelNumber = "";

    //Shared members between steps
    private static string EventKeyName = "BookedEvent";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync, Tune to service ");
        this.AddStep(new Step1(), "Step 1: Book Event Based Recording from Guide ");
        this.AddStep(new Step2(), "Step 2: Verify the Keep attribute for the recording");

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

            //Get Channel Number
            ChannelNumber = CL.EA.GetValue("Medium_SD_1");

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, ChannelNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + ChannelNumber);
            }

            res = CL.EA.CheckForVideo(true, true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check for Video after tuning to Channel : " + ChannelNumber);
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
            //Record  event from Guide
            res = CL.EA.PVR.BookFutureEventFromBanner(EventKeyName, 2, 1, false);
            //args:: BookingOffset = 2 , TimeBeforeBooking = 1 , VarifyPCAT = false
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
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
            //verify event time base recordig on my recording labrarry
            res = CL.EA.PCAT.VerifyKeep(EventKeyName, false);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Keep using PCat");
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