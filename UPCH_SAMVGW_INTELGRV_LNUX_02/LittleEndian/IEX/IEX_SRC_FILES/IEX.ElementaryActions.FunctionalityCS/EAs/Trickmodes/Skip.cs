using System;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation   
{
    /// <summary>
    /// Setting Skip Points On TrickMode
    /// </summary>
    public class Skip : IEX.ElementaryActions.BaseCommand
    {

        private bool _direction;
        private bool _PlaybackContext;
        private int _NumOfSkipPoints;
        private EnumVideoSkip _SkipDurationSetting;
        private IEX.ElementaryActions.EPG.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Performs skip operation and does verification
        /// </summary>
        /// <param name="direction">Madatory direction of skip.True if Forward else False. No default value</param>
        /// <param name="PlaybackContext">Mandatory-PLAYBACK or RB-Playback is true/RB is false </param>
        /// <param name="SkipDurationSetting">Enumvideoskip equivalent to required value </param>
        /// <param name="NumOfSkipPoints">[optional parameter]-For BOOKMARK-Num of recordings,if not specified BOFEOF check will be true </param>
        /// <param name="pManager"></param>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>303 - FasVerificationFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>313 - SetTrickModeSpeedFailure</para> 
        /// <para>318 - SetSkipFailure</para> 
        /// <para>328 - INIFailure</para> 
        /// <para>332 - NoValidParameters</para> 
        /// </remarks>
        public Skip(bool direction, bool PlaybackContext, EnumVideoSkip SkipDurationSetting, int NumOfSkipPoints, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._direction = direction;
            this._PlaybackContext = PlaybackContext;
            this._NumOfSkipPoints = NumOfSkipPoints;
            this._SkipDurationSetting = SkipDurationSetting;

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Execute()
        {

            string skipSetting = ((int)_SkipDurationSetting).ToString();
            EPG.TrickModes.RaiseTrickMode();

            EPG.TrickModes.SetSpeed(1);

            if (_PlaybackContext)
            {
                _manager.PCAT.CopyPCAT();

            }

            EPG.TrickModes.SetSkip(_direction, _PlaybackContext, skipSetting, _NumOfSkipPoints);

        }





    }
}
