Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Verify Event Partial Recording Staus
    ''' </summary>
    Public Class VerifyEventPartialStatus
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Expected As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Expected">ALL,PARTIAL Or NONE</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Expected As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            Me._Expected = Expected
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim found As Boolean = False

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If
            If Me._Expected <> "NONE" And Me._Expected <> "PARTIAL" And Me._Expected <> "ALL" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Partial Content Status Can Be Only - NONE,PARTIAL,ALL"))
            End If

            EventName = EPG.Events(_EventKeyName).Name

            EPG.Utils.LogCommentInfo("Verifying Event : " + EventName + " Partial Content Status In PCAT Is " + Me._Expected)

            res = Me._manager.PCAT.VerifyEventStatus(Me._EventKeyName, EnumPCATtables.FromRecordings, "PARTIAL_CONTENT_STATUS", Me._Expected, True)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

        End Sub

    End Class

End Namespace