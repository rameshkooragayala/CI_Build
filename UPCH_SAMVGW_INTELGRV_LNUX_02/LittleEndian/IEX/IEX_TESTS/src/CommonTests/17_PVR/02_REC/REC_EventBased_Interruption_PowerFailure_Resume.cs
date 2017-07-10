/// <summary>
///  Script Name : REC_EventBased_Interruption_PowerFailure_Resume.cs
///  Test Name   : REC-0200-Event Based - Interruption - power failure,REC-0230-Event Based - Interruption - recording resume - power failure
///  TEST ID     : 73817,73818
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : FR_FUSION/UPC
/// -----------------------------------------------
///  Modified by : MadhuKumar K
///  Modified Date: 26th Mar, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_PowerFailure : _Test
{
    [ThreadStatic]
    private static _Platform CL,GW;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    private static string resumableService;
    private static string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded
    private static int startGuardTimeInt = 0;
    private static int endGuardTimeInt = 0;
    private static string sgtFriendlyName;
    private static string egtFriendlyName;
    private static double timeToWait = 0;//Total time to wait in Stand By
    static string imageLoadDelay = "";
    static int delaytime = 0;
    static string standbyAfterBoot = "";
    private static string resumeAfterPowerLoss;//Whether to resume after signal loss or not which will be fetched from Test Ini
    static Boolean isHomeNetwork;

    //Constants used in the test
    private static class Constants
    {
        public const double waitInPowerLoss = 60;//in Secs
        public const double waitInEventBody = 60;//in Secs
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Fetch Service from COntent XML and Set GT's and Book a future Recording");
        this.AddStep(new Step1(), "Step 1: Wait for the recording to Start");
        this.AddStep(new Step2(), "Step 2: During Recording Power Off STB");
        this.AddStep(new Step3(), "Step 3: Power on the STB while in EGT/Event Body of that particular Recording");
        this.AddStep(new Step4(), "Step 4: Verify the Duration and Playback the recording from Archive");
        //Get Client Platform
        CL = GetClient();
		isHomeNetwork = Convert.ToBoolean(CL.EA.GetTestParams("IsHomeNetwork"));
        if (isHomeNetwork)
        {
             GW = GetGateway();
        }
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            resumeAfterPowerLoss = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RESUME_AFTER_POWERLOSS");
            if (resumeAfterPowerLoss == "")
            {
                FailStep(CL, "Resume after Power loss parameter is not defined in the Test ini");
            }
            LogCommentInfo(CL, "Resumable after power loss fetched from test ini is " + resumeAfterPowerLoss);
            //If resume after power loss is true then we are fetching the service number from test ini because in UPC there is an issue with the recorded stream where they are not resuming after power cycle so definging it in Test ini
            if (resumeAfterPowerLoss.ToUpper() == "TRUE")
            {
                resumableService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RESUMABLE_SERVICE");
                if (resumableService == "")
                {
                    FailStep(CL, "Failed to fetch the resumable service from Test ini");
                }
                LogCommentInfo(CL, "Resumable service fetched from test ini is " + resumableService);

                //Fetcing a recordable service
                recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;LCN=" + resumableService, "ParentalRating=High");
                if (recordableService == null)
                {
                    FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
                }
                else
                {
                    LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
                }

            }
            else
            {
                //Fetcing a recordable service
                recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True", "ParentalRating=High");
                if (recordableService == null)
                {
                    FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
                }
                else
                {
                    LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
                }
            }


            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch the sgt friendly namee from Project INI file");
            }

            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");
            if (egtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch the EGT freindly name from Project INI file");
            }

            LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);

            LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);


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

            res = CL.EA.PVR.BookFutureEventFromGuide(eventToBeRecorded, recordableService.LCN, MinTimeBeforeEvStart: startGuardTimeInt);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book future Event from Guide");
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
            res = CL.EA.WaitUntilEventStarts(eventToBeRecorded,StartGuardTime:sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until Event Starts");
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
            if (resumeAfterPowerLoss.ToUpper() == "TRUE")
            {
                //Converting start guard time to seconds and waiting few seconds to wait into event body
                timeToWait = startGuardTimeInt * 60 + Constants.waitInEventBody;
                LogCommentInfo(CL, "Waiting for " + timeToWait + " seconds into the recording");
                //waiting for event to end
                res = CL.IEX.Wait(seconds: timeToWait);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait till the event ends");
                }
                timeToWait = Constants.waitInPowerLoss;

            }
            else
            {
                //Refreeshing the EPG to get the Current time from EPG
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to main menu", exitTest: false);
                }
                string currentTime = "";
                //Getting Event info from the event collection
                string evtEndTime = CL.EA.GetEventInfo(eventToBeRecorded, EnumEventInfo.EventEndTime);
                if (string.IsNullOrEmpty(evtEndTime))
                {
                    FailStep(CL, "Retrieved end time from event info is null");
                }
                LogComment(CL, "Event End time is " + evtEndTime);

                //Get Current EPG Time
                CL.EA.UI.Live.GetEpgTime(ref currentTime);
                if (string.IsNullOrEmpty(currentTime))
                {
                    FailStep(CL, "Failed to Get the EPG time from LIVE");
                }

                LogComment(CL, "Current time is " + currentTime);
                //calculating time to wait in Stand By as we cant use Wait until Event Ends in Stand By
                timeToWait = (Convert.ToDateTime(evtEndTime).Subtract(Convert.ToDateTime(currentTime))).TotalSeconds;
                LogComment(CL, "Time to wait is " + timeToWait);
            }
			
			 if (isHomeNetwork)
            {
              
                res = GW.EA.PowerCycle(SecBeforePowerOn: Convert.ToInt32(timeToWait), GetOutOfStandBy:true, FormatSTB: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to complete a power cycle");
                }

               
            }

            res = CL.EA.PowerCycle(SecBeforePowerOn: Convert.ToInt32(timeToWait), FormatSTB: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to complete a power cycle");
            }

            //converting the image load time to int for Wait 
            int.TryParse(imageLoadDelay, out delaytime);
            //Wait for some time for STB to come to standby mode 
            res = CL.IEX.Wait(seconds:delaytime);
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
            if (resumeAfterPowerLoss.ToUpper() == "TRUE")
            {
                //Verifying that the recording is resumed after the power cycle and waiting until the event ends
                res = CL.EA.PCAT.VerifyEventIsRecording(eventToBeRecorded);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify that the event is recording");
                }
                res = CL.EA.WaitUntilEventEnds(eventToBeRecorded, EndGuardTime: egtFriendlyName);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait until event ends");
                }
            }
            else
            {
                //Verifying that the event is not resumed after the power cycle in EGT
                res = CL.EA.PCAT.VerifyEventIsRecording(eventToBeRecorded);
                if (res.CommandSucceeded)
                {
                    FailStep(CL, res, "Event is resumed after signal loss");
                }
                else
                {
                    LogCommentImportant(CL, "Event is not resumed");
                }
            }

            //verify record error information
            res = CL.EA.PVR.VerifyRecordErrorInfo(eventToBeRecorded, EnumRecordErr.Partial_PowerFailure);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the recording error information");
            }
            PassStep();
        }
    }

    #endregion Step3
    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            if (resumeAfterPowerLoss.ToUpper() == "TRUE")
            {

                string actualDuration = "";

                CL.EA.PCAT.GetEventInfo(eventToBeRecorded, EnumPCATtables.FromRecordings, FieldName: "ACTUAL_DURATION", ReturnedValue: ref actualDuration);
                if (String.IsNullOrEmpty(actualDuration))
                {
                    FailStep(CL, "Failed to retrieve the Actual Duration from PCAT");
                }

                //Converting Actual duration from milliseconds to min 
                double actualDurationInMin = Convert.ToInt32(actualDuration) / (1000 * 60);

                LogComment(CL, "Actual Duration in Min is " + actualDurationInMin);
                //Actual Duration should be greater then that of the combination of event duration and Start Guard time
                if (actualDurationInMin < (Convert.ToDouble(recordableService.EventDuration) + startGuardTimeInt))
                {
                    FailStep(CL, "Event Duration " + actualDurationInMin + " is lesser then expected " + ((Convert.ToDouble(recordableService.EventDuration) + startGuardTimeInt)));
                }
            }
            //Playing back the record from Archive and Verifying the EOF
            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, SecToPlay: 0, FromBeginning: true, VerifyEOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the record from Archive");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //delete the failed recorded event
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
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

    #endregion PostExecute
}