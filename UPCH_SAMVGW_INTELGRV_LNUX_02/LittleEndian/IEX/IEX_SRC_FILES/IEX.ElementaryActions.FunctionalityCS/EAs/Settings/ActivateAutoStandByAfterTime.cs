using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

/// <summary>
///  Set Auto Stand Time
/// </summary>
namespace EAImplementation
{
    /// <param name="pManager">Manager</param>
    /// <param name="valueToSet">time period to set</param>
    /// <summary>
    /// Set Auto Stand Time navigates to ACTIVE STANDBY AFTER and set to specific time after which the box moves to standby at IDLE situations
    /// </summary>
    public class ActivateAutoStandByAfterTime : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private string _valueToSet = "";
        private Manager _manager;

        /// <remarks>
        /// Possible Error Codes:
        /// <para>300 - NavigationFailure</para> 
        /// <para>301 - DictionaryFailure</para> 
        /// <para>302 - EmptyEpgInfoFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>349 - ReturnToLiveFailure</para> 
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>
        public ActivateAutoStandByAfterTime(string valueToSet, Manager pManager)
        {
            this._valueToSet = valueToSet;
            this._manager = pManager;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {

            EPG.Utils.LogCommentInfo("Inside ActivateAutoStandByAfterTime EA");

            EPG.Utils.EPG_Milestones_NavigateByName("STATE:ACTIVATE STANDBY AFTER");

            EPG.Utils.LogCommentInfo("Navigated to AUTO STAND BY");

            EPG.Settings.SetSetting(_valueToSet, "");

            EPG.Utils.ReturnToLiveViewing();
        }

    }
}
