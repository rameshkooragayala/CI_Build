using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    ///  Return the audio type
    /// </summary>
    public class GetAudioLanguage : IEX.ElementaryActions.BaseCommand
    {
        private object _stream;
        private int _audioLanguageIndex;
        private Manager _manager;
        private IEX.ElementaryActions.EPG.SF.UI EPG;

        /// <param name="stream">Service or VODAsset object</param>
        /// <param name="audioLanguageIndex">index of the audio language</param>
        /// <param name="manager">the manager</param>
        /// <remarks>
        /// </remarks>

        public GetAudioLanguage(object stream, int audioLanguageIndex, Manager manager)
        {
            _stream = stream;
            _audioLanguageIndex = audioLanguageIndex;
            _manager = manager;
            EPG = _manager.UI;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {
            EnumLanguage audioLanguage;
            string language = "";

            if (_stream is VODAsset)
            {
                language = ((VODAsset)_stream).AudioLanguage[_audioLanguageIndex].Split('-')[0];
            }
            else if (_stream is Service)
            {
                language = ((Service)_stream).AudioLanguage[_audioLanguageIndex].Split('-')[0];
            }
            else
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Wrong type for stream parameter"));
            }

            language = EPG.Utils.TranslateLanguage(language, false);
            audioLanguage = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), language);

            SetReturnValues(new Object[] { audioLanguage });
        }
    }
}

