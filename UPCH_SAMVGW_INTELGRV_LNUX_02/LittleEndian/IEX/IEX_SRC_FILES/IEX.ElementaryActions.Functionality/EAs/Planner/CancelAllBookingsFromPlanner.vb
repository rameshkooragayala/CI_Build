Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Deletes All Bookings In Planner
    ''' </summary>
    Public Class CancelAllBookingsFromPlanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>342 - CancelEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult

            EPG.FutureRecordings.Navigate()

            If Not EPG.FutureRecordings.isEmpty Then

                EPG.FutureRecordings.SelectEvent()
                EPG.FutureRecordings.CancelAllEvents()

            End If

            res = _manager.ReturnToLiveViewing()
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

        End Sub

    End Class

End Namespace
