using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

/// <summary>
///  Set Auto Stand By
/// </summary>
namespace EAImplementation
{
    /// <param name="pManager">Manager</param>
    /// <param name="valueToSet">valueToSet</param>
    /// <summary>
    /// Sets the power usage optios based on value to set
    /// </summary>
    public class SetAutoStandBy : IEX.ElementaryActions.BaseCommand
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
        public SetAutoStandBy(string valueToSet, Manager pManager)
        {
            this._valueToSet = valueToSet;
            this._manager = pManager;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        ///  Set Auto Standby navigates to AUTO STANDBY and sets it either to AUTOMATIC/OFF/NIGHT TIME
        /// </summary>
        protected override void Execute()
        {
            EPG.Utils.LogCommentInfo("Inside SetAutoStandBy EA");

            EPG.Utils.EPG_Milestones_NavigateByName("STATE:AUTO STANDBY");

            EPG.Utils.LogCommentInfo("Navigated to AUTO STAND BY");

            EPG.Settings.SetSetting(_valueToSet, "");

            EPG.Utils.ReturnToLiveViewing();
        }

    }
}
