/// <summary>
///  Script Name : CHBAR_2369_Adult_FutureEvent.cs
///  Test Name   : CHBAR-2369-channelbar-alternative-title-for-adult-PP-content-future-event-for-live-channel
///  TEST ID     : 67767
///  QC Version  : 1
///  JIRA ID     : FC-472
///  Variations from QC: NONE
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
 * @class   CHBAR_2369
 *
 * @brief   Chbar 2369.
 *
 * @author  Varshad
 * @date    17-Sep-13
 */

[Test("CHBAR_2369")]
public class CHBAR_2369 : _Test
{
    /**
     * @brief   The _Platform cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The adult channel.
     */

    static Service adultChannel;

    /**
     * @brief   The video service.
     */

    static Service videoService;

    /**
     * @brief   The expected adult thumb nail.
     */

    static string expectedAdultThumbNail;

    /**
     * @brief   Precondition: Get Channel Numbers From ini File.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";

    /**
     * @brief   Zap to locked channel, Navigate to NEXT, and verify alternate program title, short synopsis and poster.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Zap to locked channel, Navigate to NEXT, and verify alternate program title, short synopsis and poster";

    /**
     * @brief   Unlock locked program and verify real program name, short synopsis and poster.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Unlock locked program and verify real program name, short synopsis and poster";

    /**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Varshad
     * @date    17-Sep-13
     */

    static class Constants
    {
        /**
         * @brief   The time to press key.
         */

        public const double timeToPressKey = -1;
    }

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
     * @class   Precondition
     *
     * @brief   Precondition: Get Channel Numbers From ini File.
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

            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video Service fetched from content.xml is null");
            }

            //Get Values From ini File
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "");
            if (adultChannel == null)
            {
                FailStep(CL, "Adult Service fetched from content.xml is null");
            }
            LogCommentInfo(CL, "Adult channel: " + adultChannel.LCN);

            expectedAdultThumbNail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "ADULT_DEFAULT_THUMBNAIL");
            if (string.IsNullOrEmpty(expectedAdultThumbNail))
            {
                FailStep(CL, "Adult thumbnail value ADULT_DEFAULT_THUMBNAIL fetched from project.ini is null or empty");
            }
            LogCommentInfo(CL, "Default Adult Thumbnail: ADULT_DEFAULT_THUMBNAIL" + expectedAdultThumbNail);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + videoService.LCN,false);
            }

            //Channel Surf to locked program 
            res = CL.EA.TuneToChannel(adultChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + adultChannel.LCN);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Step 1: Zap to locked channel, Navigate to NEXT, and verify alternate program title, short synopsis and poster.
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
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    17-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            string obtainedValue = "";
            string expectedAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");

            //Launch Channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR ON LOCKED SERVICE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Channel Bar at channel: " + adultChannel.LCN);
            }

            //Clearing EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Navigate to NEXT on channel bar
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to NEXT on Channel Bar");
            }


            //Get event name from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from Channel Bar");
            }

            //Checking if event name is alternate title
            if (obtainedValue != expectedAdultEventName)
            {
                FailStep(CL, "Alternate event name is not displayed. Obtained: " + obtainedValue + ", Expected: "+ expectedAdultEventName);
            }
            
            LogCommentInfo(CL, "Obtained Adult Channel Event Name: " + obtainedValue);
            LogCommentInfo(CL, "Expected Adult Channel Event Name: " + expectedAdultEventName);

            //Get Synopsis from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("Synopsis", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Synopsis from channel bar");
            }

            //checking if synopsis is displayed
            if (!string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Synopsis value is: " + obtainedValue);
            }
            LogCommentInfo(CL, "Obtained Synopsis value is empty");

            //Get Thumnail from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get thumbnail from channel bar");
            }

            //checking if thumbnail is displayed
            if (obtainedValue != expectedAdultThumbNail)
            {
                FailStep(CL, "Thumbnail value is: " + obtainedValue + "Expected: " + expectedAdultThumbNail);
            }
            LogCommentInfo(CL, "Obtained thumbnail value is " + obtainedValue);

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Step 2: Unlock locked program and verify real program name, short synopsis and poster.
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
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    17-Sep-13
         */

        public override void Execute()
        {
            StartStep();
            
            string obtainedValue = "";
            string timeStamp = "";

            string alternateAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");

            //Pressing Select to enter PIN
            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Select' on Locked Channel");
            }          

            //Enter PIN to unlock the service
            res = CL.EA.EnterDeafultPIN("CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter default pin to unlock Parental Rating Channel: " + adultChannel.LCN);
            }

            //Navigate to NEXT on channel bar
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to NEXT on Channel Bar");
            }

            //Get Event Name from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name from channel bar");
            }

            //checking if realtitle is displayed
            if (obtainedValue.Equals(alternateAdultEventName))
            {
                FailStep(CL, "Alternate Title is displayed after entering PIN");
            }
            LogCommentInfo(CL, "Obtained Real Title after unlocking: " + obtainedValue);

            //Get Synopsis from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("Synopsis", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Synopsis from channel bar");
            }

            //checking if synopsis is displayed
            if (string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Synopsis is null or empty");
            }
            LogCommentInfo(CL, "Obtained Real Synopsis after unlocking: " + obtainedValue);

            //Get Thumnail from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get thumbnail from channel bar");
            }

            //checking if thumbnail is displayed
            if (string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Thumbnail is null or empty ");
            }
            LogCommentInfo(CL, "Obtained Real Synopsis after unlocking: " + obtainedValue);
            
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
     * @date    17-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}