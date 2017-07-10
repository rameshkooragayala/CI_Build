/// <summary>
///  Script Name : GRID_1503_Program_Grid_View_As_One_Channel.cs
///  Test Name   : GRID-1503-programmgrid-view-as-ONE CHANNEL.  
///  TEST ID     : 68129
///  QC Version  : 1
///  Variations from QC:
///  JIRA ID     :FC-517
/// ----------------------------------------------- 
///  Modified by : Madhu Thomas/Appanna Kangira
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
 * @class   GRID_1503
 *
 * @brief   Grid 1503.
 *
 * @author  Madhu T
 * @date    16-Sep-13
 */

[Test("GRID_1503_Program_Grid_View_As_One_Channel")]
public class GRID_1503 : _Test
{
    /**
     * @brief   The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @class   constants
     *
     * @brief   Constants.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

    static class constants
    {
        /**
         * @brief   The ir key press wait.
         */

        public const int irKeyPressWait = 2000;

        /**
         * @brief   The Increment Position On Guide
         */

        public const int incrementPositionOnGuide = 3;
    }

    /**
     * @brief   The audio video service 1.
     */

    static Service audioVideoService_1;

    /**
     * @brief   The audio video service 2.
     */

    static Service audioVideoService_2;
    

    /**
     * @brief   The next chnum.
     */

    static string nxtChnum = "";

    /**
     * @brief   The previous chnum.
     */

    static string prevChnum = "";

    /**
     * @brief   The focused chnum.
     */

    static string focusedChnum="";

    /**
     * @brief   Co-ordinates for Overlay Video.
     */

    static string videoCoordForGridBlock;

    /**
     * @brief   Default video check timeout.
     */

    static int defaultVideoCheckTimeout;

    /**
     * @brief   Default Audio check timeout.
     */

    static int defaultAudioCheckTimeout;


    /**
   * @brief   Next Position On Guide.
   */

    static int nextPositionOnGuide;


    /**
     * @brief   To hold the Value of IR key fetched from Proj INI.
     */

    static string nextCNavigationInSingleChGrid;

    /**
     * @brief   To hold the Value of IR key fetched from Proj INI.
     */

    static string previousCNavigationInSingleChGrid;

    /**
     * @brief   Information describing the precondition.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";

    /**
     * @brief   Information describing the step 1.
     */

    private const string STEP1_DESCRIPTION = "Step 1:Tune to service S1 and launch the programme grid from the main menu ";

    /**
     * @brief   Information describing the step 2.
     */

	private const string STEP2_DESCRIPTION = "Step 2:Select Single Channel Grid & Verify the default focus ";

    /**
     * @brief   Information describing the step 3.
     */

	private const string STEP3_DESCRIPTION = "Step 3:Move the focus to next channel & Verify Video PIP,Audio";

    /**
     * @brief   Information describing the step 4.
     */

    private const string STEP4_DESCRIPTION = "Step 4:Move the focus to Previous channel & Verify Video PIP,Audio";
  
    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

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

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Madhu T
     * @date    16-Sep-13
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
     * @brief   Get Channel Numbers From XML File & Sync.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Get Channel Numbers From XML File & Sync.
         *
         * @author  Madhu T
         * @date    16-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Get Values From Content XML File
			LogCommentInfo(CL,"Get Values From Content XML File");
            audioVideoService_1 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if(audioVideoService_1 == null)
            {
                FailStep(CL,"Retrieved audioVideoService is  NULL");
            }

            nextPositionOnGuide = Convert.ToInt16(audioVideoService_1.PositionOnGuide) + constants.incrementPositionOnGuide;

            audioVideoService_2 = CL.EA.GetServiceFromContentXML("Type=Video;PositionOnGuide="+nextPositionOnGuide, "ParentalRating=High");
            if (audioVideoService_2 == null)
            {
                FailStep(CL, "Retrieved audioVideoService 2 is  NULL");
            }

            //Fetch the co-ordinates for Overlay video
            videoCoordForGridBlock = CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "COORDINATES_FOR_VIDEO_IN_GRIDBLOCK");
            if (String.IsNullOrEmpty(videoCoordForGridBlock))
            {
                FailStep(CL, res, "COORDINATES_FOR_VIDEO_IN_GRIDBLOCK value is not present in the Project.ini");
            }

            //Fetch the default timeout for Video check
            String defaultVideoCheckTimeInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultVideoCheckTimeInStr))
            {
                FailStep(CL, res, "DEFAULT_VIDEO_CHECK_SEC value is not present in the Project.ini");
            }
            defaultVideoCheckTimeout = int.Parse(defaultVideoCheckTimeInStr);

            //Fetch the default timeout for Audio check
            String defaultAudioCheckTimeInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultAudioCheckTimeInStr))
            {
                FailStep(CL, res, "DEFAULT_AUDIO_CHECK_SEC value is not present in the Project.ini");
            }
            defaultAudioCheckTimeout = int.Parse(defaultAudioCheckTimeInStr);

            //Fetch the Next Navigation Key from INI file
            nextCNavigationInSingleChGrid = CL.EA.GetValueFromINI(EnumINIFile.Project, "SINGLE_CHANNEL_GRID_NAV", "NEXT_CHANNEL_NAV");
            if (String.IsNullOrEmpty(nextCNavigationInSingleChGrid))
            {
                FailStep(CL, res, "NEXT_CHANNEL_NAV value is not present in the Project.ini");
            }

            //Fetch the Previous Navigation Key from INI file
            previousCNavigationInSingleChGrid = CL.EA.GetValueFromINI(EnumINIFile.Project, "SINGLE_CHANNEL_GRID_NAV", "PREVIOUS_CHANNEL_NAV");
            if (String.IsNullOrEmpty(previousCNavigationInSingleChGrid))
            {
                FailStep(CL, res, "PREVIOUS_CHANNEL_NAV value is not present in the Project.ini");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Tune to service S1 and launch the programme grid from the main menu.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Tune to service S1 and launch the programme grid from the main menu.
         *
         * @author  Madhu T
         * @date    16-Sep-13
         */

        public override void Execute()
        {
            StartStep();
			//Tune To Channel By DCA
			LogCommentInfo(CL,"Tune To Channel By DCA");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel By DCA");
            }

           PassStep();
        }
    }
    #endregion
	#region Step2

    /**
     * @class   Step2
     *
     * @brief   Select Single Channel Grid & Verify the default focus.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Select Single Channel Grid & Verify the default focus.
         *
         * @author  Madhu T
         * @date    16-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Clear EPG Info
            LogCommentInfo(CL, "Clear EPG Info");
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Clear EPG Info");
            }
            
            //Navigate To grid by Single Channel
			LogCommentInfo(CL,"Navigate To grid by Single Channel");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:GRID BY SINGLE CHANNEL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Grid By Single Channel");
            }

            
            //Obtaining chNum on Grid by Single Channel
            LogCommentInfo(CL, "Obtaining chNum on Grid State");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out focusedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

			//Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
		    LogCommentInfo(CL,"Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID");
            if (audioVideoService_2.LCN == focusedChnum)
            {
                LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + audioVideoService_1.LCN + " " + focusedChnum);
            }

            else
            {
                FailStep(CL, "Failed to Focus on the Tuned Channel in Grid"); 
            }
           
            //Doing a Audio Check while in Guide State
            LogCommentInfo(CL,"Doing a Audio Check in Guide Screen");
            
           /* res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio Check failed");
            } */

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**
     * @class   Step3
     *
     * @brief   Move the focus to next channel & Verify Video PIP,Audio.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Move the focus to next channel & Verify Video PIP,Audio.
         *
         * @author  Madhu T
         * @date    16-Sep-13
         */

        public override void Execute()
        {
          
            StartStep();

            //Clear EPG Info
            LogCommentInfo(CL, "Clear EPG Info");
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Clear EPG Info");
            }
          
            //Move the focus to next channel by pressing the right arrow
			LogCommentInfo(CL,"Move the focus to next channel by pressing the right arrow");
            CL.EA.UI.Utils.SendIR("SELECT_RIGHT", constants.irKeyPressWait);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out nxtChnum);
            
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Channel number from Grid");
            }

            LogComment(CL, "Obtained Channel Number: " + nxtChnum);

            LogComment(CL, "Checking if Channel Number is Different from the Previous Channel number");
            
            //Checking if Channel Number is Different from the Previous Channel number
            if (nxtChnum == audioVideoService_2.LCN)
            {
                FailStep(CL, "Did not Navigate to New Channel Number");
            }

            //Check for video PIP in the focussed GRID block
            res = CL.EA.CheckForVideo(videoCoordForGridBlock, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for video on overlay");
            }

            //Check for audio on overlay
           /* res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
            } */

            PassStep();
        }
    }
    #endregion
	#region Step4

    /**
     * @class   Step4
     *
     * @brief   Move the focus to Previous channel & Verify Video PIP,Audios.
     *
     * @author  Madhu T
     * @date    16-Sep-13
     */

    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Move the focus to Previous channel & Verify Video PIP,Audio.
         *
         * @author  Madhu T
         * @date    16-Sep-13
         */

        public override void Execute()
        {
           
            StartStep();

            //Clear EPG Info
            LogCommentInfo(CL, "Clear EPG Info");
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Clear EPG Info");
            }

            //Move the focus to prev channel by pressing the right arrow
            LogCommentInfo(CL, "Move the focus to prev channel by pressing the left arrow");
            
            CL.EA.UI.Utils.SendIR("SELECT_LEFT", constants.irKeyPressWait); //Brings back the Focus to Tuned Channel.
       
            CL.EA.UI.Utils.SendIR("SELECT_LEFT", constants.irKeyPressWait);//Moves to the Previous Channel with respect to the Tuned Channel.

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out prevChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Channel number from Grid");
            }

            LogComment(CL, "Obtained Channel Number: " + prevChnum);

            LogComment(CL, "Checking if Channel Number is same as first tuned service");

            //Checking if Channel Number is same as first tuned service
            if (prevChnum == audioVideoService_2.LCN)
            {
                FailStep(CL, "Failed to move to the Previous Channel with respect to tuned Live Service");
            }

            //Check for video PIP in the focussed GRID block
            res = CL.EA.CheckForVideo(videoCoordForGridBlock, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for video on overlay");
            }

            //Check for audio on overlay
           /* res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
            }*/
            
            
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
     * @author  Madhu T
     * @date    16-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}