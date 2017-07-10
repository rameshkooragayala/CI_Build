Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Copies Core Dump and P*.LOG
    ''' </summary>
    Public Class CopyDiagBinaryLog

        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim _FileName As String

        Dim pLogPath As String = ""
        Dim coreDumpPath As String = ""


        Sub New(ByVal FileName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
            _FileName = FileName
        End Sub

        Protected Overrides Sub Execute()
            Dim cmd As String

            Try
                pLogPath = EPG.Utils.GetValueFromEnvironment("pLogPath")
                coreDumpPath = EPG.Utils.GetValueFromEnvironment("coreDumpPath")
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Values not defined in Enviroment.ini"))
            End Try

            Dim LogPath As String = Path.GetFullPath(_iex.LogFileName)
            EPG.Utils.LogCommentInfo(LogPath)

            'establish telnet session to box
            Me._manager.TelnetLogIn(False, False)

            'Send command to check if core dump file is present or not
            'if present copy it back to /host

            'cp /tmp/core* /host/logs
            'find /dev/ -name 'abc-*'

            cmd = "find " + coreDumpPath + " -name 'core*'"
            EPG.Utils.LogCommentBlack("find command to check if any core dump is present or not" & cmd)
            Try
                Me._manager.SendCmd(cmd, False)
            Catch ex As Exception
                Me._manager.SendCmd(cmd, False)
            End Try
            'cmd is set to null so that it can be used again
            cmd = ""

            'copy to /host/logs by first creating logs directory /logs
            cmd = "mkdir /host/logs"
            Me._manager.SendCmd(cmd, False)

            cmd = ""
            Dim copyCoreDump As String = EPG.Utils.GetValueFromEnvironment("copyCoreDump")
            cmd = copyCoreDump
            Me._manager.SendCmd(cmd, False)

            cmd = ""
            Try
                Me._manager.SendCmd(cmd, False)
            Catch ex As Exception
                Me._manager.SendCmd(cmd, False)
            End Try

            cmd = ""
            cmd = "find " + pLogPath + " -name '*.LOG'"
            EPG.Utils.LogCommentBlack("find command to check if .LOG file present or not" & cmd)
            Try
                Me._manager.SendCmd(cmd, False)
            Catch ex As Exception
                Me._manager.SendCmd(cmd, False)
            End Try

            cmd = ""
            Dim copyPath As String = EPG.Utils.GetValueFromEnvironment("copyPath")
            cmd = "cp " + pLogPath + "*.LOG" + copyPath
            EPG.Utils.LogCommentBlack("copy command to for copying P*.LOG" & cmd)
            Try
                Me._manager.SendCmd(cmd, False)
            Catch ex As Exception
                Me._manager.SendCmd(cmd, False)
            End Try

            'renaming file P*.LOG by appending script name
            cmd = ""
            Dim copyLog As String = " /host/logs/"

            cmd = "cp /host/diag_binary/binary_log_full/*.LOG" + copyLog + _FileName
            Try
                Me._manager.SendCmd(cmd, False)
            Catch ex As Exception
                Me._manager.SendCmd(cmd, False)
            End Try
        End Sub

    End Class

End Namespace
