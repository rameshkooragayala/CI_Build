Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''  Set Keep Or Un Keep On Planner Or Archive As Requested
    ''' </summary>
    Public Class SetKeepFlag
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _SetKeep As Boolean
        Private _ReturnToLive As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="SetKeep">If True Then Set To Keep If False Set To Un Keep</param>
        ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>312 - SetEventKeepFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal SetKeep As Boolean, ByVal ReturnToLive As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            EPG = Me._manager.UI
            Me._SetKeep = SetKeep
            Me._ReturnToLive = ReturnToLive
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim res As IEXGateway.IEXResult = Nothing
            Dim index As Integer = 0
            Dim IsPlanner As Boolean

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            'Checks If Keep Status Already Set As Asked In PCAT
            EPG.Utils.StartHideFailures("Checking If Event " + EventName + " Keep Status In PCAT Is Already " + Me._SetKeep.ToString)
            res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "KEEP", Me._SetKeep.ToString, True)
            If res.CommandSucceeded Then
                res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "BOOKING_EXCEPTION", "TRUE", False)
                If res.CommandSucceeded Then
                    _iex.ForceHideFailure()
                    Exit Sub
                End If
            End If
            _iex.ForceHideFailure()

            'Setting Keep Status
            EPG.Utils.StartHideFailures("Checking If Event " + EventName + " Is In Planner Or Archive By RECORD_STATUS In PCAT")
            res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "RECORD_STATUS", "CURRENT_RECORDING", False)
            If res.CommandSucceeded Then
                EPG.Utils.LogCommentInfo(EventName + " RECORD_STATUS In PCAT Is " + "CURRENT_RECORDING")
                IsPlanner = False
            Else
                res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "RECORD_STATUS", "PAST_RECORDING", False)
                If res.CommandSucceeded Then
                    EPG.Utils.LogCommentInfo(EventName + " RECORD_STATUS In PCAT Is " + "PAST_RECORDING")
                    IsPlanner = False
                Else
                    res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "RECORD_STATUS", "FUTURE_BOOKING", False)
                    EPG.Utils.LogCommentInfo(EventName + " RECORD_STATUS In PCAT Is " + "FUTURE_BOOKING")
                    IsPlanner = True
                    If res.CommandSucceeded = False Then
                        _iex.ForceHideFailure()
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.PCATFailure, "Event PARTIAL_CONTENT_STATUS Was Not CURRENT_RECORDING Or PAST_RECORDING Or FUTURE_BOOKING"))
                    End If
                End If
            End If
            _iex.ForceHideFailure()

            If IsPlanner Then
                EPG.Utils.LogCommentInfo(EventName + " Is On Planner")
                res = Me._manager.PVR.VerifyEventInPlanner(Me._EventKeyName)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If

                EPG.FutureRecordings.SelectEvent(EventName)
            Else
                EPG.Utils.LogCommentInfo(EventName + " Is On Archive")
                res = Me._manager.PVR.VerifyEventInArchive(Me._EventKeyName)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If

                EPG.ArchiveRecordings.SelectEvent(EventName)
            End If

            If Me._SetKeep Then
                EPG.Banner.SetEventKeep()
            Else
                EPG.Banner.RemoveEventKeep()
            End If

            res = Me._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "KEEP", Me._SetKeep.ToString.ToUpper, True)
            If res.CommandSucceeded Then
                EPG.Utils.LogCommentInfo(EventName + " Keep Status In PCAT Is " + Me._SetKeep.ToString.ToUpper)
            Else
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            If Me._ReturnToLive Then
                EPG.Utils.ReturnToLiveViewing()
            End If

        End Sub

    End Class

End Namespace