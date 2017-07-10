/// <summary>
///  Script Name : LIVE_0906_FZ_PLB.cs
///  Test Name   : LIVE-0906-Fast Zapping-Playback
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0906 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Short_SD_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S1");
        this.AddStep(new Step2(), "Step 2: Tune To An EPG Screen, Where Video is not displayed");
        this.AddStep(new Step3(), "Step 3: Tune To Channel S2, Using Upwards Direction");

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
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");

            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event1", 4, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }
            //Wait to have recorded content of reasonable length
            int Time_Recording = 60;//180;
            CL.IEX.Wait(Time_Recording);

            res = CL.EA.PVR.StopRecordingFromBanner("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To Channel S1  After Stanby
        public override void Execute()
        {
            StartStep();

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + Short_SD_1 + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Start PLB For Several Seconds, Than Stop PLb And Return To Live

        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", -1, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Record from Archive");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        //Step 3: Tune To Channel S2, Using Upwards Direction

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Next Perdicted Service");
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