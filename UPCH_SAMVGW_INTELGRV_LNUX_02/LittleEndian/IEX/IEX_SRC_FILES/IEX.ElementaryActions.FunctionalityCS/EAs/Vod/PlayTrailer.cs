using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Navigate to a VOD asset and play the trailer
    /// </summary>
    public class PlayTrailer : IEX.ElementaryActions.BaseCommand
    {
        private VODAsset _vodAsset;
        private bool _parentalProtection;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Navigate to a VOD asset and play the trailer
        /// </summary>
        /// <param name="vodAsset">VODAsset object</param>
        /// <remarks>
        /// </remarks>
        public PlayTrailer(VODAsset vodAsset, bool parentalProtection, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._vodAsset = vodAsset;
            this._parentalProtection = parentalProtection;
        }

        /// <summary>
        /// Navigate to a VOD asset and play the trailer
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

            // Play it
            EPG.Vod.PlayTrailer(_parentalProtection);
        }
    }
}





