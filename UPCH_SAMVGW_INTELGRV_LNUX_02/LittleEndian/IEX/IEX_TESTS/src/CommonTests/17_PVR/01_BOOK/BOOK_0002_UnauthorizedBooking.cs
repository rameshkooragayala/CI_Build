/// <summary>
///  Script Name : BOOK_0002_UnauthorizedBooking.cs
///  Test Name   : BOOK-0002-Unauthorized Booking
///  TEST ID     : 59842
///  QC Version  : 4
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BOOK_0002 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string unauthorizedService;

    //Shared members between steps
    private static string currentEventRecording = "CURRENT_EVENT";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Perform Event Based Booking on Unauthorized Service");
        this.AddStep(new Step2(), "Step 2: Perform Time Based Booking on Unauthorized Service");
        this.AddStep(new Step3(), "Step 3: Repeat For Event Booking From Guide");

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

            //Get Channel Values From ini File
            unauthorizedService = CL.EA.GetValue("Unauthorized_Service");

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

            //Tune to the service whose future event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, unauthorizedService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + unauthorizedService);
            }

            //Check for video
            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present");
            }

            //Perform a recording on the current event
            res = CL.EA.PVR.RecordCurrentEventFromBanner(currentEventRecording);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Booking is Successful For Unauthorized Service");
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

            //Book the current event for recording manually
            res = CL.EA.PVR.RecordManualFromPlanner(currentEventRecording, unauthorizedService, 1, 2, 1);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Booking is Successful For Unauthorized Service");
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

            //Perform a recording on the current event
            res = CL.EA.PVR.RecordCurrentEventFromGuide(currentEventRecording, unauthorizedService);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Booking is Successful For Unauthorized Service");
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