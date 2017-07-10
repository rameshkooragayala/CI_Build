Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Compares Current EPG Time And Given Event Times
    ''' </summary>
    Public Class VerifyEventSchedule
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _EventOccures As EnumEventOccures
        Private _EventTimes As String

        ''' <param name="EventOccures">Can Be : Past,Current Or Future</param>
        ''' <param name="EventTimes">Event Time In Format HH:MM - HH:MM</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>309 - GetEpgTimeFailure</para> 
        ''' <para>350 - ParsingFailure</para>  
        ''' <para>362 - TimeFailure </para>        
        ''' </remarks>
        Sub New(ByVal EventOccures As EnumEventOccures, ByVal EventTimes As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
            _EventOccures = EventOccures
            _EventTimes = EventTimes
        End Sub

        Protected Overrides Sub Execute()
            Dim StartTime As String = ""
            Dim EndTime As String = ""
            Dim EpgTime As String = ""
            Dim Result As Integer = 9000

            EPG.Utils.ParseEventTime(StartTime, _EventTimes, IsStartTime:=True)

            EPG.Utils.ParseEventTime(EndTime, _EventTimes, IsStartTime:=False)

            EPG.Live.GetEpgTime(EpgTime)

            Select Case _EventOccures
                Case EnumEventOccures.Past
                    Result = EPG.Utils._DateTime.Subtract(EndTime, EpgTime)
                    If Result < 0 Or Result = 9000 Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.TimeFailure, "EndTime - " + EndTime + " Is Smaller Then EPG Time - " + EpgTime))
                    End If

                    EPG.Utils.LogCommentInfo("Event Scheduled In The Past")

                Case EnumEventOccures.Current
                    Result = EPG.Utils._DateTime.Subtract(EndTime, EpgTime)
                    If Result > 0 Then
                        Result = EPG.Utils._DateTime.Subtract(StartTime, EpgTime)
                        If Result > 0 Or Result = 9000 Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.TimeFailure, "Start Time - " + StartTime + " Is Greater Then EPG Time - " + EpgTime))
                        End If
                    ElseIf Result < 0 Or Result = 9000 Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.TimeFailure, "EndTime - " + EndTime + " Is Smaller Then EPG Time - " + EpgTime))
                    End If

                    EPG.Utils.LogCommentInfo("Event Scheduled Now")

                Case EnumEventOccures.Future
                    Result = EPG.Utils._DateTime.Subtract(StartTime, EpgTime)
                    If Result < 0 Or Result = 9000 Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.TimeFailure, "StartTime - " + StartTime + " Is Smaller Then EPG Time - " + EpgTime))
                    End If

                    EPG.Utils.LogCommentInfo("Event Scheduled In The Future")
            End Select


        End Sub

    End Class

End Namespace
