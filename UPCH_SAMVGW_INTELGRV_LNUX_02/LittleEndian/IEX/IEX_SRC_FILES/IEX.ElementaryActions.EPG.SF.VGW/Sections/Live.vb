Imports FailuresHandler

Public Class Live
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Live

    Dim _UI As IEX.ElementaryActions.EPG.SF.VGW.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    Public Overrides Sub VerifyChannelNumber(ByVal ChannelNumber As String)
        Dim CurrentChannel As String = ""
        Dim Msg As String = ""
        'Polling for 2 sec as EPG has to display channel number within 2 sec as per Requirement.
        Dim Timeout As Date = Now.AddSeconds(2)

        _UI.Utils.StartHideFailures("Verifying Channel Number Is : " + ChannelNumber)

        Try
            Do
                _UI.Utils.GetEpgInfo("chnum", CurrentChannel)

                If CurrentChannel = ChannelNumber Then
                    Msg = "Verified Channel Is : " + ChannelNumber
                    Exit Sub
                End If

                Threading.Thread.Sleep(400)
            Loop Until DateDiff(DateInterval.Second, Now, Timeout) < 0

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is : " + ChannelNumber))
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub
End Class
