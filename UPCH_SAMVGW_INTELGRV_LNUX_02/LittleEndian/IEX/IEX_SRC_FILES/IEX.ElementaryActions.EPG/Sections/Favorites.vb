Public Class Favorites
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String
    Dim _UI As EPG.UI
    Protected Friend favorite As String = "LockedChannel"
    Protected Friend nonFavorite As String = "UnlockedChannel"

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub


#Region "Navigation Subs"

    Overridable Sub Navigate()
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToAllChannels()
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToFavoritesSetting()
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub NavigateToConfirmationInFavorites()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TuneToChannel(ByVal channel As String)
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Set Subs"
    Overridable Sub RemoveAllFavourites()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetChannelAsFavorite()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub UnsetChannelFromFavorite()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetChannelListAsFavorites(ByVal listChannelName As List(Of String))
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub


    Overridable Sub UnsetChannelListFromFavorites(ByVal listChannelName As List(Of String))
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub


    Overridable Sub SetChannelListAsFavorites(ByVal listChannelNum As List(Of Integer))
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub SetFavoriteFromAction()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub UnsetFavoriteFromAction()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub UnsetChannelListFromFavorites(ByVal listChannelNum As List(Of Integer))
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ConfirmFavoritesListName(ByVal listChannelName As List(Of String), ByVal setAsFavorites As Boolean)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ConfirmFavoritesListNumber(ByVal listChannelNum As List(Of String), ByVal setAsFavorites As Boolean)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ConfirmFavoritesList()
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetChannelList(ByVal listChannelName As List(Of String), ByVal setAsFavorites As Boolean)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetChannelList(ByVal listChannelName As List(Of Integer), ByVal setAsFavorites As Boolean)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SelectChannel()
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub AddChannel()
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Get Subs"

#End Region

#Region "Verify Subs"

    Overridable Function IsFavorite() As Boolean
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyChannelSelect(ByVal channel As String)
        _iex.LogComment("This UI._Favorites function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class
