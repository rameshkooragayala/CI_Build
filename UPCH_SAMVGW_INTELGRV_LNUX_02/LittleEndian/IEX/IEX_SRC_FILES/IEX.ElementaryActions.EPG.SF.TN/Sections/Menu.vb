Imports FailuresHandler

Public Class Menu
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Menu

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

   ''' <summary>
    '''    Navigating To Menu By Pressing Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        _UI.Utils.StartHideFailures("Navigating To Menu")

        Try
            If IsMenu() Then
                Exit Sub
            End If

            _UI.Utils.EPG_Milestones_Navigate("MAIN MENU")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class
