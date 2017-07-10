Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Verify Event Is Deleted From PCAT
    ''' </summary>
    Public Class VerifyEventDeleted
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _table As EnumPCATtables

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Table">In Which Table To Look In</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            EPG = Me._manager.UI
            _table = Table
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim found As Boolean = False
            Dim PcatEvId As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            EventName = EPG.Events(_EventKeyName).Name

            EPG.Utils.LogCommentInfo("Verifying Event : " + EventName + " Deleted In PCAT")

            Select Case _table
                Case EnumPCATtables.FromRecordings
                    res = Me._manager.PCAT.FindEvent(PcatEvId, _EventKeyName, EnumPCATtables.FromRecordings, True)
                    If res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If

                Case EnumPCATtables.FromBookings
                    res = Me._manager.PCAT.FindEvent(PcatEvId, _EventKeyName, EnumPCATtables.FromBookings, True)
                    If res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Table Can Be FromRecordings Or FromBookings"))
            End Select

        End Sub


    End Class

End Namespace