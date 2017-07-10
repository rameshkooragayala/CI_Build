Public Class ParentalRating
    Dim res As IEXGateway.IEXResult
    Protected _iex As IEXGateway.IEX
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

    Overridable Sub Navigate()
        _iex.LogComment("This UI._ParentalRating function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
End Class
