using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Select an asset in the list of purchased assets
    /// </summary>
    /// <remarks>
    /// Possible Error Codes:
    /// <para>346 - FindEventFailure</para> 
    /// </remarks>
    public class SelectPurchasedAsset : IEX.ElementaryActions.BaseCommand
    {
        private VODAsset _vodAsset;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Select an asset in the list of purchased assets
        /// </summary>
        /// <param name="vodAsset">VODAsset object</param>
        /// <remarks>
        /// </remarks>
        public SelectPurchasedAsset(VODAsset vodAsset, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._vodAsset = vodAsset;
        }

        /// <summary>
        /// Select an asset in the list of purchased assets
        /// </summary>
        protected override void Execute()
        {
            EPG.Vod.SelectPurchasedAsset(_vodAsset.Title);
        }
    }
}







