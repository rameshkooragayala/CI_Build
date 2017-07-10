Imports FailuresHandler
Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.ISTB.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''    Setting Event Reminder From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>311 - SetEventReminderFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetReminder()
        Dim Milestones As String = ""
        Dim State As String = ""

        _UI.Utils.StartHideFailures("Setting Event Reminder From Action Bar")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("ADD REMINDER")

            Dim ActualLines As New ArrayList

            Milestones = _UI.Utils.GetValueFromMilestones("SetReminder")

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 120)

            _UI.Utils.SendIR("SELECT")

            Dim EventRecordingState As Boolean = True
            Dim ConfirmationState As Boolean = True

            If Not _UI.Utils.VerifyState("CONFIRMATION", 5) Then
                ConfirmationState = False
            End If

            If Not _UI.Utils.VerifyState("CONFIRM RECORDING", 5) Then
                EventRecordingState = False
            End If

            If ConfirmationState = False Then
                If EventRecordingState Then
                    _UI.Utils.EPG_Milestones_SelectMenuItem("REMINDER THIS EPISODE")

                    _UI.Utils.SendIR("SELECT")
                Else

                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventReminderFailure, "Failed To Verify Confirmation State State Entered"))
                End If
            End If

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventReminderFailure, "Failed To Verify SetReminder Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class
