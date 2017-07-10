Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''    Record Manual Recording From Current Event
    ''' </summary>
    Public Class RecordManualFromCurrent
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        Private _EventKeyName As String
        Private _ChannelNumber As String
        Private _DurationInMin As Integer
        Private _Frequency As EnumFrequency
        Private _VerifyBookingInPCAT As Boolean
        Private _IsConflict As Boolean
        Private _NoEIT As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ChannelNumber">Channel Number</param>
        ''' <param name="DurationInMin">Optional Parameter Default = -1 : Duration In Minutes</param>
        ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
        ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        ''' <param name="NoEIT">Optional Parameter Default = False : If True Navigation to Manual Recording Is Different</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>309 - GetEPGTimeFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para>    
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>339 - RecordEventFailure</para> 
        ''' <para>344 - ConflictFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ChannelNumber As String, ByVal DurationInMin As Integer, ByVal Frequency As EnumFrequency, ByVal VerifyBookingInPCAT As Boolean, _
                ByVal IsConflict As Boolean, ByVal NoEIT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)

            Me._EventKeyName = EventKeyName
            Me._ChannelNumber = ChannelNumber
            Me._DurationInMin = DurationInMin
            Me._Frequency = Frequency
            Me._VerifyBookingInPCAT = VerifyBookingInPCAT
            Me._IsConflict = IsConflict
            Me._NoEIT = NoEIT
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult
            Dim EPGDate As String = ""
            Dim EPGTime As String = ""
            Dim EventName As String = ""
            Dim EndTime As String = ""
            Dim ChannelName As String = ""
            Dim NewDuration As Long = 0
            Dim Frequency As String = ""
            Dim Occurences As Integer = -1
            Dim StartTime As String = ""

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If _ChannelNumber <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be Equal Or Less Than 0"))
            End If

            Select Case _Frequency
                Case EnumFrequency.ONE_TIME
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_ONE_TIME")
                    Occurences = 1
                Case EnumFrequency.DAILY
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_DAILY")
                    Occurences = 14
                Case EnumFrequency.WEEKLY
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKLY")
                    Occurences = 2
                Case EnumFrequency.WEEKDAY
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKDAY")
                Case EnumFrequency.WEEKEND
                    Frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKEND")
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "No Valid Frequency Has Been Entered"))
            End Select

            res = Me._manager.TuneToChannel(_ChannelNumber)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            EPG.Utils.VerifyLiveReached()

            EPG.ManualRecording.Navigate(_NoEIT)

            EPG.Menu.GetMenuChannelName(ChannelName)

            EventName = ChannelName

            EPG.Live.GetEPGTime(EPGTime)

            EPG.Live.GetEPGDate(EPGDate)

            Dim ConvertedEpgDate As String = CDate(EPGDate).ToString(EPG.Utils.GetEpgDateFormat())
            Dim dateFormatForEventDic As String = EPG.Utils.GetDateFormatForEventDictionary()
            EPGDate = CDate(EPGDate).ToString(dateFormatForEventDic)

            Dim tmpStartTime As Date = CDate(EPGTime)
            Dim tmpEndTime As Date

            Dim RemainingSecondsTillNextMinute As Integer = 60 - tmpStartTime.Second
            _iex.Wait(RemainingSecondsTillNextMinute) 'Wait Till The Next Minute
            tmpStartTime = tmpStartTime.AddSeconds(RemainingSecondsTillNextMinute)
            StartTime = tmpStartTime.ToString("HH:mm")

            If _DurationInMin > 0 Then
                NewDuration = _DurationInMin
                EPG.ManualRecording.NavigateToEndTime()
                tmpEndTime = CDate(tmpStartTime).AddMinutes(_DurationInMin)
                EPG.ManualRecording.SetEndTime(tmpEndTime.ToString("HHmm"))
            End If

            EPG.ManualRecording.GetEventEndTime(EndTime)

            If _DurationInMin <= 0 Then
                Dim EventDuration As String = ""
                Dim TimeLeft As Integer
                TimeLeft = EPG.Utils._DateTime.Subtract(CDate(EndTime), CDate(EPGTime))
                EventDuration = TimeLeft.ToString
                NewDuration = EventDuration
            End If

            _ChannelNumber = "0"

            EPG.ManualRecording.NavigateToFrequency()

            EPG.ManualRecording.SetFrequency(Frequency)

            Dim TmpFrequency As String = ""

            EPG.ManualRecording.GetFrequency(TmpFrequency)

            If TmpFrequency <> Frequency Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Frequency"))
            End If

            EPG.ManualRecording.NavigateToRecord(True)

            If _IsConflict Then
                EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "ManualCurrent", StartTime, EndTime, _ChannelNumber.ToString, NewDuration, NewDuration, EPGDate, Frequency, Occurences, ConvertedEpgDate)

                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As Exception
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ConflictFailure, "Failed To Create Conflict By Recording Manual Event From Current"))
                End Try

            Else
                EPG.ManualRecording.SaveAndEnd(True)

                Try
                    EPG.ManualRecording.VerifySaveAndEndFinished(True)
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try

                EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, "ManualCurrent", StartTime, EndTime, _ChannelNumber.ToString, NewDuration, NewDuration, EPGDate, Frequency, Occurences, ConvertedEpgDate)

                If _VerifyBookingInPCAT Then
                    res = Me._manager.PCAT.VerifyEventBooked(_EventKeyName)
                    If Not res.CommandSucceeded Then
                        EPG.Events.Remove(_EventKeyName)
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                End If

                EPG.Utils.ReturnToLiveViewing()

            End If

        End Sub

    End Class

End Namespace