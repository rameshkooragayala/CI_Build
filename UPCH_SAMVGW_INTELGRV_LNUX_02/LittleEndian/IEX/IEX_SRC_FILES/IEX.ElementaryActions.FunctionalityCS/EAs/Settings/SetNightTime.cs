using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

/// <summary>
///  Sets Night Time
/// </summary>
namespace EAImplementation
{
    /// <param name="pManager">Manager</param>
    /// <param name="startTime">Start Time to set</param>
    /// <param name="endTime">End Time to set</param>
    /// <summary>
    /// Sets start and end time by navigating to DEFINE AUTO STANDBY TIME.
    /// </summary>
    public class SetNightTime : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private string _startTime = "";
        private string _endTime = "";
        private Manager _manager;

        /// <remarks>
        /// Possible Error Codes:
        /// <para>300 - NavigationFailure</para> 
        /// <para>301 - DictionaryFailure</para> 
        /// <para>302 - EmptyEPGInfoFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>349 - ReturnToLiveFailure</para> 
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>
        public SetNightTime(string startTime,string endTime, Manager pManager)
        {
            this._startTime = startTime;
            this._endTime = endTime;
            this._manager = pManager;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {

            EPG.Utils.LogCommentInfo("Inside SetNightTime EA");
            if(!(_startTime == (null)))
            {
                EPG.Utils.LogCommentInfo("Navigating to DEFINE AUTO STANDBY TIME START TIME");
                EPG.Utils.EPG_Milestones_NavigateByName("STATE:DEFINE AUTO STANDBY TIME NIGHT START TIME");
                EPG.ManualRecording.SetStartTime(_startTime);
            }

            if (!(_endTime == (null)))
            {
                EPG.Utils.LogCommentInfo("Navigating to DEFINE AUTO STANDBY TIME END TIME");
                EPG.Utils.EPG_Milestones_NavigateByName("STATE:DEFINE AUTO STANDBY TIME NIGHT END TIME");
                EPG.ManualRecording.SetEndTime(_endTime);
            }
            
            EPG.Utils.ReturnToLiveViewing();
        }

    }
}
