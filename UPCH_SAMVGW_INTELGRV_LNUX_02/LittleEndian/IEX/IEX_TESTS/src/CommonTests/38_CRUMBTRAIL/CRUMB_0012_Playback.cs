/// <summary>
///  Script Name : CRUMB_0012_Playback
///  Test Name   : EPG_Crumb display in playback Screen
///  TEST ID     : 71335
///  QC Version  : 1
///  Variations from QC:No UI validation and dictionary-key values from Dictionary file are not validated.
///   QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable-WP2429 Crumbtrail
/// ----------------------------------------------- 
///  Modified by : Ponraman Vijayakumar
//   Last modified :14/10/2013 
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
* @class   CRUMB_0012_Playback
*
* @brief   A micro iex test 1.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
[Test("CRUMB_0012")]
public class CRUMB_0012: _Test
{
    /**********************************************************************************************/
    /**
* @brief   Checking CRUMBTITLE during playback.
**************************************************************************************************/
    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/
    /**
* @brief   The service to be recorded.
**************************************************************************************************/
    private static Service serviceToBeRecorded;
    /**********************************************************************************************/
    /**
* @brief   The event to be recorded.
**************************************************************************************************/
    private static string eventToBeRecorded = "EVENT_TO_BE_RECORDED";
  
    /**********************************************************************************************/
    /**
* @brief   The Crumbtextdisplayinplayback.
**************************************************************************************************/
    private static string Crumbtextdisplayinplayback;
    /**********************************************************************************************/
    /**
* @brief   The crumbonplayback.
**************************************************************************************************/
    private static string crumbonplayback;
    /**********************************************************************************************/
    /**
* @class   Constants
*
* @brief   A constants.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    private static class Constants
    {
        /**********************************************************************************************/
        /**
* @brief   The check if video is present.
**************************************************************************************************/
        public const bool checkIfVideoIsPresent = true;
        /**********************************************************************************************/
        /**
* @brief   The check full video area.
**************************************************************************************************/
        public const bool checkFullVideoArea = true;
        /**********************************************************************************************/
        /**
* @brief   The time to check for video.
**************************************************************************************************/
        public const int timeToCheckForVideo = 10;
        /**********************************************************************************************/
        /**
* @brief   The check if audio is present.
**************************************************************************************************/
        public const bool checkIfAudioIsPresent = true;
        /**********************************************************************************************/
        /**
* @brief   The time to check for audio.
**************************************************************************************************/
        public const int timeToCheckForAudio = 10;
        /**********************************************************************************************/
        /**
* @brief   The minimum time before event end.
**************************************************************************************************/
        public const int minTimeBeforeEventEnd = 4;
        /**********************************************************************************************/
        /**
* @brief   The security to wait for record.
**************************************************************************************************/
        public const int secToWaitForRecord = 60;
        /**********************************************************************************************/
        /**
* @brief   The value for full playback.
**************************************************************************************************/
        public const int valueForFullPlayback = 0;
        /**********************************************************************************************/
        /**
* @brief   The play from beginning.
**************************************************************************************************/
        public const bool playFromBeginning = true;
        /**********************************************************************************************/
        /**
* @brief   The verify EOF.
**************************************************************************************************/
        public const bool verifyEOF = false;
    }
    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,Crumbtitle & crumbtilte value From ini File";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/
    private const string STEP1_DESCRIPTION = "Step 1:Schedule a recording to happen in the required service.Wait for the recording to finish ";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/
    private const string STEP2_DESCRIPTION = "Step 2:Playback the record and check CRUMBTITLE ";
    #region Create Structure
    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author  Ponraman V
* @date    10/21/2013
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
    /**********************************************************************************************/
    /**
* @fn  public override void PreExecute()
*
* @brief   Pre execute.
*
* @author  Ponraman V
* @date    10/21/2013
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
    /**
* @class   PreCondition
*
* @brief   A pre condition.Retreving Channel info from Content XML.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            LogCommentWarning(CL, "Warning:No UI validation and dictionary-key values from Dictionary file are not validated");
            //Get Values From Content XML File
            serviceToBeRecorded = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            Crumbtextdisplayinplayback = CL.EA.UI.Utils.GetValueFromDictionary("DIC_ACTION_NO");
            PassStep();
        }
    }
    #endregion
    #region Step1
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.Schedule a recording to happen in the required service.Wait for the recording to finish 
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.
*
* @author  Ponraman V
* @date    10/23/2013
**************************************************************************************************/
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded.LCN);
            }
            //Check for video if it is a video service
            
                LogCommentInfo(CL, "Checking for video as it is a video service..");
                res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Video not present on service - " + serviceToBeRecorded.LCN);
                }
            
            //Check if Audio is present
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on service - " + serviceToBeRecorded.LCN);
            }
            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service - " + serviceToBeRecorded.LCN);
            }
            //Wait for some time
            res = CL.IEX.Wait(Constants.secToWaitForRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after start of recording!");
            }
            PassStep();
        }
    }
    #endregion
    #region Step2
    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   A step 2.Playback the record and check CRUMBTITLE
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   A step 2.
*
* @author  Ponraman V
* @date    10/23/2013
**************************************************************************************************/
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Playback the recording
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.valueForFullPlayback, Constants.playFromBeginning, Constants.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Playback of event failed.");
            }
            //Check whether the video is present
            
                LogCommentInfo(CL, "Checking for video as it is a video service..");
                res = CL.EA.CheckForVideo(Constants.checkIfVideoIsPresent, Constants.checkFullVideoArea, Constants.timeToCheckForVideo);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Video not present on playback!!");
                }
            
            //Check if Audio is present
            res = CL.EA.CheckForAudio(Constants.checkIfAudioIsPresent, Constants.timeToCheckForAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio not present on playback!!");
            }
            //Retreving CRUMBTITLE info during playback
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonplayback);
            if (String.IsNullOrEmpty(crumbonplayback))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonplayback.Equals(Crumbtextdisplayinplayback))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed in Main Menu  screen " +"Expected:" +crumbonplayback + "Actual:" + Crumbtextdisplayinplayback);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in Main Menu Screen");
            }
            //Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop playback!!");
            }
            PassStep();
        }
    }
    #endregion
    #endregion
    #region PostExecute
    /**********************************************************************************************/
    /**
* @fn  public override void PostExecute()
*
* @brief   Posts the execute.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    [PostExecute()]
    public override void PostExecute()
    {
    }
    #endregion
}