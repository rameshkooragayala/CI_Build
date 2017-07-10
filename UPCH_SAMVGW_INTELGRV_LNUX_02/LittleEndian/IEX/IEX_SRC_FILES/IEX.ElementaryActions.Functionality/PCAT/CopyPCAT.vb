Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Copies PCAT.DB From STB By FTP
    ''' </summary>
    Public Class CopyPCAT
        Inherits IEX.ElementaryActions.BaseCommand
        Dim _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private reason As String

        Dim BuildWinPath, STB_IP As String
        Dim PcatSTBPath As String
        Dim PcatCopyCommand As String

        '''<param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim TestName As String = Path.GetFileNameWithoutExtension(_iex.LogFileName)
            Dim data As String = ""
            Dim tmpOtv As String = ""
            Dim cmd As String = ""

            PcatSTBPath = EPG.Utils.GetValueFromEnvironment("PcatSTBPath")
            PcatCopyCommand = EPG.Utils.GetValueFromEnvironment("PcatCopyCommand")

            EPG.Utils.LogCommentInfo("PcatSTBPath = " + PcatSTBPath)
            EPG.Utils.LogCommentInfo("PcatCopyCommand = " + PcatCopyCommand)

            EPG.Utils.LogCommentInfo("Starting Telnet login ...")

            cmd = PcatCopyCommand + " C:/PCAT_Modifier/IEX" + _iex.IEXServerNumber.ToString + "/PCAT.DB " + PcatSTBPath + "PCAT.DB"

            Try
                If File.Exists("C:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.DB") Then
                    Try
                        File.Delete("C:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.DB")
                    Catch ex As Exception
                    End Try
                End If
            Catch ex As Exception
            End Try
           
            Me._manager.TelnetLogIn(False)

            Try
                Me._manager.SendCmd(cmd, False)
            Catch ex As Exception
                EPG.Utils.LogCommentWarning("WARNING : Retrying To Send Command : " + cmd)
                Me._manager.SendCmd(cmd, False)
            End Try

            Me._manager.SendCmd("sync", False)

            Me._manager.TelnetDisconnect(False)

            EPG.Utils.LogCommentInfo("Telnet - PASSED")

            Dim LogPath As String = Path.GetFullPath(_iex.LogFileName)
            LogPath = LogPath.Substring(0, LogPath.LastIndexOf("\") + 1)

            Try
                File.Copy("C:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.DB", LogPath + "PCAT.DB", True)
            Catch ex As Exception
                Try
                    File.Copy("C:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.DB", LogPath + "PCAT.DB", True)
                Catch ex2 As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed To Copy PCAT From : C:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.DB" + " To : " + LogPath + " Exception : " + ex2.Message))
                End Try
            End Try

            EPG.Utils.LogCommentInfo("Copy PCAT - PASSED !")

        End Sub
    End Class
End Namespace