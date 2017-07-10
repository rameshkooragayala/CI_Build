Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Utils

    Dim _UI As IEX.ElementaryActions.EPG.SF.CISCOREFRESH.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

    ''' <summary>
    '''   Sends IR Key
    ''' </summary>
    ''' <param name="IRKey">The Key To Send</param>
    ''' <param name="WaitAfterIR">Optional Parameter Default 2000 : Wait After Sending</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SendIR(ByVal IRKey As String, Optional ByVal WaitAfterIR As Integer = 2000)
        Dim res As New IEXGateway.IEXResult
        Dim ActualLines As New ArrayList

        StartHideFailures("Sending IR : " + IRKey + " And Waiting After : " + WaitAfterIR.ToString)

        Try
            WaitAfterIR = WaitAfterIR * WaitAfterIRFactor

            For i As Integer = 0 To 2

                BeginWaitForDebugMessages("IEX IR key", 5)

                res = _iex.IR.SendIR(IRKey)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If

                Try
                    If EndWaitForDebugMessages("IEX IR key", ActualLines) Then
                        _iex.Wait(WaitAfterIR / 1000)
                        Exit Sub
                    End If

                Catch ex As EAException
                    If i = 2 Then 'If Reached Last Iteration
                        ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, "Failed To Verify Send IR " + IRKey.ToString + " : " + ex.ShortMessage))
                    End If
                Catch ex As IEXException
                    If i = 2 Then 'If Reached Last Iteration
                        ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                    End If
                End Try
            Next

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class


