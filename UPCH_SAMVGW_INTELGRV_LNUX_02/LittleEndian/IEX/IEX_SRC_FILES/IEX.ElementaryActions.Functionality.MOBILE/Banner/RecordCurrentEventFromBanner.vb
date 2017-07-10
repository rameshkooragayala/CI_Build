Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Recording Current Event From Action Bar
    ''' </summary>
    Public Class RecordCurrentEventFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _VerifyIsRecordingInPCAT As Boolean
        Private _EventKeyName As String
        Private _MinTimeBeforeEvEnd As Integer
        Private _IsResuming As Boolean
        Private _IsConflict As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="MinTimeBeforeEvEnd">Optional Parameter Default = -1 : Minutes Required Until End Of Event</param>
        ''' <param name="IsResuming">Optional Parameter Default = False : If True Checks Resume Recording Milestones</param>
        ''' <param name="VerifyIsRecordingInPCAT">Optional Parameter Default = True : If True Verify Is Recording In PCAT</param>
        ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>323 - VerifyStateFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>337 - ParseEventTimeFailure</para> 
        ''' <para>339 - RecordEventFailure</para> 
        ''' <para>344 - ConflictFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal MinTimeBeforeEvEnd As Integer, ByVal IsResuming As Boolean, ByVal VerifyIsRecordingInPCAT As Boolean, ByVal IsConflict As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._VerifyIsRecordingInPCAT = VerifyIsRecordingInPCAT
            Me._EventKeyName = EventKeyName
            Me._MinTimeBeforeEvEnd = MinTimeBeforeEvEnd
            Me._IsResuming = IsResuming
            Me._IsConflict = IsConflict
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim TimeLeft As Integer
            Dim res As New IEXGateway.IEXResult
            Dim OriginalDuration As Integer
            Dim Duration As Integer

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            EPG.Banner.Navigate()

            If Me._MinTimeBeforeEvEnd > 0 Then

                EPG.Banner.GetEventTimeLeft(TimeLeft)

                If TimeLeft <= _MinTimeBeforeEvEnd * 60 Then
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvEnd.ToString + " Minutes Waiting " + (TimeLeft + 5).ToString + " Seconds")
                    EPG.Utils.ReturnToLiveViewing()
                    _iex.Wait(TimeLeft + 5)
                    EPG.Banner.Navigate()
                End If
            End If

            EPG.Banner.GetEventName(EventName)

            If EventName = "" Or EventName = "null" Then
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use")
            End If

            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Banner.GetEventTimeLeft(Duration)

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "BannerCurrent", StartTime, EndTime, "", Duration, OriginalDuration, "", "", 0, "")

            Try
                EPG.Banner.PreRecordEvent()
            Catch ex As EAException
                EPG.Events.Remove(_EventKeyName)
                ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
            End Try

            If Me._IsConflict Then
                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try
            Else
                Try
                    EPG.Banner.RecordEvent()
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try
                Try
                    EPG.Banner.IsRecording()
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try
                'If Me._VerifyIsRecordingInPCAT Then
                '    res = Me._manager.PCAT.VerifyEventIsRecording(Me._EventKeyName)
                '    If Not res.CommandSucceeded Then
                '        EPG.Events.Remove(_EventKeyName)
                '        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                '    End If
                'End If
            End If
        End Sub

    End Class

End Namespace