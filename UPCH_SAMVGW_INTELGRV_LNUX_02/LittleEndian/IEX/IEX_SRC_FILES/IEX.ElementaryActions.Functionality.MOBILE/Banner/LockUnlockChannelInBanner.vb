Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Lock/Unlock Channel In Banner
    ''' </summary>
    Public Class LockUnlockChannelInBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _isLock As Boolean

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>346 - FindEventFailure</para>
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal IsLock As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            _isLock = IsLock
        End Sub

        Protected Overrides Sub Execute()

            EPG.Banner.Navigate()
            If _isLock Then
                EPG.Banner.LockChannel()
            Else
                EPG.Banner.UnLockChannel()
            End If
        End Sub

    End Class

End Namespace
