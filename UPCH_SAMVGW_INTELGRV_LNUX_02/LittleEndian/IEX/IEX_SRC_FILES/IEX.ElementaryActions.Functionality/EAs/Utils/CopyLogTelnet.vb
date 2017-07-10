Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Copies TelnetLog.txt From BuildWinPath To Log Folder
    ''' </summary>
    Public Class CopyLogTelnet
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim IEX As IEXGateway.IEX
        Dim BuildWinPath As String
        Dim MountCommand As String
        Dim _FileName As String

        ''' <param name="FileName">Optional Parameter Default = "Diag_TelnetLog.txt" : Filename Of The Log File</param>
        ''' <param name="pManager">Manager</param>
        Sub New(ByVal FileName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            _FileName = FileName
            EPG = _manager.UI
        End Sub

        Protected Overrides Sub Execute()

            If _manager.Project.IsEPGLikeCogeco Then
                CopyTelnetCOGECO()
            Else
                CopyTelnetUPC()
            End If

        End Sub

        Private Function CopyTelnetCOGECO() As Boolean
            Dim Res As IEXGateway._IEXResult
            Dim cmd As String = ""
            Dim UsbSupported As Boolean = False
			
            Try

                 UsbSupported = Convert.ToBoolean(EPG.Utils.GetValueFromEnvironment("COPYUSBLOGS"))

            Catch ex As Exception
                UsbSupported = True
            End Try

            If UsbSupported Then

                cmd = "/usb/copylog.sh"
                EPG.Utils.LogCommentInfo("Copying Logs By Sending Command " + cmd)
                _iex.Debug.BeginWaitForMessage("FINISH", 0, 600, IEXGateway.DebugDevice.Telnet2)
                Res = _iex.Debug.WriteLine(cmd, IEXGateway.DebugDevice.Telnet2)
                _iex.Debug.EndWaitForMessage("FINISH", 0, "", IEXGateway.DebugDevice.Telnet2)
            Else
                EPG.Utils.LogCommentInfo("Copying USB Logs Not needed ")
            End If
            Return True
        End Function

        Private Function CopyTelnetUPC() As Boolean
            Dim passed As Boolean
            Dim LogPath As String = Path.GetDirectoryName(_iex.LogFileName)

            BuildWinPath = EPG.Utils.GetValueFromEnvironment("BuildWinPath")
            MountCommand = EPG.Utils.GetValueFromEnvironment("MountCommand")

            If MountCommand.Contains("nohup") Then
                If BuildWinPath <> "" Then
                    passed = CopyFile(BuildWinPath + "\Diag_TelnetLog.txt", LogPath + "\" + Me._FileName)
                    If Not passed Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, " Failed To Copy TelnetLog From : " + BuildWinPath + "\Diag_TelnetLog.txt To " + LogPath + "\" + Me._FileName))
                    Else
                        EPG.Utils.LogCommentInfo("Copied TelnetLog To -> " + LogPath + "\" + Me._FileName)
                    End If
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, " Failed To Get BuildWinPath From Environment.ini"))
                End If
            Else
                _iex.Debug.EndLogging(IEXGateway.DebugDevice.Telnet)

                EPG.Utils.LogCommentInfo("Looking For Telnet Log At " & LogPath)
                Dim patternToFind As String = "*_Telnet_*"
                Dim fileNamesArray As String() = Directory.GetFiles(LogPath, patternToFind)
                Dim arrayLen As Integer = fileNamesArray.Length
                If arrayLen = 0 Then
                    EPG.Utils.LogCommentWarning("WARNING : Not Found File Matching The Pattern " & patternToFind & " At " & LogPath)
                Else
                    Array.Sort(fileNamesArray)
                    Dim logName As String = fileNamesArray(arrayLen - 1)
                    Dim fullPathToLog As String = Path.Combine(LogPath, logName)
                    If Not System.IO.File.Exists(fullPathToLog) Then
                        EPG.Utils.LogCommentWarning("WARNING : Not Found Or Missing Permissions To File: " & fullPathToLog)
                    Else
                        Const TEXT_TO_FIND As String = "DIAG_BINARY_DEBUG mkdir -p"
                        Dim index As Integer = -1
                        Dim pathToRetrieve As String = ""
                        Dim textWasFound As Boolean = False
                        Dim foundLine As String = ""
                        For Each line As String In System.IO.File.ReadAllLines(fullPathToLog)
                            index = line.IndexOf(TEXT_TO_FIND)
                            If index <> -1 Then
                                textWasFound = True
                                foundLine = line
                                Exit For
                            End If
                        Next
                        If Not textWasFound Then
                            EPG.Utils.LogCommentWarning("WARNING : The Following String Was Not Found In The File: " & TEXT_TO_FIND)
                        Else
                            'Extract Path And Make Post Processing
                            pathToRetrieve = foundLine.Substring(index + TEXT_TO_FIND.Length)
                            EPG.Utils.LogCommentInfo("Initial Path Found: " & pathToRetrieve)
                            pathToRetrieve = ManipulatePathToGetExpectedFormat(pathToRetrieve)

                            'Save To File
                            Const FINAL_FILENAME As String = "LinkToTalos" & ".html"
                            Dim fullPathToFileContainingPath As String = Path.Combine(LogPath, FINAL_FILENAME)

                            EPG.Utils.LogCommentInfo("Inserting Link To Talos In " & fullPathToFileContainingPath)

                            Using streamWriter As StreamWriter = New StreamWriter(fullPathToFileContainingPath, False)
                                streamWriter.WriteLine(pathToRetrieve)
                                streamWriter.Close()
                            End Using
                        End If
                    End If
                End If
            End If
            Return True

        End Function

        Private Function CopyFile(ByVal ffrom As String, ByVal fTo As String) As Boolean
            Try
                If File.Exists(fTo) Then
                    fTo = fTo.Replace(".txt", Now.Minute.ToString + ".txt")
                    Me._FileName = Path.GetFileName(fTo)
                End If
                File.Copy(ffrom, fTo, True)
                Return True

            Catch ex As Exception
                Return False
            End Try

        End Function

        Private Function WrapAsLink(path As String) As String
            Return "<html><body><a href=""file:///" & path & """" & ">Link to file</a></body></html>"
        End Function

        Private Function ManipulatePathToGetExpectedFormat(pathToRetrieve As String) As String
            Const SLASH As String = "/"
            Const BACKSLASH As String = "\"
            Const INITIAL_PATH_PREFIX As String = "/talosmount/"
            Const EXPECTED_PATH_PREFIX As String = "\\Taloslogs\projects\upc\"
            pathToRetrieve = pathToRetrieve.Trim().Replace(pathToRetrieve.Chars(pathToRetrieve.LastIndexOf(BACKSLASH)), "") _
                                                .Replace(INITIAL_PATH_PREFIX, EXPECTED_PATH_PREFIX) _
                                                .Replace(SLASH, BACKSLASH)
            EPG.Utils.LogCommentInfo("Final Path: " & pathToRetrieve)
            Return WrapAsLink(pathToRetrieve)
        End Function

    End Class

End Namespace