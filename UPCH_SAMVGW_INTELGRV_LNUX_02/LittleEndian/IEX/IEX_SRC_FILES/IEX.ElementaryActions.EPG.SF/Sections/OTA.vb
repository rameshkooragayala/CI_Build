Imports FailuresHandler
Imports System.IO
Imports System.Text


Public Class OTA
    Inherits IEX.ElementaryActions.EPG.OTA

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils
    Dim Path As String


    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    'Copy Binay from Buildwin path to OTA Folder and verify.
    Overrides Sub CopyBinary()
        _Utils.StartHideFailures("Copy image from build to specified path")
        Dim BuildPath As String
        Try
            Path = _UI.Utils.GetValueFromProject("OTA", "PATH")
            BuildPath = _UI.Utils.GetValueFromEnvironment("BuildWinPath")
            BuildPath = BuildPath.Replace("/LittleEndian\\epg\\fs\\NDS", "\\LittleEndian\\drivers\\bzimage")

            Dim file As New FileInfo(BuildPath)
            Try
                If file.Exists() Then
                    DeleteFiles(New FileInfo(Path + "bzimage"))
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

        _Utils.StartHideFailures("Copy file from OTA folder to Linux Machine")

        Try
            Path = _UI.Utils.GetValueFromProject("OTA", "PATH")
            StartWinScp(winscp)
            ' Feed in the scripting commands
            winscp.StandardInput.WriteLine("option batch abort")
            winscp.StandardInput.WriteLine("option confirm off")
            winscp.StandardInput.WriteLine("open root:systemxx2@10.201.96.24")
            If _Utils.GetValueFromEnvironment("Project").ToUpper() = "ISTB" Then
                winscp.StandardInput.WriteLine("cd madhu")
            Else
                winscp.StandardInput.WriteLine("cd sainath")
            End If
            winscp.StandardInput.WriteLine("put " + Path + "bzimage bzimage")
            winscp.StandardInput.Close()
            ' Wait until WinSCP finishes
            winscp.WaitForExit()
            'Fetching Final Output
            finaloutput = FetchFinalOutput(winscp, "OUTPUT")
            'if program runs successfully the final output will be WINSCP>. Check the same condition
            If finaloutput.Trim().ToUpper() = "WINSCP>" Then
                _iex.LogComment("Copied modified version file to Linux Machine")
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed To copy  version file to  Linux Machine. Reason: " + finaloutput))
            End If

            'Running sw_version_modifier exe from Linux Machine
            'creating a virtual bat file
            If VersionID = "16777276" Then
                If _Utils.GetValueFromEnvironment("Project").ToUpper() = "ISTB" Then
                    command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd /root/madhu; ./sw_version_modifier.exe bzimage 16777275"""
                Else
                    command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd /root/sainath; ./sw_version_modifier.exe bzimage 16777275"""
                End If
            Else
                If _Utils.GetValueFromEnvironment("Project").ToUpper() = "ISTB" Then
                    command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd /root/madhu; ./sw_version_modifier.exe bzimage 16777276"""
                Else
                    command = Path + "plink.exe 10.201.96.24 -l root -pw systemxx2 ""cd /root/sainath; ./sw_version_modifier.exe bzimage 16777276"""
                End If
            End If

            finaloutput = StartProcess(command, "OUTPUT")
            'if program runs successfully the final output will be null>. Check the same condition

            If finaloutput.Trim().ToUpper() = "" Then
                _iex.LogComment("Modifed Image Version using Image Modifier")
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed To Modify Image Version using Image Modifier. Reason: " + finaloutput))
                Exit Sub
            End If

            'copy modified image from Linux  to OTA Folder
            ' Run hidden WinSCP process
            winscp = New Process()
            StartWinScp(winscp)
            ' Feed in the scripting commands
            winscp.StandardInput.WriteLine("option batch abort")
            winscp.StandardInput.WriteLine("option confirm off")
            winscp.StandardInput.WriteLine("open root:systemxx2@10.201.96.24")
            If _Utils.GetValueFromEnvironment("Project").ToUpper() = "ISTB" Then
                winscp.StandardInput.WriteLine("cd madhu")
            Else
                winscp.StandardInput.WriteLine("cd sainath")
            End If
            If VersionID = "16777276" Then

                winscp.StandardInput.WriteLine("get bzimage.version_0x100003b  " + Path + "bzimage.version_0x100003b")
            Else
                winscp.StandardInput.WriteLine("get bzimage.version_0x100003c  " + Path + "bzimage.version_0x100003c")
            End If

            winscp.StandardInput.Close()
            ' Wait until WinSCP finishes
            winscp.WaitForExit()
            'Fetching the final output
            finaloutput = FetchFinalOutput(winscp, "OUTPUT")
            'if program runs successfully the final output will be WINSCP>. Check tghe same condition
            If finaloutput.Trim().ToUpper() = "WINSCP>" Then
                _iex.LogComment("Copied modified version file to OTA Folder")
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed To copy modified version file to OTA Folder. Reason: " + finaloutput))
                Exit Sub
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    'Create Caroussel

    Overrides Sub Create_Carousel(ByVal Version_ID As String, ByVal Usage_ID As String, ByVal _RFFeed As String)

        Try
            'Get the OTA Folder Path from the Project INI File
            Dim Path As String = _Utils.GetValueFromProject("OTA", "PATH")

            If Version_ID = "16777276" Then


                create_carousel_bat_ForVesion275(Path, Usage_ID)
                'Create_Carousal_For275.bat dyanically 
                CreatCarousalFor275(Path, _RFFeed)

            Else

                create_carousel_bat_ForVesion276(Path, Usage_ID)

                'Create_Carousal_For276.bat dyanamically
                CreatCarousalFor276(Path, _RFFeed)

            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.CreateCarausalFailure, "Failed To Create the carausal file" +ex.Message))
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    'Broadcast caroussel

    Overrides Sub BroadcastCarousel(ByVal RFFeed As String)
        'Run reset.bat

        Dim message As String
        Dim myString As String = ""
        Dim carouselBroadcastPath As String
        Dim command As String
        Dim finaloutput As String
        Path = _UI.Utils.GetValueFromProject("OTA", "PATH")
        If RFFeed = "NL" Then
            carouselBroadcastPath = _Utils.GetValueFromProject("OTA", "Carousel_ResetBroadcast_Path_NL")
            command = "psexec -i -s -d \\10.201.96.23 -u Administrator -p systemxx2 " + carouselBroadcastPath + "reset.bat"

        Else
            carouselBroadcastPath = _Utils.GetValueFromProject("OTA", "Carousel_ResetBroadcast_Path_UM")
            command = "psexec -i -s -d \\10.201.96.23 -u Administrator -p systemxx2 " + carouselBroadcastPath + "reset.bat"
        End If
        finaloutput = StartProcess(command, "ERROR")
        'if program runs successfully the final output will be null>. Check tghe same condition

        If finaloutput.Trim().Contains("started on 10.201.96.23 with process ID") Then
            _iex.LogComment("Carousel Reset Successfull")
        Else
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.BroadcastCarausalFailure, "Failed to Reset Carousel Reason: " + finaloutput))
        End If


        'Run Broadast.bat
        If RFFeed = "NL" Then
			_iex.Wait(20)
            carouselBroadcastPath = _Utils.GetValueFromProject("OTA", "Carousel_ResetBroadcast_Path_NL")
            command = "psexec -i -s -d \\10.201.96.23 -u Administrator -p systemxx2 " + carouselBroadcastPath + "broadcast.bat"
        Else
			_iex.Wait(20)
            carouselBroadcastPath = _Utils.GetValueFromProject("OTA", "Carousel_ResetBroadcast_Path_UM")
            command = "psexec -i -s -d \\10.201.96.23 -u Administrator -p systemxx2 " + carouselBroadcastPath + "broadcast.bat"
        End If
        finaloutput = StartProcess(command, "ERROR")
        'if program runs successfully the final output will be null>. Check tghe same condition

        If finaloutput.Trim().Contains("started on 10.201.96.23 with process ID") Then
            _iex.LogComment("Broadcast Carousel Successfull")
        Else
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.BroadcastCarausalFailure, "Failed to broadcast Carousel. Reason: " + finaloutput))
        End If

    End Sub

    ' NIT Broadcast
    Overrides Sub NITBraodcast(ByVal NITTable As String)

        Dim extn As String
        Dim NITFile As String()
        NITFile = NITTable.Split(".")
        extn = "." + NITFile(1)


        Dim NITPath As String = _UI.Utils.GetValueFromProject("OTA", "NITPATH") '"\\10.201.96.247\\SBI_Pilot_2.0\\"
        Dim finaloutput As String

        Try
            'check whether NL / UM
            If NITTable.ToUpper().Contains("_NL_") Then

                Dim NLSce1 As String = NITTable.Replace(extn, "_31.sce")
                Dim NLSce2 As String = NITTable.Replace(extn, "_32.sce")
                Dim NLBAT As String = NITTable.Replace(extn, ".bat")
                Dim LineText As New Text.StringBuilder()
                Path = _UI.Utils.GetValueFromProject("OTA", "PATH")

                'creating sce file1 dynamically and copy in OTA folder
                LineText.AppendLine("-- SBI 3.7.12")
                LineText.AppendLine("-- Reset the current broadcasting and send the new carousel")
                LineText.AppendLine("-- Please check the host/ port / file path / stream_id  and period")
                LineText.AppendLine("--max_msg_size: define the value of each cut message")
                LineText.AppendLine("remote(host:10.201.96.21, port: 4327, name : S_5919_31, max_msg_size: 90_000)")
                LineText.AppendLine("-- declare the carousel location")
                LineText.AppendLine("load_sections( data_id: data_1, file: D:\SBI_Pilot_2.0\" & NITTable.Trim() & ")")
                LineText.AppendLine("connect")
                LineText.AppendLine("-- declare a reset")
                LineText.AppendLine("reset(id:reset_1, stream_id:S_5919_31)")
                LineText.AppendLine("  -- send the reset command")
                LineText.AppendLine(" send_reset(id:reset_1 )")
                LineText.AppendLine(" -- send the new carousel")
                LineText.AppendLine(" put_direct(data_id:data_1, stream_id :S_5919_31, period :5);")
                LineText.AppendLine("disconnect")
                File.WriteAllText(NITPath + NLSce1.Trim(), LineText.ToString())

                'creating sce file2 dynamically and copy in OTA folder
                LineText.Clear()
                LineText.AppendLine("-- SBI 3.7.12")
                LineText.AppendLine("-- Reset the current broadcasting and send the new carousel")
                LineText.AppendLine("-- Please check the host/ port / file path / stream_id  and period")
                LineText.AppendLine("--max_msg_size: define the value of each cut message")
                LineText.AppendLine("remote(host:10.201.96.21, port: 4327, name : S_5920_32, max_msg_size: 90_000)")
                LineText.AppendLine("-- declare the carousel location")
                LineText.AppendLine("load_sections( data_id: data_1, file: D:\SBI_Pilot_2.0\" & NITTable.Trim() & ")")
                LineText.AppendLine("connect")
                LineText.AppendLine("-- declare a reset")
                LineText.AppendLine("reset(id:reset_1, stream_id:S_5920_32)")
                LineText.AppendLine("  -- send the reset command")
                LineText.AppendLine(" send_reset(id:reset_1 )")
                LineText.AppendLine(" -- send the new carousel")
                LineText.AppendLine(" put_direct(data_id:data_1, stream_id :S_5920_32, period :5);")
                LineText.AppendLine("disconnect")
                File.WriteAllText(NITPath + NLSce2.Trim(), LineText.ToString())

                'creating bat file dynamilcally and copy in OTA folder
                LineText.Clear()
                LineText.AppendLine("d:")
                LineText.AppendLine(" -- send the new carousel")
                LineText.AppendLine("cd D:\SBI_Pilot_2.0")
                LineText.AppendLine("sbi_client_simulator.exe " & NLSce1.Trim() & "")
                LineText.AppendLine("sbi_client_simulator.exe " & NLSce2.Trim() & "")
                File.WriteAllText(NITPath + NLBAT.Trim(), LineText.ToString())

                ' Run SBI Piolet
                finaloutput = StartProcess("psexec -i -s -d \\10.201.96.247 -u administrator -p Abcd1234 D:\SBI_Pilot_2.0\" & NLBAT.Trim() & "", "ERROR")
                _iex.Wait(5)
                If finaloutput.Trim().Contains("started on 10.201.96.247 with process ID") Then
                    _iex.LogComment("NIT "& NITTable.Trim() &" Broadcast Successfull")
				   
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Broadcast NIT Reason: " + finaloutput))
                End If

                ' Deleting Sce &Bat  Files
                Try
                    DeleteFiles(New FileInfo(NITPath + NLBAT.Trim()))


                    DeleteFiles(New FileInfo(NITPath + NLSce1.Trim()))



                    DeleteFiles(New FileInfo(NITPath + NLSce2.Trim()))

                Catch ex As Exception
                    _iex.LogComment("Failed to delete BAT and SCE files from 10.201.96.247 machine" + ex.Message)
                End Try

            ElseIf NITTable.ToUpper().Contains("_UM_") Then

                ' for UM
                Dim LineText As New Text.StringBuilder()
                Dim UMSce1 As String = NITTable.Replace(extn, "_51.sce")
                Dim UMBAT As String = NITTable.Replace(extn, ".bat")
                'creating sce file1 dynamically and copy in OTA folder
                LineText.Clear()
                LineText.AppendLine("-- SBI 3.7.12")
                LineText.AppendLine("-- Reset the current broadcasting and send the new carousel")
                LineText.AppendLine("-- Please check the host/ port / file path / stream_id  and period")
                LineText.AppendLine("--max_msg_size: define the value of each cut message")
                LineText.AppendLine("remote(host:10.201.96.21, port: 4326, name : S_5923_51, max_msg_size: 90_000)")
                LineText.AppendLine("-- declare the carousel location")
                LineText.AppendLine("load_sections( data_id: data_1, file: D:\SBI_Pilot_2.0\" & NITTable.Trim() & ")")
                LineText.AppendLine("connect")
                LineText.AppendLine("-- declare a reset")
                LineText.AppendLine("reset(id:reset_1, stream_id:S_5919_51)")
                LineText.AppendLine("  -- send the reset command")
                LineText.AppendLine(" send_reset(id:reset_1 )")
                LineText.AppendLine(" -- send the new carousel")
                LineText.AppendLine(" put_direct(data_id:data_1, stream_id :S_5923_51, period :5);")
                LineText.AppendLine("disconnect")
                File.WriteAllText(NITPath + UMSce1.Trim(), LineText.ToString())

                'creating bat file dynamically and copy in OTA folder
                LineText.Clear()
                LineText.AppendLine("d:")
                LineText.AppendLine(" -- send the new carousel")
                LineText.AppendLine("cd D:\SBI_Pilot_2.0")
                LineText.AppendLine("sbi_client_simulator.exe " & UMSce1.Trim() & "")
                File.WriteAllText(NITPath + UMBAT.Trim(), LineText.ToString())


                'run SBI Piolet
                finaloutput = StartProcess("psexec -i -s -d \\10.201.96.247 -u administrator -p Abcd1234 D:\SBI_Pilot_2.0\" & UMBAT.Trim() & "", "ERROR")
                _iex.Wait(5)

                If finaloutput.Trim().Contains("started on 10.201.96.247 with process ID") Then
                    _iex.LogComment("NIT " & NITTable.Trim() & " Broadcast Successfull")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Braodcast NIT Reason: " + finaloutput))
                End If

                Try
                    DeleteFiles(New FileInfo(NITPath + UMBAT.Trim()))
                    DeleteFiles(New FileInfo(NITPath + UMSce1.Trim()))
                Catch ex As Exception
                    _iex.LogComment("Failed to delete BAT and SCE files from 10.201.96.247 machine" + ex.Message)
                End Try



            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "NIT Table file is not in correct format"))

            End If


        Catch ex1 As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Error in NIT Broadcat : Reason " & ex1.Message & ""))

        Finally
            _iex.ForceHideFailure()
        End Try


        _iex.Wait(5)
    End Sub


    ' Download option in Forced Mode
    Overrides Sub OTADownloadOption(ByVal downloadOption As String, Optional ByVal IsDownload As Boolean = True)

        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""

        If downloadOption.ToUpper() = "FORCED" Then
            Try


                Dim title As String = ""
                Dim downloadTime As Integer

                downloadTime = Convert.ToInt32(_Utils.GetValueFromProject("OTA", "DOWNLOADTIME")) ' get download tme from project ini



                If _Utils.VerifyState("YES_NO", 300) Then  'if YESNO new version is available

                    'Verify from serial logs that the download is requested.
                    Res = _iex.Debug.BeginWaitForMessage("DOWNLOAD_REQUESTED", 0, 300, IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If
                    Res = _iex.Debug.EndWaitForMessage("DOWNLOAD_REQUESTED", ActualLine, "", IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If

                    'Verify from serial logs that the download is successfull.
                    Res = _iex.Debug.BeginWaitForMessage("Download success", 0, 3600, IEXGateway.DebugDevice.Serial)
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
                downloadTime = Convert.ToInt32(_Utils.GetValueFromProject("OTA", "DOWNLOADTIME")) ' get download tme from project ini

                maintanenceTime = Convert.ToInt32(_Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DEALY")) ' get maintanence time from project ini

                  _Utils.NavigateToCheckForUpdates() 'navigating to Tool Box/Advanced Settings/Horizon Box/Check For Updates


                If _Utils.VerifyState("YES_NO", 300) Then 'if YESNO new version is available
                    If IsDownload = True Then
                        _Utils.EPG_Milestones_Navigate("NOW") ' select Yes
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
                        Res = _iex.Debug.BeginWaitForMessage("Download success", 0, 3600, IEXGateway.DebugDevice.Serial)
                        If Not Res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If
                        Res = _iex.Debug.EndWaitForMessage("Download success", ActualLine, "", IEXGateway.DebugDevice.Serial)
                        If Not Res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If
                        _iex.Wait(120)
                    Else
                        _Utils.EPG_Milestones_Navigate("LATER") ' wait for maintanence time
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
            Path = _UI.Utils.GetValueFromProject("OTA", "PATH") ' fetch ota path
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

    Public Function StartProcess_Fitness(ByVal command As String) As Process

        Dim myProcess As New Process
        Try
            Dim SW As System.IO.StreamWriter = New System.IO.StreamWriter("VirtualBAT.bat")

            SW.WriteLine(command)

            SW.Flush()
            SW.Close()
            SW.Dispose()
            SW = Nothing
            ' System.Diagnostics.Process.Start("VirtualBAT.bat")

            ' starting process accoriding to command in VirtualBAT.bat

            Dim myProcessStartInfo As ProcessStartInfo = New ProcessStartInfo("VirtualBAT.bat")
            myProcessStartInfo.UseShellExecute = False
            myProcessStartInfo.RedirectStandardInput = True
            myProcessStartInfo.RedirectStandardOutput = True
            myProcessStartInfo.RedirectStandardError = True

            myProcess.StartInfo = myProcessStartInfo

            myProcess.Start() ' start process

            _UI.Utils.LogCommentImportant("Started the process for ant")

            'myProcess.WaitForExit() 'wait for process to complete


        Catch ex As Exception

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Start process : " + ex.Message))
        Finally
            _iex.ForceHideFailure()
        End Try

        Return myProcess
    End Function

    'Function to start Process
    Public Function StartProcess(ByVal command As String, ByVal outputfrm As String) As String
        Dim finaloutput As String = ""
        Try
            Dim SW As System.IO.StreamWriter = New System.IO.StreamWriter("VirtualBAT.bat")

            SW.WriteLine(command)

            SW.Flush()
            SW.Close()
            SW.Dispose()
            SW = Nothing
            ' System.Diagnostics.Process.Start("VirtualBAT.bat")

            ' starting process accoriding to command in VirtualBAT.bat
            Dim myProcess As New Process
            Dim myProcessStartInfo As ProcessStartInfo = New ProcessStartInfo("VirtualBAT.bat")
            myProcessStartInfo.UseShellExecute = False
            myProcessStartInfo.RedirectStandardInput = True
            myProcessStartInfo.RedirectStandardOutput = True
            myProcessStartInfo.RedirectStandardError = True

            myProcess.StartInfo = myProcessStartInfo

            myProcess.Start() ' start process

            myProcess.WaitForExit() 'wait for process to complete

            finaloutput = FetchFinalOutput(myProcess, outputfrm) ' calling final output function to fetch final output

        Catch ex As Exception
            finaloutput = ex.Message
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Failed to Start process : " + finaloutput))
        Finally
            _iex.ForceHideFailure()
        End Try

        Return finaloutput
    End Function
    Public Sub VerifySoftVersion(ByVal SoftVersion As String, ByVal OldSoftVersion As String)

        If (SoftVersion.Equals(OldSoftVersion)) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, " The Current and Old Software version are same after OTA download"))
        Else
            _UI.Utils.LogCommentImportant("Software Version is updated after OTA dowload")

        End If

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


    Private Sub create_carousel_bat_ForVesion275(ByVal Path As String, ByVal Usage_ID As Integer)

        Dim AutomatedCarouselToolPath As String = Path + "carouselsooo_275.au3"
        Dim VersionPath As String = Path + "bzimage.version_0x100003b"

        Dim f As FileInfo = New FileInfo(AutomatedCarouselToolPath)

        If (f.Exists) Then

            f.Delete()
        End If

        'Creating carouselsooo.au3 Dyanamicaly 

        Dim LineText As StringBuilder = New StringBuilder()
        LineText.AppendLine("Sleep ( 2000 ) " & vbCrLf & _
                   "Run(""cmd.exe"")" & vbCrLf & _
       "Sleep(2000)" & vbCrLf & _
                   "Send(""C:"")" & vbCrLf & _
       "Sleep(3000)" & vbCrLf & _
                   "Send(""{ENTER}"")" & vbCrLf & _
                   "Sleep(4000)")

        'syntax in bat file : Send("cd C:\OTA\AutomatedCarouselTool")

        Dim cdPath As String = """" + "cd" + " " + Path + """"
        LineText.AppendLine("Send(" + cdPath + ")")

        LineText.AppendLine("Sleep ( 3000 )" & vbCrLf & _
                "Send(""{ENTER}"")" & vbCrLf & _
                "Sleep(3000)" & vbCrLf & _
                            "Send(""SectionGenerator_UPC_w32_20111012.exe"")" & vbCrLf & _
                "Sleep(2000)" & vbCrLf & _
                            "Send(""{ENTER}"")" & vbCrLf & _
                "Sleep(2000)" & vbCrLf & _
                            "Send(""{TAB}"")" & vbCrLf & _
                "Sleep(2000)" & vbCrLf & _
                            "Send(""{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}"")" & vbCrLf & _
                            "Sleep ( 2000 )")

        Dim VerPath As String = """" + VersionPath + """"
        LineText.AppendLine("Send(" + VerPath + ")")
        LineText.AppendLine("Sleep ( 2000 )" & vbCrLf & _
                    "Send(""{TAB}"")" & vbCrLf & _
                    "Sleep ( 3000 )")


        'Getting Usage_ID from script

        Dim usageID As String = """" + Usage_ID.ToString() + """"

        LineText.AppendLine("Send(" + usageID + ")")

        LineText.AppendLine("Sleep ( 2000 )" & vbCrLf & _
                    "Send(""{TAB}{TAB}{ENTER}"")" & vbCrLf & _
        "Sleep(2000)" & vbCrLf & _
                    "WinClose(""Administrator: C:\Windows\system32\cmd.exe"")")
        File.WriteAllText(AutomatedCarouselToolPath, LineText.ToString())

        'Creating carouselsooo.au3 Dyanamicaly ENDS.........

    End Sub

    ''' <summary>
    '''   'Create carouselsooo_276_au3.au3 File
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub create_carousel_bat_ForVesion276(ByVal Path As String, ByVal Usage_ID As Integer)

        Dim AutomatedCarouselToolPath As String = Path + "carouselsooo_276.au3"
        Dim VersionPath As String = Path + "bzimage.version_0x100003c"

        Dim f As FileInfo = New FileInfo(AutomatedCarouselToolPath)

        If (f.Exists) Then

            f.Delete()
        End If

        'Creating carouselsooo.au3 Dyanamicaly 

        Dim LineText As StringBuilder = New StringBuilder()
        LineText.AppendLine("Sleep ( 2000 ) " & vbCrLf & _
                   "Run(""cmd.exe"")" & vbCrLf & _
       "Sleep(2000)" & vbCrLf & _
                   "Send(""C:"")" & vbCrLf & _
       "Sleep(3000)" & vbCrLf & _
                   "Send(""{ENTER}"")" & vbCrLf & _
                   "Sleep(4000)")

        'syntax in bat file : Send("cd C:\OTA")

        Dim cdPath As String = """" + "cd" + " " + Path + """"
        LineText.AppendLine("Send(" + cdPath + ")")

        LineText.AppendLine("Sleep ( 3000 )" & vbCrLf & _
                "Send(""{ENTER}"")" & vbCrLf & _
                "Sleep(3000)" & vbCrLf & _
                            "Send(""SectionGenerator_UPC_w32_20111012.exe"")" & vbCrLf & _
                "Sleep(2000)" & vbCrLf & _
                            "Send(""{ENTER}"")" & vbCrLf & _
                "Sleep(2000)" & vbCrLf & _
                            "Send(""{TAB}"")" & vbCrLf & _
                "Sleep(2000)" & vbCrLf & _
                            "Send(""{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}"")" & vbCrLf & _
                            "Sleep ( 2000 )")

        Dim VerPath As String = """" + VersionPath + """"
        LineText.AppendLine("Send(" + VerPath + ")")
        LineText.AppendLine("Sleep ( 2000 )" & vbCrLf & _
                    "Send(""{TAB}"")" & vbCrLf & _
                    "Sleep ( 3000 )")


        'Getting Usage_ID from script

        Dim usageID As String = """" + Usage_ID.ToString() + """"

        LineText.AppendLine("Send(" + usageID + ")")

        LineText.AppendLine("Sleep ( 2000 )" & vbCrLf & _
                    "Send(""{TAB}{TAB}{ENTER}"")" & vbCrLf & _
        "Sleep(2000)" & vbCrLf & _
                    "WinClose(""Administrator: C:\Windows\system32\cmd.exe"")")

        File.WriteAllText(AutomatedCarouselToolPath, LineText.ToString())

        'Creating carouselsooo.au3 Dyanamicaly ENDS.........


    End Sub
    ''' <summary>
    '''   Creat Carousal function For 275
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreatCarousalFor275(ByVal Path As String, ByVal _RFFeed As String)

        'Create Carousel using the section generator
        Dim command As String = ""
        Dim finaloutput As String = ""
        command = Path + "carouselsooo_275.au3"

        'run au3 file
        finaloutput = StartProcess(command, "ERROR")
        Dim dir As New DirectoryInfo(Path + "\result")
        If dir.Exists Then
            _iex.LogComment("Copied modified version file to Linux Machine")
        Else
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.CreateCarausalFailure, "Failed to create the carousel: Result folder not generated"))
        End If
        ' Move the .dat file created outside, inside the Result folder.
        Dim fileName As String = "DSI.DAT12345678"
        Dim sourcePath As String = Path + "result"
        Dim targetPath As String = Path + "result\570AD000"

        ' Use Path class to manipulate file and directory paths.
        Dim sourceFile As String = System.IO.Path.Combine(sourcePath, fileName)
        Dim destFile As String = System.IO.Path.Combine(targetPath, fileName)

        ' To copy a file to another location and 
        ' overwrite the destination file if it already exists.
        System.IO.File.Copy(sourceFile, destFile, True)

        'copy the carousel created into 23 machine
        Dim CarrouselPath As String

        If _RFFeed = "NL" Then
			_iex.LogComment("Copying the craousels to \\10.201.96.23\UPC_Carrousel_broadcast_tool")
            CarrouselPath = "\\10.201.96.23\UPC_Carrousel_broadcast_tool2\Boite\Carrousel\"
            For Each copyfile As FileInfo In dir.GetFiles
                DeleteFiles(New FileInfo(CarrouselPath + copyfile.Name))
                copyfile.CopyTo(CarrouselPath + copyfile.Name)
            Next
        Else
			_iex.LogComment("Copying the craousels to \\10.201.96.23\BoiteTS51-4901\Carrousel")
            CarrouselPath = "\\10.201.96.23\BoiteTS51-4901\Carrousel\"
            For Each copyfile As FileInfo In dir.GetFiles
                DeleteFiles(New FileInfo(CarrouselPath + copyfile.Name))
                copyfile.CopyTo(CarrouselPath + copyfile.Name)

            Next
        End If

        'Remove locally created craousel (result folder)
        _iex.Wait(5)
        _iex.LogComment("Copied the carousels to 10.201.96.23 successfully")
        Try
            Directory.Delete(Path + "result", True)
        Catch ex As Exception
            _iex.LogComment("Unable to delete result folder" + ex.Message)
        End Try
    End Sub

    ''' <summary>
    '''    Create Carousal function For 276
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreatCarousalFor276(ByVal Path As String, ByVal _RFFeed As String)

        'Create  Dyanamically bat file Create_Carousal_For276
        Dim LineText As StringBuilder = New StringBuilder()

        'Create Carousel using the section generator
        Dim command As String = ""
        Dim finaloutput As String = ""
        command = Path + "carouselsooo_276.au3"

        'run au3 file
        finaloutput = StartProcess(command, "ERROR")
        _iex.Wait(5)
        Dim dir As New DirectoryInfo(Path + "result")
        If dir.Exists Then
            _iex.LogComment("Created carousel successfully")
        Else
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.CreateCarausalFailure, "Failed to create the carousel: Result folder not generated"))
        End If
        ' Move the .dat file created outside, inside the Result folder.
        Dim fileName As String = "DSI.DAT12345678"
        Dim sourcePath As String = Path + "result"
        Dim targetPath As String = Path + "result\570AD000"

        ' Use Path class to manipulate file and directory paths.
        Dim sourceFile As String = System.IO.Path.Combine(sourcePath, fileName)
        Dim destFile As String = System.IO.Path.Combine(targetPath, fileName)

        ' To copy a file to another location and 
        ' overwrite the destination file if it already exists.
        _iex.Wait(5)
        System.IO.File.Copy(sourceFile, destFile, True)

        'copy the carousel created into 23 machine
        Dim CarrouselPath As String
        'Carousels generated are inside the folder 570AD000
        Dim dir570AD000 As New DirectoryInfo(Path + "result\570AD000")
        If _RFFeed = "NL" Then
			 _iex.LogComment("Copying the craousels to \\10.201.96.23\UPC_Carrousel_broadcast_tool")
            CarrouselPath = "\\10.201.96.23\UPC_Carrousel_broadcast_tool2\Boite\Carrousel\"
            For Each copyfile As FileInfo In dir570AD000.GetFiles
                DeleteFiles(New FileInfo(CarrouselPath + copyfile.Name))
                copyfile.CopyTo(CarrouselPath + copyfile.Name)
            Next
        Else
			 _iex.LogComment("Copying the craousels to \\10.201.96.23\BoiteTS51-4901\Carrousel")
            CarrouselPath = "\\10.201.96.23\BoiteTS51-4901\Carrousel\"
            For Each copyfile As FileInfo In dir570AD000.GetFiles
                DeleteFiles(New FileInfo(CarrouselPath + copyfile.Name))
                copyfile.CopyTo(CarrouselPath + copyfile.Name)

            Next
        End If

        'Remove locally created craousel (result folder)
        _iex.Wait(5)
        _iex.LogComment("Copied the carousels to 10.201.96.23 successfully")
        Try
            Directory.Delete(Path + "result", True)
        Catch ex As Exception
            _iex.LogComment("Unable to delete result folder" + ex.Message)
        End Try
    End Sub
	
 Public Sub CopyOldBinary()
        _Utils.StartHideFailures("Copy image from Old builds to specified path")
        Dim OldBinaryPath As String
        Try
            Path = _UI.Utils.GetValueFromProject("OTA", "PATH")
            OldBinaryPath = _UI.Utils.GetValueFromProject("OTA", "OLD_BINARY_PATH")

            Dim file As New FileInfo(OldBinaryPath)
            Try
                If file.Exists() Then
                    DeleteFiles(New FileInfo(Path + "bzimage"))
                    file.CopyTo(Path + file.Name)
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.CopyFileFailure, "Failed to find Binary at : " + OldBinaryPath))
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

#End Region

End Class
