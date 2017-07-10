Imports FailuresHandler

Public Class PlannerBase
    Inherits IEX.ElementaryActions.EPG.PlannerBase
    Dim _uUI As IEX.ElementaryActions.EPG.SF.UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _uUI = UI
        _Utils = _uUI.Utils
    End Sub

    ''' <summary>
    '''   Selecting Event From Planner/Archive And Verifies Playback Action Bar Event Name
    ''' </summary>
    ''' <param name="EventName">Optional Parameter Default = "" : Only For Logging Purpose</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectEvent(Optional ByVal EventName As String = "", Optional ByVal IsSeries As Boolean = False)

        _Utils.StartHideFailures("Selecting Event " + EventName)

        Try
            _Utils.SendIR("SELECT")

            If _Utils.VerifyState("ACTION BAR") Then
                If EventName <> "" Then
                    Dim ReturnedEventName As String = ""

                    _Utils.GetEpgInfo("evtName", ReturnedEventName)
                    If EventName = ReturnedEventName Then
                        Exit Sub
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify Event Name Is The Same As Requested"))
                    End If
                Else
                    Exit Sub
                End If
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ACTION BAR"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifying Playback Ended By Event Duration In Seconds
    ''' </summary>
    ''' <param name="EventDurationInSeconds">Event Duration In Seconds</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyPlaybackEnded(ByVal EventDurationInSeconds As Integer)
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Verifying Playback Ended EventDurationInSeconds=" + EventDurationInSeconds.ToString)

        Try
            Milestones = _Utils.GetValueFromMilestones("PlaybackEOF")

            Dim Duration As Integer = EventDurationInSeconds - 60
            Dim LeftDuration As Integer

            If Duration > 30 Then

                _Utils.StartHideFailures("PlannerBase.VerifyPlaybackEnded : Verifying EOF Not Found Before The Correct Time")

                Try
                    _Utils.BeginWaitForDebugMessages(Milestones, Duration)

                    If _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEOFBOFFailure, "Verified PlaybackEOF Milestones : " + Milestones + " Before The Correct Time !"))
                    End If
                Finally
                    _iex.ForceHideFailure()
                End Try

                LeftDuration = 60
            Else
                LeftDuration = EventDurationInSeconds
            End If

            _Utils.LogCommentInfo("Verifying EOF Found In " + LeftDuration.ToString + " Seconds Left Of Event")

            _Utils.BeginWaitForDebugMessages(Milestones, LeftDuration + 60)

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyEOFBOFFailure, "Failed To Verify PlaybackEOF Milestones : " + Milestones))
            End If

            _iex.Wait(5)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Resolves Adult Event Name In Archive Or Planner By Selecting It And Enter PIN Code
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub ResolveAdultEventName()
        _Utils.StartHideFailures("Trying To Resolve ADULT Event Name")

        Try
            Dim ReturnedValue As String = ""

            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("state", ReturnedValue)

            If ReturnedValue.Contains("PincodeState") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyChannelAttributeFailure, "Failed To Verify PinCodeState Entered"))
            End If

            _Utils.EnterPin("")

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Navigates to Failed Recorded Screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToFailedEventScreen()

        _Utils.StartHideFailures("Trying to Navigate to Failed Recorded Event Screen")

        Try

            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _uUI.Utils.EPG_Milestones_Navigate("MANAGE RECORDINGS/HISTORY")

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
    ''' <summary>
    ''' Verifies the Recording Error Information
    ''' </summary>
    ''' <param name="RecordIcon">Recording Icon Present</param>
    ''' <param name="ErrDescription">Recording Error Description</param>
    ''' <remarks></remarks>
    Public Overrides Sub VerifyErrorInfo(ByVal RecordIcon As String, ByVal ErrDescription As String)

        Dim ObtainedRecordIcon As String = ""
        Dim RecordErrDescription As String = ""

        _uUI.Utils.StartHideFailures("Verifying Failed Recorded Icon and Record Error Description")

        Try
            _uUI.Utils.ClearEPGInfo()

            _uUI.Banner.NavigateToEventInfo()

            _uUI.Utils.GetEpgInfo("recordIcon", ObtainedRecordIcon)

            If Not ObtainedRecordIcon.ToLower() = RecordIcon.ToLower() Then
                _uUI.Utils.LogCommentBlack("Obtained Record Icon: " + ObtainedRecordIcon)
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Record Icon displayed is not Failed Icon"))
            End If

            _uUI.Utils.GetEpgInfo("recordErrDescription", RecordErrDescription)

            'the obtained record error description is truncated, so checking if the description contains in the expected description
            If Not ErrDescription.ToLower().Trim().Contains(RecordErrDescription.ToLower().Trim()) Then
                _uUI.Utils.LogCommentBlack("Obtained Record Error Description: " + RecordErrDescription)
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Record Error description displayed is not as expected description"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''    Moving To Next Failed Event X Times
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : X Events To Move On Planner</param>
    ''' <param name="VerifyByDetails">Optional Parameter Default = False : If True Comparing Detail Key Too</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextFailedEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _uUI.Utils.StartHideFailures("Moving Down " + times.ToString + " Times")

        Try

            For i As Integer = 1 To times
                Dim CurrentEvent As String = ""
                Dim CurrentEventDetails As String = ""
                Dim NextEventAfterMove As String = ""
                Dim NextEventDetailsAfterMove As String = ""

                _uUI.FutureRecordings.GetSelectedEventName(CurrentEvent)

                If VerifyByDetails Then
                    _uUI.FutureRecordings.GetSelectedEventDetails(CurrentEventDetails)
                End If

                _uUI.Utils.SendIR("SELECT_DOWN")

                _uUI.FutureRecordings.GetSelectedEventName(NextEventAfterMove)

                If VerifyByDetails Then
                    _uUI.FutureRecordings.GetSelectedEventDetails(NextEventDetailsAfterMove)
                End If

                If CurrentEvent = NextEventAfterMove AndAlso CurrentEventDetails = NextEventDetailsAfterMove Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Planner"))
                End If
            Next

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Finding Requested Failed Event
    ''' </summary>
    ''' <param name="EventName">The Name Of The Event</param>
    ''' <param name="EventDate">The Event Converted Date</param>
    ''' <param name="StartTime">The Event Start Time</param>
    ''' <param name="EndTime">The Event End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub FindFailedRecordedEvent(ByVal EventName As String, Optional ByVal EventDate As String = "", Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "")
        Dim FirstEvent As String = ""
        Dim CheckedEvent As String = ""
        Dim CheckedEventDate As String = ""
        Dim ReturnedValue As String = ""
        Dim EpgText As String = ""
        Dim PlannerEventDate As String = ""
        Dim PlannerEventTime As String = ""
        Dim PlannerStartTime As String = ""
        Dim PlannerEndTime As String = ""
        Dim FirstLoop As Boolean = True
        Dim VerifyByDetails As Boolean = False

        _uUI.Utils.StartHideFailures("Finding Event : " + EventName + If(String.IsNullOrEmpty(EventDate), "", " EventDate :" + EventDate) + If(String.IsNullOrEmpty(StartTime), "", " StartTime :" + StartTime) + If(String.IsNullOrEmpty(EndTime), "", " EndTime :" + EndTime) + " On Planner")

        Try

            EpgText = _Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT")

            _Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")

            Do
                _uUI.FutureRecordings.GetSelectedEventName(CheckedEvent)

                'In case of ADULT content
                If CheckedEvent = EpgText Then
                    GetAdultEventName(CheckedEvent)
                End If

                If EventName.Contains(CheckedEvent) And (CheckedEvent <> "") And (EventName <> "") Then

                    If Not String.IsNullOrEmpty(EventDate) Then
                        _uUI.FutureRecordings.GetSelectedEventDetails(PlannerEventDate)
                        VerifyByDetails = True
                    End If

                    If Not String.IsNullOrEmpty(StartTime) OrElse Not String.IsNullOrEmpty(EndTime) Then
                        _uUI.FutureRecordings.GetSelectedEventTime(PlannerEventTime)
                        VerifyByDetails = True
                    End If

                    If Not String.IsNullOrEmpty(StartTime) Then
                        _uUI.Utils.ParseEventTime(ReturnedTime:=PlannerStartTime, TimeString:=PlannerEventTime, IsStartTime:=True)
                    End If
                    If Not String.IsNullOrEmpty(EndTime) Then
                        _uUI.Utils.ParseEventTime(ReturnedTime:=PlannerEndTime, TimeString:=PlannerEventTime, IsStartTime:=False)
                    End If

                    If PlannerEventDate.Contains(EventDate.ToUpper) AndAlso StartTime = PlannerStartTime AndAlso EndTime = PlannerEndTime Then
                        _iex.ForceHideFailure()
                        Exit Sub
                    ElseIf String.IsNullOrEmpty(EventDate) AndAlso String.IsNullOrEmpty(StartTime) AndAlso String.IsNullOrEmpty(EndTime) Then
                        _iex.ForceHideFailure()
                        Exit Sub
                    End If
                End If

                NextFailedEvent(times:=1, VerifyByDetails:=VerifyByDetails)

                If FirstLoop Then
                    FirstLoop = False
                    FirstEvent = CheckedEvent
                    CheckedEventDate = PlannerEventDate
                    CheckedEvent = ""
                    PlannerEventDate = ""
                End If

            Loop Until FirstEvent = CheckedEvent AndAlso PlannerEventDate = CheckedEventDate

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Find Event : " + EventName))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    ''' Verifies whether the event is series event
    ''' </summary>
    Public Overrides Sub VerifySeriesEvent()

        _Utils.StartHideFailures("Verifying the highlighted series item...")

        Try
            Dim eventType As String = ""

            _Utils.GetEPGInfo("eventType", eventType)

            If eventType.ToLower <> "series" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "The event type fetched is not series! Verification failed."))
            End If

            'TODO:: Add more verification if required
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Expand the series event to show the events within the series booking/recording
    ''' </summary>
    Public Overrides Sub ExpandSeries()

        _Utils.StartHideFailures("Expanding the series item to list its items")

        Try
            _Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
