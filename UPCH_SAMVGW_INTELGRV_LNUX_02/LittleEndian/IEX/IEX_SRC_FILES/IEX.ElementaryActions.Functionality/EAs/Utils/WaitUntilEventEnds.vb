Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''  Waits Until Event Ends
    ''' </summary>
    Public Class WaitUntilEventEnds
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _EndGuardTime As String

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
        Sub New(ByVal EventKeyName As String, ByVal EndGuardTime As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._manager = pManager
            Me._EndGuardTime = EndGuardTime
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim EndTime As String = ""
            Dim EpgTime As String = ""
            Dim TimeToWait As Integer = 0
            Dim EndGuardTimeNum As Integer = 0

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            EndTime = EPG.Events(_EventKeyName).EndTime

            EPG.Banner.Navigate()

            EPG.Live.GetEpgTime(EpgTime)

            EPG.Utils.ReturnToLiveViewing()

            TimeToWait = EPG.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(EpgTime))

            EPG.Utils.LogCommentImportant("Time To Wait Until Event Ends : " + TimeToWait.ToString + " Seconds")
            EPG.Utils.LogCommentImportant("Waiting Until : " + CDate(EpgTime).AddSeconds(TimeToWait).ToString("HH:mm:ss"))

            If TimeToWait > 0 Then
                _iex.Wait(TimeToWait)
            End If

            If Not String.IsNullOrEmpty(_EndGuardTime) Then
                EndGuardTimeNum = EPG.Utils.GetGuardTimeFromFriendlyName(_EndGuardTime, False)
                EPG.Utils.LogCommentInfo("Waiting for " + _EndGuardTime + " for EGT to complete")
                _iex.Wait(Double.Parse(EndGuardTimeNum) * 60)
            End If

        End Sub

    End Class

End Namespace