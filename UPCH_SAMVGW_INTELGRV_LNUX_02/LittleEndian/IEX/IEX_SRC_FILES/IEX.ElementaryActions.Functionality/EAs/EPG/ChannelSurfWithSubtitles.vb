Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Surfs Channel Up Down Or DCA On Live With Subtitles
    ''' </summary>
    Public Class ChannelSurfWithSubtitles
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumber As String
        Private _IsNext As Boolean
        Private _NumberOfPresses As Integer
        Private _IsPredicted As EnumPredicted

        ''' <param name="ChannelNumber">Optional Parameter Default = "" : Channel Number</param>
        ''' <param name="IsNext">Optional Parameter Default = True : If True Surfs To Next Channel Else To Previous</param>
        ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Channels To Surf Up OR Down</param>
        ''' <param name="IsPredicted">Optional Parameter Default = True : If The Next Or Previous Channel Is Predicted</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal ChannelNumber As String, ByVal IsNext As Boolean, ByVal NumberOfPresses As Integer, ByVal IsPredicted As EnumPredicted, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._IsNext = IsNext
            Me._NumberOfPresses = NumberOfPresses
            Me._IsPredicted = IsPredicted
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            EPG.Utils.ReturnToLiveViewing()

            If Me._ChannelNumber <> "" Then

                Try
                    EPG.Live.VerifyChannelNumber(Me._ChannelNumber)
                    Exit Sub
                Catch ex As IEXException
                    If ex.ExitCodeValue <> 112 Then
                        _iex.ForceHideFailure()
                        ExceptionUtils.ThrowEx(New IEXException(ex.ExitCodeValue, ex.ExitCodeName, "Failed To Check Channel Number In EpgInfo : " + ex.Message))
                    End If
                End Try

                Select Case _IsPredicted
                    Case EnumPredicted.Predicted

                        EPG.Live.TuningToChannel(Me._ChannelNumber, "Predicted", WithSubtitles:=True)
                        EPG.Live.VerifyTuneToChannelPredicted()
                        EPG.Live.VerifyTuneToChannelSubtitles()

                    Case EnumPredicted.NotPredicted

                        EPG.Live.TuningToChannel(Me._ChannelNumber, "Not Predicted", WithSubtitles:=True)
                        EPG.Live.VerifyTuneToChannelNotPredicted()
                        EPG.Live.VerifyTuneToChannelSubtitles()

                    Case EnumPredicted.Ignore
                        EPG.Live.TuningToChannel(Me._ChannelNumber, WithSubtitles:=True)
                        EPG.Live.VerifyChannelNumber(Me._ChannelNumber)
                        EPG.Live.VerifyTuneToChannelSubtitles()

                End Select
            Else
                For i = 1 To Me._NumberOfPresses
                    Select Case _IsPredicted
                        Case EnumPredicted.Predicted
                            If Me._IsNext Then
                                EPG.ChannelBar.SurfChannelUp("Predicted", WithSubtitles:=True)
                            Else
                                EPG.ChannelBar.SurfChannelDown("Predicted", WithSubtitles:=True)
                            End If

                            EPG.ChannelBar.VerifySurfChannelPredicted()

                        Case EnumPredicted.NotPredicted
                            If Me._IsNext Then
                                EPG.ChannelBar.SurfChannelUp("Not Predicted", WithSubtitles:=True)
                            Else
                                EPG.ChannelBar.SurfChannelDown("Not Predicted", WithSubtitles:=True)
                            End If

                            EPG.ChannelBar.VerifySurfChannelNotPredicted()

                        Case EnumPredicted.Ignore
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Can't Choose Ignore On Live Channel Surf Without DCA"))
                    End Select
                Next
            End If

            EPG.Utils.VerifyLiveReached()

        End Sub

    End Class

End Namespace