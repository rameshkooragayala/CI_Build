Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Verify Event Exception Reason
    ''' </summary>
    Public Class VerifyEventExceptionReason
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _ExceptionReason As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI


        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ExceptionReason">
        ''' <para>VIEWER_STOPPED</para>
        ''' <para>VIEWER_RESTARTED_AFTER_RECORDING_STOPPED</para>
        ''' <para>VIEWER_BOOKED_TOO_LATE</para>
        ''' <para>VIEWER_STOPPED_TUNER_IMMEDIATE_CONFLICT</para>
        ''' <para>VIEWER_STOPPED_TUNER_BOOKING_CONFLICT</para>
        ''' <para>VIEWER_STOPPED_DISK_CONFLICT</para>
        ''' <para>VIEWER_BOOKING_DELETED</para>
        ''' <para>POWER_FAILURE</para>
        ''' <para>INSUFFICIENT_DISK_SPACE</para>
        ''' <para>NO_EVENT_IN_SCHEDULE</para>
        ''' <para>NO_CARD_DETECTED</para>
        ''' <para>NO_TUNER_AVAILABLE</para>
        ''' <para>NO_CA_AUTHORISATION</para>
        ''' <para>RECORDED_CONTENT_MISSING</para>
        ''' <para>SIGNAL_LOSS</para>
        ''' <para>PMT_NOT_AVAILABLE</para>
        ''' <para>ES_NOT_AVAILABLE</para>
        ''' <para>RASP_OVERFLOW</para>
        ''' <para>MAX_FILE_SIZE_EXCEEDED</para>
        ''' <para>RECORDING_SUCCEEDED</para>
        ''' <para>BOOKING_EXPIRED</para>
        ''' <para>HEALTH_CHECK</para>
        ''' <para>SWDOWNLOAD</para>
        ''' <para>CONNECTION_LOST</para>
        ''' <para>HTTP Error</para>
        ''' <para>RESTARTED_AFTER_RECORDING_STOPPED</para>
        ''' <para>PASSIVE_STANDBY</para>
        ''' <para>NO_EXCEPTION</para>
        ''' <para>Unknown exception (100)</para>
        ''' </param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ExceptionReason As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            Me._ExceptionReason = ExceptionReason
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim found As Boolean = False

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            EventName = EPG.Events(_EventKeyName).Name

            EPG.Utils.LogCommentInfo("Verifying Event : " + EventName + " Exception Reason In PCAT Is " + Me._ExceptionReason)

            res = Me._manager.PCAT.VerifyEventStatus(Me._EventKeyName, EnumPCATtables.FromRecordings, "BOOKING_EXCEPTION", Me._ExceptionReason, True)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

        End Sub

    End Class

End Namespace