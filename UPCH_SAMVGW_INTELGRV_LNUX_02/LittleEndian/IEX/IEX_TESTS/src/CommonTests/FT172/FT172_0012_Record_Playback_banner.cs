/// <summary>
///  Script Name : FT172_0012_Record_Playback_banner.cs
///  Test Name   : FT172_0012_Record_Playback_banner
///  TEST ID     : 74294
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 26/06/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT172_0012")]
public class FT172_0012 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static Service s1;      // s1: favourite with channel logo
    static Service s2;      // s2: non-favourite with channel logo
    static Service s3;      // s3: favourite without channel logo
    static Service s4;      // s4: non-favourite without channel logo

    static class Constants
    {
        public static int minDurationInEvent = 1;
        public static string eventKey_1 = "E1";
        public static string eventKey_2 = "E2";
        public static string eventKey_3 = "E3";
        public static string eventKey_4 = "E4";
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Set favourite channels, record events E1, E2, E3, E4 and enable favourite mode";
    private const string STEP1_DESCRIPTION = "Step 1: Play event E1 and check information on banner";
    private const string STEP2_DESCRIPTION = "Step 2: Play event E3 and check information on banner";
    private const string STEP3_DESCRIPTION = "Step 3: Play event E2 and check information on banner";
    private const string STEP4_DESCRIPTION = "Step 4: Exit favourite, play event E1 and check information on banner";
    private const string STEP5_DESCRIPTION = "Step 5: Play event E4 and check information on banner)";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);

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

            // Get list of channel to use for recordings
            s1 = CL.EA.GetServiceFromContentXML("Encryption=Clear;HasChannelLogo=True");
            if (s1 == null)
            {
                FailStep(CL, "Failed to get clear service with channel logo from Content.xml");
            }

            s2 = CL.EA.GetServiceFromContentXML("Encryption=Clear;HasChannelLogo=True", "Name=" + s1.Name);
            if (s2 == null)
            {
                FailStep(CL, "Failed to get 2 clear services with channel logo from Content.xml");
            }

            s3 = CL.EA.GetServiceFromContentXML("Encryption=Clear;HasChannelLogo=False");
            if (s3 == null)
            {
                FailStep(CL, "Failed to get clear service without channel logo from Content.xml");
            }

            s4 = CL.EA.GetServiceFromContentXML("Encryption=Clear;HasChannelLogo=False", "Name=" + s3.Name);
            if (s4 == null)
            {
                FailStep(CL, "Failed to get 2 clear services without channel logo from Content.xml");
            }
			
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to set the banner display time out to 15");
            }
			
            // Unset all the current favourites channel 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove all Favorite channel");
            }
            
            // Set favorite channels
            res = CL.EA.STBSettings.SetFavoriteChannelNameList(s1.Name + "," + s3.Name, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels " + s1.LCN + " and " + s3.LCN + " as favorites");
            }           

            // Record E1, E2, E3, E4 on S1, S2, S3, S4
            recordEvent(s1, Constants.eventKey_1, this);
            recordEvent(s2, Constants.eventKey_2, this);
            recordEvent(s3, Constants.eventKey_3, this);
            recordEvent(s4, Constants.eventKey_4, this);

            // Set favorite mode            
            if (!enableFavoriteMode())
            {
                FailStep(CL, "Failed to set favorite mode");
            }

            PassStep();
        }
    }
    #endregion

    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Playback of event E1
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventKey_1, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording E1");
            }

            // Check playback banner
            verifyPVRChannelBar(s1, true, "1", CL.EA.UI.Events[Constants.eventKey_1].Name, this);

            // Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            // Playback of event E3
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventKey_3, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording E3");
            }

            // Check playback banner
            verifyPVRChannelBar(s3, true, "2", CL.EA.UI.Events[Constants.eventKey_3].Name, this);

            // Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Playback of event E2
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventKey_2, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording E2");
            }

            // Check playback banner
            verifyPVRChannelBar(s2, false, s2.LCN, CL.EA.UI.Events[Constants.eventKey_2].Name, this);

            // Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Exit favourite mode 
            if (!disableFavoriteMode())
            {
                FailStep(CL, "Failed to disable favorite mode");
            }

            // Playback of event E1
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventKey_1, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording E1");
            }

            // Check playback banner
            verifyPVRChannelBar(s1, true, s1.LCN, CL.EA.UI.Events[Constants.eventKey_1].Name, this);

            // Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording playback");
            }

            PassStep();
        }
    }
    #endregion

    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Playback of event E4
            res = CL.EA.PVR.PlaybackRecFromArchive(Constants.eventKey_4, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording E4");
            }

            // Check playback banner
            verifyPVRChannelBar(s4, true, s4.LCN, CL.EA.UI.Events[Constants.eventKey_4].Name, this);

            // Stop playback
            res = CL.EA.PVR.StopPlayback();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording playback");
            }

            PassStep();
        }
    }
    #endregion

    // Record current event on the given channel
    private static void recordEvent(Service service, string eventKeyName, _Step step)
    {
        // Tune to the given channel
        IEXGateway._IEXResult res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
        if (!res.CommandSucceeded)
        {
            step.FailStep(CL, res, "Failed to tune to channel " + service.LCN);
        }

        // Record the current event
        res = CL.EA.PVR.RecordCurrentEventFromBanner(eventKeyName, Constants.minDurationInEvent, false, false);
        if (!res.CommandSucceeded)
        {
            step.FailStep(CL, res, "Failed to record current event on service " + service.LCN);
        }

        // Wait for some time for event to record
        res = CL.IEX.Wait(Constants.minDurationInEvent * 60);

        // Stop recording
        res = CL.EA.PVR.StopRecordingFromArchive(eventKeyName);
        if (!res.CommandSucceeded)
        {
            step.FailStep(CL, res, "Failed to stop recording " + eventKeyName + " from archive");
        }
    }

    // Launch channel bar and check information displayed
    private static void verifyPVRChannelBar(Service service, bool isFavourite, string channelNumber, string eventName, _Step step)
    {
        // Get poster url displayed in action bar (from archive recordings) to compare with url displayed in channel bar
        string expPosterUrl = "";
        try
        {
            CL.EA.UI.Utils.GetEpgInfo("thumbnail", ref expPosterUrl);
        }
        catch (Exception) { }

        CL.EA.UI.Utils.ClearEPGInfo();
        
        // Launch channel bar
        try
        {
            CL.EA.UI.ChannelBar.Navigate();
        }
        catch (Exception)
        {
            step.FailStep(CL, "Failed to launch channel bar during record playback");
        }

        // Check channel logo
        if (service.HasChannelLogo.Equals("True"))
        {
            string channelLogo = "";
            try{
                CL.EA.UI.Utils.GetEpgInfo("channel_logo", ref channelLogo);
            }
            catch(Exception)
            {
                step.FailStep(CL, "Failed to verify channel bar: Unable to get 'channel_logo' milestone");
            }

            if (string.IsNullOrEmpty(channelLogo))
            {
                step.FailStep(CL, "Failed to verify channel bar: Channel logo is null or empty");
            }
        }
        // Else check channel name
        else 
        {
            string channelName = "";
            try
            {
                CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName);
            }
            catch(Exception){
                step.FailStep(CL, "Failed to verify channel bar: Unable to get 'chname' milestone");
            }

            if (channelName != service.Name)
            {
                step.FailStep(CL, "Failed to verify channel bar: wong channel name. Expected: " + channelName + ", Returned: " + service.Name);
            }
        }

        // Check favourite indication
        string favouriteMilestone = "";
        try
        {
            CL.EA.UI.Utils.GetEpgInfo("IsFavourite", ref favouriteMilestone);
        }
        catch(Exception)
        {
            step.FailStep(CL, "Failed to verify channel bar: unable to get 'IsFavourite' milestone");
        }

        if ((isFavourite) && (favouriteMilestone != "True"))
        {
            step.FailStep(CL, "Failed to verify channel bar: Wrong favourite indication. Expected: indication displayed, Returned: indication not displayed");
        }
        else if ((!isFavourite) && (favouriteMilestone == "True"))
        {
            step.FailStep(CL, "Failed to verify channel bar: Wrong favourite indication. Expected: indication not displayed, Returned: indication displayed");
        }           
        
        // Check channel number 
        string channelNb= "";
        try
        {
            CL.EA.UI.Utils.GetEpgInfo("chnum", ref channelNb);
        }
        catch(Exception)
        {
            step.FailStep(CL, "Failed to verify channel bar: Unable to get 'chnum' milestone");
        }

        if (channelNb != channelNumber)
        {
            step.FailStep(CL, "Failed to verify channel bar: Wrong channel number. Expected: " + channelNumber + ", Returned: " + channelNb);
        }
        //Waiting for few seconds as it takes time to load the thumbnail
        CL.IEX.Wait(8);
        // Check poster
        string posterUrl = "";
        try
        {
            CL.EA.UI.Utils.GetEpgInfo("thumbnail", ref posterUrl);
            if (posterUrl != expPosterUrl)
            {
                step.FailStep(CL, "Failed to verify channel bar: Wrong poster url. Expected: " + expPosterUrl + ", Returned: " + posterUrl);
            }
        }
        catch (Exception)
        {
            if (expPosterUrl != "")
            {
                step.FailStep(CL, "Failed to verify channel bar: Wrong poster url. Expected: " + expPosterUrl + ", Returned: null");
            }
        }
 
        // Check event name
        string evtName = "";
        try
        {
            CL.EA.UI.Utils.GetEpgInfo("evtname", ref evtName);
        }
        catch(Exception)
        {
            step.FailStep(CL, "Failed to verify channel bar: Unable to get 'evtname' milestone");
        }

        if (evtName != eventName)
        {
            step.FailStep(CL, "Failed to verify channel bar: Wrong event name. Expected: " + eventName + ", Returned: " + evtName);
        }
    }

    // Enable favorite mode in action menu
    private static bool enableFavoriteMode()
    {
        try
        {
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ENABLE FAVOURITE MODE");
            return true;
        }
        catch (Exception)
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("DISABLE FAVOURITE MODE"))
                {
                    CL.EA.UI.Utils.SendIR("RETOUR");
                    return true;
                }
            }
            catch (Exception) { }
        }
        return false;
    }

    // Disable favorite mode in action menu
    private static bool disableFavoriteMode()
    {
        try
        {
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:DISABLE FAVOURITE MODE");
            return true;
        }
        catch (Exception)
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("ENABLE FAVOURITE MODE"))
                {
                    CL.EA.UI.Utils.SendIR("RETOUR");
                    return true;
                }
            }
            catch (Exception) { }
        }
        return false;
    }

    #endregion

    #region PostExecute

    public override void PostExecute()
    {
        //Unset all the current favourites channel 
        CL.EA.STBSettings.UnsetAllFavChannels();
		CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._5);
    }

    #endregion PostExecute
}




