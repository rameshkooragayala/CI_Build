Imports FailuresHandler
Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

    ''' <summary>
    '''   Checks If Archive Has No Events
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Archive Is Empty")

        Try
            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtName", EventName)
                Msg = "Archive Is Not Empty"
                Return False
            Catch ex As EAException
                Msg = "Archive Is Empty !!!"
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
                res = _iex.MilestonesEPG.NavigateByName("STATE:MY RECORDINGS")
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
    '''   Moving Down In Archive
    ''' </summary>
    ''' <param name="times">Optional Parameter Default = 1 : Number Of Times To Move</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal times As Integer = 1, Optional ByVal VerifyByDetails As Boolean = False)

        _uUI.Utils.StartHideFailures("Moving Right " + times.ToString + " Times")
        Try
            _uUI.Utils.LogCommentInfo("ArchiveRecordings.NextEvent : Moving Right " + times.ToString + " Times")

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
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Archive"))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub



    ''' <summary>
    '''   Navigating To Archive Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try
            _uUI.Utils.StartHideFailures("Navigating To Archive")

            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _uUI.Utils.ClearEPGInfo()

            If _UI.Menu.IsLibraryNoContent Then
                _uUI.Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY RECORDINGS")
            Else
                _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY RECORDINGS")
                _iex.Wait(6)
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

        _UI.Utils.StartHideFailures("Locking Event From Archive")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("LOCK EVENT")

            _UI.Utils.SendIR("SELECT")
            If Not _uUI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify State Is INSERT PIN UNLOCK CHANNEL"))
            End If

            Dim PIN As String = _uUI.Utils.GetValueFromEnvironment("DefaultPIN")

            _UI.Utils.TypeKeys(PIN)
            _iex.Wait(6)
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

                _uUI.ArchiveRecordings.NextEvent(1, True)

                GetEventDetails(CurrentEvent)

                If ByTime Then
                    PreviousEvent.VerifyEventBeforeByDateFirst(CurrentEvent)
                Else
                    PreviousEvent.VerifyEventBeforeByNameFirst(CurrentEvent)
                End If
            Loop Until CurrentEvent.Equals(FirstEventInRow)

            _uUI.ArchiveRecordings.NextDownEvent(1, True)

            GetEventDetails(CurrentEvent)

        Loop Until FirstEvent.Equals(CurrentEvent)

        _uUI.Utils.LogCommentInfo("Verified the sorting in Archive")

    End Sub

    ''' <summary>
    ''' For verifying the state after Playback
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyStateAfterPlayback()
        _uUI.Utils.StartHideFailures("Verifying State after Playback")

        If Not _uUI.Utils.VerifyState("PLAYBACK", 25) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PLAYBACK State Entered"))
        End If

    End Sub
End Class