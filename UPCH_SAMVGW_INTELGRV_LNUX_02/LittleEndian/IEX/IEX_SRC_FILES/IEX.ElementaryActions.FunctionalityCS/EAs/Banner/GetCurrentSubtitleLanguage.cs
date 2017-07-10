using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    ///  Get current subtitle language from action menu
    /// </summary>
    public class GetCurrentSubtitleLanguage : IEX.ElementaryActions.BaseCommand
    {
        private Manager _manager;
        private IEX.ElementaryActions.EPG.SF.UI EPG;

        /// <param name="manager">the manager</param>
        /// <remarks>
        /// </remarks>

        public GetCurrentSubtitleLanguage(Manager manager)
        {
            this._manager = manager;
            this.EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {
            EnumLanguage subtitleLanguage;
            string tmp = "";

            // navigate to AUDIO settings in channel bar
            EPG.Utils.EPG_Milestones_NavigateByName("STATE:SUBTITLES ON PLAYBACK");

            // get the current selection
            EPG.Utils.GetEpgInfo("title", ref tmp);

            // validate current selection to go back to playback state
            string selectKey = EPG.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY");
            EPG.Utils.SendIR(selectKey, 5000);

            tmp = _manager.UI.Utils.TranslateLanguage(tmp, false);
            subtitleLanguage = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), tmp);

            SetReturnValues(new Object[] { subtitleLanguage });
        }
    }
}





