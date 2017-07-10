Imports FailuresHandler

Public Class Live
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Live

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    Public Overrides Sub WaitAfterLiveReached()
        _UI.Utils.LogCommentInfo("Waiting 10 Seconds After Live Reached")
        _iex.Wait(10)
    End Sub

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

			_UI.Utils.LogCommentWarning("Workaround:Verifying LIVE state as Blank screen is seen after tuning")
            _UI.Utils.VerifyState("LIVE", 25)

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
        Dim CurrentChannel As String = ""

        _UI.Utils.StartHideFailures("Tuning To Channel : " & ChannelNumber)

        Try
            If WithSubtitles Then
                SetSubtitlesVerification()
            End If


            _UI.Utils.LogCommentInfo("Waiting before doing DCA")
            _iex.Wait(5)

            If Type = "Radio" Then
                _UI.Utils.EPG_Milestones_NavigateByName("STATE:RADIO")

            End If
            SetTuneToChannelVerification(Type)

            _UI.Utils.SendChannelAsIRSequence(ChannelNumber, , Type)


            If Type = "Radio" Then


                _UI.Utils.LogCommentInfo("Waiting before Selecting Radio channel")
                _iex.Wait(5)
                _UI.Utils.SendIR("SELECT")


                'If Not _UI.Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 5) Then
                '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify dca_number Key With Value " + ChannelNumber))
                'End If
                _UI.Utils.LogCommentWarning("Workaround for DCA radio channel as DCA log does not come")

            Else

                If Not _UI.Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify dca_number Key With Value " + ChannelNumber))
                End If

            End If


        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
	
	''' <summary>
    '''    Verifies Tune To Radio Channel
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyTuneToRadioChannel()
        Dim ActualLines As New ArrayList
        Dim Radio_Milestones As String = ""

        Radio_Milestones = _UI.Utils.GetValueFromMilestones("TuneToRadio")

        If Not _UI.Utils.EndWaitForDebugMessages(Radio_Milestones, ActualLines) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify TuneToRadio Milestones : " + Radio_Milestones))
        End If

        If Not _UI.Utils.VerifyState("RADIO LINEUP", 25) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is LIVE"))
        End If
    End Sub
	

End Class
