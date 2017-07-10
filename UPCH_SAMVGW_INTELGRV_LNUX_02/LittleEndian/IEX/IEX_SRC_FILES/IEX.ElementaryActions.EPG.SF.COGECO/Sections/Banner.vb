Imports FailuresHandler

Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.Banner

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''     Recording Event From Banner
    ''' </summary>
    ''' <param name="IsCurrent">Pressing Select On Confirm Record And If True Searching Current Event Milestones Else Searching Future Event Milestones</param>
    ''' <param name="IsResuming">Pressing Select On Confirm Record And If True Searching Resume Recording Milestones Else Not Searching For Resume Milestones</param>
    ''' <param name="IsConflict">Pressing Select On Confirm Record And Expects Conflict Screen</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' </remarks>
    Overrides Sub RecordEvent(ByVal IsCurrent As Boolean, ByVal IsResuming As Boolean, ByVal IsConflict As Boolean, Optional ByVal IsPastEvent As Boolean = False, Optional ByVal IsSeriesEvent As Boolean = False)
        'MILESTONES MESSAGES
        Dim Milestones As String = ""

        _UI.Utils.StartHideFailures("Recording Event From Banner Current=" + IsCurrent.ToString + " Resuming=" + IsResuming.ToString + " Conflict=" + IsConflict.ToString)

        Try
            Dim ActualLines As New ArrayList

            If IsResuming And IsCurrent Then
                Milestones = _UI.Utils.GetValueFromMilestones("RecordEventResumeCurrent")

            ElseIf IsCurrent And Not IsPastEvent Then
                Milestones = _UI.Utils.GetValueFromMilestones("RecordCurrentEventWithConflict")

                If Not IsConflict Then
                    Milestones = _UI.Utils.GetValueFromMilestones("RecordCurrentEvent")
                End If
            ElseIf IsPastEvent Then
                Milestones = _UI.Utils.GetValueFromMilestones("RecordPastEvent")
            Else

                Milestones = _UI.Utils.GetValueFromMilestones("RecordFutureEventWithConflict")

                If Not IsConflict Then
                    Milestones = _UI.Utils.GetValueFromMilestones("RecordFutureEvent")
                End If
            End If


            _UI.Utils.BeginWaitForDebugMessages(Milestones, 10)

            _UI.Utils.SendIR("SELECT", 14000)

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RecordEventFailure, "Failed To Verify RecordEvent Milestones " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try


    End Sub
End Class
