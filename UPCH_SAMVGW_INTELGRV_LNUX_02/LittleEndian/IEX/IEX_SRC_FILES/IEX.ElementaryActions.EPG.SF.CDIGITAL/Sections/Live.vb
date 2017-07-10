Imports FailuresHandler

Public Class Live
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Live

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    Public Overrides Sub WaitAfterLiveReached()
        _UI.Utils.LogCommentInfo("Waiting 10 Seconds After Live Reached")
        _iex.Wait(10)
    End Sub
	
	''' <summary>
    '''   Retry Mechanism to send DCA keys if keys are missed
	'''   TuningToChannel function is called to resend IR Keys
    ''' </summary>  

    Public Overrides Sub TuningToChannel(ByVal ChannelNumber As String, Optional ByVal Type As String = "", Optional ByVal WithSubtitles As Boolean = False)
        Dim CurrentChannel As String = ""

        _UI.Utils.StartHideFailures("Tuning To Channel : " & ChannelNumber)

        Try
            If WithSubtitles Then
                SetSubtitlesVerification()
            End If

            SetTuneToChannelVerification(Type)

            _UI.Utils.SendChannelAsIRSequence(ChannelNumber)

            If Not _UI.Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 5) Then
                _UI.Utils.SendChannelAsIRSequence(ChannelNumber)
                If Not _UI.Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify dca_number Key With Value " + ChannelNumber))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class
