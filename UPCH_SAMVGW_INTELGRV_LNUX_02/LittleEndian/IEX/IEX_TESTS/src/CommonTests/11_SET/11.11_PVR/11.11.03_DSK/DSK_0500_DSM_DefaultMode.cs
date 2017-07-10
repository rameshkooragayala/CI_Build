/// <summary>
///  Script Name : DSK_0500_DSM_DefaultMode
///  Test Name   : DSK-0500-DSM-DefaultMode
///  TEST ID     : 68248
///  Jira ID     : FC-628
///  QC Version  : 3
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Renukaradhya 
///  Last modified : 30 Aug 2013 
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
 * @class   DSK_0500
 *
 * @brief   Dsk 0500.
 *
 * @author  Madhu R
 * @date    9/12/2013
 **************************************************************************************************/

[Test("DSK_0500")]
public class DSK_0500 : _Test
{
    /**********************************************************************************************//**
     * @brief   Platform cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The audio vedio service.
     **************************************************************************************************/

    static Service audioVedioService;

    /**********************************************************************************************//**
     * @brief   The default disk space management.
     **************************************************************************************************/

    static String DefaultDiskSpaceManagement;

    /**********************************************************************************************//**
     * @brief   Get Channel Numbers From xml File
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**********************************************************************************************//**
     * @brief   Verify default disk space management mode is set.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Verify default disk space management mode is set";

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);


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
     * @author  Madhu R
     * @date    9/12/2013
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
     * @brief   Pre condition.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhu R
         * @date    9/12/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Values From xml File
            audioVedioService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (audioVedioService.Equals(null))
            {
                FailStep(CL, "Failed to fetch audioViedoService from content xml.");

            }

            else
            {
                LogCommentInfo(CL, "audioVedioService fetched from content xml is: " + audioVedioService.LCN);

            }

            DefaultDiskSpaceManagement = CL.EA.GetValueFromINI(EnumINIFile.Project, "DISK_SPACE_MANAGEMENT", "DEFAULT");
            if (String.IsNullOrEmpty(DefaultDiskSpaceManagement))
            {
                FailStep(CL, res, "DEFAULT value is not present in the Project.ini");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Access PVR settings -> disk space management mode and Check default disk space management mode is set.
     *
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhu R
         * @date    9/12/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVedioService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to audioVedioService " + audioVedioService.LCN);
            }

            //Navigate to Disk Space Management under Settings and verify for default settings
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISK SPACE MANAGEMENT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Disk space Management ");
            }

            String obtainedDefaultDiskSpaceManagement = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedDefaultDiskSpaceManagement);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get default disk management mode");
            }

            if (obtainedDefaultDiskSpaceManagement.Equals(DefaultDiskSpaceManagement))
            {
                LogCommentInfo(CL, "Default DISK SPACE MANGEMENT mode is set");
            }
            else
            {
                FailStep(CL, "Default DISK SPACE MANGEMENT mode is not set.");
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
     * @author  Madhu R
     * @date    9/12/2013
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}