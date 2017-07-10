Imports FailuresHandler

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.Mount

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Returns The Log Name
    ''' </summary>
    ''' <param name="IsSerial">If True Returns Serial Name Else Returns Telnet Name</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetLogName(ByVal IsSerial As Boolean) As String
        If IsSerial Then
            Return LCase(_iex.LogFileName).Replace(".iexlog", "_Serial.txt")
        Else
            Return LCase(_iex.LogFileName).Replace(".iexlog", "_Telnet.txt")
        End If
    End Function

    ''' <summary>
    '''   Sets The BAUDRATE Value Of The Debug
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function SetBaudRate() As Boolean
        Dim baudrate As String

        baudrate = _UI.Utils.GetBaudRateFromConfigFile(_iex.IEXServerNumber)
        If baudrate = "" Then
            _iex.Debug.SetBaudrate(115200)
        End If

        Return True
    End Function

    ''' <summary>
    '''    Gets The EPG Version From BuildWinPath
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetCurrentEPGVersion(Optional ByVal IsLastDelivery As Boolean = False) As String
        Dim iniFile As AMS.Profile.Ini
        Dim CurrentVersion As String
        Dim BuildWinPath As String
        Try
            If (IsLastDelivery) Then
                BuildWinPath = _UI.Utils.GetValueFromEnvironment("BuildWinPath_LastDelivery")
            Else
                BuildWinPath = _UI.Utils.GetValueFromEnvironment("BuildWinPath")
            End If
        Catch ex As Exception
            _UI.Utils.LogCommentWarning("WARNING : Failed To Get BuildWinPath From Environment.ini Exception : " + ex.Message)
            Return ""
        End Try
        Try
            iniFile = New AMS.Profile.Ini(BuildWinPath + "\config\version.cfg")
            _UI.Utils.LogCommentInfo("Getting Current Version From : " + BuildWinPath)
            CurrentVersion = iniFile.GetValue("VERSION", "NDS_SW_VERSION").ToString
            _UI.Utils.LogCommentBlack("MOUNTING IMAGE VERSION : " + CurrentVersion)
            Return CurrentVersion
        Catch ex As Exception
            _UI.Utils.LogCommentWarning("WARNING : Failed To Access :" + BuildWinPath + "\config\version.cfg Exception : " + ex.Message)
            Return ""
        End Try
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
    '''   Gets The Flash Command From Environment.ini And Adds GW/Client Support If Needed
    ''' </summary>
    ''' <param name="IsGw">If True It's GW</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetFlashCommand(ByVal IsGw As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As String
        Dim FlashCommand As String = ""
        Dim Path As String = ""
        If IsLastDelivery = True Then
            Path = _UI.Utils.GetValueFromEnvironment("FlashCommand_LastBinary")

            FlashCommand = Path + "./flash_update.exe /host/bzImage"
        Else
            FlashCommand = _UI.Utils.GetValueFromEnvironment("FlashCommand")

            If IsGw Then
                FlashCommand += "./flash_update.exe /host/bzImage"
            Else
                FlashCommand += "./flash_update.exe /host/bzImage"
            End If
        End If
        Return FlashCommand
    End Function

    ''' <summary>
    '''   Closes The Serial And Telnet Logging
    ''' </summary>
    ''' <remarks></remarks>
    Overrides Sub CloseLogs()
        _UI.Utils.StartHideFailures("Trying To Close Debug Logging...")
        _iex.Debug.EndLogging(IEXGateway.DebugDevice.Serial)
        _iex.ForceHideFailure()

        _UI.Utils.StartHideFailures("Trying To Close Telnet Logging...")
        _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet)
        _iex.Debug.EndLogging(IEXGateway.DebugDevice.Telnet)
        _iex.ForceHideFailure()
    End Sub

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

            Res = _iex.Debug.BeginWaitForMessage("to disable upgrade mode", 0, 300, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
			
            'Sending the mount command for serial
            Dim cmd2 As String = "udhcpc -i br0 -V STB-UPC-Int -C -s /etc/udhcpc.script -q"
            Res = _iex.Debug.Write(cmd2 & vbCrLf, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
			
            Res = _iex.Debug.WriteLine(FlashCommand, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("to disable upgrade mode", ActualLine, "", IEXGateway.DebugDevice.Serial)
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
            '    If UsePowerCycle Then
            '        Res = _iex.Power.Restart()
            '        If Not Res.CommandSucceeded Then
            '            ExceptionUtils.ThrowEx(New IEXException(Res))
            '        End If
            '    Else
                Dim cmd As String = "reboot -f"
                Res = _iex.Debug.Write(cmd & vbCrLf, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
          '  End If

            Res = _iex.Debug.BeginWaitForMessage("starting pid", 0, 300, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("starting pid", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            _iex.Wait(WaitAfterRebootSec)
			
            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            'Sending the mount command for serial
            Dim cmd1 As String = "udhcpc -i br0 -V STB-UPC-Int -C -s /etc/udhcpc.script -q"
            Res = _iex.Debug.Write(cmd1 & vbCrLf, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
			
            Msg = "Burned Flash Image Successfuly To STB"
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Begin Serial Or Telnet Logging 
    ''' </summary>
    ''' <param name="IsSerial">If True Begin Serial Logging Else Begin Telnet Logging</param>
    ''' <param name="LogFileName">The Log FileName</param>
    ''' <param name="LoopNum">The Loop Iteration For Adding It To The Name</param>
    ''' <remarks></remarks>
    Public Overrides Sub BeginLogging(ByVal IsSerial As Boolean, ByVal LogFileName As String, ByVal LoopNum As Integer)
        Dim Res As IEXGateway.IEXResult = Nothing

        If IsSerial Then
            Res = _iex.Debug.BeginLogging(LogFileName.Replace(".txt", "_" + LoopNum.ToString + ".txt"), "", False, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
        Else
            Res = _iex.Debug.BeginLogging(LogFileName.Replace(".txt", "_" + LoopNum.ToString + ".txt"), "", False, IEXGateway.DebugDevice.Telnet)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
        End If
    End Sub

    ''' <summary>
    '''   Reboot The STB And Wait If Asked
    ''' </summary>
    ''' <param name="WithIPC">If True Waits 10 Seconds Between Turn OFF And ON</param>
    ''' <remarks></remarks>
    Public Overrides Sub RebootSTB(ByVal WithIPC As Boolean)
        Dim Res As IEXGateway.IEXResult = Nothing

        _UI.Utils.StartHideFailures("Rebooting STB")

        Try
            If WithIPC Then
                Res = _iex.Power.TurnOFF
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                _iex.Wait(10)

                Res = _iex.Power.TurnOn
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            Else
                Res = _iex.Debug.Write("reboot" & vbCrLf, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checkes If Burn Needed And Burn
    ''' </summary>
    ''' <param name="CurrentVersion">Current Build Version</param>
    ''' <param name="IsGW">If True Burnning GW Else Burrning Client</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function UpdateSTBVersion(ByVal CurrentVersion As String, ByVal IsGW As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As Boolean
        Dim ActualLine As String = ""

        If IsLastDelivery = False Then
            'GET EPG VERSION THAT IS ON THE STB
            ActualLine = GetVersionFromSTB()

            If ActualLine <> "" Then
                Try
				'commenting the verification of version as we want to flash the image irrespective of the version
                    'If ActualLine.Contains(Chr(34) + CurrentVersion + Chr(34)) = False Then
                        'BURN NEW IMAGE
                        Try
                            BurnImage(IsGW)
                        Catch ex As Exception
                            _UI.Utils.LogCommentFail("Mount.BurnImage : Failed To Burn Flash Image To The STB")
                            Return False
                        End Try
                    'Else
                      '  Try
                       '     _UI.Utils.LogCommentImportant("STB Version Is The Same : " + ActualLine.Remove(0, ActualLine.IndexOf(Chr(34)).ToString.Replace(Chr(34), "")))
                      '  Catch ex As Exception
                      '  End Try
                   ' End If
                Catch ex As Exception
                    'BURN NEW IMAGE
                    Try
                        BurnImage(IsGW)
                    Catch ex2 As Exception
                        _UI.Utils.LogCommentFail("Mount.BurnImage : Failed To Burn Flash Image To The STB")
                        Return False
                    End Try
                End Try
            Else
                Return False
            End If

        Else
            Try
                BurnImage(IsGW, IsLastDelivery)
            Catch ex2 As Exception
                _UI.Utils.LogCommentFail("Mount.BurnImage : Failed To Burn Flash Image To The STB")
                Return False
            End Try
        End If
        Return True
    End Function

    ''' <summary>
    '''    Get Version From STB
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetVersionFromSTB() As String
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""

        _UI.Utils.StartHideFailures("Comparing EPG Versions And Checking If Burn Is Needed")

        Try

            Res = _iex.Debug.BeginWaitForMessage("NDS_SW_VERSION", 0, 10, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                Return ""
            End If

            Res = _iex.Debug.Write("cd NDS/config;cat version.cfg" + vbCrLf, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("NDS_SW_VERSION", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Verify NDS_SW_VERSION")
                Return ""
            End If

            Return ActualLine

        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Sends Mount Command To Telnet
    ''' </summary>
    ''' <param name="MountCommand">The Mount Command To Send</param>
    ''' <param name="IsSerial">If True Sends The Command Through Serial Else Through Telnet</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function SendMountCommand(ByVal IsSerial As Boolean, ByVal MountCommand As String, Optional ByVal IsFormat As Boolean = False) As Boolean
        Dim Res As IEXGateway.IEXResult = Nothing

        _UI.Utils.StartHideFailures("Sending Mount Command...")

        Try
            If IsSerial Then
                Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Serial)
            Else
                Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Telnet)
            End If
            'SEND MOUNT COMMAND

            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Send Command : " + MountCommand.ToString)
                Return False
            Else
                _UI.Utils.LogCommentInfo("Mount Command Sent Successfuly")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

        Return True
    End Function

    ''' <summary>
    '''   Initilizing The STB After STB Passed The First Screen
    ''' </summary>
    ''' <param name="Msg">Returned Error Message</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function InitializeStb(ByRef Msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean
        'WAIT FOR MAIN MENU TO ARRIVE

        If Not _UI.Utils.VerifyState("MAIN MENU", 60, 5) Then
            _UI.Utils.SendIR("MENU")
            If Not _UI.Utils.VerifyState("MAIN MENU", 60, 5) Then
                _UI.Utils.LogCommentFail("Failed To Verify MAIN MENU Is On Screen")
                Msg = "Failed To Verify MAIN MENU Is On Screen After Mount"
                Return False
            End If
        End If

        'NAVIGATE TO LIVE
        If IsReturnToLive Then
            Try
                _UI.Utils.EPG_Milestones_Navigate("LIVE")
            Catch ex As IEXException
                _UI.Utils.LogCommentFail("Failed To Verify Live On Screen")
                Msg = "Failed To Verify Live On Screen"
                Return False
            End Try
        End If
        Return True

    End Function

    ''' <summary>
    '''   Waiting For First Screen To Appear On EPG
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function WaitForFirstScreen() As Boolean
        Dim ActualLines As New ArrayList

        _UI.Utils.LogCommentInfo("Waiting For title To Appear...")

        _UI.Utils.BeginWaitForDebugMessages("title", 300)

        If Not _UI.Utils.EndWaitForDebugMessages("title", ActualLines) Then
            _UI.Utils.LogCommentFail("Failed To Verify title Arrived")
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    '''   Waits For Legal Disclaimer Screen To Appear
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function WaitForLegalDisclaimer() As Boolean
        Dim EpgText As String = ""

        _UI.Utils.StartHideFailures("Checking If Legal Disclaimer Screen Appear...")
        Try
            EpgText = _UI.Utils.GetValueFromDictionary("DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_ACCEPT")
        Catch ex As Exception
            _UI.Utils.LogCommentFail("EPG Text For DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_ACCEPT Not Found On Dictionary")
            _iex.ForceHideFailure()
            Return False
        End Try

        If _UI.Utils.VerifyDebugMessage("title", EpgText, 600, 2) Then
            _iex.ForceHideFailure()
            Return True
        End If

        _iex.ForceHideFailure()
        Return False
    End Function

    ''' <summary>
    '''   Waits For Standby Power Screen To Appear
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function WaitForStandbyPowerScreen() As Boolean
        Dim EpgText As String = ""

        _UI.Utils.StartHideFailures("Checking If Standby Power mode Screen Appears...")

        Try
            EpgText = _UI.Utils.GetValueFromDictionary("DIC_STANDBY_POWER_MODE_OPTION1")
        Catch ex As Exception
            Try
                EpgText = _UI.Utils.GetValueFromDictionary("DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_REJECT")
            Catch ex1 As Exception
                _UI.Utils.LogCommentFail("EPG Text For DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_REJECT Not Found In Dictionary")
                Return False
            End Try
        End Try
        

        If _UI.Utils.VerifyDebugMessage("title", EpgText, 600, 2) Then 
            Return True 
        End If 
        _iex.ForceHideFailure() 

        Return False 


    End Function

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
                Res = _iex.Debug.BeginWaitForMessage("starting pid", 0, 120, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                Res = _iex.Debug.EndWaitForMessage("starting pid", ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Waiting For The Prompt")
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