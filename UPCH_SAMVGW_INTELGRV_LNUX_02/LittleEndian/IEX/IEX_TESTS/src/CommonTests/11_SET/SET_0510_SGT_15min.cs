/// <summary>
///  Script Name : SET_0510_SGT_15min.cs
///  Test Name   : SET-0510-REC SGT-15 Min
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class SET_0510_15 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Short_SD_2;

    //Shared members between steps
    private static int eventtime = 300;
    private static int startGuardTime = 15;
    private static int TimeLeft = 0;
    private static int TimeLeftRecordedEvent = 0;
    private static string StartGuardTimeName = "15 min.";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: set start Guard Time to" + StartGuardTimeName + " & End Guard Time to NONE  ");
        this.AddStep(new Step2(), "Step 2: record future event from banner & Verify Event Duration with Start Guar dTime");
        this.AddStep(new Step3(), "Step 3: Playback Till EndOfFile");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            Short_SD_2 = CL.EA.GetValue("FTA_1st_Mux_4");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to Navigate to MAIN MENU/TOOLBOX/SETTINGS/RECORDINGS &amp; REMINDERS/RECORDINGS/EXTRA TIME BEFORE PROGRAMME");
            }

            res = CL.IEX.MilestonesEPG.Navigate(StartGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to Navigate to " + StartGuardTimeName);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU/TOOLBOX/SETTINGS/RECORDINGS &amp; REMINDERS/RECORDINGS/EXTRA TIME AFTER PROGRAMME");
            }

            res = CL.IEX.MilestonesEPG.Navigate("NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to NONE");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel ");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("SD_Event", 1, startGuardTime + 2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book the SD_Event From banner");
            }

            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get time left to current event");
            }

            CL.IEX.LogComment("TimeLeft =:" + TimeLeft);

            res = CL.EA.ReturnToLiveViewing();
            CL.IEX.Wait(TimeLeft);

            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeftRecordedEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get time left to current event");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (TimeLeftRecordedEvent > eventtime)
            {
                CL.IEX.Wait(eventtime);
                res = CL.EA.PVR.StopRecordingFromBanner("SD_Event");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
                }
            }
            else
            {
                eventtime = TimeLeftRecordedEvent;
            }

            res = CL.EA.VerifyEventDuration("SD_Event", eventtime + startGuardTime * 60, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Long Enough ");
            }

            res = CL.EA.VerifyEventDuration("SD_Event", eventtime + startGuardTime * 60 + 115, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Not Too Long");
            }
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.PlaybackRecFromArchive("SD_Event", eventtime + startGuardTime * 60, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback  SD_Event From Achive");
            }
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}