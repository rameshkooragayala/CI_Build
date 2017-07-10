/// <summary>
///  Script Name : STORE_0210_Navigation_Adult.cs
///  Test Name   : STORE-0210-Navigation-Browsing-Adult-Classification
///  TEST ID     : 18809
///  QC Version  : 10
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date : 04.03.2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STORE_0210")]
public class STORE_0210 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //shared members between the steps
    private static string stateOfAdultCategory;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the State for Adult Category in STORE from test.ini";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to the Adult Category on STORE";
    private const string STEP2_DESCRIPTION = "Step 2: Enter the Master PIN Code and check if it give access to Adult Category.";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //STATE_ADULT_STORE should have the desired state when it is in the Adult Category in Store
            stateOfAdultCategory = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STATE_ADULT_STORE");
            if (string.IsNullOrEmpty(stateOfAdultCategory))
            {
                FailStep(CL, "STATE_ADULT_STORE is empty in Test.ini");
            }

            PassStep();
        }
    }
    #endregion

    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            //Navigate and HighLight to the Adult Category in Store
            res = CL.EA.NavigateAndHighlight("STATE:STORE_ADULT_CATEGORY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate and Highlight to Adult option in Store");
            }

            //Selecting on the Adult Category
            CL.EA.UI.Utils.SendIR("SELECT");

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Verifying if it asks to Enter Master Pin
            if (!CL.EA.UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL"))
            {
                FailStep(CL, res, "Failed to verify Insert Pin State after entering Adult Category in Store");
            }

            //Entering Dafault Pin to access the Adult Category
            res = CL.EA.EnterDeafultPIN(stateOfAdultCategory);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify the Access to Adult Category after entering PIN");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}