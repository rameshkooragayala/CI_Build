using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-0170-SUBT-DVB subtitles change track
public class LightSanity_170 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Multiple_DVB_Subtitles_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-0170-SUBT-DVB subtitles change track
        //Pre-conditions: Multiple DVB subtitles exist in the stream in channel S1.
        //Based on QualityCenter test version 4.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Change Subtitles on Live");
        this.AddStep(new Step2(), "Step 2: Change Subtitles Default Settings");

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
            Multiple_DVB_Subtitles_1 = CL.EA.GetValue("Multiple_DVB_Subtitles_1");

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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Multiple_DVB_Subtitles_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 180)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            //Open subtitles on current Event
            res = CL.EA.SubtitlesLanguageChange(EnumLanguage.English);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Engilsh Subtitles");
            }

            //CL.IEX.Wait(15);

            //Change subtitles on current Event
            // res = CL.EA.SubtitlesLanguageChange(EnumLanguage.Dutch);
            // if (!res.CommandSucceeded)
            //{
            //     FailStep(CL,res, "Failed to Navigate to Dutch Subtitles");
            // }

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

            //Turn off current event's subtitles - temmporary step !
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE/ACTION BAR/A//V SETTINGS/SUBTITLES/OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Change Subtitles of Current Event to OFF");
            }

            //Tune back to the multiple subtitles channe
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing");
            }

            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 180)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            //Check that the highlighted subtitles is not OFF (usually it is English);
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE/ACTION BAR/A//V SETTINGS/SUBTITLES/");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Subtitles of Current Event");
            }

            string subLang = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out subLang);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Subtitle Language of Current Event");
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