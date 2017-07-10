Imports System.Runtime.InteropServices
Imports IEX.ElementaryActions.EPG

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class STBSettings
    Protected _iex As IEXGateway.IEX
    Protected _Manager As IEX.ElementaryActions.Functionality.Manager

    Sub New(ByVal pIEX As IEXGateway.IEX, ByVal Manager As IEX.ElementaryActions.Manager)
        _iex = pIEX
        _Manager = Manager
    End Sub

    ''' <summary>
    '''   Sets Banner Display Time On Stb Settings
    ''' </summary>
    ''' <param name="SaveRecordings"> Save or delete existing recordings while factory reset</param>
    ''' <param name="KeepCurrentSettings"> Keep or revert the settings while factory reset</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>370 - FactoryResetFailure</para> 
    ''' </remarks>
    Public Function FactoryReset(ByVal SaveRecordings As Boolean, ByVal KeepCurrentSettings As Boolean, ByVal PinCode As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("FactoryReset", SaveRecordings, KeepCurrentSettings, PinCode, _Manager)
    End Function

    ''' <summary>
    '''   Sets Banner Display Time On Stb Settings
    ''' </summary>
    ''' <param name="DisplayInSec">Banner Display In Seconds : 5 , 7 or 10</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SetBannerDisplayTime(ByVal DisplayInSec As EnumChannelBarTimeout) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetBannerDisplayTime", DisplayInSec, _Manager)
    End Function

    ''' <summary>
    '''   Sets Start/End Guard Time On Stb Settings
    ''' </summary>
    ''' <param name="IsStart">If True Sets START Else Sets End</param>
    ''' <param name="GTCurrentVal">The Current Value Expected On Guard Time</param>
    ''' <param name="GTValueToSet">If GTCurrent Is NOT_AVAILABLE Sets The Value</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Overloads Function SetGuardTime(ByVal IsStart As Boolean, ByVal GTCurrentVal As EnumGuardTime, ByVal GTValueToSet As EnumGuardTime) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetGuardTime", IsStart, GTCurrentVal, GTValueToSet, _Manager)
    End Function

    ''' <summary>
    ''' Sets Start/End Guard Time On STB Settings
    ''' </summary>
    ''' <param name="isStartToBeSet">If True sets Start Guard Time else sets End Guard Time</param>
    ''' <param name="valueToBeSet">The value to be set in string</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    '''  Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Overloads Function SetGuardTime(ByVal isStartToBeSet As Boolean, ByVal valueToBeSet As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetSgtEgt", isStartToBeSet, valueToBeSet, _Manager)
    End Function

    ''' <summary>
    '''  Sets Subtitles On Stb Settings
    ''' </summary>
    ''' <param name="AreSubtitlesOn">If True Sets To ON Else To OFF</param>
    ''' <param name="LanguageToSet">If Empty Default Else Sets Language</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SetSubtitlesPrefs(ByVal AreSubtitlesOn As Boolean, ByVal LanguageToSet As EnumLanguage) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetSubtitlesPrefs", AreSubtitlesOn, LanguageToSet, _Manager)
    End Function

    ''' <summary>
    '''  Sets Reminder Notification
    ''' </summary>
    ''' <param name="SetRemindersOn">If True Sets To ON Else To OFF</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SetReminderNotifications(ByVal SetRemindersOn As Boolean) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetReminderNotifications", SetRemindersOn, _Manager)
    End Function

    ''' <summary>
    '''   Sets Skip Forward Interval On Stb Settings
    ''' </summary>
    ''' <param name="SkipIntervalInSec">Optional Parameter Default = BOOKMARKMODE : Skip Interval Can Be : BOOKMARKMODE,10,30,60,300,600</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SetSkipForwardInterval(Optional ByVal SkipIntervalInSec As EnumVideoSkip = EnumVideoSkip.BOOKMARKMODE) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetSkipInterval", SkipIntervalInSec, True, _Manager)
    End Function

    ''' <summary>
    '''   Sets Skip Backward Interval On Stb Settings
    ''' </summary>
    ''' <param name="SkipIntervalInSec">Optional Parameter Default = BOOKMARKMODE : Skip Interval Can Be : BOOKMARKMODE,10,30,60,300,600</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SetSkipBackwardInterval(Optional ByVal SkipIntervalInSec As EnumVideoSkip = EnumVideoSkip.BOOKMARKMODE) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetSkipInterval", SkipIntervalInSec, False, _Manager)
    End Function

    ''' <summary>
    '''   Sets Parental Control Age Limit Settings
    ''' </summary>
    ''' <param name="Age">Age Can Be : 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,16 18,G,PG,AP 12,Off</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function SetParentalControlAgeLimit(ByVal Age As EnumParentalControlAge) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetParentalControlAgeLimit", Age, _Manager)
    End Function

    ''' <summary>
    '''   Activate/deactivate purchase protection
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function SetPurchaseProtection(ByVal enable As Boolean) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetPurchaseProtection", enable, _Manager)
    End Function

    ''' <summary>
    '''   Set preferred audio language 
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function SetPreferredAudioLanguage(ByVal language As EnumLanguage) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetPreferredAudioLanguage", language, _Manager)
    End Function

    ''' <summary>
    '''   Locking Channel From Parental Control
    ''' </summary>
    ''' <param name="ChannelName">Requested Channel Name</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function SetLockChannel(ByVal ChannelName As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetLockChannel", ChannelName, -1, _Manager)
    End Function

    ''' <summary>
    '''   Locking Channel From Parental Control
    ''' </summary>
    ''' <param name="ChannelNumber">Requested Channel Number</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function SetLockChannel(ByVal ChannelNumber As Integer) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetLockChannel", "", ChannelNumber, _Manager)

    End Function

    ''' <summary>
    '''   UnLocking Channel From Parental Control
    ''' </summary>
    ''' <param name="ChannelName">Requested Channel Name</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function SetUnLockChannel(ByVal ChannelName As String, Optional ByVal UnLockAll As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetUnLockChannel", ChannelName, -1, UnLockAll, _Manager)
    End Function

    ''' <summary>
    '''   UnLocking Channel From Parental Control
    ''' </summary>
    ''' <param name="ChannelNumber">Requested Channel Name</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function SetUnLockChannel(ByVal ChannelNumber As Integer, Optional ByVal UnLockAll As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetUnLockChannel", "", ChannelNumber, UnLockAll, _Manager)
    End Function

    ''' <summary>
    '''   Sets Series Link Channel Configuration
    ''' </summary>
    ''' <param name="ChannelSelection">EnumChannelSelection : From_Single_Channel or From_All_Channels</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' </remarks>
    Public Function SetSeriesLinkChannels(ByVal ChannelSelection As EnumChannelSelection) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetSeriesLinkChannels", ChannelSelection, _Manager)
    End Function
    ''' <summary>
    '''   Remove all channels from List
    ''' </summary>
    '''
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function UnsetAllFavChannels() As IEXGateway._IEXResult
        Return _Manager.Invoke("UnsetAllFavChannels", _Manager)

    End Function
    ''' <summary>
    '''   Setting 1 Or More Channel(s) As Favorite
    ''' </summary>
    ''' <param name="ChannelNameList">Requested Channel Name(s) - If Several Channel Names Then Use Comma As Separator</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetFavoriteChannelNameList(ByVal ChannelNameList As String, ByVal FavouriteIn As EnumFavouriteIn) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetUnsetFavoriteChannelNameList", ChannelNameList, FavouriteIn, True, _Manager)
    End Function

    ''' <summary>
    '''   Removing 1 Channel Or More From Favorites
    ''' </summary>
    ''' <param name="ChannelNameList">Requested Channel Name(s) - If Several Channel Names Then Use Comma As Separator</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function UnsetFavoriteChannelNameList(ByVal ChannelNameList As String, ByVal FavouriteIn As EnumFavouriteIn) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetUnsetFavoriteChannelNameList", ChannelNameList, FavouriteIn, False, _Manager)
    End Function

    ''' <summary>
    '''   Setting 1 Or More Channel(s) As Favorite
    ''' </summary>
    ''' <param name="ChannelNumList">Requested Channel Number(s) - If Several Channel Numbers Then Use Comma As Separator</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetFavoriteChannelNumList(ByVal ChannelNumList As String, ByVal FavouriteIn As EnumFavouriteIn) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetUnsetFavoriteChannelNumList", ChannelNumList, FavouriteIn, True, _Manager)
    End Function

    ''' <summary>
    '''   Removing 1 Channel Or More From Favorites
    ''' </summary>
    ''' <param name="ChannelNumList">Requested Channel Number(s) - If Several Channel Numbers Then Use Comma As Separator</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function UnsetFavoriteChannelNumList(ByVal ChannelNumList As String, ByVal FavouriteIn As EnumFavouriteIn) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetUnsetFavoriteChannelNumList", ChannelNumList, FavouriteIn, False, _Manager)
    End Function

    ''' <summary>
    '''   Setting The TV Guide Background As Solid
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetTvGuideBackgroundAsSolid() As IEXGateway._IEXResult
        Return _Manager.Invoke("SetTvGuideBackground", EnumTvGuideBackground.SOLID, _Manager)
    End Function

    ''' <summary>
    '''   Setting The TV Guide Background As Transparent
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetTvGuideBackgroundAsTransparent() As IEXGateway._IEXResult
        Return _Manager.Invoke("SetTvGuideBackground", EnumTvGuideBackground.TRANSPARENT, _Manager)
    End Function

    ''' <summary>
    ''' Changing the Pin code to newPin 
    ''' </summary>
    ''' <param name="oldPin"></param>
    ''' <param name="newPin"></param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function ChangePinCode(ByVal oldPin As String, ByVal newPin As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("ChangePinCode", oldPin, newPin, _Manager)
    End Function
    ''' <summary>
    '''   Setting the Power Mode Usage
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetPowerMode(ByVal PowerModeOption As String, Optional ByVal warningMessage As String = "SELECT") As IEXGateway._IEXResult
        Return _Manager.Invoke("SetPowerMode", PowerModeOption, warningMessage, _Manager)
    End Function
    ''' <summary>
    '''   Setting the Power Mode Usage
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetAutoStandBy(ByVal AutoStandByOption As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetAutoStandBy", AutoStandByOption, _Manager)
    End Function
    ''' <summary>
    '''   Activating Auto Stand By After Time
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function ActivateAutoStandByAfterTime(ByVal time As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("ActivateAutoStandByAfterTime", time, _Manager)
    End Function
    ''' <summary>
    '''  Verify Power Mode
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function VerifyPowerMode(ByVal powerMode As String, ByVal jobPresent As Boolean, Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "", Optional ByVal currEPGTime As String = "", Optional ByVal isWakeUp As Boolean = True, Optional ByVal isStandBy As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyPowerMode", powerMode, jobPresent, StartTime, EndTime, currEPGTime, isWakeUp, isStandBy, _Manager)
    End Function
    ''' <summary>
    '''  Set Night Time
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' </remarks>
    Public Function SetNightTime(ByVal startTime As String, ByVal endTime As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetNightTime", startTime, endTime, _Manager)
    End Function
End Class
