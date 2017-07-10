/// <summary>
///  Script Name : BANNER_0500_PlaybackBannerTimeout_RB.cs
///  Test Name   : BANNER-0500-Playback Banner Timeout-Review Buffer
///  TEST ID     : 63832
///  JIRA ID     : FC-249
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : SHRUTHI H M
///  Last Modified Date: 12/7/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class BANNER_0500 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Static Variables
    private static EnumChannelBarTimeout bannerTimeoutToSet;
    private static EnumChannelBarTimeout bannerTimeoutDefault;
    private static string[] TM_REW = { "" };
    private static string[] TM_FF = { "" };
    private static Service service = new Service();

    //Constants used in the test
    private static class Constants
    {
        public const bool checkIfVideoIsPresent = true;
        public const bool checkFullVideoArea = false;
        public const int timeToCheckForVideo = 10;	// In Seconds
        public const int secToWaitInLive = 900;	// In Seconds
        public const int secToWaitInPause = 30;	// In Seconds
        public const bool checkEOFBOF = false;
        public const double pause = 0;
        public const double play = 1;
        public const bool isReviewBuffer = true;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get required Channel Number and values from ini File");
        this.AddStep(new Step1(), "Step 1: Pause Video during Live viewing");
        this.AddStep(new Step2(), "Step 2: Play from RB and check for Banner dismissal on timeout");
        this.AddStep(new Step3(), "Step 3: Change Timeout Duration in Channel Bar Timeout settings");
        this.AddStep(new Step4(), "Step 4: Apply Rew Trickmodes");
        this.AddStep(new Step5(), "Step 5: Play from RB and check for Banner dismissal on new timeout");
        this.AddStep(new Step6(), "Step 6: Apply FF Trickmodes");

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

            string bannerTimeout = "";

            // Get required Channel Number and values from ini File
            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out bannerTimeoutDefault);

            // Fetch another Banner timeout value <> Default value
            string[] banner_timeout_list = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "LIST").Split(',');

            if (banner_timeout_list[0].Equals(bannerTimeoutDefault))
            {
                bannerTimeout = banner_timeout_list[1];
            }
            else
            {
                bannerTimeout = banner_timeout_list[0];
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out bannerTimeoutToSet);
            TM_REW = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW").Split(',');
            TM_FF = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD").Split(',');

            //Surf to required channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }

            res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present");
            }

            // Wait for sometime so that RB can grow
            res = CL.IEX.Wait(Constants.secToWaitInLive);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in Live!");
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

            // Pause Video during Live viewing
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.pause, Constants.checkEOFBOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause video");
            }

            if (!CL.EA.UI.Utils.VerifyState("TRICKMODE BAR", bannerTimeoutDefault.GetHashCode() + 6))
            {
                FailStep(CL, res, "Failed to get playback banner on pausing the video");
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

            // Wait in Pause for sometime to prevent accidental catch-up to LIVE
            res = CL.IEX.Wait(Constants.secToWaitInPause);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait in Pause!");
            }

            // Play from RB and check for Banner dismissal on timeout
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, Constants.checkEOFBOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play video");
            }

            if (!CL.EA.UI.Utils.VerifyState("LIVE", bannerTimeoutDefault.GetHashCode() + 6))
            {
                FailStep(CL, res, "Playback banner did not dismiss in timeout duration:" + bannerTimeoutDefault);
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

            // Change Timeout Duration in Channel Bar Timeout settings
            res = CL.EA.STBSettings.SetBannerDisplayTime(bannerTimeoutToSet);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change Banner Display Time to:" + bannerTimeoutToSet);
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

            // Apply Rew Trickmode and check for Playback Banner
            foreach (string TM in TM_REW)
            {
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToDouble(TM), Constants.checkEOFBOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set RB TM to:" + TM);
                }
                if (CL.EA.UI.Utils.VerifyState("LIVE", bannerTimeoutToSet.GetHashCode() + 6))
                {
                    FailStep(CL, res, "Playback banner got dismissed during Trickmode!");
                }
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
            StartStep();

            // Play from RB and check for Banner dismissal on new timeout
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, Constants.checkEOFBOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play video");
            }

            if (!CL.EA.UI.Utils.VerifyState("LIVE", bannerTimeoutToSet.GetHashCode() + 6))
            {
                FailStep(CL, res, "Playback banner did not dismiss in timeout duration:" + bannerTimeoutToSet);
            }

            PassStep();
        }
    }

    #endregion Step5

    #region Step6

    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Apply FF Trickmode and check for Playback Banner
            foreach (string TM in TM_FF)
            {
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToDouble(TM), Constants.checkEOFBOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set RB TM to:" + TM);
                }
                if (CL.EA.UI.Utils.VerifyState("LIVE", bannerTimeoutToSet.GetHashCode() + 6))
                {
                    FailStep(CL, res, "Playback banner got dismissed during Trickmode!");
                }
            }

            PassStep();
        }
    }

    #endregion Step6

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Stop playback
        res = CL.EA.PVR.StopPlayback(Constants.isReviewBuffer);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason);
        }

        // Set Banner Display time to Default
        res = CL.EA.STBSettings.SetBannerDisplayTime(bannerTimeoutDefault);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason);
        }
    }

    #endregion PostExecute
}