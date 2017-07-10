Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    Public Class MountSerialStb
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _IsFormat As Boolean
        Private _DoReboot As Boolean
        Private _IsGw As Boolean
        Private _Retries As Integer
        Private _IsLastDelivery As Boolean
		Private _WakeUp As Boolean = False
        Private WaitForPrompt As Boolean = True
        Private DebugLogFileFullName As String = ""
        Private UsePowerCycle As Boolean
        Private CheckForVideoAfterMount As Boolean = True
        Private CurrentVersion As String = ""
        Private MountCommand As String = ""
        Private Msg As String = ""
        Private _IsReturnToLive As Boolean = True
        Private _isNFS As Boolean = False

        Sub New(ByVal IsGw As Boolean, ByVal IsFormat As Boolean, ByVal DoReboot As Boolean, ByVal Retries As Integer, ByVal IsLastDelivery As Boolean,ByVal WakeUp As Boolean, ByVal IsReturnToLive As Boolean, ByVal m As IEX.ElementaryActions.Functionality.Manager)
            _manager = m
            _IsFormat = IsFormat
            _DoReboot = DoReboot
            _Retries = Retries
            _IsGw = IsGw
            _IsLastDelivery = IsLastDelivery
            _IsReturnToLive = IsReturnToLive
			_WakeUp = WakeUp
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            If _manager.Project.MountLikeCogeco Then
                MountSerialCOGECOGw(_IsFormat)
            Else
                MountSerialUPCGw()
            End If
        End Sub
        Private Function MountSerialUPCGw()
            Dim Passed As Boolean = True
            Dim ImageType As String = ""

            EPG.Utils.WaitAfterIRFactor = 2
            EPG.Utils.BeginWaitForMessageFactor = 2

            Try
                UsePowerCycle = CBool(EPG.Utils.GetValueFromEnvironment("RebootThroughIPC"))
            Catch ex As Exception
                UsePowerCycle = True
            End Try

            Try
                CheckForVideoAfterMount = CBool(EPG.Utils.GetValueFromEnvironment("CheckForVideoAfterMount"))
            Catch ex As Exception
                CheckForVideoAfterMount = True
            End Try

            DebugLogFileFullName = EPG.Mount.GetLogName(IsSerial:=True)

            'Get Mount Command
            MountCommand = EPG.Mount.GetMountCommand(IsFormat:=_IsFormat)

            'Set BaudRate 
            EPG.Mount.SetBaudRate()

            'Get Current EPG Version
            CurrentVersion = EPG.Mount.GetCurrentEPGVersion

            'Get Image Type 
            Try
                ImageType = EPG.Utils.GetValueFromEnvironment("ImageType")
                Select Case ImageType.ToLower
                    Case "nfs"
                        WaitForPrompt = True
                        _isNFS = True
                    Case "flash"
                        WaitForPrompt = True
                    Case "production"
                        WaitForPrompt = False
                End Select
            Catch ex As Exception
                WaitForPrompt = True
            End Try

            For i As Integer = 1 To _Retries
                Passed = MountGW(i, Msg)
                If Passed = False Then
                    _iex.GetSnapshot("FAILED TRY " + i.ToString + " TAKING SNAPSHOT FOR DEBUGING")
                Else
                    Exit For
                End If
            Next

            If Passed = False Then
                EPG.Mount.CloseLogs()
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountFailure, Msg + " (Failed To Mount STB After 3 Tries)"))
            End If
            Return True
        End Function

        Private Function MountGW(ByVal LoopNum As Integer, ByRef Msg As String) As Boolean
            Dim BurnedPassed As Boolean = True
            Dim Res As IEXGateway.IEXResult = Nothing

            'Close Serial And Telnet
            EPG.Mount.CloseLogs()

            'BEGIN SERIAL / TELNET LOG
            EPG.Mount.BeginLogging(IsSerial:=True, logFileName:=DebugLogFileFullName, loopNum:=LoopNum)

            EPG.Utils.LogCommentInfo("Trying To Mount STB Try : " + LoopNum.ToString + " Out Of " + _Retries.ToString)

            If _DoReboot Then
                'REBOOT STB
                EPG.Mount.RebootSTB(UsePowerCycle)
            Else
                If _manager.Project.Name.ToUpper() = "VOO" Then
                    'Sending the udhcpc command for serial
                    Dim cmd3 As String = "udhcpc"
                    Res = _iex.Debug.Write(cmd3 & vbCrLf, IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If

                    Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If

                    _iex.Wait(40)
                End If
                WaitForPrompt = False
            End If

            If WaitForPrompt Then

                'WAIT FOR PROMPT IF ASKED
                EPG.Mount.WaitForPrompt(IsNFS:=_isNFS)

                'CHECKS IF BURN IS NEEDED AND BURN
                If Not EPG.Mount.UpdateSTBVersion(CurrentVersion, _IsGw, _IsLastDelivery) Then
                    EPG.Utils.LogCommentFail("Failed To Update Version")
                    Msg = "Failed To Update Version"
                    Return False
                End If

                'Get Current EPG Version
                CurrentVersion = EPG.Mount.GetCurrentEpgVersion(_IsLastDelivery)
                Dim ActualLine As String = ""

                'GET EPG VERSION THAT IS ON THE STB
                ActualLine = EPG.Mount.GetVersionFromStb()

                If ActualLine = "" OrElse ActualLine.Contains(Chr(34) + CurrentVersion + Chr(34)) = False Then
                    If ActualLine = "" Then
                        EPG.Utils.LogCommentFail("Failed To Get Version From STB")
                        Msg = "Failed To Get Version From STB"
                    Else
                        EPG.Utils.LogCommentFail("STB Version : " + ActualLine.Remove(0, ActualLine.IndexOf(Chr(34)).ToString.Replace(Chr(34), "")) + " Is Different Then Build Version : " + CurrentVersion)
                        Msg = "STB Version : " + ActualLine.Remove(0, ActualLine.IndexOf(Chr(34)).ToString.Replace(Chr(34), "")) + " Is Different Then Build Version : " + CurrentVersion
                    End If
                    Return False
                End If

                EPG.Utils.LogCommentImportant("STB Version Is The Same : " + ActualLine.Remove(0, ActualLine.IndexOf(Chr(34)).ToString.Replace(Chr(34), "")))

                'SENDS MOUNT COMMAND TO STB
                If Not EPG.Mount.SendMountCommand(IsSerial:=True, mountCommand:=MountCommand & vbCrLf) Then
                    EPG.Utils.LogCommentFail("Failed To Send Mount Command : " + MountCommand + " To The STB")
                    Msg = "Failed To Send Mount Command : " + MountCommand + " To The STB"
                    Return False
                End If

            End If

            If Not EPG.Mount.WaitForClientToLoad Then
                Msg = "Failed To Verify DebugTextInitialize Milestones"
                Return False
            End If

            Dim returnedString As String = ""

            If _IsFormat Then
                'EPG TEXT
                Try
                    EPG.Utils.ClearEPGInfo()
                Catch ex As Exception
                    EPG.Utils.LogCommentFail("Failed To Clear EPG Info : " + ex.Message)
                    Msg = "Failed To Clear IEX EPG Info"
                End Try

                Try
                    If Not EPG.Mount.HandleFirstScreens(_IsGw) Then
                        EPG.Utils.LogCommentFail("Failed To Handle First Screens")
                        Msg = "Failed To Handle First Screens"
                    End If
                Catch ex As Exception
                    EPG.Utils.LogCommentFail("Failed To Handle First Screens : " + ex.Message)
                    Msg = "Failed To Handle First Screens"
                End Try

                If Not EPG.Mount.InitializeStb(Msg, _IsReturnToLive) Then
                    EPG.Utils.LogCommentFail(Msg)
                    Return False
                End If

                If CheckForVideoAfterMount Then
                    Res = _manager.CheckForVideo(True, False, 120)
                    If Not Res.CommandSucceeded Then
                        EPG.Utils.LogCommentFail("Failed To Verify Video Present On Stb : " + Res.FailureReason)
                        Msg = "Failed To Verify Video Present On Stb"
                        Return False
                    End If
                End If
                If _IsReturnToLive Then
                    EPG.Utils.ReturnToLiveViewing(False)
                End If
            End If
            If _WakeUp Then
                Try
                    _iex.Wait(120)
                    _manager.StandBy(True)
                    _iex.Wait(60)
                    Dim acceptScreen As String = ""
                    If EPG.Utils.GetEpgInfo("title", acceptScreen) Then
                        If acceptScreen = "ACCEPT" Then
                            EPG.Utils.LogCommentInfo("Selecting Accept Screen")
                            EPG.Utils.SendIR("SELECT")
                        End If
                    End If
                    _iex.Wait(15)
                    Res = _manager.CheckForVideo(True, False, 120)
                    If Not Res.CommandSucceeded Then
                        EPG.Utils.LogCommentFail("Failed To Verify Video Present On Stb : " + Res.FailureReason)
                        Msg = "Failed To Verify Video Present On Stb"
                        Return False
                    End If
                Catch ex As Exception
                    EPG.Utils.LogCommentFail("Failed To move out of stand by : " + ex.Message)
                End Try


            End If
            Return True

        End Function


        Private Function MountSerialCOGECOGw(ByVal IsFormat As Boolean)
            Dim Passed As Boolean = True
            Dim ImageType As String = ""

            Try
                EPG.Utils.LogCommentInfo("Mount.CopyImage : Copy Flash Image To The TFTP PATH")
                EPG.Mount.CopyBinary()
            Catch ex As Exception
                EPG.Utils.LogCommentFail("Mount.CopyImage : Failed To Copy Flash Image To The STB")
            End Try
			'Close Serial And Telnet
            EPG.Mount.CloseLogs()

            'BEGIN SERIAL / TELNET LOG
            DebugLogFileFullName = EPG.Mount.GetLogName(IsSerial:=True)

            'Get BaudRate 
            EPG.Mount.SetBaudRate()

            'Get Mount Command
            MountCommand = EPG.Mount.GetMountCommand(IsFormat:=_IsFormat)

            'Get Current EPG Version
            CurrentVersion = EPG.Mount.GetCurrentEpgVersion
            'Get Image Type 
            Try
                ImageType = EPG.Utils.GetValueFromEnvironment("ImageType")
                Select Case ImageType.ToLower
                    Case "nfs"
                        WaitForPrompt = True
                        _isNFS = True
                    Case "flash"
                        WaitForPrompt = True
                    Case "production"
                        WaitForPrompt = False
                End Select
            Catch ex As Exception
                WaitForPrompt = True
            End Try

            For i As Integer = 1 To _Retries
                Passed = Mount(i, Msg, IsFormat)
                If Passed = False Then
                    _iex.GetSnapshot("FAILED TRY " + i.ToString + " TAKING SNAPSHOT FOR DEBUGING")
                Else
                    Exit For
                End If
            Next

            If Passed = False Then
                EPG.Mount.CloseLogs()
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountGwFailure, Msg + " (Failed To Mount STB After 3 Tries)"))
            End If
            Return True
        End Function

        Private Function Mount(ByVal LoopNum As Integer, ByRef Msg As String, ByVal IsFormat As Boolean) As Boolean
            Dim Res As IEXGateway.IEXResult = Nothing
            Dim _IsGw As Boolean = True
            Dim DoReboot As Boolean = True
            Dim IsFactoryReset As Boolean
            Dim DoTune As String = ""
            Dim DefaultChannel As Service

            'Select Case _MountAs
            '    Case EnumMountAs.FORMAT
            '        IsFormat = True
            '    Case EnumMountAs.NOFORMAT
            '        IsFormat = False
            '    Case EnumMountAs.FACTORY_RESET
            '        IsFormat = False
            '        IsFactoryReset = True
            '    Case EnumMountAs.NOFORMAT_NOREBOOT
            '        IsFormat = False
            '        DoReboot = False
            'End Select

            'Close Serial And Telnet
            EPG.Mount.CloseLogs()

            'BEGIN SERIAL / TELNET LOG
            EPG.Mount.BeginLogging(IsSerial:=True, logFileName:=DebugLogFileFullName, loopNum:=LoopNum)

            EPG.Utils.LogCommentInfo("Trying To Mount STB Try : " + LoopNum.ToString + " Out Of " + _Retries.ToString)

            Try
                UsePowerCycle = CBool(EPG.Utils.GetValueFromEnvironment("RebootThroughIPC"))
            Catch ex As Exception
                UsePowerCycle = True
            End Try

            If LoopNum = _Retries And _Retries <> 1 Then
                UsePowerCycle = True
            End If

            If _DoReboot Then
                'REBOOT 
                EPG.Mount.RebootSTB(UsePowerCycle)
            End If

            If WaitForPrompt Then
                'WAIT FOR PROMPT


                Try
                    EPG.Mount.WaitForPrompt(IsNFS:=_isNFS)

                Catch ex As Exception

                    If ex.GetType.ToString().Equals("FailuresHandler.IEXException") Then

                        EPG.Utils.LogCommentFail(ex.Message)
                        EPG.Utils.LogCommentInfo("Waiting for 10s before retrying to mount again")
                        _iex.Wait(10)

                        Return False
                    End If
                End Try

                If Not _isNFS AndAlso _DoReboot Then
                    'CHECKS IF BURN IS NEEDED AND BURN
                    If Not EPG.Mount.UpdateSTBVersion(CurrentVersion, True) Then
                        EPG.Utils.LogCommentFail("Failed To Update Version")
                        Msg = "Failed To Update Version"
                        Return False
                    End If

                End If
            End If
            'SENDS MOUNT COMMAND TO STB
            If IsFormat Then
                If Not EPG.Mount.SendMountCommand(IsSerial:=True, mountCommand:=MountCommand & vbCrLf, IsFormat:=True) Then
                    EPG.Utils.LogCommentFail("Failed To Send Mount Command : " + MountCommand + " To The STB")
                    Msg = "Failed To Send Mount Command : " + MountCommand + " To The STB"
                    Return False
                End If
            Else
                If Not EPG.Mount.SendMountCommand(IsSerial:=True, mountCommand:=MountCommand & vbCrLf) Then
                    EPG.Utils.LogCommentFail("Failed To Send Mount Command : " + MountCommand + " To The STB")
                    Msg = "Failed To Send Mount Command : " + MountCommand + " To The STB"
                    Return False
                End If
            End If

            If _manager.Project.IsProduction Then
                EPG.Utils.StartHideFailures("Trying To Telnet The GW...")
                For i As Integer = 0 To 10
                    Try
                        If Me._manager.TelnetLogIn() Then
                            Exit For
                        End If
                    Catch ex As Exception
                    End Try
                    _iex.Wait(60)
                Next
                _iex.ForceHideFailure()

                Dim version As String = EPG.Mount.GetVersionFromStb()
                EPG.Utils.LogCommentImportant("GW Version : " + version)
            End If
            If _manager.Project.GwHasEPG Then
                If IsFormat Then

                    Try
                        EPG.Utils.ClearEPGInfo()
                    Catch ex As Exception
                        EPG.Utils.LogCommentFail("Failed To Clear EPG Info : " + ex.Message)
                        Msg = "Failed To Clear IEX EPG Info"
                    End Try

                    If _manager.Project.HasFTIScreens Then
                        Try
                            If Not EPG.Mount.HandleFirstScreens(_IsGw) Then
                                EPG.Utils.LogCommentFail("Failed To Handle First Screens")
                                Msg = "Failed To Handle First Screens"
                                Return False
                            End If
                        Catch ex As Exception
                            EPG.Utils.LogCommentFail("Failed To Handle First Screens : " + ex.Message)
                            Msg = "Failed To Handle First Screens"
                        End Try
                    End If
                End If
                If _manager.Project.Name = "GET" Then
                    If Not IsFormat Then
                        Try
                            _iex.Wait(600)
                            _manager.StandBy(True)
                            _iex.Wait(60)
                        Catch ex As Exception
                            EPG.Utils.LogCommentFail("Unable to get out of stand by : " + ex.Message)
                        End Try
                        If Not EPG.Utils.VerifyState("MAIN MENU", 60) Then
                            EPG.Utils.LogCommentFail("Failed To Verify MAIN MENU Is On Screen")
                        End If

                    End If

                End If

                If Not EPG.Mount.InitializeStb(Msg) Then
                    EPG.Utils.LogCommentFail(Msg)
                    Return False
                End If

                Try
                    DoTune = EPG.Utils.GetValueFromProject("MOUNT", "DO_TUNE_AFTER_MOUNT")
                Catch
                    EPG.Utils.LogCommentInfo("'DO_TUNE_AFTER_MOUNT' is empty in Section 'MOUNT'.So not tuning to any channel after Mount")
                    DoTune = "False"
                End Try

                If Convert.ToBoolean(DoTune) Then
                    DefaultChannel = _manager.GetServiceFromContentXML("IsDefault=True")
                    If IsNothing(DefaultChannel) Then
                        EPG.Utils.LogCommentFail("Failed to find Channel for 'Default=True' in Content.xml")
                        Return False
                    End If
                    If Not EPG.Mount.TuneToDefaultChannel(DefaultChannel.LCN) Then
                        Res = _manager.ChannelSurf(EnumSurfIn.Live, DefaultChannel.LCN)
                        If Not Res.CommandSucceeded Then
                            EPG.Utils.LogCommentFail("Failed to tune to Default Channel: " + DefaultChannel.LCN)
                            Return False
                        End If
                    End If
                End If

            Else

                If Not EPG.Mount.WaitForGWToLoad Then
                    EPG.Utils.LogCommentFail("Failed To Verify TunerLocked Milestones")
                    Msg = "Failed To Verify TunerLocked Milestones"
                    Return False
                End If
            End If
            Return True

        End Function
    End Class
End Namespace