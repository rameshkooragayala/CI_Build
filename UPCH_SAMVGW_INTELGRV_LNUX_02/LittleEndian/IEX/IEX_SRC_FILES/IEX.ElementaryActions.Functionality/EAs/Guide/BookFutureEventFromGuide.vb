Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Booking Future Event From Guide
    ''' </summary>
    Public Class BookFutureEventFromGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _ChannelNumber As Integer
        Private _VerifyBookingInPCAT As Boolean
        Private _ReturnToLive As Boolean
        Private _NumberOfPresses As Integer
        Private _MinTimeBeforeEvStart As Integer
        Private _IsConflict As Boolean
        Private _StartTime As String
        Private _EndTime As String
        Private _MinEventLength As Integer
        Private _MaxEventLength As Integer
        Private _DaysDelay As Integer


        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
        ''' <param name="StartTime">Requested Exact Event Start Time</param>
        ''' <param name="EndTime">Requested Exact Event Start Time</param>
        ''' <param name="MinEventLength">Minimum Event Length</param>
        ''' <param name="MaxEventLength">Max Event Length</param>
        ''' <param name="DaysDelay">Days Delay On Guide - Not Yet Supported</param>
        ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Right Presses From Current Event</param>
        ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 1 : Minimum Time Right For The Event To Start ( EXAMPLE : For Guard Time )</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
        ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
        ''' <param name="IsConflict">Optional Parameter Default = True : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para>    
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para>   
        ''' <para>323 - VerifyStateFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>339 - RecordEventFailure</para> 
        ''' <para>344 - ConflictFailure</para> 
        ''' <para>346 - FindEventFailure</para>
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para>   
        ''' <para>355 - TuneToChannelFailure</para>  
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ChannelNumber As String, ByVal StartTime As String, ByVal EndTime As String, ByVal MinEventLength As Integer, _
                MaxEventLength As Integer, ByVal DaysDelay As Integer, ByVal NumberOfPresses As Integer, ByVal MinTimeBeforeEvStart As Integer, ByVal VerifyBookingInPCAT As Boolean, ByVal ReturnToLive As Boolean, ByVal IsConflict As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._ChannelNumber = ChannelNumber
            Me._StartTime = StartTime
            Me._EndTime = EndTime
            Me._MinEventLength = MinEventLength
            Me._MaxEventLength = MaxEventLength
            Me._DaysDelay = DaysDelay
            Me._NumberOfPresses = NumberOfPresses
            Me._MinTimeBeforeEvStart = MinTimeBeforeEvStart
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
            Dim EventDate As String = ""
            Dim TimeLeft As String = ""
            Dim sTime As Date
            Dim eTime As Date
            Dim OriginalDuration As Integer
            'Dim Duration As Integer

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If _NumberOfPresses < 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Number Of Presses Must Be 1 Or Larger"))
            End If

            Try
                If CInt(_ChannelNumber) < 0 Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Number Must Be 1 Or Larger"))
                End If
            Catch ex As Exception
                If _ChannelNumber <> "" Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Number Can Contain Only Numbers"))
                End If
            End Try
          
            If _StartTime <> "" AndAlso _StartTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Start Time Should Be Entered With Format : HH:MM"))
            End If

            If _EndTime <> "" AndAlso _EndTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "End Time Should Be Entered With Format : HH:MM"))
            End If

            If _MinEventLength > _MaxEventLength And _MaxEventLength > -1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "MinEventLength Can't Be Larger Than MaxEventLength"))
            End If

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If

            EPG.Guide.NavigateToChannel(Me._ChannelNumber, VerifyFas:=False)

            If String.IsNullOrEmpty(_StartTime) OrElse _MinTimeBeforeEvStart > 0 Then

                EPG.Utils.LogCommentInfo("Booking The " + _NumberOfPresses.ToString + " Event On Guide")

                EPG.Guide.NextEvent(Me._NumberOfPresses)

                If _NumberOfPresses = 1 OrElse _MinTimeBeforeEvStart > 0 Then
                    Dim Loops As Integer = 0

                    Do
                        EPG.Guide.GetEventTimeLeftToStart(TimeLeft)

                        If CInt(TimeLeft) <= _MinTimeBeforeEvStart Then
                            EPG.Utils.LogCommentInfo("Not Enough Time To Start Of Event, Moving To Next Event")
                            EPG.Guide.NextEvent(1)
                        End If
                        Loops += 1
                    Loop Until (CInt(TimeLeft) >= _MinTimeBeforeEvStart) OrElse Loops = 5
                End If

            Else
                EPG.Utils.LogCommentInfo("Searching For Event With Start Time " + _StartTime + " On Guide")
                EPG.Guide.FindEventByStartTime(_StartTime, DaysDelay:=_DaysDelay)
            End If


            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)

            EPG.Guide.GetEventDate(EventDate)

            If Not String.IsNullOrEmpty(_EndTime) Then

                EPG.Utils.LogCommentInfo("Checking If Event End Time Is : " + _EndTime)
                If Not (EndTime = Me._EndTime) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Book Event With StartTime " + _StartTime + " And End Time " + _EndTime))
                End If
            Else
                If _MinEventLength > 0 OrElse _MaxEventLength > -1 Then

                    Dim message As String = "Checking If Event Length Is More Than " + _MinEventLength.ToString + " Minutes"
                    If _MaxEventLength > 0 Then
                        message = message + " And Less Than " + _MaxEventLength.ToString + " Minutes"
                    End If
                    EPG.Utils.LogCommentInfo(message)
                    If Not DateTime.TryParse(StartTime, sTime) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Parse Requested Start Time " + StartTime))
                    End If

                    If Not DateTime.TryParse(EndTime, eTime) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Parse Found Event Time " + EndTime))
                    End If

                    Dim Minutes As Integer = eTime.Subtract(sTime).TotalMinutes

                    EPG.Utils.LogCommentInfo("Event Length is " + Minutes.ToString + " Minutes")
                    If Minutes < _MinEventLength Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Book Event With StartTime " + _StartTime + " And Length Larger Than " + _MinEventLength.ToString))
                    End If

                    If Minutes > _MaxEventLength Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Book Event With StartTime " + _StartTime + " And Length Smaller Than " + _MaxEventLength.ToString()))
                    End If
                End If
            End If

            EPG.Guide.NavigateToRecordEvent(False)

            EPG.Banner.GetEventName(EventName)

            If EventName = "" Or EventName = "null" Then
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use")
            End If

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "GuideFuture", StartTime, EndTime, Me._ChannelNumber.ToString, OriginalDuration, OriginalDuration, "", "", 0, EventDate)

            'If Me._VerifyBookingInPCAT Then
            '    EPG.Utils.StartHideFailures("Checking If Event Already Booked In PCAT")
            '    res = Me._manager.PCAT.VerifyEventBooked(Me._EventKeyName)
            '    If res.CommandSucceeded Then
            '        EPG.Utils.LogCommentInfo("Found Event Already Booked In PCAT Exiting BookFutureEventFromGuide")
            '        If Me._ReturnToLive Then
            '            EPG.Utils.ReturnToLiveViewing()
            '        End If
            '        _iex.ForceHideFailure()
            '        Exit Sub
            '    End If
            '    _iex.ForceHideFailure()
            'End If

            If _IsConflict Then
                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try
            Else
                Try
                    EPG.Guide.RecordEvent(False, False)
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
                    res = Me._manager.PCAT.VerifyEventStatus(Me._EventKeyName, EnumPCATtables.FromBookings, "BOOKING_TYPE", "RECORD", False)
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
