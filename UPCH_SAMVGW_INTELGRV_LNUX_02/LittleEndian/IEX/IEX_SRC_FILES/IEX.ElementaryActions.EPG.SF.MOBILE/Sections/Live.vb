Imports FailuresHandler

Public Class Live
    Inherits IEX.ElementaryActions.EPG.SF.Live

    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Checks For Live OSD When Returning From Trick Mode
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsLive() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Live Is On Screen")

        Try
            If _UI.Utils.VerifyState("CanalDPlayerController", 10) Then
                Msg = "Live Is On Screen"
                Return True
            Else
                Msg = "Live Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    ''' <summary>
    '''   Verifies Channel Number
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel Number To Verify</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyChannelNumber(ByVal ChannelNumber As String)
        Dim CurrentChannel As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Verifying Channel Number Is : " + ChannelNumber)

        Try
            _UI.Utils.GetEpgInfo("chnum", CurrentChannel)

            If CurrentChannel = ChannelNumber Then
                Msg = "Verified Channel Is : " + ChannelNumber
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is : " + ChannelNumber))

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub


    ''' <summary>
    '''   Tunning To Channel
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel To Tune To</param>
    ''' <param name="Type">Can Be : "" For Regular Channel Surf,"Predicted" For Channel Surf With Predicted,"Not Predicted" For Channel Surf With Not Predicted</param>
    ''' <param name="WithSubtitles">If True Tune To Channel With Subtitles</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub TuningToChannel(ByVal ChannelNumber As String, Optional ByVal Type As String = "", Optional ByVal WithSubtitles As Boolean = False)

        _UI.Utils.StartHideFailures("Tuning To Channel : " & ChannelNumber)

        Try
           
            _UI.ChannelBar.Navigate()

            _UI.ChannelBar.SurfToChannel(ChannelNumber)

            _UI.Utils.Tap("NowEvent", "ChannelBar.Events", VerifyAnimation:=True, WaitAfterTap:=1000)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class
