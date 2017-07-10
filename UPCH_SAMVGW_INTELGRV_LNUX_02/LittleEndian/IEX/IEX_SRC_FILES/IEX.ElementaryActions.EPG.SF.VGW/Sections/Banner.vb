Imports FailuresHandler
Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.VGW.UI


    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI

    End Sub
End Class
