/// <summary>
///  Script Name : RB_0051_TrimmingAfterFull
///  Test Name   : RB-0051-Trimming After Full
///  TEST ID     : 59181
///  QC Version  : 3
/// -----------------------------------------------
///  Modified by : Anna Levin
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0051 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string FTA_Channel;

    //Shared members between steps
    private static string RB_SIZE;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: DCA to default channel");
        this.AddStep(new Step2(), "Step 2: Wait till RB will be full");
        this.AddStep(new Step3(), "Step 3: After RB is full check that it is not bigger than the max size");

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

            res = CL.EA.CheckForVideo(true, false, 30);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present After DCA");
            }
            res = CL.EA.CheckForAudio(true, 30);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify audio is Present After DCA");
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

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string timeStamp = "";
            string timeStampMarginLine = "";

            //Verify FAS message for RB size
            res = CL.IEX.Debug.BeginWaitForMessage("IEX_ReviewBufferCurrentDepth", 0, 30, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage IEX_ReviewBufferCurrentDepth");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to rewind into RB");
            }

            res = CL.IEX.Debug.EndWaitForMessage("IEX_ReviewBufferCurrentDepth", out timeStamp, out timeStampMarginLine, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage IEX_ReviewBufferCurrentDepth");
            }

            string rbDepth = "";
            try
            {
                rbDepth = timeStampMarginLine.Remove(0, timeStampMarginLine.IndexOf(":") + 1);
            }
            catch (Exception ex)
            {
                FailStep(CL, res, "Failed to get IEX_ReviewBufferCurrentDepth Msg From FAS. Exception : " + ex.Message.ToString());
            }

            CL.IEX.LogComment("RB Depth is = " + rbDepth);
            //Adding a one min buffer which is acceptable while verifying the Max RB depth
            if ((Convert.ToDouble(rbDepth)) > ((Convert.ToDouble(RB_SIZE)+1) * 60 * 1000))
                FailStep(CL, res, "Failed: The Review Buffer Depth Is Bigger Than Max Depth");

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