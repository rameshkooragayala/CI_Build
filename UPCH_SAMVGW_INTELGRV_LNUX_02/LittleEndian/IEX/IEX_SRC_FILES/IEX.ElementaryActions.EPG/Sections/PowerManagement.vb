Public Class PowerManagement
    Protected _iex As IEXGateway.IEX

    Sub New(pIex As IEXGateway.IEX, pUI As IEX.ElementaryActions.EPG.UI)
        _iex = pIex
    End Sub

#Region "Verify Subs"

    Overridable Sub VerifyPmMilesStone(jobPresent As Object, startTime As Object, endTime As Object, currEPGTime As Object)
        _iex.LogComment("This function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function IsPwrMgmtMaintainanceSupported() As Boolean
        _iex.LogComment("This function isn't implemented under the general implementation. Please implement locally in project.")
    End Function

#End Region


#Region "Perle RPC"

    Overridable Sub PerleRpcRestart()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function IsPerleRpc() As Boolean
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub SendCommandToPerleRpc(powerUp As Boolean)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class
