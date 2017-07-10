/// <summary>
///  Script Name : LIVE_0905_FZ_EPG.cs
///  Test Name   : LIVE-0905-Fast Zapping-EPG
///  TEST ID     : 59040
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0905 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string service1;

    //Constants used in the test
    private static class Constants
    {
        public const string navigationToScreenWithNoLiveViewing = "STATE:STORE";
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 2;
        public const bool checkIfAudioIsPresent = true;
        public const int timeToCheckForAudio = 2;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the service numbers from ini file.");
        this.AddStep(new Step1(), "Step 1: Tune to service s1.");
        this.AddStep(new Step2(), "Step 2: Enter EPG screen with no live viewing in background.");
        this.AddStep(new Step3(), "Step 3: Exit to fullscreen.Check video of S1.");
        this.AddStep(new Step4(), "Step 4: Tune to next channel by tuning upwards");

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
            service1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "service");
            if (String.IsNullOrEmpty(service1))
            {
                service1 = CL.EA.GetValue("FTA_1st_Mux_1");
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

            //Tune to service s1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to service " + service1);
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

            //Navigate to a screen where there is no live viewing on background
            res = CL.IEX.MilestonesEPG.NavigateByName(Constants.navigationToScreenWithNoLiveViewing);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to " + Constants.navigationToScreenWithNoLiveViewing);
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Exit to fullscreen
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to fullscreen!");
            }

            //Check for Audio
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on service " + service1);
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

            //Surf to next predicted service
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to the next service!");
            }

            //Check for audio and video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video not present on zapped service!");
            }
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on service " + service1);
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