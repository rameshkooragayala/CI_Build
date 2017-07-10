Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   UnLocking Channel From Parental Control
    ''' </summary>
    Public Class SetUnlockChannel
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelName As String
        Private _ChannelNumber As Integer

        ''' <param name="ChannelName">Requested Channel Name</param>
        ''' <param name="ChannelNumber">Requested Channel Number</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>357 - LockUnlockChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal ChannelName As String, ByVal ChannelNumber As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelName = ChannelName
            Me._ChannelNumber = ChannelNumber
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            EPG.Settings.NavigateToParentalControlLockUnlock()

            EPG.Settings.UnLockChannel(_ChannelName)

        End Sub

    End Class

End Namespace