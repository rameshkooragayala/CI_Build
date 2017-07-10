Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Get Event Duration From PCAT
    ''' </summary>
    Public Class GetEventDuration
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _EventKeyName As String
        Private _EventName As String
        Private _Table As EnumPCATtables

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Table">The Table To Look In</param>
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
            EPG = Me._manager.UI
            Me._EventKeyName = EventKeyName
            Me._Table = Table
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim found As Boolean = False
            Dim PcatEvId As String = ""
            Dim Duration As String = ""
            Dim PcatError As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            EventName = EPG.Events(_EventKeyName).Name

            EPG.Utils.LogCommentInfo("Getting Event Duration From PCAT")

            For i As Integer = 0 To 1
                PcatError = ""
                res = Me._manager.PCAT.CopyPCAT()
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, res.FailureReason))
                End If
                If EPG.Utils.IsPCATValid(PcatError) Then
                    Exit For
                ElseIf i = 1 Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Error In PCAT : " + PcatError))
                End If
            Next

            If EventName <> "" Then
                Select Case _Table
                    Case EnumPCATtables.FromBookings
                        found = EPG.Utils.FindEventInPCAT(PcatEvId, IEX.ElementaryActions.EPG.SF.EnumTables.BOOKINGS, EventName)
                    Case EnumPCATtables.FromRecordings
                        found = EPG.Utils.FindEventInPCAT(PcatEvId, IEX.ElementaryActions.EPG.SF.EnumTables.RECORDINGS, EventName)
                End Select
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventName Can't Be Empty"))
            End If

            Try
                Select Case _Table
                    Case EnumPCATtables.FromBookings
                        If Not EPG.Utils.GetEventInfo(PcatEvId, ElementaryActions.EPG.SF.EnumTables.BOOKINGS, "DURATION", Duration) Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed To Get Duration From PCAT"))
                        End If
                    Case EnumPCATtables.FromRecordings
                        If Not EPG.Utils.GetEventInfo(PcatEvId, ElementaryActions.EPG.SF.EnumTables.RECORDINGS, "ACTUAL_DURATION", Duration) Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed To Get Actual Duration From PCAT"))
                        End If
                End Select

                EPG.Events(_EventKeyName).Duration = (CLng(Duration) / 1000)

                EPG.Utils.LogCommentImportant("Found Event Duration In PCAT : " + EPG.Events(_EventKeyName).Duration.ToString)

                SetReturnValues(New Object() {EPG.Events(_EventKeyName).Duration.ToString})

            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Failed To Find Event Duration In PCAT"))
            End Try

        End Sub

    End Class

End Namespace