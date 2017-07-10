Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Utils

    Dim _UI As IEX.ElementaryActions.EPG.SF.VGW.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

  
End Class


