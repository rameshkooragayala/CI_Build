Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI

    Private _Utils As EPG.SF.CISCOREFRESH.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

End Class
