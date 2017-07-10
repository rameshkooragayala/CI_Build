Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Search Event In VOD
    ''' </summary>
    Public Class SearchVodEvent
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventName As String
        Private _Navigate As Boolean
        Private _SupposedToFindEvent As Boolean

        ''' <param name="EventName">The Event Name To Find</param>
        ''' <param name="Navigate">If True Navigates To VOD</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventName As String, ByVal Navigate As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventName = EventName
            EPG = Me._manager.UI
            Me._Navigate = Navigate
        End Sub

        Protected Overrides Sub Execute()


            If _EventName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "EventName Is Empty"))
            End If

            If Me._Navigate Then
                EPG.Vod.Navigate()
            End If

            EPG.Vod.NavigateToEventName(_EventName)

            EPG.Vod.VerifyEventName(_EventName)

        End Sub

    End Class

End Namespace