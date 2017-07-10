using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-0250-PC-Locked_Channel
public class LightSanity_250 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string LockedChannel;
    static string UnlockedChannel;
    static string LockedName = "";
    static bool isCHLocked = false;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-0250-PC-Locked_Channel
        //Based on QualityCenter test version 6.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Verify Video is Present After Locking Current Channel");
        this.AddStep(new Step2(), "Step 2: Verify Video is Blocked After Re-Tune to The Locked Channel");
        this.AddStep(new Step3(), "Step 3: Record Current Locked Channel Event and Play it");

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
            LockedChannel = CL.EA.GetValue("FTA_1st_Mux_1");
            CL.IEX.LogComment("Retrieved Value From ini File: ChannelToLock = " + LockedChannel);

            UnlockedChannel = CL.EA.GetValue("FTA_1st_Mux_2");
            CL.IEX.LogComment("Retrieved Value From ini File: UnlockedChannel = " + UnlockedChannel);

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

            //Tune to a service, then lock it and verify that PIN Code is not asked for
            //Enter a Valid PIN Code
            //Verify PIN Screen is Removed and Video is Resumed
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, LockedChannel);
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

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check if Video is Present After Locking The Channel");
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

            //Tune to another live Service and then tune back to the locked channel           
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, UnlockedChannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            res = CL.EA.TuneToLockedChannel(LockedChannel, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to ReTune to The Locked Channel");
            }

        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

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

            //Schedule an Event-based Recording on Current PC Event, and Wait for Recording to Finish
            res = CL.EA.PVR.RecordCurrentEventFromBanner("Locked_Event", 1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Locked Event");
            }

            res = CL.EA.WaitUntilEventEnds("Locked_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Ends");
            }

            //Wait for GT to end + a few seconds to make sure PCAT is updated
            int Default_Guard_Time = 120;
            int PCAT_Wait = 60;
            int Test_Wait = Default_Guard_Time + PCAT_Wait;
            CL.IEX.Wait(Test_Wait);

            //Playback the Already Recorded Locked Event from Archive. Verify PIN is being asked for. Play to EOF
            res = CL.EA.PVR.PlaybackRecFromArchive_LockedEvent("Locked_Event", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The Locked Event Recording to EOF");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        LogComment(CL, "Enter PostExecute");

        if (isCHLocked == true)
        {
            IEXGateway._IEXResult res;

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