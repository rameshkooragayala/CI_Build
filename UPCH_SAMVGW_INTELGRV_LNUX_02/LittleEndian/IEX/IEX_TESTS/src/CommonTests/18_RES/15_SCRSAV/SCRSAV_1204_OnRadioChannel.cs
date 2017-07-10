/// <summary>
///  Script Name : SCRSAV_1204_OnRadioChannel.cs
///  Test Name   : SCRSAV-1204-On Radio Channel
///  TEST ID     : 64526
///  JIRA ID     :FC-510
///  QC Version  : 1
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by : Appanna Kangira
///  Modified Date: 10/02/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("SCRSAV-1204-On Radio Channel")]
public class SCRSAV_1204 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service radioChannel;
    private static Service avService;
    private static string obtainedRadioScrSvrStatus = "";
    private static string obtainedAVScrSvrStatus = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to Radio Channel";
    private const string STEP2_DESCRIPTION = "Step 2: Check the Activation/Launch of Screen Saver";
    private const string STEP3_DESCRIPTION = "Step 3: Tune to AV Channel and check for the Dismissal of Screensaver";

    private static class Constants
    {
        public const string scrnSaverActive = "ON";
        public const string scrnSaverInActive = "OFF";
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From XML File
            radioChannel = CL.EA.GetServiceFromContentXML("Type=Radio", "ParentalRating=High");
            avService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");

            if (radioChannel.Equals(null))
            {
                FailStep(CL, "Failed to retrieve radiochannel");
            }

            if (avService.Equals(null))
            {
                FailStep(CL, "Failed to retrieve avService");
            }
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.TuneToRadioChannel(radioChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Radio Channel : " + " With DCA" + "radio Channel:" + radioChannel.LCN);
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string screenSaverWait;
             
            try
            {
            screenSaverWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "SCREENSAVER_WAIT");
            }
           catch (Exception ex)
            {
            screenSaverWait = "0";
            LogCommentWarning(CL,"SCREENSAVER_WAIT not defined in project INI,Default Value is 0 Secs ");
            }

            double screenSaverWaitInSecs= Convert.ToDouble( screenSaverWait);
           
            //Wait for the ScreenSaver to Appear after tuning to radio Service
            CL.IEX.Wait(screenSaverWaitInSecs);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("screensaver", out  obtainedRadioScrSvrStatus);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get ScreenSaver Status");
            }
            if (obtainedRadioScrSvrStatus != Constants.scrnSaverActive)
            {
                FailStep(CL, "Screen Saver not active" + "expected:" + Constants.scrnSaverActive + "Obtained:" + obtainedRadioScrSvrStatus);
            }

            LogCommentInfo(CL, "Screen Saver ON");

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, avService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to AV Service : " + " With DCA" + avService.LCN);
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("screensaver", out  obtainedAVScrSvrStatus);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get ScreenSaver Status");
            }

            if (obtainedAVScrSvrStatus != Constants.scrnSaverInActive)
            {
                FailStep(CL, "Screen Saver active on AV Service" + "expected:" + Constants.scrnSaverInActive + "Obtained:" + obtainedAVScrSvrStatus);
            }

            LogCommentInfo(CL, "Screen Saver dismissed");
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}