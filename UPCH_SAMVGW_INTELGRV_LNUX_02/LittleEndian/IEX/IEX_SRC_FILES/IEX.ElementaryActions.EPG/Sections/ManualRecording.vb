Public Class ManualRecording
    Dim res As IEXGateway.IEXResult
    Protected _iex As IEXGateway.IEX
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Sub GetSelectedChannelName(ByRef channelName As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedDate(ByRef tDate As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventDuration(ByRef eventDuration As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventStartTime(ByRef eventStartTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventEndTime(ByRef eventEndTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetFrequency(ByRef frequency As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Set Subs"

    Overridable Sub SetDate(tDate As String, defaultValue As Boolean)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetDay(day As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetStartTime(startTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetEndTime(endTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetSchedule(recStartTime As String, recEndTime As String, Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetHour(ByVal hour As DateTime)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetMinutes(hour As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetFrequency(frequency As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetChannel(channelName As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function SetChannel(channel As Integer) As String
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Sub SaveAndEnd(Optional isCurrent As Boolean = False, Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TypeStartTime(startTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TypeDate(dateString As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TypeChannel(channel As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TypeEndTime(endTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetManualRecordingParams(ByVal recDate As String, ByVal recStartTime As String, ByVal recChannelName As String, ByVal recChannelNum As Integer, ByVal recEndTime As String, ByVal frequency As String, Optional ByVal isFirstTime As Boolean = False, Optional ByVal isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Verify Subs"

    Overridable Sub VerifyStateId(state As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySaveAndEndFinished(isFromCurrent As Boolean)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyStartTime(startTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyEndTime(endTime As String)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate(Optional fromCurrent As Boolean = False, Optional noEit As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateFromPlanner()
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToChannel(Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToStartTime(Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToEndTime(Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToDate(Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToFrequency(Optional isModify As Boolean = False)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToRecord(isFromCurrent As Boolean)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextChannel(Optional times As Integer = 1)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextDate(Optional times As Integer = 1)
        _iex.LogComment("This UI.ManualRecording function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class
