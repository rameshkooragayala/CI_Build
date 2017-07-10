using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0701b-LIB-BOOK-Booking & Recording update List
public class FullSanity_0701b : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_2;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:FullSanity-0701-LIB-BOOK-Booking & Recording update List
        //Check that event changes from Booking list to recording list once it started recording.
        //Pre-conditions: Recording of events.
        //Based on QualityCenter test version 4.
        //Variations from QualityCenter:Non
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Have Recordings on Disk");
        this.AddStep(new Step1(), "Step 1: verfy On Going Recording is On Archive And Not On Planer");

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
            Short_SD_2 = CL.EA.GetValue("Short_SD_1");


            res = CL.EA.TuneToChannel(Short_SD_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Short_SD_2);
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
        //verify event not in archive 
        //- wait to record to start 
        //verify event not in planner 
        //verify event in archive

        public override void Execute()
        {

            StartStep();

            int timeToEventEnd_sec = 0;

            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get time left to current event");
            }

            if (timeToEventEnd_sec < 420)
            //if (timeToEventEnd_sec < 180)
            {
                CL.IEX.Wait(timeToEventEnd_sec + 60);

                res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get time left to current event on second time ");
                }
            }


            res = CL.EA.PVR.BookFutureEventFromBanner("EveBookFromBanner", 1, 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Banner");
            }

            CL.IEX.Wait(5);

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


            CL.IEX.Wait(timeToEventEnd_sec + 60);
            //Verify event NOT in planer 
            res = CL.EA.PVR.VerifyEventInPlanner("EveBookFromBanner", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Verify event in archive 
            res = CL.EA.PVR.VerifyEventInArchive("EveBookFromBanner", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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