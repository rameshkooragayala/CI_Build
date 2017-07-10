Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

    ''' <summary>
    '''   Navigating To Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        _uUI.Utils.StartHideFailures("Navigating To Planner")

        Try
		    _uUI.Utils.ReturnToLiveViewing()
            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")
            _uUI.Utils.ClearEPGInfo()
            _uUI.Utils.EPG_Milestones_Navigate("MANAGE RECORDINGS")

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class

