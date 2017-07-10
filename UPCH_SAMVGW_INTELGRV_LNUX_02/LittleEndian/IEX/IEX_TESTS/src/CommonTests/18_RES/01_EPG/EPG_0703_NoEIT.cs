/// <summary>
///  Script Name : EPG_0703_NoEIT
///  Test Name   : EPG-0703-EIT-Missing Programme Information For Future Event
///  TEST ID     : 64377
///  JIRA ID     : FC-456
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 7/19/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class EPG_0703 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service noEITService;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: Tune to service, Launch the channel and  move the focus on next event then Check Event have No EIT and Default Thumbnail
        //Pre-conditions: Get Channel from xml file
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From XML File");
        this.AddStep(new Step1(), "Step 1: Tune to service S1 and check that next event have no EIT and default thumbnail");

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

            //Navigate to Channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Select the next (channel bar) event
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //Check Next Event EventName in Channel Bar
            String nextEventName;
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out nextEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Name From Channel Bar");
            }

            //Check Default EventName for the Event which have NO EIT
            String noEITeventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_NO_EVT_INFO_LIVE");
            if (!nextEventName.Equals(noEITeventName))
            {
                FailStep(CL, res, "Failed to verify that this event has No EIT");
            }
            LogCommentInfo(CL, "Verified that this Event has No EIT");

            //Check Next Event Thumbnail in Channel Bar
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
           //     FailStep(CL, res, "Failed to verified that this event has Default thumbnail");
           // }
            LogCommentInfo(CL, "Verified that this Event has Default Thumbnail");

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}