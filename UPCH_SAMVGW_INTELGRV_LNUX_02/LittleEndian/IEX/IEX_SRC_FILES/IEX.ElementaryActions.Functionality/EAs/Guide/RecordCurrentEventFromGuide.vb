Imports FailuresHandler
Imports System.Globalization

Namespace EAImplementation

    ''' <summary>
    '''   Recording Current Event From Guide
    ''' </summary>
    Public Class RecordCurrentEventFromGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _ChannelNumber As Integer
        Private _MinTimeBeforeEvEnd As Integer
        Private _VerifyBookingInPCAT As Boolean
        Private _ReturnToLive As Boolean
        Private _IsConflict As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
        ''' <param name="MinTimeBeforeEvEnd">Optional Parameter Default = -1 : Minutes Required Until End Of Event</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
        ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
        ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>339 - RecordEventFailure</para> 
        ''' <para>344 - ConflictFailure</para> 	
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ChannelNumber As String, ByVal MinTimeBeforeEvEnd As Integer, ByVal VerifyBookingInPCAT As Boolean, ByVal ReturnToLive As Boolean, ByVal IsConflict As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._ChannelNumber = ChannelNumber
            Me._MinTimeBeforeEvEnd = MinTimeBeforeEvEnd
            Me._VerifyBookingInPCAT = VerifyBookingInPCAT
            Me._ReturnToLive = ReturnToLive
            Me._IsConflict = IsConflict
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway._IEXResult
            Dim EventName As String = ""
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim OriginalDuration As Integer
            Dim Duration As Integer
            Dim milestoneDateFormat As String = ""
            Dim EventDate As String = ""
            Dim TimeLeft As Integer

            milestoneDateFormat = EPG.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT")
            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If _ChannelNumber <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be Equal Or Less Than 0"))
            End If

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If
			
			EPG.Guide.NavigateToChannel(Me._ChannelNumber, VerifyFas:=False)

            If Me._MinTimeBeforeEvEnd > 0 Then

                EPG.Banner.GetEventTimeLeft(TimeLeft)

                If TimeLeft <= _MinTimeBeforeEvEnd * 60 Then
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvEnd.ToString + " Minutes Waiting " + (TimeLeft + 5).ToString + " Seconds")
                    EPG.Utils.ReturnToLiveViewing()
                    _iex.Wait(TimeLeft + 5)
                End If
            End If

            EPG.Guide.Navigate()

            EPG.Guide.NavigateToRecordEvent(True)

            EPG.Banner.GetEventName(EventName)

            If EventName = "" Or EventName = "null" Then
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use")
            End If

            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)
            EPG.Live.GetEpgDate(EventDate)
            EventDate = DateTime.ParseExact(EventDate, milestoneDateFormat, CultureInfo.InvariantCulture).ToString(EPG.Utils.GetEpgDateFormat())

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Banner.GetEventTimeLeft(Duration)

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "GuideCurrent", StartTime, EndTime, Me._ChannelNumber.ToString, Duration, OriginalDuration, "", "", 0, EventDate)

            If _IsConflict Then
                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As Exception
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ConflictFailure, "Failed To Create Conflict By Recording Future Event From Guide"))
                End Try
            Else
                Try
                    EPG.Guide.RecordEvent(True, False)
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try

                If Me._VerifyBookingInPCAT Then
                    res = Me._manager.PCAT.VerifyEventBooked(Me._EventKeyName)
                    If Not res.CommandSucceeded Then
                        EPG.Events.Remove(_EventKeyName)
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                End If

                If Me._ReturnToLive Then
                    EPG.Utils.ReturnToLiveViewing()
                End If

            End If
        End Sub

    End Class

End Namespace