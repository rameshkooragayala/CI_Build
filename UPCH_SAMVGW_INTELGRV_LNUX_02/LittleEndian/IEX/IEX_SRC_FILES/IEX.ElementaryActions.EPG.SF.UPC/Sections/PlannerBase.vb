Public Class PlannerBase
    Inherits IEX.ElementaryActions.EPG.SF.PlannerBase

    Dim _uUI As IEX.ElementaryActions.EPG.SF.UPC.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _uUI = UI
    End Sub

    ''' <summary>
    ''' Navigating to Failed Recorded Event
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToFailedEventScreen()

        _uUI.FutureRecordings.Navigate()

    End Sub
End Class
