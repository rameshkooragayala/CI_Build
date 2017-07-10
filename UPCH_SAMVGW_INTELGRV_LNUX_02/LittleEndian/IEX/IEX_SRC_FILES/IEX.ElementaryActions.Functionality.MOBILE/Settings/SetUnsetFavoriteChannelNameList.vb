Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Setting/Unsetting 1 Or More Channel(s) As/From Favorite
    ''' </summary>
    Public Class SetUnsetFavoriteChannelNameList
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNameList As String
        Private _SetAsFavorite As Boolean

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
        Sub New(ByVal ChannelNameList As String, ByVal setAsFavorite As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNameList = ChannelNameList
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

            EPG.Favorites.Navigate()

            If _SetAsFavorite Then
                EPG.Favorites.SetChannelListAsFavorites(ListChannelName)
            Else
                EPG.Favorites.UnsetChannelListFromFavorites(ListChannelName)
            End If
        End Sub

    End Class

End Namespace