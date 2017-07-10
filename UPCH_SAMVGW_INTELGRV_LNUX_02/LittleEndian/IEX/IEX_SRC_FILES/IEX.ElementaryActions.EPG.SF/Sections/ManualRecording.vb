Imports FailuresHandler
Imports System.Globalization

Public Class ManualRecording
    Inherits IEX.ElementaryActions.EPG.ManualRecording

    Dim _UI As EPG.SF.UI
    Private CurrentMenuOnManual As String = ""
    Private SelectedChannel As String = ""
    Private SelectedDate As String = ""
    Private SelectedFrequency As String = ""
    Private SelectedStartTime As String = ""
    Private SelectedEndTime As String = ""
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Manual Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate(Optional ByVal FromCurrent As Boolean = False, Optional ByVal NoEIT As Boolean = False)
        _Utils.StartHideFailures("Navigating To Manual Recording From Banner")

        Try
            If FromCurrent Then
                If NoEIT Then
                    _Utils.EPG_Milestones_Navigate("ACTION BAR/RECORD")
                Else
                    _Utils.EPG_Milestones_NavigateByName("STATE:ADD EXTRA TIME")
                End If
            Else
                _UI.FutureRecordings.Navigate()
                _Utils.EPG_Milestones_Navigate("MANUAL RECORDING")
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Manual Recording From Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateFromPlanner()

        _Utils.StartHideFailures("Navigating To Manual Recording From Planner")

        Try

            _UI.FutureRecordings.Navigate()
            _Utils.EPG_Milestones_Navigate("MANUAL RECORDING")

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To CHANNELS On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToChannel(Optional ByVal isModify As Boolean = False)
        _Utils.StartHideFailures("Navigating To CHANNEL On Manual Recording Menu")

        Try
            _Utils.EPG_Milestones_Navigate("CHANNEL")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To DATE On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToDate(Optional ByVal isModify As Boolean = False)
        _Utils.StartHideFailures("Navigating To DATE On Manual Recording Menu")

        Try
            _Utils.EPG_Milestones_Navigate("DATE")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To START TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToStartTime(Optional ByVal isModify As Boolean = False)
        _Utils.StartHideFailures("Navigating To START TIME On Manual Recording Menu")

        Try
            _Utils.EPG_Milestones_Navigate("START TIME")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To END TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToEndTime(Optional ByVal isModify As Boolean = False)
        _Utils.StartHideFailures("Navigating To END TIME On Manual Recording Menu")

        Try
            _Utils.EPG_Milestones_Navigate("END TIME")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To FREQUENCY On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToFrequency(Optional ByVal isModify As Boolean = False)
        _Utils.StartHideFailures("Navigating To FREQUENCY On Manual Recording Menu")

        Try
            _Utils.EPG_Milestones_Navigate("FREQUENCY")
        Finally
            _iex.ForceHideFailure()
        End Try
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

        _Utils.StartHideFailures("Navigating To Confirm Manual Recording")

        Try
            _Utils.SendIR("SELECT_LEFT")
        Finally
            _Utils.LogCommentWarning("WARNING : NO EPG MILESTONES FOR 'RECORD' TAKING SNAPSHOT INSTEAD")
            _iex.GetSnapshot("For Debug Purpose...")
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Moving To Next Channel On The Channels List X Times
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : X Events To Move On Channels List</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextChannel(Optional ByVal times As Integer = 1)
        _Utils.StartHideFailures("Moving To Next Channel On The List " + times.ToString + " Times")

        Try
            For i As Integer = 1 To times
                Dim ReturnedValue As String = ""

                _UI.Utils.SendIR("SELECT_DOWN")

                _UI.Utils.GetEpgInfo("Channel", ReturnedValue)

            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Moving To Next Date On The Date List X Times
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : X Dates To Move On Date List</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextDate(Optional ByVal times As Integer = 1)

        _Utils.StartHideFailures("Moving To Next Date On The List " + times.ToString + " Times")

        Try
            For i As Integer = 1 To times

                Dim CurrentTitle As String = ""
                Dim NextTitle As String = ""

                _Utils.GetEpgInfo("title", CurrentTitle)

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", NextTitle)

                If CurrentTitle = NextTitle Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Reach Next Date"))
                End If
            Next

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

        _Utils.StartHideFailures("Inserting Start Time -> " + StartTime.ToString)

        Try
            _Utils.TypeManualRecordingKeys(StartTime)

            EpgText = _Utils.GetValueFromDictionary("DIC_CONFIRM")

            _UI.Menu.SetConfirmationMenu(EpgText)

            Dim Data As String = ""

            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("data", Data)

            If Data.Replace(":", "") <> StartTime Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Reach Manual Recording Menu"))
            End If
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

        _Utils.StartHideFailures("Inserting End Time -> " + EndTime.ToString)

        Try
            _Utils.TypeManualRecordingKeys(EndTime)

            EpgText = _Utils.GetValueFromDictionary("DIC_CONFIRM")

            _UI.Menu.SetConfirmationMenu(EpgText)

            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("data", Data)

            If Data.Replace(":", "") <> EndTime Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Reach Manual Recording Menu"))
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

        _Utils.StartHideFailures("Getting Selected Channel Name")

        Try

            Dim Data As String = ""
            Dim Title As String = ""

            _Utils.GetEpgInfo("data", Data)
            _Utils.GetEpgInfo("title", Title)

            ChannelName = Data
            CurrentMenuOnManual = Title

            Msg = "Selected Channel Name : " + ChannelName

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Getting Selected Date From Manual Recording Menu
    ''' </summary>
    ''' <param name="tDate">Returns The Selected Date</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedDate(ByRef tDate As String)
        Dim Msg As String = ""
        _Utils.StartHideFailures("Getting Selected Date")

        Try
            Dim Data As String = ""
            Dim Title As String = ""

            _Utils.GetEpgInfo("data", Data)
            _Utils.GetEpgInfo("title", Title)

            tDate = Data
            CurrentMenuOnManual = Title

            Msg = "Selected Date : " + tDate

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Getting Selected Start Time From Manual Recording Menu
    ''' </summary>
    ''' <param name="StartTime">Returns The Selected Start Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventStartTime(ByRef StartTime As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Selected Start Time")

        Try
            Dim Data As String = ""
            Dim Title As String = ""

            _Utils.GetEpgInfo("data", Data)
            _Utils.GetEpgInfo("title", Title)

            StartTime = Data
            CurrentMenuOnManual = Title

            Msg = "Event Start Time : " + StartTime

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''    Getting Selected End Time From Manual Recording Menu
    ''' </summary>
    ''' <param name="EndTime">Returns The Selected End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventEndTime(ByRef EndTime As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Selected End Time")

        Try
            Dim Data As String = ""
            Dim Title As String = ""

            _Utils.GetEpgInfo("data", Data)
            _Utils.GetEpgInfo("title", Title)

            EndTime = Data
            CurrentMenuOnManual = Title


            Msg = "Event End Time : " + EndTime

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Getting Selected Frequency From Manual Recording Menu
    ''' </summary>
    ''' <param name="Frequency">Returns The Selected Frequency</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetFrequency(ByRef Frequency As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Frequency")

        Try
            Dim Data As String = ""
            Dim Title As String = ""

            _Utils.GetEpgInfo("data", Data)
            _Utils.GetEpgInfo("title", Title)

            Frequency = Data
            CurrentMenuOnManual = Title

            Msg = "Selected Frequency : " + Frequency

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
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

        _Utils.StartHideFailures("Setting Channel To " + ChannelName.ToString)

        Try
            _UI.Menu.SetManualRecordingChannel(ChannelName)

            Dim data As String = ""

            _Utils.SendIR("SELECT", 4000)

            _Utils.GetEpgInfo("data", data)

            If data <> ChannelName Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is " + ChannelName))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Setting Channel In Channels List By Doing DCA
    ''' </summary>
    ''' <param name="Channel">Requested Channel To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Function SetChannel(ByVal Channel As Integer) As String

        _Utils.StartHideFailures("Setting Channel To " + Channel.ToString)

        Dim ChannelNumber As String = ""
        Dim ChannelName As String = ""
        Try
            _UI.Utils.TypeManualRecordingKeys(Channel.ToString)

            _iex.Wait(4)

            _Utils.GetEpgInfo("chnum", ChannelNumber)

            If ChannelNumber <> Channel.ToString Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is " + Channel.ToString))
            End If

            GetSelectedChannelName(ChannelName)

            _Utils.SendIR("SELECT", 4000)

        Finally
            _iex.ForceHideFailure()
        End Try

        Return ChannelName

    End Function

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
                EpgText = _Utils.GetValueFromDictionary("DIC_DAY_TODAY")

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

        _UI.Utils.StartHideFailures("Setting Date To " + tDate.ToUpper.ToString)

        Try
            _UI.Menu.SetManualRecordingDate(tDate.ToUpper)

            Dim ReturnedValues As New Dictionary(Of String, String)
            Dim data As String = ""

            _UI.Utils.SendIR("SELECT", 4000)

            _UI.Utils.GetEpgInfo("data", data)

            If Not data.Contains(tDate.ToUpper) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Date Is " + tDate))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Setting Start Time In Start Time Field
    ''' </summary>
    ''' <param name="StartTime">Requested Start Time To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetStartTime(ByVal StartTime As String)

        _Utils.StartHideFailures("Setting Start Time To " + StartTime.ToString)
        Try
            StartTime = Convert.ToDateTime(StartTime).ToString("HHmm")
            TypeStartTime(StartTime)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting End Time In End Time Field
    ''' </summary>
    ''' <param name="EndTime">Requested End Time To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEndTime(ByVal EndTime As String)

        _Utils.StartHideFailures("Setting End Time To " + EndTime.ToString)

        Try
            EndTime = Convert.ToDateTime(EndTime).ToString("HHmm")
            TypeEndTime(EndTime)
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
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetFrequency(ByVal Frequency As String)
        Dim FirstTitle As String = ""

        _Utils.StartHideFailures("Setting Frequency To " + Frequency.ToString)

        Try
            FirstTitle = _Utils.GetValueFromDictionary("DIC_ONE_TIME")

            Dim tFrequency As String = ""

            _Utils.GetEpgInfo("title", tFrequency)

            'If tFrequency <> FirstTitle Then
             '   ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Manual Recording Frequency Menu On Screen"))
            'End If

            _UI.Menu.SetSettingsMenuAction(Frequency)

            _Utils.SendIR("SELECT", 4000)

            _Utils.GetEpgInfo("data", tFrequency)

            If tFrequency <> Frequency Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Frequency Is - " + Frequency + " As Requested"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifying Start Time Is The Requested On Manual Recording Menu
    ''' </summary>
    ''' <param name="StartTime">Requested Start Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyStartTime(ByVal StartTime As String)
        Dim EpgText As String = ""

        _Utils.StartHideFailures("Verifying Start Time Is " + StartTime.ToString)

        Try
            Dim Data As String = ""

            _Utils.GetEpgInfo("data", Data)

            If StartTime <> Data Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Start Time Is " + StartTime))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifying End Time Is The Requested On Manual Recording Menu
    ''' </summary>
    ''' <param name="EndTime">Requested End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyEndTime(ByVal EndTime As String)
        _Utils.StartHideFailures("Verifying End Time Is " + EndTime.ToString)

        Try
            Dim Data As String = ""

            _Utils.GetEpgInfo("data", Data)

            If EndTime <> Data Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify End Time Is " + EndTime))
            End If

        Finally
            _iex.ForceHideFailure()
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

        _Utils.StartHideFailures("Saving Manual Record")

        Try

            If IsCurrent Then
                Msg = "ManualCurrentRecord"
                Milestones = _Utils.GetValueFromMilestones("ManualCurrentRecord")

            Else
                Msg = "ManualFutureRecord"
                Milestones = _Utils.GetValueFromMilestones("ManualFutureRecord")
            End If

            _Utils.BeginWaitForDebugMessages(Milestones, 15)

            _Utils.EPG_Milestones_SelectMenuItem("CONFIRM RECORDING")

            _Utils.SendIR("SELECT", 6000)

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
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
        _Utils.StartHideFailures("Verifying Manual Record Is Finished Booking IsFromCurrent=" + IsFromCurrent.ToString)

        Try
            If IsFromCurrent Then
                If Not _Utils.VerifyState("LIVE", 6) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is LIVE"))
                End If
            Else
                _UI.FutureRecordings.VerifyPlanner()
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting manual Recording Parameters
    ''' </summary>
    ''' <param name="recDate">Date to be Set</param>
    ''' <param name="recStartTime"> Recording Start Time to be set</param>
    ''' <param name="recChannelName">Channel Name to be set</param>
    ''' <param name="recChannelNum">Channel Number to be set</param>
    ''' <param name="recEndTime">Recording End Time to be set</param>
    ''' <param name="frequency">Frequency to be set</param>
    ''' <param name="isModify">Optional Parameter Default = False :If True Means modify existing recording, else New Recording </param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>

    Public Overrides Sub SetManualRecordingParams(ByVal recDate As String, ByVal recStartTime As String, ByVal recChannelName As String, ByVal recChannelNum As Integer, ByVal recEndTime As String, ByVal frequency As String, Optional ByVal isModify As Boolean = False, Optional ByVal IsFirstTime As Boolean = False)

        Dim EPGDate As String = ""
        Dim milestoneDateFormat As String = ""
        Dim defaultValueForDate As Boolean = False

        _Utils.StartHideFailures("Setting manual recording Params - Date:" + recDate + ", Start Time:" + recStartTime + ", End Time:" + recEndTime + ", Channel Name:" + recChannelName + ", Channel Number:" + recChannelNum.ToString + ", Frequency:" + frequency)

        milestoneDateFormat = _Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT")
        _UI.Live.GetEpgDate(EPGDate)
        If recDate = DateTime.ParseExact(EPGDate, milestoneDateFormat, CultureInfo.InvariantCulture).ToString(_Utils.GetEpgDateFormat()) Then
            Try
                defaultValueForDate = Convert.ToBoolean(_Utils.GetValueFromProject("MANUAL_RECORDING", "DEFAULT_DATE_SETTING_SUPPORTED"))
            Catch
                _Utils.LogCommentInfo("No default value for date specified in project, hence setting date to:" + recDate)
            End Try
        End If

        Try
            If recDate <> "" Then
                NavigateToDate(isModify)
                SetDate(recDate, defaultValueForDate)
            End If

            SetSchedule(recStartTime, recEndTime, isModify)

            If Not IsFirstTime Then
                If frequency <> "" Then
                    NavigateToFrequency(isModify)
                    SetFrequency(frequency)
                End If
            End If
           
            If recChannelName <> "" Then
                NavigateToChannel(isModify)
                SetChannel(recChannelName)
            ElseIf recChannelNum <> -1 Then
                NavigateToChannel(isModify)
                SetChannel(recChannelNum)
            Else
                _Utils.LogCommentWarning("ChannelName/ChannelNum not specified, booking on current service")
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    ''' Set the start time and end time of manual recording
    ''' </summary>
    ''' <param name="recStartTime">The recording start time</param>
    ''' <param name="recEndTime">The recording end time</param>
    ''' <param name="isModify">True if Modify required</param>
    Public Overrides Sub SetSchedule(ByVal recStartTime As String, ByVal recEndTime As String, Optional ByVal isModify As Boolean = False)

        If recStartTime <> "" Then
            NavigateToStartTime(isModify)
            SetStartTime(recStartTime)
        End If

        If recEndTime <> "" Then
            NavigateToEndTime(isModify)
            SetEndTime(recEndTime)
        End If

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

    Public Sub GetOccurences(ByVal frequency As String, ByVal daysDelay As Integer, ByRef occurences As Integer)
        Dim ScheduleDepth As Integer = 0
        Dim milestoneDateFormat As String = ""
        Dim bookingDate As String = ""
        Dim startDate As DateTime = Nothing
        Dim endDate As DateTime = Nothing
        Dim checkedDate As DateTime = Nothing

        _Utils.StartHideFailures("Getting expected number of occurences for Manual Recording Frequency:" + frequency + ",Days delay:" + daysDelay.ToString)

        Try
            ScheduleDepth = Integer.Parse(_UI.Utils.GetValueFromProject("EIT", "SCHEDULE_DEPTH_IN_DAYS"))
            milestoneDateFormat = _UI.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT")
            _UI.Live.GetEpgDate(bookingDate)
            startDate = DateTime.ParseExact(bookingDate, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(daysDelay)
            endDate = DateTime.ParseExact(bookingDate, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(ScheduleDepth - 1)
            checkedDate = startDate
            occurences = 0

            Select Case frequency
                Case _Utils.GetValueFromDictionary("DIC_ONE_TIME")
                    occurences = 1
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_DAILY")
                    occurences = ScheduleDepth - daysDelay
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_WEEKLY")
                    While checkedDate <= endDate
                        occurences += 1
                        checkedDate = checkedDate.AddDays(7)
                    End While
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_WEEKDAY")
                    While checkedDate <= endDate
                        If Not checkedDate.DayOfWeek = System.DayOfWeek.Saturday And Not checkedDate.DayOfWeek = System.DayOfWeek.Sunday Then
                            occurences += 1
                        End If
                        checkedDate = checkedDate.AddDays(1)
                    End While
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_WEEKEND")
                    While checkedDate <= endDate
                        If checkedDate.DayOfWeek <> System.DayOfWeek.Saturday Or checkedDate.DayOfWeek <> System.DayOfWeek.Sunday Then
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

            _Utils.LogCommentInfo("Expected number of occurences:" + occurences.ToString)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    ''' Getting the expected list of occurence dates for given Event
    ''' </summary>
    ''' <param name="eventKeyName">Key for the booked event</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Sub GetOccurenceList(ByVal eventKeyName As String, ByRef occurenceList As List(Of String))
        Dim ScheduleDepth As Integer
        Dim frequency As String = ""
        Dim milestoneDateFormat As String = ""
        Dim EPGDateFormat As String = ""
        Dim recStartDate As DateTime = Nothing
        Dim checkedDate As DateTime = Nothing
        Dim recEndDate As DateTime = Nothing
        Dim today As String = ""

        _Utils.StartHideFailures("Getting Occurences for Manual Recording.Event Key:" + eventKeyName)

        Try
            frequency = _UI.Events(eventKeyName).Frequency
            ScheduleDepth = Integer.Parse(_UI.Utils.GetValueFromProject("EIT", "SCHEDULE_DEPTH_IN_DAYS"))
            milestoneDateFormat = _UI.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT")
            EPGDateFormat = _UI.Utils.GetEpgDateFormat()
            recStartDate = DateTime.ParseExact(_UI.Events(eventKeyName).ConvertedDate, EPGDateFormat, CultureInfo.InvariantCulture)
            checkedDate = recStartDate
            _UI.Live.GetEpgDate(today)
             Dim year As Integer = DateTime.ParseExact(today, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(ScheduleDepth - 1).Year
            Dim endDate As String = DateTime.ParseExact(today, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(ScheduleDepth - 1).ToString(EPGDateFormat)
            Try
                recEndDate = DateTime.ParseExact(endDate, EPGDateFormat, CultureInfo.InvariantCulture)
            Catch ex As Exception
                recEndDate = DateTime.ParseExact(endDate + " " + year.ToString(), EPGDateFormat + " yyyy", CultureInfo.InvariantCulture)
            End Try
            Select Case frequency
                Case _Utils.GetValueFromDictionary("DIC_ONE_TIME")
                    occurenceList.Add(checkedDate.ToString(EPGDateFormat))
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_DAILY")
                    While checkedDate <= recEndDate
                        occurenceList.Add(checkedDate.ToString(EPGDateFormat))
                        checkedDate = checkedDate.AddDays(1)
                    End While
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_WEEKLY")
                    While checkedDate <= recEndDate
                        occurenceList.Add(checkedDate.ToString(EPGDateFormat))
                        checkedDate = checkedDate.AddDays(7)
                    End While
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_WEEKDAY")
                    While checkedDate <= recEndDate
                        If Not checkedDate.DayOfWeek = System.DayOfWeek.Saturday And Not checkedDate.DayOfWeek = System.DayOfWeek.Sunday Then
                            occurenceList.Add(checkedDate.ToString(EPGDateFormat))
                        End If
                        checkedDate = checkedDate.AddDays(1)
                    End While
                    Exit Select
                Case _Utils.GetValueFromDictionary("DIC_WEEKEND")
                    While checkedDate <= recEndDate
                        If checkedDate.DayOfWeek <> System.DayOfWeek.Saturday Or checkedDate.DayOfWeek <> System.DayOfWeek.Sunday Then
                            occurenceList.Add(checkedDate.ToString(EPGDateFormat))
                        End If
                        checkedDate = checkedDate.AddDays(1)
                    End While
                    Exit Select
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "No Valid Frequency Has Been Entered"))
                    Exit Select
            End Select

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
