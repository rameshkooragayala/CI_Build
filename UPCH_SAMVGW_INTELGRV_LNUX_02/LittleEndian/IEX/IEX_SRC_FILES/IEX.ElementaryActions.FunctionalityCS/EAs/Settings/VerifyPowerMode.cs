using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;
using System.Collections;


/// <summary>
///  This EA verifies the set power mode
///  Verifies Maintenance milestones if supported
///  Checks if any recordings are present and waits till the task is complete to triger maintenance
///  After Hot/Medium/Low box comes to live after power up
///  After Cold stand by box reboots(during reboot it verifies whether its homenetwork/only GW),After reboot if the projects goes to stand by it comes out of stand by.
///  Multiple clients connected not handled - Enhancement
/// </summary>
namespace EAImplementation
{
    /// <param name="powerMode">Mode to set</param>
    /// <param name="jobpresent">Checks for recordings present/scheduled</param>
    /// <param name="startTime">Record start time</param>
    /// <param name="endTime">Record end time</param>
    /// <param name="CurrEPGTime">Current EPG time</param>
    /// <summary>
    /// Verifies the set power mode option
    /// </summary>
    public class VerifyPowerMode : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private string _powerMode = "";
        private string _startTime = "";
        private string _endTime = "";
        private string _currEPGTime = "";
        private bool _verifyPowerMode = false;
        private bool _jobPresent = false;
        private Manager _manager;
        ArrayList list = new ArrayList();


        /// <remarks>
        /// Possible Error Codes:
        /// <para>300 - NavigationFailure</para> 
        /// <para>301 - DictionaryFailure</para> 
        /// <para>302 - EmptyEPGInfoFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>349 - ReturnToLiveFailure</para> 
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>
        public VerifyPowerMode(string powerMode,bool jobPresent, string StartTime, string EndTime, string currEPGTime, Manager pManager)
        {
            this._powerMode = powerMode;
            this._manager = pManager;
            this._startTime = StartTime;
            this._endTime = EndTime;
            this._currEPGTime = currEPGTime;
            this._jobPresent = jobPresent;
            EPG = this._manager.UI;
          
        }

        /// <summary>
        ///  EA Execution
        ///  Verify Power Mode sets the power mode and verifies the power mode is set accordingly
        /// </summary>
        protected override void Execute()
        {
            string obtainedPowerMode = "";
           // DefaultStandByPref is  = 2 (for HOT STANDBY)
           // DefaultStandByPref is  = 1 (for MEDIUM)
           // DefaultStandByPref is  = 3 (for LOW)
           // DefaultStandByPref is  = 0 (for COLD STANDBY)

            //Fetch Image load delay from project ini 
            Int32 imageLoadDelay = Int32.Parse(EPG.Utils.GetValueFromProject("BOOTUP", "IMAGE_LOAD_DELAY_SEC"));

            //Fetch the mode from project ini
            string expectedPowerMode = EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", _powerMode);
            if (string.IsNullOrEmpty(expectedPowerMode))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to fetch expected power mode value from project ini"));
            }

                //Enter Stand by
                EPG.Utils.Standby(false);

                //fetch the mode from EPG milstones
                EPG.Utils.GetEPGInfo("DefaultStandByPref", ref obtainedPowerMode);
                EPG.Utils.LogCommentInfo("Mode" + obtainedPowerMode + "was entered");

                //Verify that box is in expected stand by mode
                if (!(obtainedPowerMode.Equals(expectedPowerMode)))
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to enter expected stand by"));
                }

                //Verify for Maintainence milestones if supported by project
                verifyMPMilesStones();

                switch (_powerMode)
                {

                    case "HOT STANDBY":
                         //Out of Stand by
                        EPG.Utils.Standby(true);
                        //Verify state LIVE after exiting standby
                        EPG.Utils.VerifyState("LIVE", imageLoadDelay);
                        break;

                    case "MEDIUM":case "LOW":
                        //If Projects support maintainance,verify for UTM milestones
                        verifyMPMilesStones();
                        //Get fasShutdownmilestone from milestones ini
                        string ShutDownMilestone = EPG.Utils.GetValueFromMilestones("ShutDown");
                        //Verify for FAS shutdown milestones.
                        EPG.Utils.BeginWaitForDebugMessages(ShutDownMilestone, 10);
                        EPG.Utils.EndWaitForDebugMessages(ShutDownMilestone, ref list);
                        //Out of Stand by
                        //After FAS shut down sending power key fails 
                        //sendIR:verifies for FAS milestones fails
                        //Thus without throwing the exception the process is continued undercatch block
                        try
                        {
                            EPG.Utils.Standby(true);
                        }
                        
                        catch
                        {
                            //Verify state LIVE
                            EPG.Utils.VerifyState("LIVE", imageLoadDelay);
                        }
                        break;

                        case "COLD STANDBY":
                        //If Projects support maintainance,verify for UTM milestones
                        verifyMPMilesStones();
                        //Get fasShutdownmilestone from milestones ini
                        ShutDownMilestone = EPG.Utils.GetValueFromMilestones("ShutDown");
                        //Verify for FAS shutdown milestones.
                        EPG.Utils.BeginWaitForDebugMessages(ShutDownMilestone, 10);
                        EPG.Utils.EndWaitForDebugMessages(ShutDownMilestone, ref list);

                        //Out of Stand by
                        //After FAS shut down sending power key fails 
                        //sendIR:verifies for FAS milestones fails
                        //Thus without throwing the exception the process is continued undercatch block
                        
                        try
                        {
                            EPG.Utils.Standby(true);
                        }
                        catch
                        {
                            //Mount the box

                            Boolean standbyAfterBoot = Boolean.Parse(EPG.Utils.GetValueFromProject("BOOTUP", "STANDBY_AFTER_REBOOT"));
                            Boolean isHomeNetwork = Boolean.Parse(EPG.Utils.GetValueFromProject("BOOT", "IsHomeNetwork"));
                           
                            //if the box supports home network mount client
                            if (isHomeNetwork)
                            {
                                _manager.MountClient(EnumMountAs.NOFORMAT);

                            }
                            //if the box does not support home network mount only gateway
                            else
                            {
                                _manager.MountGw(EnumMountAs.NOFORMAT);

                                //Wait for some time for STB to come to standby mode 
                                _iex.Wait(imageLoadDelay);

                                if (standbyAfterBoot)
                                {
                                    EPG.Utils.Standby(true);

                                }
                            }
                            //Verify state LIVE
                            EPG.Utils.VerifyState("LIVE", imageLoadDelay);

                        }

                        break;
                }

            }
        
           //Method to verify MP Milestones if the project support maintenance
            void verifyMPMilesStones()
            {
                //Project supports Maintainance 
                string maintananceSupport = "";
                maintananceSupport = EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_SUPPORT");
                if (string.IsNullOrEmpty(maintananceSupport))
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to get the maintananceSupport value from project ini"));
                }

                //If Projects support maintainance,verify for UTM milestones
                if (maintananceSupport.ToUpper().Equals("TRUE"))
                {
                    //default maintenance duration fetched from project ini
                    Int32 maintenanceCompletionDuration = (Int32.Parse(EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DURATION")))*60;
                    string mpStartMilestones = "";
                    string mpCompleteMilestones = "";

                    // method calculateWaitForMP to calculate waitforMP
                      int maintenanceDelay = (calculateWaitForMP(_jobPresent,_startTime,_endTime,_currEPGTime))*60;

                    //get values from milestones ini
                    mpStartMilestones = EPG.Utils.GetValueFromMilestones("MaintenanceStart");
                    mpCompleteMilestones = EPG.Utils.GetValueFromMilestones("MaintenanceComplete");

                    //verify for UTM start milestones.
                    EPG.Utils.BeginWaitForDebugMessages(mpStartMilestones, maintenanceDelay);
                    EPG.Utils.EndWaitForDebugMessages(mpStartMilestones, ref list);

                    //Verify for UTM completed milestones.
                    EPG.Utils.BeginWaitForDebugMessages(mpCompleteMilestones, maintenanceCompletionDuration);
                    EPG.Utils.EndWaitForDebugMessages(mpCompleteMilestones, ref list);
            }
        }
            //Method to calculate wait time before MP start
            int calculateWaitForMP(bool jobPresent, string StartTime, string EndTime, string CurrEPGTime)
            {
                //Fetch Default maintenance duration from project ini.
                Int32 defaultMaintenancedelay = Int32.Parse(EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DEALY"));
                defaultMaintenancedelay = defaultMaintenancedelay * 60;//in secs
                EPG.Utils.LogCommentInfo("Time after which maintenance should start" + defaultMaintenancedelay);
                
                //Max time to check for scheduled records
                Int32 defaultMaintenanceCheckTime = Int32.Parse(EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_CHECK_TIME"));
                defaultMaintenanceCheckTime = defaultMaintenanceCheckTime * 60 * 60;//in secs
                EPG.Utils.LogCommentInfo("Max time to check for scheduled records" + defaultMaintenanceCheckTime);


                //No recording present return default maintenance delay
                if (!jobPresent)
                {
                     return defaultMaintenancedelay;
                }
                 //Recordings are ongoing/scheduled
                else
                {
                    DateTime startTime = DateTime.Parse(StartTime);
                    DateTime endTime = DateTime.Parse(EndTime);
                    DateTime currEPGTime = DateTime.Parse(CurrEPGTime);
                    TimeSpan recordDuration ;

                    //Current recording
                    Int32 waitTime;
                    if (startTime <= currEPGTime)
                    {
                       
                        recordDuration = endTime - currEPGTime;
                        waitTime = Convert.ToInt32(recordDuration) + defaultMaintenancedelay;
                        return waitTime;
                    }
                    else
                    {
                        //Near future recording(If recording scheduled within maintenence check time)
                         recordDuration = endTime - currEPGTime;
                         if ((Convert.ToInt32(recordDuration)) <= defaultMaintenanceCheckTime)
                        {
                            waitTime = Convert.ToInt32(recordDuration) + defaultMaintenancedelay;
                            return waitTime;
                        }
                         //Far future reocrding(if recording is scheduled after maintenence check time)
                         else
                         {
                             return defaultMaintenancedelay;
                         }
                       
                    }
                    

                }

                
            }
    }
}
