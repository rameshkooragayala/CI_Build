Imports FailuresHandler

Public Class Settings
    Inherits IEX.ElementaryActions.EPG.SF.Settings

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub


    ''' <summary>
    '''   Locking Channel In Parental Control Lock/Unlock Channels
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>357 - LockUnlockChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub LockChannel(Optional ByVal ChannelName As String = "")
        Dim Locked As String = ""
        Dim Confirm As String = ""
        Dim EpgText As String = ""
        Dim Title As String = ""

        _UI.Utils.StartHideFailures("Locking Channel")

        Try
            _UI.Utils.SendIR("SELECT")

            '_UI.Utils.GetEpgInfo("key", Locked)

            'If Locked <> "LockedChannel" Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockChannelFailure, "Failed To Lock Channel"))
            'End If

            _UI.Utils.SendIR("SELECT_RIGHT")

            _UI.Utils.GetEpgInfo("confirm list", Confirm)

            EpgText = _UI.Utils.GetValueFromDictionary("DIC_SETTINGS_CONFIRM_LIST")

            If Confirm <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If

            _UI.Utils.SendIR("SELECT", 5000)

            _UI.Utils.GetEpgInfo("title", Title)

            EpgText = _UI.Utils.GetValueFromDictionary("DIC_SETTINGS_PARENTAL_LOCK_CHANNEL")

            If Title <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Confirm Channel Locked"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   UnLocking Channel In Parental Control Lock/Unlock Channels
    ''' </summary>
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
    Public Overrides Sub UnLockChannel(Optional ByVal ChannelName As String = "")
        Dim UnLocked As String = ""
        Dim Confirm As String = ""
        Dim EpgText As String = ""
        Dim Title As String = ""
        Dim State As String = ""

        _UI.Utils.StartHideFailures("UnLocking Channel")

        Try
            _UI.Utils.SendIR("SELECT")

            '_UI.Utils.EnterPin("")

            '_UI.Utils.GetEpgInfo("key", UnLocked)

            'If UnLocked <> "UnlockedChannel" Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To UnLock Channel"))
            'End If

            _UI.Utils.SendIR("SELECT_RIGHT")

            _UI.Utils.GetEpgInfo("confirm list", Confirm)

            EpgText = _UI.Utils.GetValueFromDictionary("DIC_SETTINGS_CONFIRM_LIST")

            If Confirm <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.GetEpgInfo("title", Title)

            EpgText = _UI.Utils.GetValueFromDictionary("DIC_SETTINGS_PARENTAL_LOCK_CHANNEL")

            If Title <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockChannelFailure, "Failed To Verify Title Is : " + EpgText))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class
