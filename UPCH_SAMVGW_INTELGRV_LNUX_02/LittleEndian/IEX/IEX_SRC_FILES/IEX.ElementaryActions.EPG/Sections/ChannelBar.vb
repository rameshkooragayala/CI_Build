Public Class ChannelBar
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Sub GetEventName(ByRef eventName As String, ByVal isNext As Boolean)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetChannelNumber(ByRef channelNumber As String)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventTimeLeft(ByRef timeLeft As Integer, ByVal isNext As Boolean)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventStartTime(ByRef endTime As String, ByVal isNext As Boolean)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventEndTime(ByRef endTime As String, ByVal isNext As Boolean)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function GetChannelSurfKey(Optional ByVal surfUp As Boolean = True) As String
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

    Overridable Function GetChannelSurfTimeout() As Integer
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

#End Region

#Region "Set Subs"

    Overridable Sub SelectEvent()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub DoTune()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub UnlockEvent()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Sub VerifySurfChannel(Optional ByVal type As String = "")
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
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

    Overridable Function IsChannelBar() As Boolean
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextEvent(Optional ByVal selectEvent As Boolean = True)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelUp(Optional ByVal type As String = "", Optional ByVal withSubtitles As Boolean = False)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelDown(Optional ByVal type As String = "", Optional ByVal withSubtitles As Boolean = False)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Mobile"

    Overridable Sub VerifyChannelNumber(ByVal channelNumber As String)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfToChannel(ByVal channelNumber As String)
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PreRecordEvent()
        _iex.LogComment("This UI.ChannelBar function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

End Class
