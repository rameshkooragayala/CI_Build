Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Get Event Information By The Field Name
    ''' </summary>
    Public Class GetEventInfo
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _EventKeyName As String
        Private _Table As EnumPCATtables
        Private _FieldName As String

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Table">The Table To Look In</param>
        ''' <param name="FieldName">The Field To Look In</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal FieldName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._EventKeyName = EventKeyName
            Me._Table = Table
            Me._FieldName = FieldName
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim found As Boolean = False
            Dim PcatEvId As String = ""
            Dim Value As String = ""
            Dim PcatError As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            If Me._FieldName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "FieldName Is Empty"))
            End If

            EventName = EPG.Events(_EventKeyName).Name

            EPG.Utils.LogCommentInfo("Getting " + _FieldName + " Value From PCAT")

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
                        If Not EPG.Utils.GetEventInfo(PcatEvId, IEX.ElementaryActions.EPG.SF.EnumTables.BOOKINGS, Me._FieldName, Value) Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed To Get The Value On Field " + Me._FieldName + " From PCAT"))
                        End If
                    Case EnumPCATtables.FromRecordings
                        If Not EPG.Utils.GetEventInfo(PcatEvId, IEX.ElementaryActions.EPG.SF.EnumTables.RECORDINGS, Me._FieldName, Value) Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed To Get The Value On Field " + Me._FieldName + " From PCAT"))
                        End If
                End Select

                EPG.Utils.LogCommentImportant("Found Event Info On Field " + Me._FieldName + " In PCAT : " + Value)

                SetReturnValues(New Object() {Value})

            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Failed To Get Event Info On Field : " + Me._FieldName + " In PCAT"))
            End Try

        End Sub

    End Class

End Namespace