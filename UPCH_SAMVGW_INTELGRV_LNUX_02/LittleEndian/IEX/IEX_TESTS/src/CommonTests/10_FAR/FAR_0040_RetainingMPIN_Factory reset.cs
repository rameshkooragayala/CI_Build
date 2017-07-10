/// <summary>
///  Script Name : FAR_0040_RetainingMPIN_Factory_reset.cs
///  Test Name   : FAR-0040-Retaining MPIN on Factory reset
///  TEST ID     : 71584
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FAR_0040")]
public class FAR_0040 : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;

    static bool ishomenet = false;
	
	private static string defaultPin;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Change Master PIN Code";
    private const string STEP1_DESCRIPTION = "Step 1: Perform Factory reset";
    private const string STEP2_DESCRIPTION = "Step 2: Verify that new pin code is retained";

    private static class Constants
    {
        public const string newPIN = "1234";
        public const bool keepRecordings = false;
        public const bool keepSettings = false;
    }


    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        CL = GetClient();
        string isHomeNetwork = CL.EA.GetTestParams("IsHomeNetwork");
        ishomenet = Convert.ToBoolean(isHomeNetwork);

        if (ishomenet)
        {
            //Get gateway platform
            GW = GetGateway();

        }
        
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
			
			defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }

            res = CL.EA.STBSettings.ChangePinCode(defaultPin, Constants.newPIN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to change pin code");
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

            res = CL.EA.STBSettings.FactoryReset(Constants.keepRecordings, Constants.keepSettings,Constants.newPIN);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to perform factory reset");
            }

            if (ishomenet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to mount gateway");
                }
                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to mount client");
                }
            }
            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to mount cleint");
                }
            }
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANGE PIN CODE");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to navigate to Change PIN code state.");
            }

            res = CL.EA.EnterPIN(Constants.newPIN, "CHANGE PIN CODE");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to navigate to Change PIN code state. Enterd Pin code is incorrect");
            }

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        res = CL.EA.STBSettings.ChangePinCode(Constants.newPIN, defaultPin);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to reset Pin code to default");
        }
    }
    #endregion
}