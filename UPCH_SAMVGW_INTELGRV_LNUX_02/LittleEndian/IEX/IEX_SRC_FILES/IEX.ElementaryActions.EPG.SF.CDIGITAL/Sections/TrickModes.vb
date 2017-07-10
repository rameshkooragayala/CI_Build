Imports FailuresHandler
Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.TrickModes

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI
    Dim current_direction As String

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

 ''' <summary>
    '''   Raising Trickmode By Pressing PAUSE From the LIVE screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub RaiseTrickMode()
        Dim i As Integer = 0
        Dim j As Integer = 0
        _UI.Utils.StartHideFailures("Raising TrickMode")

        'we need to raise trickmode bar every time.Suppose 2x speed was there and EOF/BOF reached.It takes time for transition between states.Wait ensures this
        _iex.Wait(3)
        ''Sometimes EPG moves to FSV. but still the Get ACTIVE_STATE returns TRICKMODE BAR
        ''The setpseed call fails at this state for CanalD because the trick mode bar is not raised properly
        ''To avoid this the below workaround is added which confirms the trick mode bar state 3 times
        ''This can be removed at later stage
        Try
            For i = 0 To 1
                If Not _UI.Utils.VerifyState("TRICKMODE BAR", 10) Then
                    For j = 0 To 3
                        _UI.Utils.LogCommentInfo("Raising Trickmode bar trial::" + j.ToString)
                        Try
                            _UI.Utils.SendIR("SELECT_LEFT")
                            _UI.Utils.SendIR("SELECT_RIGHT")
                            _UI.Utils.SendIR("SELECT")
                            _UI.Utils.SendIR("SELECT")
                        Catch ex As IEXException
                            _iex.Wait(1)
                            _UI.Utils.LogCommentInfo("Could not navigate to TRICKMODE BAR Retry::" + j.ToString)
                            Continue For
                        Finally
                            _UI.Utils.LogCommentInfo("Navigation to TRICKMODE BAR Successful::")
                        End Try
                        Exit For
                    Next
                    current_direction = "DEFAULT"
                    Exit For
                Else
                    _iex.Wait(1)
                End If
                _UI.Utils.LogCommentInfo("Raising Trickmode bar trial::" + i.ToString)
            Next
            _UI.Utils.LogCommentInfo("Exiting for loop after ::" + i.ToString + "trials")
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
        Dim canald_default_key As String
        _UI.Utils.StartHideFailures("Setting Speed To " + Speed.ToString)
        canald_default_key = "SELECT"
        Try
            If RequiredSpeed = 0 Or RequiredSpeed = 1 Then
                _UI.Utils.StartHideFailures("Trying To Set Speed To " + Speed.ToString)
                Try
                    Try
                        VerifySpeedChanged("SELECT", Speed, 30) 'Timeout Changed From 6 to 10
                        current_direction = "DEFAULT"
                        Exit Sub
                    Catch ex As Exception
                        _UI.Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        current_direction = "DEFAULT"
                        If RequiredSpeed = 0 Then
                            '' There is a possibility that there is not trick mode bar at this state because setting play speed disposes the trick mode bar
                            '' Raising the trickmode bar will set the speed to PAUSE anyway
                            RaiseTrickMode()
                        Else
                            VerifySpeedChanged("SELECT", Speed, 30) 'Timeout Changed From 6 to 10
                        End If
                        Exit Sub
                    End Try
                    current_direction = "DEFAULT"
                Finally
                    _iex.ForceHideFailure()
                End Try
            ElseIf RequiredSpeed = 0.5 Then
                _UI.Utils.StartHideFailures("Trying To Pause")
                _UI.Utils.LogCommentInfo("Navigating To FF 0.5")
                _UI.Utils.SendIR("SELECT_RIGHT", 2000)
                VerifySpeedChanged(canald_default_key, Speed, 3)
                current_direction = "FF"
                Exit Sub
            Else
                Dim Direction As String = ""
                Dim Key As String = ""
                _UI.Utils.StartHideFailures("Trying To Verify Speed " + Speed.ToString)
                Try
                    If Speed < 0 Then
                        Direction = "REWIND"
                        Key = "SELECT_LEFT"
                    Else
                        Direction = "FF"
                        Key = "SELECT_RIGHT"
                    End If

                    ' Direction should be changed only if current direction is not the direction to be set
                    If current_direction <> Direction Then
                        _UI.Utils.SendIR(Key, 2000)
                    Else
                        current_direction = "DEFAULT"
                    End If

                    For i As Integer = 1 To 8
                        Try
                            VerifySpeedChanged(canald_default_key, Speed, 2)
                            _UI.Utils.LogCommentInfo("Verifyied Speed " + Speed.ToString)
                            current_direction = Direction
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
    '''   Verifying EOF/BOF
    ''' </summary>
    ''' <param name="Duration">Duration Of The Event Or Review Buffer</param>
    ''' <param name="Speed">Speed Of Trickmode</param>
    ''' <param name="IsReviewBuffer">If True Checkes Review Buffer EOF/BOF</param>
    ''' <param name="IsReviewBufferFull">If True Review Buffer Is Full</param>
    ''' <param name="Eof">If True Verifies EOF Else BOF</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyEofBof(ByVal Duration As Long, ByVal Speed As Double, ByVal IsReviewBuffer As Boolean, ByVal IsReviewBufferFull As Boolean, ByVal Eof As Boolean)
        Dim LeftDuration As Long
        Dim FullDuration As Long
        Dim FoundBOF As Boolean = False
        _UI.Utils.StartHideFailures("Verifying " + IIf(Eof, "EOF ", "BOF ") + "Duration = " + Duration.ToString + " Speed = " + Speed.ToString + " IsReviewBuffer = " _
                                      + IsReviewBuffer.ToString + " IsReviewBufferFull = " + IsReviewBufferFull.ToString)
        Try
            If Duration < 0 Then Duration = 0
            If IsReviewBuffer Then
                If Speed > 1 Then
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
            _UI.Utils.LogCommentImportant("Total Duration = " + FullDuration.ToString + " Not Suppose To Find Duration = " + Duration.ToString + " Suppose To Find Duration = " + LeftDuration.ToString)
            _UI.Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(Eof = True, "EOF", "BOF") + " After " + FullDuration.ToString + " Seconds")
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList
            Dim Msg As String = ""
            If Eof Then
                If IsReviewBuffer Then
                    Msg = "TrickModeEOFInReviewBuffer"
                    Milestones = _UI.Utils.GetValueFromMilestones("TrickModeEOFInReviewBuffer")
                Else
                    Msg = "TrickModeEOFNotInReviewBuffer"
                    Milestones = _UI.Utils.GetValueFromMilestones("TrickModeEOFNotInReviewBuffer")
                End If
            Else 'BOF
                If IsReviewBuffer Then
                    Msg = "TrickModeBOFInReviewBuffer"
                    Milestones = _UI.Utils.GetValueFromMilestones("TrickModeBOFInReviewBuffer")
                Else
                    Msg = "TrickModeBOFNotInReviewBuffer"
                    Milestones = _UI.Utils.GetValueFromMilestones("TrickModeBOFNotInReviewBuffer")
                End If
            End If
            _UI.Utils.StartHideFailures("Verifying " + IIf(Eof = True, "EOF", "BOF") + " Not Found Before Correct Time")
            Dim StartTime As DateTime
            Dim EndTime As DateTime
            Dim time_waited As Integer
            StartTime = DateTime.Now
            _UI.Utils.BeginWaitForDebugMessages(Milestones, FullDuration)
            If _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                FoundBOF = True
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(Eof = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
            End If
            EndTime = DateTime.Now
            _iex.ForceHideFailure()
            time_waited = DateDiff(DateInterval.Second, StartTime, EndTime)
            _UI.Utils.LogCommentInfo("VerifyEofBof:: Duration , FullDuration , time_waited::" + Duration.ToString + "::" + FullDuration.ToString + "::" + time_waited.ToString)
            If FoundBOF = True Then
                If time_waited > Duration And time_waited < FullDuration Then
                    _UI.Utils.LogCommentInfo("Verified the log at correct time")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(Eof = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
                End If
            End If
            If Eof And IsReviewBuffer Then
                _UI.Utils.LogCommentInfo("Waiting For STB After Catch Up To LIVE")
                If Not _UI.Utils.VerifyState("LIVE", 25, 15) Then
                    _iex.ForceHideFailure()
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is LIVE"))
                Else
                End If
            ElseIf Eof Then
                _iex.Wait(2)
            End If
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
        If (current_direction = "REWIND") Then
            _UI.Utils.SendIR("SELECT_RIGHT", 0)
            current_direction = "DEFAULT"
        ElseIf (current_direction = "FF") Then
            _UI.Utils.SendIR("SELECT_LEFT", 0)
            current_direction = "DEFAULT"
        End If
        Try
            _UI.Utils.StartHideFailures("Verifying FAS Speed Change Milestones Messages...")
            If Speed.ToString.Contains("-") Then
                Rew = "-"
            End If
            Speed = Math.Abs(Speed)
            Dim Milestones As String = ""
            Milestones = _UI.Utils.GetValueFromMilestones("TrickModeSpeed")
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
            _UI.Utils.BeginWaitForDebugMessages(Milestones, Timeout + 5)
            _UI.Utils.SendIR(IR, 2000)
            Try
                Succeed = _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            End Try
            If Not Succeed Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
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
        Dim Msg As String = ""
        Dim k As Integer = 0
        _UI.Utils.StartHideFailures("Stopping Play Event")
        Try
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList
            If IsReviewBuffer Then
                Msg = "TrickModeStopInReviewBuffer"
                Milestones = _UI.Utils.GetValueFromMilestones("TrickModeStopInReviewBuffer")
			' On Action Bar Trick Mode Navigation flow is diff,need to handle this case also
		   ElseIf _UI.Utils.VerifyState("ACTION BAR", 10) Then
                _UI.Utils.SendIR("SELECT")
                _UI.Utils.SendIR("RETOUR")
            Else
                Msg = "TrickModeStopNotInReviewBuffer"
                Milestones = _UI.Utils.GetValueFromMilestones("TrickModeStopNotInReviewBuffer")
            End If
			
			' Need to be in Pause State before doing navigation to STOP
			SetSpeed(0)
			
            _UI.Utils.BeginWaitForDebugMessages(Milestones, 60)
			
			_UI.Utils.LogCommentInfo("Navigating to STOP Option on Trickmode Bar")
            For k = 0 To 2
                _UI.Utils.SendIR("SELECT_RIGHT")
            Next
            
			'After this select we are on live thus exiting trickmode bar
            _UI.Utils.SendIR("SELECT")
            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If
            If IsReviewBuffer Then
                If Not _UI.Utils.VerifyState("LIVE", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify State Is LIVE"))
                End If

            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class
