/// <summary>
///  Script Name : PLB_0251_AudioMute_Pause_RB
///  Test Name   : PLB-0251-Audio-mute in pause-RB
///  TEST ID     : 68958
/// Repository   : STB_DIVISION
///  Jira ID     : FC-705
///  QC Version  : 1
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by :Scripted by : Appanna Kangira
/// Last modified : 04 Oct 2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************/
/**
* @class   PLB_0251
*
* @brief   Audio-mute in pause-RB
*
* @author  Appannak
* @date    04-Oct-13
**************************************************************************************************/

[Test("PLB_0251_AudioMute_Pause_RB")]
public class PLB_0251 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

   
    /**********************************************************************************************//**
     * @brief   The av service.
     **************************************************************************************************/

    private static Service avService;

    /**********************************************************************************************//**
     * @brief   The default audio check timeout.
     **************************************************************************************************/

    private static int defaultAudioCheckTimeout;

    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   A constants.
     *
     * @author  Appannak
     * @date    04-Oct-13
     **************************************************************************************************/

    private static class Constants
    {
        /**********************************************************************************************//**
         * @brief   The trick mode for pause.
         **************************************************************************************************/

        public const double trickModeForPause = 0;

        /**********************************************************************************************//**
         * @brief   The trickmode for play.
         **************************************************************************************************/

        public const int trickmodeForPlay = 1;

        /**********************************************************************************************//**
         * @brief   Duration of the rb in secs
         **************************************************************************************************/

        public const int rbDuration = 60;
        
    }

    /**********************************************************************************************//**
     * @brief   Information describing the precondition.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition:Fetch a service required for the test case.";

    /**********************************************************************************************//**
     * @brief   Information describing the step 1.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1:Tune to AV Service & Fill up the Review Buffer ";

    /**********************************************************************************************//**
     * @brief   Information describing the step 2.
     **************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Pause the RB and Verify the Audio Mute";
    

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Appannak
     * @date    04-Oct-13
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
     * @author  Appannak
     * @date    04-Oct-13
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
     * @brief   A pre condition.
     *
     * @author  Appannak
     * @date    04-Oct-13
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    04-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

           
            //Get Values From XML File
            avService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True","ParentalRating=High");
            if (avService.Equals(null))
            {
                FailStep(CL, "Service retrieved is NULL");
            }

            LogCommentInfo(CL, "Retrieved Value From XML File:  AV_Service = " + avService);


            //fetch the Default Audio Check Time Out of respective Project from Project.ini
            String defaultAudioCheckTimeoutInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultAudioCheckTimeoutInStr))
            {
                FailStep(CL, res, "DEFAULT_AUDIO_CHECK_SEC value is not present in the Project.ini");
            }

            defaultAudioCheckTimeout = int.Parse(defaultAudioCheckTimeoutInStr);        
            
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   A step 1.
     *
     * @author  Appannak
     * @date    04-Oct-13
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    04-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, avService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + avService.LCN);
            }

           //Check for Audio after Tune to Video Service.
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio is Present on the tuned Service");

            }

            //wait for RB to fill up
            LogCommentInfo(CL, "Waiting for the Review Buffer to Fill Up");
            CL.IEX.Wait(Constants.rbDuration);

         
            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   A step 2.
     *
     * @author  Appannak
     * @date    04-Oct-13
     **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    04-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            
            //Set the RB trick mode to PAUSE
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.trickModeForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause in RB");
            }

            //Check for Audio is muted on Pause mode
            res = CL.EA.CheckForAudio(false, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio is not muted on RB Pause");

            }
            else
            {
                LogCommentInfo(CL, "Audio mute on RB PAUSE successful");

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
     * @author  Appannak
     * @date    04-Oct-13
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //Stop the recording Playback
        res = CL.EA.PVR.StopPlayback(true);
        if (!res.CommandSucceeded)
        {
           CL.IEX.FailStep("Failed to Stop RB Playback ");
        }
        
    }
    #endregion
}
