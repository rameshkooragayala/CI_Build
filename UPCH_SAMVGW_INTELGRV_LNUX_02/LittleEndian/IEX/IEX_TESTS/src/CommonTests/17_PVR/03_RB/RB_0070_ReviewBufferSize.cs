/// <summary>
///  Script Name : RB_0070_ReviewBufferSize
///  Test Name   : RB-0070-Review Buffer Size
///  TEST ID     : 59183
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0070 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_Channel;

    //Shared members between steps
    private static string RB_SIZE;
    private static string Milestone = "";
    private static double rbDepthInMin;
    //Constants Class
    private static class Constants
    {
        public const int waitForMilestones = 30;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to Channel, Clean RB");
        this.AddStep(new Step2(), "Step 2: Wait 10 Minutes in That Channel");
        this.AddStep(new Step3(), "Step 3: Rewind to BOF and Play The RB in Timeshift Mode");
        this.AddStep(new Step4(), "Step 4: Wait in Timeshift Mode 2 Hours");
        this.AddStep(new Step5(), "Step 5: Check That RB Depth Is Not Bigger Than Max RB Size");

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
            FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");
            RB_SIZE = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "SIZE");
            Milestone = CL.EA.UI.Utils.GetValueFromMilestones("GetReviewBufferCurrentDepth");

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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + FTA_Channel);
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

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            double timeInRB = 60 * 10;
            CL.IEX.Wait(timeInRB);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Perform Wait 10 Minutes");
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

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -30, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
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

            double timeInRB = 60 * 120;
            res = CL.IEX.Wait(timeInRB);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Perform Wait 120 Minutes");
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


            //Verify FAS message for RB size
            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to rewind into RB");
            }

            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            CL.IEX.LogComment("obtained Milestone String :" + ActualLines[0].ToString());

            res = CL.EA.GetRBDepthInSec(ActualLines[0].ToString(), ref rbDepthInMin);
            CL.IEX.LogComment("RB Depth is = " + rbDepthInMin);

            if (Convert.ToDouble(rbDepthInMin) > (Convert.ToDouble(RB_SIZE) * 60))
                FailStep(CL, res, "Failed: The Review Buffer Depth Is Bigger Than Max Depth");

            PassStep();
        }
    }

    #endregion Step5

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}

