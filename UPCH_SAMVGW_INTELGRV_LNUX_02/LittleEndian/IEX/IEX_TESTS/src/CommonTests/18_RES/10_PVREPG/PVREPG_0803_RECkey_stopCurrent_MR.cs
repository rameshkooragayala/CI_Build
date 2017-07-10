/// <summary>
///  Script Name : PVREPG_0803_RECkey_stop_Current_MR
///  Test Name   : PVREPG-0803-REC key stop-Current-MR
///  TEST ID     : 
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 03/09/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0803 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService1 = new Service();
    private static String stopScreen = "";
    private static EnumRecordIn stopIn;


    #region Create Structure

    public override void CreateStructure()
    {
        
        //Pre-conditions: There is currently a record on-going on channel S1 and also future event is booked
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Sync,Tune to service S1, start Manual recording on service S1 include both Now and Next event");
        this.AddStep(new Step1(), "Step 1: Stop recording by pressing STOP key from Channel Bar, Live and Guide");

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

            stopScreen = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "STOP_SCREEN");

            stopIn = (EnumRecordIn)Enum.Parse(typeof(EnumRecordIn), stopScreen, true);

            //Get Values From xml File, matched with +ve and -ve criteria
            recordingService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video;IsConstantEventDuration=True;IsMinEventDuration=True", "IsDefault=True;ParentalRating=High");
            if (recordingService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            int Duration = Convert.ToInt32(recordingService1.EventDuration);

            res = CL.EA.STBSettings.SetGuardTime(true, "NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to: " + "NONE");
            }

            res = CL.EA.STBSettings.SetGuardTime(false, "NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + "NONE");
            }

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + recordingService1.LCN);
            }

            CL.EA.UI.Banner.GetEventTimeLeft(ref evtEndLeftTime);

            if (evtEndLeftTime <= 300)
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

            res = CL.EA.PVR.RecordManualFromCurrent("ManualRecord", recordingService1.LCN , Duration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Event");
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

            //Stop recording using STOP key
            res = CL.EA.PVR.StopRecordUsingStopKey(stopIn, "ManualRecord", recordingService1.LCN, IsTBR: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from "+ stopScreen);
            }

            PassStep();
        }
    }

    #endregion Step1

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