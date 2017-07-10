Imports FailuresHandler

Public Class Favorites
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Favorites

    Dim _UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
    End Sub

End Class