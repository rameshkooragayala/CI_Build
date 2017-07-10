/// <summary>
///  Script Name : VOD_0372_PP_VOD_Adult_Session_Stop_Playback.cs
///  Test Name   : VOD_0372_PP_VOD_Adult_Session_Stop_Playback
///  TEST ID     : 74535
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 01/08/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0372")]
public class VOD_0372 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    static string expectedClassification;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play an adult asset";
    private const string STEP2_DESCRIPTION = "Step 2: Stop the playback and check the STB jumps to the last non-adult category";
    private const string STEP3_DESCRIPTION = "Step 3: Replay the adult asset";
    private const string STEP4_DESCRIPTION = "Step 4: Wait till the end of playback and check the STB jumps to the last non-adult category";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
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

            /* ASSET_TITLE should contain the title of an asset in an Adult category */
            string assetTitle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_TITLE");
            if (string.IsNullOrEmpty(assetTitle))
            {
                FailStep(CL, "ASSET_TITLE is empty in Test.ini in section TEST_PARAMS");
            }
            
            /* LAST_NON_ADULT_CLASSIFICATION should contain the classification name of the last visited non-adult classification */
            expectedClassification = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LAST_NON_ADULT_CLASSIFICATION");
            if (string.IsNullOrEmpty(expectedClassification))
            {
                FailStep(CL, "LAST_NON_ADULT_CLASSIFICATION is empty in Test.ini in section TEST_PARAMS");
            }

            // Get the VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle);
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset '" + assetTitle + "' from Content.xml");
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

            // Play the adult asset
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select the adult asset");
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

            // Stop the playback          
            res = CL.EA.VOD.StopAssetPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            // Check the current classification
            string currentClassification = CL.EA.UI.Vod.GetCurrentClassification();
            if (currentClassification != expectedClassification)
            {
                FailStep(CL, res, "Wrong classification after playback stop. Expected: " + expectedClassification + ", Returned: " + currentClassification);
            }

            PassStep();
        }
    }
    #endregion

    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Play the adult asset
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to select the adult asset");
            }

            PassStep();
        }
    }
    #endregion

    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Wait till end of playback          
            res = CL.IEX.Wait((Int32.Parse(vodAsset.AssetDuration) + 1) * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the playback");
            }

            // Check the current classification
            string currentClassification = CL.EA.UI.Vod.GetCurrentClassification();
            if (currentClassification != expectedClassification)
            {
                FailStep(CL, res, "Wrong classification after playback stop. Expected: " + expectedClassification + ", Returned: " + currentClassification);
            }

            PassStep();
        }
    }
    #endregion

    #endregion
}
