Public Class PlannerBase
    Inherits IEX.ElementaryActions.EPG.SF.PlannerBase

    Dim _uUI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _uUI = UI
    End Sub

    ''' <summary>
    ''' Navigating to Failed Recorded Event
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToFailedEventScreen()

        _uUI.Utils.StartHideFailures("Trying to Navigate to Failed Recorded Event Screen")

        Try

            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _uUI.Utils.EPG_Milestones_Navigate("MANAGE RECORDINGS/HISTORY")

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
