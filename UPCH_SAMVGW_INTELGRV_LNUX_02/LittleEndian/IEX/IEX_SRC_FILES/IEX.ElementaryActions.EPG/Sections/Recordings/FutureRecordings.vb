Public Class FutureRecordings
    Dim res As IEXGateway.IEXResult
    Protected _iex As IEXGateway._IEX
    Protected _UI As EPG.UI

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _UI = pUI
        _iex = pIex
    End Sub

#Region "Get Subs"

    Overridable Sub GetNumberOfEvents(ByRef eventsNumber As Integer)
        _UI.PlannerBase.GetNumberOfEvents(eventsNumber)
    End Sub

    Overridable Sub GetSelectedEventName(ByRef eventName As String)
        _UI.PlannerBase.GetSelectedEventName(eventName)
    End Sub

    Overridable Sub GetSelectedEventDuration(ByRef eventDuration As Integer)
        _UI.PlannerBase.GetSelectedEventDuration(eventDuration)
    End Sub

    Overridable Sub GetSelectedEventTime(ByRef eventTime As String)
        _UI.PlannerBase.GetSelectedEventTime(eventTime)
    End Sub

    Overridable Sub GetSelectedEventDetails(ByRef eventDetails As String)
        _UI.PlannerBase.GetSelectedEventDetails(eventDetails)
    End Sub

    Overridable Sub GetSelectedEventStatus(ByRef eventStatus As String)
        _UI.PlannerBase.GetSelectedEventStatus(eventStatus)
    End Sub

    Overridable Sub GetSelectedEventNumber(ByRef eventNumber As Integer)
        _UI.PlannerBase.GetSelectedEventNumber(eventNumber)
    End Sub

    Overridable Sub GetEventState(ByRef state As String)
        _UI.PlannerBase.GetEventState(state)
    End Sub

    Overridable Sub GetAdultEventName(ByRef eventName As String)
        _UI.PlannerBase.GetAdultEventName(eventName)
    End Sub

#End Region

#Region "Set Subs"

    Overridable Sub SelectEvent(Optional ByVal eventName As String = "", Optional ByVal isSeries As Boolean = False)
        _UI.PlannerBase.SelectEvent(eventName)
    End Sub

    Overridable Sub PlayEvent(Optional fromTheBeginning As Boolean = True, Optional enterPin As Boolean = False)
        _UI.PlannerBase.PlayEvent(fromTheBeginning, enterPin)
    End Sub

    Overridable Sub PlaybackFromPoint(startMinute As Integer)
        _UI.PlannerBase.playbackFromPoint(startMinute)
    End Sub

    Overridable Sub StopPlayEvent()
        _UI.PlannerBase.StopPlayEvent()
    End Sub

    Overridable Sub StopRecordingEvent()
        _UI.PlannerBase.StopRecordingEvent()
    End Sub

    Overridable Sub DeleteEvent(Optional inReviewBuffer As Boolean = False)
        _UI.PlannerBase.DeleteEvent(inReviewBuffer)
    End Sub

    Overridable Sub LockEvent()
        _UI.PlannerBase.LockEvent()
    End Sub

    Overridable Sub ModifyEvent()
        _UI.PlannerBase.ModifyEvent()
    End Sub

    Overridable Sub UnlockEvent()
        _UI.PlannerBase.UnlockEvent()
    End Sub

    Overridable Sub DeleteAll()
        _UI.PlannerBase.DeleteAll()
    End Sub

    Overridable Sub CancelEvent(Optional ByVal shouldSucceed As Boolean = True, Optional ByVal isSeriesEvent As Boolean = False, Optional ByVal isComplete As Boolean = False)
        _UI.PlannerBase.CancelEvent(shouldSucceed)
    End Sub

    Overridable Sub CancelAllEvents()
        _UI.PlannerBase.CancelAllEvents()
    End Sub

    Overridable Sub SetFiltering(Optional filterType As String = "", Optional filter As String = "")
        _UI.PlannerBase.SetFiltering(filterType, filter)
    End Sub

    Overridable Sub ResolveAdultEventName()
        _UI.PlannerBase.ResolveAdultEventName()
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Function IsEventReminder() As Boolean
        Return _UI.PlannerBase.IsEventReminder()
    End Function

    Overridable Function IsEventKeep() As Boolean
        Return _UI.PlannerBase.IsEventKeep()
    End Function

    Overridable Function IsEventSeries() As Boolean
        Return _UI.PlannerBase.isEventSeries()
    End Function

    Overridable Function IsEventCollapse() As Boolean
        Return _UI.PlannerBase.isEventCollapse()
    End Function

    Overridable Function IsEventSelected() As Boolean
        Return _UI.PlannerBase.isEventSelected()
    End Function

    Overridable Sub VerifyEvent(Optional eventName As String = "")
        _UI.PlannerBase.VerifyEvent(eventName)
    End Sub

    Overridable Function IsEmpty() As Boolean
        Return _UI.PlannerBase.IsEmpty()
    End Function

    Overridable Sub VerifyPlannerEmpty()
        _UI.PlannerBase.VerifyPlannerEmpty()
    End Sub

    Overridable Function IsEventRecording() As Boolean
        Return _UI.PlannerBase.isEventRecording()
    End Function

    Overridable Sub VerifyEventname(eventName As String)
        _UI.PlannerBase.VerifyEventname(eventName)
    End Sub

    Overridable Sub VerifyEventStatus(eventStatus As String)
        _UI.PlannerBase.VerifyEventStatus(eventStatus)
    End Sub

    Overridable Function IsPlanner() As Boolean
        Return _UI.PlannerBase.isPlanner()
    End Function

    Overridable Function IsArchive() As Boolean
        Return _UI.PlannerBase.isArchive()
    End Function

    Overridable Sub VerifyPlaybackEnded(eventDurationInMin As Integer)
        _UI.PlannerBase.VerifyPlaybackEnded(eventDurationInMin)
    End Sub

    Overridable Sub VerifyRecurring(eventKeyName As String)
        _UI.PlannerBase.VerifyRecurring(eventKeyName)
    End Sub

    Overridable Sub VerifyPlanner()
        _UI.PlannerBase.VerifyPlanner()
    End Sub

    Overridable Sub VerifyMyLibrary()
        _UI.PlannerBase.VerifyMyLibrary()
    End Sub

    Overridable Sub VerifySorting(byTime As Boolean)
        _UI.PlannerBase.VerifySorting(byTime)
    End Sub

    Overridable Sub VerifyStateAfterDelete()
        _UI.PlannerBase.VerifyStateAfterDelete()
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Sub FindEvent(eventName As String, Optional eventDate As String = "", Optional startTime As String = "", Optional endTime As String = "")
        _UI.PlannerBase.FindEvent(eventName, eventDate, startTime, endTime)
    End Sub

    Overridable Sub Navigate()
        _UI.PlannerBase.Navigate()
    End Sub

    Overridable Sub NextEvent(Optional times As Integer = 1, Optional verifyByDetails As Boolean = False)
        _UI.PlannerBase.NextEvent(times, verifyByDetails)
    End Sub

    Overridable Sub PreviousEvent(Optional times As Integer = 1, Optional verifyByDetails As Boolean = False)
        _UI.PlannerBase.PreviousEvent(times, verifyByDetails)
    End Sub

    Overridable Sub NextUpEvent(Optional times As Integer = 1)
        _UI.PlannerBase.NextUpEvent(times)
    End Sub

    Overridable Sub NextDownEvent(Optional ByVal times As Integer = 1, Optional ByVal verifyByDetails As Boolean = False)
        _UI.PlannerBase.NextDownEvent(times, verifyByDetails)
    End Sub

#End Region

End Class
