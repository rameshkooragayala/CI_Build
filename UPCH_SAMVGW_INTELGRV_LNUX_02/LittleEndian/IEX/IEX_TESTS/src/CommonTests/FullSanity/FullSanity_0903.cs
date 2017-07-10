using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0903 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the tes
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    private static string endGuardTimeName;
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        /*
			Schedule [4] Future Recordings (some time-based, some event-based - at least 1 of each);.
             Make sure that SGT hasn't started for event-based events
             Stay On live On one of the channels of the recordings
             Booking events before manual recordings to be sure the correct ones are recorded (MR booking takes time, and the timing might be lost 
		*/
        this.AddStep(new PreCondition(), "Preconditions: Get Values from ini File & SyncStream");
        this.AddStep(new Step1(), "Step 1: Schedule Some Time-Based Recordings & Some Future Event-Based Simultaneous Recordings");
        this.AddStep(new Step2(), "Step 2: During the Recordings Go Into Standby for a While, Come Out & Wait for Recordings to Finish (including EGT)");
        this.AddStep(new Step3(), "Step 3: Playback All 5 Recordings to Reach EOF");


        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_2.LCN);
            }
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_3.LCN);
            }
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_4.LCN);
            }
            string StartGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (StartGuardTimeName == null)
            {
                LogCommentFailure(CL, "Failed to get SGT MIN value from Project INI file");
            }

            res = CL.EA.STBSettings.SetGuardTime(true, StartGuardTimeName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Set Guard time");
            }

            endGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MIN");
            if (endGuardTimeName == null)
            {
                LogCommentFailure(CL, "Failed to get EGT MIN value from Project INI file");
            }

            res = CL.EA.STBSettings.SetGuardTime(false, endGuardTimeName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Set Guard time");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();


            //*First Event *//
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Service_1.LCN);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EventBasedRec_1", NumOfPresses: 1, MinTimeBeforeEvStart: 5, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to book future event from Banner");
            }

            //*2ed Event *//
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Service_2.LCN);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EventBasedRec_2",NumOfPresses: 1,MinTimeBeforeEvStart:4,VerifyBookingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event2 From Banner ");
            }
            //*3ed Event *//
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Service_3.LCN);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EventBasedRec_3",NumOfPresses: 1,MinTimeBeforeEvStart:3,VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event3 From Banner ");
            }
            //*4fth Event *//
            //(Start time is a few minutes into the future, to make sure the recording will not be over before the booking sequence will complete);

            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRec_1", Convert.ToInt32(Service_4.LCN), DaysDelay: 0, MinutesDelayUntilBegining: 5, DurationInMin: 9, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Manual Recording");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_4.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + Service_4.LCN);
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //In the middle of the recordings, go into standby, stay for a while and come out
            res = CL.EA.WaitUntilEventStarts("EventBasedRec_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until EventBasedRec_1 Starts");
            }

            //Wait to be sure recording has started
            int Test_Wait = 60;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
            }

            //Wait for a while in Standby
            int Time_in_Standby = 120;
            CL.IEX.Wait(Time_in_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
            }

            //Make sure no EPG screen is on after exit standby
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after Standby");
            }

            //Wait For recordings to finish (including EGT)
            res = CL.EA.WaitUntilEventEnds("EventBasedRec_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until EventBasedRec_1 Ends");
            }

            res = CL.EA.WaitUntilEventEnds("EventBasedRec_2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until EventBasedRec_2 Ends");
            }

            res = CL.EA.WaitUntilEventEnds("EventBasedRec_3",EndGuardTime:endGuardTimeName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until EventBasedRec_1 Ends");
            }

            res = CL.EA.WaitUntilEventEnds("TimeBasedRec_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until TimeBasedRec_1 Ends");
            }


            /*Go into Recording list - All 4 recordings should appear in the recording list. Status For All events is Full.
            'Start Time and duration of Event-based events are correct (duration includes GTs - [4] minutes more than the Event Time, start Time is the SGT);
            'Start Time and duration of Time-based events are correct (as was entered in the recording request without any additions);*/


            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRec_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Item 'EventBasedRec_1' in Recording List");
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRec_2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Item 'EventBasedRec_2' in Recording List");
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventBasedRec_3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Item 'EventBasedRec_3' in Recording List");
            }

            res = CL.EA.PVR.VerifyEventInArchive("TimeBasedRec_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Find Item 'TimeBasedRec_1' in Recording List");
            }

            //Status For All events in PCAT should be Full
            res = CL.EA.PCAT.VerifyEventPartialStatus("TimeBasedRec_1", "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item TimeBasedRec_1 is Fully Recorded");
            }

            res = CL.EA.PCAT.VerifyEventPartialStatus("EventBasedRec_1", "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item EventBasedRec_1 is Fully Recorded");
            }

            res = CL.EA.PCAT.VerifyEventPartialStatus("EventBasedRec_2", "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item EventBasedRec_2 is Fully Recorded");
            }

            res = CL.EA.PCAT.VerifyEventPartialStatus("EventBasedRec_3", "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Item EventBasedRec_3 is Fully Recorded");
            }


            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Playback all recordings with regular playback and trick modes to reach the end


            res = CL.EA.PVR.PlaybackRecFromArchive("TimeBasedRec_1",SecToPlay: 0,FromBeginning: true,VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback TimeBasedRec_1");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("TimeBasedRec_1",Speed: 6,Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to FF TimeBasedRec_1 to EOF");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("EventBasedRec_1",SecToPlay: 0,FromBeginning: true,VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback EventBasedRec_1 ");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("EventBasedRec_1",Speed: 6,Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to FF EventBasedRec_1 to EOF ");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("EventBasedRec_2",SecToPlay: 0,FromBeginning: true,VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback EventBasedRec_2");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("EventBasedRec_2",Speed: 6,Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to FF EventBasedRec_2 to EOF ");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("EventBasedRec_3",SecToPlay: 0,FromBeginning: true,VerifyEOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback EventBasedRec_3 ");
            }
            res = CL.EA.PVR.SetTrickModeSpeed("EventBasedRec_3", Speed:6,Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to FF EventBasedRec_3 to EOF ");
            }

            PassStep();
        }

    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all records from Archive");
        }
    }
    #endregion
}