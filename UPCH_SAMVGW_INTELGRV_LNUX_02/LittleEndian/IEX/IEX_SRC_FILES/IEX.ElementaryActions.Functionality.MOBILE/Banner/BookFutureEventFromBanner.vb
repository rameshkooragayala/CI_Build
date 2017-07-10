Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''    Booking Future Event From Action Bar 
    ''' </summary>
    Public Class BookFutureEventFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private _NumOfPresses As Integer = 0
        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _VerifyBookingInPCAT As Boolean
        Private _EventKeyName As String
        Private _MinTimeBeforeEvStart As Integer
        Private _ReturnToLive As Boolean
        Private _IsConflict As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="NumOfPresses">Optional Parameter Default = -1 : Number Of Left Presses From Current</param>
        ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 1 : Minimum Time Right For The Event To Start ( EXAMPLE : For Guard Time )</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verify Is Booked In PCAT</param>
        ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Return To Live</param>
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
        ''' <para>338 - EventNotExistsFailure</para>
        ''' <para>339 - RecordEventFailure</para>
        ''' <para>344 - ConflictFailure</para>
        ''' <para>347 - SelectEventFailure</para>
        ''' <para>349 - ReturnToLiveFailure</para>
        ''' <para>350 - ParsingFailure </para>
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal NumOfPresses As Integer, ByVal MinTimeBeforeEvStart As Integer, ByVal VerifyBookingInPCAT As Boolean, ByVal ReturnToLive As Boolean, ByVal IsConflict As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _NumOfPresses = NumOfPresses
            Me._EventKeyName = EventKeyName
            Me._VerifyBookingInPCAT = VerifyBookingInPCAT
            Me._MinTimeBeforeEvStart = MinTimeBeforeEvStart
            Me._ReturnToLive = ReturnToLive
            Me._IsConflict = IsConflict
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim TimeLeft As Integer
            Dim ChannelNumber As String = ""
            Dim OriginalDuration As Integer

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If _MinTimeBeforeEvStart < 1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "MinTimeBeforeEvStart Has To Be At Least 1 Min"))
            End If

            Dim EventName As String = ""

            EPG.Banner.Navigate()

            EPG.Banner.GetEventTimeLeft(TimeLeft)

            If TimeLeft <= _MinTimeBeforeEvStart * 60 Then

                EPG.Utils.ReturnToLiveViewing()
             
                If TimeLeft = 0 Then
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvStart.ToString + " Minute Waiting 1 Minute")
                    _iex.Wait(60)
                Else
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + _MinTimeBeforeEvStart.ToString + " Minutes Waiting " + (TimeLeft + 5).ToString + " Seconds")
                    _iex.Wait(TimeLeft + 5)
                End If
            Else
                EPG.Utils.ReturnToLiveViewing()
            End If

            EPG.ChannelBar.Navigate()

            EPG.ChannelBar.GetChannelNumber(ChannelNumber)

            EPG.ChannelBar.PreRecordEvent()

            EPG.Banner.GetEventName(EventName)

            If EventName = "" Or EventName = "null" Then
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use")
            End If

            EPG.Banner.GetEventStartTime(StartTime)

            EPG.Banner.GetEventEndTime(EndTime)

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(StartTime))

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "BannerFuture", StartTime, EndTime, ChannelNumber, OriginalDuration, OriginalDuration, "", "", 0, "")

            If Me._IsConflict Then
                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As Exception
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ConflictFailure, "Failed To Create Conflict By Recording Future Event From Banner"))
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

                'If Me._ReturnToLive Then
                '    EPG.Utils.ReturnToLiveViewing()
                'End If

            End If

        End Sub

    End Class

End Namespace