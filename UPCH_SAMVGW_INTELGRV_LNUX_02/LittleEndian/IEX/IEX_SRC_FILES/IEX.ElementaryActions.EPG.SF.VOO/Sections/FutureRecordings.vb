Imports FailuresHandler

Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.SF.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.VOO.UI

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

        _uUI.Utils.StartHideFailures("Checking If RECORDINGS Is On The Screen")

        Try
            If _uUI.Utils.VerifyState("MY RECORDINGS", 2) Then
                Msg = "RECORDINGS Is On Screen"
                Return True
            Else
                Msg = "RECORDINGS Is Not On Screen"
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
    '''   Checking If Planner Is Empty
    ''' </summary>
    ''' <returns>True If Empty False Is Not</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Planner Is Empty")

        Try
            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtName", EventName)
                Msg = "Planner Is Not Empty"
                Return False
            Catch ex As EAException
                Msg = "Planner Is Empty !!!"
                Return True
            End Try
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    ''' <summary>
    '''   Checks If Planner Has No Events
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyPlannerEmpty()

        _uUI.Utils.StartHideFailures("Checking If Planner Is Empty")

        Try
            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtName", EventName)
            Catch ex As EAException
                Exit Sub
            End Try

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Planner Is Empty Got Event Name : " + EventName.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Get Selected EventName From Planner
    ''' </summary>
    ''' <param name="EventName">Returns The Selected Event Name</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedEventName(ByRef EventName As String)
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Get Selected EventName From Planner")

        Try
            _uUI.Utils.GetEpgInfo("evtName", EventName)
            Msg = "Selected Event Name : " + EventName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
                _iex.Wait(2)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Verify MY RECORDINGS State Is On Screen
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyPlanner()

        _uUI.Utils.StartHideFailures("Verifying MY Recordings State Arrived")

        Try
            Dim EventName As String = ""

            If Not _uUI.Utils.VerifyState("MY RECORDINGS", 15) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify MY Recordings State"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Canceling Event From Planner
    ''' </summary>
    ''' <param name="shouldSucceed">Optional Parameter Default = True : Just For The Comment Trying/Cancel</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelEvent(Optional ByVal shouldSucceed As Boolean = True, Optional ByVal isSeriesEvent As Boolean = False, Optional ByVal isComplete As Boolean = False)
        Dim Milestones As String = ""

        _uUI.Utils.StartHideFailures(IIf(shouldSucceed, "", "Trying To ") + "Cancel Event From Planner")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("CANCEL RECORDING")

            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.VerifyState("CONFIRM DELETE") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify State Is CONFIRM DELETE"))
            End If

            _uUI.Utils.EPG_Milestones_SelectMenuItem("yes")

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _uUI.Utils.ClearEPGInfo()

            Dim ActualLines As New ArrayList
            Milestones = _uUI.Utils.GetValueFromMilestones("CancelBooking")

            _uUI.Utils.BeginWaitForDebugMessages(Milestones, 10)

            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If

            Try
                VerifyMyLibrary()
            Catch
                VerifyPlanner()
            End Try

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try

            _uUI.Utils.StartHideFailures("Navigating To Planner")

            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:FUTURE RECORDINGS")

            _iex.Wait(2)
            _uUI.Utils.SendIR("SELECT_RIGHT")
            _uUI.Utils.SendIR("SELECT_LEFT")
            _iex.Wait(2)


            '_Utils.ClearEPGInfo()

            'If _uUI.Menu.IsLibraryNoContent Then
            '    _Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY PLANNER")
            'Else
            '_Utils.EPG_Milestones_NavigateByName("STATE:FUTURE RECORDINGS")
            'End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''    Moving To Next Event In Planner X Times
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : X Events To Move On Planner</param>
    ''' <param name="VerifyByDetails">Optional Parameter Default = False : If True Comparing Detail Key Too</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _uUI.Utils.StartHideFailures("Moving Right " + times.ToString + " Times")

        Try

            For i As Integer = 1 To times
                Dim CurrentEvent As String = ""
                Dim CurrentEventDetails As String = ""
                Dim NextEventAfterMove As String = ""
                Dim NextEventDetailsAfterMove As String = ""

                GetSelectedEventName(CurrentEvent)

                If VerifyByDetails Then
                    GetSelectedEventDetails(CurrentEventDetails)
                End If

                _uUI.Utils.SendIR("SELECT_RIGHT")

                GetSelectedEventName(NextEventAfterMove)

                If VerifyByDetails Then
                    GetSelectedEventDetails(NextEventDetailsAfterMove)
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
    '''   Lock The Event
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Overrides Sub LockEvent()
        Dim Milestones As String = ""
        Dim ReturnedState As String = ""

        _uUI.Utils.StartHideFailures("Locking Event From Planner")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("LOCK EVENT")

            _uUI.Utils.SendIR("SELECT")

            _uUI.Utils.GetEPGInfo("state", ReturnedState)

            If ReturnedState.Contains("PinState") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify PIN State Entered"))
            End If

            _uUI.Utils.EnterPin("")

            VerifyPlanner()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Finding Requested Event On Planner
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
    Public Overrides Sub FindEvent(ByVal EventName As String, Optional ByVal EventDate As String = "", Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "")
        'Dim FirstEvent As String = ""
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

        'Dim FirstEventInRow As New EPG.EpgEvent(_uUI.Utils)
        'Dim PreviousEvent As New EPG.EpgEvent(_uUI.Utils)
        'Dim CurrentEvent As New EPG.EpgEvent(_uUI.Utils)
        'Dim FirstEvent As New EPG.EpgEvent(_uUI.Utils)

        Dim CurrentEvent As String = ""
        Dim CurrentEventName As String = ""
        Dim FirstEvent As String = ""
        Dim FirstEventInRow As String = ""
        Dim PreviousEvent As String = ""
        Dim FirstEventName As String = ""

        _uUI.Utils.StartHideFailures("Finding Event : " + EventName + If(String.IsNullOrEmpty(EventDate), "", " EventDate :" + EventDate) + If(String.IsNullOrEmpty(StartTime), "", " StartTime :" + StartTime) + If(String.IsNullOrEmpty(EndTime), "", " EndTime :" + EndTime) + " On Planner")

        Try

            EpgText = _uUI.Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT")

            _uUI.Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")


            GetSelectedEventDetails(CurrentEvent)
            GetSelectedEventName(CurrentEventName)

            FirstEvent = CurrentEvent

            Do

                FirstEventInRow = CurrentEvent

                Do
                    PreviousEvent = CurrentEvent

                    GetSelectedEventName(CheckedEvent)

                    If FirstLoop Then
                        FirstLoop = False
                        FirstEventName = CheckedEvent
                    End If

                    'In case of ADULT content
                    If CheckedEvent = EpgText Then
                        GetAdultEventName(CheckedEvent)
                    End If

                    If EventName.Contains(CheckedEvent) And (CheckedEvent <> "") And (EventName <> "") Then

                        If Not String.IsNullOrEmpty(EventDate) Then
                            GetSelectedEventDetails(PlannerEventDate)
                            EventDate = Date.Parse(EventDate).ToString("ddd d MMM")
                            VerifyByDetails = True
                        End If

                        If Not String.IsNullOrEmpty(StartTime) OrElse Not String.IsNullOrEmpty(EndTime) Then
                            GetSelectedEventTime(PlannerEventTime)
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

                    NextEvent(times:=1, VerifyByDetails:=VerifyByDetails)
                    GetSelectedEventDetails(CurrentEvent)
                    GetSelectedEventName(CurrentEventName)

                Loop Until CurrentEventName = FirstEventName AndAlso CurrentEvent.Equals(FirstEventInRow)
                NextDownEvent(1, True)
                GetSelectedEventDetails(CurrentEvent)
                GetSelectedEventName(CurrentEventName)

            Loop Until CurrentEventName = CheckedEvent AndAlso FirstEvent.Equals(CurrentEvent)

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Find Event : " + EventName))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifying Event Recurring Events On Planner By Name,Date,StartTime And EndTime
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <remarks></remarks>
    Public Overrides Sub VerifyRecurring(ByVal EventKeyName As String)
        Dim CheckedEvent As String = ""
        Dim FoundOccurences As Integer = 0
        Dim EventName As String = _UI.Events(EventKeyName).Name
        Dim Occurences As Integer = _UI.Events(EventKeyName).Occurrences
        Dim StartTime As String = _UI.Events(EventKeyName).StartTime
        Dim EndTime As String = _UI.Events(EventKeyName).EndTime
        Dim RecurrenceList As List(Of String) = New List(Of String)

        _UI.Utils.StartHideFailures("Finding Event " + EventName + " On Planner")

        Try
            _uUI.ManualRecording.GetOccurenceList(EventKeyName, RecurrenceList)
            For Each CheckedEvent In RecurrenceList
                Try
                    FindEvent(EventName, CheckedEvent, StartTime, EndTime)
                Catch ex As EAException
                    _UI.Utils.LogCommentFail("Failed to find event " + EventName + " EventDate: " + CheckedEvent)
                    Continue For
                End Try
                FoundOccurences += 1
            Next

            If FoundOccurences <> Occurences Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Find All Occurences Expected : " + Occurences.ToString + " But Found : " + FoundOccurences.ToString))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Moving Down In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextDownEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _UI.Utils.StartHideFailures("Moving Down " + times.ToString + " Times")
        Try
            For i As Integer = 1 To times
                Dim CurrentEvent As String = ""
                Dim CurrentEventDetails As String = ""
                Dim NextEventAfterMove As String = ""
                Dim NextEventDetailsAfterMove As String = ""

                GetSelectedEventName(CurrentEvent)

                If VerifyByDetails Then
                    GetSelectedEventDetails(CurrentEventDetails)
                End If

                _UI.Utils.SendIR("SELECT_DOWN")

                GetSelectedEventName(NextEventAfterMove)

                If VerifyByDetails Then
                    GetSelectedEventDetails(NextEventDetailsAfterMove)
                End If

                If CurrentEvent = NextEventAfterMove AndAlso CurrentEventDetails = NextEventDetailsAfterMove Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Down Event On Archive"))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class

