Imports FailuresHandler

Public Class EpgEvent
    Dim res As IEXGateway.IEXResult
    Dim Sreturnvalue As String

    Private _name As String = String.Empty
    Private _startTime As String = String.Empty
    Private _endTime As String = String.Empty
    Private _channel As String = String.Empty
    Private _rightPresses As Integer = -1
    Private _status As String = String.Empty
    'Actual Recording Duration
    Private _duration As Long = 0
    'The Length Of The Event
    Private _originalDuration As Long = 0
    Private _eventDate As String = String.Empty
    Private _eventDateAsDate As Date
    Private _time As String = String.Empty
    Private _details As String = String.Empty
    Private _convertedDate As String = String.Empty
    Private _playedDuration As Long = 0
    Private _source As String = String.Empty
    Private _occurrences As Integer = -1
    Private _frequency As String = String.Empty
    Private _isSeries As Boolean = False
    Private _utils As Utils

#Region " Constructors "

    Sub New(utils As Utils)
        Me._utils = utils
    End Sub

    Sub New(utils As Utils, ByVal _event As EpgEvent)
        Me._utils = utils
        _name = _event.Name
        _startTime = _event.StartTime
        _endTime = _event.EndTime
        _eventDate = _event.EventDate
        _eventDateAsDate = _event.EventDateAsDate
    End Sub

#End Region

#Region " Properties "
    Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Overridable Property StartTime() As String
        Get
            Return _startTime
        End Get
        Set(ByVal value As String)
            _startTime = value
        End Set
    End Property

    Overridable Property EndTime() As String
        Get
            Return _endTime
        End Get
        Set(ByVal value As String)
            _endTime = value
        End Set
    End Property

    Overridable Property Channel() As String
        Get
            Return _channel
        End Get
        Set(ByVal value As String)
            _channel = value
        End Set
    End Property

    Overridable Property RightPresses() As Integer
        Get
            Return _rightPresses
        End Get
        Set(ByVal value As Integer)
            _rightPresses = value
        End Set
    End Property

    Overridable Property Status() As String
        Get
            Return _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property

    Overridable Property Duration() As Long
        Get
            Return _duration
        End Get
        Set(ByVal value As Long)
            _duration = value
        End Set
    End Property

    Overridable Property OriginalDuration() As Long
        Get
            Return _originalDuration
        End Get
        Set(ByVal value As Long)
            _originalDuration = value
        End Set
    End Property

    Overridable Property PlayedDuration() As Long
        Get
            Return _playedDuration
        End Get
        Set(ByVal value As Long)
            _playedDuration = value
        End Set
    End Property

    Overridable Property EventDate() As String
        Get
            Return _eventDate
        End Get
        Set(ByVal value As String)
            _eventDate = value
        End Set
    End Property

    Overridable Property EventDateAsDate() As Date
        Get
            Return _eventDateAsDate
        End Get
        Set(ByVal value As Date)
            _eventDateAsDate = value
        End Set
    End Property

    Overridable Property Time() As String
        Get
            Return _time
        End Get
        Set(ByVal value As String)
            _time = value
        End Set
    End Property

    Overridable Property ConvertedDate() As String
        Get
            Return _convertedDate
        End Get
        Set(ByVal value As String)
            _convertedDate = value
        End Set
    End Property

    Overridable Property Source() As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            _source = value
        End Set
    End Property

    Overridable Property Occurrences() As Integer
        Get
            Return _occurrences
        End Get
        Set(ByVal value As Integer)
            _occurrences = value
        End Set
    End Property

    Overridable Property Details() As String
        Get
            Return _details
        End Get
        Set(ByVal value As String)
            _details = value
        End Set
    End Property

    Overridable Property Frequency() As String
        Get
            Return _frequency
        End Get
        Set(ByVal value As String)
            _frequency = value
        End Set
    End Property
    
    Overridable Property IsSeries() As Boolean
        Get
            Return _IsSeries
        End Get
        Set(ByVal value As Boolean)
            _IsSeries = value
        End Set
    End Property
    
#End Region

#Region " Functions "

    Public Overrides Function Equals(obj As Object) As Boolean
        Try
            Dim EventCompared As EpgEvent = DirectCast(obj, EpgEvent)

            Return Me.Name.Equals(EventCompared.Name) AndAlso _
            Me.StartTime.Equals(EventCompared.StartTime) AndAlso _
            Me.EndTime.Equals(EventCompared.EndTime) AndAlso _
            Me.EventDate.Equals(EventCompared.EventDate)

        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function CompareEventsByName(event1 As EpgEvent, event2 As EpgEvent) As Integer
        Return event1.Name.CompareTo(event2.Name)
    End Function

    Private Function CompareEventsByStartTime(event1 As EpgEvent, event2 As EpgEvent) As Integer
        Return _utils._DateTime.Subtract(event1.StartTime, event2.StartTime)
    End Function

    Private Function CompareEventsByDate(event1 As EpgEvent, event2 As EpgEvent) As Integer
        Return _utils._DateTime.Subtract(event1.EventDateAsDate, event2.EventDateAsDate)
    End Function

    Private Function CompareEventsByA_Z(event1 As EpgEvent, event2 As EpgEvent) As Integer
        Dim CompareResult As Integer = CompareEventsByName(event1, event2)

        If CompareResult = 0 Then
            Return CompareEventsByStartTime(event1, event2)
        End If

        Return CompareResult
    End Function

    Public Sub VerifyEventBeforeByDateFirst(olderEvent As EpgEvent)
        _utils.LogCommentImportant("Comparing By Time : Date - " + Me.EventDate + " StartTime - " + Me.StartTime + " Name - " + Me.Name)
        _utils.LogCommentImportant("With : Date - " + olderEvent.EventDate + " StartTime - " + olderEvent.StartTime + " Name - " + olderEvent.Name)

        If Me.EventDate = olderEvent.EventDate Then
            If Me.StartTime = olderEvent.StartTime Then
                If CompareEventsByName(Me, olderEvent) > 0 Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventSortingFailure, "Failed To Verify Event Name : " + Me.Name + " Is Before " + olderEvent.Name))
                End If
            Else
                If CompareEventsByStartTime(Me, olderEvent) < 0 Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventSortingFailure, "Failed To Verify Event Start Time : " + Me.StartTime + " Is Before " + olderEvent.StartTime))
                End If
            End If
        Else
            If CompareEventsByDate(Me, olderEvent) < 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventSortingFailure, "Failed To Verify Event Date : " + Me.EventDate + " Is Before " + olderEvent.EventDate))
            End If
        End If
    End Sub

    Public Sub VerifyEventBeforeByNameFirst(olderEvent As EpgEvent)
        _utils.LogCommentImportant("Comparing " + Me.Name + " To " + olderEvent.Name)

        If CompareEventsByA_Z(Me, olderEvent) > 0 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventSortingFailure, "Failed To Verify Event Name : " + Me.Name + " Is Before " + olderEvent.Name))
        End If
    End Sub

#End Region

End Class

