Public Class Interactive
    Dim res As IEXGateway.IEXResult
    Protected _iex As IEXGateway.IEX
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

#End Region

#Region "Set Subs"

#End Region

#Region "Verify Subs"

    Overridable Sub VerifyInteractive()
        _iex.LogComment("This UI._Interactive function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate()
        _iex.LogComment("This UI._Interactive function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ExitInteractive()
        _iex.LogComment("This UI._Interactive function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

End Class
