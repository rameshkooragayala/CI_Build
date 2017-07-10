Public MustInherit Class PlannerBase
    Dim res As IEXGateway.IEXResult
    Protected _iex As IEXGateway._IEX
    Protected _UI As EPG.UI

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Sub GetNumberOfEvents(ByRef eventsNumber As Integer)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventName(ByRef eventName As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventDuration(ByRef eventDuration As Integer)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventTime(ByRef eventTime As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventDetails(ByRef eventDetails As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventStatus(ByRef eventStatus As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventNumber(ByRef eventNumber As Integer)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventState(ByRef state As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetAdultEventName(ByRef eventName As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Set Subs"

    Overridable Sub SelectEvent(Optional ByVal eventName As String = "", Optional ByVal isSeries As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PressSelect()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PlayEvent(Optional fromTheBeginning As Boolean = True, Optional enterPin As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PlaybackFromPoint(ByVal startMinute As Integer)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub StopPlayEvent()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub StopRecordingEvent()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub DeleteEvent(Optional inReviewBuffer As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateBeforeDelete(navigateToArchive As Boolean)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub DeleteAllEvents(navigateToArchive As Boolean)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub LockEvent()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ModifyEvent()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub UnlockEvent()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub DeleteAll()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CancelEvent(Optional ByVal shouldSucceed As Boolean = True, Optional ByVal isSeriesEvent As Boolean = False, Optional ByVal isComplete As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CancelAllEvents()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetFiltering(Optional filterType As String = "", Optional filter As String = "")
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ResolveAdultEventName()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"
    Public Delegate Function _degIsEventReminder()

    Public degIsEventReminder As _degIsEventReminder


    Overridable Function IsEventReminder() As Boolean
        degIsEventReminder()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsEmpty() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsEventKeep() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsEventSeries() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsEventCollapse() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsEventSelected() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyEvent(Optional eventName As String = "")
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyArchiveEmpty()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyPlannerEmpty()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function IsEventRecording() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyEventName(eventName As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyEventStatus(eventStatus As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function IsPlanner() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsArchive() As Boolean
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyPlaybackEnded(eventDurationInMin As Integer)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyRecurring(eventKeyName As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyPlanner()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyMyLibrary()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyArchive()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySorting(byTime As Boolean)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyErrorInfo(recordIcon As String, errDescription As String)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyStateAfterDelete()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyStateAfterPlayback()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifySeriesEvent()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Sub FindEvent(eventName As String, Optional eventDate As String = "", Optional startTime As String = "", Optional endTime As String = "")
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub Navigate()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextEvent(Optional times As Integer = 1, Optional verifyByDetails As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PreviousEvent(Optional times As Integer = 1, Optional verifyByDetails As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextUpEvent(Optional times As Integer = 1, Optional verifyByDetails As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToFailedEventScreen()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub FindFailedRecordedEvent(eventName As String, Optional eventDate As String = "", Optional startTime As String = "", Optional endTime As String = "")
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextFailedEvent(Optional ByVal times As Integer = 1, Optional ByVal verifyByDetails As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextDownEvent(Optional ByVal times As Integer = 1, Optional ByVal verifyByDetails As Boolean = False)
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub ExpandSeries()
        _iex.LogComment("This UI.PlannerBase function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class
