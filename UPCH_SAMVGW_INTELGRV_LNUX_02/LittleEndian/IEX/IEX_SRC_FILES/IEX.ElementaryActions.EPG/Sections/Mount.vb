Public Class Mount
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Public Overridable Function GetMountCommand(IsFormat As Boolean) As String
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Public Overridable Function GetFlashCommand(ByVal IsGw As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As String
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Public Overridable Function GetCurrentEpgVersion(Optional ByVal IsLastDelivery As Boolean = False) As String
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Public Overridable Function GetVersionFromStb() As String
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Public Overridable Function GetLogName(ByVal IsSerial As Boolean) As String
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

#End Region

#Region "Set Subs"

    Public Overridable Sub BurnImage(Optional ByVal IsGw As Boolean = True, Optional ByVal IsLastDelivery As Boolean = False)
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
	
	Public Overridable Sub CopyBinary()
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function UpdateSTBVersion(ByVal currentVersion As String, ByVal IsGw As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Public Overridable Function InitializeStb(ByRef msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Public Overridable Function SetBaudRate() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Function SendMountCommand(ByVal IsSerial As Boolean, ByVal mountCommand As String, Optional ByVal IsFormat As Boolean = False) As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Sub BeginLogging(ByVal IsSerial As Boolean, ByVal logFileName As String, ByVal loopNum As Integer)
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub CloseLogs()
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub RebootSTB(ByVal withIPC As Boolean)
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function WaitForGWToLoad() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Function WaitForClientToLoad() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Sub WaitForPrompt(ByVal IsNFS As Boolean)
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function WaitForFirstScreen() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Function WaitForLegalDisclaimer() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Function WaitForStandbyPowerScreen() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Function WaitAfterReset() As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Sub PressSelect()
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function HandleFirstScreens(IsGw As Boolean) As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

    Public Overridable Sub HandlePinScreens()
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function TuneToDefaultChannel(ByVal ChNumber As String) As Boolean
        _iex.LogComment("UI.Mount function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

#End Region

End Class