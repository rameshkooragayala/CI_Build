/// <summary>
///  Script Name : RB_0030_EnterTimeShiftImmediately
///  Test Name   : RB-0030-Enter Time Shift Mode Immediately
///  TEST ID     : 59178
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0030 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_Channel;
    private static Service Short_SD_Scrambled_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File and Tune to FTA, Standby On");
        this.AddStep(new Step1(), "Step 1: Standby off, before automatic start of review buffer delay: Try to enter in the review buffer");
        this.AddStep(new Step2(), "Step 2: Tune to Scrambled, Standby On");
        this.AddStep(new Step3(), "Step 3: Standby off, before automatic start of review buffer delay: Try to enter in the review buffer");

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
            FTA_Channel = CL.EA.GetValue("FTA_Channel");
            Short_SD_Scrambled_1 = CL.EA.GetServiceFromContentXML("Encryption=Scrambled;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High");
            if (Short_SD_Scrambled_1 == null)
            {
                FailStep(CL, "Failed to fetch Channel from Content.xml for the passed criterion");
            }


            //Tune to FTA ch. and enter standby
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(true, false, 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present");
            }

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(false, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
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

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Wait until TM Bar disappear
            CL.IEX.Wait(10);

            res = CL.EA.CheckForVideo(false, false, 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }

            CL.IEX.Wait(30);

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }

            //Wait until Channel Bar disappear
            CL.IEX.Wait(20);

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

            //Tune to scrambled ch. and enter standby
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_Scrambled_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(true, false, 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present");
            }

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(false, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
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

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Wait until TM Bar disappear
            CL.IEX.Wait(10);

            res = CL.EA.CheckForVideo(false, false, 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }

            CL.IEX.Wait(30);

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
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