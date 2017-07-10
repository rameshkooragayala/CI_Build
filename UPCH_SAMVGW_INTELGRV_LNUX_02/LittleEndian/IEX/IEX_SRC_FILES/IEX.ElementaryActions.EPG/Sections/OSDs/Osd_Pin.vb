Public Class OSD_PIN
    Dim _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

    Overridable Function EnterPin(ByVal PIN As String) As Boolean
        _UI.Utils.TypeKeys(PIN)
    End Function

    Overridable Function VerifyIsOSD() As Boolean
        Return _UI.OSD.VerifyIsOSD
    End Function

    Overridable Function VerifyTextOnOsd(ByVal Text As String) As Boolean
        Return _UI.OSD.VerifyTextOnOsd(Text)
    End Function

    Overridable Function VerifyNumberOsd() As Boolean
        Return _UI.OSD.VerifyNumberOsd
    End Function

    Overridable Function ConfirmOsd() As Boolean
        Return _UI.OSD.ConfirmOsd
    End Function

    Overridable Function CancelOsd() As Boolean
        Return _UI.OSD.CancelOsd

    End Function
End Class
