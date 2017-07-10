Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Finds Event In PCAT.DB And Returnes The Index In The Excel
    ''' </summary>
    Public Class FindEvent
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _table As EnumPCATtables
        Private _CopyPCAT As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Table">In Which Table To Look In</param>
        ''' <param name="CopyPCAT">If True Copies The PCAT From STB</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>328 - INIFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal CopyPCAT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            EPG = Me._manager.UI
            Me._table = Table
            Me._CopyPCAT = CopyPCAT
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim EventChannel As String = ""
            Dim found As Boolean = False
            Dim PcatEvId As String = ""
            Dim PcatError As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"))
            End If

            EventName = EPG.Events(_EventKeyName).Name
            If EventName.EndsWith("&") Then
                EventName = EventName.Substring(0, EventName.Length - 2)
            End If

            EventChannel = EPG.Events(_EventKeyName).Channel

            EPG.Utils.LogCommentInfo("Searching Event : " + EventName + " In PCAT")

            If Me._CopyPCAT Then
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
            End If

            Dim EvDateStart As String = ""
            Dim EvDuration As String = ""

            If EventChannel = "0" Then
                EvDateStart = EPG.Events(_EventKeyName).EventDate + " " + EPG.Events(_EventKeyName).StartTime
                EvDuration = ((CInt(EPG.Events(_EventKeyName).Duration) * 60) * 1000).ToString
            End If

            Select Case _table
                Case EnumPCATtables.FromRecordings
                    EPG.Utils.LogCommentInfo("Searching Event In Table : RECORDINGS")
                    If EventChannel = "0" Then
                        EPG.Utils.LogCommentInfo("Searching By : EventName - " + EPG.Events(_EventKeyName).Name + " EventDateStart - " + EvDateStart + " EventDuration - " + EvDuration)
                        found = EPG.Utils.FindEventInPCAT(PcatEvId, ElementaryActions.EPG.SF.EnumTables.RECORDINGS, EventName, EvDateStart, EvDuration)
                    Else
                        EPG.Utils.LogCommentInfo("Searching By : EventName - " + EPG.Events(_EventKeyName).Name)
                        found = EPG.Utils.FindEventInPCAT(PcatEvId, ElementaryActions.EPG.SF.EnumTables.RECORDINGS, EventName)
                    End If

                Case EnumPCATtables.FromBookings
                    EPG.Utils.LogCommentInfo("Searching Event In Table : BOOKINGS")
                    If EventChannel = "0" Then
                        EPG.Utils.LogCommentInfo("Searching By : EventName - " + EPG.Events(_EventKeyName).Name + " EventDateStart - " + EvDateStart + " EventDuration - " + EvDuration)
                        found = EPG.Utils.FindEventInPCAT(PcatEvId, ElementaryActions.EPG.SF.EnumTables.BOOKINGS, EventName, EvDateStart, EvDuration)
                    Else
                        EPG.Utils.LogCommentInfo("Searching By : EventName - " + EPG.Events(_EventKeyName).Name)
                        found = EPG.Utils.FindEventInPCAT(PcatEvId, ElementaryActions.EPG.SF.EnumTables.BOOKINGS, EventName)
                    End If
            End Select


            If found = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Failed To Find Event In PCAT"))
            End If

            SetReturnValues(New Object() {PcatEvId})

        End Sub


    End Class

End Namespace