Public Class OTA
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String
    Dim _UI As EPG.UI

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

    Overridable Sub CopyBinary()
        _iex.LogComment("This OTA function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ModifyImageVersion(ByVal VersionID As String)
        _iex.LogComment("This OTA function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub Create_Carousel(ByVal Version_ID As String, ByVal usage_id As String, ByVal _RFFeed As String)
        _iex.LogComment("UI.Settings function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub BroadcastCarousel(ByVal RFFeed As String)
        _iex.LogComment("This OTA function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub NITBraodcast(ByVal NITTable As String)
        _iex.LogComment("This OTA function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub OTADownloadOption(ByVal downloadOption As String, Optional ByVal IsDownload As Boolean = True)
        _iex.LogComment("This OTA function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
    Overridable Sub VerifySoftVersion(ByVal SoftVersion As String, ByVal OldSoftVersion As String)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
End Class
