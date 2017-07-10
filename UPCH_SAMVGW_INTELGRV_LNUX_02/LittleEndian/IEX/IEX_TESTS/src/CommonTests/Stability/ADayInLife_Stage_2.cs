/// <summary>
///  Script Name : ADayInLife_Stage_2.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using System.Runtime.InteropServices;
using System.Collections;

[Test("ADayInLife_Stage_2")]
public class ADayInLife_Stage_2 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static string timeOut;
    static string testDuration;
    private static Service service;
    private static Service FirstRecordableService;
    private static Service SecondRecordableService;
    private static Service ThirdRecordableService;
    private static Service FourthRecordableService;
    private static Service ClearService;
    private static Service RadioService_1;
    private static Service RadioService_2;
    private static Service TransponderService_1;
    private static Service TransponderService_2;
    private static string recordedEvent = "EVENT_TO_BE_RECORDED";
    private static string[] TM_REW = { "" };
    private static string[] TM_FF = { "" };
    
    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 9;	// In Minutes
        public const int minToWaitForRecord = 7;	// In minutes
        public const int valueForFullPlayback = 0;
        public const bool playFromBeginning = true;
        public const bool verifyEOF = false;
        public const bool navigateToArchive = true;
        public const double pause = 0;
        public const double play = 1;
         public const int waitAfterSendingCommand = 1000;
    }

    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    private static string nextSubtitle = "";
    private static string defaultSubtitle = "";
    private static int NoOfavailableSubtileLanguages = 0;
    private static string expectedSubtitleToChangeTo = "";
    private static string obtainedSubtitleToChangeTo = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: DITL 2 Execution starts";


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

            timeOut = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TIMEOUT");
            if (timeOut == null)
            {
                FailStep(CL, "Time Out value is not updated in test ini");
            }

            testDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TEST_DURATION");
            if (testDuration == null)
            {
                FailStep(CL, "Test Duration is not defined in Test Ini");
            }

            //we are to do 4SR therefore fetching four different services from content.xml
            //Fetch Value for Service to be Recorded
            FirstRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;IsDefault=True");

            if (FirstRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "First Recordable Service fetched is : " + FirstRecordableService.LCN);
            }

            SecondRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "LCN=" + FirstRecordableService.LCN + ";ParentalRating=High;IsDefault=True");

            if (SecondRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Second Recordable Service fetched is : " + SecondRecordableService.LCN);
            }

            ThirdRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "LCN=" + FirstRecordableService.LCN + "," + SecondRecordableService.LCN + ";ParentalRating=High;IsDefault=True");

            if (ThirdRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Third Recordable Service fetched is : " + ThirdRecordableService.LCN);
            }

            FourthRecordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "LCN=" + FirstRecordableService.LCN + "," + SecondRecordableService.LCN + "," + ThirdRecordableService.LCN + ";ParentalRating=High;IsDefault=True");

            if (FourthRecordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Fourth Recordable Service fetched is : " + FourthRecordableService.LCN);
            }
           //Fetching different services for doing DCA 
           
            RadioService_1 = CL.EA.GetServiceFromContentXML("Type=Radio","ParentalRating=High");
            if (RadioService_1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Radio Service fetched is : " + RadioService_1.LCN);
            }
            RadioService_2 = CL.EA.GetServiceFromContentXML("Type=Radio;Transponder=2", "ParentalRating=High");
            if (RadioService_2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Radio Service fetched is : " + RadioService_2.LCN);
            }

            TransponderService_1 = CL.EA.GetServiceFromContentXML("Type=Video;Transponder=1","ParentalRating=High;IsDefault=True");
            if (TransponderService_1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Transponder 1 Service fetched is : " + TransponderService_1.LCN);
            }
            TransponderService_2 = CL.EA.GetServiceFromContentXML("Type=Video;Transponder=2", "ParentalRating=High");
            if (TransponderService_2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Transponder 2 Service fetched is : " + TransponderService_2.LCN);
            }

            ClearService = CL.EA.GetServiceFromContentXML("Type=Video", "LCN=" + FirstRecordableService.LCN + "," + SecondRecordableService.LCN + "," + ThirdRecordableService.LCN + "," + FourthRecordableService.LCN + ";ParentalRating=High");
            if (ClearService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Clear Service fetched is : " + ClearService.LCN);
            }

            TM_REW = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_REW").Split(',');
            TM_FF = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "LIST_TM_FWD").Split(',');

            //fetch subtitle navigation from project ini
            nextSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_SUBTITLE");
            LogCommentInfo(CL, "Next Subtitle feteched from project ini is : " + nextSubtitle);

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

            //fetching current system time
            DateTime dateTime = DateTime.Now;
            LogCommentImportant(CL, "Current Time is :" + dateTime.ToString());

            //adding test duration to be ran
            TimeSpan timespan = new TimeSpan(Convert.ToInt32(testDuration), 0, 0);

            //getting added value upto which test to be executed

            DateTime combinedTime = dateTime.Add(timespan);
            LogCommentImportant(CL, "Combined Time upto which script to be ran :" + combinedTime);

            ArrayList Actualline = new ArrayList();
            string milestones = "IEX Selection FavoriteChannel";
            bool message = false;

            string currentEPGInfo = "";
            string currentGuideEPGInfo = "";

           
            DateTime now = DateTime.Now;
            int total = 0;

            while (now < combinedTime)
            {

                //Starting with DCA to a variety of services (Video,Recordable, Radio,Across Transponders)
                LogCommentImportant(CL, "Starting with DCA to a variety of services (Video,Recordable, Radio,Across Transponders)");

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, TransponderService_1.LCN, IsDCA: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to Transponder 1 Service");
                }


                res = CL.EA.TuneToRadioChannel(RadioService_1.LCN);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to Radio Service");
                }

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, TransponderService_2.LCN, IsDCA: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to Transponder 2 Service");
                }

                res = CL.EA.TuneToRadioChannel(RadioService_2.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to Radio Service");
                }

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FourthRecordableService.LCN, IsDCA: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to tune to Listed Service");
                }

                //Set and verify favourite channel

                LogCommentImportant(CL, "Setting Current Service as Favorite Service");
                //get current chnum value for channel to be set as favorite

                CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out currentEPGInfo);

                CL.EA.AddToFavouritesFromBanner();
                //verify fav is listed
                CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
                while (message == false)
                {
                    CL.EA.UI.Utils.BeginWaitForDebugMessages(milestones, 20);
                    CL.EA.UI.Utils.SendIR("SELECT_DOWN", 2000);
                    message = CL.EA.UI.Utils.EndWaitForDebugMessages(milestones, ref Actualline);

                    if (message == true)
                    {
                        CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out currentGuideEPGInfo);
                        if (currentEPGInfo == currentGuideEPGInfo)
                        {
                            LogCommentImportant(CL, "Verified Favourite Channel is same as set");
                            break;
                        }
                    }

                }

                LogCommentImportant(CL, "Doing a channel surf from GUIDE");
                //Surf from GUIDE
                res = CL.EA.ChannelSurf(EnumSurfIn.Guide, IsNext: true, NumberOfPresses: 1, DoTune: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Tuning from GUIDE has failed");
                }

                LogCommentImportant(CL, "Testing live pausing and rejoining live");

                res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Live Pause has failed");
                }

                res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Rejoining Live has failed");
                }

                LogCommentImportant(CL, "Testing Subtitle OFF/ON");

                res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to AV SETTINGS SUBTITLES");
                }
                //fetch default subtitle
                res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultSubtitle);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Get Current Subtitle Track Name");
                }

                //Change to any other subtitle
                string timeStamp = "";
                res = CL.IEX.IR.SendIR(nextSubtitle, out timeStamp, Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to next Subtitle in the list");
                }

                CL.EA.UI.Utils.SendIR("SELECT", 2000);


                LogCommentImportant(CL, "Doing P+/P-");

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, IsNext: true, NumberOfPresses: 1);

                CL.IEX.Wait(10);

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, IsNext: false, NumberOfPresses: 1);




                //Set a remainder

                LogCommentImportant(CL, "Setting a reminder");

                res = CL.EA.BookReminderFromGuide("RemainderEvent", FirstRecordableService.LCN, 1, 2);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book Future Reminder from Guide");
                }

                res = CL.EA.WaitUntilReminder("RemainderEvent");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Wait for Reminder Time");
                }

                LogCommentImportant(CL, "Test playback of recording, trick modes, pause, resume and position setting");

                //Schedule a record
                res = CL.EA.PVR.RecordCurrentEventFromBanner(recordedEvent, Constants.minTimeBeforeEventEnd);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to schedule recording");
                }

                //Wait for some time
                res = CL.IEX.Wait(Constants.minToWaitForRecord * 60);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait after start of recording!");
                }

                //Playback the recording
                res = CL.EA.PVR.PlaybackRecFromArchive(recordedEvent, Constants.valueForFullPlayback, Constants.playFromBeginning, Constants.verifyEOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to playback event from Archive");
                }

                // Pause video and check for playback Banner
                res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Constants.pause, Constants.verifyEOF);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to pause playback");
                }

                foreach (string TM in TM_FF)
                {
                    res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Convert.ToDouble(TM), Constants.verifyEOF);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Set playback TM to:" + TM);
                    }
                }

                CL.IEX.Wait(10);

                foreach (string TM in TM_REW)
                {
                    res = CL.EA.PVR.SetTrickModeSpeed(recordedEvent, Convert.ToDouble(TM), Constants.verifyEOF);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Set playback TM to" + TM);
                    }
                }

                LogCommentImportant(CL, "In and out of Standby(Occasionally having a longer standby )");
                res = CL.EA.StandBy(false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                CL.IEX.Wait(180);

                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                //delete earlier recording from archieve

                CL.EA.PVR.DeleteRecordFromArchive(recordedEvent, false, true);
               
             //   DO 4SR and Conflict management on 5th recording

             //    Tune to First Recordable Service fetched  and Record Ongoing Event

                res = CL.EA.TuneToChannel(FirstRecordableService.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to channel " + FirstRecordableService.LCN);
                }

                res = CL.EA.PVR.RecordCurrentEventFromBanner("Event1", -1, false, false, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book the Event From Banner");
                }

                // Tune to Second Recordable Service fetched and Record Ongoing Event

                res = CL.EA.TuneToChannel(SecondRecordableService.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to channel " + SecondRecordableService.LCN);
                }

                res = CL.EA.PVR.RecordCurrentEventFromBanner("Event2", -1, false, false, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book the Event From Banner");
                }

                // Tune to Third Recordable Service fetched and Record Ongoing Event
                CL.IEX.Wait(10);

                res = CL.EA.TuneToChannel(ThirdRecordableService.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to channel " + ThirdRecordableService.LCN);
                }

               
                res = CL.EA.PVR.RecordCurrentEventFromBanner("Event3", -1, false, false, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book the Event From Banner");
                }

                // Tune to Fourth Recordable Service fetched and Record Ongoing Event

                CL.IEX.Wait(10);
                res = CL.EA.TuneToChannel(FourthRecordableService.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to channel " + FourthRecordableService.LCN);
                }

                res = CL.EA.PVR.RecordCurrentEventFromBanner("Event4", -1, false, false, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Book the Event From Banner");
                }

                res = CL.EA.TuneToChannel(ClearService.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to tune to channel " + ClearService.LCN);
                }

                //Try to Book an Ongoing Event,Booking Conflict will occur
                res = CL.EA.PVR.RecordCurrentEventFromBanner("Event5", -1, false, false, false, false);

                //If Conflict is there then Invoke User to Cancel Fifth Recording
               
                CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);

                CL.IEX.MilestonesEPG.SelectMenuItem("DELETE AND CONTINUE");
                CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);

                CL.IEX.LogComment("Fifth Booking was Succesfully canceled by User", true);

               //Stop All Recording one by one 
                res = CL.EA.PVR.StopRecordingFromArchive("Event1", true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Stop Recording for First Event ");
                }

                CL.IEX.Wait(10);

                res = CL.EA.PVR.StopRecordingFromArchive("Event2", true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Stop Recording for Second Event ");
                }

                CL.IEX.Wait(10);

                res = CL.EA.PVR.StopRecordingFromArchive("Event3", true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Stop Recording for Third Event ");
                }

                CL.IEX.Wait(10);

                res = CL.EA.PVR.StopRecordingFromArchive("Event4", true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Stop Recording for Fourth Event ");
                }

                CL.IEX.Wait(10);
                //Delete all recording from archieve
                 res =  CL.EA.PVR.DeleteRecordFromArchive("Event1", false, true, false);
                     if (!res.CommandSucceeded)
                    {
                         FailStep(CL, res, "Failed Delete Recording for First Event ");
                    }

                     CL.IEX.Wait(10);

                 res = CL.EA.PVR.DeleteRecordFromArchive("Event2", false, true, false);
                     if (!res.CommandSucceeded)
                     {
                         FailStep(CL, res, "Failed Delete Recording for Second Event ");
                     }

                     CL.IEX.Wait(10);

                res = CL.EA.PVR.DeleteRecordFromArchive("Event3", false, true, false);
                     if (!res.CommandSucceeded)
                     {
                         FailStep(CL, res, "Failed Delete Recording for Third Event ");
                     }

                     CL.IEX.Wait(10);

                res = CL.EA.PVR.DeleteRecordFromArchive("Event4", false, true, false);
                     if (!res.CommandSucceeded)
                     {
                         FailStep(CL, res, "Failed Delete Recording for Fourth Event ");
                     }
                     
                CL.IEX.Wait(10);
                
                CL.IEX.MilestonesEPG.ClearEPGInfo();
              
                //Again do Stand By IN/OUT
                LogCommentImportant(CL, "In and out of Standby(Occasionally having a longer standby )");
                res = CL.EA.StandBy(false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                CL.IEX.Wait(180);

                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                CL.EA.UI.Utils.VerifyState("LIVE", 20);
                
                //removing favourite service from list
                CL.EA.ChannelSurf(EnumSurfIn.Live, currentEPGInfo, IsDCA: true);
                //Launch Action Bar
                CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
                CL.IEX.MilestonesEPG.Navigate("REMOVE FROM FAVORITES");

                   total++;

                LogCommentImportant(CL, "running number of iteration is :" + total.ToString());

                CL.IEX.Wait(10);

                now = DateTime.Now;
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