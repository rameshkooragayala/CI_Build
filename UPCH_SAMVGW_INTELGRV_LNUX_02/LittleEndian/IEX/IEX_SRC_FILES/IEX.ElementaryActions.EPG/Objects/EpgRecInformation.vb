Public Class EpgRecInformation
    Public _eventName As String
    Public _eventSTBTime As String
    Public _logTime As String
    Public _eventChannel, _eventChannelName As String
    Public _status As String
    Public _eventDuration As Integer
    Public _isPlayed As Boolean

    Public Sub New(eventName As String, eventChannel As String, eventStbTime As String, logTime As String, Optional eventChannelName As String = "", Optional eventDuration As Integer = 0, Optional status As String = "", Optional isPlayed As Boolean = False)
        Me._eventName = _eventName
        Me._eventChannel = _eventChannel
        Me._eventChannelName = _eventChannelName
        Me._eventSTBTime = _eventStbTime
        Me._logTime = _logTime
        Me._eventDuration = _eventDuration
        Me._status = _status
        Me._isPlayed = False
    End Sub

    Public Sub New()

    End Sub

    Public Overrides Function ToString() As String
        Dim returnValue = " The event: " & Me._eventName & _
               " in channel " & Me._eventChannel & _
               " on " & Me._eventSTBTime & " STB Time , " _
                      & Me._logTime & " Log time."
        If _eventDuration > 0 Then
            returnValue += " Event duration is: " & _eventDuration & " . "
        End If
        If Not String.IsNullOrEmpty(Me._status) Then
            returnValue += " Recording Status : " & Me._status & " . "
        End If

        Return returnValue

    End Function

End Class