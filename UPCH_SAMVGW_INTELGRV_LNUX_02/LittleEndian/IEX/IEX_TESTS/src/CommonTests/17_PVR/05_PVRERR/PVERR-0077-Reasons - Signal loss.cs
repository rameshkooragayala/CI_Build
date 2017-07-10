/// <summary>
///  Script Name : PVERR-0077-Reasons - Signal loss.cs
///  Test Name   : PVERR-0077-Reasons - Signal loss
///  TEST ID     : 71390
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 08.10.2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**
 * Pvrerr 0077.
 *
 * @author Avinoba
 * @date 07-Oct-13
 */

[Test("PVRERR_0077")]
public class PVRERR_0077 : _Test
{
    /**
     * The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * Shared members between steps.
     */

    static Service recordChannel;

    /**
     * Duration of the event.
     */

    static string evtDuration;

    /**
     * The is rf active.
     */

    static bool isRFActive = true;

    /**
    * The end guard time.
    */

    static string endGuardTime;
    

    /**
     * Information describing the precondition.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**
     * Event queue for all listeners interested in STEP1_DESCRIPTION events.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Record an Event which has lost signal in the end of the recording";

    /**
     * Information describing the step 2.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Verify the Recording Status error Info for Partial";

    #region Create Structure

    /**
     * Creates the structure.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */
    
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    /**
     * Constant.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    private static class Constant
    {

        /**
         * The minimum time before event start.
         */

        public const int minTimeBeforeEvtStart = -1;

        /**
         * The wait time in event.
         */

        public const double waitTimeInEvent = 4; //in mins

        /**
         * The wait time after signal connect.
         */

        public const double waitTimeAfterSignalConnect = 5; //in secs

        /**
         * The is resuming.
         */

        public const bool isResuming = false;

        /**
         * The verify pcat.
         */

        public const bool verifyPCAT = false;

        /**
         * The number of presses
         */
        public const int noOfPresses = 1;
    }

    #region PreExecute

    /**
     * Pre execute.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**
     * Pre condition.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 07-Oct-13
         */

        public override void Execute()
        {
            StartStep();
            //Get value from Test.ini
            evtDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EVENT_DURATION");
            if (int.Parse(evtDuration) < 10)
            {
                FailStep(CL, "Event Duration fetched from Test.ini should be more or equal to 10 mins");
            }
            //Get Values From xml File
            recordChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;EventDuration=" + evtDuration, "ParentalRating=High");
            if (recordChannel == null)
            {
                FailStep(CL, res, "Failed to get Channel from Content.xml for the passed criterion");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + recordChannel.LCN);

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

    /**
     * Step 1.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 07-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //record current event from the beginning of the event
            res = CL.EA.PVR.BookFutureEventFromGuide("recordEvent", recordChannel.LCN, Constant.noOfPresses, Constant.minTimeBeforeEvtStart, Constant.verifyPCAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event in Channel: " + recordChannel.LCN);
            }

            //wait till the event starts
            res = CL.EA.WaitUntilEventStarts("recordEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the booked event starts");
            }

            //waiting for some time to elapse in the event
            LogCommentInfo(CL, "Waiting for " + Constant.waitTimeInEvent + " mins");
            res = CL.IEX.Wait(Constant.waitTimeInEvent * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait");
            }

            //Unplug RF signal
            res = CL.IEX.RF.TurnOff("1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }

            isRFActive = false;

            LogCommentInfo(CL,"Waiting for "+Convert.ToString((double.Parse(evtDuration)-Constant.waitTimeInEvent)+" mins for event to end"));
            //waiting for event to end
            res=CL.IEX.Wait((double.Parse(evtDuration)-Constant.waitTimeInEvent)*60);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait till the event ends");
            }

            int endGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(endGuardTime, false);
            //waiting for end Guard time to end 
            res = CL.IEX.Wait(Convert.ToDouble(endGuardTimeNum) * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait till the end guard time is completed");
            }

            //Connecting the RF Signal
            res = CL.IEX.RF.ConnectToA("1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to plug back RF signal!");
            }

            isRFActive = true;

            //get value from project.ini
            string isRebootOnSignalLoss = CL.EA.GetValueFromINI(EnumINIFile.Project, "RF", "DELAY_BEFORE_REBOOT_ON_SIGNAL_LOSS");

            //checking if the STB reboots on Signal loss
            if (Convert.ToBoolean(isRebootOnSignalLoss))
            {
                //get value from project.ini for the delay on reboot
                string delayOnReboot = CL.EA.GetValueFromINI(EnumINIFile.Project, "RF", "DELAY_ON_REBOOT_SIGNAL_LOSS");

                LogCommentInfo(CL, "Waiting for " + delayOnReboot + "secs to reboot the STB");
                //waiting for rebooting the STB
                res = CL.IEX.Wait(Convert.ToDouble(delayOnReboot));
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait");
                }

                //mount the box
                res = CL.EA.MountClient(EnumMountAs.NOFORMAT_NOREBOOT);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Mount Client");
                }
            }
            else
            {
                LogCommentInfo(CL,"Waiting for "+Convert.ToString(Constant.waitTimeAfterSignalConnect)+" secs after signal reconnects");
                //waiting after signal connects
                res = CL.IEX.Wait(Constant.waitTimeAfterSignalConnect);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to wait after the signal reconnects");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * Step 2.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * Executes this object.
         *
         * @author Avinoba
         * @date 07-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //verify record error information
            res = CL.EA.PVR.VerifyRecordErrorInfo("recordEvent", EnumRecordErr.Partial_SignalLoss);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the recording error information");
            }

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute

    /**
     * Posts the execute.
     *
     * @author Avinoba
     * @date 07-Oct-13
     */

    [PostExecute()]
    public override void PostExecute()
    {

        IEXGateway._IEXResult res;

        //Plug back RF if there was failure in reconnection
        if (!isRFActive)
        {
            res = CL.IEX.RF.ConnectToA();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to plug back RF signal!");
                //FailStep(CL, res, "Failed to plug back RF signal!");
            }
        }

        //delete the recorded event from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive("recordEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to Delete Recording from Archive");
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