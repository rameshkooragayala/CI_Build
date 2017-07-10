Imports System.Runtime.InteropServices
Imports FailuresHandler

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.UPC.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

    ''' <summary>
    '''   Moving Down In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _uUI.Utils.StartHideFailures("Moving Down " + times.ToString + " Times")
        Try
            _uUI.Utils.LogCommentInfo("ArchiveRecordings.NextEvent : Moving Down " + times.ToString + " Times")

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
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Archive"))
                End If
            Next
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

            If Not _uUI.Utils.VerifyState("CLEAR HARD DISK") Then
                If Not _uUI.Utils.VerifyState("DELETE ALL RECORDS") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify State Is CLEAR HARD DISK"))
                End If
            End If

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            Dim ActualLines As New ArrayList

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _uUI.Utils.ClearEPGInfo()

            _uUI.Utils.SendIR("SELECT", 0)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigate to Delete All
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateBeforeDelete(ByVal NavigateToArchive As Boolean)
        'Navigate to Settings
        _uUI.Utils.StartHideFailures("Navigate to archive before deleting")
        _uUI.Utils.EPG_Milestones_NavigateByName("STATE:HORIZON BOX")
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

        GetEventDetails(CurrentEvent)

        FirstEvent = CurrentEvent

        PreviousEvent = CurrentEvent

        NextEvent(1, True)

        GetEventDetails(CurrentEvent)

        While Not FirstEvent.Equals(CurrentEvent)

            If ByTime Then
                PreviousEvent.VerifyEventBeforeByDateFirst(CurrentEvent)
            Else
                PreviousEvent.VerifyEventBeforeByNameFirst(CurrentEvent)
            End If

            PreviousEvent = CurrentEvent

            NextEvent(1, True)

            GetEventDetails(CurrentEvent)

        End While

        _uUI.Utils.LogCommentInfo("Verified the sorting")

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

                If Not String.IsNullOrEmpty(EventDate) Then
                    GetSelectedEventDetails(ArchiveEventDate)
                    VerifyByDetails = True
                End If

                If Not String.IsNullOrEmpty(StartTime) OrElse Not String.IsNullOrEmpty(EndTime) Then
                    _uUI.Utils.LogCommentWarning("As we are not having End Time in My Recordings We are checking the Recordings Based on Start Time only")
                    _uUI.Utils.GetSelectedEventStartTime(ArchiveStartTime)
                    VerifyByDetails = True
                End If

                If EventName.Contains(CheckedEvent) And (CheckedEvent <> "") And (EventName <> "") Then
                    _uUI.Utils.LogCommentWarning("We are not verifying the End time as it is not displayed in EPG")
                    If ArchiveEventDate.ToUpper.Contains(EventDate.ToUpper) AndAlso StartTime = ArchiveStartTime Then
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
End Class