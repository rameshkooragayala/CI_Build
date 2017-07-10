Imports System.Runtime.InteropServices
Imports FailuresHandler
Imports System.Globalization

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
        _Utils = _uUI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Archive Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try
            _Utils.StartHideFailures("Navigating To Archive")

            _Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _Utils.ClearEPGInfo()

            If _UI.Menu.IsLibraryNoContent Then
                _Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY RECORDINGS")
            Else
                _Utils.EPG_Milestones_Navigate("MY RECORDINGS")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Selecting Event From Archive By Pressing Select
    ''' </summary>
    ''' <param name="EventName">Optional Parameter Default = "" : If EventName Is Empty Select The First Event Else The Requested One</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectEvent(Optional ByVal EventName As String = "")

        _Utils.StartHideFailures("Selecting Event " + EventName + " From Archive")

        Try
            'Checking Event Name To Check If We Are Selecting Event From 

            _Utils.SendIR("SELECT")

            If _Utils.VerifyState("ACTION BAR") Then
                If EventName <> "" Then
                    Dim ReturnedEventName As String = ""

                    _Utils.GetEpgInfo("evtName", ReturnedEventName)

                    _Utils.LogCommentWarning("WORKAROUND : CHECKING THAT EVENT NAME CONTAINS THE RETURNED NAME")
                    If EventName.Contains(ReturnedEventName) = False Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Verify " + EventName + " On Archive Got : " + ReturnedEventName))
                    End If
                Else
                    _Utils.LogCommentInfo("Selected Event From Archive Successfuly.")
                End If
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Select " + EventName + " On Archive"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Pressing Select On Archive
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub PressSelect()
        _Utils.StartHideFailures("Pressing SELECT On Archive")

        Try
            _Utils.SendIR("SELECT")

            If _Utils.VerifyState("ACTION BAR") Then
                _Utils.LogCommentInfo("Selected Event From Archive Successfuly.")
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify SELECT Was Pressed On Archive"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checks If The EPG Is On Archive Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isArchive() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Archive Is On The Screen")

        Try
            If _Utils.VerifyState("MY RECORDINGS", 2) Or _Utils.VerifyState("LIBRARY ERROR", 2) Then
                Msg = "Archive Is On Screen"
                Return True
            Else
                Msg = "Archive Is Not On Screen"
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
    Public Overrides Sub VerifyArchive()
        _Utils.StartHideFailures("Verifying Archive State Arrived")

        Try
            Dim EventName As String = ""

            If Not _Utils.VerifyState("MY RECORDINGS", 15) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Archive State"))
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
    '''   Checks If Archive Has No Events
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Archive Is Empty")

        Try
            Dim EventName As String = ""

            Try
			
			    _iex.Wait(4)
                _Utils.GetEpgInfo("displayTitle", EventName)				
                Msg = "Archive Is Not Empty"
                Return False
            Catch ex As EAException
                Msg = "Archive Is Empty !!!"
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
    '''   Checks That Archive Is Empty
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyArchiveEmpty()
        Dim res As IEXGateway._IEXResult

        _uUI.Utils.StartHideFailures("Checking If Archive Is Empty")

        Try
            If _Utils.VerifyState("LIBRARY ERROR", 2) Then
                Exit Sub
            End If

            If _Utils.VerifyState("MY LIBRARY", 2) Then
                res = _iex.MilestonesEPG.Navigate("MY RECORDINGS")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If

                If _Utils.VerifyState("LIBRARY ERROR", 2) Then
                    Exit Sub
                End If
            End If

            Dim EventName As String = ""

            Try
			    _iex.Wait(4)
                _uUI.Utils.GetEpgInfo("displayTitle", EventName)
            Catch ex As EAException
                Exit Sub
            End Try

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Archive Is Empty Got Event Name : " + EventName.ToString))

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

        _uUI.Utils.StartHideFailures("Getting Adult Event Name")

        Try
            EventName = ""

            _uUI.PlannerBase.ResolveAdultEventName()

            If Not isArchive() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Verify Archive Is On Screen"))
            End If

            GetSelectedEventName(EventName)

            If EventName <> "" Then
                Msg = "Got -> " + EventName.ToString
                _iex.Wait(1)
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Get Adult Event Name From Archive"))

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try


    End Sub

    ''' <summary>
    '''   Gets The Selected Event Time From Archive
    ''' </summary>
    ''' <param name="EventTime">Returns The Selected Event Time</param>
    ''' <remarks></remarks>
    Public Overrides Sub GetSelectedEventTime(ByRef EventTime As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Get Selected Event Time From Archive")

        Try
            _Utils.GetEpgInfo("evttime", EventTime)
            Msg = "Got -> " + EventTime.ToString
            _iex.Wait(1)
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Finding Requested Event On Archive
    ''' </summary>
    ''' <param name="EventName">The Name Of The Event</param>
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
        Dim ArchiveEventDate As String = ""
        Dim ArchiveEventTime As String = ""
        Dim ArchiveStartTime As String = ""
        Dim ArchiveEndTime As String = ""
        Dim FirstLoop As Boolean = True
        Dim VerifyByDetails As Boolean = False

        _uUI.Utils.StartHideFailures("Finding Event : " + EventName + If(String.IsNullOrEmpty(EventDate), " EventDate :" + EventDate, "") + If(String.IsNullOrEmpty(StartTime), " StartTime :" + StartTime, "") + If(String.IsNullOrEmpty(StartTime), " EndTime :" + EndTime, "") + " On Archive")

        Try

            EpgText = _uUI.Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT")

            _uUI.Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")

            Do
                GetSelectedEventName(CheckedEvent)

                'In case of ADULT content
                If CheckedEvent = EpgText Then
                    GetAdultEventName(CheckedEvent)
                End If

                If EventName.Contains(CheckedEvent) And (CheckedEvent <> "") And (EventName <> "") Then

                    If Not String.IsNullOrEmpty(EventDate) Then
                        GetSelectedEventDetails(ArchiveEventDate)
                        VerifyByDetails = True
                    End If

                    If Not String.IsNullOrEmpty(StartTime) OrElse Not String.IsNullOrEmpty(EndTime) Then
                        GetSelectedEventTime(ArchiveEventTime)
                        VerifyByDetails = True
                    End If

                    If Not String.IsNullOrEmpty(StartTime) Then
                        _uUI.Utils.ParseEventTime(ReturnedTime:=ArchiveStartTime, TimeString:=ArchiveEventTime, IsStartTime:=True)
                    End If
                    If Not String.IsNullOrEmpty(EndTime) Then
                        _uUI.Utils.ParseEventTime(ReturnedTime:=ArchiveEndTime, TimeString:=ArchiveEventTime, IsStartTime:=False)
                    End If

                    If ArchiveEventDate.Contains(EventDate.ToUpper) AndAlso StartTime = ArchiveStartTime AndAlso EndTime = ArchiveEndTime Then
                        _iex.ForceHideFailure()
                        Exit Sub
                    End If
                End If

                NextEvent(times:=1, VerifyByDetails:=VerifyByDetails)

                If FirstLoop Then
                    FirstLoop = False
                    FirstEvent = CheckedEvent
                    CheckedEventDate = ArchiveEventDate
                    CheckedEvent = ""
                    ArchiveEventDate = ""
                End If

            Loop Until FirstEvent = CheckedEvent AndAlso ArchiveEventDate = CheckedEventDate

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Find Event : " + EventName))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Gets The Selected Event Name From Archive
    ''' </summary>
    ''' <param name="EventName">Returns The Selected Event Name</param>
    ''' <remarks></remarks>
    Public Overrides Sub GetSelectedEventName(ByRef EventName As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Get Selected EventName From Archive")

        Try
		    _iex.Wait(4)
            _Utils.GetEpgInfo("displayTitle", EventName)
            Msg = "Got -> " + EventName.ToString
            _iex.Wait(1)
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
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

        _Utils.StartHideFailures("Get Selected Event Details From Archive")

        Try
            _Utils.GetEpgInfo("evtdetails", EventDate)
            Msg = "Selected Event Details : " + EventDate

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
                _iex.Wait(1)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Deleting Event From Archive
    ''' </summary>
    ''' <param name="InReviewBuffer">Optional Parameter Default = False : If True Search Different Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub DeleteEvent(Optional ByVal InReviewBuffer As Boolean = False)
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Deleting Event From Archive InReviewBuffer=" + InReviewBuffer.ToString)

        Try
            _Utils.EPG_Milestones_SelectMenuItem("DELETE")

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("CONFIRM DELETE") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify Delete State Entered"))
            End If

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            'MILESTONES MESSAGES
            Dim ActualLines As New ArrayList

            If InReviewBuffer Then
                Milestones = _Utils.GetValueFromMilestones("DeleteInReviewBuffer")
            Else
                Milestones = _Utils.GetValueFromMilestones("DeleteNotInReviewBuffer")
            End If

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _Utils.ClearEPGInfo()

            _Utils.BeginWaitForDebugMessages(Milestones, 20)

            _Utils.SendIR("SELECT", 15000)

            Dim State As String = ""

            _Utils.GetActiveState(State)

            If State <> "MY RECORDINGS" And State <> "MY LIBRARY" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify State Is MY RECORDINGS Or MY LIBRARY"))
            End If

            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify DeleteEvent Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Deleting All Events From Archive
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub DeleteAllEvents(ByVal NavigateToArchive As Boolean)

        NavigateBeforeDelete(NavigateToArchive)

        _Utils.StartHideFailures("Deleting All Events From Archive")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("DELETE ALL")

            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.VerifyState("CONFIRM DELETE") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify State Is CONFIRM DELETE"))
            End If

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            Dim ActualLines As New ArrayList

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _Utils.ClearEPGInfo()

            _Utils.SendIR("SELECT", 0)

            VerifyArchive()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigate to archive before deleting
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateBeforeDelete(ByVal NavigateToArchive As Boolean)
        _Utils.StartHideFailures("Navigate to archive before deleting")
        If NavigateToArchive Then
            Navigate()
        End If
        If isArchive() Then
            PressSelect()
        End If
    End Sub

    ''' <summary>
    '''   Lock The Event
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Overrides Sub LockEvent()
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Locking Event From Archive")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("LOCK EVENT")

            _Utils.SendIR("SELECT")

            VerifyArchive()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Unlock The Event
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Overrides Sub UnlockEvent()

        _Utils.StartHideFailures("UnLocking Event From Archive")

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
    '''   Stopping Recording From Archive
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>348 - StopRecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub StopRecordingEvent()
        _Utils.StartHideFailures("Stopping Recording From Archive")

        Try
            _uUI.PlannerBase.SelectEvent()
            _uUI.Banner.StopRecording()
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Moving Up In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextUpEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _Utils.StartHideFailures("Moving Up " + times.ToString + " Times")
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

                _Utils.SendIR("SELECT_UP")

                GetSelectedEventName(NextEventAfterMove)

                If VerifyByDetails Then
                    GetSelectedEventDetails(NextEventDetailsAfterMove)
                End If

                If CurrentEvent = NextEventAfterMove AndAlso CurrentEventDetails = NextEventDetailsAfterMove Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Archive"))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Moving Right In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _Utils.StartHideFailures("Moving Right " + times.ToString + " Times")
        Try
            _Utils.LogCommentInfo("ArchiveRecordings.NextEvent : Moving Right " + times.ToString + " Times")

            For i As Integer = 1 To times
                Dim CurrentEvent As String = ""
                Dim CurrentEventDetails As String = ""
                Dim NextEventAfterMove As String = ""
                Dim NextEventDetailsAfterMove As String = ""

                If VerifyByDetails Then
                    GetSelectedEventDetails(CurrentEventDetails)
                End If

                GetSelectedEventName(CurrentEvent)

                _Utils.SendIR("SELECT_RIGHT")

                GetSelectedEventName(NextEventAfterMove)

                If VerifyByDetails Then
                    GetSelectedEventDetails(NextEventDetailsAfterMove)
                End If

                If CurrentEvent = NextEventAfterMove AndAlso CurrentEventDetails = NextEventDetailsAfterMove Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Archive"))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Moving Left In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub PreviousEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _Utils.StartHideFailures("Moving Left " + times.ToString + " Times")

        Try
            For i As Integer = 1 To times
                Dim CurrentEvent As String = ""
                Dim CurrentEventDetails As String = ""
                Dim NextEventAfterMove As String = ""
                Dim NextEventDetailsAfterMove As String = ""

                If VerifyByDetails Then
                    GetSelectedEventDetails(CurrentEventDetails)
                End If

                GetSelectedEventName(CurrentEvent)

                _Utils.SendIR("SELECT_LEFT")

                GetSelectedEventName(NextEventAfterMove)

                If VerifyByDetails Then
                    GetSelectedEventDetails(NextEventDetailsAfterMove)
                End If

                If CurrentEvent = NextEventAfterMove AndAlso CurrentEventDetails = NextEventDetailsAfterMove Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Previous Event On Archive"))
                End If
            Next

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

        _Utils.StartHideFailures("Moving Down " + times.ToString + " Times")
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

                _Utils.SendIR("SELECT_DOWN")

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

    ''' <summary>
    '''   Playback The Event From Archive
    ''' </summary>
    ''' <param name="FromBeginning">Optional Parameter Default = True : If True Plays From Beginning Else Plays From Last Viewed</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>340 - PlayEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub PlayEvent(Optional ByVal FromBeginning As Boolean = True, Optional ByVal EnterPIN As Boolean = False)
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Playing Event " + IIf(FromBeginning, "From Beginning", "From Last Viewed") + " From Archive")

        Try

            SelectEvent()

            Milestones = _Utils.GetValueFromMilestones("PlayEvent")

            Dim ActualLines As New ArrayList
            Dim returnValue As String = ""

            _Utils.BeginWaitForDebugMessages(Milestones, 90)

            If FromBeginning Then
                Try
                    _Utils.EPG_Milestones_SelectMenuItem("PLAY")
                Catch ex As Exception
                    _Utils.EPG_Milestones_SelectMenuItem("PLAY FROM BEGINNING")
                End Try

                _Utils.SendIR("SELECT", 4000)

                If EnterPIN Then
                    _Utils.EnterPin("")
                End If

            Else
                _Utils.EPG_Milestones_SelectMenuItem("PLAY FROM LAST VIEWED")

                _Utils.SendIR("SELECT", 4000)

                If EnterPIN Then
                    _Utils.EnterPin("")
                End If
            End If
	            
            'Verifying State After Playback
            VerifyStateAfterPlayback()
			
            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PlayEvent Milestones : " + Milestones))
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

        GetEventDetails(FirstEvent)

        PreviousEvent = FirstEvent

        Try
            NextUpEvent(1, True)
        Catch ex As Exception
            Exit Sub
        End Try

        Do
            GetEventDetails(CurrentEvent)

            If Not FirstLoop Then
                FirstEvent = CurrentEvent
                CurrentEvent = New EpgEvent(_uUI.Utils)
            End If

            If Not FirstEvent.Equals(CurrentEvent) Then

                If Not FirstLoop Then
                    CurrentEvent = New EpgEvent(_uUI.Utils)
                    If ByTime Then
                        PreviousEvent.VerifyEventBeforeByDateFirst(FirstEvent)
                    Else
                        PreviousEvent.VerifyEventBeforeByNameFirst(FirstEvent)
                    End If
                    PreviousEvent = FirstEvent
                    FirstLoop = True
                Else
                    'If Previouse Is Older Or A-Z Lesser 
                    If ByTime Then
                        PreviousEvent.VerifyEventBeforeByDateFirst(CurrentEvent)
                    Else
                        PreviousEvent.VerifyEventBeforeByNameFirst(CurrentEvent)
                    End If
                    PreviousEvent = CurrentEvent
                End If

                Try
                    NextUpEvent(1, True)
                Catch ex As Exception
                    Exit Sub
                End Try
            Else
                Try
                    NextEvent(1, True)
                    FirstLoop = True
                Catch ex As Exception
                    Exit Sub
                End Try

                If FirstLoop Then
                    GetEventDetails(FirstEvent)
                    FirstLoop = False
                End If
            End If
        Loop Until FirstEvent.Equals(CurrentEvent)

    End Sub


    ''' <summary>
    ''' For verifying the state after Playback
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyStateAfterPlayback()
        _Utils.StartHideFailures("Verifying State after Playback")

        If Not _Utils.VerifyState("TRICKMODE BAR", 5) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify TRICKMODE BAR State Entered"))
        End If


        If Not _Utils.VerifyState("PLAYBACK", 25) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PLAYBACK State Entered"))
        End If

    End Sub

    ''' <summary>
    ''' Get Event Details from EPG
    ''' </summary>
    ''' <param name="_event"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub GetEventDetails(ByRef _event As EpgEvent)
        Dim EPGDateFormat As String = _uUI.Utils.GetEpgDateFormat()
        GetSelectedEventName(_event.Name)
        GetSelectedEventTime(_event.Time)
        _uUI.Utils.ParseEventTime(_event.StartTime, _event.Time, True)
        _uUI.Utils.ParseEventTime(_event.EndTime, _event.Time, False)
        GetSelectedEventDetails(_event.Details)
        _uUI.Utils.ParseEventDate(_event.EventDate, _event.Details)
        Try
            _event.EventDateAsDate = Date.ParseExact(_event.EventDate, EPGDateFormat, CultureInfo.InvariantCulture)
        Catch
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed to Convert to Date Time format. Check Project.ini for DATE_FORMAT in Section EPG"))
        End Try
    End Sub
End Class
