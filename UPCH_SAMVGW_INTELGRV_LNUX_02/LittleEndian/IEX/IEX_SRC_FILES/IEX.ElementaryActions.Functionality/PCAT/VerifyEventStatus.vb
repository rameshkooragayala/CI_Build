Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Internal Function For Verifying Parameter Status In PCAT
    ''' </summary>
    Public Class VerifyEventStatus
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _ExpectedStatus As String
        Private _Field As String
        Private _CopyPCAT As Boolean
        Private _Table As EnumPCATtables

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Table">Recordings Or Bookings</param>
        ''' <param name="Field">Field Of Status In PCAT Modifier</param>
        ''' <param name="ExpectedStatus">Expected String In Column</param>
        ''' <param name="CopyPCAT">If True Copies The PCAT From STB</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal Field As String, ByVal ExpectedStatus As String, ByVal CopyPCAT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            EPG = Me._manager.UI
            Me._ExpectedStatus = ExpectedStatus
            Me._Table = Table
            Me._CopyPCAT = CopyPCAT
            Me._Field = Field
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim found As Boolean = False
            Dim PcatEvId As String = ""
            Dim MyTable As ElementaryActions.EPG.SF.EnumTables

            If Me._ExpectedStatus = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param ExpectedStatus Is Empty"))
            End If

            Select Case _Table
                Case EnumPCATtables.FromRecordings
                    MyTable = ElementaryActions.EPG.SF.EnumTables.RECORDINGS
                Case EnumPCATtables.FromBookings
                    MyTable = ElementaryActions.EPG.SF.EnumTables.BOOKINGS
            End Select

            EventName = EPG.Events(_EventKeyName).Name

            EPG.Utils.LogCommentInfo("Verifying Event : " + EventName + " Status In Field : " + Me._Field + " In PCAT Is " + _ExpectedStatus)

            res = Me._manager.PCAT.FindEvent(PcatEvId, _EventKeyName, Me._Table, _CopyPCAT)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            Dim ReturnedValue As String = ""

            found = EPG.Utils.VerifyEventInfo(PcatEvId, MyTable, Me._Field, _ExpectedStatus, ReturnedValue)

            If found = False Then
                EPG.Utils.LogCommentImportant("Status In PCAT : " + ReturnedValue.ToString)
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Failed To Verify Event Status Is " + _ExpectedStatus + " In PCAT ( Recieved: " + ReturnedValue.ToString + " )"))
            End If

            EPG.Utils.LogCommentInfo("Verifyied Event : " + EventName + " Status In Column : " + Me._Field + " In PCAT Is " + _ExpectedStatus)

        End Sub


    End Class

End Namespace