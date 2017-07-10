Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Locks Channel From Action Bar
    ''' </summary>
    Public Class LockChannelFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EnterPIN As Boolean

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>303 - FasVerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' </remarks>
        Sub New(ByVal EnterPIN As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._EnterPIN = EnterPIN
        End Sub

        Protected Overrides Sub Execute()
            EPG.Banner.Navigate()
            EPG.Banner.LockChannel(Me._EnterPIN)
        End Sub

    End Class

End Namespace