/// <summary>
///  Script Name : SET_SubtitleDefLanguage.cs
///  Test Name | TEST ID : SET-SUBT-0016-Playback-subtitle language default
///  TEST ID     : 71586
///  Variations from QC: Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Developed by : Sandesh Mainkar.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("SET_SubtitleDefLanguage")]
public class SET_SubtitleDefLanguage : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    private static Service service;
    static String eventToBeRecorded = "EVENT TO BE RECORDED";
    static String SubtState = "";
    static String SubtDisable = "";
    static String DefLang = "";
    private static List<string> OptionList = new List<string>();
    private static Dictionary<EnumEpgKeys,String> dictionary = new Dictionary<EnumEpgKeys,String>();

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Service From Content.xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Playback the recording and validate the default subtitle language";
    
    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 4;     // In minutes
        public const int waitForRecording = 4;          // In Minutes
        public const int secToPlay = 0;                 // amount of time to playback the Recorded event.
        public const Boolean playFromBeginning = true;  //Where to start the playback from.
        public const Boolean verifyEOF = false;         //Need to verify End-Of_File ?
    }

    #region Create Structure
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
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From content.xml File
            service = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;NoOfSubtitleLanguages=0,1");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }
                OptionList = service.SubtitleLanguage;

            //Tune to a service where Subtittles has to be verified.
            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }

            //If Subtitles option is not set to OFF by default, Set it to OFF.

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLE DISPLAY ON");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Switch ON the SUBTITLES under settings ");
            }

            //Now Record some event in any of the services.
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            LogCommentInfo(CL, "Waiting for " + Constants.waitForRecording + " minutes to ensure sufficient duration of recording to test all trickmodes");
            res = CL.IEX.Wait(Constants.waitForRecording * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required");
            }

            // Stop Recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    public class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            String defSubtDisp = "";
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constants.secToPlay, Constants.playFromBeginning, Constants.verifyEOF);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + eventToBeRecorded + " From Archive");
            }

            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to SUBTITLES on AV SETTINGS. REASON: " + res.FailureReason);
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out DefLang);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPGinfo: title for the Default Language. REASON: " + res.FailureReason);
            }
			//assigning value to the variable to fetch default value
            defSubtDisp = DefLang;
            //Fetch the default language value from the Content.xml

            String def_subtLang = CL.EA.UI.Utils.GetValueFromProject("UI_LANGUAGE_ITEMS","DEFAULT_SUBTITLE");
            if (OptionList.Contains(def_subtLang))
            {
	                if (!defSubtDisp.Equals(def_subtLang))
	        		{
	                		FailStep(CL, res, "SUBT language during the playback is not the default(expected).REASON: " + res.FailureReason);
	            	}
            }

            res = CL.EA.ReturnToPlaybackViewing();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to return to playback because of the reason:" + res.FailureReason);
            }


            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

        // Clean-up the recorded events from the disk.
        IEXGateway._IEXResult res;
        String defSubtDisp = "";

        //Delete all recordings in archive
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to stop the playback because of the reason:" + res.FailureReason);
        }

        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLES SETTING");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to navigate to SUBTITLES DISPLAY under settings " + res.FailureReason);
        }
        defSubtDisp = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DEFAULT");
        if (defSubtDisp == "")
        {
            LogCommentFailure(CL, "Field DEFAULT is not available in the SUBTITLE section.");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defSubtDisp);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Select the Default option under the Subtitles settings" + res.FailureReason);
        }

        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }

    }
    #endregion
}