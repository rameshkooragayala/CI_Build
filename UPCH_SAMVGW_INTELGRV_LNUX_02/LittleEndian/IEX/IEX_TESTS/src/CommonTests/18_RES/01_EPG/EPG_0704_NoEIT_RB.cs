/// <summary>
///  Script Name : EPG_0704_NoEIT_RB
///  Test Name   : EPG-0704-EIT-Missing Information During RB Playback
///  TEST ID     : 64538
///  JIRA TASK   : FC-457
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ramya BR
///  Modified Date : 26/07/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class EPG_0704 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service noEITService;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: Tune to service, Playback the event of the service and launch the channel bar then Check Event have No EIT.
        //Pre-conditions: Get Channel from xml file
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From XML File");
        this.AddStep(new Step1(), "Step 1: Tune to service S1, Playback the event of the service and launch the channel bar then Check Event have No EIT");

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
            noEITService = CL.EA.GetServiceFromContentXML("IsEITAvailable=False;Type=Video", "ParentalRating=High");
            if (noEITService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to set the Banner display timeout to 15");
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

            //Tune to service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, noEITService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + noEITService.LCN);
            }

            //Clear EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to clear EPG info");
            }

            //Rewind the event
            String pause = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "PAUSE");
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToDouble(pause), false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause the Event");
            }

            //Wait 60sec
            res = CL.IEX.Wait(60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait for Playback Review Buffer");
            }

            //Playback the event
            String playSpeed = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "PLAY");
            Double plySpeed = Convert.ToDouble(playSpeed);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", plySpeed, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Launch the Channelbar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            //Check EventName in Channel Bar
            String EventName;
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out EventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Name From Channel Bar");
            }

            //Check Default EventName for the Event which have NO EIT
            String noEITeventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_NO_EVT_INFO");
            if (!EventName.Equals(noEITeventName))
            {
                FailStep(CL, res, "Failed to verify that this Event have No EIT");
            }
            LogCommentInfo(CL, "Verified that this Event have No EIT");

            //Waiting for 10 seconds as thumbnail is taking some time to load

            CL.IEX.Wait(10);

            //Check Event Thumbnail in Channel Bar
            String thumbnailName;
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out thumbnailName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            if (string.IsNullOrEmpty(thumbnailName))
            {
                FailStep(CL,"Thumbnail value fecthed from EPG is null");
            }

            //Check Default Thumbnail for the Event which have no EIT
            String thumbnailDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
            if (thumbnailName.Equals(thumbnailDefault))
            {
                LogCommentImportant(CL, "Thumbnail for NO EIT is same as Default Thumbnail "+thumbnailDefault);
            }
            else
            {
                FailStep(CL,"Thumbnail Fetched for No EIT event "+thumbnailName+" is different from "+thumbnailDefault);
            }
            //Check Default Thumbnail for the Event which have no EIT
           // String thumbnailDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
			
           // if (thumbnailName.Equals(thumbnailDefault))
           // {
           //     FailStep(CL, res, "Failed to verify that this Event have Default thumbnail");
           // }
            LogCommentInfo(CL, "Verified that this Event have Default Thumbnail");

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
		
        CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._5);
		
        //Exit to fullscreen
        res = CL.EA.ReturnToLiveViewing(true);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to return to fullscreen");
        }
    }

    #endregion PostExecute
}