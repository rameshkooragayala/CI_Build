Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Tunning To A Locked Channel
    ''' </summary>
    Public Class TuneToLockedChannel
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumber As String
        Private _CheckForVideo As Boolean

        ''' <param name="ChannelNumber">Channel Number To Tune To</param>
        ''' <param name="CheckForVideo">Optional Parameter Default = True,If True Then Checks For Video Else Not Checking For Video</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal ChannelNumber As String, ByVal CheckForVideo As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._CheckForVideo = CheckForVideo
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            EPG.Utils.ReturnToLiveViewing()
            EPG.Live.TuningToChannel(Me._ChannelNumber)
            EPG.Live.VerifyChannelNumber(Me._ChannelNumber)

            res = Me._manager.CheckForVideo(IsPresent:=False, CheckFullArea:=False, Timeout:=15)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            EPG.Banner.SelectItem()

            EPG.Utils.LogCommentInfo("Waiting 4 Sec In Order For PIN Screen Animation To End")
            _iex.Wait(4)

            res = Me._manager.EnterDeafultPIN("LIVE")
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            If Me._CheckForVideo Then
                res = Me._manager.CheckForVideo(IsPresent:=True, CheckFullArea:=False, Timeout:=15)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If

            EPG.Utils.VerifyLiveReached()
        End Sub

    End Class

End Namespace