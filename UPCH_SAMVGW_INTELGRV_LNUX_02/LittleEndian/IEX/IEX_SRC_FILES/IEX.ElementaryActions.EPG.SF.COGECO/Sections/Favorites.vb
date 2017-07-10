Imports FailuresHandler

Public Class Favorites
    Inherits IEX.ElementaryActions.EPG.SF.Favorites

    Dim _UI As IEX.ElementaryActions.EPG.SF.COGECO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating From Favorite List Of Channels To Confirmation
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub NavigateToConfirmationInFavorites()
        _UI.Utils.SendIR("SELECT_RIGHT") 'Going to Confirm menu
    End Sub

    ''' <summary>
    '''   Confirming Current List Of Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub ConfirmFavoritesList()
        Dim Value As String = ""
        Dim EpgText As String = ""


        _UI.Utils.StartHideFailures("Confirming Current List Of Favorites")

        Try
            _UI.Utils.SendIR("SELECT")

            _UI.Utils.VerifyState("FAVOURITE CHANNELS")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class
