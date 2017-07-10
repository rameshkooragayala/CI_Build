using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Navigate to a VOD asset and buy it
    /// </summary>
    public class BuyAsset : IEX.ElementaryActions.BaseCommand
    {
        private VODAsset _vodAsset;
        private bool _parentalProtection;
        private bool _purchaseProtection;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Navigate to a VOD asset and buy it
        /// </summary>
        /// <param name="vodAsset">VODAsset object</param>
        /// <remarks>
        /// </remarks>
        public BuyAsset(VODAsset vodAsset, bool parentalProtection, bool purchaseProtection, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._vodAsset = vodAsset;
            this._parentalProtection = parentalProtection;
            this._purchaseProtection = purchaseProtection;
        }

        /// <summary>
        /// Navigate to a VOD asset and buy it
        /// </summary>
        protected override void Execute()
        {
            // Navigate and select the asset if required
            if (_vodAsset != null)
            {
                IEXGateway._IEXResult res = _manager.VOD.NavigateToVODAsset(_vodAsset, true);
                if (!res.CommandSucceeded)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NavigationFailure, "Failed to navigate to asset: " + _vodAsset.Title));
                }

                // add some delay after selecting the asset ("BUY"/"PLAY"/"RESUME" options take longer time to be displayed)
                const double MIN_DELAY = 3;
                _iex.Wait(MIN_DELAY);
            }

            // Buy it
            EPG.Vod.Buy(_purchaseProtection, _parentalProtection);
        }
    }
}





