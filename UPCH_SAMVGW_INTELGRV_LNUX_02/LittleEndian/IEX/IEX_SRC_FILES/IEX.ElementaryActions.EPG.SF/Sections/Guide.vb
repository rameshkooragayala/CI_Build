Imports FailuresHandler
Imports System.Globalization

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.Guide

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Guide
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _Utils.StartHideFailures("Navigating To Guide")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:TV GUIDE")
			_iex.Wait(12)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Guide Single Channel
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToGuideSingleChannel()

        _Utils.StartHideFailures("Navigating To Guide Single Channel")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:GRID BY SINGLE CHANNEL")
			_iex.Wait(12)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Navigating To Guide Adjust Timeline
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToGuideAdjustTimeline(ByVal GuideTimeline As String)

        _Utils.StartHideFailures("Navigating To Guide Adjust Timeline")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:ADJUST TIMELINE " + GuideTimeline)
			_iex.Wait(12)
            
        Finally
            _iex.ForceHideFailure()
        End Try

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
        _Utils.StartHideFailures("Selecting Event On Guide")

        Try
            _Utils.ClearEPGInfo()
            _Utils.SendIR("SELECT")

            If IsLocked Then
                If Not _UI.Utils.VerifyState("ZAP CHANNEL BAR") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ZAP CHANNEL BAR"))
                End If
            Else
                If Not _UI.Utils.VerifyState("ACTION BAR") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ACTION BAR"))
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
        Dim ReturnedValue As String = ""
        Dim Res As IEXGateway._IEXResult

        _Utils.StartHideFailures("Selecting Current Event On Guide")

        Try
            Select Case Type
                Case "WithoutPIP"

                    GuideTuneWithoutPIP_Milestones = _Utils.GetValueFromMilestones("GuideTuneToCurrentEventWithoutPIP")
                    _Utils.BeginWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, 15)

                Case "WithPIP", "Predicted"

                    LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
                    'LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")

                    _Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    '_Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

                Case "Default"
                    GuideSurfDefault = _Utils.GetValueFromMilestones("GuideSurfDefault")
                    _Utils.BeginWaitForDebugMessages(GuideSurfDefault, 15)

                Case "Ignore"
                    _Utils.LogCommentWarning("Skipping validation as type is Ignore!")
            End Select

            Res = _iex.IR.SendLongIR("SELECT", "", 3000, 2000)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Select Case Type
                Case "WithoutPIP"
                    If Not _Utils.EndWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideTuneToCurrentEventWithoutPIP Milestones : " + GuideTuneWithoutPIP_Milestones))
                    End If
                Case "WithPIP", "Predicted"
                    If Not _Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
                    End If

                    'If _Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
                       ' ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Verified LiveSurfSlowZapping Milestones When Shouldn't : " + LiveSurfSlowZapping_Milestones))
                    'End If
                Case "Default"
                    If Not _Utils.EndWaitForDebugMessages(GuideSurfDefault, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideSurfDefault Milestones : " + GuideSurfDefault))
                    End If
                Case "Ignore"
            End Select

            _Utils.VerifyLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Checks If The EPG Is On Guide Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsGuide() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Guide Is On The Screen")

        Try
            If _Utils.VerifyState("TV GUIDE", 2) Then
                Msg = "Guide Is On Screen"
                Return True
            Else
                Msg = "Guide Is Not On Screen"
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
    '''    Checks If The EPG Is On Guide Single Channel Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsGuideSingleChannel() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Guide Is On The Screen")

        Try
            If _Utils.VerifyState("ONE CHANNEL", 2) Then
                Msg = "Guide Single Channel Is On Screen"
                Return True
            Else
                Msg = "Guide Single Channel Is Not On Screen"
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
    '''    Checks If The EPG Is On Guide Single Channel Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsGuideAdjustTimeline() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Guide Is On The Screen")

        Try
            If _Utils.VerifyState("ADJUST TIMELINE", 2) Then
                Msg = "Guide Adjust Timeline Is On Screen"
                Return True
            Else
                Msg = "Guide Adjust Timeline Is Not On Screen"
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
    '''   Tunning To Channel On Guide
    ''' </summary>
    ''' <param name="ChannelNumber">The Requested Channel To Tune To</param>
    ''' <param name="VerifyFas">Optional Default = True : If True Verify ChannelSurf FAS Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToChannel(ByVal ChannelNumber As String, Optional ByVal VerifyFas As Boolean = True)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Tuning To Channel " + ChannelNumber.ToString + " On Guide")
        Try
            Dim CurrentChannel As String = ""

            GetSelectedChannelNumber(CurrentChannel)

            If ChannelNumber = CurrentChannel Then
                Msg = "Guide Already On Channel " + ChannelNumber.ToString
                Exit Sub
            End If

            TypeKeys(ChannelNumber, VerifyFas)

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Typing Channel Number Keys And Checking Channel Surf Milestones
    ''' </summary>
    ''' <param name="ChannelNumber">The Requested Channel To Type</param>
    ''' <param name="VerifyFas">Optional Default = True : If True Verify ChannelSurf FAS Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub TypeKeys(ByVal ChannelNumber As String, Optional ByVal VerifyFas As Boolean = True)
        Dim ActualLines As New ArrayList
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Entering " + ChannelNumber.ToString)

        Try
            If VerifyFas Then
                'MILESTONES MESSAGES
                Milestones = _Utils.GetValueFromMilestones("GuideSurfDefault")

                _Utils.BeginWaitForDebugMessages(Milestones, 20)
            End If

            _Utils.SendChannelAsIRSequence(ChannelNumber, 500)

            If Not _Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 10) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify Grid Is On Channel : " + ChannelNumber.ToString))
            End If
            If VerifyFas Then
                If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify ChannelSurf Milestones : " + Milestones))
                End If
            Else
                _UI.Live.VerifyChannelNumber(ChannelNumber)
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
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToRecordEvent(Optional ByVal IsCurrent As Boolean = True)
        'EPG TEXT
        Dim EpgText As String = ""

        _Utils.StartHideFailures("Navigating To RECORD On Guide")

        Try
            If IsCurrent Then

                _Utils.SendIR("RED")

                If Not _Utils.VerifyState("CONFIRM RECORDING") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify State Is CONFIRM RECORDING"))
                End If

                EpgText = _Utils.GetValueFromDictionary("DIC_ACTION_LIST_RECORD_EVENT")

                _Utils.StartHideFailures("Trying To Navigate To " + EpgText)
                Try
                    Try
                        _UI.Menu.SetActionBarSubAction(EpgText)
                    Catch ex As EAException
                        EpgText = _Utils.GetValueFromDictionary("DIC_ACTION_LIST_RECORD_SERIE_EVENT")

                        _UI.Menu.SetActionBarSubAction(EpgText)
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
    '''   Setting Reminder From Guide
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>311 - SetEventReminderFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEventReminder()

        _Utils.StartHideFailures("Setting Reminder From Guide")

        Try
            _UI.Banner.SetReminder()

            If Not _Utils.VerifyState("TV GUIDE", 20) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventReminderFailure, "Failed To Verify State Is TV GUIDE"))
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
    ''' <para>328 - INIFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub RecordEvent(ByVal IsCurrent As Boolean, ByVal IsConflict As Boolean)

        _Utils.StartHideFailures("Recording " + IIf(IsCurrent, "Current ", "Future ") + " Event From Guide IsConflict=" + IsConflict.ToString)

        Try
            Dim Messages As String = ""
            Dim ActualLines As New ArrayList

            If IsCurrent Then
                If IsConflict Then
                    _UI.Banner.RecordEvent(True, False, True)
                Else
                    _UI.Banner.RecordEvent(True, False, False)
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
    '''   Getting EventName From Guide
    ''' </summary>
    ''' <param name="EventName">Returns Selected EventName From Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedEventName(ByRef EventName As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting EventName From Guide")

        Try
            _Utils.GetEpgInfo("evtName", EventName)
            Msg = "Selected Event Name : " + EventName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
            _iex.Wait(2)
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

        _Utils.StartHideFailures("Getting Selected Event Time From Guide")

        Try
            _Utils.GetEpgInfo("evtTime", EventTime)
            Msg = "Selected Event Time : " + EventTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
            _iex.Wait(2)
        End Try
    End Sub

    ''' <summary>
    '''   Getting Event Time Left Before Start
    ''' </summary>
    ''' <param name="TimeLeft">Returnes The Time Left Until Start Of Event In Minutes</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventTimeLeftToStart(ByRef TimeLeft As String)
        Dim StartTime As String = ""
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event Time Left To Start")

        Try

            _Utils.GetEpgInfo("date", EPGDateTime)

            _Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)

            _Utils.LogCommentInfo("EPG Time : " + ReturnedEpgTime.ToString)

            _Utils.GetEpgInfo("evttime", StartTime)

            GetEventStartTime(StartTime)

            _Utils.LogCommentInfo("Event Start Time : " + StartTime.ToString)

            TimeLeft = _Utils._DateTime.Subtract(CDate(StartTime), CDate(ReturnedEpgTime))

            If TimeLeft = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Calculate Event Time Left"))
            End If

            Msg = "Time Left To Start : " + TimeLeft.ToString

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''    Getting Event Start Time From Guide
    ''' </summary>
    ''' <param name="StartTime">Returns Start Time From Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventStartTime(ByRef StartTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""
        Dim EvBothTimes As String()

        _Utils.StartHideFailures("Getting Event Start Time From Guide")

        Try
            _Utils.GetEpgInfo("evttime", StartTime)

            StartTime = StartTime.Replace(" > ", " ").Replace(" - ", " ").Replace("-", " ")
            EvBothTimes = StartTime.Split(" ")

            If EvBothTimes.Count > 1 Then
                StartTime = EvBothTimes(0)
                Msg = "Event Start Time : " + StartTime.ToString
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Get Event Start Time From Guide"))

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''    Getting Event Date From Guide
    ''' </summary>
    ''' <param name="EventDate">Returns Event Date From Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventDate(ByRef EventDate As String)
        Dim guideDateFormat As String = ""
        Dim Msg As String = ""

        guideDateFormat = _UI.Utils.GetValueFromProject("GUIDE", "EVENT_DATE_MILESTONE_FORMAT")
        _Utils.StartHideFailures("Getting Event Date From Guide")

        Try
            _Utils.GetEpgInfo("selection date", EventDate)
            EventDate = DateTime.ParseExact(EventDate, guideDateFormat, CultureInfo.InvariantCulture).ToString(_Utils.GetEpgDateFormat())
            Msg = "Event Date : " + EventDate.ToString

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Getting Selected Channel Number
    ''' </summary>
    ''' <param name="channelNumber">ByRef Channel Number</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedChannelNumber(ByRef ChannelNumber As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Channel From Guide")

        Try
            _Utils.GetEpgInfo("chnum", ChannelNumber)
            Msg = "Selected Channel Number : " + ChannelNumber
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
            _iex.Wait(2)
        End Try
    End Sub

    ''' <summary>
    ''' Verifies the selected service in Guide
    ''' </summary>
    ''' <param name="channelNum"> The channel number to be verified</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>306 - GetEventInfoFailure</para>
    ''' <para>322 - VerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub VerifySelectedEventChannel(ByVal channelNum As String)
        Dim epgFetchedChNum As String = ""
        Dim message As String = ""
        _Utils.StartHideFailures("Comparing channel number on guide")
        Try
            GetSelectedChannelNumber(epgFetchedChNum)
            If channelNum = epgFetchedChNum Then
                message = "Verified channel number on guide - " + channelNum
                Exit Sub
            End If
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel on Guide Is : " + channelNum))
        Finally
            _iex.ForceHideFailure()
            If message <> "" Then
                _Utils.LogCommentInfo(message)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Finds Event By Time On Guide
    ''' </summary>
    ''' <param name="EventTime">The Time Of The Event To Find</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub FindEventByTime(ByVal EventTime As String)
        'IMPORTANT : TIMES WILL DECIDE MAXIMUM PRESSES UNTIL FINISH SEARCHING
        Dim Times As Integer = 18
        Dim CheckedEvent As String = ""
        Dim Counter As Integer = 1

        _Utils.StartHideFailures("Finding Event With Time " + EventTime.ToString + " On Guide")

        Try

            Dim ReturnedValue As String = ""

            GetSelectedEventTime(CheckedEvent)

            If CheckedEvent = EventTime Then
                Exit Sub
            End If

            For i As Integer = 0 To Times

                NextEvent()

                GetSelectedEventTime(CheckedEvent)

                If CheckedEvent = EventTime Then
                    Exit Sub
                End If
            Next

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Verify Event By Time: " + EventTime))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Finds Event By Start Time On Guide
    ''' </summary>
    ''' <param name="EventStartTime">The Time Of The Event To Find</param>
    ''' <param name="IsExactTime">Optional Parameter Default = True,If True Finds Exact Time Else Find The First Greater Than Start Time Event</param>
    ''' <param name="DaysDelay">Optional Parameter Default = 0,Search For By The Event StartTime With Days Delay</param>
    ''' <param name="MaxLookup">Optional Parameter Default = 18,Maximum Movement On The Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub FindEventByStartTime(ByVal EventStartTime As String, Optional ByVal IsExactTime As Boolean = True, Optional ByVal DaysDelay As Integer = 0, Optional ByVal MaxLookup As Integer = 18)
        'IMPORTANT : TIMES WILL DECIDE MAXIMUM PRESSES UNTIL FINISH SEARCHING

        Dim CheckedEventStartTime As String = ""
        Dim Counter As Integer = 1
        Dim EventTimeDateTime, CheckedEventStartTimeDateTime As DateTime
        Dim logMessage As String

        If IsExactTime Then
            logMessage = "Finding Event With Time " + EventStartTime.ToString + " On Guide"
        Else
            logMessage = "Finding Event After The Time " + EventStartTime.ToString + " On Guide"
        End If


        _Utils.StartHideFailures(logMessage)

        Try

            Dim ReturnedValue As String = ""

            If DaysDelay > 0 Then
                For i As Integer = 0 To DaysDelay
                    _UI.Guide.NextDay()
                Next
            End If

            For i As Integer = 0 To MaxLookup

                GetEventStartTime(CheckedEventStartTime)

                If Not DateTime.TryParse(EventStartTime, EventTimeDateTime) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Parse Requested Event Time: " + EventStartTime))
                End If

                If Not DateTime.TryParse(CheckedEventStartTime, CheckedEventStartTimeDateTime) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Parse Found Event Time: " + CheckedEventStartTime))
                End If

                Dim CompareStatus As Integer = 0
                _Utils.LogCommentInfo("Comparing Requested EventStartTime: " + EventStartTime.ToString + " With : " + CheckedEventStartTime.ToString)

                CompareStatus = DateTime.Compare(EventTimeDateTime, CheckedEventStartTimeDateTime)
                If CompareStatus = 0 Then ' EventTime Is Earlier to Found Time On Guide
                    Exit Sub
                ElseIf CompareStatus < 0 Then
                    If Not IsExactTime Then
                        Exit Sub
                    Else
                        Exit For 'Throws Not Found Exception
                    End If
                End If

                NextEvent()

            Next

            If IsExactTime Then
                logMessage = "Failed To Verify Event With Time: " + EventStartTime.ToString + " On Guide"
            Else
                logMessage = "Failed To Verify Event After Time: " + EventStartTime.ToString + " On Guide"
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, logMessage))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Finding Requested Event Name On Guide On Selected Channel
    ''' </summary>
    ''' <param name="EventName">Requested Event Name To Find</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' </remarks>
    Public Overrides Sub FindEventByName(ByVal EventName As String)

        Dim EpgText As String = ""
        Dim CheckedEvent As String = ""

        _Utils.StartHideFailures("Finding Event " + EventName.ToString + " On Guide")

        Try
            EpgText = _Utils.GetValueFromDictionary("DIC_INFORMATION_NOT_AVAILABLE")

            Dim ReturnedValue As String = ""

            GetSelectedEventName(CheckedEvent)

            If CheckedEvent = EventName Then
                Exit Sub
            End If

            Do Until CheckedEvent = EpgText

                NextEvent()

                GetSelectedEventName(CheckedEvent)

                If CheckedEvent = EventName Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Verify Event By Event: " + EventName))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Moving To Previous Event X Times
    ''' </summary>
    ''' <param name="NumOfPresses">Optional Parameter Default = 1 : X Events To Move On Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub PreviousEvent(Optional ByVal NumOfPresses As Integer = 1)
        Dim CurEventTime As String = ""
        Dim CheckedEventTime As String = ""

        _Utils.StartHideFailures("Navigating To Previous Event " + NumOfPresses.ToString + " Times")

        Try
            For RepeatIR As Integer = 1 To NumOfPresses

                GetSelectedEventTime(CurEventTime)

                _Utils.SendIR("SELECT_LEFT", 8000)

                GetSelectedEventTime(CheckedEventTime)

                If CurEventTime = CheckedEventTime Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Previous Event On Guide CheckedEventTime = " + CheckedEventTime.ToString + " CurEventTime = " + CurEventTime.ToString))
                End If
            Next
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

        _Utils.StartHideFailures("Navigating To NextEvent Event " + NumOfPresses.ToString + " Times")

        Try
            For RepeatIR As Integer = 1 To NumOfPresses

                GetSelectedEventTime(CurEventTime)

                _Utils.SendIR("SELECT_RIGHT", 10000)

                GetSelectedEventTime(CheckedEventTime)

                If CurEventTime = CheckedEventTime Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Guide CheckedEventTime = " + CheckedEventTime.ToString + " CurEventTime = " + CurEventTime.ToString))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Moving UP/DOWN On Guide
    ''' </summary>
    ''' <param name="IsUp">If True Moves UP Else Moves Down</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub MoveChannelUpDown(ByVal IsUp As Boolean)
        Dim Direction As String = ""
        Dim CurChannel As String = ""
        Dim CheckedChannel As String = ""

        Try
            If IsUp Then
                _Utils.StartHideFailures("Moving Up On Guide")
                Direction = "UP"
            Else
                _Utils.StartHideFailures("Moving Down On Guide")
                Direction = "DOWN"
            End If

            GetSelectedChannelNumber(CurChannel)

            _Utils.SendIR("SELECT_" + Direction, 8000)

            GetSelectedChannelNumber(CheckedChannel)

            If CurChannel = CheckedChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Move " + Direction.ToString + " On Guide Channel Number Is The Same As Previous"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Surfing Up On Guide
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Surf Channel Up With PIP (UPC), "WithoutPIP" For Surf Channel Up Without PIP</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelUp(Optional ByVal Type As String = "")
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim CurEventChannel As String = ""
        Dim CheckedEventChannel As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Surfing Up On Guide")

        Try

            Milestones = GetMilestoneToValidate(Type)

            If Milestones <> "" Then
                _Utils.BeginWaitForDebugMessages(Milestones, 15)
            End If

            GetSelectedChannelNumber(CurEventChannel)

            'Fetch the channel surf up key
            Dim chSurfUpKey As String = GetSurfKey("UP")
            Dim chSurfTimeoutInMSec As String = GetChannelSurfTimeout()

            _Utils.SendIR(chSurfUpKey, chSurfTimeoutInMSec)
            GetSelectedChannelNumber(CheckedEventChannel)

            If CurEventChannel = CheckedEventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf Up On Guide CheckedEventChannel = " + CheckedEventChannel.ToString + " CurEventChannel = " + CurEventChannel.ToString))
            End If

            If Milestones <> "" Then
                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify " + Type + " Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Surfing Down On Guide
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Surf Channel Down With PIP (UPC), "WithoutPIP" For Surf Channel Down Without PIP</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelDown(Optional ByVal Type As String = "")
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim CurEventChannel As String = ""
        Dim CheckedEventChannel As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Surfing Down On Guide")

        Try

            Milestones = GetMilestoneToValidate(Type)

            If Milestones <> "" Then
                _Utils.BeginWaitForDebugMessages(Milestones, 15)
            End If

            GetSelectedChannelNumber(CurEventChannel)

            'Fetch the channel surf up key
            Dim chSurfDownKey As String = GetSurfKey("DOWN")
            Dim chSurfTimeoutInMSec As Integer = GetChannelSurfTimeout()

            _Utils.SendIR(chSurfDownKey, chSurfTimeoutInMSec)
            GetSelectedChannelNumber(CheckedEventChannel)

            If CurEventChannel = CheckedEventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf Down On Guide CheckedEventChannel = " + CheckedEventChannel.ToString + " CurEventChannel = " + CurEventChannel.ToString))
            End If

            If Milestones <> "" Then
                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify " + Type + " Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Surfing Left On Guide (Single Channel)
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Surf Channel Left With PIP (UPC), "WithoutPIP" For Surf Channel Left Without PIP</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelLeft(Optional ByVal Type As String = "")
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim CurEventChannel As String = ""
        Dim CheckedEventChannel As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Surfing Left On Guide")

        Try

            Milestones = GetMilestoneToValidate(Type)

            If Milestones <> "" Then
                _Utils.BeginWaitForDebugMessages(Milestones, 15)
            End If

            GetSelectedChannelNumber(CurEventChannel)

            'Fetch the channel surf left key
            Dim chSurfLeftKey As String = GetSurfKey("LEFT")
            Dim chSurfTimeoutInMSec As String = GetChannelSurfTimeout()

            _Utils.SendIR(chSurfLeftKey, chSurfTimeoutInMSec)
            GetSelectedChannelNumber(CheckedEventChannel)

            If CurEventChannel = CheckedEventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf Left On Guide CheckedEventChannel = " + CheckedEventChannel.ToString + " CurEventChannel = " + CurEventChannel.ToString))
            End If

            If Milestones <> "" Then
                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify " + Type + " Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Surfing Right On Guide (Single Channel)
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Surf Channel Right With PIP (UPC), "WithoutPIP" For Surf Channel Right Without PIP</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelRight(Optional ByVal Type As String = "")
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim CurEventChannel As String = ""
        Dim CheckedEventChannel As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Surfing Right On Guide")

        Try

            Milestones = GetMilestoneToValidate(Type)

            If Milestones <> "" Then
                _Utils.BeginWaitForDebugMessages(Milestones, 15)
            End If

            GetSelectedChannelNumber(CurEventChannel)

            'Fetch the channel surf right key
            Dim chSurfRightKey As String = GetSurfKey("RIGHT")
            Dim chSurfTimeoutInSec As String = GetChannelSurfTimeout()

            _Utils.SendIR(chSurfRightKey, chSurfTimeoutInSec)
            GetSelectedChannelNumber(CheckedEventChannel)

            If CurEventChannel = CheckedEventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf Right On Guide CheckedEventChannel = " + CheckedEventChannel.ToString + " CurEventChannel = " + CurEventChannel.ToString))
            End If

            If Milestones <> "" Then
                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify " + Type + " Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Get the milestone to be validated for particular surfing type
    ''' </summary>
    ''' <param name="type">Surfing type in String</param>
    ''' <returns>The milestone to be validated</returns>
    ''' <remarks></remarks>
    Public Function GetMilestoneToValidate(ByVal type As String) As String

        Dim Milestones As String = ""
        Select Case type
            Case "WithoutPIP"
                Milestones = _Utils.GetValueFromMilestones("ChannelSurfWithoutPIP")
            Case "WithPIP"
                Milestones = _Utils.GetValueFromMilestones("ChannelSurf")
            Case "Default", "Predicted"
                Milestones = _Utils.GetValueFromMilestones("GuideSurfDefault")
            Case "Ignore", ""

        End Select
        Return Milestones
    End Function

    ''' <summary>
    ''' Get the channel surf key    
    ''' </summary>
    ''' <param name="surfDirection">The surf direction as string</param>
    ''' <returns>The surf key from the project configuration or the default value from the code</returns>
    Public Overrides Function GetSurfKey(ByRef surfDirection As String) As String
        Dim surfUpKeyDefault As String = "SELECT_UP"
        Dim surfDownKeyDefault As String = "SELECT_DOWN"
        Dim surfRightKeyDefault As String = "SELECT_RIGHT"
        Dim surfLeftKeyDefault As String = "SELECT_LEFT"
        Dim surfKey As String = ""

        surfDirection = surfDirection.ToUpper
        Try
            'Get the key from Project ini
            surfKey = _Utils.GetValueFromProject("GUIDE", "CHANNEL_SURF_" + surfDirection + "_KEY")
        Catch
            'Taking default value
            _Utils.LogCommentWarning("CHANNEL_SURF_" + surfDirection + "_KEY in section GUIDE is missing in your project configuration file. Please add it if it deviates from the default value!")

            Select Case surfDirection
                Case "UP"
                    surfKey = surfUpKeyDefault
                Case "DOWN"
                    surfKey = surfDownKeyDefault
                Case "RIGHT"
                    surfKey = surfRightKeyDefault
                Case "LEFT"
                    surfKey = surfLeftKeyDefault
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Incorrect surf direction passed - " + surfDirection))
            End Select

            _Utils.LogCommentWarning("Taking default value from code instead - " + surfKey)
        End Try

        Return surfKey

    End Function

    ''' <summary>
    ''' Get the channel surf timeout required
    ''' </summary>
    ''' <returns>Timeout in milliseconds as integer</returns>
    ''' <remarks></remarks>
    Overrides Function GetChannelSurfTimeout() As Integer
        Dim chSurfTimeout As String = "3"
        Try
            chSurfTimeout = _Utils.GetValueFromProject("GUIDE", "CHANNEL_SURF_TIMEOUT_SEC")
        Catch
            _Utils.LogCommentWarning("CHANNEL_SURF_TIMEOUT_SEC in section GUIDE is missing in your project configuration file. Please add it if it deviates from the default value!")
            _Utils.LogCommentWarning("Taking default value from code instead - " + chSurfTimeout)
        End Try
        Return CInt(chSurfTimeout) * 1000
    End Function
 '''' <summary>
    ' ''' Check DaySkip Icon present in Guide view or not
    ' ''' </summary>
    ' ''' <returns>true or false</returns>
    ' ''' <remarks></remarks>
   Overrides Function VerifyDaySkipIcon(ByVal checkFwd As Boolean, ByVal checkrwd As Boolean) As Boolean
        Dim verifyIcon As String = ""
        Dim verifyIconrwd As String = ""
        Dim result As Boolean
        _iex.Wait(1)
        _Utils.GetEpgInfo("forwarddisplayicon", verifyIcon)
         _iex.Wait(2)
        _Utils.GetEpgInfo("backwarddisplayicon", verifyIconrwd)

        _iex.Wait(2)
        If checkFwd = True Then

            If verifyIcon.ToUpper.Trim() = "TRUE" Then
                _iex.LogComment("Forward skip icon verification successful")
                result = True

            Else
                _Utils.LogCommentWarning("Forward skip icon verification failed")
                result = False
            End If
        Else
            If verifyIcon.ToUpper.Trim() = "FALSE" Then

                _iex.LogComment("Forward skip icon verification successful")
                result = True
            Else
                _Utils.LogCommentWarning("Forward skip icon verification failed")
                result = False
            End If
        End If


        'checking backward icon is present or not





        If checkrwd = True Then

            If verifyIconrwd.ToUpper.Trim() = "TRUE" Then
                _iex.LogComment("Reverse skip icon verification successful")
                result = True
            Else
                _Utils.LogCommentWarning("Reverse skip icon verification failed")
                result = False
            End If
        Else
            If verifyIconrwd.ToUpper.Trim() = "FALSE" Then
                _iex.LogComment("Reverse skip icon verification successful")
                result = True
            Else
                _Utils.LogCommentWarning("Reverse skip icon verification failed")
                result = False
            End If
        End If



        If result Then
            Return True
        Else
            Return False
        End If

    End Function


    '''' <summary>
    ' ''' Verifying that fucused event is past or not
    ' ''' </summary>
    ' ''' <returns>true or false</returns>
    ' ''' <remarks></remarks>
    Public Overrides Function VerifyIsPastEvent() As Boolean
        Dim IsPastEvt As String = ""
        _Utils.StartHideFailures("Verifying focused event is past")

        _Utils.GetEpgInfo("ispastevent", IsPastEvt)

        If (Convert.ToBoolean(IsPastEvt)) Then
            _Utils.LogCommentImportant("Fucused event is the past event")
            Return True
        Else
            Return False
        End If

    End Function

    '''' <summary>
    ' ''' Check date in guide view after day skip to confirm day skip has happend or not
    ' ''' </summary>
    ' ''' <returns>true or false</returns>
    ' ''' <remarks></remarks>

    Overrides Function VerifyDateinGuidewithexpecteddate(ByVal expectedDate As String, ByVal expectedstarttime As Decimal, ByVal isForward As Boolean) As Boolean
        Dim result As Boolean
        Dim selectdateFromEPG As String = "" ' for taking selection date from EPG Info
        Dim eventstarttimeFromEPG As String = "" ' for taking duration from EPG Info

        Dim eventStarttime As Decimal ' for converting start date to decimal for verification purpose
        Dim eventNameFromEPG As String = ""

		_iex.Wait(3)
        _Utils.GetEpgInfo("selection date", selectdateFromEPG) ' get selection date from EPG Info
        _iex.Wait(1)
        GetEventStartTime(eventstarttimeFromEPG) ' get evt start time from EPG INfo

        _iex.Wait(1)

        If expectedDate = selectdateFromEPG Then  ' verify selection date
            _iex.LogComment("verify selection date successful")
            result = True
        Else
            DaySkipinGuide(isForward)
			_iex.Wait(3)
            _Utils.GetEpgInfo("selection date", selectdateFromEPG) ' get selection date from EPG Info
            _iex.Wait(2)
            If expectedDate = selectdateFromEPG Then  ' verify selection date
                _iex.LogComment("verify selection date successful in SECOND TRY")
                result = True
            Else

                _iex.LogComment("verify selection date Failed : Expected date is " + expectedDate + " but Actual date from EPFG is " + selectdateFromEPG)
                Return False
            End If
        End If

        eventStarttime = Convert.ToDecimal(eventstarttimeFromEPG.Replace(":", "."))
        _Utils.GetEpgInfo("evtname", eventNameFromEPG) ' For Printing Event Name
        _iex.Wait(1)

 If isForward Then

 Dim verifyIcon As String = ""

            _iex.Wait(1)
            _Utils.GetEpgInfo("forwarddisplayicon", verifyIcon)

            If verifyIcon.ToUpper().Trim() = "TRUE" Then

                If eventStarttime >= expectedstarttime Then ' checking event start time is greater prev day event duration

                    _iex.LogComment("Event Name: " + eventNameFromEPG)
                    _iex.LogComment("verify start time successful")
                    result = True
                ElseIf eventStarttime >= (expectedstarttime - 1.0) Then ' checking event start time is greater prev day event duration
                    _iex.LogComment("Event Name: " + eventNameFromEPG)
                    _iex.LogComment("verify start time successful")
                    result = True
                ElseIf eventNameFromEPG.Trim() = "No programme information available" Then
                    _iex.LogComment("verify there is no program information available for the selected date")
                    result = True
                ElseIf eventNameFromEPG.Trim() = "Catch-up not available" Then
                    _iex.LogComment("verify there is no program information available for the selected date")
                    result = True
                Else
                    _iex.LogComment("Event start Time : " + eventStarttime.ToString())
                    _iex.LogComment("Expected Event start Time : " + expectedstarttime.ToString())
                    _iex.LogComment("Failed to verify event start time")
                    Return False
                End If
            Else

                If eventStarttime >= (expectedstarttime - 2.5) Then ' checking event start time is greater prev day event duration
                    _iex.LogComment("Event Name: " + eventNameFromEPG)
                    _iex.LogComment("verify start time successful")
                    result = True
                ElseIf eventNameFromEPG.Trim() = "No programme information available" Then
                    _iex.LogComment("verify there is no program information available for the selected date")
                    result = True
                ElseIf eventNameFromEPG.Trim() = "Catch-up not available" Then
                    _iex.LogComment("verify there is no program information available for the selected date")
                    result = True
                Else
                    _iex.LogComment("Event start Time : " + eventStarttime.ToString())
                    _iex.LogComment("Expected Event start Time : " + expectedstarttime.ToString())
                    _iex.LogComment("Failed to verify event start time")
                    Return False
                End If
            End If
        Else
 Dim verifyIcon As String = ""

            _iex.Wait(1)
            _Utils.GetEpgInfo("backwarddisplayicon", verifyIcon)

            If verifyIcon.ToUpper().Trim() = "TRUE" Then
            If eventStarttime <= expectedstarttime Then ' checking event start time is greater prev day event duration

                _iex.LogComment("Event Name: " + eventNameFromEPG)
                _iex.LogComment("verify start time successful")
                result = True
            ElseIf eventStarttime <= (expectedstarttime + 1.0) Then ' checking event start time is greater prev day event duration
                _iex.LogComment("Event Name: " + eventNameFromEPG)
                _iex.LogComment("verify start time successful")
                result = True
            ElseIf eventNameFromEPG.Trim() = "No programme information available" Then
                _iex.LogComment("verify there is no program information available for the selected date")
                result = True
            Else
                 _iex.LogComment("Event start Time : " + eventStarttime.ToString())
                _iex.LogComment("Expected Event start Time : " + expectedstarttime.ToString())
           
                _iex.LogComment("Failed to verify event start time")
               Return  False
            End If
 Else
                If eventStarttime <= (expectedstarttime + 2.5) Then ' checking event start time is greater prev day event duration
                    _iex.LogComment("Event Name: " + eventNameFromEPG)
                    _iex.LogComment("verify start time successful")
                    result = True
                ElseIf eventNameFromEPG.Trim() = "No programme information available" Then
                    _iex.LogComment("verify there is no program information available for the selected date")
                    result = True
                Else
                    _iex.LogComment("Event start Time : " + eventStarttime.ToString())
                    _iex.LogComment("Expected Event start Time : " + expectedstarttime.ToString())
                    _iex.LogComment("Failed to verify event start time")
                    Return False
                End If
            End If
        End If

        Return result
    End Function

    '''' <summary>
    ' ''' To do Day skip action in Guide
    ' ''' </summary>
    ' ''' <returns>true or false</returns>
    ' ''' <remarks></remarks>

    Overrides Sub DaySkipinGuide(ByVal isForward As Boolean)
        Try

       
            If isForward Then  ' send fast forward IR Key 
                _iex.Wait(2)
                _iex.SendIRCommand("FASTFORWARD")
                _iex.Wait(15)
            Else                ' send rewind IR Key
                _iex.Wait(2)
                _iex.SendIRCommand("REWIND")
                _iex.Wait(15)
            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed to perform Day Skip in Guide"))

        End Try

    End Sub
    '''' <summary>
    ' ''' To verity date and starttime in  Day skip action in Guide for genre and by channel
    ' ''' </summary>
    ' ''' <returns>true or false</returns>
    ' ''' <remarks></remarks>

    Overrides Function VerifyDateinGuidewithexpecteddate_Genre_ByChannel(ByVal expectedDate As String, ByVal expectedstarttime As Decimal, ByVal isForward As Boolean) As Boolean

        Dim focusDateFromEPG As String = "" ' for taking selection date from EPG Info
        Dim eventstarttimeFromEPG As String = "" ' for taking duration from EPG Info
        Dim expectedDateInDateTime As DateTime
        Dim focusDateFromEPGInDateTime As DateTime
        Dim result As Boolean ' checking the final result

		_iex.Wait(5)
        _Utils.GetEpgInfo("selection date", focusDateFromEPG) ' get selection date from EPG Info
        expectedDateInDateTime = DateTime.ParseExact(expectedDate, "dd.MM.yyyy", CultureInfo.InvariantCulture)
        focusDateFromEPGInDateTime = DateTime.ParseExact(focusDateFromEPG, "dd.MM.yyyy", CultureInfo.InvariantCulture)

        If focusDateFromEPGInDateTime = expectedDateInDateTime Then  ' verify selection date

            _iex.LogComment("verify selection date successful")
            result = True

        Else
            _iex.LogComment("Selection date is not same as expected may be because of no EIT available for this date")

            If isForward Then

                If focusDateFromEPGInDateTime > expectedDateInDateTime Then  ' verify selection date
                    _iex.LogComment("verify selection date successful - skipped to " + focusDateFromEPG + " because no event in " + expectedDate + "")
                    result = True
                Else
                    _iex.LogComment("verify selection date Failed")
                    result = False
                End If
            Else

                If focusDateFromEPGInDateTime < expectedDateInDateTime Then  ' verify selection date
                    _iex.LogComment("verify selection date successful - skipped to " + focusDateFromEPG + " because no event in " + expectedDate + "")
                    result = True
                Else
                    _iex.LogComment("verify selection date Failed")
                    result = False
                End If
            End If
        End If


        
        Return result

    End Function

    '''' <summary>
    ' ''' To verity date  starttime &  Day skip action 
    ' ''' </summary>
    ' ''' <returns>null</returns>
    ' ''' <remarks></remarks>
    Overrides Sub VerifyDateStarTimeDisplayIcon(ByVal _isGridInCurrentDate As Boolean, ByVal _numberOfPresses As Integer, ByVal _isForward As Boolean, ByVal _isDisplayIconVerify As Boolean)
        ' get current date and evttime from EPGINFO
        Dim currentDate As String = "" ' for fetch curretn date
        Dim eventStartTimefromEPG As String = "" ' for fetcthing event start time form EPG
        Dim eventStartTime As String = ""  ' for changing event to required date format
        Dim expectedCurrentFocusDate As String = "" ' for storing expexted date format
        Dim maxDateInGuide As String ' for storing MAX days available in Guide
        Dim maxdaysInGuide As Integer ' for storing MAX days available in Guide in Integer
        Dim guideSelectiondateFormat As String ' for storing date format
        Dim maxDaysInPastEvents As Integer
        Dim maxDateInRevGrid As String = ""
        Dim reverseGrid As String = ""

        maxDaysInPastEvents = Integer.Parse(_Utils.GetValueFromProject("GUIDE", "REVERSE_GRID_MAX_DAYS"))

        'comment once reverse grid issue resolved
        
            Dim gridName As String = ""
            _Utils.GetEpgInfo("crumbtext", gridName)

        _iex.Wait(1)

        If gridName = "ALL CHANNELS" Then
            reverseGrid = "Enabled"
        Else

            reverseGrid = "Disabled"
        End If


        'uncomment once reverse grid issue resolved

        'Try
        '    _Utils.GetEpgInfo("reversegrid ", reverseGrid)
        'Catch ex As Exception
        '    _iex.LogComment("Unable to fetch reverseGrid enabled or not from EPG Info. Hence taking the value DISABLED")
        'End Try


        '_iex.Wait(1)

        'If String.IsNullOrEmpty(reverseGrid) Or reverseGrid = "" Then
        '    reverseGrid = "Disabled"
        'Else
        '    reverseGrid = "Enabled"

        'End If

        ' uncomment till here
        Try
  ' Fetch date format ofselectiondate in Guide from Project INI
       
        guideSelectiondateFormat = _Utils.GetValueFromProject("GUIDE", "GUIDESELECTIONDATEFORMAT")
 ' Fetch Max Date of EIT Available in Guide from Project INI
            maxdaysInGuide = Convert.ToInt32(_Utils.GetValueFromProject("GUIDE", "GUIDEMAXSELECTIONDATE"))

        _Utils.GetEpgInfo("date", currentDate)

        ' change currentdate format to selection date format

        currentDate = currentDate.Substring(0, 10).Replace("_", ".")

      ' getting date after Max days

            maxDateInGuide = DateTime.ParseExact(currentDate, guideSelectiondateFormat, CultureInfo.InvariantCulture).AddDays(maxdaysInGuide - 1).ToString("dd.MM.yyyy")
            maxDateInRevGrid = DateTime.ParseExact(currentDate, guideSelectiondateFormat, CultureInfo.InvariantCulture).AddDays(maxDaysInPastEvents).ToString("dd.MM.yyyy")
            ' getting start event time

        _UI.Guide.GetEventStartTime(eventStartTimefromEPG)

        eventStartTime = Convert.ToDecimal(eventStartTimefromEPG.Replace(":", "."))
        ' for verification purpose
        ' verifying after launching guide whether guide in current event / Not
        If _isGridInCurrentDate Then

            'Verify dayskip date
            If _UI.Guide.VerifyDateinGuidewithexpecteddate(currentDate, eventStartTime, _isForward) Then

                _iex.LogComment("Verified Grid is in current Date")
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Failed to verify Day Skip is not cyclic"))


            End If
        End If
        _iex.Wait(5)
        _Utils.GetEpgInfo("selection date", expectedCurrentFocusDate)
        ' getting selection date in grid
        For i As Integer = 0 To _numberOfPresses - 1
            If _isForward Then
                ' incrementing selection date to 1 and verifying against selection date after day skip

                expectedCurrentFocusDate = DateTime.ParseExact(expectedCurrentFocusDate, guideSelectiondateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(guideSelectiondateFormat)
                Else
                    ' decrementing selection date to 1 and verifying against selection date after day skip
                    expectedCurrentFocusDate = DateTime.ParseExact(expectedCurrentFocusDate, guideSelectiondateFormat, CultureInfo.InvariantCulture).AddDays(-1).ToString(guideSelectiondateFormat)
                End If

                ' day skip building block
                _UI.Guide.DaySkipinGuide(_isForward)

                _iex.Wait(2)

                'Verify dayskip date

                Dim expectedFocusDateInDateTime As DateTime = DateTime.ParseExact(expectedCurrentFocusDate, guideSelectiondateFormat, CultureInfo.InvariantCulture)
                Dim maxDateInGuideInDatetime As DateTime = DateTime.ParseExact(maxDateInGuide, guideSelectiondateFormat, CultureInfo.InvariantCulture)
                Dim currentDateInDatetime As DateTime = DateTime.ParseExact(currentDate, guideSelectiondateFormat, CultureInfo.InvariantCulture)
                'Dim currentDateInt = Integer.Parse(currentDate)

                If expectedFocusDateInDateTime > maxDateInGuideInDatetime Then
                    ' if selection date is greater than max days
                    If _UI.Guide.VerifyDateinGuidewithexpecteddate(maxDateInGuide, eventStartTime, _isForward) Then

                        _iex.LogComment("Verified DaySkip is not cyclic. Grid is not moving to first day after " + maxdaysInGuide.ToString() + " day")
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Failed to verify Day Skip is not cyclic"))
                    End If
                    '-----------------------------------------------------------------------------------------------
                ElseIf expectedFocusDateInDateTime < currentDateInDatetime And reverseGrid.Contains("Disabled") Then
                    ' if selection date less than current day
                    If _UI.Guide.VerifyDateinGuidewithexpecteddate(currentDate, eventStartTime, _isForward) Then

                        _iex.LogComment("Verified DaySkip is not cyclic.Grid is not moving to " + maxdaysInGuide.ToString() + "th day after 1st day")
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Failed to verify Day Skip is not cyclic"))
                    End If


                ElseIf expectedFocusDateInDateTime < (currentDateInDatetime.AddDays(maxDaysInPastEvents)) And reverseGrid.Contains("Enabled") Then

                    If _UI.Guide.VerifyDateinGuidewithexpecteddate(maxDateInRevGrid, eventStartTime, _isForward) Then

                        _iex.LogComment("Verified DaySkip is not cyclic.Grid is not moving to " + maxdaysInGuide.ToString() + "th day after 1st day")
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Failed to verify Day Skip is not cyclic"))
                    End If


                Else
                    If _UI.Guide.VerifyDateinGuidewithexpecteddate(expectedCurrentFocusDate, eventStartTime, _isForward) Then
                        _iex.LogComment("Day Skip verification successful")
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Failed to verify Day Skip"))
                    End If
                End If

                Dim isDisplayIconPresent As Boolean = False
                ' variable to check whether script to be failed / Not according to  Day Skip Icon Verification
                ' Verify display Icon
                '-------------------------------------------------------------------------------------------------------------------
                If reverseGrid.Contains("Enabled") And (currentDateInDatetime.AddDays(maxDaysInPastEvents)) >= expectedFocusDateInDateTime Then
                    DaySkipinGuide(_isForward)
                    isDisplayIconPresent = _UI.Guide.VerifyDaySkipIcon(True, False)

                ElseIf reverseGrid.Contains("Disabled") And (currentDateInDatetime) >= expectedFocusDateInDateTime Then
                    isDisplayIconPresent = _UI.Guide.VerifyDaySkipIcon(True, False)
                    ' for current only forward icon should be present
                ElseIf maxDateInGuideInDatetime <= expectedFocusDateInDateTime Then
                    isDisplayIconPresent = _UI.Guide.VerifyDaySkipIcon(False, True)
                Else
                    ' for 15th day only backward icon should be present
                    isDisplayIconPresent = _UI.Guide.VerifyDaySkipIcon(True, True)
                End If
                ' else both icons should be present
                If isDisplayIconPresent Then
                    _iex.LogComment("Display Icon Verification Successful")
                Else

                    If _isDisplayIconVerify Then ' if display icon veritfy = true throw error else only comment


                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Display Icon Verification Failed"))
                    Else

                        _iex.LogComment("Failed to Verify Day Skip  Icon")
                    End If

                End If
            Next

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEpgDateFailure, "Failed to verify Day Skip in guide Reason:" + ex.Message))

        End Try

    End Sub
End Class
