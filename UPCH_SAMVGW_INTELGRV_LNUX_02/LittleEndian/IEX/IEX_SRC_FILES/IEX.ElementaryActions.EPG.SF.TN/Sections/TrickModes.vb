Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.TrickModes

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI
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
        If Not (_UI.Utils.VerifyState("TRICKMODE BAR", 2)) Then
            _UI.Utils.StartHideFailures("Raising TrickMode")

            Try
                _UI.Utils.EPG_Milestones_NavigateByName("STATE:TRICKMODE BAR")
                _UI.Utils.SendIR("SELECT")
            Finally
                _iex.ForceHideFailure()
            End Try
        End If
    End Sub

    ''' <summary>
    '''  Sets Speed On Trickmode
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
        Dim TN_default_key As String
        Dim isInPauseState = False

        _UI.Utils.StartHideFailures("Setting Speed To " + Speed.ToString)

        TN_default_key = "SELECT"
        Try
            'For Pause and Play, speed = 0 and 1 respectively
            If RequiredSpeed = 0 Or RequiredSpeed = 1 Then

                If Not (_UI.Utils.VerifyState("TRICKMODE BAR", 2)) Then
                    _UI.Utils.StartHideFailures("Raising TrickMode")

                    Try
                        RaiseTrickMode()
                        isInPauseState = True
                    Finally
                        _iex.ForceHideFailure()
                    End Try
                    Exit Sub
                End If

                _UI.Utils.StartHideFailures("Trying To Set Speed To " + Speed.ToString)

                Try
                    If Not (isInPauseState And RequiredSpeed = 0) Then
                        For i As Integer = 1 To 5
                            Try
                                VerifySpeedChanged(TN_default_key, Speed, 30) 'Timeout Changed From 6 to 10
                                current_direction = "DEFAULT"
                                Exit Sub
                            Catch ex As Exception
                                _UI.Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                                current_direction = "DEFAULT"
                                Exit Sub
                            End Try
                        Next
                    End If
                Finally
                    _iex.ForceHideFailure()
                End Try

            Else
                Dim Direction As String = ""

                _UI.Utils.StartHideFailures("Trying To Verify Speed " + Speed.ToString)

                Try

                    If Speed < 0 Then
                        Direction = "REWIND"
                        _UI.Utils.SendIR("SELECT_LEFT", 0)
                    Else
                        Direction = "FF"
                        _UI.Utils.SendIR("SELECT_RIGHT", 0)
                    End If

                    For i As Integer = 1 To 8
                        Try
                            VerifySpeedChanged(TN_default_key, Speed, 2)
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
    '''   Sending Verifying Speed Change FAS Milestones
    ''' </summary>
    ''' <param name="IR">IR To Send</param>
    ''' <param name="Speed">Speed Of Requirement Trickmode</param>
    ''' <param name="Timeout"></param>
    ''' <remarks></remarks>
    Public Overrides Sub VerifySpeedChanged(ByVal IR As String, ByVal Speed As Double, ByVal Timeout As Integer)
        Dim ActualLines As New ArrayList
        Dim MessagesCol As String() = Nothing
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
            Speed = Math.Abs(Speed)
            Dim Milestones As String = ""
            Milestones = _UI.Utils.GetValueFromMilestones("TrickModeSpeed")

            Select Case Speed
                Case 0
                    Milestones += "pause"
                Case 1
                    Milestones += "1000"
                Case 2
                    Milestones += "2000"
                Case 4
                    Milestones += "4000"
                Case 8
                    Milestones += "8000"
                Case 16
                    Milestones += "16000"
                Case 32
                    Milestones += "32000"
            End Select
            _UI.Utils.BeginWaitForDebugMessages(Milestones, Timeout + 5)

            _UI.Utils.SendIR(IR, 0)

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

        _UI.Utils.StartHideFailures("Stopping Play Event")

        Try
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList
            Dim Msg As String = ""

            If IsReviewBuffer Then
                Msg = "TrickModeStopInReviewBuffer"
                Milestones = _UI.Utils.GetValueFromMilestones("TrickModeStopInReviewBuffer")
            Else
                Msg = "TrickModeStopNotInReviewBuffer"
                Milestones = _UI.Utils.GetValueFromMilestones("TrickModeStopNotInReviewBuffer")
            End If

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 60)

            _iex.MilestonesEPG.Navigate("STOP")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If

            If IsReviewBuffer Then
                If Not _UI.Utils.VerifyState("LIVE", 25) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify State Is LIVE"))
                End If
            Else
                _UI.ArchiveRecordings.VerifyArchive()
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
