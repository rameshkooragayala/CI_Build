/// <summary>
///  Script Name : VOD_0052_Trailer_Playback.cs
///  Test Name   : VOD-0051-Trailer-Playback
///  TEST ID     : 73915
///  QC Version  : 10
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 04/04/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0052")]
public class VOD_0052 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play asset trailer";

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
            
            // Get the VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Trailer=True");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get an asset with trailer from ini file");
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

            // Play the TVOD
            res = CL.EA.VOD.PlayTrailer(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play asset trailer");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}


