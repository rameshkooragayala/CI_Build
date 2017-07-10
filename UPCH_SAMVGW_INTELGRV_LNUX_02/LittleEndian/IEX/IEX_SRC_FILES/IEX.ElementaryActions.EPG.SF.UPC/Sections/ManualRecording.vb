Imports FailuresHandler
Imports System.Globalization

Public Class ManualRecording
    Inherits IEX.ElementaryActions.EPG.SF.ManualRecording

    Dim _UI As UI
    Private CurrentMenuOnManual As String = ""
    Private SelectedChannel As String = ""
    Private SelectedDate As String = ""
    Private SelectedFrequency As String = ""
    Private SelectedStartTime As String = ""
    Private SelectedEndTime As String = ""

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub


    ''' <summary>
    '''   Navigating To Manual Recording From Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateFromPlanner()

        _UI.Utils.StartHideFailures("Navigating To Manual Recording From Planner")

        Try

            _UI.FutureRecordings.Navigate()
            _UI.Utils.EPG_Milestones_Navigate("MANUAL RECORDING")

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To RECORD After Entering All Fields On Manual Recording
    ''' </summary>
    ''' <param name="IsFromCurrent">If True Means Manual Recording From Banner Or Modify Manual Else False</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToRecord(ByVal IsFromCurrent As Boolean)
        '************************************** WORKAROUND NO EPG MILESTONES ***************************************************

        _UI.Utils.StartHideFailures("Navigating To Confirm Manual Recording")
        If IsFromCurrent Then
            _UI.Utils.EPG_Milestones_SelectMenuItem("CONFIRM")
        Else
            Try
                _UI.Utils.SendIR("SELECT_LEFT")
            Finally
                _UI.Utils.LogCommentWarning("WARNING : NO EPG MILESTONES FOR 'RECORD' TAKING SNAPSHOT INSTEAD")
                _iex.GetSnapshot("For Debug Purpose...")
                _iex.ForceHideFailure()
            End Try
        End If
    End Sub

    ''' <summary>
    '''   Getting Selected Channel Name From Manual Recording Menu
    ''' </summary>
    ''' <param name="ChannelName">Returns The Selected Channel Name</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedChannelName(ByRef ChannelName As String)
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Selected Channel Name")

        Try

            Dim ChName As String = ""

            _UI.Utils.GetEpgInfo("chName", ChName)

            ChannelName = ChName

            Msg = "Selected Channel Name : " + ChannelName

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub


    ''' <summary>
    '''   Booking Manual Recording After All Parameters Were Entered
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = False : If True Verifies Current Booking Milestones Else Future Booking Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SaveAndEnd(Optional ByVal IsCurrent As Boolean = False, Optional ByVal isModify As Boolean = False)
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Saving Manual Record")

        Try

            If IsCurrent Then
                Msg = "ManualCurrentRecord"
                Milestones = _UI.Utils.GetValueFromMilestones("ManualCurrentRecord")

            Else
                Msg = "ManualFutureRecord"
                Milestones = _UI.Utils.GetValueFromMilestones("ManualFutureRecord")
            End If

            If Not isModify Then
                NavigateToRecord(IsCurrent)
            Else
                _UI.Utils.EPG_Milestones_SelectMenuItem("CONFIRM")
            End If

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 15)

            _UI.Utils.SendIR("SELECT", 6000)

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RecordEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
