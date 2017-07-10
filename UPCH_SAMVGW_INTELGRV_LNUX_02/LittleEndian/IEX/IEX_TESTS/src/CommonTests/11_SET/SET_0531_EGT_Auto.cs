/// <summary>
///  Script Name : SET_0531_EGT_Auto.cs
///  Test Name   : SET-0531-REC EGT-Auto
///  TEST ID     :
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class SET_0531 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Short_SD_2;

    //Shared members between steps
    private static int EndGuardTime = 5;
    private static int TimeLeft = 0;
    private static string EndGuardTimeName = "AUTOMATIC";

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: set start Guard Time to  NONE & End Guard Time to " + EndGuardTimeName);
        this.AddStep(new Step2(), "Step 2: recored current event from banner & Verify Event Duration with  End Guard Time");
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
			
			EndGuardTime = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT"));

            //Get Values From ini File
            Short_SD_2 = CL.EA.GetValue("Short_SD_2");

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
                FailStep(CL, res, "Failed to Navigate to MAIN MENU/TOOLBOX/SETTINGS/RECORDINGS &amp; REMINDERS/RECORDINGS/EXTRA TIME AFTER PROGRAMME");
            }

            res = CL.IEX.MilestonesEPG.Navigate(EndGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to Navigate to " + EndGuardTimeName);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to Navigate to MAIN MENU/TOOLBOX/SETTINGS/RECORDINGS &amp; REMINDERS/RECORDINGS/EXTRA TIME BEFORE PROGRAMME");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel ");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("SD_Event", 3, false, false);
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

            //adding 120 secound  to insure the
            CL.IEX.Wait(TimeLeft + EndGuardTime * 60 + 120);

            res = CL.EA.VerifyEventDuration("SD_Event", TimeLeft + EndGuardTime * 60, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event Duration is Long Enough ");
            }

            res = CL.EA.VerifyEventDuration("SD_Event", TimeLeft + EndGuardTime * 60 + 110, false);
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

            res = CL.EA.PVR.PlaybackRecFromArchive("SD_Event", TimeLeft + EndGuardTime * 60, true, false);
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