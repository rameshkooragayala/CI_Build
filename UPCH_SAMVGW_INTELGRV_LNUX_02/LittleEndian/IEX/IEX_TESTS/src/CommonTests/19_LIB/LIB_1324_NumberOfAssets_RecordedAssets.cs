/// <summary>
///  Script Name : LIB_1324_NumberOfAssets_RecordedAssets
///  Test Name   : LIB-1324-Video-Lib-number-of-assets-Recorded-assets
///  TEST ID     : 23514
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : MadhuKumar K
///  Modified Date: 23rd Sept, 2014
/// </summary>

using System;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;


public class LIB_1324 : _Test
{
    [ThreadStatic]
    private static _Platform CL;


    //Constants to be used in the test case
    private static Service recordableService;
    
    private static Service recordableService1;

    private static Service recordableService2;

    private static Service recordableService3;

    private static class Constants
    {    
        public const int totalNumberOfRecordings = 4;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps
        this.AddStep(new PreCondition(), "Precondition:  Get Channel Number. Have Event Based and Time Based Recordings in Library");
        this.AddStep(new Step1(), "Step 1: Navigate to library and verify the total number of assets and highlighted asset number from EPG");

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
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService.LCN);
            }
            recordableService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService.LCN);
            if (recordableService1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService1.LCN);
            }
            recordableService2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService.LCN + "," + recordableService1.LCN);
            if (recordableService2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService2.LCN);
            }
            recordableService3 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High;LCN=" + recordableService.LCN + "," + recordableService1.LCN + "," + recordableService2.LCN);
            if (recordableService3 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + recordableService3.LCN);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+recordableService.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording", VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record current event from banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService1.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording1", VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService2.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording2", VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordableService3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + recordableService3.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording3", VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }
            res = CL.EA.PVR.StopRecordingFromArchive("EventBasedRecording");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to stop record from Archive");
            }
            CL.IEX.Wait(3);
            res = CL.EA.PVR.StopRecordingFromArchive("EventBasedRecording1",Navigate:false);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop record from Archive");
            }
            CL.IEX.Wait(3);
            res = CL.EA.PVR.StopRecordingFromArchive("EventBasedRecording2", Navigate: false);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop record from Archive");
            }
            CL.IEX.Wait(3);
            res = CL.EA.PVR.StopRecordingFromArchive("EventBasedRecording3", Navigate: false);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to stop record from Archive");
            }
            CL.IEX.Wait(3);

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
            res = CL.EA.PVR.NavigateToArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to Archive");
            }

            for (int count = 1; count <= Constants.totalNumberOfRecordings; count++)
            {
                string recordPosition = "";
                res = CL.IEX.MilestonesEPG.GetEPGInfo("recordposition", out recordPosition);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the EPG info");
                }
                string[] recordPositionarr = recordPosition.Split('/');
                if (Convert.ToInt32(recordPositionarr[0]) != count)
                {
                    FailStep(CL, "After Navigating to Library record position is not "+count+" which is expected");
                }
                else
                {
                    LogCommentImportant(CL, "Record Position is "+count+" after navigating");
                }
                if (Convert.ToInt32(recordPositionarr[1]) != Constants.totalNumberOfRecordings)
                {
                    FailStep(CL, "After Navigating to Library total number of recordings are not 4 which are recorded");
                }
                else
                {
                    LogCommentImportant(CL, "Total number of recordings are 4 after entering the Library");
                }
                string timeStamp="";
                res = CL.IEX.SendIRCommand("SELECT_DOWN",-1,ref timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to send IR select down");
                }
                CL.IEX.Wait(3);
            }
            
            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}