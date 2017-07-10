Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   UnLocks Events From A Locked Channel
    ''' </summary>
    Public Class UnlockEvent
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

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
            EPG.ChannelBar.UnlockEvent()
        End Sub

    End Class

End Namespace