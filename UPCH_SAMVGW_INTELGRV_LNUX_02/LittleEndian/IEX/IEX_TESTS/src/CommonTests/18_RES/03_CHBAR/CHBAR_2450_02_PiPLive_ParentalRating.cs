/**
 * @file    18_RES\03_CHBAR\CHBAR_2450_02_PiPLive_ParentalRating.cs
 *
 * @brief   Implements the chbar 2450 02 pi p live parental rating class.
 */

/// <summary>
///  Script Name : CHBAR_2450_02_PiPLive_ParentalRating.cs
///  Test Name   : CHBAR-2450-02-PiP Live-Parental-Rating
///  TEST ID     : 68086
///  JIRA ID     : FC-562
///  QC Version  : 1
///  Variations from QC: Verification of no video in PIP instead of picture/text
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**
 * @class   CHBAR_2450_02
 *
 * @brief   Chbar 2450 02.
 *			Execution of this test case requires, locked channel with parental rating high.
 *
 * @author  Varshad
 * @date    17-Sep-13
 */

[Test("CHBAR_2450_02")]
public class CHBAR_2450_02 : _Test
{
    /**
     * @brief   The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The first video service.
     */

    static Service videoService;

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

    private const string STEP1_DESCRIPTION = "Step 1: Tune to channel S1";

    /**
     * @brief   Launch channel bar and browse to locked programme. Verify video is not displayed in PIP.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Launch channel bar and browse to locked programme. Verify video is not displayed in PIP";

    /**
     * @brief   Launch channel bar and browse to locked channel. Verify video is not displayed in PIP.
     */

    private const string STEP3_DESCRIPTION = "Step 3: Launch channel bar and browse to locked channel. Verify video is not displayed in PIP";

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

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**
     * @class   PreCondition
     *
     * @brief   Set Banner time out to MAX and Lock a channel.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    17-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            lockedService = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "");
            if (videoService == null || lockedService == null)
            {
                FailStep(CL, "Failed to fetch service from Content.xml. One of the values is null. VideoService: " + videoService + " Locked Service: " + lockedService);
            }
            LogCommentInfo(CL, "Video Service fetched from conenet.xml is: " + videoService.LCN);
            LogCommentInfo(CL, "Locked Service fetched from conenet.xml is: " + lockedService.LCN);

            serviceTobeLocked = CL.EA.GetServiceFromContentXML("Type=Video", "LCN=" + videoService.LCN + ";ParentalRating=High");
            if (serviceTobeLocked == null)
            {
                FailStep(CL, "Failed to fetch service from Content.xml. " + serviceTobeLocked);
            }
            LogCommentInfo(CL, "Video Service fetched from conenet.xml is: " + serviceTobeLocked.LCN);

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

    /**
     * @class   Step1
     *
     * @brief   Tune to channel S1.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Tune to channel S1.
         *
         * @author  Varshad
         * @date    17-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Surf to channel which is not locked
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService.LCN);
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Launch channel bar and browse to locked programme. Verify video is not displayed in PIP.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Launch channel bar and browse to locked programme. Verify video is not displayed in PIP.
         *
         * @author  Varshad
         * @date    17-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, lockedService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to "+lockedService.LCN+" on channel bar");
            }

            //Verify that video is not present in Specified co-ordinates in PIP
            res = CL.EA.CheckForVideo(coordinates, false, checkVideoTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that video is not present in PIP");
            }

            PassStep();
        }
    }
    #endregion

    #region Step3

    /**
     * @class   Step3
     *
     * @brief   Launch channel bar and browse to locked channel. Verify video is not displayed in PIP.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Launch channel bar and browse to locked channel. Verify video is not displayed in PIP.
         *
         * @author  Varshad
         * @date    17-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Launch channel list and focus on Locked program and verify pip is not available.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, serviceTobeLocked.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res,"Failed to surf to "+serviceTobeLocked.LCN+" on channel bar");
            }

            //Verify that video is not present in Specified co-ordinates in PIP
            res = CL.EA.CheckForVideo(coordinates, false, checkVideoTimeOut);
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

    /**
     * @fn  public override void PostExecute()
     *
     * @brief   Set the banner display time to default and unlock the locked channel.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

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