Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.TrickModes

    Dim res As IEXGateway._IEXResult
    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Raising Trickmode By Pressing SELECT
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub RaiseTrickMode()

        _Utils.StartHideFailures("Raising TrickMode")

        Try
            _Utils.EPG_Milestones_Navigate("TRICKMODE BAR")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifying EOF/BOF
    ''' </summary>
    ''' <param name="Duration">Duration Of The Event Or Review Buffer</param>
    ''' <param name="Speed">Speed Of Trickmode</param>
    ''' <param name="IsReviewBuffer">If True Checkes Review Buffer EOF/BOF</param>
    ''' <param name="IsReviewBufferFull">If True Review Buffer Is Full</param>
    ''' <param name="EOF">If True Verifies EOF Else BOF</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyEofBof(ByVal Duration As Long, ByVal Speed As Double, ByVal IsReviewBuffer As Boolean, ByVal IsReviewBufferFull As Boolean, ByVal EOF As Boolean)
        Dim LeftDuration As Long
        Dim FullDuration As Long
        Dim FoundBOF As Boolean = False

        _Utils.StartHideFailures("Verifying " + IIf(EOF, "EOF ", "BOF ") + "Duration = " + Duration.ToString + " Speed = " + Speed.ToString + " IsReviewBuffer = " _
                                      + IsReviewBuffer.ToString + " IsReviewBufferFull = " + IsReviewBufferFull.ToString)

        Try

            If Duration < 0 Then Duration = 0

            If IsReviewBuffer Then
                If Speed > 1 Then
                    'LeftDuration = (120 / (speed + 1)) + 2 
                    LeftDuration = ((Duration + 60) / (Math.Abs(Speed) - 1)) - ((Duration - 60) / (Math.Abs(Speed) - 1)) + 2
                    FullDuration = (Duration + 60) / (Math.Abs(Speed) - 1)
                    Duration = (Duration - 60) / (Math.Abs(Speed) - 1) - 1
                ElseIf Speed < 0 And IsReviewBufferFull Then
                    LeftDuration = ((Duration + 60) / (Math.Abs(Speed) + 1)) - ((Duration - 60) / (Math.Abs(Speed) + 1)) + 2
                    FullDuration = (Duration + 60) / (Math.Abs(Speed) + 1)
                    Duration = (Duration - 60) / (Math.Abs(Speed) + 1) - 1
                ElseIf Speed < 0 And IsReviewBufferFull = False Then
                    LeftDuration = ((Duration + 60) / (Math.Abs(Speed))) - ((Duration - 60) / (Math.Abs(Speed))) + 2
                    FullDuration = (Duration + 60) / (Math.Abs(Speed))
                    Duration = (Duration - 60) / (Math.Abs(Speed)) - 1
                End If
            Else
                LeftDuration = ((Duration + 60) / (Math.Abs(Speed))) - ((Duration - 60) / (Math.Abs(Speed))) + 2
                FullDuration = (Duration + 60) / (Math.Abs(Speed))
                Duration = (Duration - 60) / (Math.Abs(Speed)) - 1
            End If

            If Duration < 0 Then Duration = 0

            _Utils.LogCommentImportant("Total Duration = " + FullDuration.ToString + " Not Suppose To Find Duration = " + Duration.ToString + " Suppose To Find Duration = " + LeftDuration.ToString)

            _Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(EOF = True, "EOF", "BOF") + " After " + FullDuration.ToString + " Seconds")

            'MILESTONES MESSAGES
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList
            Dim Msg As String = ""

            If EOF Then
                If IsReviewBuffer Then
                    Msg = "TrickModeEOFInReviewBuffer"
                    Milestones = _Utils.GetValueFromMilestones("TrickModeEOFInReviewBuffer")
                Else
                    Msg = "TrickModeEOFNotInReviewBuffer"
                    Milestones = _Utils.GetValueFromMilestones("TrickModeEOFNotInReviewBuffer")
                End If

            Else 'BOF
                If IsReviewBuffer Then
                    Msg = "TrickModeBOFInReviewBuffer"
                    Milestones = _Utils.GetValueFromMilestones("TrickModeBOFInReviewBuffer")
                Else
                    Msg = "TrickModeBOFNotInReviewBuffer"
                    Milestones = _Utils.GetValueFromMilestones("TrickModeBOFNotInReviewBuffer")
                End If
            End If

            _Utils.StartHideFailures("Verifying " + IIf(EOF = True, "EOF", "BOF") + " Not Found Before Correct Time")

            _Utils.BeginWaitForDebugMessages(Milestones, Duration)

            If _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                FoundBOF = True
                _Utils.LogCommentWarning("WARNING : Found " + IIf(EOF = True, "EOF", "BOF") + " Before Correct Time")
            End If

            _iex.ForceHideFailure()

            If FoundBOF = False Then
                _Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(EOF = True, "EOF", "BOF") + " Found In " + LeftDuration.ToString + " Seconds Left Of Event")

                _Utils.BeginWaitForDebugMessages(Milestones, LeftDuration)

                If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(EOF = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
                End If
            End If

            If EOF And IsReviewBuffer Then
                _Utils.LogCommentInfo("Waiting For STB After Catch Up To LIVE")

                If Not _Utils.VerifyState("LIVE", 25, 15) Then
                    _iex.ForceHideFailure()
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is LIVE"))
                End If

            ElseIf EOF Then
                _iex.Wait(2)
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Sets Speed On Trickmode
    ''' </summary>
    ''' <param name="Speed">Required Speed Can Be : 1 For Play, 0 For Pause,0.5,2,6,12,30 Or -0.5,-2,-6,-12,-30</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>313 - SetTrickModeSpeedFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Overrides Sub SetSpeed(ByVal Speed As Double)
        Dim RequiredSpeed As Double = Math.Abs(Speed)

        _Utils.StartHideFailures("Setting Speed To " + Speed.ToString)

        Try
            If RequiredSpeed = 0 Or RequiredSpeed = 1 Then
                _Utils.StartHideFailures("Trying To Set Speed To " + Speed.ToString)

                Try

                    Try
                        VerifySpeedChanged("PAUSE", Speed, 3)
                        Exit Sub
                    Catch ex As Exception
                        _Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        VerifySpeedChanged("PAUSE", Speed, 3)
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try
            ElseIf RequiredSpeed = 0.5 Then

                _Utils.StartHideFailures("Trying To Pause")

                Try

                    Try
                        VerifySpeedChanged("SELECT", 0, 3)
                    Catch ex As Exception
                        _Utils.LogCommentFail("Failed To Verify Speed : 0 Trying Again")
                        VerifySpeedChanged("SELECT", 0, 3)
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try

                _Utils.LogCommentInfo("Navigating To FF 0.5")

                VerifySpeedChanged("SELECT_RIGHT", Speed, 10)

                Exit Sub
            Else
                Dim Direction As String = ""

                _Utils.StartHideFailures("Trying To Verify Speed " + Speed.ToString)

                Try

                    If Speed < 0 Then
                        Direction = "SELECT_LEFT"
                    Else
                        Direction = "SELECT_RIGHT"
                    End If

                    For i As Integer = 1 To 8
                        Try
                            VerifySpeedChanged(Direction, Speed, 2)
                            _Utils.LogCommentInfo("Verifyied Speed " + Speed.ToString)
                            Exit Sub
                        Catch ex As Exception
                        End Try
                    Next

                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetTrickModeSpeedFailure, "Failed Set Speed To " + Speed.ToString))
                Finally
                    _iex.ForceHideFailure()
                End Try

            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed Set Speed To " + Speed.ToString + " Not A Valid Speed"))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Sending Verifying Speed Change FAS Milestones
    ''' </summary>
    ''' <param name="IR">IR To Send</param>
    ''' <param name="Speed">Speed Of Requirement Trickmode</param>
    ''' <param name="Timeout"></param>
    ''' <remarks></remarks>
    Public Overrides Sub VerifySpeedChanged(ByVal IR As String, ByVal Speed As Double, ByVal Timeout As Integer)
        Dim ActualLines As New ArrayList
        Dim MessagesCol As String() = Nothing
        Dim Rew As String = ""
        Dim Succeed As Boolean

        Try
            _Utils.StartHideFailures("Verifying FAS Speed Change Milestones Messages...")

            If Speed.ToString.Contains("-") Then
                Rew = "-"
            End If

            Speed = Math.Abs(Speed)

            'MILESTONES MESSAGES
            Dim Milestones As String = ""

            Milestones = _Utils.GetValueFromMilestones("TrickModeSpeed")

            Select Case Speed
                Case 0
                    Milestones += "0"
                Case 1
                    Milestones += "1000"
                Case 0.5
                    Milestones += "500"
                Case 2
                    Milestones += Rew + "2000"
                Case 6
                    Milestones += Rew + "6000"
                Case 12
                    Milestones += Rew + "12000"
                Case 30
                    Milestones += Rew + "30000"
            End Select

            MessagesCol = Milestones.Split(",")
            Milestones = MessagesCol(0)

            _Utils.BeginWaitForDebugMessages(Milestones, Timeout)

            _Utils.SendIR(IR, 0)

            Try
                Succeed = _Utils.EndWaitForDebugMessages(Milestones, ActualLines)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            End Try

            If Not Succeed Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            End If

            If MessagesCol.Length > 1 Then

                Milestones = MessagesCol(1)

                _Utils.BeginWaitForDebugMessages(Milestones, Timeout + 2)

                Try
                    Succeed = _Utils.EndWaitForDebugMessages(Milestones, ActualLines)
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
                End Try

                If Not Succeed Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Stopping Played Event By Navigating To Stop On TrickMode
    ''' </summary>
    ''' <param name="IsReviewBuffer">If True Trickmode Is In Review Buffer Else In Playback</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>343 - StopPlayEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub StopPlayEvent(ByVal IsReviewBuffer As Boolean)
        Dim pass As Boolean = False
        Dim Result As String = ""
        Dim State As String = ""

        _Utils.StartHideFailures("Stopping Play Event")

        Try
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList
            Dim Msg As String = ""

            If IsReviewBuffer Then
                Msg = "TrickModeStopInReviewBuffer"
                Milestones = _Utils.GetValueFromMilestones("TrickModeStopInReviewBuffer")
            Else
                Msg = "TrickModeStopNotInReviewBuffer"
                Milestones = _Utils.GetValueFromMilestones("TrickModeStopNotInReviewBuffer")
            End If

            _Utils.BeginWaitForDebugMessages(Milestones, 60)

            _Utils.SendIR("STOP")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If

            If IsReviewBuffer Then
                If Not _Utils.VerifyState("LIVE", 25) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify State Is LIVE"))
                End If
            Else
                _UI.ArchiveRecordings.VerifyArchive()
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <param name="IsForward"> Madatory direction of skip.True if Forward else False. No default value</param>
    ''' <param name="PlaybackContext">Mandatory.whether RB or Playback</param>
    ''' <param name="NumOfSkipPoints">Number Of Skip Point To Press</param>
    ''' <param name="SkipDurationSetting">Current setting value for skip. Default value will be fetched from project ini,this includes the value"Last Event Boundary"</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>313 - SetTrickModeSpeedFailure</para> 
    ''' <para>318 - SetSkipFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>*/
    Public Overrides Sub SetSkip(ByVal isForward As Boolean, ByVal PlaybackContext As Boolean, ByVal SkipDurationSetting As String, Optional ByVal NumOfSkipPoints As Integer = Nothing)

        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim Msg As String = ""
        Dim EventDuration As String = ""
        Dim VerifyEofBof As Boolean = False
        Dim PlaybackContextValue As String = ""
        _Utils.StartHideFailures("Setting Skip " + IIf(IsForward, "Forward ", "Backward ") + "--Playback Context=" + IIf(PlaybackContext, "PLAYBACK ", "RB ") + "--Skip Duration Setting=" + SkipDurationSetting + "--NumOfSkipPoints=" + NumOfSkipPoints.ToString)
        Try

            If PlaybackContext Then
                PlaybackContextValue = "PLAYBACK"
            Else
                PlaybackContextValue = "RB"
            End If

            Select Case PlaybackContextValue
                Case "RB"
                    Try
                        EventDuration = _Utils.GetValueFromProject("RB", "MAX_RB_DEPTH")
                    Catch
                        _Utils.LogCommentWarning("MAX_RB_DEPTH in section RB is missing in your project configuration file. Please add it!")
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed To Fetch MAX_RB_DEPTH from projects.ini "))
                    End Try
                Case "PLAYBACK"
                    Dim PCAT_Id As String = ""
                    Dim EventName As String = ""
                    _Utils.GetEpgInfo("evtname", EventName)
                    If Not _Utils.FindEventInPCAT(PCAT_Id, EnumTables.RECORDINGS, EventName:=EventName) Then
                        _Utils.LogCommentFail("Failed to find PCAT ID for " + EventName + "event in PCAT")
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Failed To Fetch Event duration from PCAT"))
                    End If
                    If Not _Utils.GetEventInfo(PCAT_Id, EnumTables.RECORDINGS, "DURATION", EventDuration) Then
                        _Utils.LogCommentFail("Failed to get EventDuration from PCAT")
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Failed To Fetch Event duration from PCAT"))
                    End If
                    'converting duration to minutes from milliseconds
                    EventDuration = ((Convert.ToInt32(EventDuration) / 1000) / 60)
                    _Utils.LogCommentInfo("Event duration from PCAT:" + EventDuration)
            End Select
            'SkipDuration setting is 0 for checking EVENT BOUNDARY
            'If NumOfSkipPoints is not defined BOF/EOF will be checked by default
            If (NumOfSkipPoints = Nothing AndAlso Not SkipDurationSetting.Equals("0")) Then
                VerifyEofBof = True
                NumOfSkipPoints = (Convert.ToInt32(EventDuration)) * 60 / Convert.ToInt32(SkipDurationSetting)
                _UI.Utils.LogCommentImportant("Skip points after calculation=" + NumOfSkipPoints.ToString)
            End If
            'This function will perform the Skip operation
            DoSkip(PlaybackContext, IsForward, VerifyEofBof, SkipDurationSetting, NumOfSkipPoints)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Gets The Current Playback Duration From Milestones
    ''' </summary>
    ''' <param name="pbDurationSec">Returns The Duration In Seconds</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>307 - GetStreamInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetCurrentPlaybackDuration(ByRef pbDurationSec As Integer)
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Getting Current Playback Duration")

        Try
            _UI.TrickModes.RaiseTrickMode()

            Milestones = _Utils.GetValueFromMilestones("GetReviewBufferCurrentDepth")

            _Utils.LogCommentInfo("TrickModes.GetCurrentPlaybackDuration : Trying To Get Current Playback Duration From FAS Milestone")

            _Utils.BeginWaitForDebugMessages(Milestones, 20)

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetStreamInfoFailure, "Failed To Verify ReviewBufferCurrentDepth Milestones : " + Milestones))
            End If

            Try
                Dim DurationStr As String = Right(ActualLines(0), ActualLines(0).Length - (ActualLines(0).IndexOf(":") + 1)).Replace(" ", "")
                pbDurationSec = CInt(DurationStr) / 1000

                _Utils.LogCommentInfo("FAS Actual Line : " + ActualLines(0).ToString)

                _Utils.LogCommentImportant("Current Playback Duration : " + pbDurationSec.ToString)

            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetStreamInfoFailure, "Failed To GetReviewBufferCurrentDepth From FAS Milestone : " + Milestones))
            End Try

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Gets The Current Stream Postion From FAS (IEX_AFL_FAS_getStreamPosition:)
    ''' </summary>
    ''' <param name="Position">Returned Position</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>307 - GetStreamInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetStreamPosition(ByRef Position As Integer)
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Getting Current Stream Position")

        Try

            Milestones = _Utils.GetValueFromMilestones("GetStreamPosition")

            _Utils.LogCommentInfo("TrickModes.GetStreamPosition : Trying To Get Stream Position From FAS Milestone")

            _Utils.BeginWaitForDebugMessages(Milestones, 20)

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetStreamInfoFailure, "Failed To Verify GetStreamPosition Milestones : " + Milestones))
            End If

            Try
                Dim DurationStr As String = Right(ActualLines(0), ActualLines(0).Length - (ActualLines(0).IndexOf(":") + 1)).Replace(" ", "")
                Position = CInt(DurationStr) / 1000

                _Utils.LogCommentInfo("FAS Actual Line : " + ActualLines(0).ToString)

                _Utils.LogCommentImportant("Current Stream Position : " + Position.ToString)

            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetStreamInfoFailure, "Failed To GetStreamPosition From FAS Milestone : " + Milestones))
            End Try

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Gets The Current Trickmode Position
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetCurrentPosition(ByRef Position As String)
        Dim Msg As String = ""
        Dim ReturnedValues As New Dictionary(Of String, String)

        _UI.Utils.StartHideFailures("Getting Trick Mode Position")

        Try
            _UI.Utils.GetEpgInfo("title", Position)

            Select Case Position
                Case "play_mc"
                    Position = "play"
                Case "stop_mc"
                    Position = "stop"
                Case "skipRW_mc"
                    Position = "skiprew"
                Case "rw_mc"
                    Position = "rew"
                Case "ff_mc"
                    Position = "fwd"
                Case "skipFF_mc"
                    Position = "skipfwd"
                Case "rec_mc"
                    Position = "rec"
                Case "pause_mc"
                    Position = "pause"
                Case Else

                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Return Value Not Valid - " + ReturnedValues("title").ToString))
            End Select

            Msg = "Current Position : " + Position

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub




    ''' <summary>
    ''' This function performs the skip operation
    ''' </summary>
    ''' <param name="PlaybackContext">PLAYBACK or RB</param>
    ''' <param name="isForward">forward(True) or backward(False)</param>
    ''' <param name="VerifyEofBof">to check EOF or BOF(True/False)</param>
    ''' <param name="SkipDurationSetting">Skip duration setting to be checked</param>
    ''' <param name="NumOfSkipPoints">Number of skip operation to be performed</param>
    ''' <remarks></remarks>
    Public Overrides Sub DoSkip(ByVal PlaybackContext As Boolean, ByVal isForward As Boolean, ByVal VerifyEofBof As Boolean, ByVal SkipDurationSetting As String, ByVal NumOfSkipPoints As Integer)

        Dim ActualLines As New ArrayList
        Try
            _Utils.StartHideFailures("Performing Skip operation...")
            'MILESTONES MESSAGES
            Dim IR As String = ""
            Dim Milestones As String = ""
            Dim Milestones_EOFBOF As String = ""
            Dim Milestones_EventBoundary As String = ""
            Dim EofBofReached As Boolean = False
            Dim Marginlines As Integer = 30
            Dim VerifyStatus As Boolean = False

            If isForward Then
                Try
                    IR = _Utils.GetValueFromProject("KEY_MAPPING", "SKIP_FORWARD_KEY")
                Catch
                    _Utils.LogCommentWarning("SKIP_FORWARD_KEY in section KEY_MAPPING is missing in your project configuration file. Please add it!")
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed To Fetch SKIP_FORWARD_KEY from projects.ini "))
                End Try
            Else
                Try
                    IR = _Utils.GetValueFromProject("KEY_MAPPING", "SKIP_BACKWARD_KEY")
                Catch
                    _Utils.LogCommentWarning("SKIP_BACKWARD_KEY in section KEY_MAPPING is missing in your project configuration file. Please add it!")
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed To Fetch SKIP_BACKWARD_KEY from projects.ini "))
                End Try
            End If
            'Skip Duration setting is 0 for EVENT BOUNDARY CHECK
            If SkipDurationSetting.Equals("0") Then
                Milestones = _Utils.GetValueFromMilestones("EventBoundaryCheck")
                '-1 is used as for event boundary check we dont need to perform last skip as it will fail(Num 0f events - 1)
                NumOfSkipPoints -= 1
            ElseIf isForward Then
                Milestones = _Utils.GetValueFromMilestones("SkipForward")
            Else
                Milestones = _Utils.GetValueFromMilestones("SkipBackward")
            End If
            If VerifyEofBof Then
                Milestones_EOFBOF = _Utils.GetValueFromMilestones("SkipEOFBOF")
                _Utils.BeginWaitForDebugMessages(Milestones_EOFBOF, NumOfSkipPoints * 60)
            End If
            '-1 is used as for event boundary check we dont need to perform last skip as it will fail
            For i = 1 To NumOfSkipPoints
                _UI.Utils.LogCommentInfo("Performing Skip Count=" + i.ToString)
                res = _iex.Debug.BeginWaitForMessage(Milestones, Marginlines, 20, IEXGateway.DebugDevice.Udp)
                If Not res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                    Exit For
                End If
                '3 sec wait is needed as log comes after some delay
                _Utils.SendIR(IR, 3000)

                VerifyStatus = VerifySkip(PlaybackContext, SkipDurationSetting, VerifyEofBof, EofBofReached, isForward)

                If (Not VerifyStatus Or EofBofReached = True) Then
                    _UI.Utils.LogCommentImportant("Exiting verifySkip with Status=" + VerifyStatus.ToString + "And EofBofReached=" + EofBofReached.ToString)
                    Exit For
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Function VerifySkip(ByVal PlaybackContext As Boolean, ByVal SkipDurationSetting As String, ByVal VerifyEofBof As Boolean, ByRef EofBofReached As Boolean, ByVal isForward As Boolean) As Boolean

        Dim ActualLine As String = ""
        Dim ActualLines As New ArrayList
        Dim MarginLines As String = ""
        Dim status As Boolean
        status = True
        'Dim LogArray As String()
        Dim SkipPosition As String = ""
        Dim SkipPositionDuration As Integer
        Dim found As Boolean = False
        Dim CurrentStreamPos As String = ""
        Dim NextStreamPos As String = ""
        Dim Milestones_EOFBOF As String = ""
        Dim Milestones As String = ""
        Dim skip_log As String = ""
        EofBofReached = False
        Dim LastSkipEnabled As String = ""
        Dim PlaybackContextValue As String = ""
        'Dim GetStreamLog As String = "getStreamPosition"
        'Dim SetStreamLog As String = "setStreamPosition"
        Try
            _Utils.StartHideFailures("Verifying Skip Change...")
            If PlaybackContext Then
                PlaybackContextValue = "PLAYBACK"
            Else
                PlaybackContextValue = "RB"
            End If

            Try
                LastSkipEnabled = _Utils.GetValueFromProject("SET_SKIP", "LAST_SKIP_ENABLED_" + PlaybackContextValue)
            Catch
                _Utils.LogCommentWarning("LAST_SKIP_ENABLED in section SET_SKIP is missing in your project configuration file. Please add it!")
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed To Fetch LAST_SKIP_ENABLED from projects.ini "))
            End Try



            If SkipDurationSetting.Equals("0") Then
                Milestones = _Utils.GetValueFromMilestones("EventBoundaryCheck")
            ElseIf isForward Then
                Milestones = _Utils.GetValueFromMilestones("SkipForward")
                skip_log = "ORIGIN_END"
            Else
                Milestones = _Utils.GetValueFromMilestones("SkipBackward")
                skip_log = "ORIGIN_END"
            End If

            res = _iex.Debug.EndWaitForMessage(Milestones, ActualLine, MarginLines, IEXGateway.DebugDevice.Udp)
            If Not res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To End Wait For Message")
                status = False
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify FAS  " + Milestones))
                Return status
            End If
            'Skip Duration setting is 0 for EVENT BOUNDARY CHECK
            If SkipDurationSetting.Equals("0") Then
                _UI.Utils.LogCommentBlack("Skip Event boundary check successful")
                Return status
            End If
            'calling the function which does log parsing
            If SkipLogVerification(MarginLines, skip_log, CurrentStreamPos, NextStreamPos) Then
                _UI.Utils.LogCommentInfo("Log parsing Successful")
            Else
                status = False
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to parse logs "))
                Return status
            End If
            'Checking if stream position is not fetched
            If CurrentStreamPos.Equals("") Or NextStreamPos.Equals("") Then
                status = False
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed get stream positions "))
                Return status
            End If
            _UI.Utils.LogCommentImportant("Current stream Position=" + CurrentStreamPos + "And Next stream Position=" + NextStreamPos)
            'Taking the absolute value as for forward it comes in -ve
            SkipPositionDuration = Math.Abs((Convert.ToInt64(CurrentStreamPos) - Convert.ToInt64(NextStreamPos))) / 1000
            _UI.Utils.LogCommentImportant("SkipPositionDuration=" + SkipPositionDuration.ToString)
            'Checking if we get equal skip set.Possible when it skip proper duration set
            If ((SkipPositionDuration) = Convert.ToInt32(SkipDurationSetting)) Then
                _UI.Utils.LogCommentBlack("Skip successful")
                'Checking if we get less than skip set.Possible only if we are reaching BOF
            ElseIf SkipPositionDuration < Convert.ToInt32(SkipDurationSetting) AndAlso VerifyEofBof Then
                Try
                    If Convert.ToBoolean(LastSkipEnabled) Then
                        Milestones_EOFBOF = _Utils.GetValueFromMilestones("SkipEOFBOF")
                        _Utils.EndWaitForDebugMessages(Milestones_EOFBOF, ActualLines)
                        _UI.Utils.LogCommentBlack("Reached BOF")
                        EofBofReached = True
                    Else
                        _UI.Utils.LogCommentBlack("Last skip point reached")
                        EofBofReached = True
                    End If
                Catch ex As Exception
                    _UI.Utils.LogCommentFail("Did not reach BOF")
                    status = False
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS BOF Milestones : " + Milestones))
                    Return status
                Finally
                    _iex.ForceHideFailure()
                End Try
                'Checking if we get more than skip set.Possible only if we are reaching EOF
            ElseIf SkipPositionDuration > Convert.ToInt32(SkipDurationSetting) AndAlso VerifyEofBof Then
                Try
                    If Convert.ToBoolean(LastSkipEnabled) Then
                        Milestones_EOFBOF = _Utils.GetValueFromMilestones("SkipEOFBOF")
                        _Utils.EndWaitForDebugMessages(Milestones_EOFBOF, ActualLines)
                        _UI.Utils.LogCommentBlack("Reached EOF")
                        EofBofReached = True
                    Else
                        _UI.Utils.LogCommentBlack("Last skip point reached")
                        EofBofReached = True
                    End If
                Catch ex As Exception
                    _UI.Utils.LogCommentFail("Did not reach EOF/last skip point")
                    status = False
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS EOF Milestones : " + Milestones))
                    Return status
                Finally
                    _iex.ForceHideFailure()
                End Try
            Else
                'If no condition passes
                _UI.Utils.LogCommentFail("Skip Unsuccessful")
                status = False
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS EOFBOF Milestones : " + Milestones))
                Return status
            End If
        Catch ex As Exception
            status = False
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify Skip "))
        Finally
            _iex.ForceHideFailure()
        End Try
        Return status
    End Function

    Private Function SkipLogVerification(ByVal MarginLines As String, ByVal Skip_log As String, ByRef CurrentStreamPos As String, ByRef NextstreamPos As String)

        Dim LogArray As String()
        Dim GetStreamLog As String = "getStreamPosition"
        Dim SetStreamLog As String = "setStreamPosition"
        Dim found As Boolean = False
        Dim status As Boolean = False


        'SAMPLE LOG WHICH IS USED FOR VERIFICATION
        'ORIGIN_END or ORIGIN_START depends on direction,then we fetch getstream and setstream positions
        '11:20:06.121 09Oct 0000076531.896664 IEX IR key@#$39
        '11:20:06.123 09Oct 0000076531.899342 IEX_PlaybackReq_SetplayerSpeed:1000

        '11:20:06.141 09Oct 0000076531.916844 IEX_PlaybackReq_SetplayerSpeed:1000

        '11:20:06.141 09Oct 0000076531.917077 IEX_AFL_FAS_getStreamPosition:ORIGIN_END

        '11:20:06.164 09Oct 0000076531.939675 IEX_AFL_FAS_getStreamPosition:-4790723

        '11:20:06.183 09Oct 0000076531.959135 IEX_AFL_FAS_setStreamPosition: origin=26 position=-4780723 

        'Splitting the Marginlines string with ENTER as delimiter

        LogArray = Split(MarginLines, vbLf)
        For i As Integer = 0 To LogArray.Length
            If LogArray(i).Contains("IR key") Then
                For j = i + 1 To LogArray.Length
                    If LogArray(j).Contains(Skip_log) Then
                        For k = j + 1 To LogArray.Length

                            If Not found Then
                                If LogArray(k).Contains(GetStreamLog) Then
                                    CurrentStreamPos = LogArray(k).Split(":").Last()
                                    found = True
                                End If
                            Else
                                If LogArray(k).Contains(SetStreamLog) Then
                                    NextstreamPos = LogArray(k).Split("=").Last()
                                    status = True
                                    Exit For
                                End If
                            End If
                        Next
                        If found Then
                            Exit For
                        End If
                    End If
                Next
                If found Then
                    Exit For
                End If
            End If
        Next
        Return status
    End Function
End Class
