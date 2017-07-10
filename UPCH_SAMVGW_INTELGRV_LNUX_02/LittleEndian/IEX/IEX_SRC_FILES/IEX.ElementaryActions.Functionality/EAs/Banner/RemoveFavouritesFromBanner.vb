Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Adds to Favourites From Action Bar 
    ''' </summary>
    Public Class RemoveFavouritesFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>322 - VerificationFailure</para> 	  
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            EPG.Banner.Navigate()

            EPG.Banner.RemoveFromFavourites()

        End Sub

    End Class

End Namespace