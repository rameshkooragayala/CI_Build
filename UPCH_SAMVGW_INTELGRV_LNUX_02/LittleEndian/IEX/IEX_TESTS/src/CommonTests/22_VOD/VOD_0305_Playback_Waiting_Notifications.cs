/// <summary>
///  Script Name : VOD_0305_Playback_Waiting_Notifications.cs
///  Test Name   : VOD-0305-Playback_Waiting_Notifications
///  TEST ID     : 74527
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 08/04/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD_0305")]
public class VOD_0305 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play an asset and check waiting notification is displayed";

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
            vodAsset = CL.EA.GetVODAssetFromContentXML("Price=0");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a free VOD asset from ini file");
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

            // Focus a VOD asset
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check waiting notification when playing a VOD asset");
            }


            PassStep();
        }
    }
    #endregion
    #endregion
}


