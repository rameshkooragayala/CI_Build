Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.Guide

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

    ''' <summary>
    '''   Selecting Event From Guide By Pressing Select
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectEvent(Optional ByVal IsLocked As Boolean = False)
        _UI.Utils.StartHideFailures("Selecting Event On Guide")

        Try
            _UI.Utils.ClearEPGInfo()
            _UI.Utils.SendIR("SELECT")

            _UI.Utils.StartHideFailures("Checking If PIN Is Requested")
            If _UI.Utils.VerifyDebugMessage("state", "NewPinState", 5, 1) Then
                _UI.Utils.EnterPin("")
                _UI.Utils.SendIR("SELECT")
            End If
            _iex.ForceHideFailure()

            If IsLocked Then
                If Not _UI.Utils.VerifyState("ZAP CHANNEL BAR", 15, 2) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ZAP CHANNEL BAR"))
                End If
            Else
                If Not _UI.Utils.VerifyState("ACTION BAR", 15, 2) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ACTION BAR"))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Navigating To RECORD On Action Bar From Guide By Pressing RED/SELECT
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = True : If True Pressing RED Else Pressing Select For Future Events</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
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

                _UI.Utils.StartHideFailures("Checking If PIN Is Requested")
                If _UI.Utils.VerifyDebugMessage("state", "NewPinState", 5, 1) Then
                    _UI.Utils.EnterPin("")
                    _UI.Utils.SendIR("RECORD")
                End If
                _iex.ForceHideFailure()

                If Not _UI.Utils.VerifyState("CONFIRM RECORDING", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify State Is CONFIRM RECORDING"))
                End If

                EpgText = _UI.Utils.GetValueFromDictionary("DIC_ACTION_LIST_RECORD_SERIE_EVENT")

                _UI.Utils.StartHideFailures("Trying To Navigate To " + EpgText)
                Try
                    Try
                        _UI.Menu.SetActionBarSubAction(EpgText)
                    Catch ex As EAException
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
    '''   Recording Current Or Future Event From Guide
    ''' </summary>
    ''' <param name="IsCurrent">If True Recording Current Event Else Recording Future Event</param>
    ''' <param name="IsConflict">If True Not Finishing The Record But Checks For Conflict</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub RecordEvent(ByVal IsCurrent As Boolean, ByVal IsConflict As Boolean)

        _UI.Utils.StartHideFailures("Recording " + IIf(IsCurrent, "Current ", "Future ") + " Event From Guide IsConflict=" + IsConflict.ToString)

        Try
            Dim Messages As String = ""
            Dim ActualLines As New ArrayList

            If IsCurrent Then
                If IsConflict Then
                    _UI.Banner.RecordEvent(True, False, True)
                Else
                    Dim Milestones As String = ""
                    Milestones = _UI.Utils.GetValueFromMilestones("RecordCurrentEvent")

                    _UI.Utils.BeginWaitForDebugMessages(Milestones, 15)

                    _UI.Utils.SendIR("SELECT", 4000)

                    If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify RecordEvent Milestones : " + Milestones))
                    End If
                End If
            Else
                If IsConflict Then
                    _UI.Banner.RecordEvent(False, False, True)
                Else
                    _UI.Banner.RecordEvent(False, False, False)
                End If
            End If
        Finally
            _iex.ForceHideFailure()
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
        Dim GuideSurfDefault As String = ""
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("Selecting Current Event On Guide")

        Try
            Dim ReturnedValue As String = ""

            Select Case Type
                Case "WithoutPIP"

                    GuideTuneWithoutPIP_Milestones = _UI.Utils.GetValueFromMilestones("GuideTuneToCurrentEventWithoutPIP")
                    _UI.Utils.BeginWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, 15)

                Case "WithPIP", "Predicted"

                    LiveSurfPredicted_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfPredicted")
                    LiveSurfSlowZapping_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfSlowZapping")

                    _UI.Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    _UI.Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

                Case "Default"
                    GuideSurfDefault = _UI.Utils.GetValueFromMilestones("GuideSurfDefault")
                    _UI.Utils.BeginWaitForDebugMessages(GuideSurfDefault, 15)

                Case "Ignore"
                    _UI.Utils.LogCommentWarning("Skipping validation as type is Ignore!")
            End Select

            _UI.Utils.SendIR("SELECT")

            Select Case Type
                Case "WithoutPIP"
                    If Not _UI.Utils.EndWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideTuneToCurrentEventWithoutPIP Milestones : " + GuideTuneWithoutPIP_Milestones))
                    End If
                Case "WithPIP", "Predicted"
                    If Not _UI.Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
                    End If

                    If _UI.Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Verified LiveSurfSlowZapping Milestones When Shouldn't : " + LiveSurfSlowZapping_Milestones))
                    End If
                Case "Default"
                    If Not _UI.Utils.EndWaitForDebugMessages(GuideSurfDefault, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideSurfDefault Milestones : " + GuideSurfDefault))
                    End If
                Case "Ignore"
            End Select

            _UI.Utils.VerifyLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class
