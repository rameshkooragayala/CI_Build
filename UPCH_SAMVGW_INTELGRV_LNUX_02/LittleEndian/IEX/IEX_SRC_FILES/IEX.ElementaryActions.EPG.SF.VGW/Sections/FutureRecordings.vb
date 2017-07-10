Imports FailuresHandler
Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.VGW.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

End Class

