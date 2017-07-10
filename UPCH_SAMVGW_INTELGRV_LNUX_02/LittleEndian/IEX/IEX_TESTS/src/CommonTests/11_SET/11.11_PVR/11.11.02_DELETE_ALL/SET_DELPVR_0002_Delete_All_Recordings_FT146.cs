/// <summary>
///  Script Name        : SET_DELPVR_0002_Delete_All_Recordings_FT146.cs
///  Test Name          : SET-DELPVR-0001-Delete_All_Option_Available_FT146, SET-DELPVR-0002-Delete_All_Recordings_FT146
///  TEST ID            : 74653,74654
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 4th Sept, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class FT146_0002 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    //Variables used
    private static string sgtFriendlyName;
    private static string egtFriendlyName;

    //Channels used by the test
    private static Service recordedEventBasedRecordingService;
    private static Service recordedKeepEventBasedRecordingService;
    private static Service recordedSeriesEventBasedRecordingService;
    private static Service recordedKeepSerieEventBasedRecordingService;
    private static Service recordedTimeBasedRecordingService;
    private static Service recordedKeepTimeBasedRecordingService;
    private static Service recordedRecurringTimeBasedRecordingService;
    private static Service recordedKeepRecurringTimeBasedRecordingService;
    
    
    private static string recordedEventBasedRecording="recordedEventBasedRecording";
    private static string recordedKeepEventBasedRecording="recordedKeepEventBasedRecording";
    private static string futureEventBasedRecording="futureEventBasedRecording";
    private static string recordedSeriesEventBasedRecording="recordedSeriesEventBasedRecording";
    private static string recordedKeepSerieEventBasedRecording="recordedKeepSerieEventBasedRecording";
    private static string futureSeriesEventBasedRecording="futureSeriesEventBasedRecording";
    private static string recordedTimeBasedRecording="recordedTimeBasedRecording";
    private static string recordedKeepTimeBasedRecording="recordedKeepTimeBasedRecording";
    private static string futureTimeBasedRecording = "futureTimeBasedRecording";
    private static string recordedRecurringTimeBasedRecording="recordedRecurringTimeBasedRecording";
    private static string recordedKeepRecurringTimeBasedRecording="recordedKeepRecurringTimeBasedRecording";
    private static string futureRecurringTimeBasedRecording = "futureRecurringTimeBasedRecording";

    static Helper helper = new Helper();


    #region Create Structure

    public override void CreateStructure()
    {
        //Variations from QualityCenter: None
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml file and set the recordings");
        this.AddStep(new Step1(), "Step 1: Navigate to Delete All and Select No and verify all the recordings");
        this.AddStep(new Step2(), "Step 2: Navigate to Delete All and Select Yes");
        this.AddStep(new Step3(), "Step 3: Verify that all the recordings are deleted except the Keep recordings and bookings");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();



            //Fetch the required services from Content xml
            recordedEventBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High");
            if (recordedEventBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedEventBasedRecordingService.LCN);
            }
            recordedKeepEventBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + recordedEventBasedRecordingService.LCN);
            if (recordedKeepEventBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedKeepEventBasedRecordingService.LCN);
            }

            recordedSeriesEventBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=True", "ParentalRating=High");
            if (recordedSeriesEventBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedSeriesEventBasedRecordingService.LCN);
            }
            recordedKeepSerieEventBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=True", "ParentalRating=High;LCN=" + recordedSeriesEventBasedRecordingService.LCN);
            if (recordedKeepSerieEventBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedKeepSerieEventBasedRecordingService.LCN);
            }

            recordedTimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + recordedEventBasedRecordingService.LCN + "," + recordedKeepEventBasedRecordingService.LCN);
            if (recordedTimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedTimeBasedRecordingService.LCN);
            }


            recordedKeepTimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + recordedEventBasedRecordingService.LCN + "," + recordedKeepEventBasedRecordingService.LCN + "," + recordedTimeBasedRecordingService.LCN);
            if (recordedKeepTimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedKeepTimeBasedRecordingService.LCN);
            }

            recordedRecurringTimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + recordedEventBasedRecordingService.LCN + "," + recordedKeepEventBasedRecordingService.LCN + "," + recordedTimeBasedRecordingService.LCN + "," + recordedKeepTimeBasedRecordingService.LCN);
            if (recordedRecurringTimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedRecurringTimeBasedRecordingService.LCN);
            }
            recordedKeepRecurringTimeBasedRecordingService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;IsSeries=False", "ParentalRating=High;LCN=" + recordedEventBasedRecordingService.LCN + "," + recordedKeepEventBasedRecordingService.LCN + "," + recordedTimeBasedRecordingService.LCN + "," + recordedKeepTimeBasedRecordingService.LCN + "," + recordedRecurringTimeBasedRecordingService.LCN);
            if (recordedKeepRecurringTimeBasedRecordingService == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + recordedKeepRecurringTimeBasedRecordingService.LCN);
            }

            //Setting the SGT and EGT to minimum and setting those values
            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, "sgtFriendlyName is not defined in the projrct ini file");
            }

            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MIN");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, "egtFriendlyName is not defined in the projrct ini file");
            }

            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordedEventBasedRecordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+recordedEventBasedRecordingService.LCN);
            }

            //DO the required recordings and bookings

            res = CL.EA.PVR.RecordCurrentEventFromBanner(recordedEventBasedRecording,VerifyIsRecordingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record current event from banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordedKeepEventBasedRecordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordedKeepEventBasedRecordingService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner(recordedKeepEventBasedRecording,VerifyIsRecordingInPCAT:false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordedKeepSerieEventBasedRecordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordedKeepSerieEventBasedRecordingService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner(recordedKeepSerieEventBasedRecording, IsSeries: false, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordedSeriesEventBasedRecordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordedSeriesEventBasedRecordingService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner(recordedSeriesEventBasedRecording, IsSeries: true, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }

            //Waiting until the Event ends as the number of recordings which can be done at a time is only four so waiting till it completes and booking others
            res = CL.EA.WaitUntilEventEnds(recordedEventBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event ends");
            }

            res = CL.EA.PVR.RecordManualFromCurrent(recordedTimeBasedRecording, recordedTimeBasedRecordingService.LCN,DurationInMin:3, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event");
            }

            res = CL.EA.WaitUntilEventEnds(recordedKeepEventBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }
            res = CL.EA.PVR.RecordManualFromCurrent(recordedKeepTimeBasedRecording, recordedKeepTimeBasedRecordingService.LCN,DurationInMin:3, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event");
            }
            res = CL.EA.WaitUntilEventEnds(recordedKeepSerieEventBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }
            res = CL.EA.PVR.RecordManualFromCurrent(recordedRecurringTimeBasedRecording, recordedRecurringTimeBasedRecordingService.LCN, DurationInMin: 3, VerifyBookingInPCAT: false, Frequency: EnumFrequency.WEEKLY);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event");
            }
            res = CL.EA.WaitUntilEventEnds(recordedSeriesEventBasedRecording);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }

            res = CL.EA.PVR.RecordManualFromCurrent(recordedKeepRecurringTimeBasedRecording, recordedKeepRecurringTimeBasedRecordingService.LCN,DurationInMin:3, VerifyBookingInPCAT: false, Frequency: EnumFrequency.WEEKLY);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event");
            }

            res = CL.EA.PVR.RecordManualFromPlanner(futureTimeBasedRecording, recordedKeepTimeBasedRecordingService.Name,DaysDelay:-1, MinutesDelayUntilBegining: 60, DurationInMin: 2, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner");
            }

            res = CL.EA.PVR.RecordManualFromPlanner(futureRecurringTimeBasedRecording, recordedRecurringTimeBasedRecordingService.Name,DaysDelay:-1, MinutesDelayUntilBegining: 60, DurationInMin: 2, VerifyBookingInPCAT: false, Frequency: EnumFrequency.WEEKLY);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record Manual from Planner");
            }
			
            res = CL.EA.PVR.BookFutureEventFromGuide(futureEventBasedRecording, recordedEventBasedRecordingService.LCN, NumberOfPresses: 5, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event from guide");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide(futureSeriesEventBasedRecording, recordedSeriesEventBasedRecordingService.LCN, NumberOfPresses: 3, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event from guide");
            }
            res=CL.EA.WaitUntilEventEnds(recordedKeepRecurringTimeBasedRecording);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until Event Ends");
            }
            //Insert into event collection by changing the series to false so that verifying the event in archive will be easy
            CL.EA.UI.Utils.InsertEventToCollection(recordedSeriesEventBasedRecording, "", "", "", "", "", 0, 0, "", "", 0, "", IsSeries: false,IsModify:true);
            CL.EA.UI.Utils.InsertEventToCollection(recordedKeepSerieEventBasedRecording, "", "", "", "", "", 0, 0, "", "", 0, "", IsSeries: false, IsModify: true);
            //Set the keep flags
            res = CL.EA.PVR.SetKeepFlag(recordedKeepEventBasedRecording, SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Keep flag to true");
            }
            res = CL.EA.PVR.SetKeepFlag(recordedKeepRecurringTimeBasedRecording, SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Keep flag to true");
            }
            res = CL.EA.PVR.SetKeepFlag(recordedKeepSerieEventBasedRecording, SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Keep flag to true");
            }
            res = CL.EA.PVR.SetKeepFlag(recordedKeepTimeBasedRecording, SetKeep: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the Keep flag to true");
            }
            //Verifying all the events in Archive and passing true as an argument will verify all the events in Archive
            if (!helper.VerifyEventsInArchiveAndPlanner(true))
            {
                LogCommentFailure(CL, "Please check the above failures");
            }
           


            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Navigate to delete all and select NO
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DELETE ALL NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed ot Navigate to Delete all No");
            }

            LogCommentImportant(CL,"Selected no for confirmation of DELETE ALL");

            //Verifying all the events in Archive and passing true as an argument will verify all the events in Archive
            if (!helper.VerifyEventsInArchiveAndPlanner(true))
            {
                LogCommentFailure(CL, "Please check the above failures");
            }
            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Delete all the records from Archive
            res = CL.EA.PVR.DeleteAllRecordsFromArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to delete all the records from Archive");
            }
          

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Verifies all the recordings are deleted except the keep recordings and bookings
            if (!helper.VerifyEventsInArchiveAndPlanner(false))
            {
                LogCommentFailure(CL, "Please check the above failures");
            }

            PassStep();
        }
    }

    #endregion Step3



    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteRecordFromArchive(recordedKeepEventBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(recordedKeepRecurringTimeBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(recordedKeepSerieEventBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete record from Archive");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(recordedKeepTimeBasedRecording);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete record from Archive");
        }
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to cancel all bokings from Planner");
        }
       
    }

    #endregion PostExecute
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Verifies the Recordings in My Recordings and bookings in My Planner
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifyEventsInArchiveAndPlanner(bool isNotDeleted)
        {
            IEXGateway._IEXResult res;
            bool isPass = true;
            res = CL.EA.PVR.VerifyEventInArchive(recordedEventBasedRecording, SupposedToFindEvent: isNotDeleted);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedEventBasedRecording + " Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedKeepEventBasedRecording, Navigate: false, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedKeepEventBasedRecording + " Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedSeriesEventBasedRecording, Navigate: false, SupposedToFindEvent: isNotDeleted);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedSeriesEventBasedRecording + " Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedKeepSerieEventBasedRecording, Navigate: false, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedKeepSerieEventBasedRecording + " Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedTimeBasedRecording, Navigate: false, SupposedToFindEvent: isNotDeleted);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedTimeBasedRecording + " Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedKeepTimeBasedRecording, Navigate: false, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedKeepTimeBasedRecording + " Event in Archive");
            }
            res = CL.EA.PVR.VerifyEventInArchive(recordedRecurringTimeBasedRecording, Navigate: false, SupposedToFindEvent: isNotDeleted);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedRecurringTimeBasedRecording + " Event in Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive(recordedKeepRecurringTimeBasedRecording, Navigate: false, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify" + recordedKeepRecurringTimeBasedRecording + " Event in Archive");
            }


            //Verifying the booking in planner
            res = CL.EA.PVR.VerifyEventInPlanner(futureEventBasedRecording, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify event in planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(futureSeriesEventBasedRecording, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify event in planner");
            }
            res = CL.EA.PVR.VerifyEventInPlanner(futureTimeBasedRecording, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify event in planner");
            }

            res = CL.EA.PVR.VerifyEventInPlanner(futureRecurringTimeBasedRecording, SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to verify event in planner");
            }

            return isPass;
        }
    }
    #endregion
}
