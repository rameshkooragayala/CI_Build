Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.TrickModes

    Dim _UI As IEX.ElementaryActions.EPG.SF.VGW.UI
    Dim _Utils As IEX.ElementaryActions.EPG.SF.VGW.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
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

            _iex.ForceHideFailure()

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
