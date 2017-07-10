Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Waits Until Event Start
    ''' </summary>
    Public Class WaitUntilEventStarts
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _StartGuardTime As String

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>309 - GetEpgTimeFailure</para> 	
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal StartGuardTime As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._manager = pManager
            Me._StartGuardTime = StartGuardTime
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim StartTime As String = ""
            Dim EpgTime As String = ""
            Dim TimeToWait As Integer = 0
            Dim StartGuardTimeNum As Integer = 0

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            StartTime = EPG.Events(_EventKeyName).StartTime

            EPG.Banner.Navigate()

            EPG.Live.GetEpgTime(EpgTime)

            EPG.Utils.ReturnToLiveViewing()

            TimeToWait = EPG.Utils._DateTime.SubtractInSec(CDate(StartTime), CDate(EpgTime))

            EPG.Utils.LogCommentImportant("Time To Wait Until Event Starts : " + (TimeToWait + 30).ToString + " Seconds (30 Seconds Has Been Added To Be Safe)")
            EPG.Utils.LogCommentImportant("Waiting Until : " + CDate(EpgTime).AddSeconds(TimeToWait + 30).ToString("HH:mm:ss"))

            If TimeToWait > 0 Then
                If Not String.IsNullOrEmpty(_StartGuardTime) Then
                    StartGuardTimeNum = EPG.Utils.GetGuardTimeFromFriendlyName(_StartGuardTime)
                    EPG.Utils.LogCommentInfo("Waiting till " + _StartGuardTime + " before event starts due to SGT")
                    TimeToWait = TimeToWait - Double.Parse(StartGuardTimeNum) * 60
                End If
                _iex.Wait(TimeToWait + 30)
            End If

        End Sub

    End Class

End Namespace
