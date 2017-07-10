Public Class VOD
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Functions"

    Overridable Sub GetEventName(ByRef eventName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetListName(ByRef listName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub GetSelectedEventName(ByRef eventName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
	
	 
	

    Overridable Function GetAssetTitle() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetAssetPrice() As Double
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function

    Overridable Function GetAssetDuration() As Integer
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function

    Overridable Function GetAssetRentalDuration() As Integer
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function

    Overridable Function GetAssetRemainingRentalDuration() As Integer
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function

    Overridable Function GetAssetGenre() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetAssetSynopsis() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetAssetBroadcastDateTime() As Date
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function

    Overridable Function GetAssetSubscriptionStatus() As EnumSubscriptionStatus
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function
	
	Overridable Function GetAssetRentalStatus() As EnumRentalStatus
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Function

    Overridable Function GetAssetDirector() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetAssetCast() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetAssetYearOfProduction() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetAssetStarRating() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function

    Overridable Function GetCurrentClassification() As String
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
        Return ""
    End Function
	

#End Region

#Region "Set Functions"

    Overridable Sub NavigateToEventName(eventName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToListName(listName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub DoSelect()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

#End Region

#Region "Verify Functions"

    Overridable Sub VerifyEventName(eventName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub VerifyListName(listName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyPlayAsset(Optional assetListName As String = "", Optional assetName As String = "")
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyPlayTrailer(Optional assetListName As String = "", Optional assetName As String = "")
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyPlayTrailerEnded()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub VerifyPlayAssetEnded()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToDownload()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToBooked()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextLetter()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NextAsset(Optional numOfPresses As Integer = 1)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToCatchUp()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NavigateToPurchasedAssets()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NavigateToAsset(ByVal vodAsset As VODAsset, Optional ByVal doSelect As Boolean = True)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NextPurchasedAsset()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NextCategory()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NextSeason()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub NextEpisode()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub SelectPurchasedAsset(ByVal assetTitle As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub
	
	Overridable Sub DisplayExtendedInfo()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

	Overridable Sub HighLightAndCollectVODData(ByVal CatalogueName As String)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub PlayModule()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TrickSpeed()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Download"

    Overridable Sub Download()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Play Functions"

    Overridable Sub PlayTrailer(Optional ByVal parentalControl As Boolean = False)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ResumePlay()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub Play(Optional ByVal fromStart As Boolean = True, Optional ByVal parentalControl As Boolean = False, Optional ByVal purchaseProtection As Boolean = True)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub Buy(Optional ByVal purchaseProtection As Boolean = True, Optional ByVal parentalControl As Boolean = False)
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub StopPlayback()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub Subscribe()
        _iex.LogComment("This UI.VOD function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

#End Region

End Class
