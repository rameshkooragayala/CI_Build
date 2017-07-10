Public Class Live
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Sub GetEpgTime(ByRef time As String)
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function GetSurfKey(Optional surfUp As Boolean = True) As String
        _iex.LogComment("This UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

    Overridable Function GetChannelSurfTimeout() As Integer
        _iex.LogComment("This UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

    Overridable Sub GetEpgDate(ByRef epgDate As String)
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetChannelNumber(ByRef channelNumber As String)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Set Subs"

    Overridable Sub TuningToChannel(ByVal channelNumber As String, Optional ByVal type As String = "", Optional ByVal withSubtitles As Boolean = False)
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelUp(Optional type As String = "", Optional withSubtitles As Boolean = False)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelDown(Optional type As String = "", Optional withSubtitles As Boolean = False)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Function IsLive() As Boolean
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyChannelNumber(channelNumber As String)
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySurfChannelIgnore()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySurfChannelPredicted()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySurfChannelNotPredicted()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub WaitAfterLiveReached()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyTuneToChannel(ByVal type As String)
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyTuneToChannelPredicted()
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyTuneToChannelNotPredicted()
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyTuneToChannelSubtitles()
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyTuneToRadioChannel()
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyRadioStateReached()
        _iex.LogComment("UI.Live function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Navigation Subs"

#End Region

End Class
