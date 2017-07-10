Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Record Manual Recording From Planner
    ''' </summary>
    Public Class RecordManualFromPlanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        Private _EventKeyName As String
        Private _ChannelName As String
        Private _ChannelNumber As Integer
        Private _DaysDelay As Integer
        Private _MinutesDelayUntilBegining As Integer
        Private _DurationInMin As Integer
        Private _Frequency As EnumFrequency
        Private _VerifyBookingInPCAT As Boolean
        Private _IsConflict As Boolean
        Private _StartTime As String

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ChannelName">Channel Name</param>
        ''' <param name="ChannelNumber">Channel Number If Entered Doing DCA</param>
        ''' <param name="DaysDelay">Optional Parameter Default = -1 : Adds Days From Current Time</param>
        ''' <param name="MinutesDelayUntilBegining">Optional Parameter Default = -1 : Minutes Delay Until Beginning</param>
        ''' <param name="DurationInMin">Optional Parameter Default = 1 : Duration Of Recording</param>
        ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
        ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
        ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEPGInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>309 - GetEPGTimeFailure</para>    
        ''' <para>310 - GetEPGDateFailure</para>    
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
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ChannelName As String, ByVal ChannelNumber As Integer, ByVal DaysDelay As Integer, ByVal MinutesDelayUntilBegining As Integer, _
                ByVal StartTime As String, ByVal DurationInMin As Integer, ByVal Frequency As EnumFrequency, ByVal VerifyBookingInPCAT As Boolean, _
                ByVal IsConflict As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)

            Me._EventKeyName = EventKeyName
            Me._ChannelName = ChannelName
            Me._ChannelNumber = ChannelNumber
            Me._DaysDelay = DaysDelay
            Me._MinutesDelayUntilBegining = MinutesDelayUntilBegining
            Me._DurationInMin = DurationInMin
            Me._Frequency = Frequency
            Me._VerifyBookingInPCAT = VerifyBookingInPCAT
            Me._IsConflict = IsConflict
            Me._StartTime = StartTime

            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            If _manager.Project.IsEPGLikeCogeco Then
                RecordManualFromPlannerCOGECO()
            Else
                RecordManualFromPlannerUPC()
            End If

        End Sub

        Private Function RecordManualFromPlannerCOGECO() As Boolean
            Dim res As IEXGateway.IEXResult
            Dim EPGDate As String = ""
            Dim EPGTime As String = ""
            Dim EventName As String = ""
            Dim MyStartTime As String = ""
            Dim EndTime As String = ""
            Dim Current As Boolean = True
            Dim EventChannel As String = ""
            Dim EventDate As String = ""
            Dim EventConvertedDate As String = ""
            Dim EventSource As String = ""
            Dim Frequency As String = ""
            Dim Occurences As Integer = -1

            If _EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If Me._ChannelName = "" AndAlso Me._ChannelNumber = -1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Argument Can't Be Empty"))
            End If

            If _DurationInMin <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "DurationInMin Can't Be Equal Or Less Than 0"))
            End If

            If _StartTime <> "" And _DaysDelay < 1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "DaysDelay Can't Be Less Then 1 In Case StartTime Entered"))
            End If

            Current = False

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

            EventName = Me._ChannelName

            EventChannel = "0"

            EPG.Banner.Navigate()

            EPG.Live.GetEPGTime(EPGTime)

            EPG.Live.GetEPGDate(EPGDate)

            EPG.ManualRecording.NavigateFromPlanner()

            Dim StartTimeWithdelay As New Date
            Dim tmpStartTime As Date
            Dim tmpEndTime As Date
            Dim TimeArray As String() = EPGTime.Split(":")
            Dim hours As Integer = CInt(TimeArray(0))
            Dim minutes As Integer = CInt(TimeArray(1))

            If _DaysDelay < 0 Then
                _DaysDelay = 0
            Else
                Occurences = Occurences - _DaysDelay
            End If

            If _StartTime = "" Then
                MyStartTime = EPGTime
                tmpStartTime = CDate(MyStartTime).AddMinutes(_MinutesDelayUntilBegining)
                MyStartTime = tmpStartTime.ToString("HH:mm")

                'Set recording start time
                If _MinutesDelayUntilBegining > ((24 * 60) - ((hours * 60) + minutes)) Then
                    If _DaysDelay = -1 Then
                        _DaysDelay = _DaysDelay + 2
                    Else
                        _DaysDelay = _DaysDelay + 1
                    End If
                End If
            Else
                tmpStartTime = CDate(_StartTime)
                MyStartTime = _StartTime
            End If

            Dim ConvertedEpgDate As String = CDate(EPGDate).AddDays(_DaysDelay).ToString(EPG.Utils.GetEpgDateFormat())
            Dim dateFormatForEventDic As String = EPG.Utils.GetDateFormatForEventDictionary()
            EPGDate = CDate(EPGDate).AddDays(_DaysDelay).ToString(dateFormatForEventDic)

            EPG.ManualRecording.SetDate(ConvertedEpgDate, False)

            EventConvertedDate = ConvertedEpgDate

            EventDate = CDate(EPGDate).ToString(dateFormatForEventDic)

            EPG.ManualRecording.SetHour(tmpStartTime.Hour)

            EPG.ManualRecording.SetMinutes(tmpStartTime.Minute)

            EPG.ManualRecording.VerifyStateId("52")

            tmpEndTime = CDate(tmpStartTime).AddMinutes(_DurationInMin)
            EndTime = tmpEndTime.ToString("HH:mm")

            EPG.ManualRecording.SetHour(tmpEndTime.Hour)

            EPG.ManualRecording.SetMinutes(tmpEndTime.Minute)

            EPG.ManualRecording.VerifyStateId("53")

            EPG.ManualRecording.SetFrequency(Frequency)

            EPG.ManualRecording.VerifyStateId("54")

            If Me._ChannelName <> "" Then
                EPG.ManualRecording.SetChannel(Me._ChannelName)
            Else
                EventName = EPG.ManualRecording.SetChannel(Me._ChannelNumber)
            End If

            EPG.ManualRecording.VerifyStateId("55")

            EventSource = "ManualFuture"

            If _IsConflict Then

                EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, EventSource, MyStartTime, EndTime, EventChannel, _DurationInMin, _DurationInMin, EventDate, Frequency, Occurences, EventConvertedDate)

                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As Exception
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ConflictFailure, "Failed To Create Conflict By Recording Event From Planner"))
                End Try
            Else
                EPG.ManualRecording.SaveAndEnd(Current)

                Try
                    EPG.ManualRecording.VerifySaveAndEndFinished(False)
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try

                EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, EventSource, MyStartTime, EndTime, EventChannel, _DurationInMin, _DurationInMin, EventDate, Frequency, Occurences, EventConvertedDate)

                If _VerifyBookingInPCAT Then
                    res = Me._manager.PCAT.VerifyEventBooked(_EventKeyName)
                    If Not res.CommandSucceeded Then
                        EPG.Events.Remove(_EventKeyName)
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                End If

                EPG.Utils.ReturnToLiveViewing()
            End If

            Return True

        End Function

        Private Function RecordManualFromPlannerUPC() As Boolean
            Dim res As IEXGateway.IEXResult
            Dim EPGDate As String = ""
            Dim EPGTime As String = ""
            Dim EventName As String = ""
            Dim MyStartTime As String = ""
            Dim EndTime As String = ""
            Dim Current As Boolean = True
            Dim EventChannel As String = ""
            Dim EventDate As String = ""
            Dim EventConvertedDate As String = ""
            Dim EventSource As String = ""
            Dim Frequency As String = ""
            Dim Occurences As Integer = -1

            Current = False

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

            If Me._ChannelName = "" AndAlso Me._ChannelNumber = -1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Argument Can't Be Empty"))
            End If

            If _DurationInMin <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "DurationInMin Can't Be Equal Or Less Than 0"))
            End If

            If _MinutesDelayUntilBegining = -1 Then
                _MinutesDelayUntilBegining = 1
            Else
                _MinutesDelayUntilBegining += 1
            End If

            EventName = _ChannelName
            EventChannel = "0"

            EPG.Banner.Navigate()

            EPG.Live.GetEPGTime(EPGTime)

            EPG.Live.GetEPGDate(EPGDate)

            EPG.ManualRecording.NavigateFromPlanner()

            Dim StartTimeWithdelay As New Date
            Dim tmpStartTime As Date
            Dim tmpEndTime As Date
            Dim TimeArray As String() = EPGTime.Split(":")
            Dim hours As Integer = CInt(TimeArray(0))
            Dim minutes As Integer = CInt(TimeArray(1))

            If _DaysDelay < 0 Then
                _DaysDelay = 0
            Else
                Occurences = Occurences - _DaysDelay
            End If

            If _StartTime = "" Then
                MyStartTime = EPGTime
                tmpStartTime = CDate(MyStartTime).AddMinutes(_MinutesDelayUntilBegining)
                MyStartTime = tmpStartTime.ToString("HH:mm")

                'Set recording start time
                If _MinutesDelayUntilBegining > ((24 * 60) - ((hours * 60) + minutes)) Then
                    If _DaysDelay = -1 Then
                        _DaysDelay = _DaysDelay + 2
                    Else
                        _DaysDelay = _DaysDelay + 1
                    End If
                End If
            Else
                tmpStartTime = CDate(_StartTime)
                MyStartTime = _StartTime
            End If

            EPG.ManualRecording.NavigateToDate()

            Dim ConvertedEpgDate As String = ""
            Dim dateFormatForEventDic As String = EPG.Utils.GetDateFormatForEventDictionary()

            If CDate(EPGDate).ToString(dateFormatForEventDic) = CDate(EPGDate).AddDays(_DaysDelay).ToString(dateFormatForEventDic) Then
                EPG.ManualRecording.SetDate("", True)
            Else
                ConvertedEpgDate = CDate(EPGDate).AddDays(_DaysDelay).ToString(EPG.Utils.GetEpgDateFormat())
                EPG.ManualRecording.SetDate(ConvertedEpgDate, False)
            End If

            EventDate = CDate(EPGDate).AddDays(_DaysDelay).ToString(dateFormatForEventDic)

            EventConvertedDate = ConvertedEpgDate

            EPG.ManualRecording.NavigateToStartTime()

            EPG.ManualRecording.SetStartTime(tmpStartTime.ToString("HHmm"))

            EPG.ManualRecording.NavigateToChannel()

            If Me._ChannelName <> "" Then
                EPG.ManualRecording.SetChannel(Me._ChannelName)
            Else
                EventName = EPG.ManualRecording.SetChannel(Me._ChannelNumber)
            End If

            EPG.ManualRecording.NavigateToEndTime()

            tmpEndTime = tmpStartTime.AddMinutes(_DurationInMin)
            EndTime = tmpEndTime.ToString("HH:mm")
            EPG.ManualRecording.SetEndTime(tmpEndTime.ToString("HHmm"))

            EPG.ManualRecording.NavigateToFrequency()

            EPG.ManualRecording.SetFrequency(Frequency)

            Dim TmpFrequency As String = ""

            EPG.ManualRecording.GetFrequency(TmpFrequency)

            If TmpFrequency <> Frequency Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Frequency"))
            End If

            If Current Then
                EventSource = "ManualCurrent"
            Else
                EventSource = "ManualFuture"
            End If

            EPG.ManualRecording.NavigateToRecord(False)

            If _IsConflict Then

                EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, EventSource, MyStartTime, EndTime, EventChannel, _DurationInMin, _DurationInMin, EventDate, Frequency, Occurences, EventConvertedDate)

                Try
                    EPG.Menu.SelectToConflict()
                Catch ex As Exception
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ConflictFailure, "Failed To Create Conflict By Recording Event From Planner"))
                End Try
            Else
                EPG.ManualRecording.SaveAndEnd(Current)

                Try
                    EPG.ManualRecording.VerifySaveAndEndFinished(False)
                Catch ex As EAException
                    EPG.Events.Remove(_EventKeyName)
                    ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                End Try

                EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, EventSource, MyStartTime, EndTime, EventChannel, _DurationInMin, _DurationInMin, EventDate, Frequency, Occurences, EventConvertedDate)

                If _VerifyBookingInPCAT Then
                    res = Me._manager.PCAT.VerifyEventBooked(_EventKeyName)
                    If Not res.CommandSucceeded Then
                        EPG.Events.Remove(_EventKeyName)
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                End If

                EPG.Utils.ReturnToLiveViewing()
            End If

            Return True

        End Function

    End Class
End Namespace