Imports FailuresHandler
Imports System.Globalization

Public Class ManualRecording
    Inherits IEX.ElementaryActions.EPG.SF.ManualRecording

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To RECORD After Entering All Fields On Manual Recording
    ''' </summary>
    ''' <param name="IsFromCurrent">For UPC: If True Means Manual Recording From Banner Or Modify Manual Else False</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToRecord(ByVal IsFromCurrent As Boolean)

        '************************************** WORKAROUND NO EPG MILESTONES ***************************************************

        _UI.Utils.StartHideFailures("Navigating To Confirm Manual Recording")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("RECORD")
        Finally
            _UI.Utils.LogCommentWarning("WARNING : NO EPG MILESTONES FOR 'RECORD' TAKING SNAPSHOT INSTEAD")
            _iex.GetSnapshot("For Debug Purpose...")
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Manual Recording From Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateFromPlanner()

        _UI.Utils.StartHideFailures("Navigating To Manual Recording From Planner")

        Try
            _UI.Utils.EPG_Milestones_NavigateByName("STATE:MANUAL RECORDINGS")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Typing Start Time On Manual Recording Start Time Field
    ''' </summary>
    ''' <param name="StartTime">The Requested Start Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub TypeStartTime(ByVal StartTime As String)
        Dim EpgText As String = ""

        _UI.Utils.StartHideFailures("Inserting Start Time -> " + StartTime.ToString)

        Try
            _UI.Utils.TypeManualRecordingKeys(StartTime)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Typing End Time On Manual Recording End Time Field
    ''' </summary>
    ''' <param name="EndTime">The Requested End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub TypeEndTime(ByVal EndTime As String)
        'EPG TEXT
        Dim EpgText As String = ""
        Dim Data As String = ""
        Dim endTimeFromData As String = ""

        _UI.Utils.StartHideFailures("Inserting End Time -> " + EndTime.ToString)

        Try
            _UI.Utils.TypeManualRecordingKeys(EndTime)

            _UI.Utils.SendIR("SELECT")

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To START TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToStartTime(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To START TIME On Manual Recording Menu")

        Try
            If Not isModify Then
                For index As Integer = 0 To 2
                    _UI.Utils.SendIR("SELECT_LEFT")
                Next
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To END TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToEndTime(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To END TIME On Manual Recording Menu")

        Try
            _UI.Utils.SendIR("SELECT_DOWN")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting Channel In Channels List
    ''' </summary>
    ''' <param name="ChannelName">Requested Channel Name To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetChannel(ByVal ChannelName As String)

        _UI.Utils.StartHideFailures("Setting Channel To " + ChannelName.ToString)

        Try
            _UI.Menu.SetManualRecordingChannel(ChannelName)

            Dim data As String = ""

            _UI.Utils.SendIR("SELECT", 4000)

         

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Setting Date In Date List
    ''' </summary>
    ''' <param name="tDate">Requested Date To Set</param>
    ''' <param name="DefaultValue">If True Only For Logging Purposes Writes Default Value</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetDate(ByVal tDate As String, ByVal DefaultValue As Boolean)
        Dim EpgText As String = ""

        If DefaultValue Then
            _UI.Utils.StartHideFailures("Setting Date To Default Value")

            Try
                EpgText = _UI.Utils.GetValueFromDictionary("DIC_DAY_TODAY")

                Dim title As String = ""

                _UI.Utils.GetEpgInfo("title", title)

                If Not title.Contains(EpgText) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Date Is TODAY"))
                End If

                _UI.Utils.SendIR("SELECT")

                Exit Sub

            Finally
                _iex.ForceHideFailure()
            End Try
        End If

        _UI.Utils.StartHideFailures("Setting Date To " + tDate)

        Try
            _UI.Menu.SetManualRecordingDate(tDate)

            Dim ReturnedValues As New Dictionary(Of String, String)
            Dim data As String = ""

            _UI.Utils.SendIR("SELECT", 4000)

            _UI.Utils.GetEpgInfo("data", data)

            If Not data.ToUpper.Contains(tDate.ToUpper()) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Date Is " + tDate))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
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
    ''' <param name="isModify">Optional Parameter Default = False :For UPC: True means Modify Manual Else False</param>
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

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 20)

            _UI.Utils.EPG_Milestones_SelectMenuItem("confirm")

            _UI.Utils.SendIR("SELECT", 6000)

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RecordEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifying After Manual Recording Booked : If Current Verifies Action Bar Is Not On Screen Else Verifies Got Back To Planner Menu
    ''' </summary>
    ''' <param name="IsFromCurrent">If True Means Manual Recording From Banner Else From Planner</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySaveAndEndFinished(ByVal IsFromCurrent As Boolean)
        _UI.Utils.StartHideFailures("Verifying Manual Record Is Finished Booking IsFromCurrent=" + IsFromCurrent.ToString)

        Try
            If IsFromCurrent Then
                If Not _UI.Utils.VerifyState("LIVE", 6) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is LIVE"))
                End If
            Else
                Try
                    _UI.FutureRecordings.VerifyMyLibrary()
                Catch
                    _UI.ArchiveRecordings.VerifyArchive()
                End Try

            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Manual Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate(Optional ByVal FromCurrent As Boolean = False, Optional ByVal NoEIT As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To Manual Recording From Banner")

        Try
            If FromCurrent Then
                If NoEIT Then
                    _UI.Utils.EPG_Milestones_Navigate("ACTION BAR/RECORD")
                Else
                    _UI.Utils.EPG_Milestones_NavigateByName("STATE:ADD EXTRA TIME")
                End If
            Else
                _UI.Utils.EPG_Milestones_NavigateByName("STATE:MANUAL RECORDINGS")
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub SetSchedule(ByVal recStartTime As String, ByVal recEndTime As String, Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Setting start time and end time")

        Try

            If recStartTime <> "" Or recEndTime <> "" Then
                _UI.Utils.EPG_Milestones_Navigate("SCHEDULE")
            End If

            If recStartTime <> "" Then
                NavigateToStartTime(isModify)
                SetStartTime(recStartTime)
            End If

            If recEndTime <> "" Then
                NavigateToEndTime(isModify)
                SetEndTime(recEndTime)
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting Frequency In Frequency List
    ''' </summary>
    ''' <param name="Frequency">Requested Frequency To Set : One Time,Daily,Weekday,Weekly,Saturday-Sun</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEPGInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetFrequency(ByVal Frequency As String)
        Dim FirstTitle As String = ""

        _UI.Utils.StartHideFailures("Setting Frequency To " + Frequency.ToString)

        Try
            'FirstTitle = _UI.Utils.GetValueFromDictionary("DIC_ONE_TIME")

            'Dim tFrequency As String = ""

            '_UI.Utils.GetEpgInfo("title", tFrequency)

            'If tFrequency <> FirstTitle Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Manual Recording Frequency Menu On Screen"))
            'End If

            _UI.Menu.SetSettingsMenuAction(Frequency)

            _UI.Utils.SendIR("SELECT", 4000)

           

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Getting expected number of occurences for the given frequency
    ''' </summary>
    ''' <param name="frequency">Frequency of Manual Recording</param>
    ''' <param name="daysDelay">Delay in days for the Record Start</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>

    Public Overloads Sub GetOccurences(ByVal frequency As String, ByVal daysDelay As Integer, ByRef occurences As Integer)
        Dim ScheduleDepth As Integer = 0
        Dim milestoneDateFormat As String = ""
        Dim bookingDate As String = ""
        Dim startDate As DateTime = Nothing
        Dim endDate As DateTime = Nothing
        Dim checkedDate As DateTime = Nothing

        _UI.Utils.StartHideFailures("Getting expected number of occurences for Manual Recording Frequency:" + frequency + ",Days delay:" + daysDelay.ToString)

        Try
            ScheduleDepth = Integer.Parse(_UI.Utils.GetValueFromProject("EIT", "SCHEDULE_DEPTH_IN_DAYS"))
            milestoneDateFormat = _UI.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT")
            _UI.Live.GetEpgDate(bookingDate)
            startDate = DateTime.ParseExact(bookingDate, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(daysDelay)
            endDate = DateTime.ParseExact(bookingDate, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(ScheduleDepth - 1)
            checkedDate = startDate
            occurences = 0

            Select Case frequency
                Case _UI.Utils.GetValueFromDictionary("DIC_ONE_TIME")
                    occurences = 1
                    Exit Select
                Case _UI.Utils.GetValueFromDictionary("DIC_DAILY")
                    occurences = ScheduleDepth - daysDelay
                    Exit Select
                Case _UI.Utils.GetValueFromDictionary("DIC_WEEKLY")
                    While checkedDate <= endDate
                        occurences += 1
                        checkedDate = checkedDate.AddDays(7)
                    End While
                    Exit Select
                Case _UI.Utils.GetValueFromDictionary("DIC_WEEKDAY")
                    While checkedDate <= endDate
                        If Not checkedDate.DayOfWeek = System.DayOfWeek.Saturday And Not checkedDate.DayOfWeek = System.DayOfWeek.Sunday Then
                            occurences += 1
                        End If
                        checkedDate = checkedDate.AddDays(1)
                    End While
                    Exit Select
                Case _UI.Utils.GetValueFromDictionary("DIC_WEEKEND")
                    While checkedDate <= endDate
                        If checkedDate.DayOfWeek = System.DayOfWeek.Saturday Or checkedDate.DayOfWeek = System.DayOfWeek.Sunday Then
                            occurences += 1
                        End If
                        checkedDate = checkedDate.AddDays(1)
                    End While
                    Exit Select
                Case Else
                    ' Taking default as ONE_TIME for event based
                    occurences = 1
                    Exit Select
            End Select

            _UI.Utils.LogCommentInfo("Expected number of occurences:" + occurences.ToString)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
