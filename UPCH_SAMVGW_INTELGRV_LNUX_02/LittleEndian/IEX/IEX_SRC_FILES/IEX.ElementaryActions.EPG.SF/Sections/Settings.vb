Imports FailuresHandler

Public Class Settings
    Inherits IEX.ElementaryActions.EPG.Settings

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Settings From Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        _Utils.StartHideFailures("Navigating To Settings")

        Try

            _Utils.EPG_Milestones_NavigateByName("STATE:SETTINGS")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To Start Guard Time Setting
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToStartGuardTime()

        _Utils.StartHideFailures("Navigating To Start Guard Time Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To End Guard Time Setting
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToEndGuardTime()

        _Utils.StartHideFailures("Navigating To End Guard Time Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Navigating To Specific Setting On Settings List
    ''' </summary>
    ''' <param name="Setting"></param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToSetting(ByVal Setting As String)
        Dim SettingsSubMenuColumn As String = ""

        _Utils.StartHideFailures("Navigating To Setting -> " + Setting.ToString)

        Try
            _UI.Menu.SetSettingsMenuAction(Setting)

            Dim ScrnCtrlr As String = ""

            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("scrnctrl", ScrnCtrlr)

            If ScrnCtrlr <> Setting + "ScrnCtrlr" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Settings scrnctrl Is " + Setting + "ScrnCtrlr"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Sets Setting On Settings List
    ''' </summary>
    ''' <param name="Setting">Requested Setting</param>
    ''' <param name="VerifyState">Requested State To Verify After Select</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetSetting(ByVal Setting As String, ByVal VerifyState As String)

        _Utils.StartHideFailures("Set Setting To " + Setting.ToString)

        Try
            _Utils.EPG_Milestones_SelectMenuItem(Setting)

            Dim TitleBeforeSelect As String = ""
            Dim TitleAfterSelect As String = ""

            _Utils.GetEpgInfo("title", TitleBeforeSelect)

            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("title", TitleAfterSelect)

            If TitleBeforeSelect = TitleAfterSelect Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To Verify Settings Menu Changed"))
            End If

            If VerifyState <> "" Then
                If Not _Utils.VerifyState(VerifyState) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To Verify State " + VerifyState.ToString + " Reached After Select"))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigates And Sets Subtitles Setting
    ''' </summary>
    ''' <param name="IsOn">If True Set Subtitle Setting To On Else Set It To Off</param>
    ''' <param name="LanguageToSet">Which Language To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetSubtitles(ByVal IsOn As Boolean, ByVal LanguageToSet As String)
        Dim EpgText As String = ""
        Dim pass As Boolean = True

        _Utils.StartHideFailures("Setting Subtitles To " + IIf(IsOn, "ON ", "OFF ") + "With " + LanguageToSet + " Language")

        Try
            If IsOn Then
                If LanguageToSet <> "" Then

                    EpgText = _Utils.GetValueFromDictionary("DIC_SETTINGS_LANGUAGE")

                    Try
                        _Utils.EPG_Milestones_NavigateByName("STATE:SUBTITLES SETTING")
                    Catch ex As EAException
                        pass = False
                    End Try
                Else
                    Try
                        _Utils.EPG_Milestones_NavigateByName("STATE:SUBTITLE DISPLAY ON")
                    Catch ex As Exception
                        pass = False
                    End Try
                End If
            Else
                Try
                    _Utils.EPG_Milestones_NavigateByName("STATE:SUBTITLE DISPLAY OFF")
                Catch ex As Exception
                    pass = False
                End Try
            End If

            If pass = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To Set Subtitles To " + IIf(IsOn, "ON ", "OFF ") + IIf(LanguageToSet <> "", "With Language " + EpgText + " ", "")))
            End If

            If LanguageToSet <> "" Then
                SetSetting(LanguageToSet, "")
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigates to settings and set prefered audio language 
    ''' </summary>
    ''' <param name="language">language to set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEPGInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetAudioLanguage(ByVal language As String)

        _Utils.StartHideFailures("Setting audio language to " + language)

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:AUDIO LANGUAGE")

            SetSetting(language, "")

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifies Setting On Settings List
    ''' </summary>
    ''' <param name="Setting"></param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySetting(ByVal Setting As String)
        Dim CurrentSetting As String = ""

        _Utils.StartHideFailures("Verifing Setting Is : " + Setting.ToString)

        Try
            _Utils.GetEpgInfo("title", CurrentSetting)

            If CurrentSetting = Setting Then
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Setting Is : " + Setting.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Get The Setting Value From Dictionary By EnumValue
    ''' </summary>
    ''' <param name="DictionaryKey">The Dictionary Key</param>
    ''' <param name="EnumValue">The Requested Value</param>
    ''' <returns>The Dictionary Value For The Requested Value</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetSettingsValueFromDictionary(ByVal DictionaryKey As String, ByVal EnumValue As String) As String
        Dim EpgText As String = ""
        Dim Items As String() = Nothing

        If EnumValue = "" Then
            Try
                EpgText = _Utils.GetValueFromDictionary(DictionaryKey)
            Catch ex As Exception
                Return ""
            End Try

            Return EpgText
        Else
            Try
                EpgText = _Utils.GetValueFromDictionary(DictionaryKey)
                Items = EpgText.Split(",")
            Catch ex As Exception
                Return ""
            End Try

            Try
                For Each item As String In Items
                    If item.Contains(EnumValue) Then
                        Return item
                    End If
                Next
                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End If
    End Function

    ''' <summary>
    '''   Navigate To Channel In Lock/Unlock Channels
    ''' </summary>
    ''' <param name="ChannelName">The Channel Name Requested</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToChannel(ByVal ChannelName As String)
        Dim CurrentChannel As String = ""
        Dim CheckedChannel As String = "Empty"
        Dim FirstChannel As String = ""
        Dim SameItemTimes As Integer = 3

        _Utils.StartHideFailures("Navigating To Channel -> " + ChannelName)

        Try
            _iex.MilestonesEPG.ClearEPGInfo()

            _Utils.SendIR("SELECT_DOWN")

            _Utils.GetEpgInfo("chname", FirstChannel)

            If FirstChannel = ChannelName Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstChannel = CheckedChannel And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("chname", CurrentChannel)

                _Utils.SendIR("SELECT_UP")

                _Utils.GetEpgInfo("chname", CheckedChannel)

                If CurrentChannel = CheckedChannel Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN CHANNEL SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedChannel = ChannelName Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Settings Channel To : " + ChannelName))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigate To Channel In Lock/Unlock Channels
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel Number Requested</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToChannel(ByVal ChannelNumber As Integer)
        Dim CurrentChannel As String = ""
        Dim CheckedChannel As String = "Empty"
        Dim FirstChannel As String = ""
        Dim SameItemTimes As Integer = 3

        _Utils.StartHideFailures("Navigating To Channel -> " + ChannelNumber.ToString)

        Dim Chnum As String = ""

        Try

            _UI.Utils.TypeKeys(ChannelNumber.ToString)

            _iex.Wait(2)

            _Utils.GetEpgInfo("chnum", Chnum)

            If ChannelNumber.ToString <> Chnum.ToString Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is " + ChannelNumber.ToString))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To Skip Forward/Backward Interval
    ''' </summary>
    ''' <param name="IsForward">If True Set Skip Forward Else Set Skip Backward</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToSkipInterval(ByVal IsForward As Boolean)
        Dim EpgText As String = ""
        Dim pass As Boolean = True

        _Utils.StartHideFailures("Navigating To Skip " + IIf(IsForward, "Forward ", "Backward ") + "Interval")

        Try
            If IsForward Then
                Try
                    _Utils.EPG_Milestones_NavigateByName("STATE:VIDEO SKIP FORWARD")
                Catch ex As Exception
                    pass = False
                End Try
            Else
                Try
                    _Utils.EPG_Milestones_NavigateByName("STATE:VIDEO SKIP BACKWARD")
                Catch ex As Exception
                    pass = False
                End Try
            End If

            If pass = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To Skip " + IIf(IsForward, "Forward ", "Backward ") + "Interval "))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Setting Reminder Notification To ON/OFF
    ''' </summary>
    ''' <param name="IsOn">If True Set Reminder Notification To ON Else To OFF</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>314 - SetSettingsFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetReminderNotifications(ByVal IsOn As Boolean)
        Dim EpgText As String = ""
        Dim pass As Boolean = True

        _Utils.StartHideFailures("Setting Reminder Notification To " + IIf(IsOn, "ON ", "OFF "))

        Try
            If IsOn Then
                Try
                    _Utils.EPG_Milestones_NavigateByName("STATE:REMINDER NOTIFICATION ON")
                Catch ex As Exception
                    pass = False
                End Try
            Else
                Try
                    _Utils.EPG_Milestones_NavigateByName("STATE:REMINDER NOTIFICATION OFF")
                Catch ex As Exception
                    pass = False
                End Try
            End If

            If pass = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To Set Reminder Notification To " + IIf(IsOn, "ON ", "OFF ")))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Parental Control Lock Programmes By Age Rating Settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToParentalControlAgeLimit()
        _Utils.StartHideFailures("Navigating To Parental Control Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:LOCK PROGRAMMES BY AGE RATING")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To Parental Control Lock/Unlock Channels Settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToParentalControlLockUnlock()
        _Utils.StartHideFailures("Navigating To Parental Control Lock/Unlock Channels Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:PARENTAL CONTROL UN/LOCK CHANNELS")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Purchase protection in Settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToPurchaseProtection()
        _Utils.StartHideFailures("Navigating To Purchase Protection Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:PURCHASE PROTECTION")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Banner Display Timeout
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToBannerDisplay()
        _Utils.StartHideFailures("Navigating To Banner Display Timeout")

        Try
            _UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR TIME OUT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To Series Recording Setting
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToSeriesRecording()
        _Utils.StartHideFailures("Navigating To Series Recording Setting")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:SERIES RECORDING")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Menu Language Setting
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToMenuLanguage()
        _Utils.StartHideFailures("Navigating To Menu Language Setting")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:MENU LANGUAGE")
        Finally
            _iex.ForceHideFailure()
        End Try
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
             '   ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockChannelFailure, "Failed To Lock Channel"))
            'End If

            _UI.Utils.SendIR("SELECT_LEFT")

            _UI.Utils.GetEpgInfo("title", Confirm)
            If (Confirm.ToUpper() <> "CLEAR LIST") Then
                _UI.Utils.SendIR("SELECT")
            Else
                _UI.Utils.SendIR("BACK", 5000)
            End If

            'EpgText = _UI.Utils.GetValueFromDictionary("DIC_SETTINGS_CONFIRM_LIST")

            'If Confirm <> EpgText Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            'End If

           ' _UI.Utils.SendIR("BACK", 5000)

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
        Dim value As String = ""
        Dim EpgText As String = ""
        Dim Title As String = ""
        Dim State As String = ""

        _Utils.StartHideFailures("UnLocking Channel")

        Try

            _Utils.SendIR("SELECT_LEFT")

            EpgText = _Utils.GetValueFromDictionary("DIC_RESET_FAV_CHANNELS")

            _Utils.GetEpgInfo("title", value)

            If value <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If

            _Utils.SendIR("SELECT")

            _UI.Utils.GetEpgInfo("title", Title)

            'EpgText = _UI.Utils.GetValueFromDictionary("DIC_SETTINGS_PARENTAL_LOCK_CHANNEL")

            If Title.Contains(EpgText) Then
                _Utils.LogCommentImportant("Unlocked all channels sucessfully")

            else
            _Utils.EnterPin("")

                '_Utils.GetEpgInfo("key", UnLocked)

                'If UnLocked <> "UnlockedChannel" Then
                '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To UnLock Channel"))
                'End If

            End If
            _Utils.GetEpgInfo("title", Title)

            'EpgText = _Utils.GetValueFromDictionary("DIC_SETTINGS_PARENTAL_LOCK_CHANNEL")

            If Title <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockChannelFailure, "Failed To Verify Title Is : " + EpgText))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating And Selecting Restricted Setting And Verify PIN Screen 
    ''' </summary>
    ''' <param name="Setting">The Restricted Setting To Select</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectRestrictedSetting(ByVal Setting As String)
        Dim State As String = ""

        _Utils.StartHideFailures("Navigating To Setting -> " + Setting.ToString)

        Try
            _UI.Menu.SetSettingsMenuAction(Setting)

            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("ENTER PIN") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To Verify State Is ENTER PIN"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To TV Guide Background Settings From Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToTvGuideBackgroundSettings()
        _Utils.StartHideFailures("Navigating To TV Guide Background Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:TV GUIDE BACKGROUND SETTINGS")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Sets TV Guide Background As Solid
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub SetTvGuideBackgroundAsSolid()

        _Utils.StartHideFailures("Set TV Guide Background To Solid")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("SOLID BACKGROUND")
            _Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Sets TV Guide Background As Transparent
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub SetTvGuideBackgroundAsTransparent()

        _Utils.StartHideFailures("Set TV Guide Background To Transparent")

        Try
            _Utils.EPG_Milestones_SelectMenuItem("TRANSPARENT BACKGROUND")
            _Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating And Selecting EPG language as specified 
    ''' </summary>
    ''' <param name="Language">The EPG Language To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetMenuLanguage(ByVal Language As String)
        _Utils.StartHideFailures("Setting Menu Language To : " + Language)

        Try
            _Utils.EPG_Milestones_Navigate(Language)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Get the Guard Time EPG string
    ''' </summary>
    ''' <param name="guardTimeValue">The string value of Guard time</param>
    ''' <returns>The Guard Time EPG String</returns>
    Public Function GetGuardTimeEpgString(guardTimeValue As String) As String

        Dim guardTimeString As String = ""
        _Utils.StartHideFailures("Getting Guard Time String for value - " + guardTimeValue)

        Try
            guardTimeString = GetSettingsValueFromDictionary("DIC_GUARD_TIME_" + guardTimeValue, "")
            If guardTimeString = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_" + guardTimeValue + " Value From Dictionary! Please check your dictionary file."))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

        Return guardTimeString
    End Function

    ''' <summary>
    ''' Handle PowerMode Settings Exception
    ''' </summary>
    ''' <param name="key">The string value to select or cancel</param>
    Public Overrides Sub HandlePowerModeSettingsException(ByVal key As String, ByVal VerifyState As String)
        Dim message As String = ""
        Try
            _Utils.StartHideFailures("Verify for Notification Message ")

            If VerifyState <> "" Then
                If (Not (_Utils.VerifyState("NOTIFICATION MESSAGE", 30))) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed To Get Warning Message"))
                End If
            Else
			
			    _iex.Wait(4)
				
                _Utils.GetEpgInfo("PowerStandByMode", message)

                If (Not (String.IsNullOrEmpty(message))) Then
                    _Utils.SendIR(key)
                Else
                    _Utils.StartHideFailures("Notification Message is Empty ")
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
