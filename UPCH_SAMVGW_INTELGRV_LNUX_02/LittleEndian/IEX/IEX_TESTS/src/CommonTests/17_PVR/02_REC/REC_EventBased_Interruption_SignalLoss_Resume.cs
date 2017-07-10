/// <summary>
///  Script Name : REC_EventBased_Interruption_SignalLoss_Resume.cs
///  Test Name   : REC-0201-Event Based - Interruption - signal loss,REC-0231-Event Based - Interruption - recording resume - signal loss
///  TEST ID     : 73736,73737
///  QC Version  : 2
///  Variations from QC:NONE
///  Repository  : Unified_ATP_For_HMD_Cable
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 13th Mar, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_SignalLoss : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service recordableService; //The service where recordings will happen
    //Variables which are used in different steps
    private static string eventToBeRecorded = "EVENT_RECORDING"; //Event to be Recorded
    private static int startGuardTimeInt = 0;
    private static int endGuardTimeInt = 0;
    private static string sgtFriendlyName;
    private static string egtFriendlyName;
    private static double timeToWait = 0;//Total time to wait in Stand By
    private static string resumeAfterSignalLoss;//Whether to resume after signal loss or not which will be fetched from Test Ini
    static string rfSwitch;//Whether it is A or B
    static bool isRFActive = true;

    //Constants used in the test
    private static class Constants
    {
        public const double waitTimeAfterSignalConnect = 10; //in secs
        public const double waitInEventBody = 60;//in Secs
        public const double waitInSignalLoss = 60;//in Secs
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Fetch Service from Content XML and Set GT's and Book a future Recording");
        this.AddStep(new Step1(), "Step 1: Wait for the recording to Start");
        this.AddStep(new Step2(), "Step 2: During Recording Remove RF cable of the STB");
        this.AddStep(new Step3(), "Step 3: Connect the RF cable back to the STB while in Body/EGT of that particular Recording");
        this.AddStep(new Step4(), "Step 4: Verify the Duration and Playback the recording from Archive");
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
            //Fetcing a recordable service
            recordableService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (recordableService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Recordable Service fetched is : " + recordableService.LCN);
            }

            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch SGT Value from Test ini");
            }

            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");
            if (egtFriendlyName == "")
            {
                FailStep(CL, res, "Failed to fetch EGT Value from Test ini");
            }

            LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);

            LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);

            resumeAfterSignalLoss = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RESUME_AFTER_SIGANLLOSS");
            if (resumeAfterSignalLoss == "")
            {
                FailStep(CL, "Resume after Signal loss parameter is not defined in the Test ini");
            }

            LogComment(CL, "Resume after signal loss fetched from Test ini file is " + resumeAfterSignalLoss);

            rfSwitch = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RF_SWITCH");
            if (rfSwitch == "")
            {
                FailStep(CL, "RF switch is not defined in the Test ini file");
            }

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }

            res = CL.EA.PVR.BookFutureEventFromGuide(eventToBeRecorded, ChannelNumber: recordableService.LCN, MinTimeBeforeEvStart: 5);
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
            res = CL.EA.WaitUntilEventStarts(eventToBeRecorded, StartGuardTime: sgtFriendlyName);
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
            if (resumeAfterSignalLoss.ToUpper() == "TRUE")
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
                timeToWait = Constants.waitInSignalLoss;
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
            }


            //Unplug RF signal
            res = CL.IEX.RF.TurnOff(instanceName: "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }

            isRFActive = false;
            LogCommentInfo(CL, "Waiting for " + timeToWait + " seconds before connecting back the RF cable");
            //waiting for event to end
            res = CL.IEX.Wait(seconds: timeToWait);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the event ends");
            }

            //Connecting the RF Signal
            if (rfSwitch.Equals("A"))
            {
                res = CL.IEX.RF.ConnectToA(instanceName:"1");
            }
            else
            {
                res = CL.IEX.RF.ConnectToB(instanceName:"1");
            }
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Connect RF back");
            }

            isRFActive = true;

            LogCommentInfo(CL, "Waiting for " + Convert.ToString(Constants.waitTimeAfterSignalConnect) + " secs after signal reconnects");
            //waiting after signal connects
            res = CL.IEX.Wait(seconds: Constants.waitTimeAfterSignalConnect);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait after the signal reconnects");
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
            if (resumeAfterSignalLoss.ToUpper() == "TRUE")
            {
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
                //Verifying that the event is not resumed after the sinal loss in EGT
                res = CL.EA.PCAT.VerifyEventIsRecording(eventToBeRecorded);
                if (res.CommandSucceeded)
                {
                    FailStep(CL, res, "Event is resumed in EGT after the signal Loss which is not supposed to happen");
                }
                else
                {
                    LogCommentInfo(CL, "Event is not resumed in EGT after the RF cycle");
                }
            }


            //verify record error information
            res = CL.EA.PVR.VerifyRecordErrorInfo(eventToBeRecorded, EnumRecordErr.Partial_SignalLoss);
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
            //Verify that the Event is a Partial Recording
            res = CL.EA.PCAT.VerifyEventPartialStatus(eventToBeRecorded, Expected: "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed To Verify the event Partial Status");
            }

            if (resumeAfterSignalLoss.ToUpper() == "TRUE")
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
        //Plug back RF if there was failure in reconnection
        if (!isRFActive)
        {
            //Connecting the RF Signal
            if (rfSwitch.Equals("A"))
            {
                res = CL.IEX.RF.ConnectToA(instanceName:"1");
            }
            else
            {
                res = CL.IEX.RF.ConnectToB(instanceName:"1");
            }
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to plug back RF signal!");
            }
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(eventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Event based recording from Archive");
        }
        //Fetch the SGT and EGT default values
        String defSgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (String.IsNullOrEmpty(defSgtValueInStr))
        {
            LogCommentFailure(CL, "Default SGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default SGT value in minutes - " + defSgtValueInStr);

        String defEgtValueInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (String.IsNullOrEmpty(defEgtValueInStr))
        {
            LogCommentFailure(CL, "Default EGT value not present in Project.ini file.");
        }
        LogCommentInfo(CL, "Default EGT value in minutes - " + defEgtValueInStr);

        //Set SGT & EGT to default
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: true, valueToBeSet: defSgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set Start Guide time - " + defSgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
        res = CL.EA.STBSettings.SetGuardTime(isStartToBeSet: false, valueToBeSet: defEgtValueInStr);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set End Guide time - " + defEgtValueInStr + " because of the following reason - " + res.FailureReason);
        }
    }

    #endregion PostExecute
}