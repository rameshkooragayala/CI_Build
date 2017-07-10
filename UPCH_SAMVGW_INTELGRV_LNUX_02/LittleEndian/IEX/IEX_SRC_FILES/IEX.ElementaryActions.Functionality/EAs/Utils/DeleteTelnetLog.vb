Imports System.IO

Namespace EAImplementation
    ''' <summary>
    '''   Delete TelnetLog.txt From BuildWinPath
    ''' </summary>
    Public Class DeleteTelnetLog
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim IEX As IEXGateway.IEX

        ''' <param name="pManager">Manager</param>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            EPG.Utils.LogCommentInfo("Deleting Telnet Log From The STB")

            Me._manager.TelnetLogIn(False)
            Dim cmd As String
            Dim Data As String = ""

            cmd = "rm -rf /NDS/Diag_TelnetLog.txt"
            Me._manager.SendCmd(cmd, False)
            EPG.Utils.LogCommentInfo("Telnet - Recieved : " + Data)
            Me._manager.TelnetDisconnect()
            EPG.Utils.LogCommentInfo("Telnet - PASSED")

        End Sub

    End Class

End Namespace