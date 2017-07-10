using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;
using System.Collections;


/// <summary>
///  This EA verifies the set power mode.
///  Verifies Maintenance milestones if supported.
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
    /// <param name="CurrEPGTime">Current EPG Time</param>
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
        private bool _isWakeUp;
        private bool _isStandBy;
        private Manager _manager;
        ArrayList list = new ArrayList();


        /// <remarks>
        /// Possible Error Codes:
        /// <para>300 - NavigationFailure</para> 
        /// <para>301 - DictionaryFailure</para> 
        /// <para>302 - EmptyEpgInfoFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>349 - ReturnToLiveFailure</para> 
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>
        public VerifyPowerMode(string powerMode, bool jobPresent, string StartTime, string EndTime, string currEPGTime, bool isWakeUp, bool isStandBy, Manager pManager)
        {
            this._powerMode = powerMode;
            this._manager = pManager;
            this._startTime = StartTime;
            this._endTime = EndTime;
            this._currEPGTime = currEPGTime;
            this._jobPresent = jobPresent;
            this._isWakeUp = isWakeUp;
            this._isStandBy = isStandBy;
            EPG = this._manager.UI;

        }

        /// <summary>
        ///  EA Execution
        ///  Verify Power Mode sets the power mode and verifies the power mode is set accordingly
        /// </summary>
        protected override void Execute()
        {
            EPG.Utils.LogCommentInfo("Inside VerifyPowerMode EA");
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

            if (_isStandBy)
            {
                //Enter Stand by
                EPG.Utils.Standby(false);
            }
            //fetch the mode from EPG milstones
            EPG.Utils.GetEpgInfo("DefaultStandByPref is", ref obtainedPowerMode);
            EPG.Utils.LogCommentInfo("Mode" + obtainedPowerMode + "was entered");

            //Verify that box is in expected stand by mode
            if (!(obtainedPowerMode.Equals(expectedPowerMode)))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to enter expected stand by"));
            }

            //Verify for Maintainence milestones if supported by project
            if (EPG.PowerManagement.IsPwrMgmtMaintainanceSupported())
            {
                EPG.PowerManagement.VerifyPmMilesStone(_jobPresent, _startTime, _endTime, _currEPGTime);
            }
            else
            {
				EPG.Utils.LogCommentWarning("Project does not support maintenance waiting for default delay ");
                Int32 defaultDelay = Int32.Parse(EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DEALY"));
                _iex.Wait(defaultDelay);
            }

            if (_powerMode.Equals("HOT STANDBY"))
            {
                if (_isWakeUp)
                {
                    //Out of Stand by
                    EPG.Utils.Standby(true);
                    //Verify state LIVE after exiting standby
                    EPG.Utils.VerifyState("LIVE", imageLoadDelay);
                }
            }
            else
            {
			    Int32 defaultDelay = Int32.Parse(EPG.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DEALY"));
                //Get fasShutdownmilestone from milestones ini
                string ShutDownMilestone = EPG.Utils.GetValueFromMilestones("ShutDown");
                //Verify for FAS shutdown milestones.
                EPG.Utils.BeginWaitForDebugMessages(ShutDownMilestone, defaultDelay*60);
                bool isShutDownMilestoneRecieved = EPG.Utils.EndWaitForDebugMessages(ShutDownMilestone, ref list);
                if (!isShutDownMilestoneRecieved)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Failed to get FAS ShutDown Milestone"));
                }
                 if (_isWakeUp)
                {

                    _iex.Wait(120);
                    //Out of Stand by
                    //Thus without throwing the exception the process is continued undercatch block

                    try
                    {
                        EPG.Utils.Standby(true);

                    }
                    catch
                    {
                      try
                        {
                            Int32 waitBeforeTelnet = Int32.Parse(EPG.Utils.GetValueFromEnvironment("WaitBeforeTelnet"));
                            //Mount the box
                            _iex.Wait(waitBeforeTelnet);
                        }
                        catch
                        {
                            _iex.Wait(180);
                        }
                        Boolean standbyAfterBoot = Boolean.Parse(EPG.Utils.GetValueFromProject("BOOTUP", "STANDBY_AFTER_REBOOT"));
                        Boolean isHomeNetwork = Boolean.Parse(EPG.Utils.GetValueFromProject("BOOTUP", "IsHomeNetwork"));

                        //if the box supports home network mount client
                        if (isHomeNetwork)
                        {
                            _manager.MountClient(EnumMountAs.NOFORMAT);

                        }
                        //if the box does not support home network mount only gateway
                        else
                        {
                            _manager.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);

                            //Wait for some time for STB to come to standby mode 
                            _iex.Wait(imageLoadDelay);
                        }
                        //Verify state LIVE
                        EPG.Utils.VerifyState("LIVE", imageLoadDelay);
                    }
					
                }
                
            }
        }

    }
}
