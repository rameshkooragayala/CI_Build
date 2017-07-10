Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Tune To Radio Channel
    ''' </summary>
    Public Class TuneToRadioChannel
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumber As String

        ''' <param name="ChannelNumber">The Channel Number</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>333 - VideoPresent</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>335 - AudioPresent</para> 
        ''' <para>336 - AudioNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal ChannelNumber As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            If _ChannelNumber = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be Empty"))
            End If

            EPG.Live.TuningToChannel(Me._ChannelNumber, "Radio")

            EPG.Live.VerifyTuneToRadioChannel()

            res = Me._manager.CheckForVideo(IsPresent:=False, CheckFullArea:=False, Timeout:=15)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            res = Me._manager.CheckForAudio(IsPresent:=True, Timeout:=15)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If
        End Sub

    End Class

End Namespace