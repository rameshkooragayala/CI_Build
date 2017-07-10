/// <summary>
///  Script Name : CHBAR_0501_DefaultFocus_LiveAndRB.cs
///  Test Name   : EPG-0501-Channel Bar-Default Focus while Live and Playback of RB
///  TEST ID     : 64224
///  JIRA ID     : FC-464
///  QC Version  : 1
///  Variations from QC:NONE
/// -----------------------------------------------
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("CHBAR_0501")]
public class CHBAR_0501 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoServiceS1;
    private static Service videoServiceS2;
    private static Service videoServiceS3;
    private static int rewindMin;
    private static int safeDelayStandBy;
    private static Helper helper = new Helper();

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File & build E2 from S2 in review buffer";
    private const string STEP1_DESCRIPTION = "Step 1: Launch channel bar on S1 and verfy default focus is on S1";
    private const string STEP2_DESCRIPTION = "Step 2: Playback E2 from S2 which is there in review buffer and verify default focus is on S2";
    private const string STEP3_DESCRIPTION = "Step 3: Zap to S3 and launch channel bar. Verify focus is on S3";

    private static class Constants
    {
        public const int waitPeriodForRBCollection = 2 * 60; // 2 mins
        public const int timeOut = 20;
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

            //Get services from xml File
            videoServiceS1 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            videoServiceS2 = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=Low", "LCN=" + videoServiceS1.LCN);
            videoServiceS3 = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=Low", "LCN=" + videoServiceS1.LCN + "," + videoServiceS2.LCN);

            if (videoServiceS1 == null || videoServiceS2 == null || videoServiceS3 == null)
            {
                FailStep(CL, "One if the Service is null. videoServiceS1: " + videoServiceS1 + " videoServiceS2: " + videoServiceS2 + " videoServiceS3: " + videoServiceS3);
            }
            else
            {
                LogCommentInfo(CL, "First video service videoServiceS1: " + videoServiceS1.LCN);
                LogCommentInfo(CL, "First video service videoServiceS2: " + videoServiceS2.LCN);
                LogCommentInfo(CL, "First video service videoServiceS3: " + videoServiceS3.LCN);
            }

            //Get value from Project.ini
            string rewindMinStringVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            string safeDelayStandByString = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");

            if (string.IsNullOrEmpty(rewindMinStringVal) || string.IsNullOrEmpty(safeDelayStandByString))
            {
                FailStep(CL, "One of the values is empty or null. REW_MIN: " + rewindMinStringVal + ", SAFE_DELAY_SEC: " + safeDelayStandBy);
            }
            else
            {
                LogCommentInfo(CL, "REW_MIN: " + rewindMinStringVal);
                LogCommentInfo(CL, "SAFE_DELAY_SEC: " + safeDelayStandByString);
            }

            rewindMin = Int16.Parse(rewindMinStringVal);
            safeDelayStandBy = Int16.Parse(safeDelayStandByString);

            LogCommentInfo(CL, "Trick mode value for rewind: " + rewindMin);
            LogCommentInfo(CL, "Safe delay stand by: " + safeDelayStandBy);

            //Stand by entry and exit to flush previous review buffer
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "");
            }

            LogCommentInfo(CL, "Wait for " + safeDelayStandBy + " after entering stand by");
            res = CL.IEX.Wait(safeDelayStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for " + safeDelayStandBy + " after entering stand by");
            }

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.EA.CheckForVideo(true, true, Constants.timeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
            }

            //Build E2 from S2 in persistent review buffer
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoServiceS2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoServiceS2: " + videoServiceS2.LCN);
            }

            LogCommentInfo(CL, "Waiting for 2 mins for Review Buffer collection");
            res = CL.IEX.Wait(Constants.waitPeriodForRBCollection);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for 2 mins");
            }

            // currentTest = new CHBAR_0501();

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

            //Channel Surf to video Service S1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoServiceS1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoServiceS1: " + videoServiceS1.LCN);
            }

            bool result = helper.VerifyChannelInfo(videoServiceS1);
            if (!result)
            {
                FailStep(CL, "Failed to verify channel information");
            }

            //Verify live state within channel bar time out set in settings
            if (!CL.EA.UI.Utils.VerifyState("LIVE", Constants.timeOut))
            {
                FailStep(CL, res, "Failed to verify Live state");
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

            //Play back E2 on S2 from persistent review buffer
            res = CL.EA.PVR.SetTrickModeSpeed("RB", rewindMin, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to rewind till BOF");
            }

            bool result = helper.VerifyChannelInfo(videoServiceS2);
            if (!result)
            {
                FailStep(CL, "Failed to verify channel information");
            }

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

            //Channer Surf to videoService S3.
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoServiceS3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoServiceS3: " + videoServiceS3.LCN);
            }

            bool result = helper.VerifyChannelInfo(videoServiceS3);
            if (!result)
            {
                FailStep(CL, "Failed to verify channel information");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Helper

    public class Helper : _Step
    {
        public override void Execute()
        {
        }

        //Verify channel information
        public bool VerifyChannelInfo(Service videoService)
        {
            IEXGateway._IEXResult res;

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to clear EPG info");
            }

            //Launch Channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to navigate to channel bar");
                return false;
            }

            //Verify if the default focus is videoService
            //Verfy channel number
            string obtainedChannelNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedChannelNum);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to get value for key: chNum");
                return false;
            }

            if (!obtainedChannelNum.Equals(videoService.LCN))
            {
                LogCommentFailure(CL, "Channel number is not same as" + videoService.LCN + ". Obtained chNum: " + obtainedChannelNum);
                return false;
            }

            //Verify channel name
            string obtainedChannelName = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out obtainedChannelName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to get value for key: chName");
                return false;
            }

            if (!obtainedChannelName.Equals(videoService.Name))
            {
                LogCommentFailure(CL, "Channel name is not same as" + videoService.Name + ". Obtained chName: " + obtainedChannelName);
                return false;
            }
            return true;
        }
    }

    #endregion Helper

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}