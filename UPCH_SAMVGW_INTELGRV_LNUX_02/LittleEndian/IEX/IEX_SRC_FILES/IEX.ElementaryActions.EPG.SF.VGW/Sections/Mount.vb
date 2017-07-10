Imports FailuresHandler

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.VGW.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub



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

        If IsFormat Then
            MountCommand = MountCommand + " FORMAT"
        End If

        Return MountCommand
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
                _UI.Utils.LogCommentWarning("'waitAfterRebootSec' is empty in Environment.ini. setting it to 50 secs by default")
                WaitAfterRebootSec = 50
            End Try

            _iex.Wait(WaitAfterRebootSec)

            Exit Sub
        End If

        _UI.Utils.StartHideFailures("Waiting To Get The Prompt By 'grep: /proc/callisto/version: No such file or directory'")

        Try
            Res = _iex.Debug.BeginWaitForMessage("grep: /proc/callisto/version: No such file or directory", 0, 300, IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Res = _iex.Debug.EndWaitForMessage("grep: /proc/callisto/version: No such file or directory", ActualLine, "", IEXGateway.DebugDevice.Serial)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed To Verify 'grep: /proc/callisto/version: No such file or directory'")
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Function InitializeStb(ByRef Msg As String, Optional ByVal IsReturnToLive As Boolean = True) As Boolean
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim Milestones As String = ""
        Dim ActualLine As String = ""

        Milestones = "IEX_Current_Channel"

        _UI.Utils.StartHideFailures("Waiting For " + Milestones + " To Arrive")

        Try

            Res = _iex.Debug.BeginWaitForMessage(Milestones, 0, 500, IEXGateway.DebugDevice.Udp)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed to BeginWaitForMessage " + Res.FailureReason)
            End If

            Res = _iex.Debug.EndWaitForMessage(Milestones, ActualLine, 0, IEXGateway.DebugDevice.Udp)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentFail("Failed to Verify " + Milestones + " " + Res.FailureReason)
                Return False
            End If

            If Not _UI.Utils.VerifyState("MAIN MENU", 50) Then
                _UI.Utils.LogCommentInfo("Failed to verify if the STB reached live")
                Return False
            End If
            Return True

        Finally
            _iex.ForceHideFailure()
        End Try
    End Function

    ''' <summary>
    ''' Tune to a default channel after Mount. If not handled to tune from this method, it should return false else true.
    ''' </summary>
    ''' <param name="ChNumber">Channel Number of the service where it needs to tune to</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>Handled to tune to default channel in this method as it cant be handle in Functionality.MountGw.vb</remarks>
    Public Overrides Function TuneToDefaultChannel(ByVal ChNumber As String) As Boolean
        Dim Res As IEXGateway.IEXResult = Nothing

        _UI.Utils.StartHideFailures("Tunning To Default Channel : " + ChNumber)
        Try
            _UI.Utils.SendChannelAsIRSequence(ChNumber)
            'Handled to send channel number again if there is any key miss
            If Not _UI.Utils.VerifyDebugMessage("dca_number", ChNumber, 7, 5) Then
                _UI.Utils.SendChannelAsIRSequence(ChNumber)
                If Not _UI.Utils.VerifyDebugMessage("dca_number", ChNumber, 7, 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify dca_number Key With Value " + ChNumber))
                End If
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Delay after Mount due to MHSI loading delay. This part of code to be removed once the issue is fixed. CQ-ULive01911941
            _UI.Utils.LogCommentWarning("WORKAROUND: Waiting for 90 sec due to MHSI loading delay")
            Res = _iex.Wait(90)
            If Not Res.CommandSucceeded Then
                _UI.Utils.LogCommentInfo("Failed to wait for 90 secs")
                Return False
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            _UI.Utils.LogCommentInfo("STB reached live")
            Return True

        Finally
            _iex.ForceHideFailure()
        End Try


    End Function
End Class