Imports FailuresHandler

Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.SF.UPC.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.IPC.UI

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
            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _uUI.Utils.ClearEPGInfo()

            If _uUI.Menu.IsLibraryNoContent Then
                _uUI.Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY PLANNER")
            Else
                _uUI.Utils.EPG_Milestones_Navigate("MY PLANNER")
                _uUI.Utils.LogCommentWarning("WORKAROUND For Event Name Problem on Milestones")
                _iex.Wait(6)
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class

