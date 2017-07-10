Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.VGW.UI
    Dim _utils As IEX.ElementaryActions.EPG.SF.VGW.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _utils = UI.Utils
    End Sub

End Class
