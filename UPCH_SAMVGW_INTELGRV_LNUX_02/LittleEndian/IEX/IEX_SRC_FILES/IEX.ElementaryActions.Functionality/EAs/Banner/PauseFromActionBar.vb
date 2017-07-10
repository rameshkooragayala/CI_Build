Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Select And Pause From Action Bar Menu
    ''' </summary>
    Public Class PauseFromActionBar
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>303 - FasVerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            EPG.Banner.Navigate()
            EPG.Banner.PauseEvent()

        End Sub

    End Class

End Namespace