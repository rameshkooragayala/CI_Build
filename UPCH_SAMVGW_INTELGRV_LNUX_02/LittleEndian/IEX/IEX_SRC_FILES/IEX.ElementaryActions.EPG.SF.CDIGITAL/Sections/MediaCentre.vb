Imports FailuresHandler

Public Class MediaCentre

    Inherits IEX.ElementaryActions.EPG.SF.MediaCentre

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Playback Picture content
    ''' </summary>
    ''' <param name="content"> Media Content Object</param>
    ''' <param name="PlayMode">Content Playback Mode : PLAY,SLIDESHOW</param>
    ''' <param name="PlaybackSetting">Content Playback Setting : Slideshow Setting</param>
    ''' <remarks></remarks>
    Public Overrides Sub PlayPictureContent(ByVal content As MediaContent, ByVal PlayMode As EnumMCPlayMode, Optional ByVal PlaybackSetting As String = "")

        Dim dictionary As Dictionary(Of EnumEpgKeys, String) = New Dictionary(Of EnumEpgKeys, String)
        _UI.Utils.SendIR("SELECT", 500)

        If Not _UI.Utils.VerifyState("SLIDE SHOW") Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify the picture playback state"))
        End If

        If PlayMode = EnumMCPlayMode.SLIDESHOW Then

            _UI.Utils.EPG_Milestones_Navigate("ACTION BAR")
            _UI.Utils.EPG_Milestones_SelectMenuItem("START SLIDESHOW")
            _UI.Utils.SendIR("SELECT", 500)
            SetSlideshowSettings(PlaybackSetting)

            If Not _UI.Utils.VerifyState("SLIDE SHOW") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify slideshow state"))
            End If

        End If
    End Sub


End Class
