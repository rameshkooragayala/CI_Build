Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''  Navigates To Archive
    ''' </summary>
    Public Class NavigateToArchive
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            EPG.ArchiveRecordings.Navigate()
        End Sub

    End Class

End Namespace