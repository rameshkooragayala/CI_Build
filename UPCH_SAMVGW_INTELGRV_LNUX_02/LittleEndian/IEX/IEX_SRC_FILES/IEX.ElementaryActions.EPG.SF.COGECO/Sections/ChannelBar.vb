Public Class ChannelBar
    Inherits IEX.ElementaryActions.EPG.SF.ChannelBar

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

End Class
