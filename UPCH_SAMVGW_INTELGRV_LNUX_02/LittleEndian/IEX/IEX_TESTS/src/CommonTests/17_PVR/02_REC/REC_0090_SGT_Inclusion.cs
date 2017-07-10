/// <summary>
///  Script Name : REC_0090_SGT_Inclusion.cs
///  Test Name   : REC-0090-SGT-Append Missed From Previous Event EGT
///  TEST ID     : 61127
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_0090 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string serviceA; //The service where recordings will happen
    private static string serviceB; //The service where live viewing will happen
    private static string futureEventRecording = "FUTURE_EVENT"; //The future event to be recorded
    private static string sgtFriendlyName = "";
    private static string egtFriendlyName = "";
    private static int endGuardTimeInt = 0;
    private static int startGuardTimeInt = 0;

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 10;
        public const int setSgtTime = 1;
        public const int setEgtTime = 1;
        public const int minTimeForEventEnd = setSgtTime + 1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File.Set SGT,EGT.Tune to other service.");
        this.AddStep(new Step1(), "Step 1: Book event and wait for recording to end");
        this.AddStep(new Step2(), "Step 2: Playback recorded event and validate");

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
            serviceA = CL.EA.GetValue("Short_SD_1");
            serviceB = CL.EA.GetValue("Short_SD_2");

            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");

            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");

            LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);

            LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            if (startGuardTimeInt < 2 || endGuardTimeInt < 2)
            {
                FailStep(CL, "SGT and EGT values fetched from test ini are less then 2 min's");
            }

            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }
			
		    //Tune to service B
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceB);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + serviceB);
            }

            //Check for video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for video on these service!");
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

            //Schedule future recording of first event of service A
            res = CL.EA.PVR.BookFutureEventFromGuide(futureEventRecording, serviceA, Constants.minTimeForEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule future recording of first event");
            }

            //Wait for event to end
            res = CL.EA.WaitUntilEventEnds(futureEventRecording, EndGuardTime: egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until recording completed!");
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

            //Check whether it is fully recorded
            res = CL.EA.PCAT.VerifyEventPartialStatus(futureEventRecording, "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Event not fully recorded!");
            }

            //Playback the event
            res = CL.EA.PVR.PlaybackRecFromArchive(futureEventRecording, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the complete recording!");
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

