Imports FailuresHandler

Public Class Banner
    Inherits IEX.ElementaryActions.EPG.Banner

    Dim _UI As UI
    Private Shadows _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Action Bar
    ''' </summary>
    ''' <remarks></remarks>
    Overrides Sub Navigate(Optional ByVal FromPlayback As Boolean = False)

        _Utils.StartHideFailures("Navigating To Action Bar")
        Try

            If IsActionBar() Then
                Exit Sub
            End If

            If FromPlayback Then
                _Utils.EPG_Milestones_Navigate("MAIN MENU/PLAYBACK/ACTION BAR")
            Else
                _Utils.EPG_Milestones_Navigate("MAIN MENU/LIVE/ACTION BAR")
            End If


        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Checking If Action Bar Is Already On Screen
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsActionBar() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Action Bar Is On Screen")

        Try
            If _Utils.VerifyState("ACTION BAR", 2) Then
                Msg = "ActionBar Is On Screen"
                Return True
            Else
                Msg = "ActionBar Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    ''' <summary>
    '''   Getting Event Time Left To End
    ''' </summary>
    ''' <param name="TimeLeft">Returnes The Time Left Until End Of Event In Minutes</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventTimeLeft(ByRef TimeLeft As Integer)
        Dim EndTime As String = ""
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event Time Left To End")

        Try
            _Utils.GetEpgInfo("date", EPGDateTime)

            _Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)

            _Utils.LogCommentInfo("EPG Time : " + ReturnedEpgTime.ToString)

            GetEventEndTime(EndTime)

            Try
                TimeLeft = _Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(ReturnedEpgTime))
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParseEventTimeFailure, "Failed To Substract Time EndTime : " + EndTime + " EpgTime : " + ReturnedEpgTime))
            End Try

            Msg = "Time Left To End : " + TimeLeft.ToString
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    Public Overrides Sub GetEpgTime(ByRef EPGTime As Date)
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Current EPG Time")
        Try
            _Utils.GetEpgInfo("date", EPGDateTime)

            _Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)
            Try
                EPGTime = Convert.ToDateTime(ReturnedEpgTime)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParseEventTimeFailure, "Failed To Convert " + ReturnedEpgTime + " to DateTime"))
            End Try

            Msg = "Current EPG Time: " + ReturnedEpgTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''  Getting Event Time from Start
    ''' </summary>
    ''' <param name="TimePassed">Returnes The Time From Beginning Of Event In Minutes</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventTimePassed(ByRef TimePassed As Integer)
        Dim StartTime As String = ""
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Time Passed From Start Of the Event")

        Try
            _Utils.GetEpgInfo("date", EPGDateTime)

            _Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)

            GetEventStartTime(StartTime)

            Try
                TimePassed = _Utils._DateTime.SubtractInSec(CDate(ReturnedEpgTime), CDate(StartTime))
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParseEventTimeFailure, "Failed To Substract StartTime : " + StartTime + " EpgTime : " + ReturnedEpgTime))
            End Try

            Msg = "Time Passed From Start of Event : " + TimePassed.ToString
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Getting Channel Number From Action Bar
    ''' </summary>
    ''' <param name="ChannelNumber">ByRef The Returned Channel Number</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetChannelNumber(ByRef ChannelNumber As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Channel Number From Action Bar")

        Try
            Dim ReturnedValue As String = ""

            _Utils.GetEpgInfo("chNum", ChannelNumber)

            Msg = "Channel Number : " + ChannelNumber
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Preparing Recording Event From Action Bar - Navigating To Confirm Record Without Confirming The Record
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>323 - VerifyStateFailure</para> 
    ''' </remarks>
    Public Overrides Sub PreRecordEvent(Optional ByVal IsSeries As Boolean = False)

        _Utils.StartHideFailures("Preparing Recording Event From Action Bar")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("RECORD")

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("CONFIRM RECORDING") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify State Is CONFIRM RECORDING"))
            End If

            If IsSeries Then
                Try
                    _Utils.EPG_Milestones_SelectMenuItem("RECORD ALL EPISODES")
                Catch ex As Exception
                    _iex.ForceHideFailure()
                End Try
            Else
                Try
                    _Utils.EPG_Milestones_SelectMenuItem("CONFIRM RECORDING")
                Catch ex As Exception
                    _Utils.EPG_Milestones_SelectMenuItem("RECORD THIS EPISODE")
                End Try
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

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

        _Utils.StartHideFailures("Setting Event Reminder From Action Bar")
        Try

            _Utils.EPG_Milestones_SelectMenuItem("ADD REMINDER")

            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("SetReminder")

            _Utils.BeginWaitForDebugMessages(Milestones, 120)

            _Utils.SendIR("SELECT")

            Dim EventRecordingState As Boolean = True
            Dim ConfirmationState As Boolean = True

            If Not _Utils.VerifyState("CONFIRMATION", 5) Then
                ConfirmationState = False
            End If

            If Not _Utils.VerifyState("CONFIRM RECORDING", 5) Then
                EventRecordingState = False
            End If

            If ConfirmationState = False Then
                If EventRecordingState Then

                    _UI.Utils.EPG_Milestones_SelectMenuItem("REMINDER THIS EPISODE")

                    _Utils.SendIR("SELECT")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventReminderFailure, "Failed To Verify Confirmation State State Entered"))
                End If
            End If

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventReminderFailure, "Failed To Verify SetReminder Milestones : " + Milestones))
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

        _Utils.StartHideFailures("Setting Subtitles Language To " + Language + " From Action Bar")

        Try

            _Utils.EPG_Milestones_SelectMenuItem("A//V SETTINGS")

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("A//V SETTINGS") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is A/V SETTINGS"))
            End If

            _UI.Utils.EPG_Milestones_SelectMenuItem("SUBTITLES")

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("SUBTITLES") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is SUBTITLES SELECTION"))
            End If

            _UI.Menu.SetActionBarSubAction(Language)

            Milestones = _Utils.GetValueFromMilestones("SetSubtitlesLanguage")

            _Utils.BeginWaitForDebugMessages(Milestones, 60)

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState(expState, 25) Then

                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is " & expState))
            End If

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify SetSubtitlesLanguage Milestones : " + Milestones))
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

        _Utils.StartHideFailures("Recording Event From Banner Current=" + IsCurrent.ToString + " Resuming=" + IsResuming.ToString + " Conflict=" + IsConflict.ToString)

        Try

            Dim ActualLines As New ArrayList

            If IsResuming And IsCurrent Then
                Milestones = _Utils.GetValueFromMilestones("RecordEventResumeCurrent")

            ElseIf IsCurrent And Not IsPastEvent Then
                Milestones = _Utils.GetValueFromMilestones("RecordCurrentEventWithConflict")

                If Not IsConflict Then
                    Milestones = _Utils.GetValueFromMilestones("RecordCurrentEvent")
                End If
            ElseIf IsPastEvent Then
                Milestones = _UI.Utils.GetValueFromMilestones("RecordPastEvent")
            Else
                Milestones = _Utils.GetValueFromMilestones("RecordFutureEventWithConflict")

                If Not IsConflict Then
                    Milestones = _Utils.GetValueFromMilestones("RecordFutureEvent")
                End If
            End If			
            If IsSeriesEvent And IsCurrent Then
                Milestones = _Utils.GetValueFromMilestones("RecordEntireSeries")
            ElseIf IsSeriesEvent Then
                Milestones = _Utils.GetValueFromMilestones("BookedEntireSeries")
            End If

            _Utils.BeginWaitForDebugMessages(Milestones, 25)

            _Utils.SendIR("SELECT", 4000)

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RecordEventFailure, "Failed To Verify RecordEvent Milestones " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Pausing Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub PauseEvent()

        _Utils.StartHideFailures("Pausing Event From Action Bar")

        Try

            _Utils.EPG_Milestones_SelectMenuItem("PAUSE")

            Dim Milestones As String = ""

            Milestones = _Utils.GetValueFromMilestones("TrickModeSpeed")
            Milestones += "0"

            _Utils.VerifyFas("SELECT", Milestones, 20, True)

            _iex.Wait(2)

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

        _Utils.StartHideFailures("Stopping Recording Event From Action Bar")

        Try
            _Utils.LogCommentInfo("Banner.StopRecording : Stopping Recording Event From Action Bar")

            _Utils.EPG_Milestones_SelectMenuItem("STOP RECORDING")

            _Utils.ClearEPGInfo()

            'MILESTONES MESSAGES

            Milestones = _Utils.GetValueFromMilestones("StopRecording")

            Dim ActualLines As New ArrayList

            _Utils.BeginWaitForDebugMessages(Milestones, 10)

            _Utils.ClearEPGInfo()

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify StopRecording Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

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

        _Utils.StartHideFailures("Canceling Booked Event From Action Bar")

        Try
            If IsSeries Then
            _Utils.StartHideFailures("Trying To Navigate To CANCEL THIS EPISODE")
                If IsComplete Then
            Try
                        _Utils.EPG_Milestones_SelectMenuItem("CANCEL ALL EPISODES")
                    Catch ex2 As Exception
                        _iex.ForceHideFailure()
                    End Try
                Else
                Try
                    _Utils.EPG_Milestones_SelectMenuItem("CANCEL THIS EPISODE")
                Catch ex2 As Exception
                    _iex.ForceHideFailure()
                    End Try
                End If
            Else
                    _Utils.StartHideFailures("Trying To Navigate To CANCEL RECORDING")
                    _Utils.EPG_Milestones_SelectMenuItem("CANCEL RECORDING")
            End If

            Dim ActualLines As New ArrayList

            'MILESTONES MESSAGES
            Milestones = _Utils.GetValueFromMilestones("CancelBooking")

            _Utils.BeginWaitForDebugMessages(Milestones, 15)

            'Not Checking If Action Bar Disappeared Cause When Canceling Event From Guide Need To Check Change And Not Disappear
            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
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

        _Utils.StartHideFailures("Getting Event Name From Action Bar")

        Try
            _Utils.GetEpgInfo("evtName", EventName)
            Msg = "Event Name : " + EventName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''    Getting Event Start Time From Action Bar
    ''' </summary>
    ''' <param name="StartTime">Returns Start Time From Action Bar</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventStartTime(ByRef StartTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event Start Time From Action Bar")

        Try
            _Utils.GetEpgInfo("evttime", EvTime)
            _Utils.ParseEventTime(StartTime, EvTime, IsStartTime:=True)
            Msg = "Event Start Time : " + StartTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Getting Event End Time From Action Bar
    ''' </summary>
    ''' <param name="ReturnedEndTime">Returns End Time From Action Bar</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventEndTime(ByRef ReturnedEndTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event End Time From Action Bar")

        Try
            _Utils.GetEpgInfo("evttime", EvTime)

            _Utils.ParseEventTime(ReturnedEndTime, EvTime, IsStartTime:=False)

            Msg = "Event End Time : " + ReturnedEndTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''    Verifying Event Name Is On Action Bar
    ''' </summary>
    ''' <param name="EventName">Event Name To Verify</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Overrides Sub VerifyEventName(ByVal EventName As String)
        Dim REventName As String = ""

        _Utils.StartHideFailures("Verifying Event Name " + EventName.ToString + " Is On Action Bar")

        Try
            _Utils.GetEpgInfo("evtName", REventName)

            If EventName = REventName Then
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify EventName : " + EventName + " On Banner"))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Removing Event Keep From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>312 - SetEventKeepFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub RemoveEventKeep()

        _Utils.StartHideFailures("Removing Event Keep")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("UNPROTECT")

            'MILESTONES MESSAGES
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("SetUnKeep")

            _Utils.BeginWaitForDebugMessages(Milestones, 10)

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventKeepFailure, "Failed To Verify SetUnKeep Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting Event Keep From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>312 - SetEventKeepFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEventKeep()

        _Utils.StartHideFailures("Setting Event Keep")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("PROTECT")

            'MILESTONES MESSAGES
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("SetKeep")

            _Utils.BeginWaitForDebugMessages(Milestones, 10)

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventKeepFailure, "Failed To Verify SetKeep Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''  Pressing Select On Banner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub SelectItem()
        _UI.Utils.StartHideFailures("Pressing Select On Banner")

        Try
            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Settings Audio Track On Banner
    ''' </summary>
    ''' <param name="AudioTrack">Requested Audio Track</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>317 - SetAudioTrackFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetAudioTrack(ByVal AudioTrack As String)

        _Utils.StartHideFailures("Setting Audio Track To " + AudioTrack)

        Try
            Dim state As String = ""
            _Utils.GetActiveState(state)
            If state = "LIVE" Then
                _Utils.EPG_Milestones_NavigateByName("STATE:AV SETTINGS AUDIO")
            ElseIf state = "PLAYBACK" Then
                _Utils.EPG_Milestones_NavigateByName("STATE:AUDIO ON PLAYBACK")
            End If

            'MILESTONES MESSAGES
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("SetAudioTrack")

            Dim nextAudioKey As String
            nextAudioKey = _Utils.GetValueFromProject("KEY_MAPPING", "NEXT_AUDIO")

            Dim title As String = ""
            Dim initialTitle As String = ""
            Dim previousTitle As String = ""

            _Utils.GetEpgInfo("title", title)
            initialTitle = title
            Do While title <> AudioTrack
                previousTitle = title
                _Utils.SendIR(nextAudioKey)
                _Utils.GetEpgInfo("title", title)
                If ((title = initialTitle) Or (title = previousTitle)) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetAudioTrackFailure, "Failed to find audio option : " + AudioTrack))
                End If
            Loop

            _Utils.BeginWaitForDebugMessages(Milestones, 3)

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetAudioTrackFailure, "Failed To Verify SetAudioTrack Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''     Cancelling Reminder From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>368 - CancelReminderFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelReminder()

        Dim Milestones As String = ""
        Dim State As String = ""

        _Utils.StartHideFailures("Cancelling Reminder")
        Try

            _Utils.EPG_Milestones_SelectMenuItem("CANCEL REMINDER")

            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("CancelReminder")

            _Utils.BeginWaitForDebugMessages(Milestones, 120)

            _Utils.SendIR("SELECT")

            Dim EventRecordingState As Boolean = True
            Dim ConfirmationState As Boolean = True

            If Not _Utils.VerifyState("CONFIRMATION", 5) Then
                ConfirmationState = False
            End If

            If Not _Utils.VerifyState("CONFIRM RECORDING", 5) Then
                EventRecordingState = False
            End If

            If ConfirmationState = False Then
                If EventRecordingState Then

                    _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL REMINDER EPISODE")

                    _Utils.SendIR("SELECT")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelReminderFailure, "Failed To Verify Confirmation State State Entered"))
                End If
            End If

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelReminderFailure, "Failed To Verify CancelReminder Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub LockChannel(Optional ByVal EnterPIN As Boolean = True)
        _Utils.StartHideFailures("Locking Channel")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("LOCK CHANNEL")

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyDebugMessage("state", "NewPinState", 5, 2) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify Enter PIN Screen Reached"))
            End If

            If EnterPIN Then
                _Utils.EnterPin("")

                _Utils.VerifyState("CONFIRMATION", 10, 10)
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub UnLockChannel()
        _Utils.StartHideFailures("UnLocking Channel")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("UNLOCK CHANNEL")

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyDebugMessage("state", "NewPinState", 5, 2) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify Enter PIN Screen Reached"))
            End If

            _Utils.EnterPin("")

            _Utils.VerifyState("CONFIRMATION", 10, 10)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub AddToFavourites()
        _Utils.StartHideFailures("Adding Channel To Favourites")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("ADD TO FAVOURITES")

            _Utils.SendIR("SELECT")

            _Utils.VerifyState("CONFIRMATION", 10, 10)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub RemoveFromFavourites()
        _Utils.StartHideFailures("Removing Channel From Favourites")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("REMOVE FROM FAVOURITES")

            _Utils.SendIR("SELECT")

            _Utils.VerifyState("CONFIRMATION", 10, 10)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Renting Current Or Future PPV Event With Purchase Confirmation
    ''' </summary>
    ''' <param name="IsCurrent">If True Renting Current Event Else Renting Future Event</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>328 - INIFailure</para>
    ''' <para>369 - RentPPVEventFailure</para>
    ''' </remarks>
    Public Overrides Sub RentPpvEvent(ByVal IsCurrent As Boolean)

        _UI.Utils.StartHideFailures("Renting PPV Event From Action Bar")

        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("RENT")

            Dim ExpectedMilestonesKey As String = If(IsCurrent, "RentCurrentPPVEvent", "RentFuturePPVEvent")
            Milestones = _UI.Utils.GetValueFromMilestones(ExpectedMilestonesKey)

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 120)

            _UI.Utils.SendIR("SELECT") 'IEX_Event_deleted ??

            Dim EventRecordingState As Boolean = True
            Dim ConfirmationState As Boolean = True

            If Not _UI.Utils.VerifyState("CONFIRMATION", 5) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RentPPVEventFailure, "Failed To Verify Confirmation State Entered"))
            Else
                _UI.Utils.SendIR("SELECT") 'YES to confirm Purchase
            End If

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RentPPVEventFailure, "Failed To Verify " & ExpectedMilestonesKey & " Milestones : " + Milestones))
            End If

            Dim EpgText As String = _UI.Utils.GetValueFromDictionary("DIC_PRESS_OK_FOR_EXIT")
            If _UI.Utils.VerifyDebugMessage("title", EpgText, 30) Then 'Purchase confirmed - press OK to exit
                _UI.Utils.SendIR("SELECT")
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RentPPVEventFailure, "Failed To Press OK For Confirmation"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
    ''' <summary>
    ''' Navigates to Info Screen in Action Bar
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToEventInfo()
        _UI.Utils.EPG_Milestones_Navigate("INFO")
    End Sub

    ''' <summary>
    ''' Refres EIT Details in Action Bar
    ''' </summary>
    ''' <remarks></remarks>

    Public Overrides Sub RefreshEITOnActionBar()
        _Utils.LogCommentInfo("By Default EIT should be refreshed")
    End Sub
End Class
