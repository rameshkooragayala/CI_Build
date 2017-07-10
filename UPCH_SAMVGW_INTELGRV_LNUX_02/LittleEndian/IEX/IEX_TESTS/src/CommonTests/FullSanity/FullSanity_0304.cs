using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0304-INSTL-DataCaching-dynamicMiddlewareConfigFiels
public class FullSanity_0304 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Medium_SD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:FullSanity-1502-PC-Locked_Channel
        //Based on QualityCenter test version 6.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to Parental Control locked Channel, Verify that it is locked & Unlock it");
        this.AddStep(new Step2(), "Step 2: Verify that language is English");
        this.AddStep(new Step3(), "Step 3: Verify that Menu Language in Settings has 2 Lang.: English & Ducth");

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
            Medium_SD_1 = CL.EA.GetValue("Medium_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Medium_SD_1 = " + Medium_SD_1);

            StartStep();


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

            res = CL.EA.TuneToLockedChannel(Medium_SD_1, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to  Locked Channel & Verify video");
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

            // since step 1 was success, we know that we are in english, otherwise it should have fail.
            // in the future , if we will have several dictionaries, we should open and use the english dictionary to verify that we got the engilsh key.
            // currently we use only the english dictionary


            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MENU LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:MENU LANGUAGE ");
            }

            res = CL.IEX.MilestonesEPG.SelectMenuItem("ENGLISH");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to SelectMenuItem : ENGLISH");
            }

            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEDERLANDS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to SelectMenuItem : NEDERLANDS");
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