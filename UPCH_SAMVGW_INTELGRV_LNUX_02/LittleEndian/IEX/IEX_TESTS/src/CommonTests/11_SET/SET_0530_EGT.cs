/// <summary>
/// Script Name : SET_0530_EGT.cs
/// Test Name   : SET-0530-REC EGT-0 Min
/// Test Name   : SET-0530-REC EGT-1 Min
/// Test Name   : SET-0530-REC EGT-2 Min
/// Test Name   : SET-0530-REC EGT-3 Min
/// Test Name   : SET-0530-REC EGT-5 Min
/// Test Name   : SET-0530-REC EGT-10 Min
/// Test Name   : SET-0530-REC EGT-15 Min
/// Test Name   : SET-0530-REC EGT-30 Min
/// Test Name   : SET-0530-REC EGT-60 Min
/// Test Name   : SET-0530-REC EGT-120 Min
/// Test Name   : SET-0530-REC EGT-Auto Min
/// TEST ID     :
/// QC Version  :
/// -----------------------------------------------
/// Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class SET_0530 : _Test
{
    [ThreadStatic]
    private static _Platform GW;

    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static string defaultGT;
    private static string serviceType;
    private static string serviceToBeRecorded; //The service to be recorded
    private static int EndGuardTime;
    private static int TimeLeft = 0;
    private static string EndGuardTimeName;
    private static string EndGuardTimeString;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Set Start Guard Time to NONE & End Guard Time to " + EndGuardTimeName);
        this.AddStep(new Step2(), "Step 2: Recored Current Event From Banner & Verify Event Duration with End Guard Time");
        this.AddStep(new Step3(), "Step 3: Playback Till End of File");

        //Get Platforms
        GW = GetGateway();
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
            serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");
            serviceToBeRecorded = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", serviceType);
            EndGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_NAME");
            EndGuardTimeString = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");
            EndGuardTime = Convert.ToInt32(EndGuardTimeString);

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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to EXTRA TIME AFTER PROGRAMME Screen");
            }

            res = CL.IEX.MilestonesEPG.Navigate(EndGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + EndGuardTimeName + " Option on EXTRA TIME AFTER PROGRAMME Screen");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to EXTRA TIME BEFORE PROGRAMME Screen");
            }

            res = CL.IEX.MilestonesEPG.Navigate("NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to NONE Option on EXTRA TIME BEFORE PROGRAMME Screen");
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

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(false, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
            }

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event", 3, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book the Event From Banner");
            }

            res = CL.EA.GetCurrentEventLeftTime(ref TimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }

            CL.IEX.LogComment("Time Left to Current Event = " + TimeLeft);

            CL.EA.ReturnToLiveViewing();
            CL.IEX.Wait(TimeLeft + EndGuardTime * 60 + 120);

            res = CL.EA.VerifyEventDuration("Event", TimeLeft + EndGuardTime * 60, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Long Enough");
            }

            res = CL.EA.VerifyEventDuration("Event", TimeLeft + EndGuardTime * 60 + 110, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Not Too Long");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.PlaybackRecFromArchive("Event", TimeLeft + EndGuardTime * 60, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Event From Archive");
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        res = CL.EA.StillAlive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Confirm StillAlive, Have to Mount Again");

            res = GW.EA.MountGw(EnumMountAs.NOFORMAT, 3);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(GW, "Failed to Mount Gateway");
                LogCommentFailure(CL, "Failed to Mount Gateway");
            }

            res = CL.EA.MountClient(EnumMountAs.NOFORMAT, 3);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Mount Client: " + res.FailureReason);
            }
            CL.IEX.Wait(60);
        }

        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Navigate to EXTRA TIME BEFORE PROGRAMME");
        }

        defaultGT = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        res = CL.IEX.MilestonesEPG.Navigate(defaultGT);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Navigate to Default " + defaultGT);
        }

        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Navigate to EXTRA TIME AFTER PROGRAMME");
        }

        defaultGT = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        res = CL.IEX.MilestonesEPG.Navigate(defaultGT);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Navigate to NONE " + defaultGT);
        }
    }

    #endregion PostExecute
}