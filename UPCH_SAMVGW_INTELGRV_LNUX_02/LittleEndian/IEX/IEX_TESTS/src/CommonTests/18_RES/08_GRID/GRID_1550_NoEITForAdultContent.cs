/// <summary>
///  Script Name : GRID_1550_NoEITForAdultContent.cs
///  Test Name   : GRID-1550-Programme-Grid-Adult Programme
///  TEST ID     : 68082
///  JIRA ID     : FC-557
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 08/08/2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("GRID_1550")]
public class GRID_1550 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service lockedChannel;
    static string obtainedEventTime = "";
    static Helper helper = new Helper();

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channels From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Launch Program Grid and select OK on future locked event";
    private const string STEP2_DESCRIPTION = "Step 2: Enter a Valid PIN";
    private const string STEP3_DESCRIPTION = "Step 3: Exit the Prgram Grid & Invoke it again";
    private const string STEP4_DESCRIPTION = "Step 4: Navigate to future locked program and select OK";

    static class Constants
    {
        public const double timeToPress = -1;
        public const bool moveRight = true;
        public const int noOfRightPresses = 2;
        public const string nextState = "TV GUIDE";
        public const double timeOutInSec = 3;
    }

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);

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

            //Get Values From xml File
            lockedChannel = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "");
            if (lockedChannel == null)
            {
                FailStep(CL, "Failed to get channel from Content.xml for the criterion passed");
            }

            LogCommentInfo(CL, "Channel fetched from  Content.xml: " + lockedChannel.LCN);
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

            //Tune to Locked Channel In Guide
            res = CL.EA.SurfToChannelInGuide(lockedChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Surf to Channel: " + lockedChannel.LCN + " in Guide");
            }

            //Surf to Future Event in Guide
            res = CL.EA.BrowseGuideFuture(Constants.moveRight, Constants.noOfRightPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Browse to Future Event in Guide");
            }

            //Get Event Time from the current focussed Event
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Time from EPG");
            }

            //Selecting and verifying PIN entry State
            if (!helper.VerifyPinStateOnSelect())
            {
                FailStep(CL, "Failed to Select and Verify PIN entry state");
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
            string obtainedEventName = "";
            //Get Value from Project.ini
            string alternateEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");
            if (string.IsNullOrEmpty(alternateEventName))
            {
                FailStep(CL, "Failed to get Alternate Name for Locked Event from Project.ini");
            }

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Enter default PIN to unlock the channel
            res = CL.EA.EnterDeafultPIN(Constants.nextState);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter PIN to unlock the Adult Channel: " + lockedChannel.LCN + " from Guide and Verify the Next State as " + Constants.nextState);
            }

            DateTime timeOut = DateTime.Now.AddSeconds(Constants.timeOutInSec);

            do
            {
                //Get Event Name from EPG
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Event Name from Guide");
                }
                if (string.IsNullOrEmpty(obtainedEventName))
                {
                    LogCommentWarning(CL, "Event Name is Empty from milestone");
                }
                else
                {
                    break;
                }

                System.Threading.Thread.Sleep(300);
            }
            while (DateTime.Now.Subtract(timeOut).TotalSeconds < 0);

            if (string.IsNullOrEmpty(obtainedEventName))
            {
                FailStep(CL, "Event Name is Empty in TV GUIDE after Unlocking");
            }
            //Checking if Real Event Name is shown
            else if (obtainedEventName.Equals(alternateEventName))
            {
                FailStep(CL, "Failed to Verify the Event Information after Unlocking the Locked Channel: " + lockedChannel.LCN);
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
            //Exiting from Guide
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to exit from TV Guide");
            }

            //Tune to locked channel
            res = CL.EA.SurfToChannelInGuide(lockedChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel: " + lockedChannel.LCN + " in Guide");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            int count = 0;
            string expectedEventTime = obtainedEventTime;

            do
            {

                //Focuss on Next Event
                res = CL.EA.BrowseGuideFuture(Constants.moveRight);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to move right in Guide");
                }

                //Get Event Time for EPG
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Event Time from Guide on Channel: " + lockedChannel.LCN);
                }

                count++;

                if (count > Constants.noOfRightPresses)
                {
                    FailStep(CL, "Failed to get same event which was unlocked in STEP3 from Guide");
                }

            } while (!obtainedEventTime.Equals(expectedEventTime) || count != Constants.noOfRightPresses);

            //Selecting and verifying PIN entry State
            if (!helper.VerifyPinStateOnSelect())
            {
                FailStep(CL, "Failed to Select and Verify PIN entry state");
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

    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Press "Select" and Verifies the PIN Entry state
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifyPinStateOnSelect()
        {
            IEXGateway._IEXResult res;
            bool isPass = true;
            string expectedStateForPinEntry = "INSERT PIN UNLOCK CHANNEL";

            res = CL.IEX.MilestonesEPG.Navigate(expectedStateForPinEntry);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to select on locked channel from Guide");
                isPass = false;
            }

            return isPass;
        }
    }
    #endregion
}

