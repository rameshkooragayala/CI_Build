Imports FailuresHandler

Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.SF.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.UPC.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub


    ''' <summary>
    '''    Checks If The EPG Is On Planner Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isPlanner() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Planner Is On The Screen")

        Try
            If _uUI.Utils.VerifyState("MY RECORDINGS", 2) Then
                Msg = "Planner Is On Screen"
                Return True
            Else
                Msg = "Planner Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    ''' <summary>
    '''   Verify MY RECORDINGS State Is On Screen
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyPlanner()

        _uUI.Utils.StartHideFailures("Verifying MY RECORDINGS State Arrived")

        Try
            Dim EventName As String = ""

            If Not _uUI.Utils.VerifyState("MY RECORDINGS", 15) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify MY RECORDINGS State"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Canceling All Events From Planner By Deleting All Events DELETE ALL
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelAllEvents()

        Dim Milestones As String = ""

        _uUI.Utils.StartHideFailures("Canceling All Events From Planner")

        Try
            _uUI.Utils.EPG_Milestones_Navigate("ACTION BAR/DELETE ALL")

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            Dim ActualLines As New ArrayList

            Milestones = _uUI.Utils.GetValueFromMilestones("CancelAllBooking")

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _uUI.Utils.ClearEPGInfo()

            _uUI.Utils.BeginWaitForDebugMessages(Milestones, 10)

            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If

            VerifyPlanner()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class

