Imports FailuresHandler

Public Class Live
    Inherits IEX.ElementaryActions.EPG.Live

    Dim _UI As UI
    Private Shadows _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Checks For Live OSD When Returning From Trick Mode
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsLive() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Live Is On Screen")

        Try
            If _Utils.VerifyState("LIVE", 45) Then
                Msg = "Live Is On Screen"
                Return True
            Else
                Msg = "Live Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
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

        _Utils.StartHideFailures("Verifying Channel Number Is : " + ChannelNumber)

        Try
            _Utils.GetEpgInfo("chnum", CurrentChannel)

            If CurrentChannel = ChannelNumber Then
                Msg = "Verified Channel Is : " + ChannelNumber
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is : " + ChannelNumber))

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Getting EPG Date From Screen
    ''' </summary>
    ''' <param name="EPG_Date">Returned EPG Date</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>310 - GetEpgDateFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEpgDate(ByRef EPG_Date As String)
        Dim EpgDate As String = ""
        Dim TDate As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting EPG Date")

        Try

            _UI.Utils.GetEpgInfo("date", EpgDate)
            _UI.Utils.ParseEPGDate(EpgDate, TDate)

            EPG_Date = TDate

            If EPG_Date = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEPGDateFailure, "Failed To Get EPG Time"))
            End If

            Msg = "EPG Date Is " + EPG_Date

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try


    End Sub

    ''' <summary>
    '''   Getting EPG Time From Screen
    ''' </summary>
    ''' <param name="EPG_Time">Returned EPG Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>309 - GetEpgTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEpgTime(ByRef EPG_Time As String)
        Dim EpgDate As String = ""
        Dim Ttime As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting EPG Time")

        Try

            _UI.Utils.GetEpgInfo("date", EpgDate)
            _UI.Utils.ParseEPGTime(EpgDate, Ttime)

            EPG_Time = Ttime

            If EPG_Time = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEPGTimeFailure, "Failed To Get EPG Time"))
            End If

            Msg = "EPG Time Is " + EPG_Time

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try


    End Sub

    ''' <summary>
    '''    Surfing Up On Live
    ''' </summary> 
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelUp(Optional ByVal Type As String = "", Optional ByVal WithSubtitles As Boolean = False)
        Dim CurrentChNum As String = ""
        Dim CheckedChNum As String = ""
        Dim chSurfUpKey As String = ""
        Dim chSurfTimeoutInSecInMSec As Integer

        _Utils.StartHideFailures("Surfing Up On Live")

        Try
            GetChannelNumber(CurrentChNum)

            If WithSubtitles Then
                _UI.Live.SetSubtitlesVerification()
            End If

            If Type <> "None" Then
                SetSurfChannelVerification(Type)
            End If

            'Fetch the channel surf up key
            chSurfUpKey = GetSurfKey()
            chSurfTimeoutInSecInMSec = GetChannelSurfTimeout()

            'Send the key
            _Utils.SendIR(chSurfUpKey, chSurfTimeoutInSecInMSec)

            GetChannelNumber(CheckedChNum)

            If CurrentChNum = CheckedChNum Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Channel Number Is The Same As Previous Failed To Surf Channel Up"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Surfing Down On Live
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelDown(Optional ByVal Type As String = "", Optional ByVal WithSubtitles As Boolean = False)
        Dim Messages As String = ""
        Dim CurrentChNum As String = ""
        Dim CheckedChNum As String = ""
        Dim chSurfDownKey As String = ""
        Dim chSurfTimeoutInSec As String = ""

        _Utils.StartHideFailures("Surfing Down On Live")

        Try
            GetChannelNumber(CurrentChNum)

            If WithSubtitles Then
                _UI.Live.SetSubtitlesVerification()
            End If

            If Type <> "None" Then
                SetSurfChannelVerification(Type)
            End If

            'Fetch the channel surf down key
            chSurfDownKey = GetSurfKey(False)
            chSurfTimeoutInSec = GetChannelSurfTimeout()

            'Send the key
            _Utils.SendIR(chSurfDownKey, chSurfTimeoutInSec)

            GetChannelNumber(CheckedChNum)

            If CurrentChNum = CheckedChNum Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Channel Number Is The Same As Previous Failed To Surf Channel Down"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Get the channel surf key    
    ''' </summary>
    ''' <param name="surfUp">Optional. The surf direction.</param>
    ''' <returns>The surf key from the project configuration or the default value from the code</returns>
    Overrides Function GetSurfKey(Optional ByVal surfUp As Boolean = True) As String
        Dim surfUpKeyDefault As String = "CH_PLUS"
        Dim surfDownKeyDefault As String = "CH_MINUS"
        Dim surfKey As String = ""
        Dim surfDirection As String = ""

        'Checking surf direction
        If surfUp Then
            surfDirection = "UP"
        Else
            surfDirection = "DOWN"
        End If

        Try
            'Get the key from Project ini
            surfKey = _Utils.GetValueFromProject("LIVE", "CHANNEL_SURF_" + surfDirection + "_KEY")
        Catch
            'Taking default value
            _Utils.LogCommentWarning("CHANNEL_SURF_" + surfDirection + "_KEY in section LIVE is missing in your project configuration file. Please add it if it deviates from the default value!")

            If surfUp Then
                surfKey = surfUpKeyDefault
            Else
                surfKey = surfDownKeyDefault
            End If

            _Utils.LogCommentWarning("Taking default value from code instead - " + surfKey)
        End Try

        Return surfKey
    End Function

    ''' <summary>
    ''' Get the channel surf timeout required
    ''' </summary>
    ''' <returns>Timeout in milliseconds as integer</returns>
    ''' <remarks></remarks>
    Overrides Function GetChannelSurfTimeout() As Integer
        Dim chSurfTimeout As String = "3"
        Try
            chSurfTimeout = _Utils.GetValueFromProject("LIVE", "CHANNEL_SURF_TIMEOUT_SEC")
        Catch
            _Utils.LogCommentWarning("CHANNEL_SURF_TIMEOUT_SEC in section LIVE is missing in your project configuration file. Please add it if it deviates from the default value!")
            _Utils.LogCommentWarning("Taking default value from code instead - " + chSurfTimeout)
        End Try
        Return CInt(chSurfTimeout) * 1000
    End Function

    ''' <summary>
    '''   Getting Channel Number From Live
    ''' </summary>
    ''' <param name="ChannelNumber">ByRef The Returned Channel Number</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetChannelNumber(ByRef ChannelNumber As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Channel Number From Live")

        Try
            Dim ReturnedValue As String = ""

            _Utils.GetEpgInfo("chNum", ChannelNumber)

            Msg = "Channel Number : " + ChannelNumber

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Sets Surf Channel Verifications
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Regular Channel Surf,"Predicted" For Channel Surf With Predicted,"Not Predicted" For Channel Surf With Not Predicted</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Protected Friend Sub SetSurfChannelVerification(ByVal Type As String)
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Setting Verification Of " + Type + " Milestones")

        Try
            Select Case Type
                Case "", "Ignore"
                    _Utils.LogCommentWarning("Skipping validation as type is Ignore!")

                Case "Predicted"
                    Dim LiveSurfPredicted_Milestones As String = ""
                    'Dim LiveSurfSlowZapping_Milestones As String = ""

                    LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
                    'LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")


                    _Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    '_Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

                Case "Not Predicted"
                    Dim LiveSurfNotPredicted_Milestones As String = ""

                    LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")

                    _Utils.BeginWaitForDebugMessages(LiveSurfNotPredicted_Milestones, 15)

                Case "Default"
                    Dim LiveSurfDefault As String = ""

                    LiveSurfDefault = _Utils.GetValueFromMilestones("LiveSurfDefault")

                    _Utils.BeginWaitForDebugMessages(LiveSurfDefault, 15)


                Case "WithoutPIP"
                    Dim LiveSurfWithoutPIP As String = ""
                    LiveSurfWithoutPIP = _Utils.GetValueFromMilestones("ChannelSurfWithoutPIP")

                    _Utils.BeginWaitForDebugMessages(LiveSurfWithoutPIP, 15)

                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Type " + Type + " Is Not Supported"))
            End Select

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifies Channel Surf Default Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Sub VerifySurfChannelDefault()
        Dim LiveSurfDefault As String = ""
        Dim ActualLines As New ArrayList
        _Utils.StartHideFailures("Verifying Surf Channel Default Milestones")
        Try
            LiveSurfDefault = _Utils.GetValueFromMilestones("LiveSurfDefault")
            If Not _Utils.EndWaitForDebugMessages(LiveSurfDefault, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Live LiveSurfDefault Milestones : " + LiveSurfDefault))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifies Channel Surf Ignore Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySurfChannelIgnore()
        Dim LiveSurfPredicted_Milestones As String = ""
        Dim LiveSurfNotPredicted_Milestones As String = ""
        Dim ActualLines As New ArrayList

        Dim PredictedPassed As Boolean = True
        Dim NotPredictedPassed As Boolean = True

        _Utils.StartHideFailures("Verifying Surf Channel Ignore Milestones")

        Try
            LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
            LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")

            _UI.Utils.StartHideFailures("Trying To Verify Predicted")
            Try
                If Not _Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                    PredictedPassed = False
                End If
            Finally
                _iex.ForceHideFailure()
            End Try

            _UI.Utils.StartHideFailures("Trying To Verify Not Predicted")
            Try
                If Not _Utils.EndWaitForDebugMessages(LiveSurfNotPredicted_Milestones, ActualLines) Then
                    NotPredictedPassed = False
                End If
            Finally
                _iex.ForceHideFailure()
            End Try

            If PredictedPassed OrElse NotPredictedPassed Then
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Predicted Milestones : " + LiveSurfPredicted_Milestones + " Or Not Predicted Milestones : " + LiveSurfNotPredicted_Milestones))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifies Channel Surf Predicted Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySurfChannelPredicted()
        Dim LiveSurfPredicted_Milestones As String = ""
        Dim LiveSurfSlowZapping_Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Verifying Surf Channel Predicted Milestones")

        Try
            LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
            LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")

            If Not _Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Live LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
            End If

            'If _Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Live LiveSurfSlowZapping Milestones : " + LiveSurfSlowZapping_Milestones + " Not Arriving"))
            'End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifies Channel Surf Not Predicated Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySurfChannelNotPredicted()
        Dim LiveSurfNotPredicted_Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Verifying Surf Channel Not Predicted Milestones")

        Try
            LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")

            If Not _Utils.EndWaitForDebugMessages(LiveSurfNotPredicted_Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Live LiveSurfNotPredicted Milestones : " + LiveSurfNotPredicted_Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub WaitAfterLiveReached()
        _UI.Utils.LogCommentInfo("Waiting 1 Second After Live Reached")
        _iex.Wait(1)
    End Sub

#Region "Tune To Channel"

    ''' <summary>
    '''   Setting Tune To Channel With Subtitles Verification
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function SetSubtitlesVerification() As Boolean
        Dim Subtitles_Milestones As String = ""

        Subtitles_Milestones = _Utils.GetValueFromMilestones("Subtitles")
        _UI.Utils.BeginWaitForDebugMessages(Subtitles_Milestones, 15)

        Return True

    End Function

    ''' <summary>
    '''    Setting Tune To Channel Verifications
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Regular Channel Surf,"Predicted" For Channel Surf With Predicted,"Not Predicted"</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Function SetTuneToChannelVerification(ByVal Type As String) As Boolean
        Select Case Type
            Case "", "Ignore"


            Case "Predicted"
                Dim LiveSurfPredicted_Milestones As String = ""
                'Dim LiveSurfSlowZapping_Milestones As String = ""

                LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
                'LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")

                _UI.Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                '_UI.Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

            Case "Not Predicted"
                Dim LiveSurfNotPredicted_Milestones As String = ""

                LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")
                _UI.Utils.BeginWaitForDebugMessages(LiveSurfNotPredicted_Milestones, 15)

            Case "Default"
                Dim LiveSurfDefault As String = ""
                LiveSurfDefault = _Utils.GetValueFromMilestones("LiveSurfDefault")
                _UI.Utils.BeginWaitForDebugMessages(LiveSurfDefault, 15)

            Case "Radio"
                Dim Radio_Milestones As String = ""
                Radio_Milestones = _Utils.GetValueFromMilestones("TuneToRadio")

                _UI.Utils.BeginWaitForDebugMessages(Radio_Milestones, 20)

            Case Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Type " + Type + " Is Not Valid Type"))
        End Select

        Return True
    End Function

    ''' <summary>
    '''   Verifies Tune To Channel Predicted
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyTuneToChannelPredicted()
        Dim ActualLines As New ArrayList

        Dim LiveSurfPredicted_Milestones As String = ""
        'Dim LiveSurfSlowZapping_Milestones As String = ""

        LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
        'LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")

        If Not _UI.Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
        End If

        'If _UI.Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
        '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Got LiveSurfSlowZapping Milestones : " + LiveSurfSlowZapping_Milestones + " even though it was not supposed to arrive!"))
        'End If

    End Sub

    ''' <summary>
    '''   Verifies Tune To Channel Not Predicted
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyTuneToChannelNotPredicted()
        Dim LiveSurfNotPredicted_Milestones As String = ""
        Dim ActualLines As New ArrayList

        LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")

        If Not _UI.Utils.EndWaitForDebugMessages(LiveSurfNotPredicted_Milestones, ActualLines) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify LiveSurfNotPredicted Milestones : " + LiveSurfNotPredicted_Milestones))
        End If

    End Sub

    ''' <summary>
    '''    Verifies Tune To Channel With Subtitles
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyTuneToChannelSubtitles()
        Dim ActualLines As New ArrayList
        Dim Subtitles_Milestones As String = ""

        Subtitles_Milestones = _Utils.GetValueFromMilestones("Subtitles")

        If Not _UI.Utils.EndWaitForDebugMessages(Subtitles_Milestones, ActualLines) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify Subtitles Milestones : " + Subtitles_Milestones))
        End If

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

        Radio_Milestones = _Utils.GetValueFromMilestones("TuneToRadio")

        If Not _UI.Utils.EndWaitForDebugMessages(Radio_Milestones, ActualLines) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify TuneToRadio Milestones : " + Radio_Milestones))
        End If
    End Sub

    ''' <summary>
    ''' Verifying Tune to Channel
    ''' </summary>
    ''' <param name="Type">The channel tune type</param>
    Public Overrides Sub VerifyTuneToChannel(ByVal Type As String)
        Select Case Type
            Case "Predicted"
                VerifyTuneToChannelPredicted()
            Case "Not Predicted"
                VerifySurfChannelNotPredicted()
            Case "Default"
                VerifySurfChannelDefault()
            Case "", "Ignore"
        End Select
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

            SetTuneToChannelVerification(Type)

            _UI.Utils.SendChannelAsIRSequence(ChannelNumber)

            If Not _UI.Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 5) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify dca_number Key With Value " + ChannelNumber))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

#End Region
    ''' <summary>
    '''   Verifying the State in Radio Service
    ''' </summary>
    ''' <remarks> 
    ''' All the Projects Except VOO the Active State on Radio Service is LIVE
    ''' </remarks>

    Public Overrides Sub VerifyRadioStateReached()

        _Utils.VerifyLiveReached()

    End Sub
End Class
