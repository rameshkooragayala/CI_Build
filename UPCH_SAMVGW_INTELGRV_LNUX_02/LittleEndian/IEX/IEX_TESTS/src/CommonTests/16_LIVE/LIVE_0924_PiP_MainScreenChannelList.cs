/// <summary>
///  Script Name : LIVE_0924_PiP_MainScreenChannelList.cs
///  Test Name   : LIVE-0924-PiP-Choose Main Screen Channel List
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0924 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_1st_Mux_3;
    private static string Same_Schedule_4;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Set Banner Display Time");
        this.AddStep(new Step1(), "Step 1: Tune to Channel S1 Using DCA");
        this.AddStep(new Step2(), "Step 2: Browse Current Events in channel S4 from  channel bar & tune this channel   ");
        this.AddStep(new Step3(), "Step 3: Tune to Predicted Channel S5 ");

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
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            Same_Schedule_4 = CL.EA.GetValue("Same_Schedule_4");

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

            //Tune to channel S1 with DCA
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_3, true, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on S1");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 3, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Channel Bar");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on S1");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Channel Bar Future Events Surfing");
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