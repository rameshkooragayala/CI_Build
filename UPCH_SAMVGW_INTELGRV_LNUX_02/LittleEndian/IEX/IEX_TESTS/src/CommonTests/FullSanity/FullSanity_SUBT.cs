using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-FullSanity_SUBT subtitles change track
public class FullSanity_SUBT : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    private static string service;
    private static String nextSubtitle = "";
    private static string defaultSubtitle = "";
    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:FullSanity-1701-SUBT-DVB subtitles change track
        //Pre-conditions: Multiple DVB subtitles exist in the stream in channel S1.
        //Based on QualityCenter test version 4.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to multiple subtitle service");
        this.AddStep(new Step2(), "Step 2: Change Subtitles on Live");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File and service from xml file.
            
           // string serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");
           //string serviceType = "ParentalRating=High;NoOfSubtitleLanguages=0,1;SubtitleType=Dvb";
            service = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LCN");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service);
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

            //Tune to a multiple subtitles channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service);
            }

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

            //Navigate to Action Menu subtitle
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to subtitles on Action Menu");
            }
            
            //Get the default lanuage.

           
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Subtitle Language of Current Event");
            }
            else
            {
                LogCommentInfo(CL, "Defaultsubtitle is:" + defaultSubtitle);
            }
            //Change subtitles from default.

            nextSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_SUBTITLE");
            LogCommentInfo(CL, "Next Subtitle feteched from project ini is : " + nextSubtitle);

            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextSubtitle, out timeStamp, 5000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next Subtitle in the list");
            }

            //Get destination Subtitle
            string SubtitleChangedTo = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out SubtitleChangedTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Subtitle" + SubtitleChangedTo);
            }
            else
            {
                LogCommentInfo(CL, "Subtitle is changed to:" + SubtitleChangedTo);
            }

            //Select the different subtitle
            res = CL.IEX.MilestonesEPG.Navigate(SubtitleChangedTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select the Subtitle: " + SubtitleChangedTo);
            }

            //Navigate back to Action menu subtitle

            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to subtitles on Action Menu");
            }
            
            //verify the subtitle is changed
            if (SubtitleChangedTo.Equals("defaultSubtitle"))
            {
                FailStep(CL, "Failed to change the subtitle from default to another)");
            }
            else
            {
                LogCommentInfo(CL,"Subtitle Changed successfully on action menu.");
            }


            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
        if (!res.CommandSucceeded)
        {
           CL.IEX.FailStep("Failed to Navigate to subtitles on Action Menu");
        }

        res = CL.IEX.MilestonesEPG.Navigate(defaultSubtitle);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to set back to default subtitle: " + defaultSubtitle);
        }

    }
    #endregion
}