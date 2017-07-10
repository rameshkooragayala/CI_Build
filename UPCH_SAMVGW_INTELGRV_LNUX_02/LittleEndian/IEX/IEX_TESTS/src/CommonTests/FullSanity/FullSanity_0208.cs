using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0208-LIVE-Zapping_10 times
public class FullSanity_0208 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string First_of_10_Consecutive_FTA;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0208-LIVE-Zapping_10 times
        // Performs Channel Up and Down Several Times, Slow and Fast
        //Based on QualityCenter - version 4
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Channel Up 5  Times (fast)");
        this.AddStep(new Step2(), "Step 2: Channel UP/Down 5 Times (slow)");
        this.AddStep(new Step3(), "Step 3: Channel Down 5 Times (fast)");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {

            StartStep();

            //Get Values From ini File

            First_of_10_Consecutive_FTA = CL.EA.GetValue("First_of_10_Consecutive_FTA");
            CL.IEX.LogComment("Retrieved Value From ini File: First_of_10_Consecutive_FTA = " + First_of_10_Consecutive_FTA);



            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_10_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + First_of_10_Consecutive_FTA);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    //create not predicted by DCA to the other dirction 

    private class Step1 : _Step
    {
        //Step 1: Channel Up 4  Times (fast)
        public override void Execute()
        {
            StartStep();
            // first time channel up in order to make the predicted direction - up
            res = CL.EA.TuneToChannel(EnumSurfIn.Live, 1, EnumPredicted.Ignore);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Failed change channel up");
            }

            for (int i = 1; i < 4; i++)
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Channel Up in the " + i + " Channel Change ");
                }

                res = CL.EA.CheckForVideo(true, false, 10);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify Video is Present After " + i + " Channel Changes Up ");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {


        //Step 2: Channel UP/Down 4 Times (slow)


        public override void Execute()
        {
            StartStep();

            for (int i = 1; i < 5; i++)
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.NotPredicted);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Channel Down as not predicted in " + i + " round ");
                }

                res = CL.EA.CheckForVideo(true, false, 10);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify Video is Present in " + i + "  rounds ");
                }

                CL.IEX.Wait(2);

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.NotPredicted);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Channel Up  as not predicted in " + i + " round ");
                }

                res = CL.EA.CheckForVideo(true, false, 10);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify Video is Present in " + i + " rounds ");
                }
            }
            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Step 3: Channel Down 4 Times (fast)
        public override void Execute()
        {
            StartStep();


            // first time channel down in order to make the predicted direction - down
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.NotPredicted);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed Channel down as not predicted");
            }

            for (int i = 1; i < 4; i++)
            {

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Predicted);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Channel down in the " + i + " Channel Change ");
                }

                res = CL.EA.CheckForVideo(true, false, 10);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify Video is Present After " + i + " Channel Changes Down ");
                }
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}