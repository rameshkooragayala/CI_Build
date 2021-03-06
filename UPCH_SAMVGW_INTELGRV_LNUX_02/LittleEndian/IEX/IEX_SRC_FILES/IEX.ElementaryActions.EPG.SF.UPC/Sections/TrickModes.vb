﻿Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.TrickModes

    Dim _UI As IEX.ElementaryActions.EPG.SF.UPC.UI
    Dim _Utils As IEX.ElementaryActions.EPG.SF.UPC.Utils

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

        If Not (_UI.Utils.VerifyState("TRICKMODE BAR", 5)) Then
            _UI.Utils.StartHideFailures("Raising TrickMode")
            Try

                _Utils.EPG_Milestones_Navigate("TRICKMODE BAR")

                _iex.Wait(2)
                _iex.SendIRCommand("PAUSE")

            Finally
                _iex.ForceHideFailure()
            End Try
            Exit Sub
        End If

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
                If Not (_UI.Utils.VerifyState("TRICKMODE BAR", 5)) Then
                    _UI.Utils.StartHideFailures("Raising TrickMode")

                    Try
                        RaiseTrickMode()

                    Finally
                        _iex.ForceHideFailure()
                    End Try

                End If

                _UI.Utils.StartHideFailures("Trying To Set Speed To " + Speed.ToString)

                Try

                    For i As Integer = 1 To 3
                        Try
                            VerifySpeedChanged("PAUSE", Speed, 10)
                            Exit Sub
                        Catch ex As Exception
                            _UI.Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        End Try
                    Next


                Finally
                    _iex.ForceHideFailure()
                End Try
            ElseIf RequiredSpeed = 0.5 Then

                _Utils.StartHideFailures("Trying To Pause")

                Try

                    Try
                        SetSpeed(0)
                    Catch ex As Exception
                        _Utils.LogCommentFail("Failed To Verify Speed : 0")
                        Exit Sub
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try

                _Utils.LogCommentInfo("Navigating To FF 0.5")

                VerifySpeedChanged("FF", Speed, 10)

                Exit Sub
            Else
                Dim Direction As String = ""

                _Utils.StartHideFailures("Trying To Verify Speed " + Speed.ToString)

                Try

                    If Speed < 0 Then
                        Direction = "REWIND"
                    Else
                        Direction = "FF"
                    End If

                    For i As Integer = 1 To 8
                        Try
                            VerifySpeedChanged(Direction, Speed, 5)
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

            If Speed = 0 Or Speed = 1 Then
                Milestones = _Utils.GetValueFromMilestones("TrickModeSpeed")
            Else
                Milestones = _Utils.GetValueFromMilestones("TrickModeSpeed")
            End If

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

           ''' If Speed = 0 Or Speed = 1 Then
                
           '''     _Utils.BeginWaitForDebugMessages(Milestones, Timeout)
		   '''	_Utils.SendIR(IR, 0)
           ''' Else
                _Utils.BeginWaitForDebugMessages(Milestones, Timeout)

                _Utils.SendIR(IR, 0)

           ''' End If

            Try
                Succeed = _Utils.EndWaitForDebugMessages(Milestones, ActualLines)
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
    ''' 
    Public Overrides Sub VerifyEofBof(ByVal Duration As Long, ByVal Speed As Double, ByVal IsReviewBuffer As Boolean, ByVal IsReviewBufferFull As Boolean, ByVal Eof As Boolean)
        Dim LeftDuration As Long
        Dim FullDuration As Long
        Dim FoundBof As Boolean = False

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

            'MILESTONES MESSAGES
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
            _iex.Wait(3)
            '_Utils.BeginWaitForDebugMessages(Milestones, Duration)
            _UI.Utils.BeginWaitForDebugMessages(Milestones, FullDuration)

            If _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                FoundBof = True
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEofBofFailure, "Failed To Verify " + IIf(Eof = True, "EOF ", "BOF ") + Msg + " Milestones : " + Milestones + " At Correct Time"))
            End If

            EndTime = DateTime.Now
            time_waited = DateDiff(DateInterval.Second, StartTime, EndTime)
            _UI.Utils.LogCommentInfo("VerifyEofBof:: Duration , FullDuration , time_waited::" + Duration.ToString + "::" + FullDuration.ToString + "::" + time_waited.ToString)
            If FoundBof = True Then
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
                _UI.Utils.LogCommentInfo("Waiting For STB After Catch Up To LIVE")

                If Not _UI.Utils.VerifyState("LIVE", 25, 15) Then
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



