Imports System.Runtime.InteropServices
Imports FailuresHandler

Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub
    Public Overrides Sub Navigate()
        Try
            _UI.Utils.StartHideFailures("Navigating To Archive")

            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")
            _uUI.Utils.ClearEPGInfo()
            If _UI.Menu.IsLibraryNoContent Then
                _uUI.Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY RECORDINGS")
            Else
                _uUI.Utils.EPG_Milestones_Navigate("MY RECORDINGS/BY DATE")
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
        Dim returnedValue As String = ""

        _uUI.Utils.StartHideFailures("Checking If Archive Is Empty")
        Try
            _uUI.Utils.LogCommentInfo("ArchiveRecordings.isEmpty : Checking If Archive Is Empty")
            If _uUI.Utils.VerifyState("LIBRARY ERROR", 2) Then
                Return True
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
    '''   Moving Right In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)
        _UI.Utils.StartHideFailures("Moving Right " + times.ToString + " Times")
        Try
            _UI.Utils.LogCommentInfo("ArchiveRecordings.NextEvent : Moving Right " + times.ToString + " Times")
            For i As Integer = 1 To times
                Dim CurrentEvent As String = ""
                Dim CurrentEventDetails As String = ""
                Dim NextEventAfterMove As String = ""
                Dim NextEventDetailsAfterMove As String = ""
                If VerifyByDetails Then
                    GetSelectedEventDetails(CurrentEventDetails)
                End If
                GetSelectedEventName(CurrentEvent)
                _uUI.Utils.SendIR("SELECT_RIGHT")
                _uUI.Utils.VerifyState("ACTION BAR", 2)
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
        _UI.Utils.StartHideFailures("Moving Left " + times.ToString + " Times")
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
                _uUI.Utils.SendIR("SELECT_LEFT")
                _iex.Wait(2)
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
    Public Overrides Sub SelectEvent(Optional ByVal EventName As String = "")
        _uUI.Utils.StartHideFailures("Selecting Event " + EventName + " From Archive")
        Try
            _uUI.Utils.SendIR("SELECT")
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
        _uUI.Utils.StartHideFailures("Deleting Event From Archive InReviewBuffer=" + InReviewBuffer.ToString)
        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("DELETE")
            _uUI.Utils.SendIR("SELECT")
            _iex.Wait(2)
            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")
            _uUI.Utils.SendIR("SELECT")

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

        _uUI.Utils.StartHideFailures("Deleting All Events From Archive")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("DELETE ALL")

            _uUI.Utils.SendIR("SELECT")

            If Not _uUI.Utils.VerifyState("CONFIRM DELETE") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify State Is CONFIRM DELETE"))
            End If

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            Dim ActualLines As New ArrayList

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _uUI.Utils.ClearEPGInfo()

            _uUI.Utils.SendIR("SELECT", 0)

            VerifyArchive()

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
        Dim FirstEventInRow As New EPG.EpgEvent(_uUI.Utils)
        Dim PreviousEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim CurrentEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim FirstEvent As New EPG.EpgEvent(_uUI.Utils)

        GetEventDetails(CurrentEvent)

        FirstEvent = CurrentEvent

        Do
            FirstEventInRow = CurrentEvent

            Do

                PreviousEvent = CurrentEvent

                NextEvent(1, True)

                GetEventDetails(CurrentEvent)

                If ByTime Then
                    PreviousEvent.VerifyEventBeforeByDateFirst(CurrentEvent)
                Else
                    PreviousEvent.VerifyEventBeforeByNameFirst(CurrentEvent)
                End If
            Loop Until CurrentEvent.Equals(FirstEventInRow)

            Try
                NextDownEvent(1, True)
            Catch
                Try
                    NextDownEvent(1, True)
                Catch
                    _uUI.Utils.LogCommentInfo("Verified the Sorting in Archive")
                    Exit Sub
                End Try
            End Try

            GetEventDetails(CurrentEvent)

        Loop Until FirstEvent.Equals(CurrentEvent)

    End Sub
End Class