Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.TrickModes

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI
    Dim _Utils As IEX.ElementaryActions.EPG.SF.GET.Utils

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

        _UI.Utils.StartHideFailures("Raising TrickMode")
        _UI.Utils.LogCommentInfo("Waiting for Timeout..!! : ")
        _iex.Wait(10)
        Try
            If Not _UI.Utils.VerifyState("TRICKMODE BAR") Then
                Try
                    VerifySpeedChanged("PAUSE", 0, 10)
                Catch ex As Exception
                    _UI.Utils.LogCommentInfo("Sending PAUSE again as previous PAUSE failed to set speed 0")
                    VerifySpeedChanged("PAUSE", 0, 10)
                End Try
            Else
                _UI.Utils.LogCommentInfo("Already on Trickmode Bar..!! : ")
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

        _UI.Utils.StartHideFailures("Setting Speed To " + Speed.ToString)

        Try
            If RequiredSpeed = 0 Or RequiredSpeed = 1 Then
                _UI.Utils.StartHideFailures("Trying To Set Speed To " + Speed.ToString)

                Try
                    Try

                        VerifySpeedChanged("PAUSE", Speed, 20) 'Timeout Changed From 6 to 10
                        Exit Sub
                    Catch ex As Exception
                        _UI.Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        VerifySpeedChanged("PAUSE", Speed, 20) 'Timeout Changed From 6 to 10
                        Exit Sub
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try
            ElseIf RequiredSpeed = 0.5 Then

                '_UI.Utils.StartHideFailures("Trying To Pause")

                'Try

                '    Try
                '        VerifySpeedChanged("PAUSE", 0, 20) 'Timeout Changed From 6 to 10
                '    Catch ex As Exception
                '        _UI.Utils.LogCommentFail("Failed To Verify Speed : 0 Trying Again")
                '        VerifySpeedChanged("PAUSE", 0, 20) 'Timeout Changed From 6 to 10
                '    End Try
                'Finally
                '    _iex.ForceHideFailure()
                'End Try

                _UI.Utils.LogCommentInfo("Waiting for Timeout..!! : ")
                _iex.Wait(10)
                Try
                    _UI.Utils.SendIR("PAUSE")
                    _UI.Utils.LogCommentInfo("Waiting for Timeout..!! : ")
                    _iex.Wait(10)
                    If Not _UI.Utils.VerifyState("TRICKMODE BAR", 20) Then
                        _UI.Utils.SendIR("PAUSE")
                    Else
                        _UI.Utils.LogCommentInfo("Trickmode launched..!! : ")
                    End If
                    ' VerifySpeedChanged("PAUSE", 0, 20) 'Timeout Changed From 6 to 10

                Finally
                    _iex.ForceHideFailure()
                End Try



                _UI.Utils.LogCommentInfo("Navigating To FF 0.5")

                VerifySpeedChanged("FF", Speed, 20)

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

            'MessagesCol = Milestones.Split(",")
            'Milestones = MessagesCol(0)

            _Utils.BeginWaitForDebugMessages(Milestones, Timeout + 5)

            _Utils.SendIR(IR, 0)

            Try
                Succeed = _Utils.EndWaitForDebugMessages(Milestones, ActualLines)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            End Try

            If Not Succeed Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            End If

            'If MessagesCol.Length > 1 Then

            '    Milestones = MessagesCol(1)

            '    _Utils.BeginWaitForDebugMessages(Milestones, Timeout + 2)

            '    Try
            '        Succeed = _Utils.EndWaitForDebugMessages(Milestones, ActualLines)
            '    Catch ex As Exception
            '        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            '    End Try

            '    If Not Succeed Then
            '        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Milestones : " + Milestones))
            '    End If
            'End If

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
        Dim FoundBOF As Boolean = False

        _Utils.StartHideFailures("Verifying " + IIf(Eof, "EOF ", "BOF ") + "Duration = " + Duration.ToString + " Speed = " + Speed.ToString + " IsReviewBuffer = " _
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

            _Utils.LogCommentImportant("Total Duration = " + FullDuration.ToString + " Not Suppose To Find Duration = " + Duration.ToString + " Suppose To Find Duration = " + LeftDuration.ToString)

            _Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(Eof = True, "EOF", "BOF") + " After " + FullDuration.ToString + " Seconds")

            'MILESTONES MESSAGES
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList
            Dim Msg As String = ""

            If Eof Then
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

            _Utils.StartHideFailures("Verifying " + IIf(Eof = True, "EOF", "BOF") + " Not Found Before Correct Time")


            Dim StartTime As DateTime
            Dim EndTime As DateTime
            Dim time_waited As Integer

            StartTime = DateTime.Now

            '_Utils.BeginWaitForDebugMessages(Milestones, Duration)
            _Utils.BeginWaitForDebugMessages(Milestones, FullDuration)

            If _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                FoundBOF = True
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(Eof = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
            End If

            EndTime = DateTime.Now
            time_waited = DateDiff(DateInterval.Second, StartTime, EndTime)
            _UI.Utils.LogCommentInfo("VerifyEofBof:: Duration , FullDuration , time_waited::" + Duration.ToString + "::" + FullDuration.ToString + "::" + time_waited.ToString)
            If FoundBOF = True Then
                If time_waited > Duration And time_waited < FullDuration Then
                    _UI.Utils.LogCommentInfo("Verified the log at correct time")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(Eof = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
                End If

            End If



            'If _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
            '    FoundBOF = True
            '    _Utils.LogCommentWarning("Found " + IIf(EOF = True, "EOF", "BOF") + " Before Correct Time")
            'End If

            _iex.ForceHideFailure()

            'If FoundBOF = False Then
            '    _Utils.LogCommentInfo("TrickModes.VerifyEofBof : Verifying " + IIf(EOF = True, "EOF", "BOF") + " Found In " + LeftDuration.ToString + " Seconds Left Of Event")

            '    _Utils.BeginWaitForDebugMessages(Milestones, LeftDuration)

            '    If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
            '        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(EOF = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
            '    End If
            'End If

            If Eof And IsReviewBuffer Then
                _Utils.LogCommentInfo("Waiting For STB After Catch Up To LIVE")

                If Not _Utils.VerifyState("LIVE", 25, 15) Then
                    _iex.ForceHideFailure()
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
