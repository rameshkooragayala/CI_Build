using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-013-BOOK-Booking future Event from Guide and Banner
public class LightSanity_013 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_1;
    static string Long_SD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-013-BOOK-Booking future Event from Guide and Banner
        //Testing sanity for booking and recording for event based.
        //Pre-conditions: None.
        //Based on QualityCenter test version .
        //Variations from QualityCenter: Book form Guide and Bunner were separated to two steps,
        //                               check recording list was inserted into relevant step (Guide,Bunner)

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book Future Event From Banner");
        this.AddStep(new Step2(), "Step 2: Book Future Event From Guide");
        this.AddStep(new Step3(), "Step 3: Playback Banner Event");

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
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");
            Long_SD_1 = CL.EA.GetValue("Long_SD_1");


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
    private class Step1 : _Step
    {
        //Book future event from banner 
        //verify event in planner 
        //verify event not in archiv 
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + Short_SD_1);
            }

            //Check for enough time left on the event
            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 420)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec + 60);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EveBookFromBanner");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Banner");
            }

            // CL.IEX.Wait(5);
            res = CL.EA.PVR.VerifyEventInPlanner("EveBookFromBanner", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Verify event NOT in archive 
            res = CL.EA.PVR.VerifyEventInArchive("EveBookFromBanner", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            /*  res = CL.EA.ReturnToLiveViewing();
              if (!res.CommandSucceeded)
              {
                  FailStep(CL,res, "Failed to Return To Live");
              }*/

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Book future event from Guide 
        //verify event in planner
        //verify event not in archive 
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + Long_SD_1);
            }

            res = CL.EA.PVR.BookFutureEventFromGuide("EveBookFromGuide", Long_SD_1, 2, 1, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide");
            }

            //CL.IEX.Wait(5);
            //res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU");

            res = CL.EA.PVR.VerifyEventInPlanner("EveBookFromGuide", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Verify event NOT in archive 
            res = CL.EA.PVR.VerifyEventInArchive("EveBookFromGuide", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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

            res = CL.EA.WaitUntilEventEnds("EveBookFromBanner");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Record From Banner Ends");
            }
            CL.IEX.Wait(120 + 30);

            res = CL.EA.PVR.PlaybackRecFromArchive("EveBookFromBanner", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Event");
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