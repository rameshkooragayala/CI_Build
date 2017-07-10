Public Class Osd
    Dim _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

#End Region

#Region "Set Subs"

    Overridable Function ConfirmOsd() As Boolean
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function CancelOsd() As Boolean
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

#End Region

#Region "Verify Subs"

    Overridable Function VerifyIsOsd() As Boolean
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function VerifyTextOnOsd(text As String) As Boolean
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function VerifyNumberOsd() As Boolean
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

#End Region

#Region "Navigation Subs"

#End Region

End Class
