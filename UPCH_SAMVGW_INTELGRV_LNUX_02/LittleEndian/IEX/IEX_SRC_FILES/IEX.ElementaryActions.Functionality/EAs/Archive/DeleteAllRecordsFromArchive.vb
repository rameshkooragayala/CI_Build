Imports FailuresHandler

Namespace EAImplementation

    Public Class DeleteAllRecordsFromArchive
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _Navigate As Boolean

        Sub New(ByVal Navigate As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._Navigate = Navigate
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            EPG.ArchiveRecordings.DeleteAllEvents(True)
        End Sub

    End Class

End Namespace