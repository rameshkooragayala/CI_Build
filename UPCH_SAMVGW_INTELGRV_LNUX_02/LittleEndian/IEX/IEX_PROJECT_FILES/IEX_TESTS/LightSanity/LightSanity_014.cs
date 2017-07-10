using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-014-BOOK Booking current event from Guide and Banner
public class LightSanity_014 : _Test
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

        //Brief Description: LightSanity-014-BOOK Booking current event from Guide and Banner
        //Booking of current event should be successfull.
        //Pre-conditions: None.
        //Based on QualityCenter test version .
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book Current Event From Guide");
        this.AddStep(new Step2(), "Step 2: Book Current Event From Banner");

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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            //Book on going event from Guide
            res = CL.EA.PVR.RecordCurrentEventFromGuide("EveRecFromGuide", Long_SD_1, 1, true, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Current Event From Guide");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            res = CL.EA.PVR.VerifyEventInPlanner("EveRecFromGuide", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            //Check enough time left for the event
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

            //Book on going event from Banner 
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecFromBanner", 1, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Current Event From Banner");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EveRecFromBanner", true, false);
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