Public Class TrickModes
    Dim res As IEXGateway.IEXResult
    Protected _iex As IEXGateway.IEX
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Sub GetCurrentPlaybackTime(ByRef pbTime As String)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetCurrentPlaybackDuration(ByRef pbDurationSec As Integer)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetCurrentPosition(ByRef position As String)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetStreamPosition(ByRef position As Integer)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function GetSkipForwardKey() As String
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

    Overridable Function GetSkipBackwardKey() As String
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

#End Region

#Region "Set Subs"

    Overridable Sub SetSpeed(speed As Double)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetSkip(isForward As Boolean, playbackContext As Boolean, skipDurationSettings As String, Optional numOfSkipPoints As Integer = Nothing)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PlayEvent(ev As EpgEvent)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub StopPlayEvent(isReviewBuffer As Boolean)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetPlaybackTime(time As String)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetBookmark()
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub DoSkip(playbackContext As Boolean, ByVal isForward As Boolean, verifyEofBof As Boolean, skipDurationSettings As String, numOfSkipPoints As Integer)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Function IsTrickMode() As Boolean
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyPlayback(ev As EpgEvent, channel As EpgChannel, duration As Integer, hasVideo As Boolean)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyStartOfPlayback()
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyEndOfPlayback()
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySpeedChanged(ir As String, speed As Double, timeout As Integer)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyEofBof(duration As Long, speed As Double, isReviewBuffer As Boolean, isReviewBufferFull As Boolean, eof As Boolean)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function VerifySkip(playbackContext As Boolean, skipDurationSettings As String, verifyEofBof As Boolean, ByRef eofBofReached As Boolean, isForward As Boolean) As Boolean
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate(speed As Double, isSkip As Boolean)
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RaiseTrickMode()
        _iex.LogComment("This UI.TrickModes function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class
