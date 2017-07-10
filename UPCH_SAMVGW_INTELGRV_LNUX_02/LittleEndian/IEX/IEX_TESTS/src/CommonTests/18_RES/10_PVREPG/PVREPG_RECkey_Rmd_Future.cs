/// <summary>
///  Script Name : PVREPG_RECkey_Rmd_Future
///  Test Name   : PVREPG-RECkey reminder-Future
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 26/08/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_RECKey_Rmd : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();
    private static String recordScreen = "";
    private static EnumRecordIn recordIn;



    private static class Constants
    {
        public const int minTimeForEventStart = 4;
        public const int numOfPresses = 1;
        public const int minTimeForEventToEnd = 2;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        
        //Pre-conditions: There is currently a record on-going on channel S1 and also future event is booked
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1, Book a reminder for future event");
        this.AddStep(new Step1(), "Step 1: Book future recording by pressing REC key from Channel Bar and Guide ");
        this.AddStep(new Step2(), "Step 2: Verify booked event is in Planner");

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
            
            int evtEndLeftTime = 0;

            //Get Values From xml File, +ve criteria as IsEIT, video
            recordingService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video", "IsDefault=True;ParentalRating=High");
            if (recordingService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            recordScreen = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "RECORD_SCREEN");

            recordIn = (EnumRecordIn)Enum.Parse(typeof(EnumRecordIn), recordScreen, true);

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
            }

            CL.EA.UI.Banner.GetEventTimeLeft(ref evtEndLeftTime);

            if (evtEndLeftTime <= 240)
            {
                LogComment(CL, "Returning to Live viewing from Action Bar Launched During GetCurrentEventLeftTime");
                res = CL.EA.ReturnToLiveViewing();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed To Return to Live Viewing");
                }

                LogComment(CL, "Wait till event end");
                CL.IEX.Wait(evtEndLeftTime + 60);

                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:LIVE");

            }

            // Book reminder for future event
            res = CL.EA.BookReminderFromGuide("ReminderEvent", recordingService1.LCN, Constants.numOfPresses, Constants.minTimeForEventStart);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Reminder for event on service " + recordingService1.LCN);
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

            //Book recording on future event
            res = CL.EA.PVR.RecordUsingRECkey(recordIn, "BookEvent", recordingService1.LCN, Constants.minTimeForEventToEnd, IsCurrent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book future event");
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

            //Verify Event is booked and present in Planner
            res = CL.EA.PVR.VerifyEventInPlanner("BookEvent", SupposedToFindEvent: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that booked event is cancel");
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