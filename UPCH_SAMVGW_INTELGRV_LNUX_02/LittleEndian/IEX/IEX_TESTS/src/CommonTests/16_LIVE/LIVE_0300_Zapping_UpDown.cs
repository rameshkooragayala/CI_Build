/// <summary>
///  Script Name : LIVE_0300_Zapping_UpDown.cs
///  Test Name   : LIVE-0300-Channel Change Up/Down
///  TEST ID     : 61086
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0300 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string service1;
    private static string service2;
    private static string serviceWithNoVideo;

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = true;
        public const int timeToCheckForVideo = 2;
        public const bool checkIfAudioIsPresent = true;
        public const int timeToCheckForAudio = 2;
    }

    //Shared members between steps

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get the required services from ini file");
        this.AddStep(new Step1(), "Step 1: Tune to service S1 by up/down navigation.Check for audio and video");
        this.AddStep(new Step2(), "Step 2: Tune to service S2 by up/down navigation.Check for audio and video");
        this.AddStep(new Step3(), "Step 3: Repeat step 2 when surfing from and to channel with no video");

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
            service1 = CL.EA.GetValue("FTA_1st_Mux_1");
            service2 = CL.EA.GetValue("FTA_1st_Mux_2");
            serviceWithNoVideo = CL.EA.GetValue("SERVICE_WITH_NO_VIDEO");

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

            //Tune to service s1 using channel bar
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, service1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to service " + service1);
            }

            //Check for audio and video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video not present on service " + service1);
            }
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on service " + service1);
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

            //Tune to service s2 using channel bar
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, service2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to service " + service2);
            }

            //Check for audio and video
            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video not present on service " + service2);
            }
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on service " + service2);
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

            //Tune to service with no video from service s2 using channel bar
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, serviceWithNoVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to service " + serviceWithNoVideo);
            }

            //Tune to service s2 from the service with no video using channel bar
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, service2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to service " + service2);
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