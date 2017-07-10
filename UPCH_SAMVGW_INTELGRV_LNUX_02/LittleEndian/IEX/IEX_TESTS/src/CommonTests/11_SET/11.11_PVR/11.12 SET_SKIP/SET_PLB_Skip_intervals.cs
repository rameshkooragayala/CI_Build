/// <summary>
///  Script Name : SET_PLB_Skip_intervals.cs
///  Test Name   : SET-PLB-0550-Skip Forward - intervals
///  Test Name   : SET-PLB-0552-Skip Backward  - interval
///  TEST ID     : 71545
///  TEST ID     : 71546
///  TEST Repository:Unified_ATP_FOr_HMD_Cable
///  QC Version  : 2
///  Variations from QC:Not checking step 5 as event playback exits on extra last skip
/// ----------------------------------------------- 
///  Modified by : Fahim G
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("SET_PLB_Skip_intervals")]
public class SET_PLB_Skip_intervals : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration


    //Shared members between steps

    static string[] SkipFFIntervalItems;
    static string[] SkipBKIntervalItems;
    static Service videoService;


    //Constants used in the test
    private static class Constants
    {

        public static int minimumMinsRequiredInEvent = 5;
        public static int EventDurationwait = 10;
        public static int SecsToPlay = 0;
        public static bool FromBeginning = true;
        public static bool VerifyEOF = false;
        public static bool directionFF = true;
        public static bool directionREW = false;
        public static bool PlaybackContext = true;

    }



    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From content.xml";
    private const string STEP1_DESCRIPTION = "Step 1:Access forward skip parameters settings (in My preferences --> General --> Skip Forward Interval) ";
    private const string STEP2_DESCRIPTION = "Step 2:Repeat following steps for each one of the values above: Set FORWARD SKIP PARAMETERS to the value. ";
    private const string STEP3_DESCRIPTION = "Step 3: Playback the recording.";
    private const string STEP4_DESCRIPTION = "Step 4:Skip forward in the recording. ";
    private const string STEP5_DESCRIPTION = "Step 5:Skip backward in the recording ";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);

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

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=15;IsRecordable=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video Service fetched from content.xml is null");
            }
            LogCommentInfo(CL, "Retrieved Value From Content XML File: videoService = " + videoService.LCN);


            String ForwardSkipIntervals = CL.EA.GetValueFromINI(EnumINIFile.Project, "SET_SKIP", "SKIP_FORWARD_INTERVALS");
            if (!ForwardSkipIntervals.Equals(""))
            {
                SkipFFIntervalItems = ForwardSkipIntervals.Split(',');
            }
            else
            {
                FailStep(CL, "Failed to fetch SKIP_FORWARD_INTERVALS  items in SET_SKIP from the Project ini file!");
            }

            String BackwardSkipIntervals = CL.EA.GetValueFromINI(EnumINIFile.Project, "SET_SKIP", "SKIP_BACKWARD_INTERVALS");
            if (!BackwardSkipIntervals.Equals(""))
            {
                SkipBKIntervalItems = BackwardSkipIntervals.Split(',');
            }
            else
            {
                FailStep(CL, "Failed to fetch SKIP_BACKWARD_INTERVALS  items in SET_SKIP from the Project ini file!");
            }




            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + videoService.LCN);
            }

            //Record the current event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event1", Constants.minimumMinsRequiredInEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service " + videoService.LCN);
            }

            //Record the current event
            res = CL.EA.PVR.BookFutureEventFromBanner("Event2",1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event on service " + videoService.LCN);
            }


            //Wait for some time
            LogComment(CL, "Waiting for recording to complete");

            res = CL.IEX.Wait(Constants.EventDurationwait*2*60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for record to complete!");
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


             res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP FORWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to VIDEO SKIP FORWARD Setting");
            }

            //ON DEMAND

            foreach (String SkipMenuItem in SkipFFIntervalItems)
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem(SkipMenuItem);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to select the menu item " + SkipMenuItem);
        		}
                else
        		{
                    CL.IEX.LogComment("Traverse to menu option" + SkipMenuItem);
                }
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after Main menu Navigation");
            }


            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP BACKWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to VIDEO SKIP BACKWARD Setting");
            }

            //ON DEMAND

            foreach (String SkipMenuItem in SkipBKIntervalItems)
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem(SkipMenuItem);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to select the menu item " + SkipMenuItem);
                }
                else
                {
                    CL.IEX.LogComment("Traverse to menu option" + SkipMenuItem);
                }
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after Main menu Navigation");
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



            //Set the Skip duration to Bookmark
            res = CL.EA.STBSettings.SetSkipForwardInterval(EnumVideoSkip.BOOKMARKMODE);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  set Skip Forward Interval setting");
            }

            //Set the Skip duration to Bookmark
            res = CL.EA.STBSettings.SetSkipBackwardInterval(EnumVideoSkip.BOOKMARKMODE);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to  set Skip backward Interval setting");
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




            //Playback event to check for glitches
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", Constants.SecsToPlay, Constants.FromBeginning, Constants.VerifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the event recording!");
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


            res = CL.EA.PVR.Skip(Constants.directionFF, Constants.PlaybackContext, EnumVideoSkip.BOOKMARKMODE, 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do forward skip with boookmark interval!");
            }


            PassStep();
        }
    }
    #endregion

    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();


            res = CL.EA.PVR.Skip(Constants.directionREW, Constants.PlaybackContext, EnumVideoSkip.BOOKMARKMODE, 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do backward skip with boookmark interval!");
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