using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    ///  Get current audio language from action menu
    /// </summary>
    public class GetCurrentAudioLanguage : IEX.ElementaryActions.BaseCommand
    {
        private Manager _manager;
        private IEX.ElementaryActions.EPG.SF.UI EPG;

        /// <param name="manager">the manager</param>
        /// <remarks>
        /// </remarks>

        public GetCurrentAudioLanguage(Manager manager)
        {
            this._manager = manager;
            this.EPG = this._manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {
            EnumLanguage audioLanguage;
            string tmp = "";

            // navigate to AUDIO settings in channel bar
            EPG.Utils.EPG_Milestones_NavigateByName("STATE:AUDIO ON PLAYBACK");

            // get the current selection
            EPG.Utils.GetEpgInfo("title", ref tmp);

            // validate current selection to go back to playback state
            string selectKey = EPG.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY");
            EPG.Utils.SendIR(selectKey, 5000);

            tmp = tmp.Split('-')[0];
            tmp = _manager.UI.Utils.TranslateLanguage(tmp, false);
            audioLanguage = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), tmp);

            SetReturnValues(new Object[] { audioLanguage });
        }
    }
}




