Imports FailuresHandler
Imports System.IO
Imports System.Text


Public Class OTA
    Inherits IEX.ElementaryActions.EPG.SF.OTA
    Dim _UI As IEX.ElementaryActions.EPG.SF.IPC.UI
    Private _Utils As EPG.SF.Utils
    Dim Path As String


    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.IPC.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    'Copy Binay from Buildwin path to OTA Folder and verify.
    Public Overrides Sub CopyBinary()
        _iex.LogComment("Copy image from build to specified path")
        Dim BuildPath As String
        Try
            Path = _UI.Utils.GetValueFromProject("OTA", "IPC_BuildPATH")
            BuildPath = _UI.Utils.GetValueFromEnvironment("BuildWinPath")
            BuildPath = BuildPath.Replace("/LittleEndian\\epg\\fs\\NDS", "\\LittleEndian\\drivers\\bzimage")
            _iex.LogComment("Path of the image: " + BuildPath)
            Dim file As New FileInfo(BuildPath)
            Try
                If file.Exists() Then
                    DeleteFiles(New FileInfo(Path + file.Name))
                    file.CopyTo(Path + file.Name)
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed to find Binary at : " + BuildPath))
                End If
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed to copy image from build to " + Path))
            End Try
        Catch ex1 As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed to copy image from build to " + Path))
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    '  Modify image version copy the modified file back to OTA Folder
    Overrides Sub ModifyImageVersion(ByVal VersionID As String)

        Dim finaloutput As String = ""
        Dim winscp As Process = New Process()
        Dim command As String

        _iex.LogComment("Copy file from OTA folder to Linux Machine")

        Try
            Path = _UI.Utils.GetValueFromProject("OTA", "IPC_BuildPATH")
            StartWinScp(winscp)
            ' Feed in the scripting commands
            winscp.StandardInput.WriteLine("option batch abort")
            winscp.StandardInput.WriteLine("option confirm off")
            winscp.StandardInput.WriteLine("open root:systemxx2@10.201.96.24")
            ' /var/www/html/SSU/ECE09B-6400-0001
            winscp.StandardInput.WriteLine("cd ..")
            winscp.StandardInput.WriteLine("cd var")
            winscp.StandardInput.WriteLine("cd www")
            winscp.StandardInput.WriteLine("cd html")
            winscp.StandardInput.WriteLine("cd SSU")
            winscp.StandardInput.WriteLine("cd ECE09B-6400-0001")
            winscp.StandardInput.WriteLine("put " + Path + "bzimage bzimage")
            winscp.StandardInput.Close()
            ' Wait until WinSCP finishes
            winscp.WaitForExit()
            'Fetching Final Output
            finaloutput = FetchFinalOutput(winscp, "OUTPUT")
            'if program runs successfully the final output will be WINSCP>. Check the same condition
            If finaloutput.Trim().ToUpper() = "WINSCP>" Then
                _iex.LogComment("Copied modified version file to Linux Machine")
				_iex.Wait(20)
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed To copy  version file to  Linux Machine. Reason: " + finaloutput))
            End If

            'Running sw_version_modifier exe from Linux Machine
            'creating a virtual bat file
            If VersionID = "16777276" Then

                command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd //var/www/html/SSU/ECE09B-6400-0001; ./sw_version_modifier.exe bzimage 16777275"""
            Else

                command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd //var/www/html/SSU/ECE09B-6400-0001; ./sw_version_modifier.exe bzimage 16777276"""

            End If
            finaloutput = StartProcess(command, "OUTPUT")
            'if program runs successfully the final output will be null>. Check the same condition

            If finaloutput.Trim().ToUpper() = "" Then
                _iex.LogComment("Modifed Image Version using Image Modifier")
				_iex.Wait(20)
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Modify Image Version using Image Modifier. Reason: " + finaloutput))
                Exit Sub
            End If

            'rename file to v0100003C.img
            If VersionID <> "16777276" Then
                command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd //var/www/html/SSU/ECE09B-6400-0001; mv bzimage.version_0x100003c v0100003C.img"""
                finaloutput = StartProcess(command, "OUTPUT")
                If finaloutput.Trim().Contains("v0100003C.img") Then
                    _iex.LogComment("Renamed the file successfully")
					_iex.Wait(20)
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Rename the file to v0100003C.img Reason: " + finaloutput))
                    Exit Sub
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ' Download option in Forced Mode
    Overrides Sub OTADownloadOption(ByVal downloadOption As String, Optional ByVal IsDownload As Boolean = True)

        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""

        If downloadOption.ToUpper() = "FORCED" Then
            Try


                Dim title As String = ""
                Dim downloadTime As Integer

                downloadTime = Convert.ToInt32(_UI.Utils.GetValueFromProject("OTA", "DOWNLOADTIME")) ' get download tme from project ini

                If _UI.Utils.VerifyState("YES_NO", 1500) Then  'if YESNO new version is available

                    'Verify from serial logs that the download is requested.
                    Res = _iex.Debug.BeginWaitForMessage("DOWNLOAD_REQUESTED", 0, 3600, IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If
                    Res = _iex.Debug.EndWaitForMessage("DOWNLOAD_REQUESTED", ActualLine, "", IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If

                    'Verify from serial logs that the download is successfull.
                    Res = _iex.Debug.BeginWaitForMessage("Download success", 0, 120, IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If
                    Res = _iex.Debug.EndWaitForMessage("Download success", ActualLine, "", IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If
                    _iex.Wait(120)

                Else  ' Milestone is getting
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Verify State: Failed to get Milestone"))

                End If
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Download: Reason:" + ex.Message))
            End Try
        ElseIf downloadOption.ToUpper() = "AUTOMATIC" Then
            Try

                Dim title As String = ""
                Dim downloadTime As Integer
                Dim maintanenceTime As Integer
                Dim now As String = ""

                downloadTime = Convert.ToInt32(_UI.Utils.GetValueFromProject("OTA", "DOWNLOADTIME")) ' get download tme from project ini

                maintanenceTime = Convert.ToInt32(_UI.Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DEALY")) ' get maintanence time from project ini

                _UI.Utils.NavigateToCheckForUpdates() 'navigating to Tool Box/Advanced Settings/Horizon Box/Check For Updates
                _UI.Utils.GetEpgInfo("title", now)
                If (now.ToUpper() = "NOW") Then
                    _iex.LogComment("Selecting option NOW for downloading binary")
                    _UI.Utils.SendIR("SELECT")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EmptyEpgInfoFailure, "Software up to date No NOW option displayed"))
                End If
                _iex.LogComment("Waiting for binary downloading ")
                _iex.Wait(180)

                If _UI.Utils.VerifyState("YES_NO", 1200) Then 'if YESNO new version is available
                    If IsDownload = True Then
                        _UI.Utils.EPG_Milestones_Navigate("NOW") ' select Yes
                        'Verify from serial logs that the download is requested.
                        Res = _iex.Debug.BeginWaitForMessage("DOWNLOAD_REQUESTED", 0, 600, IEXGateway.DebugDevice.Serial)
                        If Not Res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If
                        Res = _iex.Debug.EndWaitForMessage("DOWNLOAD_REQUESTED", ActualLine, "", IEXGateway.DebugDevice.Serial)
                        If Not Res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If

                        'Verify from serial logs that the download is successfull.
                        Res = _iex.Debug.BeginWaitForMessage("Download success", 0, 120, IEXGateway.DebugDevice.Serial)
                        If Not Res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If
                        Res = _iex.Debug.EndWaitForMessage("Download success", ActualLine, "", IEXGateway.DebugDevice.Serial)
                        If Not Res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If
                        _iex.Wait(120)
                    Else
                        _UI.Utils.EPG_Milestones_Navigate("LATER") ' wait for maintanence time
                        _iex.Wait(maintanenceTime + downloadTime)
                    End If


                Else ' Milestone is getting
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EmptyEpgInfoFailure, "Failed to Braodcast NIT Reason: Failed to get Milestone"))

                End If
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Download: Reason:" + ex.Message))
            End Try
        End If


    End Sub




#Region "Helper Funtions"

    'Funtion to start winscp

    Private Sub StartWinScp(ByVal winscp As Process)
        Try
            Path = _UI.Utils.GetValueFromProject("OTA", "IPC_BuildPATH") ' fetch ota path
            winscp.StartInfo.FileName = Path + "WinSCP.com" ' call winscp.com
            winscp.StartInfo.UseShellExecute = False
            winscp.StartInfo.RedirectStandardInput = True
            winscp.StartInfo.RedirectStandardOutput = True
            winscp.StartInfo.CreateNoWindow = True
            winscp.Start() ' start winscp

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to run winscp"))
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ' Function to fetch final output from logs

    Private Function FetchFinalOutput(ByVal process As Process, ByVal output As String) As String
        Dim finaloutput As String = ""
        If output.ToUpper() = "OUTPUT" Then
            While Not process.StandardOutput.EndOfStream ' fetching logs till end of stream
                finaloutput = process.StandardOutput.ReadLine()
            End While
            Return finaloutput
        Else
            finaloutput = process.StandardError.ReadToEnd()
            Return finaloutput
        End If

    End Function
    'Function to Delete files from remote shared folder

    Private Sub DeleteFiles(ByVal Dfile As FileInfo)
        Try


            If Dfile.Exists Then ' delete if file exists
                Dfile.Delete()

            End If
        Catch ex As Exception
            _iex.LogComment(Dfile.FullName & " is not deleted")
        End Try
    End Sub

#End Region

End Class

