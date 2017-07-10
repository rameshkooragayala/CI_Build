/// <summary>
///  Script Name : CHBAR_2450_04_BadSignal.cs
///  Test Name   : CHBAR-2450-04-PiP Live-Bad signal
///  TEST ID     : 68375
///  JIRA ID     : FS-560
///  QC Version  : 1
///  Variations from QC: None
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
 * @class   CHBAR_2450_02_BadSignal
 *
 * @brief   Chbar 2450 02 bad signal.
 *
 * @author  Varshad
 * @date    18-Sep-13
 */

[Test("CHBAR_2450_04_BadSignal")]
public class CHBAR_2450_04 : _Test
{
    /**
     * @brief   The _Platform cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The video service.
     */

    static Service videoService;

    /**
     * @brief   The PIP coordinates.
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
     * @brief   The maximum channel bar time out value.
     */

    static EnumChannelBarTimeout maxChannelBarTimeOutVal;

    /**
     * @brief   The default channel bar time out value.
     */

    static EnumChannelBarTimeout defaultChannelBarTimeOutVal;

    /**
     * @brief   The default channel bar time out value.
     */

    static string rfSwitch;

    /**
     * @brief   Flag to check whether RF is active.
     */

    private static bool isRFActive = true;

    /**
     * @brief   Get Channel Numbers From xml File and set banner timeout value to maximum.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and set banner timeout value to maximum";

    /**
     * @brief   Channel surf to video service, and verify video is present in PIP on channel list.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Channel surf to video service, and verify video is present in PIP on channel list ";

    /**
     * @brief   Remove RF then launch channel list and verify video is not present in PIP.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Remove RF then launch channel list and verify video is not present in PIP";


    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    18-Sep-13
     */

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

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    18-Sep-13
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
     * @brief   Get Channel Numbers From xml File and set banner timeout value to maximum.
     *
     * @author  Varshad
     * @date    18-Sep-13
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
         * @date    18-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
           
            if (videoService == null)
            {
                FailStep(CL, "Failed to fetch service from Content.xml. One of the values is null. VideoService: " + videoService);
            }
            LogCommentInfo(CL, "Video Service fetched from conenet.xml is: " + videoService.LCN);
 
            coordinates = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "COORDINATES_FOR_PIP");
            string videoTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            string channelBarTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");
            rfSwitch = CL.EA.GetValueFromINI(EnumINIFile.Project, "RF_SWITCH", "RF_SWITCH");

            if (string.IsNullOrEmpty(coordinates) || string.IsNullOrEmpty(videoTimeOut) || string.IsNullOrEmpty(channelBarTimeOut) || string.IsNullOrEmpty(rfSwitch))
            {
                FailStep(CL, "One of the values is null. COORDINATES_FOR_PIP ON CHANNEL_BAR : " + coordinates + 
                    " , DEFAULT_VIDEO_CHECK_SEC: " + videoTimeOut + ",  DELAY_STATE_TRANSITION: " + channelBarTimeOut
                    + ", RF_SWITCH: "+rfSwitch);
            }
            checkVideoTimeOut = Int32.Parse(videoTimeOut);
            delayStateTransition = Int32.Parse(channelBarTimeOut);

            LogCommentInfo(CL, "COORDINATES_FOR_PIP on CHANNEL BAR: " + coordinates);
            LogCommentInfo(CL, "DEFAULT_VIDEO_CHECK_SEC: " + checkVideoTimeOut);
            LogCommentInfo(CL, "DELAY_STATE_TRANSITION: " + delayStateTransition);
            LogCommentInfo(CL, "RF_SWITCH: " + rfSwitch);

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

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Channel surf to video service, and verify video is present in PIP on channel list .
     *
     * @author  Varshad
     * @date    18-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    18-Sep-13
         */

        public override void Execute()
        {
            StartStep();           

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService.LCN);
            }          

            //Launch channel list and focus on next channel.
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Verify that video is present in Specified co-ordinates in PIP
            res = CL.EA.CheckForVideo(coordinates, true, checkVideoTimeOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that video is not present in PIP");
            }

            PassStep();
        }
    }
    #endregion

    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Launch channel list and verify video is not present in PIP.
     *
     * @author  Varshad
     * @date    18-Sep-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    18-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Turn Off RF Switch
            res = CL.IEX.RF.TurnOff("1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }
            isRFActive = false;

            //Wait till the error pop up is launched
            LogCommentInfo(CL, "Wait till the Error pop is launched");
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait for 10 seconds");
            }

            //Launch channel bar
			res = CL.IEX.MilestonesEPG.Navigate("CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch to Channel Bar");
            }

            string timeStamp = "";
            res = CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR command Select_UP");
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
     * @brief   Connect back RF cable if it is disconnected.
     *
     * @author  Varshad
     * @date    18-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Plug back RF if there was failure in reconnection
        if (!isRFActive)
        {
            if(rfSwitch.Equals("A"))
            {
                res = CL.IEX.RF.ConnectToA(instanceName:"1");
            }
            else
            {
                res = CL.IEX.RF.ConnectToB(instanceName:"1");
            }
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to plug back RF signal!");
            }

            
        }

        //Set Channel Bar Time Out to Default 
        res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to change Banner Display Time to:" + defaultChannelBarTimeOutVal);
        }
    }
    #endregion
}