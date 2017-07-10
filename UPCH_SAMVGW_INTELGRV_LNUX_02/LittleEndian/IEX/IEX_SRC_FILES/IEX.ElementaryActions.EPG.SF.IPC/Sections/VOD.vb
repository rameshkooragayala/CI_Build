Public Class VOD
    Inherits IEX.ElementaryActions.EPG.SF.VOD

    Dim _UI As IEX.ElementaryActions.EPG.SF.IPC.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.IPC.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

End Class
