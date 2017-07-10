/// <summary>
///  Script Name : PVREPG_StopKey
///  Test Name   : PVREPG-0800, 0801, 0802, 0810 and 0811
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 25/08/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_StopKey : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();
    private static String stopScreen = "";
    private static EnumRecordIn stopIn;



    private static class Constants
    {
        public const int minTimeForEventToEnd = 4;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        
        //Pre-conditions: There is currently a record on-going on channel S1 and also future event is booked
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1, start event based recording on service S1 and Book a future event");
        this.AddStep(new Step1(), "Step 1: Stop recording by pressing STOP key from Channel Bar, Live and Guide ");
        this.AddStep(new Step2(), "Step 2: Cancel booking by pressing STOP key from Channel Bar and Guide");

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

            //Get Values From xml File, +ve criteria as IsEIT, video
            recordingService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video", "IsDefault=True;ParentalRating=High");
            if (recordingService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            stopScreen = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "STOP_SCREEN");

            stopIn = (EnumRecordIn)Enum.Parse(typeof(EnumRecordIn), stopScreen, true);

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
            }

            //Start recording an event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("CurrentEveRecord", Constants.minTimeForEventToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Event");
            }

            //Booking a future event
            res = CL.EA.PVR.BookFutureEventFromBanner("FutureEvtBooked", Constants.minTimeForEventToEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event");
            }
			
			CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:LIVE");

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

            //Stop recording using STOP key
            res = CL.EA.PVR.StopRecordUsingStopKey(stopIn, "CurrentEveRecord", recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from " + stopScreen);
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

            if (stopIn.Equals(EnumRecordIn.ChannelBar) || stopIn.Equals(EnumRecordIn.Guide))
            {
                //Stop recording using STOP key
                res = CL.EA.PVR.StopRecordUsingStopKey(stopIn, "FutureEvtBooked", recordingService1.LCN, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to stop recording from " + stopScreen);
                }

                res = CL.EA.PVR.VerifyEventInPlanner("FutureEvtBooked", SupposedToFindEvent: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify that booked event is cancel");
                }
            }

            else
            {
                LogCommentImportant(CL, "Cancel Booking is not possible from " + stopScreen);
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Delete All Recording from Archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to Delete recording because" + res.FailureReason);
        }
    }

    #endregion PostExecute
}