/// <summary>
///  Script Name : CHBAR_2388_RealTitleForAdultContent.cs
///  Test Name   : EPG-2388-Channel Bar-Access Real Title For Adult Content In Live
///  TEST ID     : 64268
///  JIRA ID     : FC-473
///  QC Version  : 1
///  Variations from QC: None
/// -----------------------------------------------
///  Modified by : Avinob Aich
///  Modified Date: 18/07/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("CHBAR_2388")]
public class CHBAR_2388 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service adultChannel;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Launch Channel Bar on Adult Service and check if alternate title is displayed";
    private const string STEP2_DESCRIPTION = "Step 2: Enter PIN to unlock the Service and check if real title is displayed";

    private static class Constants
    {
        public const double timeToPressKey = -1;
        public const string expectedPinState = "INSERT PIN UNLOCK CHANNEL";
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            string standByWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");

            //Get Values From xml File
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "");
            if (adultChannel.Equals(null))
            {
                FailStep(CL, "No Channels found having Parental Rating as High");
            }

            //Entering Stanby to ensure the channel is locked.
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to Enter Standby. Cannot ensure if the Adult Channel:" + adultChannel.LCN + "is locked");
            }
            else
            {
                //min time to stay on Stand By
                LogComment(CL, "Waiting for " + standByWait + " seconds in standby");
                CL.IEX.Wait(double.Parse(standByWait));
                //exiting from StandBy
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit from StandBy");
                }
            }
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            string obtainedAdultEventName = "";
            string expectedAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");

            //Tune to a Adult Service
            res = CL.EA.TuneToChannel(adultChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel: " + adultChannel.LCN);
            }

            //Clearing EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Launch Channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR ON LOCKED SERVICE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Channel Bar at channel: " + adultChannel.LCN);
            }

            //Get event name from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedAdultEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from Channel Bar");
            }

            LogComment(CL, "Obtained Adult Channel Event Name: " + obtainedAdultEventName);
            LogComment(CL, "Expected Adult Channel Event Name: " + expectedAdultEventName);

            //Checking if event name is alternate title
            if (obtainedAdultEventName != expectedAdultEventName)
            {
                FailStep(CL, "Event Name of Adult Event is not Alternate title");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string obtainedRealTitle = "";
            string pinState = "";
            string timeStamp = "";
            string nextStateAfterUnlock = "";

            string waitTimePinState = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DELAY_STATE_TRANSITION");
            string alternateAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");

            //Pressing Select to enter PIN
            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Select' on Locked Channel");
            }

            CL.IEX.Wait(double.Parse(waitTimePinState));

            //Get  Current State for PIN entry
            res = CL.IEX.MilestonesEPG.GetActiveState(out pinState);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Current Active State after pressing 'SELECT'");
            }

            //checking for valid state
            if (pinState != Constants.expectedPinState)
            {
                FailStep(CL, "Failed to Verify the State for Entering PIN");
            }
            //Clearing EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            nextStateAfterUnlock = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "STATE_AFTER_UNLOCK_CHANNEL");
            if (String.IsNullOrEmpty(nextStateAfterUnlock))
            {
                nextStateAfterUnlock = "CHANNEL BAR";
            }

             //Enter PIN to unlock the service
            res = CL.EA.EnterDeafultPIN(nextStateAfterUnlock);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter default pin to unlock Parental Rating Channel: " + adultChannel.LCN);
            }
			
			res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to Channel Bar");
            }
			
			//Waiting for 4 secs for the EPG to refresh as it will take some time to load the EIT
			res = CL.IEX.Wait(seconds: 4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to few seconds after Navigating");
            }

            //Get Event Name from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedRealTitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name from channel bar");
            }
            LogComment(CL, "Obtained Real Title after unlocking: " + obtainedRealTitle);

            //checking if realtitle is displayed
            if (obtainedRealTitle.Equals(alternateAdultEventName))
            {
                FailStep(CL, "Alternate Title is displayed after entering PIN");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}