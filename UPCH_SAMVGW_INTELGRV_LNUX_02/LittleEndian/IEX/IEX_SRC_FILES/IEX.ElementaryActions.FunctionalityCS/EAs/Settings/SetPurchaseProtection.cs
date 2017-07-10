using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Active/deactivate prurchase protection in settings
    /// </summary>
    public class SetPurchaseProtection : IEX.ElementaryActions.BaseCommand
    {
        private bool _enable;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Activate/deactivate purchase protection in settings
        /// </summary>
        /// <param name="enable">enable if true, disable if false</param>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>322 - VerificationFailure</para> 
        /// </remarks>
        public SetPurchaseProtection(bool enable, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._enable = enable;
        }

        /// <summary>
        /// Activate/deactivate the purchase protection in settings
        /// </summary>
        protected override void Execute()
        {
            string itemName;

            // Navigate to settings
            EPG.Settings.NavigateToPurchaseProtection();

            if (_enable)
            {
                itemName = EPG.Utils.GetValueFromDictionary("DIC_SETTINGS_OPTION_ON");
            }
            else
            {
                itemName = EPG.Utils.GetValueFromDictionary("DIC_SETTINGS_OPTION_OFF");
            }

            // Set the wanted settings value
            EPG.Settings.SetSetting(itemName, "");

            // Enter PIN if asked)
            if (EPG.Utils.VerifyState("INSERT PIN", 5))
            {
                string pinValue = _manager.UI.Utils.GetValueFromEnvironment("DefaultPIN");
                EPG.Utils.EnterPin(pinValue);
            }

            // Verify final state
            if (!EPG.Utils.VerifyState("PIN & PARENTAL CONTROL", 5))
            {
                string currentState = "";
                EPG.Utils.GetActiveState(ref currentState);
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong state after changing settings. Expected: 'PIN & PARENTAL CONTROL', Received: '" + currentState + "'"));
            }
        }
    }
}


