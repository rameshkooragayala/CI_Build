Imports FailuresHandler

Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.Banner

    Dim _UI As UI
    Private Shadows Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''     Stopping Recording Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelBooking(Optional ByVal IsSeries As Boolean = False, Optional ByVal IsComplete As Boolean = False)
        'EPG TEXT
        Dim EpgText As String = ""
        Dim Milestones As String = ""

        _UI.Utils.StartHideFailures("Cancelling Booked Event From Action Bar")

        _UI.Utils.EPG_Milestones_Navigate("CANCEL RECORDING")

        Try
            If IsSeries Then
            _UI.Utils.StartHideFailures("Trying To Navigate To CANCEL THIS EPISODE")
                If IsComplete Then
            Try
                        _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL ALL EPISODES")
                    Catch ex2 As Exception
                        _iex.ForceHideFailure()
                    End Try
                Else
                Try
                    _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL THIS EPISODE")
                Catch ex2 As Exception
                    _iex.ForceHideFailure()
                    End Try
                End If
            Else
                    _UI.Utils.StartHideFailures("Trying To Navigate To CANCEL RECORDING")
                    _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL RECORDING")
            End If

            Dim ActualLines As New ArrayList
            If IsComplete Then
                Milestones = _UI.Utils.GetValueFromMilestones("CancelAllBooking")
            Else
                Milestones = _UI.Utils.GetValueFromMilestones("CancelBooking")
            End If

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 15)

            'Not Checking If Action Bar Disappeared Cause When Canceling Event From Guide Need To Check Change And Not Disappear
            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Stopping Recording Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>348 - StopRecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub StopRecording()
        'EPG TEXT
        Dim EpgText As String = ""
        Dim Milestones As String = ""

        _UI.Utils.StartHideFailures("Stopping Recording Event From Action Bar")

        Try
            _UI.Utils.LogCommentInfo("Banner.StopRecording : Stopping Recording Event From Action Bar")

            _UI.Utils.EPG_Milestones_Navigate("STOP RECORDING")

            _UI.Utils.LogCommentInfo("Verifying State 'CONFIRM DELETE'")

            If Not _UI.Utils.VerifyState("CONFIRM DELETE", 3) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify CONFIRM DELETE"))
            End If

            _UI.Utils.EPG_Milestones_SelectMenuItem("YES")

            'MILESTONES MESSAGES

            Milestones = _UI.Utils.GetValueFromMilestones("StopRecording")

            Dim ActualLines As New ArrayList

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 10)


            _UI.Utils.ClearEPGInfo()

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify StopRecording Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
