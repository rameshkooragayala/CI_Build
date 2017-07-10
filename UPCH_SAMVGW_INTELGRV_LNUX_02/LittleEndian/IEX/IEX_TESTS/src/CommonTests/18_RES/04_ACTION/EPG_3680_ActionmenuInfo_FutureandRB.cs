/// <summary>
///  Script Name : EPG_3680_ActionmenuInfo_FutureandRB.cs
///  Test Name   : EPG-3680-Actionmenu information during Review buffer plaback-FT-148,EPG-3930-Simple programme information for future programme-FT -148
///  TEST ID     : 
///  QC Version  : 1
///  Variations from QC: Step 3 of 3810 not covered as its for affiliates
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

[Test("ACTION_EPG_3680")]
public class ACTION_EPG_3680 : _Test
{

    [ThreadStatic]
    private static _Platform CL;


    private static Service Service_1;
    private static Service Service_2;
    private static string _favoriteChannelList = "";
    private static bool isReviewBuffer = false;

    private static class Constants
    {
        public const int speedForPlay = 1;
        public const int speedForPause = 0;
        public const int timeToWaitinPause = 15;
    }

    #region Create Structure
    public override void CreateStructure()
    {
        //Get Client Platform
        CL = GetClient();
        //Get value from Test ini
        isReviewBuffer = Convert.ToBoolean(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ReviewBuffer"));
        LogCommentInfo(CL, "Value Of review buffer is : " + isReviewBuffer);

        this.AddStep(new PreCondition(), "Precondition: Set the service with and without channel logo as favourites.");
        if (isReviewBuffer)
        {
            this.AddStep(new Step1(), "Step 1:  Collect RB on service with channel logo and verify the info on action menu");
            this.AddStep(new Step2(), "Step 2:  Collect RB on service without channel logo  and verify the info on action menu");
        }
        else
        {
            this.AddStep(new Step3(), "Step 1:  Navigate on future event on the service with channel logo and  verify the info");
            this.AddStep(new Step4(), "Step 2:  Navigate on future event on the service without channel logo and  verify the info");
        }


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
            res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + Service_1.LCN + ", " + Service_2.LCN, EnumFavouriteIn.Settings);
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

            //Collect RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause on Live");
            }

            res = CL.IEX.Wait(Constants.timeToWaitinPause);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play RB");
            }

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar
            res = CL.EA.LaunchActionBar();
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
          
            string expectedChannelName = "";
            expectedChannelName = Service_2.Name;
            LogComment(CL, "Expected Channel name is" + expectedChannelName);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + Service_2.LCN);
            }

            //Collect RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause on Live");
            }

            res = CL.IEX.Wait(Constants.timeToWaitinPause);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play RB");
            }

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar
            res = CL.EA.LaunchActionBar();
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
    #endregion Step2

    #region Step3
    private class Step3 : _Step
    {

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + Service_1.LCN);
            }
			
			//Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar next

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on future event ");
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
    #endregion Step3

    #region Step4
    private class Step4 : _Step
    {

        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + Service_1.LCN);
            }
            
            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar next

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on future event ");
            }

           
            String obtainedChannelLogo = CL.EA.GetChannelLogo();
            LogComment(CL, "Obtained ChannelLogo is =  " + obtainedChannelLogo);

            if (String.IsNullOrEmpty(obtainedChannelLogo))
            {
                LogComment(CL, "Channel logo is not present = " + obtainedChannelLogo);
            }

            else
            {
                FailStep(CL, res, "Channel logo is present ");
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
    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        
        //Flush RB
        res = CL.EA.ReturnToLiveViewing();
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to return to Live");
        }
        CL.IEX.Wait(5);
        
        //Remove favourite service
        res = CL.EA.STBSettings.UnsetAllFavChannels();
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to UnsetAllFavChannels");
        }
        CL.IEX.LogComment("Removed Favourite List");
    }

    #endregion PostExecute
}

