Public Class Banner
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

    Enum BannerType
        Live = 1
        PB = 2
        Radio = 3
    End Enum

#Region "Properties"

    Public Overridable Property EpgTime As String
        Get
            _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
            Return ""
        End Get
        Set(ByVal value As String)
            _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
        End Set
    End Property

#End Region

#Region "Get Subs"

    Overridable Sub GetEpgTime(ByRef time As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEpgTime(ByRef epgTime As DateTime)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetChannelNumber(ByRef channelNumber As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetChannelName(ByRef cName As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventStartTime(ByRef startTime As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventTimePassed(ByRef timePassed As Integer)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventEndTime(ByRef endTime As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventName(ByRef eName As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetBannerType(ByRef type As BannerType)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetEventTimeLeft(ByRef timeLeft As Integer)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Set Subs"

    Overridable Sub SetReminder()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CancelReminder()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PurchaseEvent(enterParentalControlPin As Boolean, _
                                  enterPin As Boolean, _
                                  Optional finishPurchase As Boolean = True, _
                                  Optional pricePound As String = "", _
                                  Optional eventName As String = "")
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PreRecordEvent(Optional isSeries As Boolean = False)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RecordEvent()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RecordEvent(isCurrent As Boolean, isResuming As Boolean, isConflict As Boolean, Optional isPastEvent As Boolean = False, Optional isSeriesEvent As Boolean = False)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PauseEvent()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub StopRecording()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CancelBooking(Optional ByVal isSeriesEvent As Boolean = False, Optional ByVal isComplete As Boolean = False)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetEventKeep()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RemoveEventKeep()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetSubtitlesLanguage(ByVal language As String, Optional ByVal expState As String = "LIVE")
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetAudioTrack(audioTrack As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub AddToFavourites()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RemoveFromFavourites()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RentPpvEvent(isCurrent As Boolean)
        _iex.LogComment("This UI.Guide function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub RefreshEITOnActionBar()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Sub CheckForBanner(banner As String, Optional isPresent As Boolean = True)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CheckForBanner(Optional isPresent As Boolean = True)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub IsRecording()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub IsRecording(numOfPresses As Integer)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function IsLocked() As Boolean
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsFavorite() As Boolean
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsActionBar() As Boolean
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Public Overridable Sub VerifyEventName(expectedText As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyChannelNumber(number As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyChannelNumber(dvt As String, column As String, number As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyEventInfoInBanner(expectedText As String, Optional dvt As String = "FirstEventName")
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CheckBannerType(type As BannerType)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Overloads Sub Navigate(Optional fromPlayback As Boolean = False)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Overloads Sub LaunchActionBar(screen As String, dvt As String)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SendIRLaunchActionBar()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PreviousEvent(Optional repeat As Integer = 1)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextEvent(Optional numOfPresses As Integer = 1, Optional navigate As Boolean = True, Optional selectEvent As Boolean = True)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextChannel(Optional numOfPresses As Integer = 1)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub PreviousChannel(Optional numOfPresses As Integer = 1)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToChannelList()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SelectItem()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToEventInfo()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Offline Settings Subs"

    Overridable Sub BeginOfflineBanner()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub EndOfflineBanner()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Mobile"
    Overridable Sub LockChannel(Optional enterPin As Boolean = True)
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub UnlockChannel()
        _iex.LogComment("UI.Banner function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class

