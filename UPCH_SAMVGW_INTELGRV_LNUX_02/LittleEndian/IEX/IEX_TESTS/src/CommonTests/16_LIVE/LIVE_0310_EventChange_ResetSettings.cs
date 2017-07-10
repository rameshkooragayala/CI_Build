/// <summary>
///  Script Name : LIVE_0310_EventChange_ResetSettings.cs
///  Test Name   : LIVE-0310-Event Change-Reset Settings
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0310 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Multiple_Audio_2;

    //Shared members between steps
    private static string Default_Audio_Track;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To A Multi Audio Channel S1");
        this.AddStep(new Step2(), "Step 2: Change the Audio Language");
        this.AddStep(new Step3(), "Step 3: Wait Till Next Event starts And Check The Audio Language");

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
            Multiple_Audio_2 = CL.EA.GetValue("Multiple_Audio_2");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To A Multi Audio Channel S1

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Multiple_Audio_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + Multiple_Audio_2 + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Change the Audio Language

        public override void Execute()
        {
            StartStep();

            //Get Default Audio Track
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTINGS AUDIO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out Default_Audio_Track);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Audio Track");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANGE AUDIO TO SECOND-STEREO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:CHANGE AUDIO TO SECOND-STEREO");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        //Step 3: Wait Till Next Event starts And Check The Audio Language

        public override void Execute()
        {
            StartStep();
            int LeftTime = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref LeftTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To WGet Event Left Time");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live");
            }

            res = CL.IEX.Wait(LeftTime + 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait Till Event Ends");
            }

            //Get Current Audio Track
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTINGS AUDIO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:AV SETTINGS AUDIO");
            }

            string AudTrack = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out AudTrack);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Audio Track");
            }

            // Verify That Audio track is back to default
            if (AudTrack != Default_Audio_Track)
            {
                FailStep(CL, "Received Differnt Audio (" + AudTrack + ") than requested  (" + Default_Audio_Track + ")");
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