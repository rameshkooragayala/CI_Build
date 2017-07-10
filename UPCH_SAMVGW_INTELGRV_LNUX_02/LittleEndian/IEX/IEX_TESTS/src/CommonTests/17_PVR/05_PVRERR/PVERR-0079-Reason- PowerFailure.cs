/// <summary>
///  Script Name : PVERR-0079-Reason- PowerFailure.cs
///  Test Name   : PVERR-0079-Reasons - Power failure
///  TEST ID     : 71512
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 31.10.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("PVRERR_0079")]
public class PVRERR_0079 : _Test
{
    [ThreadStatic]

    static _Platform CL;
    static _Platform GW;


    //Shared members between steps
    static Service recordChannel;

    static string isHomeClient;

    static string startGuardTime;

    static string endGuardTime;

    static string imageLoadDelay = "";

    static int delaytime = 0;

    static string standbyAfterBoot = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Book a future recording and Power Off the Box and Power On after the completion of the booked event";
    private const string STEP2_DESCRIPTION = "Step 2: Verify the Recording Error Information";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

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
        public const int noOfPresses = 1;
        public const bool isFailedRecord = true;
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

            //Get Values From xml File
            recordChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
            if (recordChannel == null)
            {
                FailStep(CL, res, "Failed to get Channel from Content.xml for the passed criterion");
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

            //get value from project.ini for EGT
            string startGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "LIST");
            if (string.IsNullOrEmpty(startGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }


            startGuardTime = startGuardTimeList.Split(',').First();

            //set EGT to first value of list
            res = CL.EA.STBSettings.SetGuardTime(true, startGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Start Guard Time to " + startGuardTime);
            }

            //get value from project.ini for EGT
            string endGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "LIST");
            if (string.IsNullOrEmpty(endGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }


            endGuardTime = endGuardTimeList.Split(',').First();

            //set EGT to first value of list
            res = CL.EA.STBSettings.SetGuardTime(false, endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set End Guard Time to " + endGuardTime);
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

            string currentTime = "";
            string evtEndTime = "";
            double timeToWait = 0;
            //get friendly name for SGT
            int startGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(startGuardTime, true);

            //Book future Event from Guide
            res = CL.EA.PVR.BookFutureEventFromGuide("recEvent", recordChannel.LCN, Constant.noOfPresses, startGuardTimeNum + 2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book event from Guide");
            }

            //get friendly name  for end guard time
            int endGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(endGuardTime, false);

            //navigate to Main Menu to update the EPG Milestone
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Main Menu");
            }

            //Get Current EPG Time
            CL.EA.UI.Live.GetEpgTime(ref currentTime);

            //get the booked event end time
            evtEndTime = CL.EA.GetEventInfo("recEvent", EnumEventInfo.EventEndTime);

            //time to wait in power loss
            timeToWait = (Convert.ToDateTime(evtEndTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds;
            //turn off the Power
            res = CL.IEX.Power.TurnOFF();

            //checking if home client box
            if (!Convert.ToBoolean(isHomeClient))
            {

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to turn off the power");
                }

                LogCommentInfo(CL, "Waiting for " + timeToWait.ToString() + " secs in power loss");
                //wait in Power loss for the booked event to end
                res = CL.IEX.Wait(timeToWait);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till the booked event is finished");
                }

                LogCommentInfo(CL, "Waiting for " + endGuardTimeNum + " mins for EGT to complete");
                //waiting for end Guard time to end 
                res = CL.IEX.Wait(Convert.ToDouble(endGuardTimeNum) * 60);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till the end guard time is completed");
                }
                // Mount the Box 
                res = CL.EA.MountGw(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to mount the box");
                }

            }
            else
            {
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to turn off the power of the client box");
                }
                //turn off the power for Gateway box if its home client
                res = GW.IEX.Power.TurnOFF();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to turn off the power of gateway box");
                }

                LogCommentInfo(CL, "Wait for " + timeToWait.ToString() + " secs in power loss");
                //wait in power loss
                res = CL.IEX.Wait(timeToWait);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till the booked event is finished");
                }

                LogCommentInfo(CL, "Waiting for " + endGuardTimeNum + " mins for EGT to complete");
                //waiting for end Guard time to end 
                res = CL.IEX.Wait(Convert.ToDouble(endGuardTimeNum) * 60);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till the end guard time is completed");
                }

                //mount gateway box
                res = GW.EA.MountGw(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(GW, res, "Failed to mount the box");
                }
                //mount the client box
                res = CL.EA.MountClient(EnumMountAs.NOFORMAT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to mount the box");
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
		       //Wait for some time after the STB came to LIVE
               res = CL.IEX.Wait(seconds: 10);
               if (!res.CommandSucceeded)
               {
                   FailStep(CL, res, "Failed to wait for few seconds after the STB came to LIVE");
               }
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
            //verify the Record error information
            res = CL.EA.PVR.VerifyRecordErrorInfo("recEvent",EnumRecordErr.Failed_PowerFailure);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the recorded error info");
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

        //delete the failed recorded event
        res = CL.EA.PVR.DeleteFailedRecordedEvent("recEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Failed Recorded Event");
        }
        
        //get default value from project.ini for EGT
        string defaultStartGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultStartGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }

        //set SGT to default
        res = CL.EA.STBSettings.SetGuardTime(true, defaultStartGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultStartGuardTime);
        }

        //get default value from project.ini for EGT
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }

        //set SGT to default
        res = CL.EA.STBSettings.SetGuardTime(false, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }

    }
    #endregion

}