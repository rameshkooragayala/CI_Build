Imports FailuresHandler

Public Class TrickModes
    Inherits IEX.ElementaryActions.EPG.SF.UPC.TrickModes

    Dim res As IEXGateway._IEXResult
    Dim _UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub
End Class
