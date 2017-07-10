Imports System.Threading.Tasks
Imports FailuresHandler
Imports System.IO
Imports System.Text
Imports System.Xml
Namespace EAImplementation


    Public Class VODDataBase

        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _catalogueName As String

        Sub New(ByVal CatalogueName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._catalogueName = CatalogueName
        End Sub

        Protected Overrides Sub Execute()

            '  Dim firstScreenNavigation As String = EPG.Utils.GetValueFromTestIni("TEST PARAMS", "NAVIGATION PATH")
            '   EPG.Utils.EPG_Milestones_NavigateByName(firstScreenNavigation)
            Try
                EPG.Vod.HighLightAndCollectVODData(_catalogueName)
            Catch ex As EAException
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Failed To Navigate to Requested Path"))
            End Try
        End Sub
    End Class
End Namespace

