Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Utils

    Dim _UI As EPG.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub
End Class


