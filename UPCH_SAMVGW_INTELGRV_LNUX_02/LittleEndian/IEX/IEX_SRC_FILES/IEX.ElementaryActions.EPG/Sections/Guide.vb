Public Class Guide
    Protected _iex As IEXGateway.IEX
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Sub ErrorWaitValue(ByRef seconds As Integer)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEpgTime(ByRef epgTime As DateTime)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PrevDay()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextDay()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetGuideDate(ByRef returnedDateTime As DateTime)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedChannelNumber(ByRef channelNumber As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventName(ByRef eventName As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventTime(ByRef eventTime As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventStartTime(ByRef startTime As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventEndTime(ByRef endTime As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventTimeLeftToStart(ByRef timeLeft As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventStartTime(ByRef startTime As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventDate(ByRef eventDate As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function GetSurfKey(ByRef surfDirection As String) As String
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

    Overridable Function GetChannelSurfTimeout() As Integer
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

#End Region

#Region "Set Subs"

    Overridable Sub SelectEvent(Optional ByVal isLocked As Boolean = False)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SelectCurrentEvent(Optional ByVal type As String = "")
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RecordSelected()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PauseBetweenIRs()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PurchaseEvent()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RecordEvent()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RecordEvent(ByVal isCurrent As Boolean, ByVal isConflict As Boolean)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetEventReminder()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TypeKeys(ByVal stringToType As String, Optional ByVal verifyFas As Boolean = True)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Function IsGuide() As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsGuideSingleChannel() As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsGuideAdjustTimeline() As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsSelectedMarkedToRecord() As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsSelectedMarkedToRemind() As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsDateTime(ByVal stringTime As String, ByRef rDateTime As DateTime) As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyChannelNums(ByVal expectedServiceNums As ArrayList, ByVal isOnly As Boolean)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CheckEventInfo()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySelectedEventChannel(ByVal channelNum As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySelectedMarkedToRecord()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function VerifyIsPastEvent() As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifySelectedRecording()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
 Overridable Function VerifyDaySkipIcon(ByVal checkFwd As Boolean, ByVal checkrwd As Boolean) As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Function
    Overridable Function VerifyDateinGuidewithexpecteddate(ByVal expectedDate As String, ByVal expectedstarttime As Decimal, ByVal isForward As Boolean) As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Function
    Overridable Function VerifyDateinGuidewithexpecteddate_Genre_ByChannel(ByVal expectedDate As String, ByVal expectedstarttime As Decimal, ByVal isForward As Boolean) As Boolean
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

#End Region

#Region "Navigation Subs"
    Overridable Sub Navigate()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToGuideSingleChannel()
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToGuideAdjustTimeline(ByVal GuideTimeline As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToChannel(ByVal channelNumber As String, Optional ByVal verifyFas As Boolean = True)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextEvent(Optional ByVal times As Integer = 1)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToRecordEvent(Optional ByVal isCurrent As Boolean = True)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextEvent(ByVal currentEvent As EpgEvent, ByRef futureEvent As EpgEvent, Optional ByVal repeat As Integer = 1)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextEventbyEndTime(ByVal currentEvent As EPG.EpgEvent, ByRef futureEvent As EPG.EpgEvent, Optional ByVal repeat As Integer = 1)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PreviousEvent(Optional ByVal times As Integer = 1)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub MoveChannelUpDown(ByVal isUp As Boolean)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelUp(Optional ByVal type As String = "")
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelDown(Optional ByVal type As String = "")
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelRight(Optional ByVal type As String = "")
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelLeft(Optional ByVal type As String = "")
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub FindEventByName(ByVal eventName As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub FindEventByTime(ByVal eventTime As String)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub FindEventByStartTime(ByVal eventTime As String, Optional ByVal isExactTime As Boolean = True, Optional ByVal daysDelay As Integer = 0, Optional ByVal maxLookup As Integer = 18)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
 Overridable Sub DaySkipinGuide(ByVal isForward As Boolean)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
 Overridable Sub VerifyDateStarTimeDisplayIcon(ByVal _isGridInCurrentDate As Boolean, ByVal _numberOfPresses As Integer, ByVal _isForward As Boolean, ByVal _isDisplayIconVerify As Boolean)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")

    End Sub
#End Region

End Class
