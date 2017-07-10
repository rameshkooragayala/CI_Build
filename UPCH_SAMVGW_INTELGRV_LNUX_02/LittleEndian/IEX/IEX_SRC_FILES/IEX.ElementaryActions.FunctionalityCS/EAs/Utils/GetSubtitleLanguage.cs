using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    ///  Return the audio type
    /// </summary>
    public class GetSubtitleLanguage : IEX.ElementaryActions.BaseCommand
    {
        private object _stream;
        private int _subtitleLanguageIndex;
        private Manager _manager;
        private IEX.ElementaryActions.EPG.SF.UI EPG;

        /// <param name="stream">Service or VODAsset object</param>
        /// <param name="subtitleLanguageIndex">index of the subtitle language</param>
        /// <param name="manager">the manager</param>
        /// <remarks>
        /// </remarks>

        public GetSubtitleLanguage(object stream, int subtitleLanguageIndex, Manager manager)
        {
            _stream = stream;
            _subtitleLanguageIndex = subtitleLanguageIndex;
            _manager = manager;
            EPG = _manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {
            EnumLanguage subtitleLanguage;
            string language = "";

            if (_stream is VODAsset)
            {
                language = ((VODAsset)_stream).SubtitleLanguage[_subtitleLanguageIndex].Split('-')[0];
            }
            else if (_stream is Service)
            {
                language = ((Service)_stream).SubtitleLanguage[_subtitleLanguageIndex].Split('-')[0];
            }
            else
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Wrong type for stream parameter"));
            }

            language = EPG.Utils.TranslateLanguage(language, false);
            subtitleLanguage = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), language);

            SetReturnValues(new Object[] { subtitleLanguage });
        }
    }
}



