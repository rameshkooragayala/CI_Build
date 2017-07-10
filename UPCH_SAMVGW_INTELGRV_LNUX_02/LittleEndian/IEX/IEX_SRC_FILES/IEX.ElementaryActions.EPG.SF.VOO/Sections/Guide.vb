Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To RECORD On Action Bar From Guide By Pressing RECORD
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = True : If True Pressing RED Else Pressing Select For Future Events</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToRecordEvent(Optional ByVal IsCurrent As Boolean = True)
        'EPG TEXT
        Dim EpgText As String = ""

        _UI.Utils.StartHideFailures("Navigating To RECORD On Guide")

        Try
            If IsCurrent Then

                _UI.Utils.SendIR("RECORD")

                If Not _UI.Utils.VerifyState("CONFIRM RECORDING", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify State Is CONFIRM RECORDING"))
                End If

                EpgText = _UI.Utils.GetValueFromDictionary("DIC_ACTION_LIST_RECORD_SERIE_EVENT")

                _UI.Utils.StartHideFailures("Trying To Navigate To " + EpgText)
                Try
                    Try
                        _UI.Menu.SetActionBarSubAction(EpgText)
                    Catch ex As Exception

                        _UI.Utils.LogCommentFail("Failed To Set " + EpgText + " On Action Bar")
                    End Try

                Finally
                    _iex.ForceHideFailure()
                End Try

            Else
                SelectEvent()
                _UI.Banner.PreRecordEvent()

            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

	 ''' <summary>
    '''    Gets Selected Event Time
    ''' </summary>
    ''' <param name="EventTime">ByRef Event Time As String</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedEventTime(ByRef EventTime As String)
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Selected Event Time From Guide")

        Try
            _UI.Utils.GetEpgInfo("evtTime", EventTime)
            Msg = "Selected Event Time : " + EventTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
            _iex.Wait(5)
        End Try
    End Sub

    ''' <summary>
    '''   Selecting Current Event From Guide And Checking Tunning Fas Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectCurrentEvent(Optional ByVal Type As String = "")
        Dim LiveSurfPredicted_Milestones As String = ""
        Dim LiveSurfSlowZapping_Milestones As String = ""
        Dim GuideTuneWithoutPIP_Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim ReturnedValue As String = ""
        Dim Res As IEXGateway._IEXResult

        _UI.Utils.StartHideFailures("Selecting Current Event On Guide")

        Try
            Select Case Type
                Case "WithoutPIP"

                    GuideTuneWithoutPIP_Milestones = _UI.Utils.GetValueFromMilestones("GuideTuneToCurrentEventWithoutPIP")
                    _UI.Utils.BeginWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, 15)

                Case Else

                    LiveSurfPredicted_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfPredicted")
                    LiveSurfSlowZapping_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfSlowZapping")

                    _UI.Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    _UI.Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)
            End Select

            Res = _iex.IR.SendIR("SELECT")
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Select Case Type
                Case "WithoutPIP"
                    If Not _UI.Utils.EndWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideTuneToCurrentEventWithoutPIP Milestones : " + GuideTuneWithoutPIP_Milestones))
                    End If
                Case Else
                    If Not _UI.Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
                    End If

                    If _UI.Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Verified LiveSurfSlowZapping Milestones When Shouldn't : " + LiveSurfSlowZapping_Milestones))
                    End If
            End Select

            _UI.Utils.VerifyLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Moving To Next Event X Times
    ''' </summary>
    ''' <param name="NumOfPresses">Optional Parameter Default = 1 : X Events To Move On Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal NumOfPresses As Integer = 1)
        'Must Be EventName Since No Time In No Information Availble
        Dim CurEventTime As String = ""
        Dim CheckedEventTime As String = ""

        _UI.Utils.StartHideFailures("Navigating To NextEvent Event " + NumOfPresses.ToString + " Times")

        Try
            For RepeatIR As Integer = 1 To NumOfPresses

                GetSelectedEventTime(CurEventTime)

                _UI.Utils.SendIR("SELECT_RIGHT", 10000)
                _iex.Wait(6)

                GetSelectedEventTime(CheckedEventTime)


                If CurEventTime = CheckedEventTime Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Guide CheckedEventTime = " + CheckedEventTime.ToString + " CurEventTime = " + CurEventTime.ToString))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class
