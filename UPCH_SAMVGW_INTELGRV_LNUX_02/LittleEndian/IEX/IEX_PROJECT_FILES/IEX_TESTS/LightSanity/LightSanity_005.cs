using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-005-LIVE-PiP Display-Channel-Bar
public class LightSanity_005 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string First_of_4_Consecutive_FTA;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {



        //Brief Description: LightSanity-005-LIVE-PiP Display-Channel-Bar
        //Browse Current and Future Events (of Other Channels ) in Channel Bar. Verify PiP is Shown for Current Events of Other Channels. 
        //S1, S2 ,S3, S4 - Services in the channel list one after the other.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: Not checking Audio.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Set Banner Display Time");
        this.AddStep(new Step1(), "Step 1: Tune to Channel S1 Using DCA");
        this.AddStep(new Step2(), "Step 2: Browse Current Events in Channel Bar");
        this.AddStep(new Step3(), "Step 3: Close the Channel Bar");
        this.AddStep(new Step4(), "Step 4: Again Browse Current Events in Channel Bar");
        this.AddStep(new Step5(), "Step 5: Browse Future Events in the Channel Bar");

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
            First_of_4_Consecutive_FTA = CL.EA.GetValue("First_of_4_Consecutive_FTA");


            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to channel S1 with DCA
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_4_Consecutive_FTA);
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
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Open the Channel List and browse current events. Focus channel to S4.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 3, EnumPredicted.PredictedWithoutPIP);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Channel Bar");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present in Background of Channel Bar Surfing");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Close the channel list (still tuned on S1)
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Channel Bar Surfing");
            }

            //Tune to channel S1 with DCA
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_4_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Open the Channel List and browse current events. Focused channel to S4.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 3, EnumPredicted.PredictedWithoutPIP);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Channel Bar a Second Time");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present in Background of Channel Bar Surfing");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Channel Bar Surfing");
            }

            PassStep();
        }
    }
    #endregion
    #region Step5
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Browse future events in the channel list.
            res = CL.EA.ChannelBarSurfFuture(true, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Future Event in Channel Bar");
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Channel Bar Future Events Surfing");
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