Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    Public Class MountClient
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _Retries As Integer
        Private _MountAs As EnumMountAs
        Private _IsLastDelivery As Boolean
        Dim IEX As IEXGateway.IEX
        Dim MountCommand As String
        Dim CurrentVersion As String
        Dim Msg As String = ""
        Dim LogFileFullName As String
        Dim UsePowerCycle As Boolean
        Dim WaitForPrompt As Boolean = True
        Dim CheckForVideoAfterMount As Boolean = True
        Dim Reboot As Boolean = True
        Private _IsReturnToLive As Boolean = True
        Private _IsNFS As Boolean = False
        Dim _WakeUp As Boolean = False

        Sub New(ByVal MountAs As EnumMountAs, ByVal Retries As Integer, ByVal IsLastDelivery As Boolean, ByVal WakeUp As Boolean, ByVal IsReturnToLive As Boolean, ByVal m As IEX.ElementaryActions.Functionality.Manager)
            _manager = m
            _MountAs = MountAs
            _Retries = Retries
            _IsLastDelivery = IsLastDelivery
            _WakeUp = WakeUp
            _IsReturnToLive = IsReturnToLive
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            If _manager.Project.MountLikeCogeco Then
                MountCOGECOClient()
            Else
                MountUPCClient()
            End If
        End Sub

        Private Function MountUPCClient()
            Dim res As IEXGateway.IEXResult
            Dim MountThroughTelnet As Boolean
            Dim IsFormat As Boolean
            Dim DoReboot As Boolean = True
            Dim _IsFactoryReset As Boolean = False
            Dim WakeUp As Boolean = False
            Try
                MountThroughTelnet = CBool(EPG.Utils.GetValueFromEnvironment("MountThroughTelnet"))
            Catch ex As Exception
                MountThroughTelnet = True
            End Try

            Select Case _MountAs
                Case EnumMountAs.FORMAT
                    IsFormat = True
                Case EnumMountAs.NOFORMAT
                    IsFormat = False
                Case EnumMountAs.FACTORY_RESET
                    IsFormat = False
                    _IsFactoryReset = True
                Case EnumMountAs.NOFORMAT_NOREBOOT
                    IsFormat = False
                    DoReboot = False
                Case EnumMountAs.NOFORMAT_WAKEUP
                    IsFormat = False
                    WakeUp = True
            End Select

            If MountThroughTelnet Then
                res = _manager.MountTelnetStb(IsGw:=False, IsFormat:=IsFormat, DoReboot:=DoReboot, Retries:=_Retries, IsFactoryReset:=_IsFactoryReset,IsLastDelivery:=_IsLastDelivery, WakeUp:=WakeUp, IsReturnToLive:=_IsReturnToLive)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountClientFailure, res.FailureReason))
                End If
            Else
                res = _manager.MountSerialStb(IsGw:=False, IsFormat:=IsFormat, DoReboot:=DoReboot, Retries:=_Retries, IsLastDelivery:=_IsLastDelivery, WakeUp:=WakeUp, IsReturnToLive:=_IsReturnToLive)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountClientFailure, res.FailureReason))
                End If
            End If

            Return True

        End Function

        Private Function MountCOGECOClient()
            Dim Passed As Boolean = True
            Dim ActualLine As String = ""
            Dim ImageType As String = ""

            Try
                CheckForVideoAfterMount = CBool(EPG.Utils.GetValueFromEnvironment("CheckForVideoAfterMount"))
            Catch ex As Exception
                CheckForVideoAfterMount = True
            End Try

            'Set Log Names
            LogFileFullName = EPG.Mount.GetLogName(IsSerial:=True)

            'Get Mount Command
            Select Case _MountAs
                Case EnumMountAs.FORMAT
                    MountCommand = EPG.Mount.GetMountCommand(IsFormat:=True)
                Case EnumMountAs.NOFORMAT, EnumMountAs.FACTORY_RESET
                    MountCommand = EPG.Mount.GetMountCommand(IsFormat:=False)
                Case EnumMountAs.NOFORMAT_NOREBOOT
                    MountCommand = EPG.Mount.GetMountCommand(IsFormat:=False)
                    Reboot = False
            End Select

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
                        _IsNFS = True
                    Case "flash"
                        WaitForPrompt = True
                    Case "production"
                        WaitForPrompt = False
                End Select
            Catch ex As Exception
                WaitForPrompt = True
            End Try

            For i As Integer = 1 To _Retries
                Passed = Mount(i, Msg)
                If Passed = False Then
                    _iex.GetSnapshot("FAILED TRY " + i.ToString + " TAKING SNAPSHOT FOR DEBUGING")
                Else
                    Exit For
                End If
            Next

            If Passed = False Then
                EPG.Mount.CloseLogs()
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountClientFailure, Msg + " (Failed To Mount STB After " + _Retries.ToString + " Tries)"))
            End If

            Return True
        End Function

        Private Function Mount(ByVal LoopNum As Integer, ByRef Msg As String) As Boolean
            Dim Res As IEXGateway.IEXResult = Nothing

            'Close Serial Or Telnet
            EPG.Mount.CloseLogs()

            'BEGIN SERIAL / TELNET LOG
            EPG.Mount.BeginLogging(IsSerial:=True, LogFileName:=LogFileFullName, LoopNum:=LoopNum)

            EPG.Utils.LogCommentInfo("Trying To Mount STB Try : " + LoopNum.ToString + " Out Of " + _Retries.ToString)

            Try
                UsePowerCycle = CBool(EPG.Utils.GetValueFromEnvironment("RebootThroughIPC"))
            Catch ex As Exception
                UsePowerCycle = True
            End Try

            If LoopNum = _Retries And _Retries <> 1 Then
                UsePowerCycle = True
            End If

            If Reboot Then
                'REBOOT 
                EPG.Mount.RebootSTB(UsePowerCycle)
            End If

            If WaitForPrompt Then

                'WAIT FOR PROMPT IF ASKED
                EPG.Mount.WaitForPrompt(IsNFS:=_IsNFS)

                If Not _IsNFS AndAlso Reboot Then
                    'CHECKS IF BURN IS NEEDED AND BURN
                    If Not EPG.Mount.UpdateSTBVersion(CurrentVersion, False) Then
                        EPG.Utils.LogCommentFail("Failed To Update Version")
                        Msg = "Failed To Update Version"
                        Return False
                    End If
                End If

                'SENDS MOUNT COMMAND TO STB
                If Not EPG.Mount.SendMountCommand(IsSerial:=True, MountCommand:=MountCommand & vbCrLf) Then
                    EPG.Utils.LogCommentFail("Failed To Send Mount Command : " + MountCommand + " To The STB")
                    Msg = "Failed To Send Mount Command : " + MountCommand + " To The STB"
                    Return False
                End If
            End If

            If Not EPG.Mount.WaitForClientToLoad Then
                Msg = "Failed To Verify DebugTextInitialize Milestones"
                Return False
            End If

            If _MountAs = EnumMountAs.NOFORMAT Then
                If EPG.Mount.WaitAfterReset Then
                    _manager.StandBy(True)
                Else
                    EPG.Utils.LogCommentFail("Failed To Verify STB Initialize After Factory Reset")
                    Msg = "Failed To Verify STB Initialize After Factory Reset"
                    Return False
                End If
            End If

            'Since in condition other than NoFormat And NoReboot first installation is needed
            If Not _MountAs = EnumMountAs.NOFORMAT_NOREBOOT AndAlso Not _MountAs = EnumMountAs.NOFORMAT Then
                'WAIT FOR OSD AND PRESS SELECT
                If EPG.Mount.WaitForFirstScreen() Then
                    If _manager.Project.IsProduction Then
                        EPG.Utils.StartHideFailures("Trying To Telnet The Client...")
                        _manager.TelnetLogIn(LoginToGw:=False)
                        _iex.ForceHideFailure()
                        Dim Version As String = EPG.Mount.GetVersionFromSTB
                        EPG.Utils.LogCommentImportant("Client Version : " + Version)
                    End If


                    EPG.Mount.PressSelect()
                Else
                    EPG.Utils.LogCommentFail("Failed To Verify OSD Appeared")
                    Msg = "Failed To Verify STB Reached OSD"
                    Return False
                End If
            End If
            If Not EPG.Mount.InitializeStb(Msg) Then
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

            Return True
        End Function

    End Class

End Namespace