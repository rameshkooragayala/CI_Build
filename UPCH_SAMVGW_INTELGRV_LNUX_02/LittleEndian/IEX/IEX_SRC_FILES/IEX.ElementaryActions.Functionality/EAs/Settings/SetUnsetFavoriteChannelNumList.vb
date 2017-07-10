Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Setting/Unsetting 1 Or More Channel(s) As/From Favorite
    ''' </summary>
    Public Class SetUnsetFavoriteChannelNumList
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumList As String
        Private _SetAsFavorite As Boolean
        Private _FavouriteIn As EnumFavouriteIn
        Dim favouriteKey As String = ""
        Private Shadows _Utils As EPG.SF.Utils

        ''' <param name="ChannelNumList">Requested Channel Number(s) - If Several Channel Numbers Then Use Comma As Separator</param>
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
        Sub New(ByVal ChannelNumList As String, ByVal FavouriteIn As EnumFavouriteIn, ByVal setAsFavorite As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumList = ChannelNumList
            Me._FavouriteIn = FavouriteIn
            Me._SetAsFavorite = setAsFavorite
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            'Parameters Validation
            If _ChannelNumList.Trim = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Num List Cannot Be Empty"))
            End If

            Dim ListChannelNumAsString As New List(Of String)(_ChannelNumList.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries))
            Dim ListChannelNum As New List(Of Integer)


            'Trim All To Eliminate Any Superfluous Space(s) In Input
            For i As Integer = 0 To ListChannelNumAsString.Count - 1
                ListChannelNum.Add(CInt(ListChannelNumAsString.Item(i).ToString.Trim()))
            Next

            Select Case _FavouriteIn
                Case EnumFavouriteIn.Settings
                    'Navigate to Settings
                    EPG.Favorites.Navigate()
                    If _SetAsFavorite Then
                        EPG.Favorites.SetChannelListAsFavorites(ListChannelNum)
                    Else
                        EPG.Favorites.UnsetChannelListFromFavorites(ListChannelNum)                       
                    End If
                    EPG.Favorites.NavigateToConfirmationInFavorites()
                    Dim title As String = ""
                    EPG.Utils.GetEpgInfo("title", title)
                    If (title.ToUpper() <> "CLEAR LIST") Then
                        EPG.Favorites.ConfirmFavoritesListNumber(ListChannelNumAsString, _SetAsFavorite)
                    End If

                Case EnumFavouriteIn.ActionBar
                    'Tune to channel
                    Dim ChannelNum = Convert.ToInt32(_ChannelNumList)
                    'Tune to channel
                    EPG.Live.TuningToChannel(ChannelNum)
                    EPG.Live.VerifyChannelNumber(ChannelNum)
                    'Navigate to ActionBar
                    EPG.Banner.Navigate()
                    'Verify the favourite is set from action bar
                    If _SetAsFavorite Then
                        EPG.Favorites.SetFavoriteFromAction()
                    Else
                        EPG.Favorites.UnsetFavoriteFromAction()
                    End If

                Case EnumFavouriteIn.Guide
                    'Tune to channel
                    Dim ChannelNum = Convert.ToInt32(_ChannelNumList)
                    'Tune to channel
                    EPG.Live.TuningToChannel(ChannelNum)
                    EPG.Live.VerifyChannelNumber(ChannelNum)
                    'Navigate to Guide
                    EPG.Guide.Navigate()
                    'Launch Action bar
                    EPG.Utils.SendIR("SELECT", 500)
                    'Verify the favourite is set from Guide
                    If _SetAsFavorite Then
                        EPG.Favorites.SetFavoriteFromAction()
                    Else
                        EPG.Favorites.UnsetFavoriteFromAction()
                    End If

            End Select

        End Sub

    End Class

End Namespace