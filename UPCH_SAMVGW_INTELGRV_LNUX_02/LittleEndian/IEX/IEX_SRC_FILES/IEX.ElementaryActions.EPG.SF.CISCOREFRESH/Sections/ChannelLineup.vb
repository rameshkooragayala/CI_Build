﻿Public Class ChannelLineup
    Inherits IEX.ElementaryActions.EPG.SF.UPC.ChannelLineup

    Dim _UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub
End Class
