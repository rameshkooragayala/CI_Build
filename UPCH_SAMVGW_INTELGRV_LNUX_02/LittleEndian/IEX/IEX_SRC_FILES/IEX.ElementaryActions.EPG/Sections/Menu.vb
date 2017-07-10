Public Class Menu
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Menu"

    Public Overridable Sub GetMenuAction(ByRef action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub GetMenuChannelName(ByRef channelName As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub GetChannelNumber(ByRef channelNumber As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function IsMenu() As Boolean
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Public Overridable Sub Navigate()
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetMenuAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetMenuSubAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SelectToConflict()
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Settings Menu"


    Public Overridable Sub SetPlannerMenuAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetSettingsMenuAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetSettingsConfirmationAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetSettingsSeconds(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "ActionBar Menu"

    Public Overridable Sub GetActionBarAction(ByRef action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetActionBarAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetActionBarSubAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Library Menu"

    Public Overridable Function IsLibraryNoContent() As Boolean
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Public Overridable Sub MoveUpInLibrary()
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetLibraryNoContent(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SetLibraryMenuAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Guide View Menu"

    Public Overridable Sub SetGuideViewMenuAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "ManualRecording Menu"

    Public Overridable Sub SetManualRecordingMenu(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetManualRecordingAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetManualRecordingChannel(channel As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub SetManualRecordingDate(tDate As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Conflict Menu"

    Overridable Sub SetConflictAction(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Language Menu"
    Overridable Sub SetLanguage(language As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Country Menu"
    Overridable Sub SetCountry(country As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Confirmation Menu"
    Public Overridable Sub SetConfirmationMenu(action As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Mobile Menu"
    Public Overridable Sub TuneToChannel(channelName As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub FindChannel(channelName As String)
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Sub NavigateToTV()
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Public Overridable Function IsTV() As Boolean
        _iex.LogComment("UI.Menu function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

#End Region

End Class
