Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    Public Class MountGw
        Inherits IEX.ElementaryActions.BaseCommand

        Private reason As String
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _MountAs As EnumMountAs
        Private _Retries As Integer
        Private _IsLastDelivery As Boolean
        Private LogFileFullName As String = ""
        Private WaitForPrompt As Boolean = True
        Private UsePowerCycle As Boolean = True
        Private _IsNFS As Boolean = False
        Private _IsReturnToLive As Boolean = True
        Dim MountCommand As String
        Dim CurrentVersion As String
        Dim Msg As String = ""
        Dim Reboot As Boolean = True
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
                MountCOGECOGw()
            Else
                MountUPCGw()
            End If
        End Sub

        Private Function MountUPCGw()
            Dim res As IEXGateway.IEXResult
            Dim MountThroughTelnet As Boolean
            Dim IsFormat As Boolean
            Dim DoReboot As Boolean = True
            Dim IsFactoryReset As Boolean
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
                    IsFactoryReset = True
                Case EnumMountAs.NOFORMAT_NOREBOOT
                    IsFormat = False
                    DoReboot = False
                Case EnumMountAs.NOFORMAT_WAKEUP
                    IsFormat = False
                    WakeUp = True
            End Select

            If MountThroughTelnet Then
                res = _manager.MountTelnetStb(IsGw:=True, IsFormat:=IsFormat, DoReboot:=DoReboot, IsFactoryReset:=IsFactoryReset, Retries:=_Retries, IsLastDelivery:=_IsLastDelivery, WakeUp:=WakeUp, IsReturnToLive:=_IsReturnToLive)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountGwFailure, res.FailureReason))
                End If
            Else
                res = _manager.MountSerialStb(IsGw:=True, IsFormat:=IsFormat, DoReboot:=DoReboot, Retries:=_Retries, IsLastDelivery:=_IsLastDelivery, WakeUp:=WakeUp, IsReturnToLive:=_IsReturnToLive)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountGwFailure, res.FailureReason))
                End If
            End If

            Return True
        End Function

        Private Function MountCOGECOGw()
            'Dim res As IEXGateway.IEXResult
            Dim MountThroughTelnet As Boolean
            Dim IsFormat As Boolean
            Dim DoReboot As Boolean = True
            Dim IsFactoryReset As Boolean
            Dim WakeUp As Boolean = False

            Dim Res As IEXGateway.IEXResult = Nothing
            Try
                MountThroughTelnet = CBool(EPG.Utils.GetValueFromEnvironment("MountThroughTelnet"))
            Catch ex As Exception
                MountThroughTelnet = True
            End Try
            'Set Log Names
            '  LogFileFullName = EPG.Mount.GetLogName(IsSerial:=True)

            'Get Mount Command
            Select Case _MountAs
                Case EnumMountAs.FORMAT
                    IsFormat = True
                    'MountCommand = EPG.Mount.GetMountCommand(IsFormat:=True)
                Case EnumMountAs.NOFORMAT, EnumMountAs.FACTORY_RESET
                    IsFormat = False
                    IsFactoryReset = True
                    ' MountCommand = EPG.Mount.GetMountCommand(IsFormat:=False)
                Case EnumMountAs.NOFORMAT_NOREBOOT
                    ' MountCommand = EPG.Mount.GetMountCommand(IsFormat:=False)
                    IsFormat = False
                    DoReboot = False
                    '  Reboot = False
            End Select

            If MountThroughTelnet Then
                Res = _manager.MountTelnetStb(IsGw:=True, IsFormat:=IsFormat, DoReboot:=DoReboot, IsFactoryReset:=IsFactoryReset, Retries:=_Retries, IsLastDelivery:=_IsLastDelivery, WakeUp:=WakeUp, IsReturnToLive:=_IsReturnToLive)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountGwFailure, Res.FailureReason))
                End If
            Else
                Res = _manager.MountSerialStb(IsGw:=True, IsFormat:=IsFormat, DoReboot:=DoReboot, Retries:=_Retries, IsLastDelivery:=_IsLastDelivery, WakeUp:=WakeUp, IsReturnToLive:=_IsReturnToLive)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountGwFailure, Res.FailureReason))
                End If
            End If
            Return True

        End Function
    End Class

End Namespace