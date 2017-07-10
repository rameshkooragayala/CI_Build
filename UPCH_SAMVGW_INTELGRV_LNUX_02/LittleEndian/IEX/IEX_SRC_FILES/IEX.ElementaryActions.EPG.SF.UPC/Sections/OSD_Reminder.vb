Public Class OSD_Reminder
    Inherits IEX.ElementaryActions.EPG.SF.OSD_Reminder

    Dim _UI As IEX.ElementaryActions.EPG.SF.UPC.UI
    Dim res As IEXGateway._IEXResult

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

End Class
