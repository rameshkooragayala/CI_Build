/// <summary>
///  Script Name : GRID_1502_Program_Grid_By_Genre.cs
///  Test Name   : GRID-1502-programmgrid-by-genre
///  TEST ID     : 35629
///  QC Version  : 1
///  Variations from QC:
///  JIRA ID     :FC-516
/// ----------------------------------------------- 
///  Modified by : Madhu Thomas
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************//**
 * @class   GRID_1502
 *
 * @brief   Grid 1502.
 *
 * @author  Anshulu
 * @date    9/24/2013
 **************************************************************************************************/

[Test("GRID_1502_Program_Grid_By_Genre")]
public class GRID_1502 : _Test 
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //static int testDuration = 0;

    /**********************************************************************************************//**
     * @brief   The audio video service.
     **************************************************************************************************/

    static Service audioVideoService;

    /**********************************************************************************************//**
     * @brief   The service in movie genre.
     **************************************************************************************************/

    static Service serviceInMovieGenre;

    /**********************************************************************************************//**
     * @brief   The service not in movie genre.
     **************************************************************************************************/

    static Service serviceNotInMovieGenre;

    /**********************************************************************************************//**
     * @brief   The tuned chnum.
     **************************************************************************************************/

    static string tunedChnum="";

    /**********************************************************************************************//**
     * @brief   The focused chnum.
     **************************************************************************************************/

    static string focusedChnum="";

    /**********************************************************************************************//**
     * @brief   The timeout.
     **************************************************************************************************/

    static string timeout = "";

    /**********************************************************************************************//**
     * @brief   The audio timeout.
     **************************************************************************************************/

    static int AudioTimeout;

    /**********************************************************************************************//**
     * @brief   The genreOptions array will store the list of genre read from Project ini
     **************************************************************************************************/

    static string[] genreOptions = {};

    /**********************************************************************************************//**
     * @brief   The time stamp.
     **************************************************************************************************/

    static string timeStamp = "";

    /**********************************************************************************************//**
     * @brief   The time to press key.
     **************************************************************************************************/

    public const int timeToPressKey = -1;

    /**********************************************************************************************//**
     * @brief   Precondition: Get Channel Numbers From XML File &amp; Sync.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";

    /**********************************************************************************************//**
     * @brief   Step 1:Tune to service S1 and launch the programme grid from the main menu.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1:Tune to service S1 and launch the programme grid from the main menu";

    /**********************************************************************************************//**
     * @brief   Step 2:Select By Programme Genre.
     **************************************************************************************************/

	private const string STEP2_DESCRIPTION = "Step 2:Select By Programme Genre";

    /**********************************************************************************************//**
     * @brief   Step 3:Select Movie Genre.
     **************************************************************************************************/

	private const string STEP3_DESCRIPTION = "Step 3:Select any Genre and verify the events list";
  
    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

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

    /**********************************************************************************************//**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Precondition: Get Channel Numbers From XML File &amp; Sync.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    9/24/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            
            //Get Values From Content XML File
			LogCommentInfo(CL,"Get Values From Content XML File");
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (audioVideoService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }

            PassStep();
        }
    }

    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Step 1:Tune to service S1 and launch the programme grid from the main menu.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    9/24/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            timeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            int.TryParse(timeout, out AudioTimeout);

			//Tune To Channel By DCA
			LogCommentInfo(CL,"Tune To Channel By DCA");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel By DCA");
            }

            //Obtaining chNum on Live State
			LogCommentInfo(CL,"Obtaining chNum on Live State");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out tunedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get tuned Channel number " + audioVideoService.LCN);
            }
			
            //Navigate To TV GUIDE
			LogCommentInfo(CL,"Navigate To TV GUIDE");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }
			
			//Obtaining chNum on Grid State
            LogCommentInfo(CL,"Obtaining focusedChnum on Grid State");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out focusedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel " + audioVideoService.LCN + " in Grid");
            }

            //Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
		    LogCommentInfo(CL,"Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID");
            if (audioVideoService.LCN == focusedChnum)
            {
                LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + audioVideoService.LCN + " " + focusedChnum);
            }

            else
            {
                FailStep(CL, "Failed to Focus on the Tuned Channel in Grid"); 
            }
           
            //Doing a Audio Check while in Guide State
            LogCommentInfo(CL,"Doing a Audio Check in Guide Screen");
            res = CL.EA.CheckForAudio(true, AudioTimeout);
            if(!res.CommandSucceeded)
            {
                 FailStep(CL,res, "Audio Check failed");
            }

            PassStep();
        }
    }
    #endregion
	#region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   Step 2:Select By Programme Genre.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    9/24/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            timeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            int.TryParse(timeout, out AudioTimeout);

            string genreList;            
            
            //Navigate To grid by Genre
			LogCommentInfo(CL,"Navigate To grid by Genre");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:BY GENRE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Grid By Genre");
            }
           
            //Getting the Genre List from Project.ini
            genreOptions = CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "GENRE").Split(',');
                        
            //looping through the list of Genre's
            foreach (string options in genreOptions)
            {
                genreList = "";
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out genreList);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Sub-Genre Name from EPG info");
                }

                if (options.ToLower() != genreList.ToLower())
                {
                    FailStep(CL, res, "Failed to get "+ options +" genre from Grid");
                }

                res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPressKey, ref timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to move select down the next sub-genre in the genre list");
                }

                //Wait command for just in case GetEPGInfo is not missed
                res = CL.IEX.Wait(2);
            }

            //Doing a Audio Check while in Guide State
            LogCommentInfo(CL,"Doing a Audio Check in Guide Screen");
            res = CL.EA.CheckForAudio(true, AudioTimeout);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res, "Audio Check failed");
            }
                                   
            PassStep();
        }
    }
    #endregion
    #region Step3

    /**********************************************************************************************//**
     * @class   Step3
     *
     * @brief   Step 3:Select Movie Genre.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    9/24/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            string eventName = "";
            
            //selecting the hightlighted option in Genre
            res = CL.IEX.SendIRCommand("SELECT", timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to move select down the next sub-genre in the genre list");
            }

            //verifying that the events are listed in the selected Genre
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the event name from Milestone viewer");
            }

            if (eventName == null)
            {
                FailStep(CL, res, "there are no events in the selected genre or the events are not listed for selected genre");
            }

           PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   Posts the execute.
     *
     * @author  Anshulu
     * @date    9/24/2013
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}