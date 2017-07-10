Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Modify Manual Recording From Planner
    ''' </summary>
    Public Class ModifyManualRecording
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        Private _EventKeyName As String
        Private _StartTime As String
        Private _EndTime As String
        Private _ChannelName As String
        Private _Days As Integer
        Private _Frequency As EnumFrequency

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="StartTime">Channel Name</param>
        ''' <param name="EndTime">Channel Number If Entered Doing DCA</param>
        ''' <param name="ChannelName">Optional Parameter Default = "" : The Channel Name</param>
        ''' <param name="Days">Optional Parameter Default = 0 : Adds Days From Current Date</param>
        ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
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
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal StartTime As String, ByVal EndTime As String, ByVal ChannelName As String, _
                ByVal Days As Integer, ByVal Frequency As EnumFrequency, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)

            Me._EventKeyName = EventKeyName
            Me._StartTime = StartTime
            Me._EndTime = EndTime
            Me._ChannelName = ChannelName
            Me._Days = Days
            Me._Frequency = Frequency
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            If _manager.Project.IsEPGLikeCogeco Then
                ModifyManualFromPlannerCOGECO()
            Else
                ModifyManualFromPlannerUPC()
            End If
        End Sub

        Private Function ModifyManualFromPlannerCOGECO() As Boolean
            Dim res As IEXGateway.IEXResult
            Dim EventDate As String = ""
            Dim EventName As String = ""
            Dim MyStartTime As String = ""
            Dim EndTime As String = ""
            Dim Current As Boolean = True
            Dim EventChannel As String = ""
            Dim EventConvertedDate As String = ""
            Dim EventSource As String = ""
            Dim Frequency As String = ""
            Dim Occurences As Integer = -1

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            If _StartTime <> "" AndAlso _StartTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Start Time Should Be Entered With Format : HH:MM"))
            End If

            If _EndTime <> "" AndAlso _EndTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "End Time Should Be Entered With Format : HH:MM"))
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

            EventDate = EPG.Events(_EventKeyName).EventDate

            res = _manager.PVR.VerifyEventInPlanner(_EventKeyName)
            If res.CommandSucceeded = False Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            EPG.FutureRecordings.SelectEvent(EventName)

            EPG.FutureRecordings.ModifyEvent()

            Select Case _Frequency
                Case EnumFrequency.ONE_TIME, EnumFrequency.DAILY
                    If _Days > 0 Then
                        Occurences = Occurences - _Days
                    End If
                Case Else
                    If _Days > 7 Then
                        Occurences = 1
                    End If
            End Select

            If _Days <> 0 Then
                Dim ConvertedEpgDate As String = CDate(EventDate).AddDays(_Days).ToString(EPG.Utils.GetEpgDateFormat())
                Dim dateFormatForEventDic As String = EPG.Utils.GetDateFormatForEventDictionary()
                EventDate = CDate(EventDate).AddDays(_Days).ToString(dateFormatForEventDic)

                EPG.ManualRecording.NavigateToDate()

                EPG.ManualRecording.SetDate(ConvertedEpgDate, False)

                EventConvertedDate = ConvertedEpgDate

                EventDate = CDate(EventDate).ToString(dateFormatForEventDic)
            End If

            If _StartTime <> "" Then
                Dim tmpStartTime As Date
                tmpStartTime = CDate(_StartTime)

                EPG.ManualRecording.NavigateToStartTime()

                MyStartTime = tmpStartTime.ToString("HH:mm")

                EPG.ManualRecording.SetHour(tmpStartTime.Hour)

                EPG.ManualRecording.SetMinutes(tmpStartTime.Minute)

                EPG.ManualRecording.VerifyStateId("55")
            End If

            If _EndTime <> "" Then
                Dim tmpEndTime As Date
                tmpEndTime = CDate(_EndTime)

                EPG.ManualRecording.NavigateToEndTime()

                EndTime = tmpEndTime.ToString("HH:mm")

                EPG.ManualRecording.SetHour(tmpEndTime.Hour)

                EPG.ManualRecording.SetMinutes(tmpEndTime.Minute)

                EPG.ManualRecording.VerifyStateId("55")
            End If

            If _Frequency <> EnumFrequency.ONE_TIME Then
                EPG.ManualRecording.NavigateToFrequency()

                EPG.ManualRecording.SetFrequency(Frequency)

                EPG.ManualRecording.VerifyStateId("55")
            End If


            If Me._ChannelName <> "" AndAlso Me._ChannelName <> EPG.Events(_EventKeyName).Channel Then
                EPG.ManualRecording.NavigateToChannel()

                EPG.ManualRecording.SetChannel(Me._ChannelName)

                EPG.ManualRecording.VerifyStateId("55")
            End If

            EPG.ManualRecording.NavigateToRecord(False)

            EventSource = "ManualFuture"


            EPG.ManualRecording.SaveAndEnd(Current)

            Try
                EPG.ManualRecording.VerifySaveAndEndFinished(False)
            Catch ex As EAException
                EPG.Events.Remove(_EventKeyName)
                ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
            End Try

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, EventSource, MyStartTime, EndTime, EventChannel, 0, 0, EventDate, Frequency, Occurences, EventConvertedDate, True)

            EPG.Utils.ReturnToLiveViewing()

            Return True
        End Function

        Private Function ModifyManualFromPlannerUPC() As Boolean
            Dim res As IEXGateway.IEXResult
            Dim EventDate As String = ""
            Dim EventName As String = ""
            Dim MyStartTime As String = ""
            Dim EndTime As String = ""
            Dim Current As Boolean = True
            Dim EventChannel As String = ""
            Dim EventConvertedDate As String = ""
            Dim EventSource As String = ""
            Dim Frequency As String = ""
            Dim Occurences As Integer = -1

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            If _StartTime <> "" AndAlso _StartTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Start Time Should Be Entered With Format : HH:MM"))
            End If

            If _EndTime <> "" AndAlso _EndTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "End Time Should Be Entered With Format : HH:MM"))
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

            EventDate = EPG.Events(_EventKeyName).EventDate

            res = _manager.PVR.VerifyEventInPlanner(_EventKeyName)
            If res.CommandSucceeded = False Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            EPG.FutureRecordings.SelectEvent(EventName)

            EPG.FutureRecordings.ModifyEvent()

            Select Case _Frequency
                Case EnumFrequency.ONE_TIME, EnumFrequency.DAILY
                    If _Days > 0 Then
                        Occurences = Occurences - _Days
                    End If
                Case Else
                    If _Days > 7 Then
                        Occurences = 1
                    End If
            End Select
           

            If _Days > 0 Then
                Dim ConvertedEpgDate As String = CDate(EventDate).AddDays(_Days).ToString(EPG.Utils.GetEpgDateFormat())
                Dim dateFormatForEventDic As String = EPG.Utils.GetDateFormatForEventDictionary()
                EventDate = CDate(EventDate).AddDays(_Days).ToString(dateFormatForEventDic)

                EPG.ManualRecording.NavigateToDate()

                EPG.ManualRecording.SetDate(ConvertedEpgDate, False)

                EventConvertedDate = ConvertedEpgDate

                EventDate = CDate(EventDate).ToString(dateFormatForEventDic)
            End If

            If _StartTime <> "" Then
                Dim tmpStartTime As Date
                tmpStartTime = CDate(_StartTime)

                EPG.ManualRecording.NavigateToStartTime()

                MyStartTime = tmpStartTime.ToString("HH:mm")

                EPG.ManualRecording.SetStartTime(tmpStartTime.ToString("HHmm"))

                EPG.ManualRecording.VerifyStateId("52")
            End If

            If _EndTime <> "" Then
                Dim tmpEndTime As Date
                tmpEndTime = CDate(_EndTime)

                EPG.ManualRecording.NavigateToEndTime()

                EndTime = tmpEndTime.ToString("HH:mm")

                EPG.ManualRecording.SetEndTime(tmpEndTime.ToString("HHmm"))
            End If

            If _Frequency <> EnumFrequency.ONE_TIME Then
                EPG.ManualRecording.NavigateToFrequency()

                EPG.ManualRecording.SetFrequency(Frequency)
            End If


            If Me._ChannelName <> "" AndAlso Me._ChannelName <> EPG.Events(_EventKeyName).Channel Then
                EPG.ManualRecording.NavigateToChannel()

                EPG.ManualRecording.SetChannel(Me._ChannelName)
            End If

            EPG.ManualRecording.NavigateToRecord(True)

            EventSource = "ManualFuture"

            EPG.ManualRecording.SaveAndEnd(Current)

            Try
                EPG.ManualRecording.VerifySaveAndEndFinished(False)
            Catch ex As EAException
                EPG.Events.Remove(_EventKeyName)
                ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
            End Try

            EPG.Utils.InsertEventToCollection(Me._EventKeyName, EventName, EventSource, MyStartTime, EndTime, EventChannel, 0, 0, EventDate, Frequency, Occurences, EventConvertedDate, True)

            EPG.Utils.ReturnToLiveViewing()

            Return True
        End Function
    End Class


End Namespace