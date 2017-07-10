Imports System.IO
Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Mount The STB Through Telnet Blindly Without Verification of the IEX For Release Build
    ''' </summary>
    Public Class MountTelnetStbBlind
        Inherits IEX.ElementaryActions.BaseCommand

        Private reason As String
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim IEX As IEXGateway.IEX
        Dim MountCommand, STB_IP As String
        Dim baudrate As String
        Dim data As String = ""
        Dim WaitAfterRebootSec As Double = 50
        Dim cmd As String

        ''' <param name="pManager">Manager</param>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim Res As IEXGateway.IEXResult = Nothing
            Dim DebugLogFilFullName As String
            Dim Passed As Boolean = True

            EPG.Utils.LogCommentInfo("Mounting STB Blindly...")

            DebugLogFilFullName = LCase(_iex.LogFileName).Replace(".iexlog", "_Serial.txt")

            MountCommand = EPG.Utils.GetValueFromEnvironment("MountCommand") + " FORMAT &"

            _iex.Debug.BeginLogging(DebugLogFilFullName, "", True)

            Res = _iex.Power.Restart()
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Try
                WaitAfterRebootSec = CDbl(EPG.Utils.GetValueFromEnvironment("WaitAfterRebootSec"))
            Catch ex As Exception
                WaitAfterRebootSec = 50
            End Try

            'waiting to the box to wake up fom the reboot:
            Res = _iex.Wait(WaitAfterRebootSec)

            If baudrate = "" Then
                baudrate = "115200"
            End If

            _iex.Debug.EndLogging()

            If Not UpdateStbIP(DebugLogFilFullName, _iex) Then
                SetWarning("**WARNING** Failed To Update IP On Telnet.ini")
            End If

            EPG.Utils.LogCommentInfo("Starting Telnet login ...")

            Me._manager.TelnetLogIn()
            cmd = MountCommand
            Me._manager.SendCmd(cmd)
            Me._manager.TelnetDisconnect()

            EPG.Utils.LogCommentInfo("Telnet - PASSED")

            If Passed Then
                EPG.Utils.LogCommentInfo("Waiting 180 Seconds For Language Selection To Appear")

                _iex.Wait(180)

                EPG.Utils.LogCommentInfo("Selecting English On Language Menu")

                _iex.GetSnapshot("Taking Screenshot For Debuging Before Selecting Language")

                _iex.IR.SendIR("SELECT_UP", "", 2000)

                _iex.IR.SendIR("SELECT", "", 2000)

                EPG.Utils.LogCommentInfo("Waiting 180 Seconds For CHANNELS")

                _iex.Wait(180)

                Res = _manager.CheckForVideo(True, False, 120)
                If Not Res.CommandSucceeded Then
                    EPG.Utils.LogCommentFail("Failed To Verify Video Present On Stb : " + Res.FailureReason)
                    Passed = False
                End If

                EPG.Utils.LogCommentInfo("Waiting 180 Seconds For Mem Files To Be Created")

                _iex.Wait(180)
            End If

            If Passed = False Then
                _iex.GetSnapshot("FAILED TRY TAKING SNAPSHOT FOR DEBUGING")
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountFailure, "Failed To Mount STB"))
            End If
        End Sub

        Private Function UpdateStbIP(ByVal FilePath As String, ByVal _iex As IEXGateway.IEX) As Boolean
            Try
                Dim _iexNumber As String = _iex.IEXServerNumber
                Dim IP As String = ""
                Dim FileAsString As String = File.ReadAllText(FilePath.Replace(".txt", "_0.txt"))
                Try
                    FileAsString = FileAsString.Remove(0, FileAsString.IndexOf("addr="))
                    IP = FileAsString.Substring(FileAsString.IndexOf("=") + 1, FileAsString.IndexOf(",") - (FileAsString.IndexOf("=") + 1))
                Catch ex As Exception
                    EPG.Utils.LogCommentFail("UpdateStbIP Error : IP Not Found Problem With Serial Or STB Network")
                End Try

                If IP <> "" Then
                    Dim iniFile As New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Telnet.ini")
                    iniFile.SetValue("TELNET-iex" & _iexNumber, "unixServer", IP)
                    EPG.Utils.LogCommentImportant("Found STB IP : " + IP.ToString)
                Else
                    EPG.Utils.LogCommentFail("UpdateStbIP : Can't Find The IP From STB Serial Not Updating")
                End If

            Catch ex As Exception
                EPG.Utils.LogCommentFail("UpdateStbIP Exception : " + ex.Message)
                Return False
            End Try

            Return True
        End Function
    End Class

End Namespace