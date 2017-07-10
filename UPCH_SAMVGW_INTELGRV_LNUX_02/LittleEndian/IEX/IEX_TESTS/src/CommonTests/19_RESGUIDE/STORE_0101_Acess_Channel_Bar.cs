/// <summary>
///  Script Name : STORE_0101_Access_Channel_Bar.cs
///  Test Name   : STORE-0101-Access-Channel-Bar
///  TEST ID     : 73842
///  QC Version  : 6
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 20/03/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STORE_0101")]
public class STORE_0101 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Access STORE from CHANNEL BAR";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
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
            
            // Get a catchup VOD asset
            vodAsset = CL.EA.GetVODAssetFromContentXML("IsCatchUp=True");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get catchUp VOD asset from ini file");
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

            //Tune to a channel with catchup
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, vodAsset.CatchupService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + vodAsset.CatchupService);
            }

            //Enter STORE from CHANNEL BAR
            res = CL.EA.NavigateAndHighlight(vodAsset.NamedNavigation);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enter STORE from CHANNEL BAR");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}

