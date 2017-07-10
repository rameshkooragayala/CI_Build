/// <summary>
///  Script Name : PLB_AudioSubtitleSelection.cs
///  Test Name   : PLB-0300-AudioSelection,PLB-0350-SubtitlesSelection,PLB-0351-SubtitlesSelectionTeletext
//   Test Repository : FR_Fusion/UPC
///  TEST ID     : 68953,68954,68955
///  QC Version  : 2
///  Variations from QC:Last step is not coveerd as it is coverd as part of prior scripts.
/// ----------------------------------------------- 
///  Modified by : 
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
 * @class   PLB_AudioSubtitleSelection
 *
 * @brief   Plb audio subtitle selection.
 *
 * @author  Madhur
 * @date    10/7/2013
 **************************************************************************************************/

[Test("PLB_AudioSubtitleSelection")]
public class PLB_AudioSubtitleSelection : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The event to be recorded.
     **************************************************************************************************/

    static string eventToBeRecorded = "";

    /**********************************************************************************************//**
     * @brief   The service.
     **************************************************************************************************/

    private static Service service;

    /**********************************************************************************************//**
     * @brief   Options for controlling the no ofavailable.
     **************************************************************************************************/

    private static int NoOfavailableOptions = 0;
    private static string serviceType;

    /**********************************************************************************************//**
     * @brief   The next audio.
     **************************************************************************************************/

    private static string nextAudio = "";

    /**********************************************************************************************//**
     * @brief   The state.
     **************************************************************************************************/

    private static string stateOnPlayback = "";
    private static string stateOnLive = "";
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();

    /**********************************************************************************************//**
     * @brief   List of options.
     **************************************************************************************************/

    private static List<string> OptionList = new List<string>();

    /**********************************************************************************************//**
     * @brief   Information describing the precondition.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**********************************************************************************************//**
     * @brief   Event queue for all listeners interested in STEP1_DESCRIPTION events.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Record an event and playback";

    /**********************************************************************************************//**
     * @brief   Information describing the step 2.
     **************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Check expected audio/subtitle streams are displayed (no audio/subtile type missing)";

    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Madhur
     * @date    10/7/2013
     **************************************************************************************************/

    private static class Constants
    {
        /**********************************************************************************************//**
         * @brief   In minutes.
         **************************************************************************************************/

        public const int minTimeBeforeEventEnd = 5;

        /**********************************************************************************************//**
         * @brief   In minutes.
         **************************************************************************************************/

        public const int recordDuration = 4 ;

        /**********************************************************************************************//**
         * @brief   The security to play.
         **************************************************************************************************/

        public const int secToPlay = 0;

        /**********************************************************************************************//**
         * @brief   in miliseconds.
         **************************************************************************************************/

        public const int waitAfterSendingCommand = 1000;
       
    }

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Madhur
     * @date    10/7/2013
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
     * @author  Madhur
     * @date    10/7/2013
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
     * @brief   Gets the values from ini file and service from content xml.
     *
     * @author  Madhur
     * @date    10/7/2013
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/7/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");
            //serviceType = "ParentalRating=High;NoOfSubtitleLanguages=0,1;SubtitleType=Dvb";

            service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", serviceType);
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }


            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Record the current event.
     *
     * @author  Madhur
     * @date    10/7/2013
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/7/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Tune to the service whose event will be recorded
            
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }
            
            //Based on the option Audio/Subtitle fetch no.of langauges,language available and state to navigate
            if (serviceType.Contains("NoOfAudioLanguages"))
            {
                NoOfavailableOptions = Convert.ToInt32(service.NoOfAudioLanguages);
                OptionList = service.AudioLanguage;
                stateOnPlayback = "STATE:PB AV SETTING AUDIO";
                stateOnLive = "STATE:AV SETTINGS AUDIO";
            }
            else
                if (serviceType.Contains("NoOfSubtitleLanguages"))
            {
                NoOfavailableOptions = Convert.ToInt32(service.NoOfSubtitleLanguages);
                OptionList = service.SubtitleLanguage;
                stateOnPlayback = "STATE:PB AV SETTING SUBTITLES";
                stateOnLive = "STATE:AV SETTING SUBTITLES";
            }

            //Check the option list on live.
            //Navigate to option audio/subtitle from Live
            
            res = CL.EA.NavigateAndHighlight(stateOnLive, dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to "+stateOnLive);
            }

            string currSel = "";
           //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currSel);

            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            LogCommentInfo(CL, "Next Audio/subtitle feteched from project ini is : " + nextAudio);

            for (int match = 0; match < NoOfavailableOptions; match++)
            {
                if (OptionList.Contains(currSel))
                {
                    LogCommentInfo(CL, "option" + match + ":" + currSel + "is present in the list");
                }
                else 
                {
                    FailStep(CL, res, "option" + match + ":" + currSel + "is not present in the list");
                }
                //Navigate to next option in the list
                string timeStamp = "";
                res = CL.IEX.IR.SendIR(nextAudio, out timeStamp,Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to next audio/subtitle in the list");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currSel);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the current selection");
                }
            }

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner("eventToBeRecorded", Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            LogCommentInfo(CL, "Waiting for " + Constants.recordDuration + " minutes to ensure sufficient duration of recording to test all options");
            res = CL.IEX.Wait(Constants.recordDuration * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required");
            }

            // Stop Recording
            res = CL.EA.PVR.StopRecordingFromArchive("eventToBeRecorded", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from Archive");
            }


            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   Step 2 PlayBack from Archieve and  Check expected audio/subtitle streams are displayed.
     *
     * @author  Madhur
     * @date    10/7/2013
     **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/7/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Playback from archieve
            res = CL.EA.PVR.PlaybackRecFromArchive("eventToBeRecorded", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + eventToBeRecorded + " From Archive");
            }
                        
            
            //Navigate to option audio/subtitle from playback

            res = CL.EA.NavigateAndHighlight(stateOnPlayback, dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS SUBTITLES");
            }

            string currSel = "";
           //get the current selction
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currSel);

            nextAudio = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_AUDIO");
            LogCommentInfo(CL, "Next Audio/Subtitle feteched from project ini is : " + nextAudio);

            for (int match = 0; match < NoOfavailableOptions; match++)
            {
                if (OptionList.Contains(currSel))
                {
                    LogCommentInfo(CL, "option" + match + ":" + currSel + "is present in the list");
                }
                else 
                {
                    FailStep(CL, res, "option" + match + ":" + currSel + "is not present in the list");
                }
                //clear EPG info
                res = CL.IEX.MilestonesEPG.ClearEPGInfo();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to clear epg info",false);
                }
                string timeStamp = "";
                res = CL.IEX.IR.SendIR(nextAudio, out timeStamp,Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to next audio/Subtitle in the list");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currSel);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the current selection");
                }
            }
            
            //Stop Playing 
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live");
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
     * @brief   Delete the recorded content.
     *
     * @author  Madhur
     * @date    10/7/2013
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Delete  recordings in archive
        res = CL.EA.PVR.DeleteRecordFromArchive("eventToBeRecorded");
         if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }
    }
    #endregion
}