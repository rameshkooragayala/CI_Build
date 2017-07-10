Imports System.Text.RegularExpressions

Public Class EAException
    Inherits IEX.ElementaryActions.Objects.StopEA
    Private _msg As String
    Private _shortMsg As String

    Public Overrides ReadOnly Property Message As String
        Get
            Return _msg
        End Get
    End Property

    Public Property ShortMessage() As String
        Get
            Return _shortMsg
        End Get
        Set(ByVal value As String)
            _shortMsg = value
        End Set
    End Property

    Sub New(ByVal NewExitCodeType As ExitCodes, ByVal Msg As String)
        MyBase.New(NewExitCodeType, Msg, False, False)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

    Sub New(ByVal ExitCodeValue As Integer, ByVal ExitCodeName As String, ByVal Msg As String)
        MyBase.New(ExitCodeValue, ExitCodeName, Msg, False, False)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub
End Class

Public Class IEXException
    Inherits IEX.ElementaryActions.Objects.StopEA
    Private _msg As String
    Private _shortMsg As String

    Public Property ShortMessage() As String
        Get
            Return _shortMsg
        End Get
        Set(ByVal value As String)
            _shortMsg = value
        End Set
    End Property

    Public Overrides ReadOnly Property Message As String
        Get
            Return _msg
        End Get
    End Property

    Sub New(ByVal res As IEXGateway._IEXResult)
        MyBase.New(res, False, True)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

    Sub New(ByVal res As IEXGateway._IEXResult, ByVal Msg As String)
        MyBase.New(res.FailureCode, res.FailureType, Msg, False, True)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

    Sub New(ByVal ExitCodeValue As Integer, ByVal ExitCodeName As String, ByVal Msg As String)
        MyBase.New(ExitCodeValue, ExitCodeName, Msg, False, True)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

End Class

Public Class IEXWarnning
    Inherits IEX.ElementaryActions.Objects.StopEA
    Private _msg As String
    Private _shortMsg As String

    Public Property ShortMessage() As String
        Get
            Return _shortMsg
        End Get
        Set(ByVal value As String)
            _shortMsg = value
        End Set
    End Property

    Sub New(ByVal res As IEXGateway._IEXResult)
        MyBase.New(res, True, False)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

    Sub New(ByVal ExitCodeValue As Integer, ByVal ExitCodeName As String, ByVal Msg As String)
        MyBase.New(ExitCodeValue, ExitCodeName, Msg, True, False)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

    Sub New(ByVal ExitCode As ExitCodes, ByVal Msg As String)
        MyBase.New(ExitCode, Msg, False, True)
        ShortMessage = MyBase.Message
        _msg = MyBase.Message + ExceptionUtils.ParseStack
    End Sub

End Class

Public Class ExceptionUtils
    Private Shared _tmpStack As String
    Private Shared _ignoredExceptions As New ArrayList
    Private Shared _iex As IEXGateway.IEX

    Public Shared Property ExIEX As IEXGateway.IEX
        Get
            Return _iex
        End Get
        Set(ByVal value As IEXGateway.IEX)
            _iex = value
        End Set
    End Property

    Public Shared ReadOnly Property IgnoredExceptions As ArrayList
        Get
            Return _ignoredExceptions
        End Get
    End Property

    Public Shared ReadOnly Property TempStack As String
        Get
            Return _tmpStack
        End Get
    End Property

    Public Shared Function ParseStack() As String
        Dim stack As New StackTrace
        Dim CountExecutes As Integer = Regex.Matches(stack.ToString, Regex.Escape("ManagerExecute()")).Count
        Dim StackString As String() = stack.ToString.Split(vbCrLf)
        Dim Msg As String = ""

        If _tmpStack <> "" And CountExecutes = 1 Then
            Msg = _tmpStack
            _tmpStack = ""
            Return vbCrLf + " At " + Msg
        End If

        For i As Integer = StackString.Length - 1 To 2 Step -1
            Dim line As String = StackString(i)
            If line.Contains(".EPG.") Then
                line = line.Remove(0, line.IndexOf(".EPG.") + 5)
                Msg += line.Substring(0, line.IndexOf("(")) + " -- "
            ElseIf line.Contains(".EAImplementation.") Then
                line = line.Remove(0, line.IndexOf(".EAImplementation.") + 18)
                Try
                    Msg += "EA." + line.Substring(0, line.IndexOf(".Execute()")) + " -- "
                Catch ex As Exception
                End Try
            End If
        Next

        Msg = Msg.Remove(Msg.LastIndexOf(" -- "), 4)

        If CountExecutes > 1 Then
            _tmpStack = Msg
            Return ""
        End If

        _tmpStack = ""
        Return " At " + Msg
    End Function

    Public Shared Function ThrowEx(ByVal MyException As Exception) As Boolean
        If TypeOf (MyException) Is IEXException Then
            For Each errorCode As String In ExceptionUtils.IgnoredExceptions
                If DirectCast(MyException, IEXException).ExitCodeValue = errorCode Then
                    _iex.LogComment("Ignoring Error Code : " + errorCode, True, False, False, CByte(10), "Orange", True)
                    Return True
                End If
            Next

            Throw New IEXException(DirectCast(MyException, IEXException).ExitCodeValue, DirectCast(MyException, IEXException).ExitCodeName, DirectCast(MyException, IEXException).ShortMessage)

        ElseIf TypeOf (MyException) Is EAException Then

            For Each errorCode As String In ExceptionUtils.IgnoredExceptions
                If DirectCast(MyException, EAException).ExitCodeValue = errorCode Then
                    _iex.LogComment("Ignoring Error Code : " + errorCode, True, False, False, CByte(10), "Orange", True)
                    Return True
                End If
            Next

            Throw New EAException(DirectCast(MyException, EAException).ExitCode, DirectCast(MyException, EAException).ShortMessage)
        Else
            Return True
        End If
    End Function

    Public Shared Function InsertToIgnoredExceptions(ByVal ParamArray ExitCodes() As ExitCodes) As Boolean
        Try
            _ignoredExceptions = New ArrayList(ExitCodes)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Shared Function InsertToIgnoredExceptions(ByVal ExitCodes As String) As Boolean

        Try
            Dim ECodes As String() = ExitCodes.Split(",")
            For Each ECode As String In ECodes
                _ignoredExceptions.Add(ECode)
            Next
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Shared Function ClearIgnoredExceptions() As Boolean
        Try
            _ignoredExceptions.Clear()
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

End Class

Public Enum ExitCodes
    'Milestones
    NavigationFailure = 300
    DictionaryFailure = 301
    EmptyEpgInfoFailure = 302
    FasVerificationFailure = 303
    IRVerificationFailure = 304
    PCATFailure = 305

    'Get
    GetEventInfoFailure = 306
    GetStreamInfoFailure = 307
    GetChannelFailure = 308
    GetEpgTimeFailure = 309
    GetEpgDateFailure = 310

    'Set
    SetEventReminderFailure = 311
    SetEventKeepFailure = 312
    SetTrickModeSpeedFailure = 313
    SetSettingsFailure = 314
    SetManualRecordingParamFailure = 315
    SetEventInfoFailure = 316
    SetAudioTrackFailure = 317
    SetSkipFailure = 318
    SetSubtitlesLanguageFailure = 319

    'Verify 
    VerifyEofBofFailure = 320
    VerifyChannelAttributeFailure = 321
    VerificationFailure = 322
    VerifyStateFailure = 323

    'Mount
    MountFailure = 324
    MountGwFailure = 325
    MountClientFailure = 326
    SyncFailure = 327
    STBReadyFailure = 363

    'IEX System
    INIFailure = 328
    IEXSystemError = 329
    TelnetFailure = 330
    CopyFileFailure = 331
    NoValidParameters = 332

    'Video/Audio
    VideoPresent = 333
    VideoNotPresent = 334
    AudioPresent = 335
    AudioNotPresent = 336

    'Events
    ParseEventTimeFailure = 337
    EventNotExistsFailure = 338
    RecordEventFailure = 339
    PlayEventFailure = 340
    PauseEventFailure = 341
    CancelEventFailure = 342
    StopPlayEventFailure = 343
    ConflictFailure = 344
    DeleteEventFailure = 345
    FindEventFailure = 346
    SelectEventFailure = 347
    StopRecordEventFailure = 348
    EventSortingFailure = 365
    CancelReminderFailure = 368

    'EPG
    ReturnToLiveFailure = 349
    ParsingFailure = 350
    SurfingFailure = 351
    ChannelAnimationFailure = 352
    RaiseTrickModeFailure = 353
    ReminderFailure = 354
    TuneToChannelFailure = 355
    STBCrashFailure = 356
    LockUnlockChannelFailure = 357
    ModifyEventFailure = 364
    StandByFailure = 358
    LockUnlockFailure = 359
    HealthCheckFailure = 360
    PINFailure = 361
    TimeFailure = 362
	FactoryResetPVRFailure = 370

    'Mobile
    AnimationFailure = 366
    SwipeNotAvaiable = 367

    'PPV
    RentPPVEventFailure = 369
    CreateCarausalFailure = 371
	BroadcastCarausalFailure = 372
    'Last Error Code = 370
	 'rms
    LoginFailure = 375
    BoxIdFormatFailure = 376
    ClickParamFailure = 377
    NavigateFailure = 378
End Enum