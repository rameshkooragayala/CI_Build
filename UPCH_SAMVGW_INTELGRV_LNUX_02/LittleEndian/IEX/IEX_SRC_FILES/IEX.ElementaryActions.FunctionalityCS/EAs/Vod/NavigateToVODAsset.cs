using System;
using System.Collections.Generic;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Navigate to a VOD asset and select it
    /// </summary>
    public class NavigateToVODAsset : IEX.ElementaryActions.BaseCommand
    {
        private VODAsset _vodAsset;
        private bool _doSelect;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Navigate to a VOD asset and select it
        /// </summary>
        /// <param name="vodAsset">VODAsset object</param>
        /// <remarks>
        /// </remarks>
        public NavigateToVODAsset(VODAsset vodAsset, bool doSelect, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._vodAsset = vodAsset;
            this._doSelect = doSelect;
        }

        /// <summary>
        /// Navigate to a VOD asset and select it
        /// </summary>
        protected override void Execute()
        {          
            // Navigate to the VOD asset
            EPG.Vod.NavigateToAsset(_vodAsset, _doSelect);
        }
    }
}








