using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Navigate to a VOD asset and subscribe it
    /// </summary>
    public class SubscribeAsset : IEX.ElementaryActions.BaseCommand
    {
        private VODAsset _vodAsset;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Navigate to a VOD asset and subscribe it
        /// </summary>
        /// <param name="vodAsset">VODAsset object</param>
        /// <remarks>
        /// </remarks>
        public SubscribeAsset(VODAsset vodAsset, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._vodAsset = vodAsset;
        }

        /// <summary>
        /// Navigate to a VOD asset and subscribe it
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

                // add some delay after selecting the asset ("BUY"/"PLAY"/"RESUME"/"SUBSCRIBE" options take longer time to be displayed)
                const double MIN_DELAY = 3;
                _iex.Wait(MIN_DELAY);
            }

            // Subscribe it
            EPG.Vod.Subscribe();
        }
    }
}







