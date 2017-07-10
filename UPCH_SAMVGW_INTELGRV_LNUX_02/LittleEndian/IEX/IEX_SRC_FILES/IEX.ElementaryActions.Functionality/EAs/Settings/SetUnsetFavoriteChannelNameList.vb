Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Setting/Unsetting 1 Or More Channel(s) As/From Favorite
    ''' </summary>
    Public Class SetUnsetFavoriteChannelNameList
        Inherits IEX.ElementaryActions.BaseCommand
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNameList As String
        Private _FavouriteIn As EnumFavouriteIn
        Private _SetAsFavorite As Boolean
        Dim favouriteKey As String = ""
        Private Shadows _Utils As EPG.SF.Utils

        ''' <param name="ChannelNameList">Requested Channel Name(s) - If Several Channel Names Then Use Comma As Separator</param>
        ''' <param name="SetAsFavorite">True to Set favorite(s), False to Unset favorite(s)</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para>
        ''' <para>332 - NoValidParameters</para>
        ''' </remarks>
        Sub New(ByVal ChannelNameList As String, ByVal FavouriteIn As EnumFavouriteIn, ByVal setAsFavorite As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNameList = ChannelNameList
            Me._FavouriteIn = FavouriteIn
            Me._SetAsFavorite = setAsFavorite
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            'Parameters Validation
            If _ChannelNameList.Trim = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Name List Cannot Be Empty"))
            End If

            Dim ListChannelName As New List(Of String)(_ChannelNameList.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries))

            'Trim All To Eliminate Any Superfluous Space(s) In Input
            For i As Integer = 0 To ListChannelName.Count - 1
                ListChannelName.Item(i) = ListChannelName.Item(i).Trim()
            Next

            Select Case _FavouriteIn
                Case EnumFavouriteIn.Settings
                    'Navigate to Settings
                    EPG.Favorites.Navigate()
                    If _SetAsFavorite Then
                        EPG.Favorites.SetChannelListAsFavorites(ListChannelName)
                    Else
                        EPG.Favorites.UnsetChannelListFromFavorites(ListChannelName)
                    End If
					
                    EPG.Favorites.NavigateToConfirmationInFavorites()
                    Dim title As String = ""
                    EPG.Utils.GetEpgInfo("title", title)
                    If (title.ToUpper() <> "CLEAR LIST") Then
                        EPG.Favorites.ConfirmFavoritesListName(ListChannelName, _SetAsFavorite)
                    End If
					
                    'EPG.Favorites.NavigateToConfirmationInFavorites()
                    ' EPG.Favorites.ConfirmFavoritesList()
                    'EPG.Favorites.ConfirmFavoritesListName(ListChannelName, _SetAsFavorite)
            End Select

            Select Case _FavouriteIn
                Case EnumFavouriteIn.ActionBar
                    'Tune to channel
                    Dim ChannelName = _ChannelNameList
                    Dim targetCh As New Service
                    'Tune to channel
                    targetCh = Me._manager.GetServiceFromContentXML("Name=" + ChannelName)
                    Dim ChannelNum = targetCh.LCN

                    'Tune to the service
                    EPG.Live.TuningToChannel(ChannelNum)
                    'verify the service in tuned
                    EPG.Live.VerifyChannelNumber(ChannelNum)

                    'Navigate to ActionBar
                    EPG.Banner.Navigate()
                    'Verify favourite is set from Action bar
                    If _SetAsFavorite Then
                        EPG.Favorites.SetFavoriteFromAction()
                    Else
                        EPG.Favorites.UnsetFavoriteFromAction()
                    End If
            End Select

            Select Case _FavouriteIn
                Case EnumFavouriteIn.Guide
                    Dim ChannelName = _ChannelNameList
                    Dim targetCh As New Service
                    'Tune to channel
                    targetCh = Me._manager.GetServiceFromContentXML("Name=" + ChannelName)
                    Dim ChannelNum = targetCh.LCN

                    'Tune to the service
                    EPG.Live.TuningToChannel(ChannelNum)
                    'verify the service in tuned
                    EPG.Live.VerifyChannelNumber(ChannelNum)
                    'Navigate to action bar on guide
                    EPG.Guide.Navigate()
                    'Launch Action bar
                    EPG.Utils.SendIR("SELECT", 500)
                    'Verify favourite is set from guide
                    If _SetAsFavorite Then
                        EPG.Favorites.SetFavoriteFromAction()
                    Else
                        EPG.Favorites.UnsetFavoriteFromAction()
                    End If

            End Select
        End Sub

    End Class

End Namespace