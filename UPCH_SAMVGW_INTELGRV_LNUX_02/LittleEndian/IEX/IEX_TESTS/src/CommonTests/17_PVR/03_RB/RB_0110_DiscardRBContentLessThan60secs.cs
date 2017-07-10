/// <summary>
///  Script Name : RB-0110_DiscardRBContentLessThan60secs.cs
///  Test Name   : RB-0110-Discard RB content of less then 60 seconds
///  TEST ID     : 16022
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class RB_0110 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static Service AVService1;
    private static Service AVService2;
    private static string evtInfo1;
    private static string evtInfo2_1;
    private static string evtInfo2_2;
    private static string evtInfoOnPlb;
    private static string Rewind;
    private static double rewindSpeed;
    private static int evtTimeLeft = 0;

    private static class Constants
    {
        public const int timeInPlayback = 15;
        public const int timeToWaitinPause = 15;
        public const int timeInStby = 10;
        public const int waitInLive = 120;
        public const int timeToReachLive = 1;
        public const int waitInRB = 20;
        public const int minTimeReq = 240;

    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        // change the description in step 2
        this.AddStep(new Step1(), "Step 1: Tune to Channel wait until current event ends , Clean RB");
        this.AddStep(new Step2(), "Step 2: Tune to a channel,wait  for less than 60 sens and tune to another channel");
        this.AddStep(new Step3(), "Step 3: Rewind to BOF and Play The RB in Timeshift Mode");
        //this.AddStep(new Step4(), "Step 4: Tune to Channel wait until current event ends , Clean RB");
        //this.AddStep(new Step5(), "Step 5: Tune to a channel and pause on live ,wait  for less than 60 sens and tune to another channel");
        //this.AddStep(new Step6(), "Step 6: Rewind to BOF and Play The RB in Timeshift Mode");


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

            AVService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High");
            if (AVService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            AVService2 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + AVService1.LCN);

            if (AVService2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            Rewind = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            rewindSpeed = double.Parse(Rewind);

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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService1.LCN);
            }

            CL.EA.GetCurrentEventLeftTime(ref evtTimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }

            if (evtTimeLeft < Constants.minTimeReq)
            {
                CL.IEX.Wait(evtTimeLeft);
            }

            res = CL.EA.FlushRB();

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to standy and wakeup the STB");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService1.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            //add reason and move 15 to const
            CL.IEX.Wait(15);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService2.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo2_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo2_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
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
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to return to live viewing");
            }
            res = CL.EA.PVR.SetTrickModeSpeed("RB", rewindSpeed, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            //y? Handle dismissal of trickmode in better way
            CL.IEX.Wait(Constants.waitInRB);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }


            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfoOnPlb);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            //Consider evtInfo2 transition

            if (evtInfo1.Equals(evtInfoOnPlb) && (!evtInfo2_1.Equals(evtInfoOnPlb) || !evtInfo2_2.Equals(evtInfoOnPlb)))
            {
                FailStep(CL, "Event information doesnt match on RB playback");
            }

            PassStep();
        }
    }

    #endregion Step3


    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService1.LCN);
            }

            CL.EA.GetCurrentEventLeftTime(ref evtTimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }

            //waiting for the event to end if it is less the minimum time required to make sure no event transition is happening while
            // collecting the event info details

            if (evtTimeLeft < Constants.minTimeReq)
            {
                CL.IEX.Wait(evtTimeLeft);
            }

            res = CL.EA.FlushRB();

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to standy and wakeup the STB");
            }

            PassStep();
        }
    }

    #endregion Step4

    #region Step5

    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            //0 - move
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            //y-?
            CL.IEX.Wait(15);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService2.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo2_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo2_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            PassStep();
        }
    }

    #endregion Step5

    #region Step6

    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }

            //-30 read from project.ini
            res = CL.EA.PVR.SetTrickModeSpeed("RB", rewindSpeed, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            //remove the wait and handle in better way
            CL.IEX.Wait(Constants.waitInRB);
			CL.IEX.Wait(10);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfoOnPlb);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }


            if (evtInfo1.Equals(evtInfoOnPlb) && (!evtInfo2_1.Equals(evtInfoOnPlb) || !evtInfo2_2.Equals(evtInfoOnPlb)))
            {
                FailStep(CL, "Event information doesnt match on RB playback");
            }

            PassStep();
        }
    }

    #endregion Step6

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {


    }

    #endregion PostExecute
}