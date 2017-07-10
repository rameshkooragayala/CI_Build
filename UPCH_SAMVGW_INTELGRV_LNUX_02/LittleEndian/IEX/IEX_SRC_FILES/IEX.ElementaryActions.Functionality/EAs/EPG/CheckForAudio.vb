Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Checks If Audio Is Present Or Not Present
    ''' </summary>
    Public Class CheckForAudio
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _IsPresent As Boolean
        Private _Timeout As Integer


        ''' <param name="IsPresent">If True Audio Present Else No Audio Present</param>
        ''' <param name="Timeout">Timeout To Check For Audio Presence</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>335 - AudioPresent</para> 
        ''' <para>336 - AudioNotPresent</para> 
        ''' </remarks>
        Sub New(ByVal IsPresent As Boolean, ByVal Timeout As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
            _IsPresent = IsPresent
            _Timeout = Timeout
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult
			Dim AudioDecoderStatPath As String
			Dim AudioDecoderValidationParam As String() = {"Started", "Buffer"}
			Dim AudioDecoderInfo As String() = {"", ""}
			Dim BufferValue As Integer() = {0, 0, 0, 0, 0}
            Dim AudioCheckCommand As String = ""
            Dim audioCheckPass As Boolean = False
            Dim timeToWait As Integer = 0

            Try
                res = _iex.CheckForAudio(_Timeout, _IsPresent)
				If (Not res.CommandSucceeded) And (res.CommandExecutionFailed = False) Then
						If _IsPresent Then
							ExceptionUtils.ThrowEx(New EAException(ExitCodes.AudioNotPresent, "Audio Was Not Present"))
						Else
							ExceptionUtils.ThrowEx(New EAException(ExitCodes.AudioPresent, "Audio Was Present"))
						End If
				End If

            Catch ex As Exception

                EPG.Utils.LogCommentWarning("Validating Audio Decoder Status from procfs entries")

                AudioDecoderStatPath = EPG.Utils.GetValueFromEnvironment("AudioDecoderStatPath")

                EPG.Utils.LogCommentInfo("Audio Decoder status path: " + AudioDecoderStatPath)
                AudioCheckCommand = "cat " + AudioDecoderStatPath

                Me._manager.TelnetLogIn(False, False)
                Try
                    ' Take maximum 5 samples of buffer values to check that audio is changing 
                    For i As Integer = 0 To 4
                    If (_IsPresent) Then
                        Try
                            Me._manager.SendCmd(AudioCheckCommand, False, AudioDecoderValidationParam, AudioDecoderInfo)
                        Catch ex1 As Exception
                            Me._manager.SendCmd(AudioCheckCommand, False, AudioDecoderValidationParam, AudioDecoderInfo)
                        End Try
                    Else
                        Try
                            Me._manager.SendCmd(AudioCheckCommand, False, AudioDecoderValidationParam, AudioDecoderInfo)
                        Catch ex1 As Exception
                            EPG.Utils.LogCommentInfo("Audio Decoder Buffer values are not different! Audio is not present")
                            Exit Sub
                        End Try
                    End If

                        'Sample buffer value - Buffer 2842:262144
                        Dim bufferValueStr = Split(AudioDecoderInfo(1), AudioDecoderValidationParam(1))(1)
                        BufferValue(i) = Convert.ToInt32(Trim(Split(bufferValueStr, ":")(0)))

                        ' Compare buffer value with previous to check if its changing
                        If i > 0 Then
                            If BufferValue(i) <> BufferValue(i - 1) Then
                                audioCheckPass = True
                                Exit For
                            End If
                        End If

                        timeToWait = _Timeout / 5
                        If (timeToWait > 0) Then
                            _iex.Wait(timeToWait)
                        Else
                            _iex.Wait(1)
                        End If
                    Next

                    If Not _IsPresent And Not audioCheckPass Then
                        EPG.Utils.LogCommentInfo("Audio is not present")
                    ElseIf Not _IsPresent And audioCheckPass Then
                        ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.AudioPresent, "Audio Decoder Buffer values are different! "))
                    ElseIf Not audioCheckPass Then
                        ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.AudioNotPresent, "Audio Decoder Buffer values are not different! "))
                    End If

                Catch ex2 As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, ex2.Message + " ;" + "Failed to Verify the Audio Check in Audio Decoder Buffer"))
                Finally
                    Me._manager.TelnetDisconnect(False)
                End Try
            End Try

            EPG.Utils.LogCommentInfo("Check for Audio Passed")
        End Sub

    End Class

End Namespace
