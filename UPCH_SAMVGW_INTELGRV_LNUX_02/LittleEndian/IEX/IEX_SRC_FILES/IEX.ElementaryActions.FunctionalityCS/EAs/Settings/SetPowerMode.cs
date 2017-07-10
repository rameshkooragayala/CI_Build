using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;

/// <summary>
///  Sets Power Mode any power modes available based on the value passed.
///  Also handles warning messages if available
/// </summary>
namespace EAImplementation
{
    /// <param name="pManager">Manager</param>
    /// <param name="valueToSet">Power Mode Paramter to be set</param>
    /// <param name="warningMessageAvailable">warningMessage value to select/cancel low powermodes</param>
    /// <summary>
    /// Sets the power usage options based on value to set
    /// </summary>
    public class SetPowerMode : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private string _valueToSet = "";
        private string _warningMessage = "";
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
        public SetPowerMode(string valueToSet, string warningMessage, Manager pManager)
        {
            this._valueToSet = valueToSet;
            this._manager = pManager;
            this._warningMessage = warningMessage;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        ///  Set Power Mode navigates to STANDBY POWER USAGE and set to any available mode based on value passed.
        ///  Also handles warning messages if available
        /// </summary>
        protected override void Execute()
        {
           EPG.Utils.LogCommentInfo("Inside SetPowerMode EA"); 
           EPG.Utils.LogCommentInfo("Navigate to STAND BY POWER USAGE");          
           EPG.Utils.EPG_Milestones_NavigateByName("STATE:STANDBY POWER USAGE");
           //Verify for intermediate state and set setting
            try
               {
                   EPG.Settings.SetSetting(_valueToSet, "");

               }
           //This block verifies for the intermediate state notification message and throws exception if state not found
            catch
               {
                    
                    EPG.Utils.LogCommentInfo("Verify for the Notification Message");
                    EPG.Settings.HandlePowerModeSettingsException(_warningMessage,"");
               }

             EPG.Utils.ReturnToLiveViewing();
        }

     }
}
