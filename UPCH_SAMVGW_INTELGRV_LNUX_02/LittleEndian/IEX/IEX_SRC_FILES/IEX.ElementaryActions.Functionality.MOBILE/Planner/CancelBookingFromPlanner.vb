Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Cancel Booking From Planner Screen
    ''' </summary>
    Public Class CancelBookingFromPlanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Navigate As Boolean
        Private _OccurrenceNumber As Integer
        Private _VerifyCancelInPCAT As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="OccurrenceNumber">The Occurrence To Cancel From 1 To 14</param>
        ''' <param name="VerifyCancelInPCAT">Optional Parameter Default = True : If True Verifies Event Cancelled In PCAT</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>342 - CancelEventFailure</para> 
        ''' <para>345 - DeleteEventFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal OccurrenceNumber As Integer, ByVal VerifyCancelInPCAT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            Me._OccurrenceNumber = OccurrenceNumber
            Me._VerifyCancelInPCAT = VerifyCancelInPCAT
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult
            Dim EventName As String = ""


            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            Dim Occurrences As Integer = EPG.Events(_EventKeyName).Occurrences

            If Occurrences > 1 Then 'Recurrent Event
                Dim EventDate As String = CDate(EPG.Events(_EventKeyName).EventDate).AddDays(_OccurrenceNumber - 1).ToString(EPG.Utils.GetEpgDateFormat())
                Dim StartTime As String = EPG.Events(_EventKeyName).StartTime
                Dim EndTime As String = EPG.Events(_EventKeyName).EndTime

                EPG.FutureRecordings.Navigate()
                EPG.FutureRecordings.FindEvent(EventName, EventDate, StartTime, EndTime)
            Else
                res = Me._manager.PVR.VerifyEventInPlanner(Me._EventKeyName)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If
           
            EPG.FutureRecordings.SelectEvent(EventName)

            EPG.FutureRecordings.CancelEvent()

            'If Me._VerifyCancelInPCAT Then
            '    res = Me._manager.PCAT.VerifyEventDeleted(Me._EventKeyName, 2)
            '    If Not res.CommandSucceeded Then
            '        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            '    Else
            '        EPG.Events(_EventKeyName).Occurrences -= 1 'Decrement The Number Of Occurrences Since Event Cancelled
            '    End If
            'End If

        End Sub

    End Class

End Namespace