Imports FailuresHandler
Imports System.Net.NetworkInformation

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
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


                'SEND MOUNT COMMAND
                'Res = _iex.Debug.Write(MountCommand & vbCrLf, IEXGateway.DebugDevice.Serial)
                Dim spltStrArr As String() = MountCommand.Split(";")
                Dim str_length As Integer = spltStrArr.Length
                Dim itr As Integer = 0
                For itr = 0 To str_length - 1
                    Res = _iex.Debug.Write(spltStrArr(itr) & vbCrLf, IEXGateway.DebugDevice.Serial)
                    _iex.Wait(1)
                Next
                If Not Res.CommandSucceeded Then
                    _UI.Utils.LogCommentFail("Failed To Send Command : " + MountCommand.ToString)
                    Return False
                Else
                    _UI.Utils.LogCommentInfo("Mount Command Sent Successfuly")
                End If

            Next
        Finally
            _iex.ForceHideFailure()
        End Try
        Return True
    End Function

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
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Overrides Function GetMountCommand(ByVal IsFormat As Boolean) As String
        If _UI.Utils.GetValueFromEnvironment("Project").ToString.ToLower.Contains("prd") Then
            Return True
        End If
        Dim MountCommand As String
        MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommand")
        MountCommand = MountCommand + IIf(IsFormat, " FORMAT", " ")
        Return MountCommand
    End Function
    
	 Public Overrides Function HandleFirstScreens(ByVal IsGW As Boolean) As Boolean

        Dim Res As IEXGateway.IEXResult = Nothing
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim first_option As String = ""
        Dim highlighted_option As String = ""
        Dim EPGTime As String = ""
        Dim firstscreenmilestones As String = ""
        _UI.Utils.LogCommentInfo("Waiting For Debug Text Initialize Milestone To Appear...")

        Milestones = _UI.Utils.GetValueFromMilestones("DebugTextInitialize")
        firstscreenmilestones = _UI.Utils.GetValueFromMilestones("FirstScreenMilestones")

        _UI.Utils.BeginWaitForDebugMessages(firstscreenmilestones, 700)
        If Not _UI.Utils.EndWaitForDebugMessages(firstscreenmilestones, ActualLines) Then
            _UI.Utils.LogCommentFail("Failed To Verify DebugTextInitialize Milestones")
            Return False
        End If
        _iex.Wait(5)
        _UI.Utils.SendIR("SELECT", 2000)
        _UI.Utils.BeginWaitForDebugMessages(Milestones, 120)

        If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
            _UI.Utils.LogCommentFail("Failed To Verify DebugTextInitialize Milestones")
            Return False
        End If

        _iex.Wait(10)

        _UI.Utils.SendIR("SELECT_DOWN", 1000)
        _UI.Utils.SendIR("SELECT", 2000)
        Try
            _UI.Utils.SendIR("SELECT_DOWN", 2000)
            _UI.Utils.SendIR("SELECT_UP", 2000)
            _UI.Utils.GetEpgInfo("title", first_option)
            If first_option <> "ENGLISH" Then
                While highlighted_option <> "ENGLISH" And highlighted_option <> first_option
                    _UI.Utils.SendIR("SELECT_DOWN", 2000)
                    _UI.Utils.GetEpgInfo("title", highlighted_option)
                End While
                If highlighted_option = first_option Then
                    highlighted_option = ""
                    While highlighted_option <> "FRAN?AIS" And highlighted_option <> first_option
                        _UI.Utils.SendIR("SELECT_DOWN", 2000)
                        _UI.Utils.GetEpgInfo("title", highlighted_option)
                    End While
                End If
            End If
        Catch ex As IEXException
            Dim IP As String
            Dim reply As PingReply
            IP = "10.100.33.74"
            reply = New Ping().Send(IP)
            If reply.Status = IPStatus.Success Then
                _iex.LogComment("Success Ping...!!!")
            Else
                _iex.LogComment("Failed Ping...!!!")
            End If
            _UI.Utils.LogCommentFail("Failed To set EPG Language")
        End Try
        _UI.Utils.SendIR("SELECT", 2000)
		'Added due to new screen of Satellite connection
        _UI.Utils.SendIR("SELECT_DOWN", 2000)
        _UI.Utils.SendIR("SELECT", 2000)
        _UI.Utils.SendIR("SELECT", 2000)
        _UI.Utils.SendIR("SELECT_DOWN", 1000)
        _UI.Utils.SendIR("SELECT", 4000)
        _UI.Utils.SendIR("SELECT_DOWN", 1000)
        _UI.Utils.SendIR("SELECT", 4000)
        _UI.Utils.SendIR("SELECT_DOWN", 1000)
        _UI.Utils.SendIR("SELECT", 4000)
        _UI.Utils.SendIR("SELECT_DOWN", 1000)
        _UI.Utils.SendIR("SELECT", 4000)
        _UI.Utils.SendIR("SELECT_DOWN", 1000)
        _UI.Utils.SendIR("SELECT", 4000)
     

        If Not _UI.Utils.VerifyState("MAIN MENU", 150, 5) Then
           _UI.Utils.SendIR("RETOUR")
           If Not _UI.Utils.VerifyState("MAIN MENU", 10, 5) Then
               _UI.Utils.LogCommentFail("Failed To Verify MAIN MENU Is On Screen")
               Return False
           End If
       End If
        'Retour Key is Pressed to Launch Channel Bar to fetch CHNUM value
        _iex.Wait(10)
        _UI.Utils.SendIR("RETOUR")
        Try
            _UI.Live.GetEpgTime(EPGTime)
			_UI.Utils.LogCommentInfo("Calling StreamSync to ensure sufficient time left for test execution")
            _UI.Utils.StreamSync(EPGTime)
        Catch
            _UI.Utils.LogCommentFail("NOT RUNNING STREAM SYNC")
        End Try

        Return True

    End Function

    Public Overrides Function InitializeStb(ByRef Msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean

        'WAIT FOR MAIN MENU TO ARRIVE
        _UI.Utils.LogCommentInfo("Entering InitilizeSTB Function...")

        If Not _UI.Utils.VerifyState("LIVE", 700) Then
            _UI.Utils.LogCommentFail("Failed To Verify LIVE Is On Screen")
            Msg = "Failed To Verify LIVE Is On Screen"
            Return False
        End If

        Return True

    End Function

End Class