/// <summary>
///  Script Name : FT173_013_Event_Nav_Available_Timeline.cs
///  Test Name   : FT173-013-Event-Nav-Available-Timeline
///  TEST ID     :25477 
///  QC Version  : 5
///  Variations from QC: instead of doing right navigation from day 1, performing day skip naviagtion till last day EIT available and then perform right navigation
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT173_013_Event_Nav_Available_Timeline")]
public class FT173_013 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static EnumGuideViews guideView;
    static EnumSurfIn enumSurfIn;
    static string guideviewFromTestIni;
    static int EIT;
    static string channel1;
    static string lastEventTime;
    static string selectionDate;
    static string selectionDateAfterChangeEvt;
    static string evtTime;
    static string evtTimeAfterChangeEvt = "";
    static string futureEventKey;
    static string timestamp;
    static string adjTimeLineDuration;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,EIT Available,EIT Last event time,GuideView";
    private const string STEP1_DESCRIPTION = "Step 1:Perform day skip till last day EIT available ";
    private const string STEP2_DESCRIPTION = "Step 2:Perform Right Navigation till last event and Verify till navigation is happening till last event";


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

           // Get values from TEst INI
            try
            {
                channel1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel1");

                guideviewFromTestIni = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "GuideView");
                if (guideviewFromTestIni == "")
                {
                    FailStep(CL, "Fail to fetch GuideView from Test INI");
                }

                EIT = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EIT_AVAILABLE"));

                adjTimeLineDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DURATION");
                if (adjTimeLineDuration == "")
                {
                    FailStep(CL, "Fail to fetch GuideView from Test INI");
                }


                switch (guideviewFromTestIni)
                {

                    case "ALL_CHANNELS":
                        {
                            guideView = EnumGuideViews.ALL_CHANNELS;
                            enumSurfIn = EnumSurfIn.Guide;
                            futureEventKey = "SELECT_RIGHT";
                            break;
                        }
                    case "ADJUST_TIMELINE":
                        {
                            guideView = EnumGuideViews.ADJUST_TIMELINE;
                            enumSurfIn = EnumSurfIn.GuideAdjustTimeline;
                            futureEventKey = "SELECT_RIGHT";
                            break;
                        }
                    case "BY_GENRE":
                        {
                            guideView = EnumGuideViews.BY_GENRE;
                            futureEventKey = "SELECT_DOWN";
                            break;
                        }
                    case "SINGLE_CHANNEL":
                        {
                            guideView = EnumGuideViews.SINGLE_CHANNEL;
                            enumSurfIn = EnumSurfIn.GuideSingleChannel;
                            futureEventKey = "SELECT_DOWN";
                            break;

                        }

                }

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel1);
               if (!res.CommandSucceeded)
               {
                   FailStep(CL, "fail to tune to channel1");
               }
               
            }
            catch(Exception ex)
            {
                FailStep(CL, "Fail in Precondition. Reason: " + ex.Message);
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

            if(EIT!=0)
            {
            res=CL.EA.DaySkipInGuide(guideView, true, EIT - 1, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to perform day skip till last day eit");
            }
            }
            else
            {
                res = CL.EA.ChannelSurf(enumSurfIn, channel1, GuideTimeline: adjTimeLineDuration);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to Tune to service s2");
                }

                LogCommentInfo(CL, "Not performing dayskip since only EIT PF is available");
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

            if (EIT != 0)
            {
                // get selection date of last day eit is available
                res = CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out selectionDate);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch selection date from EPG Milestone");
                }

                //move to prev day skip
                res = CL.EA.DaySkipInGuide(guideView, false, 1, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to perform day skip to prev day");
                }
                // get event name
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch evt name from EPG Milestone");
                }

                // moving to next event until EIT data ends
                while (evtTime != evtTimeAfterChangeEvt)
                {
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTime);
                    CL.IEX.Wait(2);

                    // moving to next event
                    res = CL.IEX.IR.SendIR(futureEventKey, out timestamp, 2);

                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail select future event");
                    }
                    CL.IEX.Wait(4);
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTimeAfterChangeEvt);
                    CL.IEX.Wait(2);
                    LogCommentInfo(CL, "Moved to Next Event Evt Time:" + evtTimeAfterChangeEvt);

                    if (evtTime == evtTimeAfterChangeEvt) // work around for key  miss
                    {
                        // moving to next event
                        res = CL.IEX.IR.SendIR(futureEventKey, out timestamp, 2);

                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, "Fail select future event");
                        }
                        CL.IEX.Wait(4);
                        res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTimeAfterChangeEvt );
                        CL.IEX.Wait(2);
                        LogCommentInfo(CL, "Moved to Next Event Evt Time:" + evtTimeAfterChangeEvt);
                    }

                }

                // verifiying once again EIT data not available
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch event name from EPG Milestone");
                }
                CL.IEX.Wait(1);

                // moving to next event
                res = CL.IEX.IR.SendIR(futureEventKey, out timestamp, 2);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail select future event");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTimeAfterChangeEvt);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch eventname after change event from EPG Milestone");
                }
                CL.IEX.Wait(1);


                // getting selection date 

                res = CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out selectionDateAfterChangeEvt);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch selection date from EPG Milestone");
                }


                // verifiing event and selection date 

                if (evtTime == evtTimeAfterChangeEvt && selectionDate == selectionDateAfterChangeEvt)
                {
                    LogCommentInfo(CL, "Verified guide reached last event");
                }

                else
                {
                    FailStep(CL, "Failed to Verified guide reached last event");
                }
            }
            else
            {

                //CL.EA.UI.Guide.NextEvent(1);
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch event name from EPG Milestone");
                }
                CL.IEX.Wait(1);

                // moving to next event
                res = CL.IEX.IR.SendIR(futureEventKey, out timestamp, 2);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail select future event");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTimeAfterChangeEvt);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch eventname after change event from EPG Milestone");
                }

                if (evtTime == evtTimeAfterChangeEvt)
                {
                    LogCommentInfo(CL, "Verified only EIT PF is available");
                }

                else
                {
                    FailStep(CL, "Failed to Verifyonly EIT PF is available");
                }

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