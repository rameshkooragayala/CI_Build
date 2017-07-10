/// <summary>
///  Script Name : RB_0041_MaxDepthHD
///  Test Name   : RB-0041-Max Depth For HD Event
///  TEST ID     : 59186
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0041 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Short_HD_1;

    //Shared members between steps
    private static string RB_SIZE;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Tune To HD Channel and Wait Max RB Size");
        this.AddStep(new Step1(), "Step 1: Check RB Size By Playing Its Depth");

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
            Short_HD_1 = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "Short_HD_1");
            RB_SIZE = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "SIZE");

            //Tune to HD channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present");
            }

            //Wait RB size + 5min as safe time
            double waitInRB = (Convert.ToDouble(RB_SIZE) * 60) + 300;
            res = CL.IEX.Wait(waitInRB);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Wait Till RB Will Be Full");
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

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -30, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            CL.IEX.Wait(60);

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}