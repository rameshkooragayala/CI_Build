using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-004-EPG-Banner
public class LightSanity_004 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_Channel;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-004-EPG-Banner
        //Launch the Channel Bar using remote control key and access the channel bar items from live viewing
        //Pre-conditions:
        //E1 is a current event in the service S1. 
        //E2 is a past event in S1 which is there in the review buffer.
        //E3 is a next event on service S1
        //Based on QualityCenter test version .
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to E1 on service S1, and launch the channel bar");
        this.AddStep(new Step2(), "Step 2: Tuned to service S1, browse the event of next service");
        this.AddStep(new Step3(), "Step 3: Access the actions related to the current programme");
        this.AddStep(new Step4(), "Step 4: Access the actions related to next event");

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

            //Get Values From ini File
            FTA_Channel = CL.EA.GetValue("FTA_Channel");


            //Better to run this test with new RB - after mount or standby
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            //Stay in Standby for a few seconds, to make sure RB was Deleted
            double timeInStandby = 15;
            CL.IEX.Wait(timeInStandby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            CL.IEX.Wait(timeInStandby);
            res = CL.EA.CheckForVideo(true, false, 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Paused");
            }

            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    /**
     * Check event name on channel bar, by comparing to action menu 
    **/
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //check for enough time left on the event
            int timeToEventEnd_sec = 0;

            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 240)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            //Navigate to channel bar
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE/CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Channel bar milestone gives next event and afterwards current event time, so need to wait to make sure 
            //not reading next event time by mistake
            CL.IEX.Wait(2);

            //Check program time in channel bar
            string eventTimeChannelBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out eventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Time From Channel Bar");
            }

            //Check that EPG time is between eventTimeChannelBar (Event Start & End times)
            res = CL.EA.PVR.VerifyEventSchedule(EnumEventOccures.Current, eventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: EPG Time isn't Between Start & End Times of Current Event");
            }

            //Check program name in channel bar
            string eventNameChannelBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventNameChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Name From Channel Bar");
            }

            if (eventNameChannelBar.Length == 0) //TODO: check no information available
            {
                FailStep(CL, res, "Failed: Event Name in Channel Bar is Empty");
            }

            //Select the next (channel bar) event for time compare
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar");
            }

            //Check next program time in channel bar
            string nextEventTimeChannelBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out nextEventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Next Time from Channel Bar");
            }

            //Check that nextEventTimeChannelBar is starts after current EPG time 
            res = CL.EA.PVR.VerifyEventSchedule(EnumEventOccures.Future, nextEventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Future Event Start Time isn't After Current EPG Time");
            }

            //Load the action menu
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE/ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to LIVE/ACTION BAR");
            }

            //Check program name in action menu
            string eventNameActionMenu = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventNameActionMenu);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Name From Action Menu");
            }

            //Compre the first 20 chars of current event names (event name in channel bar has limited length)
            if (!eventNameChannelBar.Substring(0, 20).Equals(eventNameActionMenu.Substring(0, 20)))
            {
                FailStep(CL, "Failed: Event Name is Different Between Channel Bar and Action Menu");
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

            //Surf to next channel from chanel bar (not tune to it)
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 1, EnumPredicted.NotPredictedWithoutPIP, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf on Channel Bar to Next Service)");
            }

            //Check program time in channel bar
            string eventTimeChannelBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out eventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Time From Channel Bar (For Next Service)");
            }

            //Select the next (channel bar next event) event for time compare
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select NEXT on Channel Bar (For Next Service)");
            }

            //Check next program time in channel bar
            string nextEventTimeChannelBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out nextEventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Event Time From Channel Bar (Next service)");
            }

            //Check that EPG time is between eventTimeChannelBar (Event Start & End times)
            res = CL.EA.PVR.VerifyEventSchedule(EnumEventOccures.Current, eventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: EPG Time isn't Between Start & End Times for Current Event (Next Service)");
            }

            //Check that nextEventTimeChannelBar is starts after current EPG time 
            res = CL.EA.PVR.VerifyEventSchedule(EnumEventOccures.Future, nextEventTimeChannelBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Future Event Start Time isn't After Current EPG Time (Next Service)");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Load the action bar
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE/ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU/LIVE/ACTION BAR");
            }

            //Check time in action bar
            string eventTimeActionlBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out eventTimeActionlBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Event Time From Action Bar");
            }

            //Check that EPG time is between eventTimeActionlBar (Event Start & End times)
            res = CL.EA.PVR.VerifyEventSchedule(EnumEventOccures.Current, eventTimeActionlBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: EPG Time isn't Between Start & End Times of Current Event");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Load action bar of next event through channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate action bar of next event through channel bar");
            }

            //Check time in action bar in order to verify acton bar of curent event 
            string nextEventTimeActionlBar = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out nextEventTimeActionlBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Event Time From Action Bar");
            }

            //Check that nextEventTimeChannelBar is starts after current EPG time 
            res = CL.EA.PVR.VerifyEventSchedule(EnumEventOccures.Future, nextEventTimeActionlBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Future Event Start Time isn't After Current EPG Time");
            }

            PassStep();

        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}