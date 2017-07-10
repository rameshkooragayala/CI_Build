/// <summary>
///  Script Name : SET_PLB_Skip.cs
///  Test Name   : SET-PLB-0551-Skip Forward - default value
///  Test Name   : SET-PLB-0553-Skip Backward - default value
///  TEST ID     : 71547
///  TEST ID     : 71548
///  TEST Repository:Unified_ATP_FOr_HMD_Cable
///  QC Version  : 2
///  Variations from QC:
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

[Test("SET_PLB_Skip_values")]
public class SET_PLB_Skip_values : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration


    //Shared members between steps

    static string SkipFFIntervalDefault;
    static string SkipBKIntervalDefault;
    static Service videoService;

    //Constants used in the test
    private static class Constants
    {

        public static int minimumMinsRequiredInEvent = 5;
        public static int EventDurationwait = 10;
        public static int SecsToPlay =0;
        public static bool FromBeginning = true;
        public static bool VerifyEOF = false;
        public static bool directionFF = true;
        public static bool directionREW = false;
        public static bool PlaybackContext = true;

    }



    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From content.xml";
    private const string STEP1_DESCRIPTION = "Step 1:Access forward skip parameters settings (in settings --> My Preferences --> General --> Skip Forward Interval) ";
    private const string STEP2_DESCRIPTION = "Step 2:Playback the recording / RB";
    private const string STEP3_DESCRIPTION = "Step 3:Skip forward/Backward in the recording.";


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

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=15;IsRecordable=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video Service fetched from content.xml is null");
            }
            LogCommentInfo(CL, "Retrieved Value From Content XML File: videoService = " + videoService.LCN);


            String ForwardSkipDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "SET_SKIP", "SKIP_FORWARD_DEFAULT");
            if (ForwardSkipDefault.Equals(""))
            {

                FailStep(CL, "Failed to fetch SKIP_FORWARD_DEFAULT  items in SET_SKIP from the Project ini file!");
            }
            else
            {
                SkipFFIntervalDefault = ForwardSkipDefault;
            }

            String BackwardSkipDefault = CL.EA.GetValueFromINI(EnumINIFile.Project, "SET_SKIP", "SKIP_BACKWARD_DEFAULT");
            if (BackwardSkipDefault.Equals(""))
            {

                FailStep(CL, "Failed to fetch SKIP_BACKWARD_DEFAULT  items in SET_SKIP from the Project ini file!");
            }
            else
            {
                SkipBKIntervalDefault = BackwardSkipDefault;
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

            //Wait for some time

            LogComment(CL,"Waiting for recording to complete");

            res = CL.IEX.Wait(Constants.EventDurationwait * 60);
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

            string default_option_from_epg;
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP FORWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to VIDEO SKIP FORWARD setting");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out default_option_from_epg);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to get current selection title");
            }

            if (SkipFFIntervalDefault != default_option_from_epg)
            {
                FailStep(CL, res, "the default option : " + default_option_from_epg + " should have been: " + SkipFFIntervalDefault);
            }


            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
            }


            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:VIDEO SKIP BACKWARD");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to VIDEO SKIP BACKWARD setting");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out default_option_from_epg);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to get current selection title");
            }

            if (SkipBKIntervalDefault != default_option_from_epg)
            {
                FailStep(CL, res, "the default option : " + default_option_from_epg + " should have been: " + SkipBKIntervalDefault);
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing");
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
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //EnumVideoSkip valueset = new EnumVideoSkip();

            string Interval_to_set = SkipFFIntervalDefault;
            EnumVideoSkip valueset=0;

            switch (Interval_to_set)
            {
                case "+60 SEC":

                 valueset = EnumVideoSkip._60;
                 break;


                case "+10 SEC":


                 valueset = EnumVideoSkip._10;
                 break;

                case "+30 SEC":


                 valueset = EnumVideoSkip._30;
                 break;


                case "+5 MIN":


                 valueset = EnumVideoSkip._300;
                 break;


                case "+10 MIN":


                 valueset = EnumVideoSkip._600;
                 break;


            }




            res = CL.EA.PVR.Skip(Constants.directionFF, Constants.PlaybackContext, valueset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do forward skip with:" + valueset.ToString());
            }


            Interval_to_set = SkipBKIntervalDefault;
             valueset = 0;

            switch (Interval_to_set)
            {
                case "-60 SEC":

                    valueset = EnumVideoSkip._60;
                    break;


                case "-7 SEC":

                    valueset = EnumVideoSkip._7;
                    break;

                case "-15 SEC":


                    valueset = EnumVideoSkip._15;
                    break;

                case "-10 SEC":


                    valueset = EnumVideoSkip._10;
                    break;

                case "-30 SEC":


                    valueset = EnumVideoSkip._30;
                    break;


                case "-5 MIN":


                    valueset = EnumVideoSkip._300;
                    break;


                case "-10 MIN":


                    valueset = EnumVideoSkip._600;
                    break;


            }

            //Playback event to check for glitches
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", Constants.SecsToPlay, Constants.FromBeginning, Constants.VerifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play the event recording!");
            }

            //Wait for some time
            LogComment(CL, "Waiting for recording to play for some time");

            res = CL.IEX.Wait(Constants.EventDurationwait * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for recording content!");
            }

            res = CL.EA.PVR.Skip(Constants.directionREW, Constants.PlaybackContext, valueset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do backward skip with:"+ valueset.ToString());
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
   





    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}