Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''  Resolves recording and booking conflicts
    ''' </summary>
    Public Class ResolveConflict
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _ResolveOption As String
        Private _VerifyInPCAT As Boolean
        Private _EventToCancel As Integer

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="ResolveOption">Use: DEFER, AUTOMATICALLY, MANUALLY, CANCEL BOOKING</param>
        ''' <param name="VerifyInPCAT">If True Verifies Event Booked or Canceled In PCAT</param>
        ''' <param name="EventToCancel">Optional Parameter Default = 1 : Not Implemented</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>339 - RecordEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal ResolveOption As String, ByVal VerifyInPCAT As Boolean, ByVal EventToCancel As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._ResolveOption = ResolveOption
            Me._VerifyInPCAT = VerifyInPCAT
            Me._EventToCancel = EventToCancel
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim Action As String = ""
            Dim res As IEXGateway._IEXResult
            Dim EventName As String = ""

            'Commenting the verification of Event key name as it is not required for few of the conflict options
            'If Me._EventKeyName = "" Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            'Else
            '    Try
            '        EventName = EPG.Events(_EventKeyName).Name
            '    Catch ex As Exception
            '        ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
            '    End Try
            'End If

            'EPG TEXT
            Dim DefferText As String = ""
            Dim AutomaticallyText As String = ""
            Dim ConflictManually As String = ""
            Dim ConflictCancelBooking As String = ""
            DefferText = EPG.Utils.GetValueFromDictionary("DIC_CONFLICT_ACTION_DEFER")
            EPG.Menu.SetConflictAction(_ResolveOption.ToUpper)
            If DefferText = _ResolveOption.ToUpper Then
                Action = "DEFER"
            Else
                AutomaticallyText = EPG.Utils.GetValueFromDictionary("DIC_CONFLICT_ACTION_AUTOMATICALLY")
                If AutomaticallyText = _ResolveOption.ToUpper Then
                    EPG.Utils.SendIR("SELECT")
                    'ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "AUTOMATICALLY Is Not Implemented."))
                Else

                    ConflictManually = EPG.Utils.GetValueFromDictionary("DIC_CONFLICT_ACTION_MANUALLY")
                    If ConflictManually = _ResolveOption.ToUpper Then

                        EPG.Utils.SendIR("SELECT")
                        EPG.Utils.VerifyState("MAUNAL CANCEL OPTIONS", TimeOutInSec:=25)
                        Try
                            EventName = EPG.Events(_EventKeyName).Name
                        Catch ex As Exception
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                        End Try
                        EPG.Menu.SetConflictAction(EventName)
                        EPG.Utils.SendIR("SELECT")
                        'ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "MANUALLY Is Not Implemented."))
                    Else
                        ConflictCancelBooking = EPG.Utils.GetValueFromDictionary("DIC_CONFLICT_ACTION_CANCEL")
                        If ConflictCancelBooking = _ResolveOption.ToUpper Then
                            EPG.Utils.SendIR("SELECT")
                        Else
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ResolveOption Is Empty Or Not Valid. Use: " + DefferText + "," + AutomaticallyText + "," + ConflictManually + " Or " + ConflictCancelBooking))
                        End If
                    End If
                End If
            End If



            If Action = "DEFER" Then

                Select Case EPG.Events(_EventKeyName).Source
                    Case "BannerCurrent"
                        EPG.Banner.RecordEvent(True, False, True)
                    Case "BannerFuture"
                        EPG.Banner.RecordEvent(False, False, True)
                    Case "ManualCurrent"
                        EPG.ManualRecording.SaveAndEnd(True)
                    Case "ManualFuture"
                        EPG.ManualRecording.SaveAndEnd(False)
                    Case "GuideCurrent"
                        EPG.Guide.RecordEvent(True, True)
                    Case "GuideFuture"
                        EPG.Guide.RecordEvent(False, True)
                    Case Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventSource Was Not Recognized."))
                End Select

                If Me._VerifyInPCAT Then
                    res = Me._manager.PCAT.VerifyEventBooked(Me._EventKeyName)
                    If Not res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                End If
            Else
                If Me._VerifyInPCAT Then
                    res = Me._manager.PCAT.FindEvent("", Me._EventKeyName, EnumPCATtables.FromBookings, True)
                    If res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If
                End If
            End If
        End Sub

    End Class
End Namespace

