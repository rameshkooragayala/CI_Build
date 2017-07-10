Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Setting/Unsetting 1 Or More Channel(s) As/From Favorite
    ''' </summary>
    Public Class UnsetAllFavChannels
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumList As String
        Private _SetAsFavorite As Boolean
        Private _FavouriteIn As EnumFavouriteIn
        Dim favouriteKey As String = ""
        Private Shadows _Utils As EPG.SF.Utils

        '''''' <param name="pManager">Manager</param>
       
        '''''' <remarks>
        '''''' <para>Remove ALl Favourites</param>
        ''''' <para>Check faourite channels from set favourite and unset the channels using UnsetfavouritechannelNumlist EA</param>

        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para>
        ''' <para>332 - NoValidParameters</para>
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
          
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            EPG.Favorites.RemoveAllFavourites()

        End Sub

    End Class
End Namespace

