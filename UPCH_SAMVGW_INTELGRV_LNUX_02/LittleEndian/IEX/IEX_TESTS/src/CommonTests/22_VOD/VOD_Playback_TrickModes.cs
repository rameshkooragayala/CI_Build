/// <summary>
///  Script Name : VOD_Playback_TrickModes.cs
///  Test Name   : VOD-0345-VOD-Playback-Slow-Motion, VOD-0346-VOD-Playback-Fast-Forward, VOD-0347-VOD-Playback-Fast-Reverse
///  TEST ID     : 73841
///  QC Version  : 9
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

[Test("VOD_TrickModes")]
public class VOD_TrickModes : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static VODAsset vodAsset;
    static string[] fwdTrickModeSpeedArray;
    static string[] rewTrickModeSpeedArray;
    static class Constants
    {
        public const string EVENT_KEY_NAME = "vodAsset";
        public const double SLOW_MOTION_SPEED = 0.5;
    }
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters from .ini file";
    private const string STEP1_DESCRIPTION = "Step 1: Play a VOD asset";
    private const string STEP2_DESCRIPTION = "Step 2: Request slow motion trick mode";
    private const string STEP3_DESCRIPTION = "Step 3: Request fast forward with all possible speeds";
    private const string STEP4_DESCRIPTION = "Step 4: Request fast reverse with all possible speeds";

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

            // Get a VOD asset object
            vodAsset = CL.EA.GetVODAssetFromContentXML("Price=0");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a free VOD asset from ini file");
            }

            // Insert the vod asset into the event collection
            CL.EA.UI.Utils.InsertEventToCollection(Constants.EVENT_KEY_NAME, vodAsset.Title, "VOD", "", "", "", Int32.Parse(vodAsset.AssetDuration) * 60, 0, "", "", 0, "");

            // Fetch the supported FWD speeds
            string fwdTrickModeSpeeds = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD");
            if (string.IsNullOrEmpty(fwdTrickModeSpeeds))
            {
                FailStep(CL, "Forward Trick mode list not present in Project.ini file.");
            }
            fwdTrickModeSpeedArray = fwdTrickModeSpeeds.Split(',');

            // Fetch the supported FWD speeds
            string rewTrickModeSpeeds = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            if (string.IsNullOrEmpty(rewTrickModeSpeeds))
            {
                FailStep(CL, "Forward Trick mode list not present in Project.ini file.");
            }
            rewTrickModeSpeedArray = rewTrickModeSpeeds.Split(',');

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

            // Play the VOD asset
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the VOD asset");
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

            // Request slow motion trick mode
            res = CL.EA.PVR.SetTrickModeSpeed(Constants.EVENT_KEY_NAME, Constants.SLOW_MOTION_SPEED, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set slow motion trick mode");
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

            double fwdSpeed;
            foreach (string speed in fwdTrickModeSpeedArray)
            {
                // FWD 
                fwdSpeed = Double.Parse(speed);
                res = CL.EA.PVR.SetTrickModeSpeed(Constants.EVENT_KEY_NAME, fwdSpeed, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play the asset at x " + fwdSpeed + " Speed");
                }
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

            double rewSpeed;
            foreach (string speed in rewTrickModeSpeedArray)
            {
                // REW
                rewSpeed = Double.Parse(speed);
                res = CL.EA.PVR.SetTrickModeSpeed(Constants.EVENT_KEY_NAME, rewSpeed, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play the asset at x " + rewSpeed + " Speed");
                }
            }

            PassStep();
        }
    }
    #endregion
    #endregion
}


