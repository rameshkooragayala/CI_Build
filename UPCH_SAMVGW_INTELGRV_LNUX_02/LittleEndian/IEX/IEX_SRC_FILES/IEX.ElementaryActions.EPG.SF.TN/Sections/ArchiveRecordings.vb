Imports System.Runtime.InteropServices
Imports FailuresHandler

Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

    ''' <summary>
    '''   Navigating To Archive Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try
            _UI.Utils.StartHideFailures("Navigating To Archive")

            _UI.Utils.ReturnToLiveViewing()
            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")
            _uUI.Utils.ClearEPGInfo()

            If _UI.Menu.IsLibraryNoContent Then
                _uUI.Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY RECORDINGS")
            Else
                _uUI.Utils.EPG_Milestones_Navigate("MY RECORDINGS")
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
        _UI.Utils.StartHideFailures("Stopping Recording From Archive")

        Try
            _uUI.PlannerBase.SelectEvent()
            _uUI.Banner.StopRecording()
            _uUI.Utils.ReturnToLiveViewing()
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

            _uUI.Utils.EPG_Milestones_SelectMenuItem("YES")

            'MILESTONES MESSAGES
            Dim ActualLines As New ArrayList

            If InReviewBuffer Then
                Milestones = _uUI.Utils.GetValueFromMilestones("DeleteInReviewBuffer")
            Else
                Milestones = _uUI.Utils.GetValueFromMilestones("DeleteNotInReviewBuffer")
            End If

            'Clearing EPG Dictionary To Avoid Existing Event Info After Cancel
            _uUI.Utils.ClearEPGInfo()

            _uUI.Utils.BeginWaitForDebugMessages(Milestones, 20)

            _uUI.Utils.SendIR("SELECT", 15000)

            Dim State As String = ""

            _uUI.Utils.GetActiveState(State)

            If State <> "MY RECORDINGS" And State <> "MY LIBRARY" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify State Is MY RECORDINGS Or MY LIBRARY"))
            End If

            If Not _uUI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DeleteEventFailure, "Failed To Verify DeleteEvent Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
	
		
    ''' <summary>
    ''' For verifying the state after playback
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyStateAfterPlayback()
        _UI.Utils.StartHideFailures("Verifying State after Playback")

        If Not _uUI.Utils.VerifyState("LIVE", 5) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify LIVE State Entered"))
        End If

    End Sub
    ''' <summary>
    '''   Verifying Sorting ByA-Z Or ByDate
    ''' </summary>
    ''' <param name="ByTime">If True Verifyies Sorting By Time Else By Name</param>
    ''' <remarks></remarks>
    Public Overrides Sub VerifySorting(ByVal ByTime As Boolean)
        Dim FirstEventInRow As New EPG.EpgEvent(_uUI.Utils)
        Dim PrevEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim CurrentEvent As New EPG.EpgEvent(_uUI.Utils)
        Dim FirstEvent As New EPG.EpgEvent(_uUI.Utils)

        TraverseToFirstEvent()

        GetEventDetails(CurrentEvent)

        FirstEvent = CurrentEvent

        Do
            PrevEvent = CurrentEvent

            Try
                NextEvent(1, True)
            Catch
                Try
                    NextEvent(1, True)
                Catch ex As Exception
                    _uUI.Utils.LogCommentInfo("Verified the Sorting in Event in Archive")
                    Exit Sub
                End Try
            End Try

            GetEventDetails(CurrentEvent)

            If ByTime Then
                PrevEvent.VerifyEventBeforeByDateFirst(CurrentEvent)
            Else
                PrevEvent.VerifyEventBeforeByNameFirst(CurrentEvent)
            End If

        Loop Until FirstEvent.Equals(CurrentEvent)

    End Sub

    ''' <summary>
    ''' Traversing to first event in Archive
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub TraverseToFirstEvent()
        Do
            Try
                PreviousEvent(1, True)
            Catch
                Try
                    PreviousEvent(1, True) 'doing previousEvent 2 time to ensure that its not due to key miss
                Catch
                    _uUI.Utils.LogCommentInfo("Reached First Event")
                    Exit Sub
                End Try
            End Try
        Loop
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

        _uUI.Utils.StartHideFailures("Locking Event From Archive")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("LOCK")

            _uUI.Utils.SendIR("SELECT")

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

        _uUI.Utils.StartHideFailures("UnLocking Event From Archive")

        Try
            _uUI.Utils.EPG_Milestones_SelectMenuItem("UNLOCK")

            Dim ReturnedState As String = ""

            _uUI.Utils.SendIR("SELECT")

            _uUI.Utils.GetEPGInfo("state", ReturnedState)

            If ReturnedState.Contains("PinState") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify PIN State Entered"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class