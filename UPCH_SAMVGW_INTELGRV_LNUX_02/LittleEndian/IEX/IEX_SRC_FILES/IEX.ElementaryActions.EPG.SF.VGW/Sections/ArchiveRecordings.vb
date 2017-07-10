Imports System.Runtime.InteropServices
Imports FailuresHandler

Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.VGW.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

End Class