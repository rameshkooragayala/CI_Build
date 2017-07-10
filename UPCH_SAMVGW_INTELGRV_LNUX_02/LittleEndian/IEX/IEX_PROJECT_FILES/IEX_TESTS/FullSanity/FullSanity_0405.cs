using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
//FullSanity-0405-SET-Change_PIN_Code
public class FullSanity_0405 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static string FTA_1st_Mux_1;

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0405-SET-Change_PIN_Code
        //Check that User is able to change the master PIN Code
        //Pre-conditions: None
        //Based on QualityCenter test version: 3
        //Variations from QualityCenter: None
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1:Navigate To Perental Control ");
        this.AddStep(new Step2(), "Step 2:Enter the current PIN Code ");
        this.AddStep(new Step3(), "Step 3:Enter the new PIN Code");
        this.AddStep(new Step4(), "Step 4:Re-Enter the new PIN Code");

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
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_1st_Mux_1 = " + FTA_1st_Mux_1);

            StartStep();

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Step 1:Navigate To Perental Control        
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANGE PIN CODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Perntal Control Screen");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Step 2:Enter the current PIN Code
        public override void Execute()
        {
            StartStep();

            res = CL.EA.EnterDeafultPIN("INSERT PIN");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Deafult PIN And Verfy State SETUP PIN ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        //Step 3:Enter the new PIN Code        
        public override void Execute()
        {
            StartStep();



            res = CL.EA.EnterPIN("1234", "INSERT PIN");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter New PIN first Time");
            }
            PassStep();
        }
    }
    #endregion
    #region Step4
    //Step 4:Re-Enter the new PIN Code
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();


            res = CL.EA.EnterPIN("1234", "PIN MANAGEMENT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter New PIN second Time");
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