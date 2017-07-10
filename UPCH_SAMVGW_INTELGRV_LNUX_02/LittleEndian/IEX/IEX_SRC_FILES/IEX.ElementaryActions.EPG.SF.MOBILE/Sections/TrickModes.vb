Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.TrickModes

    Dim res As IEXGateway._IEXResult
    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

End Class
