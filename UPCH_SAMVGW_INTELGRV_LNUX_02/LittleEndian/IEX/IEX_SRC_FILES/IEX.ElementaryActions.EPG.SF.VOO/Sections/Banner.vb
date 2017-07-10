Imports FailuresHandler

Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

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

        _UI.Utils.StartHideFailures("Canceling Booked Event From Action Bar")

        _UI.Utils.EPG_Milestones_Navigate("CANCEL RECORDING")

        Try
            _UI.Utils.StartHideFailures("Trying To Navigate To CANCEL THIS EPISODE")
            Try
                Try
                    _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL THIS EPISODE")
                Catch ex2 As Exception
                    _iex.ForceHideFailure()
                    _UI.Utils.StartHideFailures("Trying To Navigate To CANCEL RECORDING")
                    _UI.Utils.EPG_Milestones_SelectMenuItem("yes")
                End Try
            Finally
                _iex.ForceHideFailure()
            End Try

            Dim ActualLines As New ArrayList

            'MILESTONES MESSAGES
            Milestones = _UI.Utils.GetValueFromMilestones("CancelBooking")

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

            _UI.Utils.LogCommentInfo("Verifying State 'CONFIRMATION'")

            If Not _UI.Utils.VerifyState("CONFIRMATION", 3) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify CONFIRMATION"))
            End If

            _UI.Utils.EPG_Milestones_SelectMenuItem("yes")

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

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 32)

            _UI.Utils.SendIR("SELECT", 4000)

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RecordEventFailure, "Failed To Verify RecordEvent Milestones " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''    Sets Subtitles Language On Action Bar
    ''' </summary>
    ''' <param name="Language">Language Of Subtitles To Select</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetSubtitlesLanguage(ByVal Language As String, Optional ByVal expState As String = "LIVE")
        Dim Milestones As String = ""
        Dim State As String = ""
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("Setting Subtitles Language To " + Language + " From Action Bar")

        Try

            _UI.Utils.EPG_Milestones_SelectMenuItem("A//V SETTINGS")

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.VerifyState("A//V SETTINGS") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is A/V SETTINGS"))
            End If

            _UI.Utils.EPG_Milestones_SelectMenuItem("SUBTITLES")

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.VerifyState("SUBTITLES") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is SUBTITLES"))
            End If

            _UI.Menu.SetActionBarSubAction(Language.ToLower)

            Milestones = _UI.Utils.GetValueFromMilestones("SetSubtitlesLanguage")

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 60)

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.VerifyState(expState, 25) Then

                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is " & expState))
            End If

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify SetSubtitlesLanguage Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Getting Event Name From Action Bar
    ''' </summary>
    ''' <param name="EventName">Returns EventName From Action Bar</param>
    ''' <remarks></remarks>
    Overrides Sub GetEventName(ByRef EventName As String)
        Dim returnedValue As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event Name From Action Bar")
        _iex.Wait(5)
        Try
            _UI.Utils.GetEpgInfo("evtName", EventName)
            Msg = "Event Name : " + EventName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Refres EIT Details in Action Bar
    ''' </summary>
    ''' <remarks></remarks>

    Public Overrides Sub RefreshEITOnActionBar()
        _UI.Utils.LogCommentInfo("Performing CH+ and CH- to refresh EIT Data")
        _UI.Utils.SendIR("SELECT_DOWN")
        _UI.Utils.SendIR("SELECT_UP")
    End Sub
End Class
