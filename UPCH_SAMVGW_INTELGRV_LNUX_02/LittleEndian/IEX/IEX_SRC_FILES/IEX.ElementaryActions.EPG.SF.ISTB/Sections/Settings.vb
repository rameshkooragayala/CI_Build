Public Class Settings
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Settings

    Dim _UI As IEX.ElementaryActions.EPG.SF.ISTB.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

End Class
