using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0206 LIVE-Zapping_Video-Radio channels
public class FullSanity_0206 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string First_of_2_Consecutive_Radio_Service;
    static string SD_Service;
    static string HD_Service;

    //Shared members between steps
    static int soundValue = 0;

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0203-LIVE-Fast Zapping_numeric _P+_P-
        // Performs Live Surfing between SD Services, HD Services and Radio Channels
        //Based on QualityCenter - version 4
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");

        this.AddStep(new Step1(), "Step 1: Tune to a SD service");
        this.AddStep(new Step2(), "Step 2: Zap to a Radio Channel Usnig p+/ P-");
        this.AddStep(new Step3(), "Step 3: Channel Change between Radio Channels with P+P-");
        this.AddStep(new Step4(), "Step 4: Tune back to any HD service");
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

            First_of_2_Consecutive_Radio_Service = CL.EA.GetValue("First_of_2_Consecutive_Radio_Service");
            CL.IEX.LogComment("Retrieved Value From ini File: First_of_2_Consecutive_Radio_Service = " + First_of_2_Consecutive_Radio_Service);

            HD_Service = CL.EA.GetValue("Short_HD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: HD_Service = " + HD_Service);

            SD_Service = CL.EA.GetValue("Short_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: SD_Service = " + SD_Service);

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Tune to a SD service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SD_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_SD Service : " + " After DCA");
            }


            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion

    #region Step2
    private class Step2 : _Step
    {
        //Zap to a Radio Channel
        //Check for audio 
        public override void Execute()
        {
            StartStep();
            res = CL.EA.TuneToRadioChannel(First_of_2_Consecutive_Radio_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Radio Channel : " + " With DCA");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        // Channel Change between Radio Channels with P+P-
        //Check for audio
        public override void Execute()
        {
            StartStep();

            res = CL.EA.TuneToChannel(EnumSurfIn.Live, 1, EnumPredicted.Ignore);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune Up From Radio Channel To Radio Channel");
            }
            res = CL.EA.TuneToChannel(EnumSurfIn.Live, -1, EnumPredicted.Default);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune Down From Radio Channel To Radio Channel");
            }
            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        //Tune back to any HD service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, HD_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to HD Service : " + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on HD Service : " + " After DCA");
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