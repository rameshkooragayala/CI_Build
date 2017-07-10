using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Stop asset playback
    /// </summary>
    public class StopAssetPlayback : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Stop asset playback
        /// </summary>
        /// <remarks>
        /// </remarks>
        public StopAssetPlayback(IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
        }

        /// <summary>
        /// Stop asset playback
        /// </summary>
        protected override void Execute()
        {
            EPG.Vod.StopPlayback();
        }
    }
}






