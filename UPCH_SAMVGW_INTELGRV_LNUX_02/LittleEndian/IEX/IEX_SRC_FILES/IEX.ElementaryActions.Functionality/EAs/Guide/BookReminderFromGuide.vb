Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Adding Reminder From Guide
    ''' </summary>
    Public Class BookReminderFromGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _ChannelNumber As Integer
        Private _VerifyBookingInPCAT As Boolean
        Private _ReturnToLive As Boolean
        Private _NumberOfPresses As Integer
        Private _MinTimeBeforeEvStart As Integer

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ChannelNumber">Channel Of The Event To Be Added As Reminder</param>
        ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Right Presses From Current Event</param>
        ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 2 : Minimum Time Left For The Event To Start ( EXAMPLE : For Guard Time )</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
        ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>323 - VerifyStateFailure</para>    
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>339 - RecordEventFailure</para> 
        ''' <para>344 - ConflictFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 		
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ChannelNumber As String, ByVal NumberOfPresses As Integer, ByVal MinTimeBeforeEvStart As Integer, ByVal VerifyBookingInPCAT As Boolean, ByVal ReturnToLive As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._ChannelNumber = ChannelNumber
            Me._NumberOfPresses = NumberOfPresses
            Me._MinTimeBeforeEvStart = MinTimeBeforeEvStart
            Me._VerifyBookingInPCAT = VerifyBookingInPCAT
            Me._ReturnToLive = ReturnToLive
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway._IEXResult
            Dim EventName As String = ""
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim TimeLeft As String = ""
            Dim OriginalDuration As Integer
            'Dim Duration As Integer

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If _ChannelNumber <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be 0 Or Less"))
            End If

            If _NumberOfPresses < 1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Number Of Presses Must Be 1 Or Larger"))
            End If

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If

            EPG.Guide.NavigateToChannel(Me._ChannelNumber)

            EPG.Guide.NextEvent(Me._NumberOfPresses)

            If _NumberOfPresses = 1 Then

                EPG.Guide.GetEventTimeLeftToStart(TimeLeft)

                If CInt(TimeLeft) <= _MinTimeBeforeEvStart Then
                    EPG.Utils.LogCommentInfo("Not Enough Time To Start Of Event, Moving To Next Event")
                    EPG.Guide.NextEvent(1)
                End If
            End If

            EPG.Guide.SelectEvent()

            EPG.Banner.GetEventName(EventName)

            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Guide.SetEventReminder()

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "", StartTime, EndTime, Me._ChannelNumber.ToString, OriginalDuration, OriginalDuration, "", "", 0, "")

            If Me._VerifyBookingInPCAT Then
                res = Me._manager.PCAT.VerifyEventBooked(Me._EventKeyName)
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

            If Me._ReturnToLive Then
                EPG.Utils.ReturnToLiveViewing()
            End If

        End Sub

    End Class

End Namespace
