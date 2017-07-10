/// <summary>
///  Script Name : STORE_0211_Navigation_AdultDismiss.cs
///  Test Name   : STORE-0211-Navigation-Browsing-Adult-Classification-Dismiss-PIN
///  TEST ID     : 20234
///  QC Version  : 6
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

[Test("STORE_0211")]
public class STORE_0211 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    private const string STEP1_DESCRIPTION = "Step 1: Navigate to the Adult Category on STORE";
    private const string STEP2_DESCRIPTION = "Step 2: Check if it ask to Insert Pin to Access the Adult Category and then dismiss it (insert wrong PIN code)";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
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

            //Enter wrong PIN
            if (!enterWrongPIN("INSERT PIN UNLOCK CHANNEL"))
            {
                FailStep(CL, res, "Failed to verify Insert Pin State after entering a wrong PIN code");    
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    static private bool enterWrongPIN(string nextState)
    {
        //Get default PIN code
        string defaultPIN = CL.EA.UI.Utils.GetValueFromEnvironment("DefaultPIN");

        //Produce a random PIN code different from default PIN code
        string wrongPIN;
        Random rnd = new Random();
        do
        {
            wrongPIN = rnd.Next(0, 10000).ToString("0000");
        } 
        while (wrongPIN == defaultPIN);     

        //Enter wrong PIN code and check state
        IEXGateway._IEXResult res = CL.EA.EnterPIN(wrongPIN, nextState);
        if (!res.CommandSucceeded)
        {
            return false;
        }       
        return true;
    }
}