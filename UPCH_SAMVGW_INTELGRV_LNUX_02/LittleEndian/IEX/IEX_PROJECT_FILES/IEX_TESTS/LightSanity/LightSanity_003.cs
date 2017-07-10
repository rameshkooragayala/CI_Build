using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-003-AUD-Audio stream selection
public class LightSanity_003 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Multiple_Audio_1;
    static string Multiple_Audio_2;
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-003-AUD-Audio stream selection
        //Check that from programme action box, User is able to select an audio stream when several audio are available.
        //Pre-conditions: On service S1, Event E1 has at least two audio streams available in its component descriptor. 
        //Based on QualityCenter test version .
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Zap to Service S1");
        this.AddStep(new Step2(), "Step 2: Change The Audio Stream Through Action Menu");
        this.AddStep(new Step3(), "Step 3: Change The Audio Stream Through Action Menu");

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
            Multiple_Audio_1 = CL.EA.GetValue("Multiple_Audio_1");
            Multiple_Audio_2 = CL.EA.GetValue("Multiple_Audio_2");

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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Multiple_Audio_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Multiple_Audio_1 Channel");
            }
            //check for audio 
            res = CL.EA.CheckForAudio(true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check for Audio");
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
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR/A//V SETTINGS/AUDIO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navegte To ACTION BAR/A/V SETTINGS/AUDIO");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR("SELECT_UP", out timeStamp, 3000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT_UP ");
            }

            timeStamp = "";
            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }

            //check for audio 
            res = CL.EA.CheckForAudio(true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check for Audio");
            }
            CL.IEX.Wait(7);
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Multiple_Audio_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Multiple_Audio_1 Channel");
            }
            //check for audio 
            res = CL.EA.CheckForAudio(true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check for Audio");
            }
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR/A//V SETTINGS/AUDIO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navegte To ACTION BAR/A/V SETTINGS/AUDIO");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR("SELECT_UP", out timeStamp, 3000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT_UP ");
            }

            timeStamp = "";
            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }

            //check for audio 
            res = CL.EA.CheckForAudio(true, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check for Audio");
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