Public Class EpgDateTime
    Inherits EPG.EpgDateTime

    Dim _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult

    Public Sub New(ByVal _iex As IEXGateway.IEX)
        MyBase.New(_iex)
        _iex = _iex
    End Sub

End Class
