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
        Private _UnLockAll As Boolean

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
        Sub New(ByVal ChannelName As String, ByVal ChannelNumber As Integer, ByVal UnLockAll As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelName = ChannelName
            Me._ChannelNumber = ChannelNumber
            Me._manager = pManager
            Me._UnLockAll = UnLockAll
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            'Flag to check if Pin entry screen is present at entry
            Dim isPinScreenPresent As Boolean = False

            EPG.Settings.NavigateToParentalControlLockUnlock()

            Try
                EPG.Utils.LogCommentInfo("Checking whether Pin entry is required when entering Parental Control screen")
                Dim isPinScreenPresentStr As String = EPG.Utils.GetValueFromProject("CHANNEL_BLOCK", "IS_PIN_SCREEN_ON_UNLOCK_ENTRY")
                isPinScreenPresent = CBool(isPinScreenPresentStr)
            Catch
                EPG.Utils.LogCommentWarning("Value not defined in project.ini. Please define it in case it is different from the default value! Taking default value..")
                isPinScreenPresent = False
            End Try

            If isPinScreenPresent Then
                EPG.Utils.EnterPin("")
            End If

            If Not _UnLockAll Then
                If _ChannelNumber <> -1 Then
                    EPG.Settings.NavigateToChannel(Me._ChannelNumber)
                Else
                    EPG.Settings.NavigateToChannel(Me._ChannelName)
                End If
            End If

            EPG.Settings.UnLockChannel()

        End Sub

    End Class

End Namespace