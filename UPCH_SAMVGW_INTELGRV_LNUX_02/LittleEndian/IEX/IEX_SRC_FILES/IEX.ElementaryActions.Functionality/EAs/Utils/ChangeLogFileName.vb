Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    ''' In case of executing from Iexecuter the this function is taking care that a new folder
    ''' under the name of the test with time stamp will create for the 
    ''' log in the path alocated for the logs in the XML file
    ''' By this every execution of the same test will create a new log file
    ''' and the log will be located in the local machine and not in the tests folder (shared drive in most of the cases)
    ''' </summary>
    Public Class ChangeLogFileName
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim _LogPath As String
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="LogPath">The Path Of The IEX Log</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>329 - IEXSystemError</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' </remarks>
        Sub New(ByVal LogPath As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _LogPath = LogPath
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim ComputerName As String = ""
            Dim now1 As String = ""
            Dim LogFolderName As String = ""
            Dim iexNumber As String = ""

            Try
                If _LogPath = "" Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed To Get logdirectory From Environment.ini"))
                End If

                If _LogPath.EndsWith("\") = False Then
                    _LogPath = _LogPath + "\"
                End If

                EPG.Utils.LogCommentImportant("Changing Log Path To : " + _LogPath)

                If _manager.TestName = "N/A" Then
                    _manager.TestName = "Test"
                End If

                Try
                    ComputerName = System.Environment.MachineName
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Get Computer Name"))
                End Try

                Try
                    now1 = Now.ToString("dd/MM/yyyy HH:mm:ss tt")
                    now1 = Replace(now1, "/", "-")
                    now1 = Replace(now1, "\", "-")
                    now1 = Replace(now1, ":", ".")
                    now1 = Replace(now1, " ", "_")
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Parse Now Date Time"))
                End Try

                Try
                    iexNumber = _iex.IEXServerNumber.ToString
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Get IEX Server Number"))
                End Try

                LogFolderName = ComputerName & "_Server-" & iexNumber & "_" & now1

                Try
                    If Directory.Exists(_LogPath) = False Then
                        Directory.CreateDirectory(_LogPath)
                    End If
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Create Directory -> " + Me._LogPath))
                End Try

                Try
                    _iex.LogFileName = _LogPath & _manager.TestName & "\" & LogFolderName & "\" & _manager.TestName & ".iexlog"
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Set IEX Log Name"))
                End Try

                Try
                    _manager.LogFile = _LogPath & _manager.TestName & "\" & LogFolderName & "\" & _manager.TestName & ".iexlog"
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Set Manager Log File Name"))
                End Try

                EPG.Utils.LogCommentImportant("IEX Log File Changed To -> " & _iex.LogFileName)

                _iex.Wait(0.2)

            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Change Log File Name : " + ex.Message.ToString()))
            End Try

        End Sub
    End Class

End Namespace