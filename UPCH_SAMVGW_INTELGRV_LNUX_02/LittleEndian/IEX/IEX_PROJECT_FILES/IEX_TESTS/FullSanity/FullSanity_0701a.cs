using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0701a-LIB-BOOK-Booking & Recording update List
public class FullSanity_0701a : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string EventBasedRecordingCannel;
    static string MR_Channel;
    static string MR_Channel_lName;

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
        this.AddStep(new Step1(), "Step 1: Verify Recordings");
        //        this.AddStep(new Step2(), "Step 2: verfy On Going Recording is On Archive And Not On Planer");


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
            MR_Channel = CL.EA.GetValue("Short_SD_1");
            MR_Channel_lName = CL.EA.GetValue("Name_Short_SD_1");

            EventBasedRecordingCannel = CL.EA.GetValue("Medium_SD_1");


            res = CL.EA.TuneToChannel(EventBasedRecordingCannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + EventBasedRecordingCannel);
            }

            //res = CL.EA.PVR.BookFutureEventFromBanner("EnventBase",1,1,false);
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EnventBase", 5, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book the EnventBase From banner");
            }


            // Wait for the Recording to Finish
            //res = CL.EA.WaitUntilEventEnds("EnventBase");
            res = CL.IEX.Wait(120);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait till EnventBase end Recording");
            }
            res = CL.EA.PVR.StopRecordingFromBanner("EnventBase");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "stop rec failed");
            }
            CL.IEX.Wait(30); //+ 120

            //Record event (verify in pcat)
            res = CL.EA.TuneToChannel(MR_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + MR_Channel);
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TimeBased", MR_Channel_lName, -1, 3, 3, EnumFrequency.ONE_TIME, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book the HD_Event From banner");
            }

            //            res = CL.EA.WaitUntilEventEnds("TimeBased");
            res = CL.IEX.Wait(210);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait till TimeBased end Recording");
            }

            /*res = CL.EA.PVR.StopRecordingFromBanner("TimeBased");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "stop rec failed");
            }*/
            //Pcat update + Gurd time 
            CL.IEX.Wait(30); //+ 120

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

            res = CL.EA.PVR.VerifyEventInArchive("EnventBase");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verfiy EnventBase on Archive ");
            }

            res = CL.EA.PVR.VerifyEventInArchive("TimeBased");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verfiy TimeBased on Archive ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
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

            //if (timeToEventEnd_sec < 420)
            if (timeToEventEnd_sec < 180)
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