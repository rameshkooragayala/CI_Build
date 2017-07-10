Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Copies All Csv Files On \NDS To Log Folder
    ''' </summary>
    Public Class CopyCsvFiles
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim IEX As IEXGateway.IEX
        Dim iniFile As AMS.Profile.Ini
        Dim BuildWinPath As String

        ''' <param name="pManager">Manager</param>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = _manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim passed As Boolean
            Dim LogPath As String = Path.GetDirectoryName(_iex.LogFileName)

            BuildWinPath = EPG.Utils.GetValueFromEnvironment("BuildWinPath")

            If BuildWinPath <> "" Then
                passed = CopyScvFiles(BuildWinPath, LogPath + "\")
                If Not passed Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, " Failed To All Csv Files From : " + BuildWinPath + " To " + LogPath + "\"))
                Else
                    EPG.Utils.LogCommentInfo("Copied Csv Files To -> " + LogPath + "\")
                End If
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, " Failed To Get BuildWinPath From Environment.ini"))
            End If

        End Sub

        Private Function CopyScvFiles(ByVal RootDir As String, ByVal fTo As String) As Boolean
            Try
                Dim filePaths As String() = Nothing

                filePaths = Directory.GetFiles(RootDir, "*.csv")
                If filePaths IsNot Nothing Then
                    For Each f As String In filePaths
                        Dim fi As New FileInfo(f)
                        File.Copy(fi.FullName, fTo + fi.Name)
                    Next
                End If
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

    End Class

End Namespace