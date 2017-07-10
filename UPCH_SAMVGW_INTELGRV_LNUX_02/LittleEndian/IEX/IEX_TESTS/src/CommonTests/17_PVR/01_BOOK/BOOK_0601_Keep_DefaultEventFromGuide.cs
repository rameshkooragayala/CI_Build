/// <summary>
///  Script Name : BOOK_0601_Keep_DefaultEventFromGuide.cs
///  Test Name   : BOOK-0601-Keep Default Event From Guide
///  TEST ID     : 61252
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.Tests.Engine;

public class BOOK_0601 : _Test
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
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Book Event Based Recording From Guide");
        this.AddStep(new Step2(), "Step 2: Verify The Keep Attribute For The Recording");

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
            ChannelNumber = CL.EA.GetValue("Medium_HD_1");

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
            res = CL.EA.PVR.BookFutureEventFromGuide(EventKeyName, ChannelNumber, 2, 1, false);
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
                FailStep(CL, res, "Failed toVerify Keep");
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