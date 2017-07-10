/// <summary>
///  Script Name : PVERR_0082_Resume_Reboot.cs
///  Test Name   : PVERR_0082_Resume_Reboot
///  TEST ID     : 71890
///  QC Version  : 2
///  Variations from QC: None
///  Repository: STB_DIVISION/Unified_ATP_For_HMD_Cable
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 26.11.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("PVERR_0082")]
public class PVERR_0082 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static _Platform GW;

    //Shared members between steps
    static Service recordChannel;
    static string isHomeClient;
    private static string resumableService;
    private static string eventToBeRecorded = "TIMEBASED_RECORDING"; //Event to be Recorded
    static string imageLoadDelay = "";
    static int delaytime = 0;
    static string standbyAfterBoot = "";


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Book a recording and reboot when the event is recording, verify if the event is recording after reboot";
    private const string STEP2_DESCRIPTION = "Step 2: wait till the recording is complete and verify the Recording Error Information";
    private const string STEP3_DESCRIPTION = "Step 3: Playback the Recorded Event";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();

        isHomeClient = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_HOMECLIENT");
        if (Convert.ToBoolean(isHomeClient))
        {
            GW = GetGateway();
        }

    }
    #endregion


    private static class Constant
    {
        public const double waitTimeInEvent = 120; // in secs
        public const int secToPlay = 0;
        public const bool fromBeginning = true;
        public const bool verifyEOF = false;
        public const bool sgtToSet = true;
        public const bool egtToSet = false;
    }

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

            resumableService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RESUMABLE_SERVICE");
            if (resumableService == "")
            {
                FailStep(CL, "Failed to fetch the resumable service from Test ini");
            }
            LogCommentInfo(CL, "Resumable service fetched from test ini is " + resumableService);
            //Get Values From xml File
            recordChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;LCN=" + resumableService, "ParentalRating=High");
            if (recordChannel == null)
            {
                FailStep(CL, "Failed to fetch Channel from Content.xml for the passed criterion");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + recordChannel.LCN);

            standbyAfterBoot = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "STANDBY_AFTER_REBOOT");
            if (standbyAfterBoot == "")
            {
                FailStep(CL, res, "Failed to fetch the stand by after reboot variable from Project INI file");
            }

            imageLoadDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "IMAGE_LOAD_DELAY_SEC");
            if (imageLoadDelay == "")
            {
                FailStep(CL, res, "Failed to fetch the load image load delay time from Project INI file");
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

           //Book future Event from Planner
            res = CL.EA.PVR.RecordManualFromPlanner(eventToBeRecorded, recordChannel.Name, DaysDelay: -1, MinutesDelayUntilBegining: 5, DurationInMin: 30);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record manual from planner");
            }

            //waiting till the event starts
            res = CL.EA.WaitUntilEventStarts(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait till the Booked Event Starts");
            }

            //waiting for 2 mins in event
            LogCommentInfo(CL, "Waiting for " + Constant.waitTimeInEvent + "  mins for some part of event to elapse");
            res = CL.IEX.Wait(Constant.waitTimeInEvent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for Event to elapse for " + Constant.waitTimeInEvent + " secs");
            }

            
            if (isHomeClient.ToUpper()!="TRUE")
            {
                //mount the box
                res = CL.EA.MountGw(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to reboot the Box");
                }
            }
            else
            {
                //mount the GW box
                res = GW.EA.MountGw(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to reboot the Gateway Box");
                }

                //mount the client box
                res = CL.EA.MountClient(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to reboot the client box");
                }

            }

            //converting the image load time to int for Wait 
            int.TryParse(imageLoadDelay, out delaytime);
            //Wait for some time for STB to come to standby mode 
            res = CL.IEX.Wait(seconds: delaytime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for STB to come to stand by");
            }

            if (Convert.ToBoolean(standbyAfterBoot))
            {
                //Navigate out of standby 
                res = CL.EA.StandBy(IsOn: true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit from stand by");
                }
            }

            //verifying event is recording after reboot
            res = CL.EA.PCAT.VerifyEventIsRecording(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify if the Event is recording");
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

            //wait till the recording is complete
            res = CL.EA.WaitUntilEventEnds(eventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the Event Ends");
            }

            //verifying record error Information
            res = CL.EA.PVR.VerifyRecordErrorInfo(eventToBeRecorded, EnumRecordErr.Partial_PowerFailure);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Record Error Information");
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
            //Playing back the recorded event
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, Constant.secToPlay, Constant.fromBeginning, Constant.verifyEOF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recorded event");
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

        //stoping the playback
        res = CL.EA.PVR.StopPlayback();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + " Failed to Stop Playback");
        }
        //Deleteing the recorded event
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + " Failed to Delete Recording from Archive");
        }

    }
    #endregion

}