Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Copies TelnetLog.txt From BuildWinPath To Log Folder
    ''' </summary>
    Public Class CopyLogSerial
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim _FileName As String

        ''' <param name="FileName">Optional Parameter Default = "Diag_TelnetLog.txt" : Filename Of The Log File</param>
        ''' <param name="pManager">Manager</param>
        Sub New(ByVal FileName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            _FileName = FileName
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            CopySerialCOGECO()
        End Sub

        Private Function CopySerialCOGECO() As Boolean
            Dim Res As IEXGateway._IEXResult
            Dim cmd As String = ""

            cmd = "/usb/copylog.sh"

            EPG.Utils.LogCommentInfo("Copying Logs By Sending Command " + cmd)

            _iex.Debug.BeginWaitForMessage("FINISH", 0, 600, IEXGateway.DebugDevice.Serial)
            Res = _iex.Debug.WriteLine(cmd, IEXGateway.DebugDevice.Serial)
            _iex.Debug.EndWaitForMessage("FINISH", 0, "", IEXGateway.DebugDevice.Serial)

            Return True
        End Function
    End Class

End Namespace