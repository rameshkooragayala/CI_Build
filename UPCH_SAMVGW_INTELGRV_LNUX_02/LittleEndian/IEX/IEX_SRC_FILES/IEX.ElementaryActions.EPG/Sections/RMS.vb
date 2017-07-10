Imports OpenQA.Selenium.Firefox


Public Class RMS
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub


#Region "Rms"

    Overridable Sub RmsLoginValidation(ByVal driver As FirefoxDriver)
        _iex.LogComment("UI.RmsLogin function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub EnterCpeId(ByVal driver As FirefoxDriver, ByVal cpeId As String)
        _iex.LogComment("UI.RmsLogin function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SelectTab(ByVal driver As FirefoxDriver, ByVal BrowserTabControlId As String, Optional ByVal FindElementby As EnumFindElementBy = EnumFindElementBy.Xpath)
        _iex.LogComment("UI.Get Parameter function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
     Overridable Sub QuickActionSelect(ByVal driver As FirefoxDriver, ByVal quickActionControlId As String, ByVal quickActionConfirmId As String)
        _iex.LogComment("UI.Get Parameter function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Function GetParameterValues(ByVal driver As FirefoxDriver, ByVal paramId As String) As String
        _iex.LogComment("UI.Get Parameter function isn't implemented under the general implementation. Please implement locally in project.")
    End Function
	 Overridable Function SetParameterValues(ByVal driver As FirefoxDriver, ByVal setParameterPath As String, ByVal applyButtonPath As String, ByVal divTabName As String, ByVal sendValue As String) As Boolean
        _iex.LogComment("UI.Get Parameter function isn't implemented under the general implementation. Please implement locally in project.")
    End Function
	
#End Region





End Class
