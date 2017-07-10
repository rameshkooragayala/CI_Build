Public Class ChannelLineup
    Inherits IEX.ElementaryActions.EPG.SF.ChannelLineup

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub
End Class
