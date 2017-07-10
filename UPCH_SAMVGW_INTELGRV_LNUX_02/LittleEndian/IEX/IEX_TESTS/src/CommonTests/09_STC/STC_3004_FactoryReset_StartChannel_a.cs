/// <summary>
///  Script Name : STC_3004_FactoryReset_StartChannel_a.cs
///  Test Name   : STC-3004-start channel-factory reset restore settings
///  TEST ID     : 71557
///  QC Version  : 1
///  Variations from QC: NONE
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

[Test("STC_3004_a")]
public class STC_3004_a : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;

    //Shared members between steps

    static Service defaultVideoService;

    static Service firstService;

    static bool ishomenet = false;

    static string startChannelType;

    static string defaultPin;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Perform Factory Reset";
    private const string STEP2_DESCRIPTION = "Step 2: Verify STB has tuned to valid start channel";

    private static class Constants
    {      
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

        //Get Client Platform
        CL = GetClient();

        string isHomeNetwork = CL.EA.GetTestParams("IsHomeNetwork");

        //If Home network is true perform GetGateway
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

            //Get Values From ini File
            defaultVideoService = CL.EA.GetServiceFromContentXML("IsDefault=True", "ParentalRating=High");
            if (defaultVideoService == null)
            {
                FailStep(CL, "Video service fetched from Content.xml is null");
            }

            firstService = CL.EA.GetServiceFromContentXML("PositionOnList=1", "ParentalRating=High");
            if (firstService == null)
            {
                FailStep(CL, "Video service fetched from Content.xml is null");
            }

            startChannelType = CL.EA.GetValueFromINI(EnumINIFile.Project, "START_CHANNEL", "START_CHANNEL_TYPE");
            if (string.IsNullOrEmpty(startChannelType))
            {
                FailStep(CL, "START_CHANNEL_TYPE is null.");
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

            res = CL.EA.STBSettings.FactoryReset(Constants.keepRecordings, Constants.keepSettings, defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }

            if (ishomenet)
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
                    FailStep(CL, "Failed to mount cleint");
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Channel Bar");
            }

            string obtainedChName = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedChName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get ");
            }

            if (startChannelType.Equals("DEFAULT_CHANNEL"))
            {
                //Verify tuned to defatlt channel
                if (!obtainedChName.Equals(defaultVideoService.LCN))
                {
                    FailStep(CL, res, "Failed to tune to default channel. Default channel is: " + 
                        defaultVideoService.LCN + "Currently tuned channel is: " + obtainedChName);
                }
            }
            else if (startChannelType.Equals("FIRST_CHANNEL"))
            {
                if (!obtainedChName.Equals(firstService.LCN))
                {
                    FailStep(CL, res, "Failed to tune to first channel. First channel is: " +
                        firstService.LCN + "Currently tuned channel is: " + obtainedChName);
                }
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

    }
    #endregion
}