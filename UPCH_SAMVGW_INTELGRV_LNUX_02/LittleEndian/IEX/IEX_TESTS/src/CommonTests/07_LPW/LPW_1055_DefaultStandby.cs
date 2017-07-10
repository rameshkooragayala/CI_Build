/// <summary>
///  Script Name : The fusion based product shall set the default statdby mode to lukewarm suspend standby
///  Test Name   : LPW-1055-DefaultStandby
///  TEST ID     : 72088
///  QC Version  : 10
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Anshul Upadhyay
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("LPW_1055")]
public class LPW_1055 : _Test
{
    [ThreadStatic]
    static _Platform CL, GW;

    //value of this variable will be fetched from test.ini
    static string nonDefaultPowerMode = "";

    //this value will be fetched from project.ini
    static string defaultPowerMode = "";

    private static string defaultPin;

    static bool isHomeNet = false;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch the required paramenters from INI files";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Standby mode settings";
    private const string STEP2_DESCRIPTION = "Step 2: Verify that the default state is Lukewarm Suspend";
    private const string STEP3_DESCRIPTION = "Step 3: Change the setting to Cold standby";
    private const string STEP4_DESCRIPTION = "Step 4: Perform factory reset and select NO THANKS for power disclaimer message";
    private const string STEP5_DESCRIPTION = "Step 5: Verify that the default mode of Gateway is set to Lukewarm Suspend after factory reset";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
        
        string isHomeNetwork = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IsHomeNetwork");

        //If Home network is true perform GetGateway
        isHomeNet = Convert.ToBoolean(isHomeNetwork);
        if (isHomeNet)
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
            
            //Get Values From ini File
            nonDefaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NON_DEFAULT_POWER_MODE");
            if (string.IsNullOrEmpty(nonDefaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }
            
            //get Value from Project INI file
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, "Failed to get default power mode value from project INI file");
            }
            
            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to STANDBY POWER USAGE");
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

            String defaultMode = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get default standby state");
            }

            if (defaultPowerMode.ToLower() == defaultMode.ToLower())
            {
                LogComment(CL, "Default power mode is " + defaultPowerMode);
            }
            else
            {
                FailStep(CL, "Default power mode is not " + defaultMode + " Default power mode should be " + defaultPowerMode);
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();


            res = CL.EA.STBSettings.SetPowerMode(nonDefaultPowerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + nonDefaultPowerMode);
            }


            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Perform Factory reset with Keep User Settings option
            res = CL.EA.STBSettings.FactoryReset(SaveRecordings:false, KeepCurrentSettings:false, PinCode:defaultPin );
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }
            
            if (isHomeNet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }

                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount client");
                }
            }

            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to STANDBY POWER USAGE");
            }

            String defaultMode = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultMode);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get default standby state");
            }

            if (defaultPowerMode.ToLower() == defaultMode.ToLower())
            {
                LogComment(CL, "Default power mode is " + defaultPowerMode);
            }
            else
            {
                FailStep(CL, "Default power mode is not " + defaultPowerMode);
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
        res = CL.EA.STBSettings.SetPowerMode(defaultPowerMode);
        if (!(res.CommandSucceeded))
        {
           LogCommentFailure(CL,"Failed to set the power mode option to " + defaultPowerMode);
        }
    }
    #endregion
}