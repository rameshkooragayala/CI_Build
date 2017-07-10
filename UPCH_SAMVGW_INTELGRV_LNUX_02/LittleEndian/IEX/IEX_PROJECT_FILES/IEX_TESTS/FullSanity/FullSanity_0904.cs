using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class FullSanity_0904 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static string Short_HD_1;
    static string Medium_SD_1;

    //Shared members between steps
    static int eventDuration = 1;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Preconditions: Get Values from ini File, SyncStream and Have On-Going Recording");
        this.AddStep(new Step1(), "Step 1: Access the Recordings List & Stop the Recording");
        this.AddStep(new Step2(), "Step 2: Playback the Recording (twice); from Start to End of File.");

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
            Short_HD_1 = CL.EA.GetValue("Short_HD_1");
            Medium_SD_1 = CL.EA.GetValue("Medium_SD_1");

            // Precondition: Have one Recording On-going
            //Tune to a Channel and Book Recording from Banner
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel ");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("Event1", 1, 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Next Event from Banner ");
            }
            res = CL.EA.WaitUntilEventStarts("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait for Event to Start  ");
            }
            res = CL.EA.GetCurrentEventLeftTime(ref eventDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Event Left Time To Use On Playbak");
            }

            // Wait to be sure event recording has started (EPG time has accuracy of minutes, not seconds);, and have measurable length
            int Test_wait = 60;
            CL.IEX.Wait(Test_wait);

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

            // Access the Recording List
            // From the Recording List Stop the Recording

            int timeStop = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeStop);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Event Left Time To Use On Playbak");
            }

            res = CL.EA.PVR.StopRecordingFromArchive("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording from Archive ");
            }


            eventDuration = eventDuration - timeStop;

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
            //Playback the recording from start to end of file.
            //Repeat twice
            CL.IEX.LogComment("plaing envent duretion " + eventDuration + "");

            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", eventDuration, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The Event");
            }
            //Tune to a channel other than the one recorded, and playback again
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", eventDuration, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event a Second Time");
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live after 2 Playbacks of Event");
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