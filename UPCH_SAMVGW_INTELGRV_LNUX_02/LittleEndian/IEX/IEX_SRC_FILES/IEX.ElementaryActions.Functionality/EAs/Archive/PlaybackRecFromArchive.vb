Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Playback Event From Archive 
    '''   If SecToPlay=0 And VerifyEOF=False Then Plays The Event And Exit The EA
    '''   If SecToPlay=0 And VerifyEOF=True Then Plays The Event Until End And Verifies EOF Then Exit EA 
    ''' </summary>
    Public Class PlaybackRecFromArchive
        Inherits IEX.ElementaryActions.BaseCommand

        Private _EventKeyName As String
        Private _SecToPlay As Integer = -1
        Private _FromBeginning As Boolean
        Private _VerifyEOF As Boolean
        Private _EnterPIN As Boolean
        Private _StartTime As String
        Private _EndTime As String
        Private _Date As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="SecToPlay">Seconds To Play If 0 And VerifyEOF Is True Playes Until End Else If Verify EOF Is False Exit The EA.If SecToPlay Is Not Equal To 0 Then Plays The Event SecToPlay And StopPlayback</param>
        ''' <param name="FromBeginning">If Already Played To Play From The Beginning Or Last Viewed</param>
        ''' <param name="VerifyEOF">If True Verify Playback Until End</param>
        ''' <param name="EnterPIN">If True Enters PIN When Selecting Event For Playback</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>320 - VerifyEofBofFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>340 - PlayEventFailure</para> 
        ''' <para>343 - StopPlayEventFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal SecToPlay As Integer, ByVal FromBeginning As Boolean, ByVal VerifyEOF As Boolean, ByVal EnterPIN As Boolean, _
                ByVal StartTime As String, ByVal EndTime As String, ByVal EvDate As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _EventKeyName = EventKeyName
            _SecToPlay = SecToPlay
            _FromBeginning = FromBeginning
            _VerifyEOF = VerifyEOF
            _EnterPIN = EnterPIN
            _StartTime = StartTime
            _EndTime = EndTime
            _Date = EvDate
            _manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult = Nothing
            Dim pass As Boolean = False
            Dim EventName As String = ""
            Dim eventDurationInSec As Long = 0
            Dim Duration As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            EPG.Utils.StartHideFailures("Checking If Already On Archive Screen")
            pass = EPG.ArchiveRecordings.isArchive
            _iex.ForceHideFailure()

            If Not pass Then
                EPG.ArchiveRecordings.Navigate()
            End If

            EPG.ArchiveRecordings.FindEvent(EventName, _Date, _StartTime, _EndTime)

            If _FromBeginning Then
                EPG.Events(_EventKeyName).PlayedDuration = 0
            End If

            Dim StartPlayback As DateTime = Now

            If _EnterPIN Then
                EPG.ArchiveRecordings.PlayEvent(_FromBeginning, EnterPIN:=True)
            Else
                EPG.ArchiveRecordings.PlayEvent(_FromBeginning, EnterPIN:=False)
            End If


            Dim NeedToGetDuration As Boolean = False

            If _SecToPlay > 0 Then
                EPG.Utils.StartHideFailures("Checking If Event Is Recording")
                res = Me._manager.PCAT.VerifyEventIsRecording(_EventKeyName)
                If res.CommandSucceeded Then
                    NeedToGetDuration = False
                    If _VerifyEOF Then
                        _iex.ForceHideFailure()
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Can't Verify EOF For On Going Recording"))
                    End If
                Else
                    NeedToGetDuration = True
                End If
                _iex.ForceHideFailure()
            End If

            If _VerifyEOF Then
                NeedToGetDuration = True
            End If

            If NeedToGetDuration Then
                EPG.Utils.LogCommentBlack("Event Source Is : " + EPG.Events(_EventKeyName).Source + " Event Duration Is : " + EPG.Events(_EventKeyName).Duration.ToString)
                If EPG.Events(_EventKeyName).Source.Contains("Manual") Then
                    'Bug In PCAT StartTime So Getting Duration From PCAT Failes Taking Given Duration From User
                    EPG.Utils.LogCommentWarning("WORKAROUND : BUG IN PCAT STARTTIME SO GETTING DURATION FROM PCAT FAILES TAKING GIVEN DURATION FROM USER")
                    eventDurationInSec = EPG.Events(_EventKeyName).Duration
                Else
                    res = Me._manager.PCAT.GetEventDuration(_EventKeyName, EnumPCATtables.FromRecordings, Duration)
                    If Not res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If

                    eventDurationInSec = EPG.Events(_EventKeyName).Duration
                End If
                If (_SecToPlay > 0) And (_SecToPlay > (eventDurationInSec)) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "SecToPlay Asked Is More Than Event Duration " + _SecToPlay.ToString + " > " + (eventDurationInSec).ToString))
                End If
            End If

            If _SecToPlay > 0 Then
                EPG.Events(_EventKeyName).PlayedDuration += _SecToPlay
                EPG.Utils.LogCommentImportant("Waiting " + _SecToPlay.ToString + " Seconds")
                _iex.Wait(_SecToPlay)
                res = _manager.PVR.StopPlayback()
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                Else
                    Exit Sub
                End If
            End If

            If _VerifyEOF Then

                If EPG.Events(_EventKeyName).PlayedDuration >= (EPG.Events(_EventKeyName).Duration) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Event Played Duartion " + EPG.Events(_EventKeyName).PlayedDuration.ToString + " Is Bigger Than Event Duration " + ((EPG.Events(_EventKeyName).Duration)).ToString))
                End If

                EPG.ArchiveRecordings.VerifyPlaybackEnded((eventDurationInSec) - EPG.Events(_EventKeyName).PlayedDuration)

                Dim EndPlayback As DateTime = Now

                EPG.Utils.LogCommentInfo("Playback Verified : Played " + (EPG.Utils._DateTime.SubtractInSec(EndPlayback, StartPlayback).ToString + " Seconds"))

            End If
        End Sub

    End Class
End Namespace
