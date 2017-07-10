Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   UnLocks Channel From Action Bar
    ''' </summary>
    Public Class UnlockChannelFromBanner
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
            EPG.Banner.Navigate()
            EPG.Banner.UnLockChannel()
        End Sub

    End Class

End Namespace


