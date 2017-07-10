using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Set audio prefered language
    /// </summary>
    public class SetPreferredAudioLanguage : IEX.ElementaryActions.BaseCommand
    {
        private EnumLanguage _language;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Set audio prefered language
        /// </summary>
        /// <param name="enable">language to set</param>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>301 - DictionaryFailure</para> 
        /// <para>302 - EmptyEPGInfoFailure</para> 
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>314 - SetSettingsFailure</para> 
        /// <para>332 - NoValidParameters</para> 	
        /// <para>334 - VideoNotPresent</para> 
        /// <para>349 - ReturnToLiveFailure</para> 
        /// </remarks>
        public SetPreferredAudioLanguage(EnumLanguage language, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._language = language;
        }

        /// <summary>
        /// Set audio prefered language
        /// </summary>
        protected override void Execute()
        {
            string language = EPG.Utils.TranslateLanguage(_language.ToString());

            EPG.Settings.SetAudioLanguage(language.ToUpper());

            EPG.Utils.ReturnToLiveViewing();
        }
    }
}



