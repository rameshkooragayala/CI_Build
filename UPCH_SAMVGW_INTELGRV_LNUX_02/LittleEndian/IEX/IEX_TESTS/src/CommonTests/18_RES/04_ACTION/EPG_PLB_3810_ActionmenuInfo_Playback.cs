/// <summary>
///  Script Name : EPG_PLB_3810_ActionmenuInfo_Playback.cs
///  Test Name   : EPG-PLB-3810-Actionmenu information during playback of recording,EPG-PLB-3101-Simple information playback banner
///  TEST ID     : 26496,26498
///  QC Version  : 1
///  Variations from QC:Step 3 of 3810 not covered.
//  Step 3 of 3101 is covered in EPG_PLB_0011_Action_Menu_Simple_Program_Info.cs
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 18 Sept 2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("ACTION_EPG_PLB_3810")]
public class ACTION_EPG_PLB_3810 : _Test
{

    [ThreadStatic]
    private static _Platform CL;

    
    private static Service Service_1;
    private static Service Service_2;
    private static string _favoriteChannelList = "";
    
    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 4;
        public const int recordDuration = 2 * 60;
        public const int secToPlay = 0;
    }

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition:Get values from XML and ini files");
        this.AddStep(new Step1(), "Step 1: Record a service with channel logo and verify on playback all the info");
        this.AddStep(new Step2(), "Step 2: Record a service without channel logo and verify on playback all the info");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion Create Structure

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From xml File
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_1.LCN);
            }
            //Get Values From xml File
            int channel1 = Convert.ToInt32(Service_1.LCN);
            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=False", "ParentalRating=High");
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_2.LCN);
            }
            int channel2 = Convert.ToInt32(Service_2.LCN);
  
             //remove all favorites channels 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to UnsetAllFavChannels");
            }
            // Set favorite channels
            _favoriteChannelList = "Service_1.LCN,Service_2.LCN";
            res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + Service_1.LCN + ", " + Service_2.LCN , EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels " + _favoriteChannelList + " as favorites");
            }
            
            PassStep();
        }
    }
    #endregion PreCondition

    #region Step1
    private class Step1 : _Step
    {

       public override void Execute()
        {
            StartStep();
        
             res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + Service_1.LCN);
            }

            //Record event and play it
            res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent", Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }

            //Wait to Record for 2 mins
            res = CL.IEX.Wait(Constants.recordDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res ,"Failed while waiting to record");
            }

            res = CL.EA.PVR.StopRecordingFromBanner("RecEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
            }

            //Playback the Already Recorded Event from Archive.
            res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play back from archieve");
            }

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on Playback");
            }

            String obtainedChannelLogo = CL.EA.GetChannelLogo();
            LogComment(CL, "Obtained ChannelLogo is =  " + obtainedChannelLogo);

            if (!String.IsNullOrEmpty(obtainedChannelLogo))
            {
                LogComment(CL, "Channel logo is present = " + obtainedChannelLogo);
            }

            else
            {
                FailStep(CL, res, "Channel logo is empty ");
            }

            String IsFavourite = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("IsFavourite", out IsFavourite);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail on Future event from Action Menu");
            }

            if (IsFavourite == "True")
            {
                LogComment(CL, "Service " + Service_1.LCN + "is set as favorite");

            }
            else
            {
                FailStep(CL, res, "Failed to display Favorite channel indication");
            }

            //get Progressbar time
            LogCommentInfo(CL, "get Progressbar during Playback");
            string progressTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("ProgressBarTime", out progressTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get ProgressBarTime");
            }
            PassStep();
    }
}
    #endregion Step1

    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }
            string expectedChannelName = "";
            expectedChannelName = Service_2.Name;
            LogComment(CL, "Expected Channel name is" + expectedChannelName);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + Service_2.LCN);
            }

            //Record event and play it
            res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent1", Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }

            //Wait to Record for 2 mins
            res = CL.IEX.Wait(Constants.recordDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed while waiting to record");
            }

            res = CL.EA.PVR.StopRecordingFromBanner("RecEvent1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
            }

            //Playback the Already Recorded Event from Archive.
            res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent1", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play back from archieve");
            }

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on Playback");
            }

            String obtainedChannelLogoURL = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("channel_logo", out obtainedChannelLogoURL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel logo");
            }
            String obtainedChannelName = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out obtainedChannelName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel name");
            }
            LogComment(CL, "obtainedChannelName is " + obtainedChannelName);

            //Validating for channel name in the absence of channel logo.

            if (!String.IsNullOrEmpty(obtainedChannelLogoURL))
            {
                FailStep(CL, res, "Channel logo is present and is diplayed instead of channel name " + obtainedChannelLogoURL);
            }
            else
            {
                LogComment(CL, "obtainedChannelName is " + obtainedChannelName);
                LogComment(CL, "Expected Channel name is " + expectedChannelName);

                if (obtainedChannelName.Equals(expectedChannelName))
                {
                    LogComment(CL, "Expected Channel name recieved " + expectedChannelName);
                }

                else
                {
                    FailStep(CL, res, "Expected Channel name not recieved " + expectedChannelName);
                }
            }

            String IsFavourite = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("IsFavourite", out IsFavourite);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display ISfavavourite indication milestones from Action Menu");
            }

            if (IsFavourite == "True")
            {
                LogComment(CL, "Service " + Service_2.LCN + "is set as favorite");

            }
            else
            {
                FailStep(CL, res, "Failed to display Favorite channel indication");
            }

            PassStep();
        }
    }
    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;


        //Delete the recorded event  from Archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive(true);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to delete the recorded event from archive!");
        }
    }

    #endregion PostExecute
}

