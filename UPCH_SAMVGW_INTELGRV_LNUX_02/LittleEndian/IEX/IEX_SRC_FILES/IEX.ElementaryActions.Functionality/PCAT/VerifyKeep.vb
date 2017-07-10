Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''  Verify Event Keep Status In PCAT
    ''' </summary>
    Public Class VerifyKeep
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Keep As Boolean
        Private EPG As IEX.ElementaryActions.EPG.SF.UI

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Keep">If True Search Status TRUE Else Search FALSE</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Keep As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            Me._Keep = Keep
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "KEEP", Me._Keep.ToString.ToUpper, True)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

        End Sub

    End Class

End Namespace