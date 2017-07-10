using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0905 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Medium_SD_1;
    static string Medium_HD_1;
    private static int endGuardTimeInt = 0;
    private static int startGuardTimeInt = 0;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precontitions: Get Values from ini File & SyncStream");
        this.AddStep(new Step1(), "Step 1: Start & Stop a Recording ");
        this.AddStep(new Step2(), "Step 2: Restart the Recording");
        this.AddStep(new Step3(), "Step 3: Wait for Recording to Finish & Verify Exception Reason");
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
            // Get Values from ini File
            Medium_SD_1 = CL.EA.GetValue("Medium_SD_1");

            Medium_HD_1 = CL.EA.GetValue("Medium_HD_1");

            string sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");

            string egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");

            LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);

            LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);


            LogComment(CL, "Setting the Start Guard Time to " + sgtFriendlyName);
            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }


            LogComment(CL, "Setting the End Guard Time to " + egtFriendlyName);
            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
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

            // Schedule a Future Recording. choose Event that is long enough to start, stop and restart recording without event end.
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel ");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("Event1", 1, 2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event from Banner");
            }
            // Wait For recording to start + a little more (>10 seconds);
            res = CL.EA.WaitUntilEventStarts("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Starts");
            }

            int Test_Wait = 30;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.PCAT.VerifyEventIsRecording("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event is Recording in PCAT");
            }

            //Stop the Recording
            res = CL.EA.PVR.StopRecordingFromArchive("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording From Archive");
            }

            //Move to another channel, so the rest of the event is not part of the RB);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            // Wait to be sure PCAT is updated
            int PCAT_Wait = 15;
            CL.IEX.Wait(PCAT_Wait);

            // Verify status and exception reason
            res = CL.EA.PCAT.VerifyEventPartialStatus("Event1", "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            res = CL.EA.PCAT.VerifyEventExceptionReason("Event1", "VIEWER_STOPPED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event's Exception Reason");
            }

            // Check duration to have something to compare against in step 3);
            res = CL.EA.VerifyEventDuration("Event1", 400, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item is Not Too Long ");
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

            //Restart the Recording
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune Back to Channel");
            }
            // We can't book the event again with the same key name 'Event1', so it will be booked with a different key name. In PCAT they will obviously be in the same line
            res = CL.EA.PVR.RecordCurrentEventFromBanner("Rebook", -1, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rebook Current Event from Banner");
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


            //Wait for recording to finish
            res = CL.EA.WaitUntilEventEnds("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Ends");
            }
            // Wait for GT to end

            CL.IEX.Wait(endGuardTimeInt*60);

            //Verify Event Status
            res = CL.EA.PCAT.VerifyEventPartialStatus("Event1", "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify in PCAT the Event is Partially Recorded ");
            }
            // Verify Exception Reason
            res = CL.EA.PCAT.VerifyEventExceptionReason("Event1", "VIEWER_RESTARTED_AFTER_RECORDING_STOPPED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event's Exception Reason ");
            }
            // Verify event is longer than in step 1 (appending performed);
            res = CL.EA.VerifyEventDuration("Event1", 200, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item is Longer than Before");
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
            StartStep();

            //Playback the recording
            //'Play the append point with normal speed
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The Event");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing (with Video); after Playback of Appended Recording");
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