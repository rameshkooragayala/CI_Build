Imports FailuresHandler

Namespace EAImplementation
    Public Class PowerCycle
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _SecBeforePowerOn As Integer
        Private _GetOutOfStandBy As Boolean
        Private _FormatSTB As Boolean
        Private _ImageType As String

        Sub New(ByVal SecBeforePowerOn As Integer, ByVal GetOutOfStandBy As Boolean, ByVal FormatSTB As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            Me._SecBeforePowerOn = SecBeforePowerOn
            Me._GetOutOfStandBy = GetOutOfStandBy
            Me._FormatSTB = FormatSTB
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway._IEXResult
            Dim waitBeforeTelnet As String
			
            If EPG.PowerManagement.IsPerleRpc Then
                EPG.PowerManagement.SendCommandToPerleRpc(powerUp:=False)

                EPG.Utils.LogCommentInfo("Waiting " + _SecBeforePowerOn.ToString() + " Sec Before Powering On STB")
                res = _iex.Wait(_SecBeforePowerOn)

                EPG.PowerManagement.SendCommandToPerleRpc(powerUp:=True)

            Else
                res = _iex.SendPowerCycle("OFF")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
                EPG.Utils.LogCommentInfo("Waiting " + _SecBeforePowerOn.ToString() + " Sec Before Powering On STB")
                res = _iex.Wait(_SecBeforePowerOn)

                res = _iex.SendPowerCycle("ON")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            End If

            'Waiting for sh prompt and giving the udhcpc command to enable the telnet session
            Try
                waitBeforeTelnet = EPG.Utils.GetValueFromEnvironment("WaitBeforeTelnet")
                _iex.Wait(Convert.ToDouble(waitBeforeTelnet))
            Catch
                _iex.Wait(30)
            End Try
			
			 If _manager.Project.Name.ToUpper() = "VOO" Then
                Dim cmd3 As String = "udhcpc"
                res = _iex.Debug.Write(cmd3 & vbCrLf, IEXGateway.DebugDevice.Serial)
                _iex.Wait(40)
            Else
                Dim cmd2 As String = "udhcpc -i br0 -V STB-UPC-Int -C -s /etc/udhcpc.script -q"
                res = _iex.Debug.Write(cmd2 & vbCrLf, IEXGateway.DebugDevice.Serial)
            End If
			
            If (_FormatSTB) Then
                res = _manager.MountGw(EnumMountAs.FORMAT)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            Else
                res = _manager.MountGw(EnumMountAs.NOFORMAT_NOREBOOT)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            End If

        End Sub
    End Class


End Namespace
