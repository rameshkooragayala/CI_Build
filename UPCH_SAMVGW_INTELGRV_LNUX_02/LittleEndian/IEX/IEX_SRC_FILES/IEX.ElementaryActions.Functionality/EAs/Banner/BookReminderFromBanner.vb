Imports IEX.ElementaryActions.EPG
Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''  Adding Reminder From Action Bar
    ''' </summary>
    Public Class BookReminderFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _VerifyBookingInPCAT As Boolean
        Private _EventKeyName As String
        Private _MinTimeBeforeEvStart As Integer

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 2 : Minimum Time Left For The Event To Start ( EXAMPLE : For Guard Time )</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>311 - SetEventReminderFailure</para> 
        ''' <para>328 - INIFailure</para> 		  
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>337 - ParseEventTimeFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal MinTimeBeforeEvStart As Integer, ByVal VerifyBookingInPCAT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._VerifyBookingInPCAT = VerifyBookingInPCAT
            Me._EventKeyName = EventKeyName
            Me._MinTimeBeforeEvStart = MinTimeBeforeEvStart
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim TimeLeft As Integer = 0
            Dim OriginalDuration As Integer
            Dim Duration As Integer

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If _MinTimeBeforeEvStart < 1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "MinTimeBeforeEvStart Has To Be At Least 1 Min"))
            End If

            Dim EventName As String = ""

            EPG.Banner.Navigate()

            EPG.Banner.GetEventTimeLeft(TimeLeft)

            If TimeLeft <= _MinTimeBeforeEvStart * 60 Then

                EPG.Utils.ReturnToLiveViewing()

                If CInt(TimeLeft) = 0 Then
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvStart.ToString + " Minute Waiting 1 Minute")
                    _iex.Wait(60)
                Else
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvStart.ToString + " Minutes Waiting " + TimeLeft.ToString + " second.")
                    _iex.Wait((TimeLeft) + 5)
                End If
            Else
                EPG.Utils.ReturnToLiveViewing()
            End If

            EPG.ChannelBar.NextEvent(True)

            EPG.Banner.GetEventName(EventName)

            If EventName = "" Or EventName = "null" Then
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use")
            End If

            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Banner.GetEventTimeLeft(Duration)

            EPG.Banner.SetReminder()

            If Not EPG.Live.IsLive Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReturnToLiveFailure, "Failed To Verify Is Live After Adding The Reminder"))
            End If

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "", StartTime, EndTime, "", Duration, OriginalDuration, "", "", 0, "") 'Duration (N/R for now) sent with default value = 0

            If Me._VerifyBookingInPCAT Then

                res = Me._manager.PCAT.VerifyEventBooked(_EventKeyName)
                If Not res.CommandSucceeded Then
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If

                res = Me._manager.PCAT.VerifyEventStatus(Me._EventKeyName, EnumPCATtables.FromBookings, "BOOKING_TYPE", "REMINDER", False)
                If Not res.CommandSucceeded Then
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If

            End If
        End Sub
    End Class

End Namespace