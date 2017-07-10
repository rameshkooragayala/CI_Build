Imports FailuresHandler

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    Public Overrides Sub WaitForPrompt(ByVal IsNFS As Boolean)
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""
        Dim WaitAfterRebootSec As Double
        Dim Prompt As String = ""
        Dim Username As String = ""
        Dim gatewayIP As String = ""
        Dim Passed As Boolean = False
        Dim BuildWinPath As String = ""
        Dim src As String = ""
        Dim dst As String = ""
        Dim IpfromBox As String = ""

        Try
            Try
                WaitAfterRebootSec = CDbl(_UI.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
            Catch ex As Exception
                WaitAfterRebootSec = 50
            End Try

            _iex.Wait(WaitAfterRebootSec)

            'Try
            '    BuildWinPath = _UI.Utils.GetValueFromEnvironment("BuildWinPath")
            '    src = BuildWinPath + "\\..\\..\\IEX\\IEX_PROJECT_FILES\\IEX_INI_FILES\\diag.cfg"
            '    dst = BuildWinPath + "\\config\\diag.cfg"
            '    FileCopy(src, dst)
            '    _UI.Utils.LogCommentInfo("Copied Diag")
            'Catch ex As Exception
            '    _UI.Utils.LogCommentWarning("Copy diag.cfg command failed")
            'End Try


            Dim iniFile As AMS.Profile.Ini

            iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iex.IEXServerNumber.ToString + "\Telnet.ini")
            Prompt = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "prompt").ToString
            Username = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "username").ToString
            gatewayIP = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "gatewayIP").ToString

            _UI.Utils.StartHideFailures("Waiting For Prompt...")
            '_iex.Wait(180)


            For i As Integer = 0 To 10
                Res = _iex.Debug.BeginWaitForMessage(Prompt, 0, 600, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                End If

                Res = _iex.Debug.EndWaitForMessage(Prompt, ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Waiting For The Prompt")
                Else
                    Passed = True
                    Exit For
                End If
            Next
            _iex.Debug.WriteLine("ifconfig eth0 down", IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine("ifconfig eth0 " + gatewayIP + " netmask 255.255.255.0 broadcast 0.0.0.0", IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine("route add default gw 192.168.0.1", IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine("ifconfig eth0 up", IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine(Username, IEXGateway.DebugDevice.Serial)
            _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)

            If Not Passed Then

                Res = _iex.Debug.BeginWaitForMessage("CFE>", 0, 10, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                End If

                _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)

                Res = _iex.Debug.EndWaitForMessage("CFE>", ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res, "Box in CFE Prompt..!!"))
                Else
                    ExceptionUtils.ThrowEx(New IEXException(Res, "Failed to verify the " + Prompt + "and the CFE>"))
                End If



            End If


            Res = _iex.Debug.BeginWaitForMessage("inet addr:", 0, 10, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
            End If

            _iex.Debug.WriteLine("/sbin/ifconfig", IEXGateway.DebugDevice.Serial)

            Res = _iex.Debug.EndWaitForMessage("inet addr:", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res, "Failed to send ifconfig command"))

            Else

                IpfromBox = Split(Split(ActualLine, "inet addr:")(1), " ")(0)


            End If

            If StrComp(IpfromBox, gatewayIP) = 0 Then
                _UI.Utils.LogCommentBlack("DHCP Success IP address matched")
            ElseIf (IpfromBox = "") Then
                _UI.Utils.LogCommentBlack("DHCP Failed – IP not acquired")
                ExceptionUtils.ThrowEx(New IEXException(Res, "DHCP Failed – IP not acquired"))

            Else
                _UI.Utils.LogCommentBlack("IP address not matching")
                _UI.Utils.LogCommentInfo("IP Address from Box:" + IpfromBox + "--IP address from Telnet.ini:" + gatewayIP)
                ExceptionUtils.ThrowEx(New IEXException(Res, "IP address not matching"))

            End If



        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub



    ''' <summary>
    '''   Gets The Mount Command And Adding Values Needed To Mount
    ''' </summary>
    ''' <param name="IsFormat">If True Adds FORMAT Else Adds NOCLEAN</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetMountCommand(ByVal IsFormat As Boolean) As String
        Dim MountCommand As String = ""
        MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommand")
        If MountCommand.Contains("nohup") Then
            MountCommand = MountCommand + " " + IIf(IsFormat, "FORMAT", "NOCLEAN") + " &>Diag_TelnetLog.txt"
        Else
            If IsFormat Then
                MountCommand = MountCommand + " " + IIf(IsFormat, "FORMAT", "NOCLEAN")
            Else
                MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommandStart")
                MountCommand = MountCommand
            End If

        End If

        Return MountCommand
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
            'SEND MOUNT COMMAND
            'Sending the first mount command for format
            Dim Prompt As String = ""
            Dim ActualLine As String = ""
            Dim iniFile As AMS.Profile.Ini
            iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iex.IEXServerNumber.ToString + "\Telnet.ini")
            Prompt = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "prompt").ToString

            ' Res = _iex.Debug.BeginWaitForMessage(Prompt, 0, 600, IEXGateway.DebugDevice.Serial)
            ' If Not Res.CommandSucceeded Then
            '_UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
            'End If
            Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Serial)
            Res = _iex.Debug.Write(vbCrLf, IEXGateway.DebugDevice.Serial)
            Res = _iex.Wait(20)
            'Res = _iex.Debug.EndWaitForMessage(Prompt, ActualLine, "", IEXGateway.DebugDevice.Serial)
            'If Not Res.CommandSucceeded Then
            '    _UI.Utils.LogCommentFail("Waiting For The Prompt")
            'End If

            If IsFormat Then
                ' WaitForPrompt(IsNFS:=False)
                Res = _iex.SendPowerCycle("OFF")
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
                Res = _iex.Wait(10)
                Res = _iex.SendPowerCycle("ON")
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If

                'wait for prompt to send mount command after FORMAT
                WaitForPrompt(IsNFS:=False)
                MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommandStart")
                'Sending the Second mount command for boot up
                Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Serial)
            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New IEXException(Res))
        Finally
            _iex.ForceHideFailure()
        End Try
        Return True
    End Function

    Public Overrides Function InitializeStb(ByRef Msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean
        Dim EPGTime As String = ""

        'WAIT FOR MAIN MENU TO ARRIVE
        _UI.Utils.LogCommentInfo("Entering InitilizeSTB Function...")
		
		 Try
            _UI.Utils.TypeKeys("26")
		Catch
            _UI.Utils.SendIR("KEY_2", 500)
            _UI.Utils.SendIR("KEY_6", 500)

        End Try
		
        If Not _UI.Utils.VerifyState("LIVE", 700) Then
            _UI.Utils.LogCommentFail("Failed To Verify LIVE Is On Screen")
            Msg = "Failed To Verify LIVE Is On Screen"
            Return False
        End If

        Try
            _UI.Live.GetEpgTime(EPGTime)
            _UI.Utils.StreamSync(EPGTime)
        Catch
            _UI.Utils.LogCommentFail("NOT RUNNING STREAM SYNC")
        End Try



        _UI.Utils.LogCommentInfo("Waiting 300 s for Event names to load in ENGLISH Language")
        _iex.Wait(300)
        Return True

    End Function

    Public Overrides Function HandleFirstScreens(ByVal IsGW As Boolean) As Boolean

        Dim PIN As String = _UI.Utils.GetValueFromEnvironment("DefaultPIN")
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _UI.Utils.LogCommentInfo("Waiting For FTILANGUAGE state...")

        If Not _UI.Utils.VerifyState("FTILANGUAGE", 800) Then
            _UI.Utils.LogCommentFail("Failed To Verify FTILANGUAGE Is On Screen")
            Return False
        End If

        _UI.Utils.LogCommentInfo("Waiting 60s before sending keys .")
        _iex.Wait(60)

        Try

            _UI.Utils.SendIR("SELECT_DOWN", 8000)


            _UI.Utils.EPG_Milestones_SelectMenuItem("ENGLISH")

            _UI.Utils.SendIR("SELECT", 4000)
            _UI.Utils.SendIR("SELECT", 4000)
            _UI.Utils.SendIR("SELECT", 4000)
            _UI.Utils.SendIR("SELECT", 4000)
            _UI.Utils.SendIR("SELECT_DOWN", 2000)
            _UI.Utils.SendIR("SELECT", 4000)

        Catch ex As Exception
            Return False
        End Try

        _UI.Utils.LogCommentInfo("Waiting 20s before sending keys .")
        _iex.Wait(20)

        If PIN.Length <> 4 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "PIN Length Is Not 4 Please Check DefaultPIN In Your Environment.ini"))
        End If

        _UI.Utils.ClearEPGInfo() 'Before The Next VerifyState (Which Should Be The Same)

        _UI.Utils.TypeKeys(PIN)

        'If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL") Then
        '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify State Is INSERT PIN UNLOCK CHANNEL"))
        'End If

        'Insert PIN Another Time For Confirmation
        _UI.Utils.TypeKeys(PIN)


        _iex.Wait(10)



        If PIN.Length <> 4 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "PIN Length Is Not 4 Please Check DefaultPIN In Your Environment.ini"))
        End If

        _UI.Utils.ClearEPGInfo() 'Before The Next VerifyState (Which Should Be The Same)

        _UI.Utils.TypeKeys(PIN)

        'If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL") Then
        '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify State Is INSERT PIN UNLOCK CHANNEL"))
        'End If

        'Insert PIN Another Time For Confirmation
        _UI.Utils.TypeKeys(PIN)

        _UI.Utils.LogCommentInfo("Waiting 10s before taking action on PRESENTATION screen.")
        _iex.Wait(10)
        _UI.Utils.SendIR("SELECT_DOWN", 2000)
        _UI.Utils.SendIR("SELECT", 4000)
        _UI.Utils.LogCommentInfo("Waiting 10s before taking action on Presonal Recommendation screen.")
        _iex.Wait(10)
        _UI.Utils.SendIR("SELECT_DOWN", 2000)
        _UI.Utils.SendIR("SELECT", 4000)

        _iex.Wait(30)


        Return True

    End Function
    ''' <summary>
    '''   Checkes If Burn Needed And Burn
    ''' </summary>
    ''' <param name="CurrentVersion">Current Build Version</param>
    ''' <param name="IsGW">If True Burns Gw Else Burns Client</param>
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
                        ' BurnImage(IsGW)
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
                    '  BurnImage(IsGW)
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
End Class