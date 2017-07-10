/// <summary>
///  Script Name : VOD_0051_Asset_Playback.cs
///  Test Name   : VOD-0051-Asset-Playback
///  TEST ID     : 73835
///  QC Version  : 10
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

[Test("VOD_0051")]
public class VOD_0051 : _Test
{
    [ThreadStatic]
    static _Platform CL;    
    static VODAsset tVodAsset;
    static VODAsset sVodAsset;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play a TVOD";
    private const string STEP2_DESCRIPTION = "Step 2: Play a SVOD";

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

            /*TVOD_TITLE should contain a title of a TVOD asset in STORE*/
            string tVodTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TVOD_TITLE");
            if (string.IsNullOrEmpty(tVodTitle))
            {
                FailStep(CL, "TVOD_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the TVOD asset object
            tVodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + tVodTitle);
            if (tVodAsset == null)
            {
                FailStep(CL, res, "Failed to get TVOD asset from ini file");
            }

            /*SVOD_TITLE should contain a title of a SVOD asset in STORE*/
            string sVodTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SVOD_TITLE");
            if (string.IsNullOrEmpty(sVodTitle))
            {
                FailStep(CL, "SVOD_TITLE is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the SVOD asset object
            sVodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + sVodTitle);
            if (sVodAsset == null)
            {
                FailStep(CL, res, "Failed to get SVOD asset from ini file");
            }
            // Set purchase protection to ON
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to UNLOCK ALL");
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
            res = CL.EA.VOD.PlayAsset(tVodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the TVOD asset");
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

            // Play the SVOD
            res = CL.EA.VOD.PlayAsset(sVodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the SVOD asset");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}

