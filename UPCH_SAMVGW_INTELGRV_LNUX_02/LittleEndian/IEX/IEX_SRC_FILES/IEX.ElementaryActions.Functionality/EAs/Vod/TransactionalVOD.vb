Imports System.Threading.Tasks
Imports FailuresHandler
Imports System.IO
Imports System.Text
Imports System.Xml
Namespace EAImplementation
    Public Class TransactionalVOD
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _transaction As Integer

        Sub New(ByVal Transaction As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._transaction = Transaction

        End Sub

        Protected Overrides Sub Execute()
            Dim returnString As String = ""
            Try
                EPG.Utils.LogCommentBlack("Performing Transaction for VOD Asset")

                '***************************** Align in One Direction ***************************************
                'Align in one direction
                Dim keyPress As String = EPG.Utils.GetValueFromProject("VOD", "KEY_PRESS")
                Dim nextKeyPress As String = EPG.Utils.GetValueFromProject("VOD", "NEXT_KEY_PRESS")
                Dim currentHighlight As String = ""
                Dim prevHighlight As String = ""
                Dim nextHighlight As String = ""
                'send key and ensure we are at one extreme end 
                'fetch current EPG highlight value
                EPG.Utils.LogCommentInfo("Fetching the current highlighted EPG menu item")
                EPG.Utils.GetEpgInfo("title", currentHighlight)
                EPG.Utils.LogCommentInfo("Current Highlight is " & currentHighlight)
                'this will allow us to reach one extreme end
                While Not prevHighlight = currentHighlight
                    prevHighlight = currentHighlight
                    EPG.Utils.SendIR(keyPress, 2000)
                    EPG.Utils.GetEpgInfo("title", currentHighlight)
                    EPG.Utils.LogCommentInfo("Current Highlight after key press is " & currentHighlight)

                End While
                EPG.Utils.LogCommentInfo("Reached One Extreme")
                '**********************************************************************************************
                If _transaction = 1 Then

                    'try to perform purchase  for first asset
                    EPG.Utils.SendIR("SELECT", 2000)
                    EPG.Utils.GetEpgInfo("title", currentHighlight)
                    If currentHighlight = "REN MOVIE" Then
                        EPG.Utils.SendIR("SELECT", 2000)
                        _iex.Wait(120)
                        EPG.Utils.SendIR("STOP", 2000)
                    Else
                        EPG.Utils.LogCommentInfo("Unable to Perform purchase for first asset")
                        EPG.Utils.SendIR("RETOUR", 2000)
                        EPG.Utils.GetEpgInfo("title", currentHighlight)

                    End If





                End If
                


            Catch ex As EAException

            End Try
        End Sub
    End Class
End Namespace


