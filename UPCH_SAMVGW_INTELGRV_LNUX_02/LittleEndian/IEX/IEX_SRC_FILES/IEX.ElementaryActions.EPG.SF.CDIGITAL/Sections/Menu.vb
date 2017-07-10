Imports FailuresHandler

Public Class Menu
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Menu

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub
End Class
