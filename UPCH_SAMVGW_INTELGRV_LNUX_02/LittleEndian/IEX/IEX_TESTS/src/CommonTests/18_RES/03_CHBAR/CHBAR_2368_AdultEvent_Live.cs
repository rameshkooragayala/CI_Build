/// <summary>
///  Script Name : CHBAR_2368_AdultEvent_Live.cs
///  Test Name   : EPG-2368-channelbar-alternative-title-for-adult-PP-content-in-live-channel
///  TEST ID     : 67768
///  QC Version  : 1
///  JIRA ID     : FC-471
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
 * @class   CHBAR_2368
 *
 * @brief   Channelbar alternative title for adult PP content in live channel.
 *
 * @author  Varshad
 * @date    12-Sep-13
 */

[Test("CHBAR_2368")]
public class CHBAR_2368 : _Test
{
    /**
     * @brief   _Platform CL.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The adult channel.
     */

    static Service adultChannel;

    /**
     * @brief   Name of the expected adult event.
     */

    static string expectedAdultEventName;

    /**
     * @brief   The expected adult thumb nail.
     */

    static string expectedAdultThumbNail;

    /**
     * @brief   The Helper class.
     */
    static Helper helper = new Helper();


    /**
     * @brief   Precondition: Zap to locked event service and unlock.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Zap to locked event service and unlock ";

    /**
     * @brief   Launch channel bar and verify event name, synopsis and thumbnail
     */

    private const string STEP1_DESCRIPTION = "Step 1: Launch channel bar and verify event name, synopsis and thumbnail";

    /**
     * @brief   Wait till the event ends and next locked event begins and verify title, synopsis and thumbnail.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Wait till the event ends and next locked event begins and verify title, synopsis and thumbnail";

    /**
     * @brief   Enter valid pin and validate title, synopsis and thumbnail.
     */

    private const string STEP3_DESCRIPTION = "Step 3: Enter valid pin and validate title, synopsis and thumbnail";

    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    12-Sep-13
     */

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
    #endregion

    #region PreExecute

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    12-Sep-13
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
     * @brief   Zap to locked event service and unlock.
     *
     * @author  Varshad
     * @date    12-Sep-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Zap to locked event service and unlock.
         *
         * @author  Varshad
         * @date    12-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;ParentalRating=High;IsMinEventDuration=True;HasThumbnail=True;HasSynopsis=True", "");
            if (adultChannel == null)
            {
                FailStep(CL, "Video Service fetched from content.xml is null");
            }
            LogCommentInfo(CL, "Locked service: " + adultChannel.LCN);

            expectedAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");
            expectedAdultThumbNail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "ADULT_DEFAULT_THUMBNAIL");

            if (string.IsNullOrEmpty(expectedAdultEventName))
            {
                FailStep(CL, "One of the values is null or empty. LOG_RATING_LOCKED_EVTNAME: " + expectedAdultEventName +
                    ", ADULT_DEFAULT_THUMBNAIL " + expectedAdultThumbNail );
            }
            LogCommentInfo(CL, "LOG_RATING_LOCKED_EVTNAME: " + expectedAdultEventName);


            res = CL.EA.TuneToLockedChannel(adultChannel.LCN);
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
     * @brief   Launch channel bar and verify event name, synopsis and thumbnail
     *
     * @author  Varshad
     * @date    12-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Launch channel bar and verify event name, synopsis and thumbnail.
         *
         * @author  Varshad
         * @date    12-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Clearing EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Launch Channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Channel Bar at channel: " + adultChannel.LCN);
            }

            helper.verifyChannelBarInfo(false);
            
            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Wait till the event ends and next locked event begins and verify title, synopsis and thumbnail.
     *
     * @author  Varshad
     * @date    12-Sep-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Wait till the event ends and next locked event begins and verify title, synopsis and thumbnail.
         *
         * @author  Varshad
         * @date    12-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            int timeLeftInSecond = 0;
            CL.EA.GetCurrentEventLeftTime(ref timeLeftInSecond);
            res = CL.IEX.Wait(timeLeftInSecond);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to wait for "+timeLeftInSecond+" seconds.");
            }
			
			 //Clearing EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Launch Channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR ON LOCKED SERVICE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Channel Bar at channel: " + adultChannel.LCN);
            }

            helper.verifyChannelBarInfo(true);
            
            PassStep();
        }
    }
    #endregion
    #region Step3

    /**
     * @class   Step3
     *
     * @brief   Enter valid pin and validate title, synopsis and thumbnail.
     *
     * @author  Varshad
     * @date    12-Sep-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Enter valid pin and validate title, synopsis and thumbnail.
         *
         * @author  Varshad
         * @date    12-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Unlock the current locked channel by performing TuneToLockedChannel
            res = CL.EA.TuneToLockedChannel(adultChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to locked channel. " + adultChannel.LCN);
            }
           
            helper.verifyChannelBarInfo(false);
            
            PassStep();
        }
    }
    #endregion

    
    /**
     * @fn  public void verify(bool locked)
     *
     * @brief   Verifies.
     *
     * @author  Varshad
     * @date    12-Sep-13
     *
     * @param   locked  true to lock, false to unlock.
     */    
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        public void verifyChannelBarInfo(bool locked)
        {
            string obtainedValue = "";
            IEXGateway._IEXResult res;

            //Get Event Name from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to get event name from channel bar");
            }


            if (locked)
            {
                //Verify that alternate title is displayed
                if (!obtainedValue.Equals(expectedAdultEventName))
                {
                    CL.IEX.FailStep("Alternate Title is not displayed. Obtained Value is: " + obtainedValue + ", Expected: " + expectedAdultEventName);
                }
            }
            else
            {
                //verify that realtitle is displayed
                if (string.IsNullOrEmpty(obtainedValue) && obtainedValue.Equals(expectedAdultEventName))
                {
                    CL.IEX.FailStep("Real title is null/empty or Alternate Title is displayed even after entering PIN. Obtained value: " + obtainedValue);
                }
            }
            LogCommentInfo(CL, "Obtained Title is " + obtainedValue);


            //Get Synopsis from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("Synopsis", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to get Synopsis from channel bar");
            }


            if (locked)
            {
                //verify that synopsis is not displayed
                if (!string.IsNullOrEmpty(obtainedValue))
                {
                    CL.IEX.FailStep("Synopsis is displayed for locked programme.");
                }
            }
            else
            {
                //verify that synopsis is displayed
                if (string.IsNullOrEmpty(obtainedValue))
                {
                    CL.IEX.FailStep("Synopsis is null or empty even after unlocking");
                }
            }
            LogCommentInfo(CL, "Obtained Synopsis is: " + obtainedValue);

            //Get Thumbnail from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to get thumbnail from channel bar");
            }


            if (locked)
            {
                //Verify that alternate title is displayed
                if (obtainedValue != expectedAdultThumbNail)
                {
                    CL.IEX.FailStep("Alternate thumbnail is not displayed. Obtained value is: " + obtainedValue);
                }
            }
            else
            {
                //Verify that real title is displayed
                if (string.IsNullOrEmpty(obtainedValue) && obtainedValue == expectedAdultThumbNail)
                {
                    CL.IEX.FailStep("Real Thumbnail is not displayed. Obtained value is: " + obtainedValue);
                }
            }
            LogCommentInfo(CL, "Obtained thumbnail is: " + obtainedValue);

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
     * @date    12-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}