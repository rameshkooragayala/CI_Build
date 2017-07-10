
/// <summary>
///  Script Name :CHBAR_0705_presenting_programme_information.cs
///  Test Name   : CHBAR_0705-presenting-programme-information-in-main-manu-and-channel-bar
///  TEST ID     : 23649 taken from hp qc
///  JIRA ID     : FC-562
///  QC Version  : 1
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar C
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("CHBAR_0705")]
public class CHBAR_0705 : _Test
{
  
    [ThreadStatic]
    static _Platform CL;
   
    static Service videoService1;
    static Service videoService2;
    static EnumChannelBarTimeout maxChannelBarTimeOutVal;
    static string ChannelList ="";
    static string delayStateTransition = "";
    static EnumChannelBarTimeout defaultChannelBarTimeOutVal;
    static string thumbnailDefault = "";
  
    //Constants used in the test
    private static class Constants
    {
      
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get channel fron content xml and set SetBannerDisplayTime.");
        this.AddStep(new Step1(), "Step 1: Tune to service s1 without thumbnail and check for default thumbnail on the channel bar .");
        this.AddStep(new Step2(), "Step 2: Tune to service s2 with thumbnail, Launch the channel bar and move the focus on next event and get the chbar informations");
       
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
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get List of Channel from TEST ini File
            ChannelList = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channelNumberList");
            if (string.IsNullOrEmpty(ChannelList))
            {
                FailStep(CL, res, "Unable to fetch the ChannelList from test ini file");
            }

            string[] ChannelListNumber = ChannelList.Split(',');

            videoService1 = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[0].Trim(), "ParentalRating=High");
            if (videoService1 == null)
            {
                FailStep(CL, "Failed to fetch videoService1 from Content.xml");
            }
            LogCommentInfo(CL, "Video Service1 fetched from content.xml is: " + videoService1.LCN);

            videoService2 = CL.EA.GetServiceFromContentXML("LCN=" + ChannelListNumber[1].Trim(), "ParentalRating=High");
            if (videoService2 == null)
            {
                FailStep(CL, "Failed to fetch videoService2 from Content.xml");
            }
        
            string bannerTimeout = "";

            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");
            if (string.IsNullOrEmpty(bannerTimeout))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, MAX fetched from Project.ini is null or empty", false);
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out maxChannelBarTimeOutVal);
            LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> MAX = " + maxChannelBarTimeOutVal);


            bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
            if (string.IsNullOrEmpty(bannerTimeout))
            {
                FailStep(CL, "CHANNEL_BAR_TIMEOUT, DEFAULT fetched from Project.ini is null or empty", false);
            }
            Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out defaultChannelBarTimeOutVal);
            LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> DEFAULT = " + defaultChannelBarTimeOutVal);

            delayStateTransition = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DELAY_STATE_TRANSITION");

            //Check Default Thumbnail for the Event which have no EIT
            thumbnailDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");

            //Change Timeout Duration in Channel Bar Timeout settings
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Banner display timeout to 15");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {

        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Tune to the service s1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService1.LCN);
            }
          
            //Navigate to channel bar.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            //waiting for state transition
            res = CL.IEX.Wait(Convert.ToDouble(delayStateTransition));
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to wait for state transition");
            }

            string obtainedValue = "";

            //Check Next Event Thumbnail in Channel Bar 
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            if (!obtainedValue.Equals(thumbnailDefault))
            {
                FailStep(CL, res, "Failed to verified that this event has Default thumbnail");
            }

            LogCommentInfo(CL, "Obtained thumbnail Name is: " + obtainedValue);

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {

        public override void Execute()
        {
            StartStep();

            //Tune to the service s2 which si having thumbnail and synopsis available.
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to videoService: " + videoService2.LCN);
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Navigate to channel bar and move the focus on Next event.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate Channel Bar");
            }

            //Select the next (channel bar) event
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //waiting for state transition
            res = CL.IEX.Wait(Convert.ToDouble(delayStateTransition));
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to wait for state transition");
            }

            string obtainedValue = "";

            //Check Next Event Thumbnail in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained thumbnail Name is: " + obtainedValue);

            //Check Next Event synopsis in Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            LogCommentInfo(CL, "Obtained synopsis Value is: " + obtainedValue);

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Set Channel Bar Time Out to Default 
        res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to change Banner Display Time to:" + defaultChannelBarTimeOutVal);
        }
    }
    #endregion
}