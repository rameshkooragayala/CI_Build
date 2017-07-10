Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.UPC.TrickModes

    Dim res As IEXGateway._IEXResult
    Dim _UI As IEX.ElementaryActions.EPG.SF.ISTB.UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
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
        Dim PauseMessages As String = ""
        Dim Rew As String = ""

        If Speed.ToString.Contains("-") Then
            Rew = "-"
        End If

        _Utils.StartHideFailures("Setting Speed To " + Rew + Speed.ToString)

        Try
            Speed = Math.Abs(Speed)

            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("TrickModeSpeed")

            Select Case Speed
                Case 0
                    Milestones += "0"
                Case 1
                    Milestones += "1000"
                Case 0.5
                    PauseMessages = Milestones + "0"
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

            If Speed = 0 Or Speed = 1 Then
                _Utils.StartHideFailures("Trying To Set Speed To " + Rew + Speed.ToString)

                Try
                    Try
                        _Utils.VerifyFas("PAUSE", Milestones, 9, True)
                    Catch ex As Exception
                        _Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        _Utils.VerifyFas("PAUSE", Milestones, 9, True)
                    End Try

                    Exit Sub
                Finally
                    _iex.ForceHideFailure()
                End Try

            ElseIf Speed = 0.5 Then
                _Utils.StartHideFailures("Trying To Pause")

                Try
                    Try
                        _Utils.VerifyFas("SELECT", PauseMessages, 6, True)
                    Catch ex As Exception
                        _Utils.LogCommentFail("Failed To Verify Speed : " + Speed.ToString + " Trying Again")
                        _Utils.VerifyFas("SELECT", PauseMessages, 6, True)
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try

                _Utils.LogCommentInfo("Navigating To FF 0.5")
                _Utils.VerifyFas("FF", Milestones, 10, True)

                Exit Sub
            Else
                Dim Direction As String = ""

                _Utils.StartHideFailures("Trying To Verify Speed " + Speed.ToString)

                Try
                    If Rew = "-" Then
                        Direction = "REWIND"
                    Else
                        Direction = "FF"
                    End If

                    For i As Integer = 1 To 8
                        Try
                            _Utils.VerifyFas(Direction, Milestones, 2, True)
                            _Utils.LogCommentInfo("Verifyied Speed " + Rew + Speed.ToString)
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
End Class
