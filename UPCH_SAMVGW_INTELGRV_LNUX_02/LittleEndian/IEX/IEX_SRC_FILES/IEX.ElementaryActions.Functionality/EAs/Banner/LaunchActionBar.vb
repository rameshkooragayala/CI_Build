Namespace EAImplementation

    ''' <summary>
    '''   Raise The Action Bar
    ''' </summary>
    Public Class LaunchActionBar
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _FromPlayback As Boolean

        '''<param name="pManager">Manager</param>
        Sub New(ByVal FromPlayback As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            _FromPlayback = FromPlayback
        End Sub

        Protected Overrides Sub Execute()
            EPG.Banner.Navigate(_FromPlayback)
        End Sub
    End Class

End Namespace