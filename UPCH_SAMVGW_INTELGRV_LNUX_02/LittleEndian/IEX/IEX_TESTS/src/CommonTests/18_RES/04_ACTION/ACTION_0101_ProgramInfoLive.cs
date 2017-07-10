/// <summary>
///  Script Name : ACTION_0101_ProgramInfoLive
///  Test Name   : ACTION-0101-ProgramInfoDuringLiveViewing
///  TEST ID     : 68033
///  QC Version  : 2
///  Variations from QC: Step2 ,check for affiliates not scripted
///  JIRA ID     :FC-550
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Renukaradhya 
///  Last modified : 14 Aug 2013 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("ACTION_0101")]
public class ACTION_0101 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //static int testDuration = 0;

    //Shared members between steps
    static Service audioVideoService;
    static string evtName = "";
    static string evtDuration = "";
    static String eventStartTime = "";
    static String eventEndTime = "";

    //Variables used


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";
    private const string STEP1_DESCRIPTION = "Step 1: Surf to a Audio video service ";
    private const string STEP2_DESCRIPTION = "Step 2: Navigate to Action menu and verify that Programme Name,start time and end time of programme is displayed.";
    


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
            else
            {
                LogComment(CL, "Tuned to service having audio and video: "+audioVideoService.LCN);
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

            //Navigate To Action menu
            res = CL.EA.LaunchActionBar();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Action bar");
            }


            //Obtaining event name on action menu.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get program name on action bar");
            }
            else
            {
                LogCommentInfo(CL, "Event name on action bar " + evtName);
            }

            //Obtaining Event duartion on action menu.
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out evtDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event duration on action menu.");
            }
            else
            {
                LogCommentInfo(CL, "Event duration on action menu is " + evtDuration);
            }

            //Get start time on action menu

            eventStartTime = "";

            CL.EA.UI.Utils.ParseEventTime(ref eventStartTime, evtDuration, true);

            if (String.IsNullOrEmpty(eventStartTime))
            {
                CL.IEX.FailStep("Failed to get the event start time on action menu");
            }
            else
            {
                LogCommentInfo(CL, "Event start time on action menu: " + eventStartTime);
            }

            //Get end time on action menu
            eventEndTime = "";

            CL.EA.UI.Utils.ParseEventTime(ref eventEndTime, evtDuration, true);

            if (String.IsNullOrEmpty(eventEndTime))
            {
                CL.IEX.FailStep("Failed to get the event end time on action menu");
            }
            else
            {
                LogCommentInfo(CL, "Event end time on action menu is: " + eventEndTime);
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

