Imports FailuresHandler
Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub
    ''' <summary>
    '''   Gets The Flash Command From Environment.ini And Adds GW/Client Support If Needed.
    ''' </summary>
    ''' <param name="IsGw">If True It's GW</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetFlashCommand(ByVal IsGw As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As String
        Dim FlashCommand As String = ""
        Dim Path As String = ""


        'for Normal execution of the GW and IPC
        If IsLastDelivery = True Then

            Path = _UI.Utils.GetValueFromEnvironment("FlashCommand_LastBinary")

            FlashCommand = Path + "./cisco_userspace /host/MAIN.sao"
        Else
            FlashCommand = _UI.Utils.GetValueFromEnvironment("FlashCommand")

            If IsGw Then
                FlashCommand += "./cisco_userspace /host/MAIN.sao"
            Else
                FlashCommand += "./cisco_userspace /host/MAIN.sao"
            End If
        End If

        Return FlashCommand
    End Function
	
    ''' <summary>
    '''   Gets The Mount Command And Adding Values Needed To Mount 
    ''' </summary>
    ''' <param name="IsFormat">If True Adds FORMAT FORMAT_FLASH FOUR_K</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetMountCommand(ByVal IsFormat As Boolean) As String
        Dim MountCommand As String = ""

        MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommand")
        If MountCommand.Contains("nohup") Then
            MountCommand = MountCommand + " " + IIf(IsFormat, "FORMAT", "NOCLEAN") + " &>Diag_TelnetLog.txt"
        Else
            MountCommand = MountCommand + " " + IIf(IsFormat, "FORMAT", "NOCLEAN") + " &"
        End If

        Return MountCommand
    End Function
	
    ''' <summary>
    '''   Burns The Flash Image To The STB
    ''' </summary>
    ''' <param name="IsGw">If True It Is The GW Else The Client</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub BurnImage(Optional ByVal IsGw As Boolean = True, Optional ByVal IsLastDelivery As Boolean = False)
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim FlashCommand As String = ""
        Dim UsePowerCycle As Boolean
        Dim ActualLine As String = ""
        Dim WaitAfterRebootSec As Double
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Burnning Flash Image To STB ...")

        Try
            Try
                WaitAfterRebootSec = CDbl(_UI.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
            Catch ex As Exception
                WaitAfterRebootSec = 90
            End Try

            Try
                UsePowerCycle = CBool(_UI.Utils.GetValueFromEnvironment("RebootThroughIPC"))
            Catch ex As Exception
                UsePowerCycle = True
            End Try

            FlashCommand = GetFlashCommand(IsGw, IsLastDelivery)

            Res = _iex.Debug.BeginWaitForMessage("WRITE DONE", 0, 300, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            'Sending the mount command for serial
            'Dim cmd2 As String = "udhcpc -i eth0 -s /etc/udhcpc.script -q"
            'Res = _iex.Debug.Write(cmd2 & vbCrLf, IEXGateway.DebugDevice.Serial)
            'If Not Res.CommandSucceeded Then
                'ExceptionUtils.ThrowEx(New IEXException(Res))
           ' End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            _iex.Wait(60)

            Res = _iex.Debug.WriteLine(FlashCommand, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("WRITE DONE", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.BeginWaitForMessage("sh-3.2#", 0, 30, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("/bin/oneconfig.exe upgrade 0", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("sh-3.2#", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

        _UI.Utils.StartHideFailures("Rebooting And Waiting For 'adding dns' ...")

        Try
            'If UsePowerCycle Then
            '    Res = _iex.Power.Restart()
             '   If Not Res.CommandSucceeded Then
             '       ExceptionUtils.ThrowEx(New IEXException(Res))
            '    End If
           ' Else
                Dim cmd As String = "reboot -f"
                Res = _iex.Debug.Write(cmd & vbCrLf, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
           ' End If

            Res = _iex.Debug.BeginWaitForMessage("starting pid", 0, 300, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("starting pid", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
            'Reducing the wait as we dont need to wait for more time 
            _iex.Wait(80)

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            'Sending the mount command for serial
           ' Dim cmd1 As String = "udhcpc -i eth0 -s /etc/udhcpc.script -q"
            'Res = _iex.Debug.Write(cmd1 & vbCrLf, IEXGateway.DebugDevice.Serial)
            'If Not Res.CommandSucceeded Then
               ' ExceptionUtils.ThrowEx(New IEXException(Res))
            'End If

            Msg = "Burned Flash Image Successfuly To STB"
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub


    ''' <summary>
    '''    Waiting For Pormpt
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub WaitForPrompt(ByVal IsNFS As Boolean)
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""
        Dim WaitAfterRebootSec As Double
        Dim Prompt As String = ""

        _UI.Utils.StartHideFailures("Waiting For Prompt")

        If IsNFS Then
            Try
                Try
                    WaitAfterRebootSec = CDbl(_UI.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
                Catch ex As Exception
                    WaitAfterRebootSec = 50
                End Try

                _iex.Wait(WaitAfterRebootSec)

                Dim iniFile As AMS.Profile.Ini

                iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iex.IEXServerNumber.ToString + "\Telnet.ini")
                Prompt = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "prompt").ToString

                Res = _iex.Debug.BeginWaitForMessage(Prompt, 0, 120, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                Res = _iex.Debug.EndWaitForMessage(Prompt, ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Waiting For The Prompt")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            Finally
                _iex.ForceHideFailure()
            End Try
        Else
            Try
                Dim iniFile As AMS.Profile.Ini

                iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iex.IEXServerNumber.ToString + "\Telnet.ini")
                Prompt = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "prompt").ToString

                Res = _iex.Debug.BeginWaitForMessage("Please press Enter to activate this console", 0, 120, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
				
                Res = _iex.Debug.EndWaitForMessage("Please press Enter to activate this console", ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Didnt receive Messgae Please press Enter to activate this console")

                End If
				'waiting 10 seconds after we receive the milestone
                _iex.Wait(80)
				
				Res = _iex.Debug.BeginWaitForMessage(Prompt, 0, 120, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
				
                'Press Enter after CISCO REFRESH box asks to Enter
                Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                'Press Enter after CISCO REFRESH box asks to Enter
                Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                'Press Enter after CISCO REFRESH box asks to Enter
                Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                Res = _iex.Debug.EndWaitForMessage(Prompt, ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Didnt receive Wait For The Prompt")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            Finally
                _iex.ForceHideFailure()
            End Try

            Try
                WaitAfterRebootSec = CDbl(_UI.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
            Catch ex As Exception
                WaitAfterRebootSec = 90
            End Try

            _iex.Wait(WaitAfterRebootSec)
        End If



    End Sub
End Class