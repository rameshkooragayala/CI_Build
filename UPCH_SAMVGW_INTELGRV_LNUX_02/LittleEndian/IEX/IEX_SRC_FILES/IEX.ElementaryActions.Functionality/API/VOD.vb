Imports System.Runtime.InteropServices
Imports IEX.ElementaryActions.EPG

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class VOD
    Protected _iex As IEXGateway.IEX
    Protected _Manager As IEX.ElementaryActions.Functionality.Manager

    Sub New(ByVal pIEX As IEXGateway.IEX, ByVal Manager As IEX.ElementaryActions.Manager)
        _iex = pIEX
        _Manager = Manager
    End Sub

    ''' <summary>
    '''   Navigate to a VOD asset and select it
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function NavigateToVODAsset(ByVal vodAsset As VODAsset, ByVal doSelect As Boolean) As IEXGateway._IEXResult
        Return _Manager.Invoke("NavigateToVODAsset", vodAsset, doSelect, _Manager)
    End Function

    ''' <summary>
    '''   Navigate to a VOD asset and play it
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function PlayAsset(ByVal vodAsset As VODAsset, _
                              Optional ByVal fromStart As Boolean = True, _
                              Optional ByVal parentalProtection As Boolean = False, _
                              Optional ByVal purchaseProtection As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("PlayAsset", vodAsset, fromStart, parentalProtection, purchaseProtection, _Manager)
    End Function

    ''' <summary>
    '''   Navigate to a VOD asset and play the trailer
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function PlayTrailer(Optional ByVal vodAsset As VODAsset = Nothing, _
                                Optional ByVal parentalProtection As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("PlayTrailer", vodAsset, parentalProtection, _Manager)
    End Function

    ''' <summary>
    '''   Navigate to a VOD asset and buy it
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function BuyAsset(ByVal vodAsset As VODAsset, _
                              Optional ByVal parentalProtection As Boolean = False, _
                              Optional ByVal purchaseProtection As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("BuyAsset", vodAsset, parentalProtection, purchaseProtection, _Manager)
    End Function

    ''' <summary>
    '''   Select an asset in the list of purchased assets
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function SelectPurchasedAsset(ByVal vodAsset As VODAsset) As IEXGateway._IEXResult
        Return _Manager.Invoke("SelectPurchasedAsset", vodAsset, _Manager)
    End Function

    ''' <summary>
    '''   Stop current asset playback
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function StopAssetPlayback() As IEXGateway._IEXResult
        Return _Manager.Invoke("StopAssetPlayback", _Manager)
    End Function

    ''' <summary>
    '''   Verify asset details (in asset action menu)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function VerifyAssetDetails(ByVal vodAsset As VODAsset, ByVal isPurchased As Boolean) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyAssetDetails", vodAsset, isPurchased, _Manager)
    End Function

    ''' <summary>
    '''   Subscribe SVOD asset
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function SubscribeAsset(ByVal vodAsset As VODAsset) As IEXGateway._IEXResult
        Return _Manager.Invoke("SubscribeAsset", vodAsset, _Manager)
    End Function

End Class
