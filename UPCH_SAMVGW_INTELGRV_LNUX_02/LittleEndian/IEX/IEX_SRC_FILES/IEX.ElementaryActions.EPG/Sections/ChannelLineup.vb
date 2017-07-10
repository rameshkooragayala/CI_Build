Public Class ChannelLineup
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(pIex As IEXGateway.IEX, pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

#Region "Get Subs"

    Overridable Function GetSurfKey(Optional surfUp As Boolean = True) As String
        _iex.LogComment("This UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

    Overridable Function GetChannelSurfTimeout() As Integer
        _iex.LogComment("This UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
        Return Nothing
    End Function

#End Region

#Region "Set Subs"

    Overridable Sub SurfChannelUp()
        _iex.LogComment("UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SurfChannelDown()
        _iex.LogComment("UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SelectChannel()
        _iex.LogComment("UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

#Region "Verify Subs"

    Overridable Function IsChannelLineUp() As Boolean
        _iex.LogComment("UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

#End Region

#Region "Navigation Subs"

    Overridable Sub Navigate()
        _iex.LogComment("UI.ChannelLineup function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class

