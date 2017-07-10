using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class LightSanity_240 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration 
    //Channels used by the test
    static string All_PC_Events_ch;
    static string Alternating_PC_and_Non_PC_Events;
    static string FTA_1st_Mux_1;
    static string LockedName = "";
    static bool isCHLocked = false;

    //Shared parameters between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:
        //Tunning to a Service with an Ongoing PC Event, Recording such an Event and Playing it.
        //Pre-conditions: 
        //The PC Age Rating is set to 6.
        //Service is having PC Event With rating >= 6 (E1 on S1 and E2 on S2)
        //Based on QualityCenter test version 4.
        //Variations from QualityCenter: Not checking Audio at all, including during PIN screen.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Set Parental Control Age Limit");
        this.AddStep(new Step1(), "Step 1: Tune to a Channel with an Ongoing PC Event");
        this.AddStep(new Step2(), "Step 2: Tune to another Channel with an Ongoing PC Event");
        this.AddStep(new Step3(), "Step 3: Record Current PC Event");
        this.AddStep(new Step4(), "Step 4: Playback the Recording");

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
            All_PC_Events_ch = CL.EA.GetValue("All_PC_Events_ch");
            Alternating_PC_and_Non_PC_Events = CL.EA.GetValue("Alternating_PC_and_Non_PC_Events");
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Set the PC Age Rating to 6
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge._6);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit");
            }


            //Tune channel which has to be locked.
            //Lock the channel
            //Zap out another channel
            //Again zap the previous channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, All_PC_Events_ch);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            //get channel name            
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out LockedName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Name");
            }

            //lock the channel
            res = CL.EA.STBSettings.SetLockChannel(LockedName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Lock Channel");
            }

            isCHLocked = true;

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
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

            //Tune to a service with an ongoing PC Event E1. Verify PIN Code is asked for
            //Enter a Valid PIN Code
            //Verify PIN Screen is Removed and Video is Resumed
            res = CL.EA.TuneToLockedChannel(Alternating_PC_and_Non_PC_Events, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a PC Event on Channel");
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

            res = CL.EA.TuneToLockedChannel(All_PC_Events_ch, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a PC Event on Another Channel and Enter PIN");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Schedule an Event-based Recording on Current PC Event, and Wait for Recording to Finish
            res = CL.EA.PVR.RecordCurrentEventFromBanner("PC_Event", 1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current PC Event");
            }

            //Leave S1, so the rest of E1 is not in the RB
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            res = CL.EA.WaitUntilEventEnds("PC_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Ends");
            }

            //Wait for GT to end + a few seconds to make sure PCAT is updated
            double Default_Guard_Time = 120;
            double PCAT_Wait = 160;
            double Test_Wait = Default_Guard_Time + PCAT_Wait;
            CL.IEX.Wait(Test_Wait);

            //Verification to recorded duration, including EGT of 2 minutes
            res = CL.EA.VerifyEventDuration("PC_Event", 180, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Recording Duration is Long Enough");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            int sec2play = 30;
            StartStep();

            //Playback the Already Recorded PC Event from Archive. Verify PIN is being asked for. Play to EOF
            res = CL.EA.PVR.PlaybackRecFromArchive_LockedEvent("PC_Event", sec2play, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The PC Event Recording to EOF");
            }

            CL.IEX.Wait(sec2play);

            PassStep();

        }
    }
    #endregion
    #endregion
        
    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        LogComment(CL, "Enter PostExecute");

        res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
        if (!res.CommandSucceeded)
        {
            LogComment(CL, "Failed to Set Parental Control Age Limit to UNLOCK_ALL");
        } 
        
        if (isCHLocked == true)
        {
            //Unlock the channel
            res = CL.EA.STBSettings.SetUnLockChannel(LockedName);
            if (!res.CommandSucceeded)
            {
                LogComment(CL, "Failed to UnLock Channel");
            }
        }

        LogComment(CL, "Exit PostExecute");

    }
    #endregion
}

