using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    ///  Return the audio type
    /// </summary>
    public class GetAudioType : IEX.ElementaryActions.BaseCommand
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

        public GetAudioType(object stream, int audioLanguageIndex, Manager manager)
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
            EnumAudioType audioType = EnumAudioType.STEREO;

            if(_stream is VODAsset)
            {
                audioType = (EnumAudioType)Enum.Parse(typeof(EnumAudioType), ((VODAsset)_stream).AudioLanguage[_audioLanguageIndex].Split('-')[1]);  
            }
            else if (_stream is Service)
            {
                audioType = (EnumAudioType)Enum.Parse(typeof(EnumAudioType), ((Service)_stream).AudioLanguage[_audioLanguageIndex].Split('-')[1]);
            }
            else
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Wrong type for stream parameter"));
            }

            SetReturnValues(new Object[] { audioType });
        }
    }
}
