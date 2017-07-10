Namespace EAImplementation
    ''' <summary>
    '''   Replace The FailStep Function Of The Userlib
    ''' </summary>
    Public Class FailStep
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="pManager">Manager</param>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            'Dim res As IEXGateway._IEXResult

            'res = _manager.StillAlive()
            'res = _manager.MountTelnetStb(True)

        End Sub

    End Class

End Namespace