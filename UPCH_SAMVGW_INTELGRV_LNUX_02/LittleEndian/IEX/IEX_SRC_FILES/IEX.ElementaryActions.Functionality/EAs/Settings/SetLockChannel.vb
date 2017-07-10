Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Locking Channel From Parental Control
    ''' </summary>
    Public Class SetLockChannel
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
        ''' <para>322 - VerificationFailure</para> 
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
            Dim Res As IEXGateway._IEXResult
            Dim isPinEntryRequired As Boolean
            Dim projSectionForLockedScreen As String = "LOCKED_CHANNELS_SCREEN"
            Dim projKeyForPinRequired As String = "IS_PIN_REQUIRED"

            EPG.Settings.NavigateToParentalControlLockUnlock()

            'Check if pin entry is required from project configuration file
            Try
                Dim isPinRequired As String = EPG.Utils.GetValueFromProject(projSectionForLockedScreen, projKeyForPinRequired)
                isPinEntryRequired = CBool(isPinRequired)
            Catch
                EPG.Utils.LogCommentWarning("The key " + projKeyForPinRequired + " is not defined in the section " + projSectionForLockedScreen + " in the project configuration file. Taking default value instead!")
                isPinEntryRequired = _manager.Project.IsEPGLikeCogeco
            End Try

            If isPinEntryRequired Then
                Res = Me._manager.EnterDeafultPIN("LOCKED CHANNELS")
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(Res.FailureCode, Res.FailureReason))
                End If
            End If

            If _ChannelNumber <> -1 Then
                EPG.Settings.NavigateToChannel(Me._ChannelNumber)
            Else
                EPG.Settings.NavigateToChannel(Me._ChannelName)
            End If

            EPG.Settings.LockChannel()

        End Sub

    End Class

End Namespace