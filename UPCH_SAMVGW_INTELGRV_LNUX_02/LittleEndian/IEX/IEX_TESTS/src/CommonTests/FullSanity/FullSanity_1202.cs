using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_1202 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the tes
    private static Service Service_1;
    private static string trickmodeFFList;
    private static string trickmodeREWList;
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Preconditions: Get Values from ini File & Book a recording and Wait until its completed");
        this.AddStep(new Step1(), "Step 1: Play back the recording and verify the EOF and BOF with all the available trick mode speeds");

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
            string StartGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
            if (StartGuardTimeName == null)
            {
                LogCommentFailure(CL, "Failed to get SGT Default value from Project INI file");
            }

            string endGuardTimeName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
            if (endGuardTimeName == null)
            {
                LogCommentFailure(CL, "Failed to get EGT Default value from Project INI file");
            }
            trickmodeFFList = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_FWD");
            if (trickmodeFFList == "")
            {
                FailStep(CL, res, "Failed to get the LIST_TM_FWD from project ini");
            }

            trickmodeREWList = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            if (trickmodeREWList == "")
            {
                FailStep(CL, res, "Failed to get the LIST_TM_FWD from project ini");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("TIME_BASED", Convert.ToInt32(Service_1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 3, DurationInMin: 12, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record Manual from Planner");
            }
            res = CL.EA.WaitUntilEventEnds("TIME_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event ends");
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
            string[] arrTrickmodeFFList=trickmodeFFList.Split(',');
            string[] arrTrickmodeREWList = trickmodeREWList.Split(',');
            foreach (string trickmode in arrTrickmodeFFList)
            {
                res = CL.EA.PVR.PlaybackRecFromArchive("TIME_BASED", SecToPlay: 0, FromBeginning: true, VerifyEOF: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to playback the record from Archive");
                }
                CL.IEX.Wait(120);
                if (trickmode == "30")
                {
                    CL.IEX.Wait(120);
                }
                res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED", Speed: Convert.ToDouble("-" + trickmode), Verify_EOF_BOF: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to set the trick mode speed to -"+trickmode);
                }
                res = CL.EA.PVR.SetTrickModeSpeed("TIME_BASED", Speed: Convert.ToDouble(trickmode), Verify_EOF_BOF: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to set the trick mode speed to "+trickmode);
                }
            }
            LogCommentImportant(CL,"Verified all the trick mode speeds");
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
            LogCommentFailure(CL, "Failed to delete all records from Archive");
        }
    }
    #endregion
}