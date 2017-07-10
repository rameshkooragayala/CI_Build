/// <summary>
///  Script Name : FAR_0076_Retain_Usersettings.cs
///  Test Name   : FAR-0076-Retain User settings to current user settings
///  TEST ID     : 72099
///  QC Version  : 1
///  Variations from QC: Only channel bar time out, start guard time and end guard time is set
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
///  Repository  : STP - IPC_UPC Software test plan
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using FailuresHandler;
using IEX.Tests.Reflections;

/**
 * @class   FAR_0076_Retain_Usersettings
 *
 * @brief   FAR_0076_Retain_Usersettings
 *
 * @author  Varshad
 * @date    09-Oct-13
 */

[Test("FAR_0076")]
public class FAR_0076 : _Test
{
    [ThreadStatic]

    /**
     * @property    static _Platform CL, GW
     *
     * @brief   Gets the gw.
     *
     * @return  The gw.
     */

    static _Platform CL, GW;

    /**
     * @brief   The ishomenet.
     */

    static bool isHomeNet = false;

    /**
     * @brief   The default channel bar time out value.
     */

    static string defaultChannelBarTimeOut;

    static string maxChannelBannerTimeout;

    /**
     * @brief   The maximum channel bar time out value.
     */

    static EnumChannelBarTimeout maxChannelBarTimeOutVal;

    static EnumChannelBarTimeout defaultChannelBarTimeOutVal;

    static string channelBarTimeOutSupported;

    static EnumLanguage defaultSubtitleLangVal;

    static EnumLanguage prefSubtitleLangVal;

    static string defaultSubtitleLang;

    /**
     * @brief   Precondition: Get Channel Numbers From ini File.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Value from Project.ini and Test.ini";

    /**
     * @brief   Change user settings.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Change user settings";

    /**
     * @brief   Perform Factory reset with reset to default settings option.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Perform Factory reset with retain user settings option";

    /**
     * @brief   Verify User settings.
     */

    private const string STEP3_DESCRIPTION = "Step 3: Verify User settings.Settings should be same as before FR";

    /**
     * @brief   The keep recordingd.
     */
    private const bool keepRecordings = false;

    /**
     * @brief   The keep settings.
     */
    private const bool keepSettings = true;

    private static string defaultPin;

    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    09-Oct-13
     */

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        //this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
        string isHomeNetwork = "";
        try
        {
            isHomeNetwork = CL.EA.GetTestParams("IsHomeNetwork");
        }
        catch
        {
            LogCommentInfo(CL, "IsHomeNetwork is not defined in Test.ini. Therefore taking default value as false");
            isHomeNetwork = "false";
        }
        //If Home network is true perform GetGateway
        isHomeNet = Convert.ToBoolean(isHomeNetwork);
        if (isHomeNet)
        {
            //Get gateway platform
            GW = GetGateway();
        }

    }
    #endregion

    #region PreExecute

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    09-Oct-13
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
     * @brief   Pre condition.
     *
     * @author  Varshad
     * @date    09-Oct-13
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
         * @date    09-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            maxChannelBannerTimeout = "";
            string bannerTimeout = "";

            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }

            channelBarTimeOutSupported = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "SUPPORTED");
            if (string.IsNullOrEmpty(channelBarTimeOutSupported))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, SUPPORTED fetched from Project.ini is null or empty", false);
            }

            defaultSubtitleLang = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DEFAULT");
            if (string.IsNullOrEmpty(defaultSubtitleLang))
            {
                FailStep(CL, "SUBTITLE, DEFAULT fetched from Project.ini is null or empty", false);
            }

            Enum.TryParse<EnumLanguage>(defaultSubtitleLang,true, out defaultSubtitleLangVal);

            string prefSubtitleLang = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "OTHER");
            if (string.IsNullOrEmpty(prefSubtitleLang))
            {
                FailStep(CL, "SUBTITLE, OTHER fetched from Project.ini is null or empty", true);
            }

            Enum.TryParse<EnumLanguage>(prefSubtitleLang, true,out prefSubtitleLangVal);

            if (channelBarTimeOutSupported.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");
                if (string.IsNullOrEmpty(bannerTimeout))
                {
                    FailStep(CL, "CHANNEL_BAR_TIMEOUT, MAX fetched from Project.ini is null or empty", false);
                }
                Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out maxChannelBarTimeOutVal);
                LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> MAX = " + maxChannelBarTimeOutVal);


                defaultChannelBarTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
                if (string.IsNullOrEmpty(defaultChannelBarTimeOut))
                {
                    FailStep(CL, "CHANNEL_BAR_TIMEOUT, DEFAULT fetched from Project.ini is null or empty", false);
                }

                Enum.TryParse<EnumChannelBarTimeout>(defaultChannelBarTimeOut, out defaultChannelBarTimeOutVal);
            }            

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Change user settings.
     *
     * @author  Varshad
     * @date    09-Oct-13
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
         * @date    09-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //Change Timeout Duration in Channel Bar Timeout settings
            if (channelBarTimeOutSupported.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                res = CL.EA.STBSettings.SetBannerDisplayTime(maxChannelBarTimeOutVal);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to change Banner Display Time to:" + maxChannelBarTimeOutVal, false);
                }
            }

            res = CL.EA.STBSettings.SetSubtitlesPrefs(true, prefSubtitleLangVal);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set prefered subtitle language to: "+ prefSubtitleLangVal);
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Perform Factory reset with reset to default settings option.
     *
     * @author  Varshad
     * @date    09-Oct-13
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
         * @date    09-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //Perform Factory reset with Keep Recording option Yes
            res = CL.EA.STBSettings.FactoryReset(keepRecordings, keepSettings, defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }

            if (isHomeNet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount client");
                }
            }
            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount cleint");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**
     * @class   Step3
     *
     * @brief   Verify User settings.
     *
     * @author  Varshad
     * @date    09-Oct-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    09-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            if (channelBarTimeOutSupported.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to navigate to channel bar time out");
                }

                string curChannelBarTimeOut = "";
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out curChannelBarTimeOut);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to perform GetEpgInfo");
                }

                if (!curChannelBarTimeOut.Contains(maxChannelBannerTimeout))
                {
                    FailStep(CL, "Setting is not same as default value after factory reset.");
                }
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLES SETTING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to Subtitle setting");
            }

            string curPrefSubtitleSetting = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out curPrefSubtitleSetting);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform GetEpgInfo");
            }

            if (!curPrefSubtitleSetting.Contains(curPrefSubtitleSetting))
            {
                FailStep(CL, "Setting is not same as previously set value after factory reset. Current Value: " + curPrefSubtitleSetting);
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
     * @brief   Posts the execute.
     *
     * @author  Varshad
     * @date    09-Oct-13
     */

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.STBSettings.SetSubtitlesPrefs(true, defaultSubtitleLangVal);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to set prefered subtitle language to default.");
        }

        if (channelBarTimeOutSupported.Equals("True", StringComparison.OrdinalIgnoreCase))
        {
            res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to change Banner Display Time to:" + defaultChannelBarTimeOut, false);
            }
        }
    }
    #endregion
}