Imports FailuresHandler

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
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
        Dim Username As String = ""
        Dim Passed As Boolean = False

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
            Username = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "username").ToString

            _UI.Utils.StartHideFailures("Waiting For Prompt...")

            For i As Integer = 0 To 10
                Res = _iex.Debug.BeginWaitForMessage(Prompt, 0, 10, IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                End If

                _iex.Debug.WriteLine(Username, IEXGateway.DebugDevice.Serial)
                _iex.Debug.WriteLine(vbCrLf, IEXGateway.DebugDevice.Serial)

                Res = _iex.Debug.EndWaitForMessage(Prompt, ActualLine, "", IEXGateway.DebugDevice.Serial)
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Waiting For The Prompt")
                Else
                    Passed = True
                    Exit For
                End If
            Next

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
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Function InitializeStb(ByRef Msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean

        Dim Res As IEXGateway.IEXResult = Nothing

        _UI.Utils.StartHideFailures("Trying To Verify Gw Is Loaded By Waiting For MainMenu EPG")
        Try
            Try
                If _UI.Utils.VerifyState("MAIN MENU", 700) Then
                    Try
                        _UI.Utils.LogCommentInfo("Waiting 30 Sec For STB To Become Ready")
                        _iex.Wait(30)
                        _UI.Utils.LogCommentWarning("WORKAROUND : Event Information is not available on default service")
                        _UI.Utils.ReturnToLiveViewing()
                        _iex.Wait(3)
                        _UI.ChannelBar.SurfChannelDown("Not Predicted")
                        _UI.ChannelBar.VerifySurfChannelNotPredicted()
                    Catch ex As Exception
                        Return False
                    End Try
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try

            Return False
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Gets The Mount Command And Adding Values Needed To Mount
    ''' </summary>
    ''' <param name="IsFormat">If True Adds FORMAT Else Adds NOCLEAN</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetMountCommand(ByVal IsFormat As Boolean) As String
        Dim MountCommand As String = ""

        MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommand")

        MountCommand = MountCommand + " " + IIf(IsFormat, "FORMAT", "NOCLEAN")


        Return MountCommand
    End Function

    ''' <summary>
    '''   Waiting For First Screen To Appear On EPG
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function WaitForFirstScreen() As Boolean
        Return True
    End Function

    Public Overrides Sub PressSelect()
        Exit Sub
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
End Class