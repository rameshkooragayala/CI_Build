Imports FailuresHandler
Imports System.IO
Imports System.Text

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.Mount

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
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
            If WithIPC OrElse _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
                If _UI.PowerManagement.IsPerleRpc Then
                    _UI.PowerManagement.PerleRpcRestart()
                Else
                    Res = _iex.Power.TurnOFF
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If

                    _iex.Wait(10)

                    Res = _iex.Power.TurnOn
                    If Not Res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New IEXException(Res))
                    End If
                End If
           
            Else
                Res = _iex.Debug.Write("reboot -f" & vbCrLf, IEXGateway.DebugDevice.Serial) 'Added -f per Tomer's request
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Waiting For GW To Load By Verifying GWInitialize Milestones
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function WaitForGWToLoad() As Boolean
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim Cmd As String = ""
        Dim Milestones As String = ""
        Dim ActualLine As String = ""
        Dim Passed As Boolean
        Dim Device As IEXGateway.DebugDevice

        Cmd = "cat /proc/callisto/cdi/tuner"

        Milestones = _UI.Utils.GetValueFromMilestones("GWInitialize")

        _UI.Utils.StartHideFailures("Waiting For " + Milestones + " To Arrive")

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Device = IEXGateway.DebugDevice.Telnet
        Else
            Device = IEXGateway.DebugDevice.Serial
        End If

        Try
            For i As Integer = 0 To 10
                Passed = True

                Res = _iex.Debug.BeginWaitForMessage(Milestones, 0, 3, Device)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed to BeginWaitForMessage " + Cmd + " " + Res.FailureReason)
                    Passed = False
                End If

                If Passed Then
                    Res = _iex.Debug.WriteLine(Cmd + vbCrLf, Device)
                    If Not Res.CommandSucceeded Then
                        _UI.Utils.LogCommentFail("Failed to BeginWaitForMessage State: SyncLocked" + Res.FailureReason)
                        Passed = False
                    End If
                End If

                Res = _iex.Debug.EndWaitForMessage(Milestones, ActualLine, 0, Device)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed to Verify " + Milestones + " " + Res.FailureReason)
                    Passed = False
                End If

                If Passed Then
                    _UI.Utils.LogCommentInfo("Gw Tuner Locked !!!")
                    Return True
                End If

                _iex.Wait(60)
            Next

            Return False

        Finally
            _iex.ForceHideFailure()
        End Try


    End Function

    ''' <summary>
    '''    Get Version From STB
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetVersionFromSTB() As String
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""
        Dim Device As IEXGateway.DebugDevice

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            _UI.Utils.StartHideFailures("Getting Version From STB")
            Device = IEXGateway.DebugDevice.Telnet
        Else
            _UI.Utils.StartHideFailures("Comparing EPG Versions And Checking If Burn Is Needed")
            Device = IEXGateway.DebugDevice.Serial
        End If

        Try

            Res = _iex.Debug.BeginWaitForMessage("NDS_SW_VERSION", 0, 10, Device)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                Return ""
            End If

            Res = _iex.Debug.Write("cat /NDS/config/version.cfg" + vbCrLf, Device)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("NDS_SW_VERSION", ActualLine, "", Device)
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

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Return True
        End If

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
    Overrides Function GetMountCommand(ByVal IsFormat As Boolean) As String

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Return True
        End If

        Dim MountCommand As String

        MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommand")
        MountCommand = MountCommand + IIf(IsFormat, "-mw-format.sh", "_mw.sh")

        Return MountCommand
    End Function

    ''' <summary>
    '''   Gets The Flash Command From Environment.ini And Adds GW/Client Support If Needed
    ''' </summary>
    ''' <param name="IsGw">If True It's GW</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetFlashCommand(ByVal IsGw As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As String
        Return _UI.Utils.GetValueFromEnvironment("FlashCommand")
    End Function

    ''' <summary>
    '''    Gets The EPG Version From BuildWinPath
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetCurrentEPGVersion(Optional ByVal IsLastDelivery As Boolean = False) As String

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Return True
        End If

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
    '''   Closes The Serial Or Telnet Logging
    ''' </summary>
    ''' <remarks></remarks>
    Overrides Sub CloseLogs()

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            _UI.Utils.StartHideFailures("Trying To Close Telnet Logging...")
            _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet)
            _iex.Debug.EndLogging(IEXGateway.DebugDevice.Telnet)
        Else
            _UI.Utils.StartHideFailures("Trying To Close Serial Logging...")
            _iex.Debug.EndLogging(IEXGateway.DebugDevice.Serial)
        End If

        _iex.ForceHideFailure()
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

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Res = _iex.Debug.BeginLogging(LogFileName.Replace(".txt", "_" + LoopNum.ToString + ".txt"), "", False, IEXGateway.DebugDevice.Telnet)
        Else
            Res = _iex.Debug.BeginLogging(LogFileName.Replace(".txt", "_" + LoopNum.ToString + ".txt"), "", False, IEXGateway.DebugDevice.Serial)
        End If

        If Not Res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(Res))
        End If
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

        _UI.Utils.StartHideFailures("Burnning Flash Image To STB ...")

        Try
            _UI.Utils.GetValueFromEnvironment("WarningAsError")

            Try
                UsePowerCycle = CBool(_UI.Utils.GetValueFromEnvironment("RebootThroughIPC"))
            Catch ex As Exception
                UsePowerCycle = True
            End Try

            FlashCommand = GetFlashCommand(IsGw)

            If UsePowerCycle Then
                Res = _iex.Power.Restart()
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            Else
                Dim cmd As String = "reboot"
                Res = _iex.Debug.Write(cmd & vbCrLf, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try


        _UI.Utils.StartHideFailures("Waiting For CFE initialized To Appear ...")

        Try
            Res = _iex.Debug.BeginWaitForMessage("CFE initialized", 0, 50, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Dim until As DateTime = Now.AddSeconds(5)

            Do While DateDiff(DateInterval.Second, Now, until) > 0
                Res = _iex.Debug.WriteLine(Chr(3), IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            Loop

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("CFE initialized", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

        _UI.Utils.StartHideFailures("Waiting For CFE> Prompt To Appear ...")

        Try

            Res = _iex.Debug.BeginWaitForMessage("CFE>", 0, 5, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("CFE>", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try


        _iex.Wait(40)

        _UI.Utils.StartHideFailures("Burnning Image To STB ...")

        Try
            Res = _iex.Debug.BeginWaitForMessage("command status = 0", 0, 180, IEXGateway.DebugDevice.Serial)
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

            Res = _iex.Debug.EndWaitForMessage("command status = 0", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

        'StartHideFailures("Rebooting And Pressing x In Order To Get The Prompt ...")

        If UsePowerCycle Then
            Res = _iex.Power.Restart()
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
        Else
            Dim cmd As String = "reboot" 'removed <reboot -f> according to Tomer
            Res = _iex.Debug.Write(cmd & vbCrLf, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
        End If

        _UI.Utils.StartHideFailures("Waiting To Get The Prompt")

        Try
            WaitForPrompt(IsNFS:=False)

            _UI.Utils.LogCommentImportant("Burned Flash Image Successfuly To STB")

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub CopyBinary()
        _UI.Utils.StartHideFailures("Copy image from build to specified path")
        Dim BuildPath As String
        Dim Path As String
        Try
            Path = _UI.Utils.GetValueFromEnvironment("TFTP_Path")
            Path = Path + _iex.IEXServerNumber.ToString + "\\"
            BuildPath = _UI.Utils.GetValueFromEnvironment("FlashBuildPath")
            Dim file As New FileInfo(BuildPath + "vmlinux")
            Try
                If file.Exists() Then
                    DeleteFiles(New FileInfo(Path + "vmlinux"))
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
            Msg = "Failed To Verify Live On Screen"
            Return False
        End Try

        Return True

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
            Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Serial)
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
    '''    Waiting For Pormpt
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub WaitForPrompt(ByVal IsNFS As Boolean)
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim ActualLine As String = ""

        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Dim WaitAfterRebootSec As Double

            Try
                WaitAfterRebootSec = CDbl(_UI.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
            Catch ex As Exception
                WaitAfterRebootSec = 50
            End Try

            _iex.Wait(WaitAfterRebootSec)

            Exit Sub
        End If

        _UI.Utils.StartHideFailures("Waiting To Get The Prompt By 'Stopped because of boot variable'")

        Try
            Res = _iex.Debug.BeginWaitForMessage("Stopped because of boot variable", 0, 300, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("Stopped because of boot variable", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Verify 'Stopped because of boot variable Before Prompt'")
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Waiting For Messages After STB Reset
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function WaitAfterReset() As Boolean
        Dim ActualLines As String = ""
        Dim res As IEXGateway._IEXResult

        _UI.Utils.LogCommentInfo("Waiting For IEX_Current_Channel To Arrive...")

        _iex.Debug.BeginWaitForMessage("IEX_Current_Channel", 0, 240, IEXGateway.DebugDevice.Udp)

        res = _iex.Debug.EndWaitForMessage("IEX_Current_Channel", ActualLines, 0, IEXGateway.DebugDevice.Udp)

        If res.CommandSucceeded = False Then
            _UI.Utils.LogCommentFail("Failed To Verify IEX_Current_Channel Arrived")
            Return False
        End If

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
End Class
