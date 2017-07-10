/// <summary>
///  Script Name : PVREPG_0851_RECkey_Future_ActMenu.cs
///  Test Name   : PVREPG-0851-REC key Record-Future-Action Menu
///  TEST ID     : 74652
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 11/09/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PVREPG_0851 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service recordingService;
    private static String Milestones = "";

    private static class Constants
    {
        public const int timeToPressKey = -1;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: Book a future event, from Channel Bar next using REC key
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync,Tune to service S1");
        this.AddStep(new Step1(), "Step 1: Navigate to Channel Bar Next, press Select and then press REC key for confirm booking");

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

            //Get Values From xml File
            recordingService = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "IsDefault=True;ParentalRating=High;IsSeries=True");
            if (recordingService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            Milestones = CL.EA.UI.Utils.GetValueFromMilestones("RecordFutureEvent");

            //Tune to channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + recordingService.LCN);
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
            string timeStamp = "";
            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();


            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR NEXT");

            //Press Record Key
            res = CL.IEX.SendIRCommand("REC", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send STOP key to stop recording");
            }

            //Verifying for CONFIRM RECORDING state
            if (!(CL.EA.UI.Utils.VerifyState("CONFIRM RECORDING")))
            {
                FailStep(CL, res, "Failed To Verify State Is CONFIRM RECORDING");
            }

            //Focus is on CONFIRM RECORDING
            res = CL.IEX.MilestonesEPG.SelectMenuItem("CONFIRM RECORDING");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to highlight menu item CONFIRM RECORDING");
            }
           
            //Begin wait for FAS log validation for future booking
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, 10);

            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPressKey, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book event");
            }

            //End wait for FAS log validation for future booking
            if (!(CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref ActualLines)))
            {
                FailStep(CL, res, "Failed to verify FAS log :" + Milestones);
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
        res = CL.EA.PVR.CancelAllBookingsFromPlanner();
        if (!res.CommandSucceeded)
        { 
            LogCommentFailure(CL,"Failed to all booking because" + res.FailureReason);
        }
    }

    #endregion PostExecute
}