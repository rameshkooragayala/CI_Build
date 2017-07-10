/// <summary>
///  Script Name : GRID_1545_Program_Grid_Alternate_Title_For_Adult_Content.cs
///  Test Name   : GRID-1545-Programme-grid-alternate title for Adult content 
///  TEST ID     : 35837
///  QC Version  : 1
///  Variations from QC:
///  JIRA ID     :FC-518
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
 * @class   GRID_1545
 *
 * @brief   Grid 1545.
 *
 * @author  Anshulu
 * @date    9/24/2013
 **************************************************************************************************/

[Test("GRID_1545_Program_Grid_Alternate_Title_For_Adult_Content")]
public class GRID_1545 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The audio video service.
     **************************************************************************************************/

    static Service audioVideoService;

    /**********************************************************************************************//**
     * @brief   The high parental service.
     **************************************************************************************************/

    static Service highParentalService;

    /**********************************************************************************************//**
     * @brief   The tuned chnum.
     **************************************************************************************************/

    static string tunedChnum="";

    /**********************************************************************************************/
    /*** @brief   Precondition: Get Channel Numbers From XML File & Sync
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";

    /**********************************************************************************************/
    /*** @brief   Step 1:Navigate to Program Grid and launch all channels
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1:Navigate to Program Grid and launch all channels ";

    /**********************************************************************************************/
    /*** @brief   Step 2:Move the focus on to ongoing event on High Parental Service
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2:Move the focus on to ongoing event on High Parental Service";
    

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

    /**********************************************************************************************/
    /*** @class   PreCondition
    *
    * @brief   Precondition: Get Channel Numbers From XML File & Sync
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
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (audioVideoService == null)
            {
                FailStep(CL, res, "Failed to get channel number from ContentXML");
            }

			highParentalService = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "LCN=" + audioVideoService.LCN);
            if (highParentalService == null)
            {
                FailStep(CL, res, "Failed to get channel number from ContentXML");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Step 1:Navigate to Program Grid and launch all channels
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Obtaining chNum on Live State
			LogCommentInfo(CL,"Obtaining chNum on Live State");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out tunedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get tuned Channel number");
            }
            //Navigate To grid
			LogCommentInfo(CL,"Navigate To grid");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }
            
            PassStep();
        }
    }
    #endregion
	#region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   Step 2:Move the focus on to ongoing event on High Parental Service
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

            string obtainedAdultEventName = "";
            string expectedAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");
                             
			LogCommentInfo(CL,"Move the focus on to ongoing event on High Parental Service");
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, highParentalService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Move the Focus to " + highParentalService.LCN);
            }
	
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedAdultEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from Grid");
            }

            LogComment(CL, "Obtained Adult Channel Event Name: " + obtainedAdultEventName);
            LogComment(CL, "Expected Adult Channel Event Name: " + expectedAdultEventName);

            //Checking if event name is alternate title
            if (obtainedAdultEventName != expectedAdultEventName)
            {
                FailStep(CL, "Event Name of Adult Event is not Alternate title");
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