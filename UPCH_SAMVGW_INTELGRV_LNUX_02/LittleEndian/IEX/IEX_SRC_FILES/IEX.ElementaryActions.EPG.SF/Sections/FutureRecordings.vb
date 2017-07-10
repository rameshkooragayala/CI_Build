Imports FailuresHandler
Imports System.Globalization

Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try

            _Utils.StartHideFailures("Navigating To Planner")

            _Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _Utils.ClearEPGInfo()

            If _uUI.Menu.IsLibraryNoContent Then
                _Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY PLANNER")
            Else
                _Utils.EPG_Milestones_Navigate("MY PLANNER")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Selecting Event From Planner By Pressing Select
    ''' </summary>
    ''' <param name="EventName">Optional Parameter Default = "" : If EventName Is Empty Select The First Event Else The Requested One</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectEvent(Optional ByVal EventName As String = "", Optional ByVal IsSeries As Boolean = False)

        _Utils.StartHideFailures("Selecting Event " + EventName + " From Planner")
        Dim noOfTimes As Integer = 0
		
        Try
            If (IsSeries) Then
                _Utils.SendIR("SELECT")

                While _Utils.VerifyState("ACTION BAR", 2) = False
                    noOfTimes = noOfTimes + 1
                    If (noOfTimes = 4) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate ACTION BAR from Planner"))
                        Exit While
                    End If

                    _UI.Utils.SendIR("SELECT")
                End While

            Else
                _Utils.SendIR("SELECT")
            End If

            If Not _Utils.VerifyState("ACTION BAR") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ACTION BAR"))
            End If

            If EventName <> "" Then
                Dim ReturnedEventName As String = ""

                _Utils.GetEpgInfo("evtName", ReturnedEventName)

                If EventName = ReturnedEventName Then
                    Exit Sub
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "FutureRecordings.SelectEvent : Failed To Select Event " + EventName + " From Planner"))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

        _Utils.LogCommentInfo("Selected Event From Planner Successfuly.")
    End Sub

    ''' <summary>
    '''    Checks If The EPG Is On Planner Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isPlanner() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Planner Is On The Screen")

        Try
            If _Utils.VerifyState("MY PLANNER", 2) Then
                Msg = "Planner Is On Screen"
                Return True
            Else
                Msg = "Planner Is Not On Screen"
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
    '''   Verify Planner State Is On Screen
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyPlanner()

        _Utils.StartHideFailures("Verifying Planner State Arrived")

        Try
            Dim EventName As String = ""

            If Not _Utils.VerifyState("MY PLANNER", 15) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Planner State"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verify My Library State Is On Screen
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyMyLibrary()

        _Utils.StartHideFailures("Verifying My Library State Arrived")

        Try
            Dim EventName As String = ""

            If Not _Utils.VerifyState("MY LIBRARY", 15) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify My Library State"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checking If Planner Is Empty
    ''' </summary>
    ''' <returns>True If Empty False Is Not</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Planner Is Empty")

        Try
            Dim EventName As String = ""
            _iex.Wait(3)
            Try
                _Utils.GetEpgInfo("displayTitle", EventName)
                Msg = "Planner Is Not Empty"
                Return False
            Catch ex As EAException
                Msg = "Planner Is Empty !!!"
                Return True
            End Try
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
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

        _Utils.StartHideFailures("Checking If Planner Is Empty")

        Try
            Dim EventName As String = ""
            _iex.Wait(3)
            Try
                _Utils.GetEpgInfo("displayTitle", EventName)
            Catch ex As EAException
                Exit Sub
            End Try

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Planner Is Empty Got Event Name : " + EventName.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Get Adult Event Name After Entering PIN
    ''' </summary>
    ''' <param name="EventName">ByRef The Returned EventName</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetAdultEventName(ByRef EventName As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Get Adult Event Name From Planner")

        Try
            EventName = ""

            _uUI.PlannerBase.ResolveAdultEventName()

            If Not isPlanner() Then

                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Planner Is On Screen"))
            End If

            GetSelectedEventName(EventName)

            If EventName <> "" Then
                Msg = "Adult Event Name : " + EventName
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Get Adult Event Name From Planner"))

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
                _iex.Wait(2)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Gets The Selected Event Time From Planner
    ''' </summary>
    ''' <param name="EventTime">Returns The Selected Event Time</param>
    ''' <remarks></remarks>
    Public Overrides Sub GetSelectedEventTime(ByRef EventTime As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Get Selected Event Time From Planner")

        Try
            _Utils.GetEpgInfo("evttime", EventTime)
            Msg = "Selected Event Time : " + EventTime

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
                _iex.Wait(2)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Gets The Selected Event Date From Planner
    ''' </summary>
    ''' <param name="EventDate">Returns The Selected Event Date</param>
    ''' <remarks></remarks>
    Public Overrides Sub GetSelectedEventDetails(ByRef EventDate As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Get Selected Event Details From Planner")

        Try
            _Utils.GetEpgInfo("evtdetails", EventDate)
            Msg = "Selected Event Details : " + EventDate

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
                _iex.Wait(2)
            End If
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

        _uUI.Utils.StartHideFailures("Moving Down " + times.ToString + " Times")

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

                _uUI.Utils.SendIR("SELECT_DOWN")

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
                GetSelectedEventName(CheckedEvent)

                'In case of ADULT content
                If CheckedEvent = EpgText Then
                    GetAdultEventName(CheckedEvent)
                End If

                If EventName.Contains(CheckedEvent) And (CheckedEvent <> "") And (EventName <> "") Then

                    If Not String.IsNullOrEmpty(EventDate) Then
                        GetSelectedEventDetails(PlannerEventDate)
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

                    If (PlannerEventDate.ToUpper).Contains(EventDate.ToUpper) AndAlso StartTime = PlannerStartTime AndAlso EndTime = PlannerEndTime Then
                        _iex.ForceHideFailure()
                        Exit Sub
                    ElseIf String.IsNullOrEmpty(EventDate) AndAlso String.IsNullOrEmpty(StartTime) AndAlso String.IsNullOrEmpty(EndTime) Then
                        _iex.ForceHideFailure()
                        Exit Sub
                    End If
                End If

                NextEvent(times:=1, VerifyByDetails:=VerifyByDetails)

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
    '''   Get Selected EventName From Planner
    ''' </summary>
    ''' <param name="EventName">Returns The Selected Event Name</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedEventName(ByRef EventName As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Get Selected EventName From Planner")
        _iex.Wait(3)
        Try
            _Utils.GetEpgInfo("displayTitle", EventName)
            Msg = "Selected Event Name : " + EventName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
                _iex.Wait(2)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Deleting Event From Planner
    ''' </summary>
    ''' <param name="InReviewBuffer">Optional Parameter Default = False : If True Search Different Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub DeleteEvent(Optional ByVal InReviewBuffer As Boolean = False)

        _Utils.StartHideFailures("Deleting Event From Planner InReviewBuffer=" + InReviewBuffer.ToString)

        Try

            _Utils.EPG_Milestones_SelectMenuItem("DELETE")

            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.VerifyState("CONFIRM DELETE") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify State Is CONFIRM DELETE"))
            End If

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            Dim ActualLines As New ArrayList
            Dim Milestones As String = ""

            If InReviewBuffer Then
                Milestones = _Utils.GetValueFromMilestones("DeleteInReviewBuffer")
            Else
                Milestones = _Utils.GetValueFromMilestones("DeleteNotInReviewBuffer")
            End If

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _Utils.ClearEPGInfo()

            _Utils.BeginWaitForDebugMessages(Milestones, 10)

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify DeleteEvent Milestones : " + Milestones))
            End If

            VerifyStateAfterDelete()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Modify Event From Planner
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>359 - ModifyEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub ModifyEvent()
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Navigating To Modify Event From Planner")

        Try

            _Utils.EPG_Milestones_NavigateByName("STATE:MODIFY")

            '_Utils.EPG_Milestones_SelectMenuItem("MODIFY")

            '_Utils.SendIR("SELECT")

            'If Not _Utils.VerifyState("MANUAL RECORDING DATE") Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ModifyEventFailure, "Failed To Verify State Is MANUAL RECORDING DATE"))
            'End If
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

        _Utils.StartHideFailures("Locking Event From Planner")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("LOCK EVENT")

            _Utils.SendIR("SELECT")


            VerifyPlanner()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   UnLock The Event
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Overrides Sub UnLockEvent()
        Dim Milestones As String = ""

        _Utils.StartHideFailures("UnLocking Event From Planner")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("UNLOCK EVENT")

            Dim ReturnedState As String = ""

            _uUI.Utils.SendIR("SELECT")

            _uUI.Utils.GetEpgInfo("state", ReturnedState)

            If ReturnedState.Contains("PinState") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify PIN State Entered"))
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
    Public Overrides Sub CancelEvent(Optional ByVal shouldSucceed As Boolean = True, Optional ByVal IsSeriesEvent As Boolean = False, Optional ByVal IsComplete As Boolean = False)
        Dim Milestones As String = ""

        _Utils.StartHideFailures(IIf(shouldSucceed, "", "Trying To ") + "Cancel Event From Planner")

        Try
            If IsSeriesEvent Then
                _Utils.StartHideFailures("Trying To Navigate To CANCEL THIS/ALL EPISODE")
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
            _Utils.EPG_Milestones_SelectMenuItem("DELETE")

            End If
            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.VerifyState("CONFIRM DELETE") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify State Is CONFIRM DELETE"))
            End If

            _Utils.EPG_Milestones_SelectMenuItem("YES")

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _Utils.ClearEPGInfo()

            Dim ActualLines As New ArrayList
            If IsComplete Then
                Milestones = _Utils.GetValueFromMilestones("CancelAllBooking")
            Else
            Milestones = _Utils.GetValueFromMilestones("CancelBooking")
            End If

            _Utils.BeginWaitForDebugMessages(Milestones, 10)

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If

            VerifyPlanner()

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Canceling All Events From Planner By Deleting All Events DELETE ALL
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelAllEvents()

        Dim Milestones As String = ""

        _Utils.StartHideFailures("Canceling All Events From Planner")

        Try
            _Utils.EPG_Milestones_Navigate("ACTION BAR/DELETE ALL")

            _Utils.EPG_Milestones_SelectMenuItem("YES")

            Dim ActualLines As New ArrayList

            Milestones = _Utils.GetValueFromMilestones("CancelBooking")

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _Utils.ClearEPGInfo()

            _Utils.BeginWaitForDebugMessages(Milestones, 10)

            _Utils.SendIR("SELECT")

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If

            VerifyPlanner()

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

        _Utils.StartHideFailures("Finding Event " + EventName + " On Planner")

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
    '''   Verifying Sorting ByA-Z Or ByDate
    ''' </summary>
    ''' <param name="ByTime">If True Verifyies Sorting By Time Else By Name</param>
    ''' <remarks></remarks>
    Public Overrides Sub VerifySorting(ByVal ByTime As Boolean)
        Dim FirstEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim PreviousEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim CurrentEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim FirstLoop As Boolean = True
        Dim EvDetails As String = ""

        Do
            If FirstLoop Then
                GetSelectedEventName(PreviousEvent.Name)
                GetSelectedEventTime(PreviousEvent.Time)
                _uUI.Utils.ParseEventTime(PreviousEvent.StartTime, PreviousEvent.Time, True)
                _uUI.Utils.ParseEventTime(PreviousEvent.EndTime, PreviousEvent.Time, False)
                GetSelectedEventDetails(EvDetails)
                _uUI.Utils.ParseEventDate(PreviousEvent.EventDate, EvDetails)
                PreviousEvent.EventDateAsDate = CDate(PreviousEvent.EventDate)
                FirstLoop = False
            Else
                PreviousEvent = New EpgEvent(_uUI.Utils, CurrentEvent)
            End If

            Try
                NextEvent(1, True)
            Catch ex As Exception
                Exit Sub
            End Try

            GetSelectedEventName(CurrentEvent.Name)
            GetSelectedEventTime(CurrentEvent.Time)
            _uUI.Utils.ParseEventTime(CurrentEvent.StartTime, CurrentEvent.Time, True)
            _uUI.Utils.ParseEventTime(CurrentEvent.EndTime, CurrentEvent.Time, False)
            GetSelectedEventDetails(EvDetails)
            _uUI.Utils.ParseEventDate(CurrentEvent.EventDate, EvDetails)
            CurrentEvent.EventDateAsDate = CDate(CurrentEvent.EventDate)

            If ByTime Then
                PreviousEvent.VerifyEventBeforeByDateFirst(CurrentEvent)
            Else
                PreviousEvent.VerifyEventBeforeByNameFirst(CurrentEvent)
            End If
        Loop

    End Sub
    ''' <summary>
    ''' Verifies the State reached after Deleting the Event
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub VerifyStateAfterDelete()
        Try
            VerifyPlanner()
        Catch
            VerifyMyLibrary()
        End Try
    End Sub
   
End Class