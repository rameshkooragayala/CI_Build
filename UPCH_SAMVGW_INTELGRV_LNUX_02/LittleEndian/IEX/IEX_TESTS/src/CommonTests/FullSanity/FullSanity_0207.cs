using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0207-LIVE-Zapping_SD_HD
public class FullSanity_0207 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string first_SD_Service;
    static string HD_Service;
    static string secend_SD_Service;

    //Shared members between steps
    static int soundValue = 0;

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0207-LIVE-Zapping_SD_HD
        //Performs live surfing Betwin SD and HD Services
        //Based on QualityCenter - version 4
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to a SD Service Using DCA");
        this.AddStep(new Step2(), "Step 2: Zap To a HD Service");
        this.AddStep(new Step3(), "Step 3: Tune Back to any SD Service Using DCA");

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
            //first_SD_Service = CL.EA.GetValue("Short_SD_Scrambled_1");
            secend_SD_Service = CL.EA.GetValue("Medium_SD_1");
            HD_Service = CL.EA.GetValue("Short_HD_1");
            first_SD_Service = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Short_SD_Scrambled_1");
            if (first_SD_Service == "")
            {
                FailStep(CL, "Failed to get the Short_SD_Scrambled_1 value from test ini");
            }
            CL.IEX.LogComment("Retrieved Value From ini File: first_SD_Service = " + first_SD_Service);
  
            CL.IEX.LogComment("Retrieved Value From ini File: secend_SD_Service = " + secend_SD_Service);

            CL.IEX.LogComment("Retrieved Value From ini File: HD_Service = " + HD_Service);

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Tune to a SD Service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, first_SD_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to SD Service : " + first_SD_Service + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present  SD Service : " + first_SD_Service + " After DCA");
            }
           
            //TODO: Check channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Tune to a HD Service
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, HD_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to HD Service : " + HD_Service + " With DCA From SD Service");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On  Service : " + HD_Service + " After DCA");
            }
           
            //TODO: Check channel bar, event information
            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Tune to a SD Service (From HD )
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, secend_SD_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to SD Service : " + secend_SD_Service + " With DCA From HD Service");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present  SD Service : " + secend_SD_Service + " After DCA From HD Service ");
            }
            //Check if audio alive
           
            //TODO: Check channel bar, event information
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