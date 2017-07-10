/// <summary>
///  Script Name : RB_0011_SaveHDEvent
///  Test Name   : RB-0011-Save HD Event From RB
///  TEST ID     : 59177
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0011 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_Channel;
    private static string Short_HD_1;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Tune to HD Channel, Enter Review Buffer, Play it, Check A/V");
        this.AddStep(new Step2(), "Step 2: Save The Current Playing Event");
        this.AddStep(new Step3(), "Step 3: Check The Event Was Saved in Archive");

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
            Short_HD_1 = CL.EA.GetValue("Short_HD_1");

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

            //Tune to HD channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_HD_1);
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

            //Check video paused
            res = CL.EA.CheckForVideo(false, false, 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }
            //Check no audio
            res = CL.EA.CheckForAudio(false, 5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Audio is Paused");
            }

            CL.IEX.Wait(360);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Wait until TM Bar disappear
            CL.IEX.Wait(10);

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present When Starting Playing RB");
            }
            //check for audio
            res = CL.EA.CheckForAudio(true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Audio is Present When Starting Playing RB");
            }

            CL.IEX.Wait(360);

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

            //book from banner

            res = CL.EA.PVR.RecordCurrentEventFromBanner("HD_Event", -1, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Save RB Evnet");
            }
            CL.IEX.Wait(60);

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
            res = CL.EA.PVR.VerifyEventInArchive("HD_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verfiy HD Event on Archive");
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