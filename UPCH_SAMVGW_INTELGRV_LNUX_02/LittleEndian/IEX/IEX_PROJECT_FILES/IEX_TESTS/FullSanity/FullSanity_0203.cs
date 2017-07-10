using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0203-LIVE-Fast Zapping_numeric _P+_P-
public class FullSanity_0203 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string First_of_4_Consecutive_FTA;
    static string SD_Service;
    static string scrambledService;
    static string HD_Service;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-203-LIVE-Fast Zapping_numeric _P+_P-
        //Performs Fast Zapping between FTA, Scrambled, SD & HD services by Numeric Key and Channel Up\Down
        //Details:S1-S2-S3-S4 are FTA-Scrambled-SD-HD, one after the other in the channel Line-up
        //Based on QualityCenter 3
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Channel Up Between the Four Services (Fast Zapping)");
        this.AddStep(new Step2(), "Step 2: DCA to All Four Services in Order");
        this.AddStep(new Step3(), "Step 3: Channel Down Between the Four Services (Fast Zapping)");

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

            SD_Service = CL.EA.GetValue("FTA_1st_Mux_2");
            CL.IEX.LogComment("Retrieved Value From ini File: SD_Service = " + SD_Service);

            scrambledService = CL.EA.GetValue("FTA_1st_Mux_3");
            CL.IEX.LogComment("Retrieved Value From ini File: scrambledService = " + scrambledService);

            HD_Service = CL.EA.GetValue("FTA_1st_Mux_4");
            CL.IEX.LogComment("Retrieved Value From ini File: HD_Service = " + HD_Service);

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Step 1: Channel Up Between the Four Services (Fast Zapping)
        //Tune to  Free Air_SD -> Free Air_HD  -> Scramble_SD
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_4_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + First_of_4_Consecutive_FTA + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_SD Service : " + First_of_4_Consecutive_FTA + " After DCA");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_HD Service : " + HD_Service + " With P+ From Free Air_SD ");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_HD Service : " + HD_Service + " After P+ From Free Air_SD");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Scramble_SD Service : " + scrambledService + " With P+ From Free Air_HD ");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Scramble_SD Service : " + scrambledService + " After P+ From Free Air_HD");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Step 2: DCA to All Four Services in Order
        //Zap among all services S1, S2, S3, S4 in using Numeric Zapping (The first DCA is not Fast Zapping)
        //Check for video
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_4_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + First_of_4_Consecutive_FTA + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_SD Service : " + First_of_4_Consecutive_FTA + " After DCA");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, HD_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_HD Service : " + HD_Service + " With DCA From Free Air_SD ");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_HD Service : " + HD_Service + " After DCA From Free Air_SD");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, scrambledService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Scramble_SD Service : " + scrambledService + " With DCA From Free Air_HD ");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Scramble_SDService : " + scrambledService + " After DCA From Free Air_HD");
            }
            //TODO: Check audio, channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Step 3: Channel Down Between the Four Services (Fast Zapping)
        //Tune to  Free Air_SD <- Free Air_HD  <- Scramble_SD (last step lefts us at Scramble_SD)
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.NotPredicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_HD Service : " + HD_Service + " With P- From Scramble_SD");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_HD Service : " + HD_Service + " After P- From Scramble_SD");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Predicted, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + scrambledService + " With P- From Free Air_HD ");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on Free Air_SD Service : " + scrambledService + " After P- From Free Air_HD");
            }
            //TODO: Check audio, channel bar, event information
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