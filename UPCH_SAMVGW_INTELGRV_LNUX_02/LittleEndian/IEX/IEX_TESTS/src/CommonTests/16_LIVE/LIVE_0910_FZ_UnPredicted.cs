/// <summary>
///  Script Name : LIVE_0910_FZ_UnPredicted.cs
///  Test Name   : LIVE-0910-Fast Zapping-UnPredicted
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0910 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service Short_SD_Scrambled_1;


    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune To An UnPredicted Channel S1 After Standby");
        this.AddStep(new Step2(), "Step 2: Tune To A Predicted Channel S2, Using Upwards Direction");
        this.AddStep(new Step3(), "Step 3: Tune To  An UnPredicted Channel S1, Via Guide");
        this.AddStep(new Step4(), "Step 4: Tune To A Predicted Channel S2, Via Guide");

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
            Short_SD_Scrambled_1 = CL.EA.GetServiceFromContentXML("Encryption=Scrambled;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High");
            if (Short_SD_Scrambled_1 == null)
            {
                FailStep(CL, "Failed to fetch Channel from Content.xml for the passed criterion");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To An UnPredicted Channel S1 After Standby

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

            // Check that ther is a present video after standby
            res = CL.EA.CheckForVideo(true, false, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video Is Present After Standby");
            }
            // temp surf due to service problems
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_Scrambled_1.LCN, true, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Service : " + Short_SD_Scrambled_1 + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Tune To A Predicted Channel S2, Using Upwards Direction

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

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        //Step 3: Tune To  An UnPredicted Channel S1, Via Guide

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 3, EnumPredicted.NotPredicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The UnPredicted Service");
            }
            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    private class Step4 : _Step
    {
        //Step 4: Tune To A Predicted Channel S2, Via Guide

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to The Next Predicted Service");
            }
            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}