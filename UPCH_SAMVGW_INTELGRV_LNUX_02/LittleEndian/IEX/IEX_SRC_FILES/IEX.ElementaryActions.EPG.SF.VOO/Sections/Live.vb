Imports FailuresHandler
Public Class Live
    Inherits IEX.ElementaryActions.EPG.SF.Live

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Verifying the State in Radio Service
    ''' </summary>
    ''' <remarks> 
    ''' In VOO the Active State on Radio Service is Zap Channel Bar
    ''' </remarks>

    Public Overrides Sub VerifyRadioStateReached()

        If Not _UI.Utils.VerifyState("ZAP CHANNEL BAR", 10) Then

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed to Verify required state ->Zap Channel Bar on Radio Service"))
        End If


    End Sub
End Class
