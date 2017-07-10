/// <summary>
///  Script Name : ACTION_0007_FutureProgramInfo
///  Test Name   : ACTION-0007-FutureProgramInfo
///  TEST ID     : 68029
///  QC Version  : 4
///  Variations from QC:none
///  JIRA ID     : FC-547
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Renukaradhya 
///  Last modified : 12 Aug 2013 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("ACTION_0007")]
public class ACTION_0007 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //static int testDuration = 0;

    //Shared members between steps
    static Service audioVideoService;
    static string evtNameFuture = "";
    static string dateOnFutureEvt = "";
    static string evtDurationOnFutureEvt = "";
    static string eventStartTime = "";
    static string eventEndTime = "";

    //Variables used
 

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";
    private const string STEP1_DESCRIPTION = "Step 1:Navigate to Program Grid and launch all channels ";
    private const string STEP2_DESCRIPTION = "Step 2:Navigate to Future event on Gride and verify that Programme Name,Start Date,start time and end time of programme is displayed.";
    private const string STEP3_DESCRIPTION = "Step 3:Navigate to Future event on channel bar and verify that Programme Name,Start Date,start time and end time of programme is displayed.";

    static class Constants
    {
        public const bool moveRight = true;
        public const int noOfRightPresses = 1;
        

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

            //Get Values From Content XML File
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (audioVideoService == (null))
            {
                FailStep(CL, "Failed to fetch audioVideoService from content xml.");

            }

            else
            {
                LogCommentInfo(CL, "audioVideoService: " + audioVideoService.LCN);

            }
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
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

            //Navigate to future event on Grid.
            res = CL.EA.BrowseGuideFuture(Constants.moveRight, Constants.noOfRightPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to future event on grid");
            }

            //Obtaining event name on Grid for future event.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtNameFuture);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get program name on future event on grid");
            }
            else
            {
                LogCommentInfo(CL, "Future event name on grid is " + evtNameFuture);
            }

            //Obtaining Date on Grid for future event.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("Selection date", out dateOnFutureEvt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get date on future event on Grid");
            }
            else
            {
                LogCommentInfo(CL, "Start Date on future event on grid is " + dateOnFutureEvt);
            }

            //Obtaining Event duration on Grid for future event.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out evtDurationOnFutureEvt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event duration on future event on Grid");
            }
            else
            {
                LogCommentInfo(CL, "Event duration on future event on grid is " + evtDurationOnFutureEvt);
            }

            //Get start time on future event on grid

            eventStartTime = "";

            CL.EA.UI.Utils.ParseEventTime(ref eventStartTime,evtDurationOnFutureEvt,true);

            if (String.IsNullOrEmpty(eventStartTime))
            {
                CL.IEX.FailStep("Failed to get the event start time on grid");
            }
            else
            {
                LogCommentInfo(CL, "Event start time on future event on grid is: " + eventStartTime);
            }

            //Get end time on future event.
            eventEndTime = "";

            CL.EA.UI.Utils.ParseEventTime(ref eventEndTime, evtDurationOnFutureEvt, false);

            if (String.IsNullOrEmpty(eventEndTime))
            {
                CL.IEX.FailStep("Failed to get the event end time on grid");
            }
            else
            {
                LogCommentInfo(CL, "Event end time on future event on grid is: " + eventEndTime);
            }

            CL.EA.ReturnToLiveViewing(true);
            PassStep();
        }
    }
    #endregion

    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Launch channel bar.
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            //Navigate to future event on channel bar.
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to future event on channel bar");
            }

            //Obtaining event name on channel bar for future event.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtNameFuture);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get programme name on future event on channel bar");
            }
            else
            {
                LogCommentInfo(CL, "Event Name on future event on channel bar is: " + evtNameFuture);
            }
            //Obtaining Date on channel bar for future event.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("Selection date", out dateOnFutureEvt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get start date on future event on channel bar");
            }
            else
            {
                LogCommentInfo(CL, "Start date on future event on channel bar is: " + dateOnFutureEvt);
            }

            //Obtaining Event time on channel bar for future event.
            
            
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out evtDurationOnFutureEvt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event duration on future event on channel bar");
            }
            else
            {
                LogCommentInfo(CL, "Event duration on future event is on channel bar is: " + evtDurationOnFutureEvt);
            }

            //Get start time on future event.
            eventStartTime = "";

            CL.EA.UI.Utils.ParseEventTime(ref eventStartTime, evtDurationOnFutureEvt, true);

            if (String.IsNullOrEmpty(eventStartTime))
            {
                CL.IEX.FailStep("Failed to get the event start time on channel bar");
            }
            else
            {
                LogCommentInfo(CL, "Event start time on future event is on channel bar is: " + eventStartTime);
            }

            //Get end time on future event.
            eventEndTime = "";

            CL.EA.UI.Utils.ParseEventTime(ref eventEndTime, evtDurationOnFutureEvt, false);

            if (String.IsNullOrEmpty(eventEndTime))
            {
                CL.IEX.FailStep("Failed to get the event end time on channel bar");
            }
            else
            {
                LogCommentInfo(CL, "Event end time on future event on channel bar is: " + eventEndTime);
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
    
