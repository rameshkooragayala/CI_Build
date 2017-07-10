Public Class Settings
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Function GetSettingsValueFromDictionary(dictionaryKey As String, enumValue As String) As String
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

#End Region

#Region "Set Subs"

    Overridable Sub SetSetting(setting As String, verifyState As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetConfirmation(setting As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetReminderNotifications(isOn As Boolean)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetSubtitles(isOn As Boolean, languageToSet As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetAudioLanguage(ByVal language As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub SelectRestrictedSetting(setting As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub LockChannel(Optional channelName As String = "")
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub UnlockChannel(Optional channelName As String = "")
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetTvGuideBackgroundAsSolid()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetTvGuideBackgroundAsTransparent()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetMenuLanguage(language As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub HandlePowerModeSettingsException(key As String, verifyState As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Sub VerifySetting(setting As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToSetting(setting As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToBannerDisplay()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToStartGuardTime()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToEndGuardTime()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToParentalControlAgeLimit()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToParentalControlLockUnlock()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToPurchaseProtection()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NavigateToChannel(channelName As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToChannel(channelNumber As Integer)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToSkipInterval(isForward As Boolean)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToSeriesRecording()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToTvGuideBackgroundSettings()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToMenuLanguage()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

End Class
