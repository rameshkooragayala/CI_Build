/// <summary>
///  Script Name : GRID_1526_PrgmGrid_Channel_Prgm_Info_HDPVR.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 1
///  Variations from QC:
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

[Test("GRID_1526_PrgmGrid_Channel_Prgm_Info_HDPVR")]
public class GRID_1526 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static Service S1;
    static Service S2;
    static Service S3;
    private static string futureEventRecording = "FUTURE_EVENT";
    private static string futureTimeBasedRecording = "futureTimeBasedRecording";
    static int evtEndLeftTime;
    public const int numberofRightKeyPressinGrid = 1;
    public const int numOfPressesForNextEvent = 2;
    public const int minTimeBeforeEventStart = 1;
    static bool isInGuide;
    static string evtName;
    static string evtNameFrmGrid;
    static string timestamp;
    static string reminderStatus;
    static string recordrStatus;
    public const double timeToPressKey = -1;


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Services from content xml & Set Reminder, Event based recording & Time based recording";
    private const string STEP1_DESCRIPTION = "Step 1: Laucnh All CHannels Guide & verify guide is focused to current Evt";
    private const string STEP2_DESCRIPTION = "Step 2: Verify Reminder indicator and Recording Booking indicator in Service S1 ";
    private const string STEP3_DESCRIPTION = "Step 3: Launch All Channels Guide in Service S2 & Verify Recording Booking indicator in Service S2";
    private const string STEP4_DESCRIPTION = "Step 4: Launch All Channels Guide in Service S3 & Verify No Programme Information Available in Service S3";


   

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
           

           // get service from content xml
            S1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;IsRecordable=True;EventDuration=60", "ParentalRating=High");
            S2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;IsRecordable=True", "ParentalRating=High;LCN=" + S1.LCN + "");
            S3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=False;IsRecordable=True", "ParentalRating=High;LCN=" + S1.LCN + "," + S2.LCN + "");
           
            int evtDuration = Convert.ToInt32(S2.EventDuration);
            int duration = Convert.ToInt32((Convert.ToInt32(S2.EventDuration) * 60) / 2);


            //Book a time-based recording on live
              //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, S2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S1.LCN);
            }

            CL.EA.UI.Banner.GetEventTimeLeft(ref evtEndLeftTime);

            if (evtEndLeftTime <= 540)
            {
                LogComment(CL, "Returning to Live viewing from Action Bar Launched During GetCurrentEventLeftTime");
                res = CL.EA.ReturnToLiveViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Return to Live Viewing");
                }

                LogComment(CL, "Wait till event end");
                CL.IEX.Wait(evtEndLeftTime);

                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:LIVE");

                CL.EA.UI.Banner.GetEventTimeLeft(ref evtEndLeftTime);
            }

            //(Start time is a few minutes into the future, to make sure the recording will not be over before the booking sequence will complete);
            res = CL.EA.PVR.RecordManualFromCurrent("ManualRecord", S2.LCN, evtDuration + (duration / 60));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
            }
            CL.IEX.Wait(2);

            //  Book Reminder on Future Event on S1 Next Event
            res = CL.EA.BookReminderFromGuide("Event1", S1.LCN, numberofRightKeyPressinGrid, minTimeBeforeEventStart);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Reminder from Guide on future event of service 1" + S1.LCN);
            }

            CL.IEX.Wait(2);

            //Book the future event for recording
            res = CL.EA.PVR.BookFutureEventFromGuide(futureEventRecording, S1.LCN, numOfPressesForNextEvent, minTimeBeforeEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event for recording!");
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
            // fetch event name

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, S1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S1.LCN);
            }

            CL.IEX.Wait(1);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to get eventName Milestone");
            }
            // navigate to All Channel
            CL.EA.UI.Guide.Navigate();
            isInGuide = CL.EA.UI.Guide.IsGuide();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch All channels Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified All Channels guide launched");
            }
            CL.IEX.Wait(1);

            // fetch evtName from grid

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtNameFrmGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to get evtName Milestone");
            }

            if (evtName == evtNameFrmGrid)
            {
                LogCommentInfo(CL, "Verified focus is on current event after launching grid");
            }
            else
            {
                FailStep(CL, "Fail to verify focus is on current event after launching grid");
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
           
            // navigating to future event
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR key select");
            }

            CL.IEX.Wait(1);

            res = CL.IEX.SendIRCommand("SELECT",timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR key select");
            }
            CL.IEX.Wait(2);

            // get EPG milestone for checking Reminder Indicator

            res = CL.IEX.MilestonesEPG.GetEPGInfo("reminder/recording status", out reminderStatus);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get reminder Milestone");
            }
            CL.IEX.Wait(2);

            // verifing Reminder Indicator
            if (reminderStatus.Trim() != "[REMINDER_EPISODE]")
            {
                FailStep(CL, res, "Failed to verify reminder indicator: " + S1.LCN);
            }
            else
            {
                LogCommentInfo(CL,"verifed reminder indicator: " + S1.LCN);
            }


            res = CL.IEX.SendIRCommand("BACK", timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Fail to press BACK Key");
            }

            CL.IEX.Wait(4);
          
           // navigating to future event
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR key select");
            }

           CL.IEX.Wait(10);

            //Selecting future event to get recording status milestone

            res = CL.IEX.SendIRCommand("SELECT", timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR key select");
            }

            CL.IEX.Wait(4);

            // getting recording status milestone

           res= CL.IEX.MilestonesEPG.GetEPGInfo("recordingstatus", out recordrStatus);
           if (!res.CommandSucceeded)
           {
               FailStep(CL, res, "Failed to get recordingstatus Milestone");
           }

            // Verifiing Record Indicator
           CL.IEX.Wait(2);
            if (recordrStatus.Trim() != "Future Recording State")
            {
                FailStep(CL, res, "Failed to verify record indicator: " + S1.LCN);
            }
            else
            {
                LogCommentInfo(CL, "verifed record indicator: " + S1.LCN);
            }

            //Retirn to Live

            res= CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed return to live");
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

            //Surf to Service S2
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, S2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S2.LCN);
            }
            CL.IEX.Wait(1);
            // navigating to future event
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPressKey, ref timestamp);

            CL.IEX.Wait(10);

            // select future event to get recording status milestone

            res = CL.IEX.SendIRCommand("SELECT", timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR key select");
            }
            CL.IEX.Wait(4);

            //fetching recording status
            res = CL.IEX.MilestonesEPG.GetEPGInfo("recordingstatus", out recordrStatus);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get recordingstatus Milestone");
            }
            CL.IEX.Wait(2);
            //verifing recording status

            if (recordrStatus.Trim().Contains("Recording State"))
            {
                LogCommentInfo(CL, "verifed record indicator: " + S1.LCN);              
            }
            else
            {
                FailStep(CL, res, "Failed to verify record indicator: " + S1.LCN);
            }

            //return to live
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed return to live");
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

            //Surf to Service S3
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, S3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S3.LCN);
            }

            // navigate to All Channel
            CL.EA.UI.Guide.Navigate();
            isInGuide = CL.EA.UI.Guide.IsGuide();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch All channels Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified All Channels guide launched");
            }
            CL.IEX.Wait(1);

            //Get Event Information in Guide

            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            CL.IEX.Wait(1);
            //Verify event Information inguide
            if (evtName == "No programme information available")
            {

                LogCommentInfo(CL, "Verified No information available");
            }
            else
            {
                FailStep(CL, "Failed - Event information is  available");
            }

            //Return to live

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed return to live");
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
        //Delete All Recording from Archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive(false);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete recording because" + res.FailureReason);
        }
    }
    #endregion
}