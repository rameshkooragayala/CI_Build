Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''  Waits Until Reminder Supposed To Be Shown ( Start Time Of The Event Minus 60 Sec )
    ''' </summary>
    Public Class WaitUntilReminder
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String

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
        ''' <para>354 - ReminderFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim StartTime As String = ""
            Dim EpgTime As String = ""
            Dim TimeToWait As Integer = 0

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

            TimeToWait = EPG.Utils._DateTime.SubtractInSec(CDate(StartTime).AddMinutes(-1), CDate(EpgTime))

            _iex.Wait(TimeToWait - 90)

            EPG.Utils.LogCommentImportant("Time To Wait Until Reminder : 90 Seconds")
            EPG.Utils.LogCommentImportant("Waiting For Reminder Until : " + CDate(EpgTime).AddSeconds(TimeToWait).ToString("HH:mm:ss"))

            EPG.OSD_Reminder.VerifyReminderAppeared()

        End Sub

    End Class

End Namespace
