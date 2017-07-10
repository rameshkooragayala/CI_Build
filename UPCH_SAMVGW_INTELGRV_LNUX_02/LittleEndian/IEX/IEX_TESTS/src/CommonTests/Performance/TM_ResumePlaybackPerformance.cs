/// <summary>
///  Script Name : TM_ResumePlaybackPerformance.cs
///  Test Name   : TM_ResumePlaybackPerformance
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Fahim G
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("TM_ResumePlaybackPerformance")]
public class TM_ResumePlaybackPerformance : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration


    //Shared members between steps
    static Service videoService;
    static string irKey;
    static string startString;
    static string endString;
    static string loop;
    static string threshold;
    static string item;
    static bool navigationToDo=true ;
    static string timeout;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Value for Test Parameters and Print their Value";
    private const string STEP1_DESCRIPTION = "Step 1: Perform EPG Performance Test ";


    private static class Constants
    {

        public static int minimumMinsRequiredInEvent = 3;
        public static int EventDurationwait = 5;
        public static bool navigate = true;
        public static bool findEvent = true;
        public static bool IsRB = false;
        public static int SecsToPlay = 0;
        public static bool FromBeginning = true;
        public static bool VerifyEOF = false;
        public static double SpeedPlay = 1;
        public static double SpeedPause = 0;
        public static string EventKey = "EventRecorded";
    }


    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);

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

            //Get Values From Test.ini File

            irKey = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IR_KEY");
            if (string.IsNullOrEmpty(irKey))
            {
                FailStep(CL, "Failed to fetch IR_KEY  items in TEST PARAMS from the Test ini file!");
            }

            startString = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "START_STRING");
            if (string.IsNullOrEmpty(startString))
            {
                FailStep(CL, "Failed to fetch START_STRING  items in TEST PARAMS from the Test ini file!");
            }

            endString = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "END_STRING");
            if (string.IsNullOrEmpty(endString))
            {
                FailStep(CL, "Failed to fetch END_STRING  items in TEST PARAMS from the Test ini file!");
            }

            loop = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LOOP");
            if (string.IsNullOrEmpty(loop))
            {
                FailStep(CL, "Failed to fetch LOOP  items in TEST PARAMS from the Test ini file!");
            }

            threshold = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "THRESHOLD");
            if (string.IsNullOrEmpty(threshold))
            {
                FailStep(CL, "Failed to fetch THRESHOLD  items in TEST PARAMS from the Test ini file!");
            }


            item = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ITEM");
            if (string.IsNullOrEmpty(item))
            {
                navigationToDo = false;
                LogCommentWarning(CL, "Failed to fetch ITEM  items in TEST PARAMS from the Test ini file!");
            }

            timeout = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TIMEOUT");
            if (string.IsNullOrEmpty(startString))
            {
                FailStep(CL, "Failed to fetch TIMEOUT  items in TEST PARAMS from the Test ini file!");
            }

            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video Service fetched from content.xml is null");
            }
            LogCommentInfo(CL, "Retrieved Value From Content XML File: videoService = " + videoService.LCN);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + videoService.LCN);
            }

            //Record the current event
            res = CL.EA.PVR.RecordCurrentEventFromBanner(Constants.EventKey, Constants.minimumMinsRequiredInEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service " + videoService.LCN);
            }

            //Wait for some time

            LogComment(CL, "Waiting for 3 minutes to have some content");

            res = CL.IEX.Wait(Constants.EventDurationwait * 60);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to wait for 3 minutes!");
            }

            res = CL.EA.PVR.StopRecordingFromArchive(Constants.EventKey, Constants.navigate);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording ");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {

        double timedifference = 0;

        public override void Execute()
        {
            StartStep();

            string milestones = startString + "," + endString;

            LogComment(CL, "Logs to be validated for measuring Performance are :" + milestones);

            int numberOfIteration = Convert.ToInt32(loop);
            double totalTimeDifference = 0;
            LogComment(CL, "These many number of iterations are going to be performed :" + loop);

            for (int i = 1; i <= numberOfIteration; i++)
            {

                ArrayList Actualline = new ArrayList();
                LogComment(CL, "Playback from Archive for ResumePlayback.Iteration : " + i.ToString());


                string timeStamp = "";

               
                //Playback event to check for glitches
                res = CL.EA.PVR.PlaybackRecFromArchive(Constants.EventKey, Constants.SecsToPlay, Constants.FromBeginning, Constants.VerifyEOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to play the event recording!");
                }


                CL.EA.PVR.SetTrickModeSpeed(Constants.EventKey, Constants.SpeedPause, Constants.VerifyEOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to pause playback ");
                }

                if (navigationToDo)
                {


                    Dictionary<EnumEpgKeys, String> dict = new Dictionary<EnumEpgKeys, String>();

                
                    dict.Add(EnumEpgKeys.TITLE, item);
                    LogComment(CL, "Navigating to ACTION BAR and highlighting :" + item);
                    res = CL.EA.NavigateAndHighlight("STATE:ACTION_BAR_PLAYBACK", dict);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to do navigation and highlight");
                    }
                
                }


                CL.EA.UI.Utils.BeginWaitForDebugMessages(milestones, Convert.ToInt32(timeout));
                CL.IEX.Wait(3);

                LogComment(CL, "Sending IR Key : " + irKey);

                res = CL.IEX.IR.SendIR(irKey, out timeStamp, 3000);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to send irkey " + irKey);
                }
                //CL.IEX.Wait (5);
                CL.EA.UI.Utils.EndWaitForDebugMessages(milestones, ref Actualline);

                List<double> timeArray = CL.EA.UI.Utils.ParseMileStoneTimeFromLogArray(Actualline);

                timedifference = timeArray[1] - timeArray[0];

                totalTimeDifference = totalTimeDifference + timedifference;
                CL.IEX.LogComment("Time taken by the scenario in milli seconds after subtracting value of startlines from endlines:" + timedifference.ToString(), true);


               res= CL.EA.PVR.StopPlayback(Constants.IsRB);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to stop playback ");
                }
            }

            double averageTime = totalTimeDifference / numberOfIteration;

           LogComment(CL,"Average time taken for" + numberOfIteration + " executions:" + averageTime.ToString(), true);

            //Comparing Threshold Value and Average Time 

            int thresholdValue = Convert.ToInt32(threshold);

          LogComment(CL,"Checking against threshold value set for execution :" + threshold);
            if (averageTime > thresholdValue)
            {
                FailStep(CL, "Average Time:" + averageTime.ToString() + "was more than Threshold Value:" + threshold);

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

        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive(Constants.EventKey);
        if (!res.CommandSucceeded)
        {
           LogComment(CL,"Failed to delete recording from archive");
        }
    }
    #endregion
}