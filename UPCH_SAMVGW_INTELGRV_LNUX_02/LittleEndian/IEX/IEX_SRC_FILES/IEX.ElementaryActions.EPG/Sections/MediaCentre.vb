Public Class MediaCentre
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim Sreturnvalue As String
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Navigate"

    Overridable Sub Navigate(Optional fromDeviceNavigator As Boolean = True, Optional contentType As String = "")
        _iex.LogComment("This UI.MediaCentre function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToContent(content As MediaContent)
        _iex.LogComment("This UI.MediaCentre function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Play Functions"
    Overridable Sub PlayAudioContent(content As MediaContent)
        _iex.LogComment("This UI.MediaCentre function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PlayVideoContent(content As MediaContent)
        _iex.LogComment("This UI.MediaCentre function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PlayPictureContent(content As MediaContent, playMode As EnumMCPlayMode, Optional playbackSettings As String = "")
        _iex.LogComment("This UI.MediaCentre function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetSlideshowSettings(slideshowSettings As String)
        _iex.LogComment("This UI.MediaCentre function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

End Class
