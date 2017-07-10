/// <summary>
///  Script Name : STORE_0240_Navigation_Background_Image.cs
///  Test Name   : STORE_0240_Navigation_Background_Image
///  TEST ID     : 74521
///  QC Version  : 1
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 25.07.2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STORE_0240")]
public class STORE_0240 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static string backgroundImage;
    static string assetTitle_in_categoryWithBackgound;
    static string assetTitle_out_categoryWithBackgound;
    static VODAsset vodAsset_in_categoryWithBackgound;
    static VODAsset vodAsset_out_categoryWithBackgound;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the navigation paths from test.ini";
    private const string STEP1_DESCRIPTION = "Step 1: Enter a classification where there is a background image related to this classification";
    private const string STEP2_DESCRIPTION = "Step 2: Enter a classification where there is a no background image related to this classification";

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

            // ASSET_IN_CAT_WITH_BACKGROUND contains the navigation path to an asset inside a category with background
            assetTitle_in_categoryWithBackgound = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_IN_CAT_WITH_BACKGROUND");
            if (string.IsNullOrEmpty(assetTitle_in_categoryWithBackgound))
            {
                FailStep(CL, "ASSET_IN_CAT_WITH_BACKGROUND is empty in Test.ini");
            }

            // ASSET_OUT_CAT_WITH_BACKGROUND contains the navigation path to an asset outside a category with background
            assetTitle_out_categoryWithBackgound = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ASSET_OUT_CAT_WITH_BACKGROUND");
            if (string.IsNullOrEmpty(assetTitle_out_categoryWithBackgound))
            {
                FailStep(CL, "ASSET_OUT_CAT_WITH_BACKGROUND is empty in Test.ini");
            }

            // Get the VODAsset objects        
            vodAsset_in_categoryWithBackgound = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle_in_categoryWithBackgound);
            if (vodAsset_in_categoryWithBackgound == null)
            {
                FailStep(CL, res, "Failed to get asset " + assetTitle_in_categoryWithBackgound + " from Content.xml");
            }
      
            vodAsset_out_categoryWithBackgound = CL.EA.GetVODAssetFromContentXML("Title=" + assetTitle_out_categoryWithBackgound);
            if (vodAsset_out_categoryWithBackgound == null)
            {
                FailStep(CL, res, "Failed to get asset " + assetTitle_out_categoryWithBackgound + " from Content.xml");
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

            // clear EPG info
            CL.IEX.MilestonesEPG.ClearEPGInfo();

            // enter a classification where there is a background image
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset_in_categoryWithBackgound, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to a classification with background image");
            }
             
            // check background image is displayed
            res = CL.IEX.MilestonesEPG.GetEPGInfo("VOD background image", out backgroundImage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Background image is not displayed. Expected: background should be displayed");
            }
			if (backgroundImage == "")
            {
                FailStep(CL, res, "Background image is not displayed. Expected: background image shoul be displayed");
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

            // clear EPG info
            CL.IEX.MilestonesEPG.ClearEPGInfo();

            // enter a classification where there is no background image
            res = CL.EA.VOD.NavigateToVODAsset(vodAsset_out_categoryWithBackgound, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to a classification without background image");
            }

            // check background image is not displayed
            res = CL.IEX.MilestonesEPG.GetEPGInfo("VOD background image", out backgroundImage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Couldn't check if background image is displayed or not: unable to get 'VOD background image' milestone");
            }
            if (backgroundImage != "")
            {
                FailStep(CL, res, "Background image is displayed. Expected: background image shouldn't be displayed");
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}
