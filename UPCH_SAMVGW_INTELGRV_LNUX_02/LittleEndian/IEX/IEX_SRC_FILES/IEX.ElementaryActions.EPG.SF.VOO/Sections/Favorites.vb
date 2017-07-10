Imports FailuresHandler

Public Class Favorites
    Inherits IEX.ElementaryActions.EPG.SF.Favorites

    Dim _UI As UI
    Private Shadows _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
        _Utils = _UI.Utils
    End Sub


End Class