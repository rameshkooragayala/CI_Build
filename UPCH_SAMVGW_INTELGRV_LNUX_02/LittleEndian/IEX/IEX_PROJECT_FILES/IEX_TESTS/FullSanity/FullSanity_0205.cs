using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0205-LIVE-PiP Display-Guide
public class FullSanity_0205 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string First_of_4_Consecutive_FTA;


    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0205-LIVE-PiP Display-Guide
        //Browse Current and Future Events in Guide. Verify PiP is Shown for Current Events of Other Channels. 
        //S1, S2 ,S3, S4  in the channel list are one after the other.
        //Based on QualityCenter - version 2
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to Channel S1");
        this.AddStep(new Step2(), "Step 2: Browse Current Events in Programme Grid. Focus Channel to S4");
        this.AddStep(new Step3(), "Step 3: Exit the Programme Grid");
        this.AddStep(new Step4(), "Step 4: Again Browse Current Events in Programme Grid. Focus Channel to S4");
        this.AddStep(new Step5(), "Step 5: Browse Future Events in the Programme Grid");

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
            CL.IEX.LogComment("Retrieved Value From ini File: First_of_4_Consecutive_FTA = " + First_of_4_Consecutive_FTA);

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Step 1: Tune to Channel S1
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_4_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + First_of_4_Consecutive_FTA);
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present " + First_of_4_Consecutive_FTA + " After DCA");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Step 2: Browse Current Events in Programme Grid. Focus Channel to S4
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Guide");
            }


            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Step 3: Exit the Programme Grid
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Guide Surfing ");
            }
            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        //Step 4: Again Browse Current Events in Programme Grid. Focus Channel to S4
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel S4 in Guide a Second Time");
            }
            PassStep();
        }
    }
    #endregion
    #region Step5
    private class Step5 : _Step
    {
        //Step 5: Browse Future Events in the Programme Grid
        public override void Execute()
        {
            StartStep();

            res = CL.EA.BrowseGuideFuture(true, 4, true, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Future Event in Guide");
            }

            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live from Guide Future Events Surfing");
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