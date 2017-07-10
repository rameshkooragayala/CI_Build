Imports FailuresHandler

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub
  
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
            BuildWinPath = _UI.Utils.GetValueFromEnvironment("BuildWinPath")
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
        FlashCommand = _UI.Utils.GetValueFromEnvironment("FlashCommand")

        If IsGw Then
            FlashCommand += "./flash_gateway.exe /host/drivers/bzImage"
        Else
            FlashCommand += "./flash_client.exe /host/drivers/bzImage"
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

            FlashCommand = _UI.Utils.GetValueFromEnvironment("FlashCommand")

            ' FlashCommand += "./banker commit /host/drivers/FFFFFFFF.sao -bMAIN -mM5F"
            FlashCommand += "./banker commit /host/FFFFFFFF.sao -bMAIN -mM5F"

            Res = _iex.Debug.BeginWaitForMessage("Success.", 0, 120, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
			
            'Sending the mount command for serial
            Dim cmd2 As String = "udhcpc"
            Res = _iex.Debug.Write(cmd2 & vbCrLf, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            _iex.Wait(20)

            Res = _iex.Debug.WriteLine(FlashCommand, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("Success.", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

        _UI.Utils.StartHideFailures("Rebooting And Waiting For 'adding dns' ...")

        Try
            If UsePowerCycle Then
                Res = _iex.Power.Restart()
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            Else
                Dim cmd As String = "reboot -f"
                Res = _iex.Debug.Write(cmd & vbCrLf, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            End If
            _UI.Utils.LogCommentInfo("commenting seraching for dns second time")
            'Res = _iex.Debug.BeginWaitForMessage("adding dns", 0, 300, IEXGateway.DebugDevice.Serial)
            'If Not Res.CommandSucceeded Then
            '    ExceptionUtils.ThrowEx(New IEXException(Res))
            'End If

            'Res = _iex.Debug.EndWaitForMessage("adding dns", ActualLine, "", IEXGateway.DebugDevice.Serial)
            'If Not Res.CommandSucceeded Then
            '    ExceptionUtils.ThrowEx(New IEXException(Res))
            'End If

            _iex.Wait(150)
            _iex.Wait(WaitAfterRebootSec)
			
            Dim cmd3 As String = "udhcpc"
            Res = _iex.Debug.Write(cmd3 & vbCrLf, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
			
			Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            _iex.Wait(20)

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

        'GET EPG VERSION THAT IS ON THE STB
        ActualLine = GetVersionFromSTB()

        If ActualLine <> "" Then
            Try
                If ActualLine.Contains(Chr(34) + CurrentVersion + Chr(34)) = False Then
                    'BURN NEW IMAGE
                    Try
                        BurnImage(IsGW)
                    Catch ex As Exception
                        _UI.Utils.LogCommentFail("Mount.BurnImage : Failed To Burn Flash Image To The STB")
                        Return False
                    End Try
                Else
                    Try
                        _UI.Utils.LogCommentImportant("STB Version Is The Same : " + ActualLine.Remove(0, ActualLine.IndexOf(Chr(34)).ToString.Replace(Chr(34), "")))
                    Catch ex As Exception
                    End Try
                End If
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
    '''   Initializing The STB After STB Passed The First Screen
    ''' </summary>
    ''' <param name="Msg">Returned Error Message</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function InitializeStb(ByRef Msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean
        'WAIT FOR MAIN MENU TO ARRIVE

        If Not _UI.Utils.VerifyState("MAIN MENU", 600, 5) Then
            _UI.Utils.LogCommentFail("Failed To Verify MAIN MENU Is On Screen")
            Msg = "Failed To Verify MAIN MENU Is On Screen After Mount"
            Return False
        End If

        'NAVIGATE TO LIVE
        Try
            _UI.Utils.EPG_Milestones_Navigate("LIVE")
        Catch ex As IEXException
            _UI.Utils.LogCommentFail("Failed To Verify Live On Screen")
            Msg = "Failed To Verify Li ve On Screen"
            Return False
        End Try
        _iex.Wait(120)

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

        If _UI.Utils.VerifyDebugMessage("title", EpgText, 1800, 2) Then
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
            EpgText = _UI.Utils.GetValueFromDictionary("DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_REJECT")
        Catch ex As Exception
            _UI.Utils.LogCommentFail("EPG Text For DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_REJECT Not Found In Dictionary")
            Return False
        End Try

        If _UI.Utils.VerifyDebugMessage("title", EpgText, 200, 2) Then
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

        _UI.Utils.StartHideFailures("Adding some wait and commenting dns")
        'Try
        '    Res = _iex.Debug.BeginWaitForMessage("adding dns", 0, 120, IEXGateway.DebugDevice.Serial)
        '    If Not Res.CommandSucceeded Then
        '        _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
        '        ExceptionUtils.ThrowEx(New IEXException(Res))
        '    End If

        '    Res = _iex.Debug.EndWaitForMessage("adding dns", ActualLine, "", IEXGateway.DebugDevice.Serial)
        '    If Not Res.CommandSucceeded Then
        '        _UI.Utils.LogCommentFail("Waiting For The Prompt")
        '        ExceptionUtils.ThrowEx(New IEXException(Res))
        '    End If
        'Finally
        '    _iex.ForceHideFailure()
        'End Try

        Try
            WaitAfterRebootSec = CDbl(_UI.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
        Catch ex As Exception
            WaitAfterRebootSec = 200
        End Try

        _iex.Wait(WaitAfterRebootSec)

        Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
        If Not Res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(Res))
        End If
        _iex.Wait(2)
        Dim cmd2 As String = "udhcpc"
        Res = _iex.Debug.Write(cmd2 & vbCrLf, IEXGateway.DebugDevice.Serial)
        If Not Res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(Res))
        End If
        _iex.Wait(2)
        Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
        If Not Res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(Res))
        End If
        _iex.Wait(20)
    End Sub

    ''' <summary>
    '''   Sends Mount Command To Telnet
    ''' </summary>
    ''' <param name="MountCommand">The Mount Command To Send</param>
    ''' <param name="IsSerial">If True Sends The Command Through Serial Else Through Telnet</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function SendMountCommand(ByVal IsSerial As Boolean, ByVal MountCommand As String, Optional ByVal IsFormat As Boolean = False) As Boolean
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""

        _UI.Utils.StartHideFailures("Sending Mount Command...")

        Try
            For i As Integer = 1 To 3

                _UI.Utils.LogCommentInfo("Trying To Send Mount Command Try : " + i.ToString)

                Res = _iex.Debug.BeginWaitForMessage("Connection refused", 0, 3, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If


                'SEND MOUNT COMMAND
                Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Send Command : " + MountCommand.ToString)
                    Return False
                Else
                    _UI.Utils.LogCommentInfo("Mount Command Sent Successfuly")
                End If


                Res = _iex.Debug.EndWaitForMessage("Connection refused", ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    Return True
                Else
                    _UI.Utils.LogCommentFail("'Connection Refused' Restarting The 'Server for NFS' Service")
                    ''''Restart the NFS Server''''
                    _UI.Utils.StartStopService("Server for NFS")
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try
        Return True
    End Function

    ''' <summary>
    '''   Handling New PIN Screens (Inserting The PIN Twice)
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>323 - VerifyStateFailure</para>
    ''' <para>328 - INIFailure</para>
    ''' </remarks>
    Overrides Sub HandlePinScreens()

        _UI.Utils.LogCommentInfo("Handle pin screen disabled on factory reset")

    End Sub


    ''' <summary>
    '''   Handles All The Screens Before LIVE/MENU Arrive
    ''' </summary>
    ''' <param name="_IsGw">If True Handles GW First Screens Else The Client</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function HandleFirstScreens(ByVal _IsGw As Boolean) As Boolean
        Dim CheckLanguageSelection As Boolean = True
        Dim CheckLegalDisclaimer As Boolean = False

        Try
            CheckLanguageSelection = CBool(_UI.Utils.GetValueFromEnvironment("CheckLanguageSelection"))
        Catch ex As Exception
            CheckLanguageSelection = True
        End Try

        Try
            CheckLegalDisclaimer = CBool(_UI.Utils.GetValueFromEnvironment("CheckLegalDisclaimer"))
        Catch ex As Exception
            CheckLegalDisclaimer = True
        End Try

        If CheckLanguageSelection And _IsGw Then

            If Not _UI.Mount.WaitForFirstScreen() Then
                _UI.Utils.LogCommentFail("Failed To Verify First Screen Arrived")
                Return False
            End If

            Try
                _UI.Menu.SetLanguage("English")
            Catch ex As Exception
                _UI.Utils.LogCommentFail("Failed To Verify Language Is English : " + ex.Message)
                Return False
            End Try

            _iex.Wait(5)
            _UI.Utils.SendIR("SELECT")

            If CheckLegalDisclaimer Then
                If WaitForLegalDisclaimer() Then
                    _UI.Utils.SendIR("SELECT")
                End If
            End If

        End If

        _UI.Utils.LogCommentImportant("Handling pin screen without language for voo")

        _UI.Mount.HandlePinScreens()

        If WaitForStandbyPowerScreen() Then
            _UI.Utils.SendIR("SELECT")
        Else
            _UI.Utils.LogCommentInfo("Sending IR To Restore The Menu") 'Waited For Standby Power Screen Unsuccessfully, So Meanwhile Menu Is Down, Therefore Need To Get It Up Again
            _UI.Utils.SendIR("MENU")
        End If

        Return True

    End Function
End Class