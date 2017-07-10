Imports FailuresHandler
Imports System.Globalization

Namespace EAImplementation

    ''' <summary>
    '''   Recording Current Event From Action Bar
    ''' </summary>
    Public Class RecordCurrentEventFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _VerifyIsRecordingInPCAT As Boolean
        Private _EventKeyName As String
        Private _MinTimeBeforeEvEnd As Integer
        Private _IsResuming As Boolean
        Private _IsConflict As Boolean
        Private _IsPastEvent As Boolean
        Private _IsSeries As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="MinTimeBeforeEvEnd">Optional Parameter Default = -1 : Minutes Required Until End Of Event</param>
        ''' <param name="IsResuming">Optional Parameter Default = False : If True Checks Resume Recording Milestones</param>
        ''' <param name="VerifyIsRecordingInPCAT">Optional Parameter Default = True : If True Verify Is Recording In PCAT</param>
        ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        ''' <param name="IsPastEvent">Optional Parameter Default = False : If True Verify STB is in RB on Past Event and Milestones on Past Event Recording.</param>
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
        Sub New(ByVal EventKeyName As String, ByVal MinTimeBeforeEvEnd As Integer, ByVal IsResuming As Boolean, ByVal VerifyIsRecordingInPCAT As Boolean, ByVal IsConflict As Boolean, ByVal IsPastEvent As Boolean, ByVal IsSeries As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._VerifyIsRecordingInPCAT = VerifyIsRecordingInPCAT
            Me._EventKeyName = EventKeyName
            Me._MinTimeBeforeEvEnd = MinTimeBeforeEvEnd
            Me._IsResuming = IsResuming
            Me._IsConflict = IsConflict
            Me._IsPastEvent = IsPastEvent
            Me._IsSeries = IsSeries
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim ChannelNumber As String = ""
            Dim TimeLeft As Integer
            Dim res As New IEXGateway.IEXResult
            Dim OriginalDuration As Integer
            Dim Duration As Integer
            Dim EpgTime As Date
            Dim EvtEndTime As Date
            Dim milestoneDateFormat As String = ""
            Dim EventDate As String = ""

            milestoneDateFormat = EPG.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT")
            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            EPG.Banner.Navigate()

            If Me._IsPastEvent Then
                EPG.Banner.GetEpgTime(EpgTime)

                EPG.Banner.GetEventEndTime(EndTime)

                EvtEndTime = Convert.ToDateTime(EndTime)

                If Not (EpgTime > EvtEndTime) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "STB is not playing from Review Buffer"))
                End If

            End If

            If Not Me._IsPastEvent And Me._MinTimeBeforeEvEnd > 0 Then

                EPG.Banner.GetEventTimeLeft(TimeLeft)

                If TimeLeft <= _MinTimeBeforeEvEnd * 60 Then
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvEnd.ToString + " Minutes Waiting " + (TimeLeft + 5).ToString + " Seconds")
                    EPG.Utils.ReturnToLiveViewing()
                    _iex.Wait(TimeLeft + 5)
                    EPG.Banner.RefreshEITOnActionBar()
                    EPG.Banner.Navigate()
                End If
            End If

            EPG.Banner.GetEventName(EventName)

            If EventName = "" Or EventName = "null" Then
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use")
            End If

            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)
            EPG.Live.GetEpgDate(EventDate)
            EventDate = DateTime.ParseExact(EventDate, milestoneDateFormat, CultureInfo.InvariantCulture).ToString(EPG.Utils.GetEpgDateFormat())

            EPG.Banner.GetChannelNumber(ChannelNumber)

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Banner.GetEventTimeLeft(Duration)

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "BannerCurrent", StartTime, EndTime, ChannelNumber, Duration, OriginalDuration, "", "", 0, EventDate, IsSeries:=_IsSeries)

            Try
                EPG.Banner.PreRecordEvent(_IsSeries)
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
                    EPG.Banner.RecordEvent(True, _IsResuming, False, _IsPastEvent, _IsSeries)
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try
                If Me._VerifyIsRecordingInPCAT Then
                    If Me._IsPastEvent Then
                        EPG.Utils.LogCommentInfo("Verifying Past Event : " + EventName + " Is Recorded In PCAT")
                        res = Me._manager.PCAT.VerifyEventStatus(Me._EventKeyName, EnumPCATtables.FromRecordings, "RECORD_STATUS", "PAST_RECORDING", True)
                        If Not res.CommandSucceeded Then
                            EPG.Events.Remove(_EventKeyName)
                            ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                        End If
                    Else
                        res = Me._manager.PCAT.VerifyEventIsRecording(Me._EventKeyName)
                        If Not res.CommandSucceeded Then
                            EPG.Events.Remove(_EventKeyName)
                            ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                        End If
                    End If
                End If
            End If
        End Sub

    End Class

End Namespace