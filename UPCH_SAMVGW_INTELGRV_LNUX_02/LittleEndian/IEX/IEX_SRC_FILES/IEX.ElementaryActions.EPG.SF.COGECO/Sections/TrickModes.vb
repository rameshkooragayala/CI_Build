Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.TrickModes

    Dim res As IEXGateway._IEXResult
    Dim _UI As IEX.ElementaryActions.EPG.SF.COGECO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
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

        _UI.Utils.StartHideFailures("Setting Speed To " + Speed.ToString)

        Try
            If RequiredSpeed = 0 Or RequiredSpeed = 1 Then
                _UI.Utils.StartHideFailures("Trying To Set Speed To " + Speed.ToString)

                Try
                    Try
                        VerifySpeedChanged("PAUSE", Speed, 10) 'Timeout Changed From 6 to 10
                        Exit Sub
                    Catch ex As Exception
                        _UI.Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        VerifySpeedChanged("PAUSE", Speed, 10) 'Timeout Changed From 6 to 10
                        Exit Sub
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try
            ElseIf RequiredSpeed = 0.5 Then

                _UI.Utils.StartHideFailures("Trying To Pause")

                Try

                    Try
                        VerifySpeedChanged("PAUSE", 0, 10) 'Timeout Changed From 6 to 10
                    Catch ex As Exception
                        _UI.Utils.LogCommentFail("Failed To Verify Speed : 0 Trying Again")
                        VerifySpeedChanged("PAUSE", 0, 10) 'Timeout Changed From 6 to 10
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try

                _UI.Utils.LogCommentInfo("Navigating To FF 0.5")

                VerifySpeedChanged("FF", Speed, 10)

                Exit Sub
            Else
                Dim Direction As String = ""

                _UI.Utils.StartHideFailures("Trying To Verify Speed " + Speed.ToString)

                Try

                    If Speed < 0 Then
                        Direction = "REWIND"
                    Else
                        Direction = "FF"
                    End If

                    For i As Integer = 1 To 8
                        Try
                            VerifySpeedChanged(Direction, Speed, 10) 'Timeout Changed From 2 to 10
                            _UI.Utils.LogCommentInfo("Verifyied Speed " + Speed.ToString)
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

        _UI.Utils.StartHideFailures("Stopping Play Event")

        Try
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            If IsReviewBuffer Then
                Msg = "TrickModeStopInReviewBuffer"
                Milestones = _UI.Utils.GetValueFromMilestones("TrickModeStopInReviewBuffer")
            Else
                Msg = "TrickModeStopNotInReviewBuffer"
                Milestones = _UI.Utils.GetValueFromMilestones("TrickModeStopNotInReviewBuffer")
            End If

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 60)

            _UI.Utils.SendIR("STOP")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If

            If IsReviewBuffer Then
                If Not _UI.Utils.VerifyState("LIVE", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify State Is LIVE"))
                End If
            Else
                _UI.ArchiveRecordings.VerifyArchive()
            End If

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
    Public Overrides Sub VerifyEofBof(ByVal Duration As Long, ByVal Speed As Double, ByVal IsReviewBuffer As Boolean, ByVal IsReviewBufferFull As Boolean, ByVal Eof As Boolean)
        Dim LeftDuration As Long
        Dim FullDuration As Long
        Dim FoundBof As Boolean = False
        Dim Msg As String = ""

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

            _UI.Utils.LogCommentImportant("LeftDuration = " + LeftDuration.ToString + " FullDuration = " + FullDuration.ToString + " Duration = " + Duration.ToString)

            _UI.Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(Eof = True, "EOF", "BOF") + " After " + FullDuration.ToString + " Seconds")

            'MILESTONES MESSAGES
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

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

            _UI.Utils.BeginWaitForDebugMessages(Milestones, Duration)

            If _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                FoundBof = True
                _UI.Utils.LogCommentWarning("WARNING : Found " + IIf(Eof = True, "EOF", "BOF") + " Before Correct Time")
                '_Utils.LogCommentFail("TrickModes.VerifyEofBof : Found " + IIf(EOF = True, "EOF", "BOF") + " Before Correct Time")
                '_iex.ForceHideFailure()
                '_iex.ForceHideFailure()
                '_Utils.LogCommentFail("Found " + IIf(EOF = True, "EOF", "BOF") + " Before Correct Time")
                'Return False
            End If

            _iex.ForceHideFailure()

            If FoundBof = False Then
                _UI.Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(Eof = True, "EOF", "BOF") + " Found In " + LeftDuration.ToString + " Seconds Left Of Event")

                _UI.Utils.BeginWaitForDebugMessages(Milestones, LeftDuration)

                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(Eof = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
                End If
            End If

            If Eof And IsReviewBuffer Then
                _UI.Utils.LogCommentInfo("Waiting For STB After Catch Up To LIVE")

                If Not _UI.Utils.VerifyState("LIVE", 40) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is LIVE"))
                End If

            ElseIf Eof Then
                _iex.Wait(2)
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
