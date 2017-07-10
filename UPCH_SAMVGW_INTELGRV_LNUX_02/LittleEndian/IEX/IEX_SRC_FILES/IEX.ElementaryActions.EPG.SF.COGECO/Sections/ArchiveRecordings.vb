Imports System.Runtime.InteropServices
Imports FailuresHandler

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.COGECO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

    ''' <summary>
    '''   Navigating To Archive Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _uUI.Utils.StartHideFailures("Navigating To Archive")

        Try
            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _uUI.Utils.ClearEPGInfo()

            _uUI.Utils.EPG_Milestones_Navigate("MY RECORDINGS/BY DATES")

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

        _uUI.Utils.StartHideFailures("Selecting Event " + EventName + " From Archive")

        Try
            'Checking Event Name To Check If We Are Selecting Event From 

            _uUI.Utils.SendIR("SELECT")

            _UI.Utils.StartHideFailures("Checking If PIN Is Requested")
            If _uUI.Utils.VerifyDebugMessage("state", "NewPinState", 5, 1) Then
                _UI.Utils.EnterPin("")
                _uUI.Utils.SendIR("SELECT")
            End If
            _iex.ForceHideFailure()

            If _uUI.Utils.VerifyState("ACTION BAR") Then
                If EventName <> "" Then
                    Dim ReturnedEventName As String = ""

                    _uUI.Utils.GetEpgInfo("evtName", ReturnedEventName)

                    _uUI.Utils.LogCommentWarning("WORKAROUND : CHECKING THAT EVENT NAME CONTAINS THE RETURNED NAME")
                    If EventName.Contains(ReturnedEventName) = False Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Verify " + EventName + " On Archive Got : " + ReturnedEventName))
                    End If
                Else
                    _uUI.Utils.LogCommentInfo("Selected Event From Archive Successfuly.")
                End If
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Select " + EventName + " On Archive"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
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
        Dim ReturnedState As String = ""

        _uUI.Utils.StartHideFailures("Locking Event From Archive")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("LOCK EVENT")

            _uUI.Utils.SendIR("SELECT")

            _uUI.Utils.GetEpgInfo("state", ReturnedState)

            If ReturnedState.Contains("PinState") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify PIN State Entered"))
            End If

            _uUI.Utils.EnterPin("")

            VerifyArchive()

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
        Dim returnedValue As String = ""
        Dim res As IEXGateway._IEXResult

        _uUI.Utils.StartHideFailures("Checking If Archive Is Empty")

        Try
            _uUI.Utils.LogCommentInfo("ArchiveRecordings.isEmpty : Checking If Archive Is Empty")

            If _uUI.Utils.VerifyState("MY LIBRARY", 2) Then
                res = _iex.MilestonesEPG.Navigate("MY RECORDINGS/BY DATES")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            End If

            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtName", EventName)
            Catch ex As Exception
                Return True
            End Try

        Finally
            _iex.ForceHideFailure()
        End Try

        Return False
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
            If _uUI.Utils.VerifyState("LIBRARY ERROR", 2) Then
                Exit Sub
            End If

            If _uUI.Utils.VerifyState("MY LIBRARY", 2) Then
                res = _iex.MilestonesEPG.Navigate("MY RECORDINGS")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If

                If _uUI.Utils.VerifyState("LIBRARY ERROR", 2) Then
                    Exit Sub
                End If
            End If

            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtName", EventName)
            Catch ex As EAException
                Exit Sub
            End Try

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Archive Is Empty Got Event Name : " + EventName.ToString))

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

        _uUI.Utils.StartHideFailures("Get Selected EventName From Archive")

        Try
            _uUI.Utils.GetEpgInfo("evtName", EventName)
            Msg = "Got -> " + EventName.ToString
            _iex.Wait(1)
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
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

        _uUI.Utils.StartHideFailures("Playing Event " + IIf(FromBeginning, "From Beginning", "From Last Viewed") + " From Archive")

        Try
            SelectEvent()

            Milestones = _uUI.Utils.GetValueFromMilestones("PlayEvent")

            Dim ActualLines As New ArrayList
            Dim returnValue As String = ""

            _uUI.Utils.BeginWaitForDebugMessages(Milestones, 90)

            If FromBeginning Then
                Try
                    _uUI.Utils.EPG_Milestones_SelectMenuItem("PLAY")
                Catch ex As Exception
                    _uUI.Utils.EPG_Milestones_SelectMenuItem("PLAY FROM BEGINNING")
                End Try

                _uUI.Utils.SendIR("SELECT", 1000)

                If EnterPIN Then
                    _uUI.Utils.EnterPin("")
                End If

            Else

                _uUI.Utils.EPG_Milestones_SelectMenuItem("PLAY FROM LAST VIEWED")

                _uUI.Utils.SendIR("SELECT", 1000)

                If EnterPIN Then
                    _uUI.Utils.EnterPin(PIN:="")
                End If

            End If
			
            'Verifying State After Playback
            VerifyStateAfterPlayback()

            If Not _uUI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PlayEvent Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Finding Requested Event On Archive
    ''' </summary>
    ''' <param name="EventName">The Name Of The Event</param>
    ''' <param name="EventDate">The Event Converted Date</param>
    ''' <param name="StartTime">The Event Start Time</param>
    ''' <param name="EndTime">The Event End Time</param>
    ''' <remarks></remarks>
    Public Overrides Sub FindEvent(ByVal EventName As String, Optional ByVal EventDate As String = "", Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "")
        Dim Times As Integer = 0
        Dim ItemCounter As Integer = 0
        Dim EpgText As String = ""
        Dim VerifyByDetails As Boolean = False
        Dim evTime As String = ""

        _uUI.Utils.StartHideFailures("Finding Event : " + EventName + If(String.IsNullOrEmpty(EventDate), " EventDate :" + EventDate, "") + If(String.IsNullOrEmpty(StartTime), " StartTime :" + StartTime, "") + If(String.IsNullOrEmpty(StartTime), " EndTime :" + EndTime, "") + " On Archive")

        Dim FirstEvent As New EpgEvent(_uUI.Utils)
        Dim CurrentEvent As New EpgEvent(_uUI.Utils)
        Dim LastEvent As New EpgEvent(_uUI.Utils)

        Try
            EpgText = _uUI.Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT")

            GetSelectedEventName(FirstEvent.Name)

            CurrentEvent.Name = "Empty"
            LastEvent.Name = ""

            'In case of ADULT content
            If FirstEvent.Name = EpgText Then
                GetAdultEventName(FirstEvent.Name)
            End If

            If Not String.IsNullOrEmpty(EventDate) Then
                GetSelectedEventDetails(FirstEvent.EventDate)
                VerifyByDetails = True
            End If

            If Not String.IsNullOrEmpty(StartTime) OrElse Not String.IsNullOrEmpty(EndTime) Then
                GetSelectedEventTime(evTime)
                VerifyByDetails = True
            End If

            If Not String.IsNullOrEmpty(StartTime) Then
                _uUI.Utils.ParseEventTime(ReturnedTime:=FirstEvent.StartTime, TimeString:=evTime, IsStartTime:=True)
            End If
            If Not String.IsNullOrEmpty(EndTime) Then
                _uUI.Utils.ParseEventTime(ReturnedTime:=FirstEvent.EndTime, TimeString:=evTime, IsStartTime:=False)
            End If

            _uUI.Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")

            If EventName.Contains(FirstEvent.Name) Then
                If FirstEvent.EventDate.Contains(EventDate.ToUpper) AndAlso StartTime = FirstEvent.StartTime AndAlso EndTime = FirstEvent.EndTime Then
                    Exit Sub
                End If
            End If

            Times = 0

            Do Until (CurrentEvent.Equals(FirstEvent)) Or (CurrentEvent.Equals(LastEvent)) Or Times = 10

                LastEvent = New EpgEvent(_uUI.Utils, CurrentEvent)

                Try
                    NextEvent(VerifyByDetails:=VerifyByDetails)
                Catch ex As Exception
                    Exit Do
                End Try

                ItemCounter += 1

                GetSelectedEventName(CurrentEvent.Name)

                'In case of ADULT content
                If FirstEvent.Name = EpgText Then
                    GetAdultEventName(CurrentEvent.Name)
                End If

                If Not String.IsNullOrEmpty(EventDate) Then
                    GetSelectedEventDetails(FirstEvent.EventDate)
                End If

                GetSelectedEventTime(evTime)

                If Not String.IsNullOrEmpty(StartTime) Then
                    _uUI.Utils.ParseEventTime(ReturnedTime:=CurrentEvent.StartTime, TimeString:=evTime, IsStartTime:=True)
                End If
                If Not String.IsNullOrEmpty(EndTime) Then
                    _uUI.Utils.ParseEventTime(ReturnedTime:=CurrentEvent.EndTime, TimeString:=evTime, IsStartTime:=False)
                End If

                _uUI.Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")

                If EventName.Contains(CurrentEvent.Name) Then
                    If CurrentEvent.EventDate.Contains(EventDate.ToUpper) AndAlso StartTime = CurrentEvent.StartTime AndAlso EndTime = CurrentEvent.EndTime Then
                        Exit Sub
                    End If
                End If

                Times += 1
            Loop

            Try
                NextUpEvent(VerifyByDetails:=VerifyByDetails)
            Catch ex As EAException
                Try
                    PreviousEvent(VerifyByDetails:=VerifyByDetails)
                Catch ex2 As EAException
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Previous Up Event"))
                End Try

                NextUpEvent(VerifyByDetails:=VerifyByDetails)
            End Try

            FirstEvent = New EpgEvent(_uUI.Utils)
            CurrentEvent = New EpgEvent(_uUI.Utils)

            GetSelectedEventName(FirstEvent.Name)

            CurrentEvent.Name = "Empty"
            LastEvent.Name = ""
            Times = 0

            'In case of ADULT content
            If FirstEvent.Name = EpgText Then
                GetAdultEventName(FirstEvent.Name)
            End If

            If Not String.IsNullOrEmpty(EventDate) Then
                GetSelectedEventDetails(FirstEvent.EventDate)
            End If

            GetSelectedEventTime(evTime)

            If Not String.IsNullOrEmpty(StartTime) Then
                _uUI.Utils.ParseEventTime(ReturnedTime:=FirstEvent.StartTime, TimeString:=evTime, IsStartTime:=True)
            End If
            If Not String.IsNullOrEmpty(EndTime) Then
                _uUI.Utils.ParseEventTime(ReturnedTime:=FirstEvent.EndTime, TimeString:=evTime, IsStartTime:=False)
            End If

            _uUI.Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")

            If EventName.Contains(FirstEvent.Name) Then
                If FirstEvent.EventDate.Contains(EventDate.ToUpper) AndAlso StartTime = FirstEvent.StartTime AndAlso EndTime = FirstEvent.EndTime Then
                    Exit Sub
                End If
            End If

            Do Until (CurrentEvent.Equals(FirstEvent)) Or (CurrentEvent.Equals(LastEvent)) Or Times = 10

                LastEvent = New EpgEvent(_uUI.Utils, CurrentEvent)

                PreviousEvent(VerifyByDetails:=VerifyByDetails)

                GetSelectedEventName(CurrentEvent.Name)

                'In case of ADULT content
                If FirstEvent.Name = EpgText Then
                    GetAdultEventName(CurrentEvent.Name)
                End If

                If Not String.IsNullOrEmpty(EventDate) Then
                    GetSelectedEventDetails(CurrentEvent.EventDate)
                End If

                GetSelectedEventTime(evTime)

                If Not String.IsNullOrEmpty(StartTime) Then
                    _uUI.Utils.ParseEventTime(ReturnedTime:=CurrentEvent.StartTime, TimeString:=evTime, IsStartTime:=True)
                End If
                If Not String.IsNullOrEmpty(EndTime) Then
                    _uUI.Utils.ParseEventTime(ReturnedTime:=CurrentEvent.EndTime, TimeString:=evTime, IsStartTime:=False)
                End If

                _uUI.Utils.LogCommentWarning("Workaround : Checking If EventName Contains Checked Event")

                If EventName.Contains(CurrentEvent.Name) Then
                    If CurrentEvent.EventDate.Contains(EventDate.ToUpper) AndAlso StartTime = CurrentEvent.StartTime AndAlso EndTime = CurrentEvent.EndTime Then
                        Exit Sub
                    End If
                End If

                Times += 1
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Find Event : " + EventName))

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

        Dim Milestones As String = ""
        Dim FirstLoop = True

        NavigateBeforeDelete(NavigateToArchive)

        _uUI.Utils.StartHideFailures("Deleting All Events From Archive")

        Try
            Do
                If Not FirstLoop Then
                    PressSelect()
                End If

                Milestones = _uUI.Utils.GetValueFromMilestones("DeleteNotInReviewBuffer")

                _uUI.Utils.EPG_Milestones_SelectMenuItem("DELETE")

                _uUI.Utils.SendIR("SELECT")

                If Not _uUI.Utils.VerifyState("CONFIRM DELETE") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify Delete State Entered"))
                End If

                _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

                'MILESTONES MESSAGES
                Dim ActualLines As New ArrayList

                _uUI.Utils.BeginWaitForDebugMessages(Milestones, 20)

                _uUI.Utils.SendIR("SELECT", 5000)

                If Not _uUI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify DeleteEvent Milestones : " + Milestones))
                End If

                FirstLoop = False

            Loop While _uUI.Utils.VerifyState("MY RECORDINGS", 5)

            _uUI.Utils.VerifyState("MY LIBRARY")

            _uUI.Utils.EPG_Milestones_Navigate("MY RECORDINGS/BY DATES")

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _uUI.Utils.ClearEPGInfo()

            If Not _uUI.ArchiveRecordings.isEmpty() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Archive is not empty after deleting all events"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class