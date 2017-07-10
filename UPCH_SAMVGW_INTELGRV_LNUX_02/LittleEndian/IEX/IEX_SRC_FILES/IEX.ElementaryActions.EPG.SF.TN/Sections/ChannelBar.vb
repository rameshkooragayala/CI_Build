Public Class ChannelBar
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.ChannelBar

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To Channel List By Pressing BACK_UP
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _UI.Utils.StartHideFailures("Navigating To Channel Bar")

        Try
            _iex.Wait(3)
            _UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
