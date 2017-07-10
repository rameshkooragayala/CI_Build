Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Verify Event Duration Is Greater Or Smaller From Given Duration
    ''' </summary>
    Public Class VerifyEventDuration
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _DurationInSec As Integer
        Private _IsDurationLarger As Boolean

        ''' <param name="EventKeyName">The Key Of The Event</param>
        ''' <param name="DurationInSec">Duration In Seconds To Check</param>
        ''' <param name="IsDurationLarger">If True Larger Else Smaller</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>305 - PCATFailure</para> 
        ''' <para>307 - GetStreamInfoFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal DurationInSec As Integer, ByVal IsDurationLarger As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._DurationInSec = DurationInSec
            Me._IsDurationLarger = IsDurationLarger
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventDuration As Long
            Dim res As IEXGateway._IEXResult
            Dim EventName As String = ""
            Dim Duration As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            ElseIf Me._EventKeyName.ToLower <> "rb" Then
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            If Me._EventKeyName.ToLower = "rb" Then
                EPG.TrickModes.GetCurrentPlaybackDuration(EventDuration)
            Else
                If EPG.Events(_EventKeyName).Source.Contains("Manual") Then
                    'Bug In PCAT StartTime So Getting Duration From PCAT Failes Taking Given Duration From User
                    EPG.Utils.LogCommentWarning("WORKAROUND : BUG IN PCAT STARTTIME SO GETTING DURATION FROM PCAT FAILES TAKING GIVEN DURATION FROM USER")
                    EventDuration = EPG.Events(_EventKeyName).Duration
                Else
                    res = Me._manager.PCAT.GetEventDuration(_EventKeyName, EnumPCATtables.FromRecordings, Duration)
                    If Not res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If

                    EventDuration = EPG.Events(_EventKeyName).Duration
                End If

                'res = _manager.PCAT.GetEventDuration(_EventKeyName)
                'If Not res.CommandSucceeded Then
                '    ExceptionUtils.ThrowEx( New EAException(res.FailureCode, res.FailureReason))
                'End If
                'EventDuration = EPG.Events(_EventKeyName).Duration
            End If

            If _IsDurationLarger Then
                If EventDuration >= _DurationInSec Or EventDuration = (_DurationInSec - 1) Then
                    EPG.Utils.LogCommentInfo("Verified Actual Duration -> " + EventDuration.ToString + " Is Equal Or Greater Then Asked Duration -> " + _DurationInSec.ToString)
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Actual Duration -> " + EventDuration.ToString + " Is Smaller Then Asked Duration -> " + _DurationInSec.ToString))
                End If
            Else
                If EventDuration < _DurationInSec Then
                    EPG.Utils.LogCommentInfo("Verified Actual Duration -> " + EventDuration.ToString + " Is Smaller Then Asked Duration -> " + _DurationInSec.ToString)
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Actual Duration -> " + EventDuration.ToString + " Is Equal Or Greater Then Asked Duration -> " + _DurationInSec.ToString))
                End If
            End If

        End Sub

    End Class

End Namespace