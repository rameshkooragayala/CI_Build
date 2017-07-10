using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;
using System.Globalization;

namespace IEX.ElementaryActions.FunctionalityCS.EAs.MediaCentre
{
    class PlaybackMCContent : IEX.ElementaryActions.BaseCommand
    {

        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private MediaContent _Content;
        private EnumMCPlayMode _PlaybackMode;
        private String _PlaybackSetting;


        // <param name="content">Media Content to play</param>
        // <param name="playMode">Optional Parameter Content PlaybackMode Default = PLAY</param>
        // <param name="playbackSetting">Optional Parameter Content playback setting(Slideshow Setting for Picture Playback)</param>
        // <param name="pManager">Manager</param>
        // <remarks>
        // Possible Error Codes:
        // <para>300 - NavigationFailure</para> 
        // <para>301 - DictionaryFailure</para> 
        // <para>302 - EmptyEpgInfoFailure</para> 
        // <para>304 - IRVerificationFailure</para> 
        // <para>322 - VerificationFailure</para> 
        // <para>328 - INIFailure</para>    
        // <para>332 - NoValidParameters</para> 
        // </remarks>


        public PlaybackMCContent(MediaContent content, EnumMCPlayMode playMode,String playbackSetting,IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._Content = content;
            this._PlaybackMode = playMode;
            this._PlaybackSetting = playbackSetting;
            this._manager = pManager;
            EPG = this._manager.UI;
        }


        protected override void Execute()
        {
            IEXGateway._IEXResult res = null;
            // Validate input parameters
            if (_Content.Equals(null))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Media Content object is null!"));
            }
            if ((_PlaybackMode.Equals(null)) && !String.IsNullOrEmpty(_PlaybackSetting))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Cannot pass playback setting without specifying the play mode!"));
            }

            switch(_Content.Type.ToUpper())
            {
                case "MUSIC":
                    EPG.MediaCentre.PlayAudioContent(_Content);
                    res = _manager.CheckForAudio(true, 10);
                    if (!res.CommandSucceeded)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.AudioNotPresent, res.FailureReason));
                    }

                    break;

                case "VIDEOS":
                    EPG.MediaCentre.PlayVideoContent(_Content);
                    res = _manager.CheckForVideo(true, true, 10);
                    if (!res.CommandSucceeded)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VideoNotPresent, res.FailureReason));
                    }
                    break;

                case "PICTURES":
                    EPG.MediaCentre.PlayPictureContent(_Content, _PlaybackMode, _PlaybackSetting);
                    break;
                default:
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"));
                    break;
            }
        }
    }
}
