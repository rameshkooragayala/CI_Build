/// <summary>
///  Script Name : GRID_2130_EITForAdultContent.cs
///  Test Name   : GRID-2130-Grid-Detailed-Program-Info-Future-Event-Adult-Session-Open
///  TEST ID     : 68110
///  JIRA ID     : FC-588
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 22.08.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("GRID_2130")]
public class GRID_2130 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps

    static Service adultChannel;

    static string alternateEventName;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Launch TV GUIDE & Surf to Locked Channel";
    private const string STEP2_DESCRIPTION = "Step 2: Unlock a Future Event of Parental Locked Channel from Guide and verify Info";
    private const string STEP3_DESCRIPTION = "Step 3: Press SELECT and verify Program Info";

    static class Constants
    {
        public const bool isMoveRight = true;
        public const int numberOfRightPresses = 2;
        public const double timeToPress = -1;
        public const int eventStartEndTimeLength = 2;
        public const string nextState = "TV GUIDE";
    }

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Value for alternate title for locked channel from Project.ini
            alternateEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");
            if (string.IsNullOrEmpty(alternateEventName))
            {
                FailStep(CL, "Failed to get Alternate Name for Locked Event from Project.ini", false);
            }
            //Get Channels from xml File
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasSynopsis=True;ParentalRating=High", "");

            if (adultChannel == null)
            {
                FailStep(CL, "Failed to fetched from Content.xml for the passed criterion");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + adultChannel.LCN);

            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Surf to adult channel in guide
            res = CL.EA.SurfToChannelInGuide(adultChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to adult channel: " + adultChannel.LCN + " in Guide");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            bool isPass = true;
            string obtainedEventName = "";
            string obtainedEventTime = "";
            string obtainedChannelName = "";
            string obtainedChannelLogo = "";
            string expectedStateForPinEntry = "INSERT PIN UNLOCK CHANNEL";

            //Focus on future event in locked service
            res = CL.EA.BrowseGuideFuture(Constants.isMoveRight, Constants.numberOfRightPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to move to future event in Guide");
            }

            //Pressing Select to get Pin PopUp State
            res = CL.IEX.MilestonesEPG.Navigate(expectedStateForPinEntry);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify Pin PopUp state on selecting from Guide");
            }

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to clear EPG Info");
            }

            //Enter default PIN to unlock the channel
            res = CL.EA.EnterDeafultPIN(Constants.nextState);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter PIN to unlock the Adult Channel: " + adultChannel.LCN + " from Guide and Verify the Next State as " + Constants.nextState);
            }

            //Get Channel Logo from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("channel_logo", out obtainedChannelLogo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Channel Logo from EPG", false);
                isPass = false;
            }
            //Checking if channel logo is empty
            else if (!string.IsNullOrEmpty(obtainedChannelLogo))
            {
                if (!obtainedChannelLogo.Equals(adultChannel.ChannelLogo))
                {
                    FailStep(CL, "Failed to verify if channel logo is as expected", false);
                    isPass = false;
                }
            }
            else
            {
                //Get Channel Name from EPG
                res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out obtainedChannelName);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Channel Name from " + Constants.nextState, false);
                    isPass = false;
                }
                //Checking if channel name is as expected
                else if (obtainedChannelName != adultChannel.Name)
                {
                    FailStep(CL, res, "Failed to verify the Channel Name after Unlocking from Guide", false);
                    isPass = false;
                }

            }

            //Get event time from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Time from " + Constants.nextState, false);
                isPass = false;
            }
            else
            {

                string startTime = "";
                string endTime = "";

                //Checking if StartTime is present
                CL.EA.UI.Utils.ParseEventTime(ref startTime, obtainedEventTime, true);

                if (string.IsNullOrEmpty(startTime))
                {
                    FailStep(CL, "Start Time is Empty in TV Guide", false);
                    isPass = false;
                }
                //Checking if EndTime is present
                CL.EA.UI.Utils.ParseEventTime(ref endTime, obtainedEventTime, false);
                if (string.IsNullOrEmpty(endTime))
                {
                    FailStep(CL, "End Time is Empty in TV Guide", false);
                    isPass = false;
                }
            }


            //Get event name from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from " + Constants.nextState, false);
                isPass = false;
            }
            else if (string.IsNullOrEmpty(obtainedEventName))
            {
                FailStep(CL, res, "Event Name is Empty", false);
                isPass = true;
            }
            //checking if the event name is still alternate title
            else if (obtainedEventName.Equals(alternateEventName))
            {
                FailStep(CL, "Failed to verify Event Name as Real Title in " + Constants.nextState, false);
                isPass = false;
            }

            if (!isPass)
            {
                FailStep(CL, "Failed to verify the requirement(s) in Guide. Details mentioned above");
            }
            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            bool isPass = true; ;
            string obtainedEventName = "";
            string obtainedEventTime = "";
            string obtainedSynopsis = "";

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Pressing Select on Guide to launch ACTION BAR
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Action Bar from Guide");
            }

            //Get Event Name from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from " + Constants.nextState, false);
                isPass = false;
            }
            else if (string.IsNullOrEmpty(obtainedEventName))
            {
                FailStep(CL, res, "Event Name is Empty in ACTION BAR", false);
                isPass = false;
            }
            //Checking if Event Name is still alternate Name
            else if (obtainedEventName.Equals(alternateEventName))
            {
                FailStep(CL, "Failed to verify Event Name as Real Title in " + Constants.nextState, false);
                isPass = false;
            }

            //Get Event Time from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Time from " + Constants.nextState, false);
                isPass = false;
            }
            else
            {

                string startTime = "";
                string endTime = "";

                //Checking if StartTime is present
                CL.EA.UI.Utils.ParseEventTime(ref startTime, obtainedEventTime, true);
                if (string.IsNullOrEmpty(startTime))
                {
                    FailStep(CL, "Start Time is Empty in TV Guide", false);
                    isPass = false;
                }
                //Checking if EndTime is present
                CL.EA.UI.Utils.ParseEventTime(ref endTime, obtainedEventTime, false);
                if (string.IsNullOrEmpty(endTime))
                {
                    FailStep(CL, "End Time is Empty in TV Guide", false);
                    isPass = false;
                }
            }


            //Get Synopsis from EPG
            res = CL.IEX.MilestonesEPG.GetEPGInfo("synopsis", out obtainedSynopsis);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to get Synopsis from ACTION BAR", false);
                isPass = false;
            }
            //Checking if Synopsis is empty
            else if (string.IsNullOrEmpty(obtainedSynopsis))
            {
                FailStep(CL, "Failed to verify if synopsis is present in ACTION BAR");
                isPass = false;
            }

            if (!isPass)
            {
                FailStep(CL, "Failed to verify the requirement(s). Details mentioned above");
            }
            PassStep();

        }
    }
    #endregion
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}