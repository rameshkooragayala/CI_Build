
/// <summary>
///  Script Name : CHBAR_2450_03_PiPLive_ChannelLocked.cs
///  Test Name   :CHBAR_2450_03_PiPLive_ChannelLocked
///  TEST ID     : 
///  JIRA ID     : FC-562
///  QC Version  : 1
///  Variations from QC: Verification of no video in PIP instead of picture/text
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar C
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("CHBAR_2450_03")]
public class CHBAR_2450_03: _Test
{
    /**
     * @brief   The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The first video service.
     */

    static Service videoService1;


    static Service videoService2;
    /**
     * @brief   The service to be Locked.
     */

    static Service serviceTobeLocked;

    /**
     * @brief   The locked service.
     */

    static Service lockedService;

    /**
     * @brief   The on channel bar PIP coordinates.
     */

    static string ChannelList = "";

    static string coordinates;

    /**
     * @brief   The check video time out.
     */

    static int checkVideoTimeOut;

    /**
     * @brief   The delay state transition.
     */

    static int delayStateTransition;

    /**
    * @brief   The is locked.
    */

    static bool isLocked = false;

    /**
     * @brief   The maximum channel bar time out value.
     */

    static EnumChannelBarTimeout maxChannelBarTimeOutVal;

    /**
     * @brief   The default channel bar time out value.
     */

    static EnumChannelBarTimeout defaultChannelBarTimeOutVal;

    /**
     * @brief   Precondition: Set Banner time out to MAX and Lock a channel.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Set Banner time out to MAX and Lock a channel";

    /**
     * @brief   Tune to channel S1.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Recoridng is scheduled on event E2 on Service S1 which is the next event. ";

    /**
     * @brief   Launch channel bar and browse to locked programme. Verify video is not displayed in PIP.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Launch channel bar and browse to locked channel. Verify video is not displayed in PIP";

    /**
     * @brief   Launch channel bar and browse to locked channel. Verify video is not displayed in PIP.
     */

    private const string STEP3_DESCRIPTION = "Step 3: Launch channel bar and browse to video service s2. Verify video is displayed in PIP";

    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step2(), STEP3_DESCRIPTION);

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

            //Get List of Channel from TEST ini File
            ChannelList = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channelNumberList");
            if (string.IsNullOrEmpty(ChannelList))
            {
                FailStep(CL, res, "Unable to fetch the ChannelList from test ini file");
            }
            string[] ChannelListNumber = ChannelList.Split(',');

            //Get Values From ini File
            videoService1 = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[0].Trim(), "ParentalRating=High");
            if (videoService1 == null)
            {
                FailStep(CL, "Failed to fetch videoService1 from Content.xml");
            }
            LogCommentInfo(CL, "Video Service1 fetched from content.xml is: " + videoService1.LCN);

            serviceTobeLocked = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[1].Trim(), "ParentalRating=High");
            if (serviceTobeLocked == null)
            {
                FailStep(CL, "Failed to fetch serviceTobeLocked from Content.xml");
            }
            LogCommentInfo(CL, "service To beLocked fetched from content.xml is: " + serviceTobeLocked.LCN);

            videoService2 = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[2].Trim(), "ParentalRating=High");
            if (videoService2 == null)
            {
                FailStep(CL, "Failed to fetch videoService2 from Content.xml");
            }

            LogCommentInfo(CL, "videoService2 fetched from content.xml is: " + videoService2.LCN);

            coordinates = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "COORDINATES_FOR_PIP");
            string videoTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            string channelBarTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");

            if (string.IsNullOrEmpty(coordinates) || string.IsNullOrEmpty(videoTimeOut) || string.IsNullOrEmpty(channelBarTimeOut))
            {
                FailStep(CL, "One of the values is null. COORDINATES_FOR_PIP ON CHANNEL_BAR : " + coordinates + " , DEFAULT_VIDEO_CHECK_SEC: " + videoTimeOut + " & DELAY_STATE_TRANSITION: " + channelBarTimeOut);
            }
            checkVideoTimeOut = Int32.Parse(videoTimeOut);
            delayStateTransition = Int32.Parse(channelBarTimeOut);

            LogCommentInfo(CL, "COORDINATES_FOR_PIP on CHANNEL BAR: " + coordinates);
            LogCommentInfo(CL, "DEFAULT_VIDEO_CHECK_SEC: " + checkVideoTimeOut);
            LogCommentInfo(CL, "DELAY_STATE_TRANSITION: " + delayStateTransition);

            string bannerTimeout = "";

            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");
            if (string.IsNullOrEmpty(bannerTimeout))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, MAX fetched from Project.ini is null or empty", false);
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out maxChannelBarTimeOutVal);
            LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> MAX = " + maxChannelBarTimeOutVal);


            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
            if (string.IsNullOrEmpty(bannerTimeout))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, DEFAULT fetched from Project.ini is null or empty", false);
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out defaultChannelBarTimeOutVal);
            LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> DEFAULT = " + defaultChannelBarTimeOutVal);


            //Change Timeout Duration in Channel Bar Timeout settings
            res = CL.EA.STBSettings.SetBannerDisplayTime(maxChannelBarTimeOutVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to change Banner Display Time to:" + maxChannelBarTimeOutVal, false);
            }

            res = CL.EA.STBSettings.SetLockChannel(serviceTobeLocked.Name);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set lock on channel: " + serviceTobeLocked.LCN);
            }
            isLocked = true;

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

            //Surf to channel S1 which is not locked 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService1.LCN);
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

            //Launch channel list and focus on Locked channel
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, serviceTobeLocked.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to " + serviceTobeLocked.LCN + " on channel bar");
            }

            //Dedicated generic picture/text for locked channel displayed in PiP window  
            string PiPtitleText = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out PiPtitleText);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get text for locked channel displayed in PiP window");
            }

            if (PiPtitleText != "UNLOCK")
            {
                FailStep(CL, "Dedicated generic picture/text for locked channel not displayed in PiP window");
            }

            LogCommentInfo(CL, "Dedicated generic picture/text for locked channel = " + PiPtitleText);

            //Verify that video is not present in Specified co-ordinates in PIP
            res = CL.EA.CheckForVideo(coordinates, false, checkVideoTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that video is not present in PIP");
            }

            //Dedicated generic picture/text for locked channel displayed in PiP window           

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

            //Launch channel list and focus on videoService2 channel which is not locked.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, videoService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to " + videoService2.LCN + " on channel bar");
            }

            // Video of focused channel S3 displayed in PiP mode.
            res = CL.EA.CheckForVideo(coordinates, true, checkVideoTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that video is not present in PIP");
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
        IEXGateway._IEXResult res;

        //Set Channel Bar Time Out to Default 
        res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to change Banner Display Time to:" + defaultChannelBarTimeOutVal);
        }

        //Unlock the locked channel
        if (isLocked)
        {
            res = CL.EA.STBSettings.SetUnLockChannel(serviceTobeLocked.LCN);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to unlock on channel: " + serviceTobeLocked.LCN);
            }
        }
    }
    #endregion
}