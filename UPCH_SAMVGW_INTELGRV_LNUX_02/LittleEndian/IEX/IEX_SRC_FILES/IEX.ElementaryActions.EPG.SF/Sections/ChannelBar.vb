Imports FailuresHandler

Public Class ChannelBar
    Inherits IEX.ElementaryActions.EPG.ChannelBar

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Channel List By Pressing BACK_UP
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _Utils.StartHideFailures("Navigating To Channel Bar")

        Try
            _Utils.EPG_Milestones_Navigate("CHANNEL BAR")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Moving To Next Event On Channel List 
    ''' </summary>
    ''' <param name="SelectEvent">Optional Parameter Default = True : If True Pressing Select To Select The Event</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Overrides Sub NextEvent(Optional ByVal SelectEvent As Boolean = True)

        _Utils.StartHideFailures("Navigating To Next Event On Channel Bar")

        Try
            _UI.Utils.EPG_Milestones_Navigate("CHANNEL BAR")
            _UI.Utils.EPG_Milestones_SelectMenuItem("NEXT")

            If SelectEvent Then
                _Utils.StartHideFailures("Selecting Next Event From Channel Bar")
                Try
                    _UI.ChannelBar.SelectEvent()
                Finally
                    _iex.ForceHideFailure()
                End Try
            End If
        Finally
            _iex.ForceHideFailure()
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
    Private Sub SetSurfChannelVerification(ByVal Type As String)
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Setting Verification Of " + Type + " Milestones")

        Try
            Select Case Type

                Case "Predicted"
                    Dim LiveSurfPredicted_Milestones As String = ""
                    Dim LiveSurfSlowZapping_Milestones As String = ""

                    LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
                    LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")


                    _Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    _Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

                Case "Not Predicted"
                    Dim LiveSurfNotPredicted_Milestones As String = ""

                    LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")

                    _Utils.BeginWaitForDebugMessages(LiveSurfNotPredicted_Milestones, 15)

                Case "WithPIP"
                    Dim LiveSurfWithPIP As String = ""

                    LiveSurfWithPIP = _Utils.GetValueFromMilestones("ChannelSurfWithPIP")

                    _Utils.BeginWaitForDebugMessages(LiveSurfWithPIP, 15)

                Case "WithoutPIP"
                    Dim LiveSurfWithoutPIP As String = ""
                    LiveSurfWithoutPIP = _Utils.GetValueFromMilestones("ChannelSurfWithoutPIP")

                    _Utils.BeginWaitForDebugMessages(LiveSurfWithoutPIP, 15)

                Case "Default"
                    Dim LiveSurfDefault As String = ""
                    LiveSurfDefault = _Utils.GetValueFromMilestones("ChannelSurfDefault")
                    _Utils.BeginWaitForDebugMessages(LiveSurfDefault, 15)

                Case "Ignore"
                    _Utils.LogCommentWarning("Skipping validation as type is Ignore!")

                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Type " + Type + " Is Not Supported"))
            End Select

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifies Channel Surf Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySurfChannel(Optional ByVal Type As String = "")
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim Msg As String = ""

        _Utils.StartHideFailures("Verifying " + Type + " Milestones")

        Try
            Select Case Type
                Case "WithoutPIP"
                    Msg = "WithoutPIP"
                    Milestones = _Utils.GetValueFromMilestones("ChannelSurfWithoutPIP")

                Case "Default"
                    Msg = "Default"
                    Milestones = _Utils.GetValueFromMilestones("ChannelSurfDefault")

                Case "Predicted"
                    VerifySurfChannelPredicted()
                    Return

                Case "Not Predicted"
                    VerifySurfChannelNotPredicted()
                    Return

            End Select

            If Type <> "Ignore" Then
            If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Channel Bar " + Msg + " Milestones : " + Milestones))
                End If
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
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Channel Bar LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
            End If

            If _Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Channel Bar LiveSurfSlowZapping Milestones : " + LiveSurfSlowZapping_Milestones + " Not Arriving"))
            End If

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
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify Channel Bar LiveSurfNotPredicted Milestones : " + LiveSurfNotPredicted_Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Surfing Up On Channel List
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
        Dim chSurfTimeoutInMSec As Integer

        _Utils.StartHideFailures("Surfing Up On Channel Bar")

        Try
            GetChannelNumber(CurrentChNum)

            If WithSubtitles Then
                _UI.Live.SetSubtitlesVerification()
            End If

            If Type <> "None" Then
                SetSurfChannelVerification(Type)
            End If

            'Fetch the channel surf up key
            chSurfUpKey = GetChannelSurfKey()
            chSurfTimeoutInMSec = GetChannelSurfTimeout()

            'Send the key
            _Utils.SendIR(chSurfUpKey, chSurfTimeoutInMSec)

            GetChannelNumber(CheckedChNum)

            If CurrentChNum = CheckedChNum Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Channel Number Is The Same As Previous Failed To Surf Channel Up"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Surfing Down On Channel List
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
        Dim chSurfTimeoutInMSec As Integer

        _Utils.StartHideFailures("Surfing Down On Channel Bar")

        Try
            GetChannelNumber(CurrentChNum)

            If WithSubtitles Then
                _UI.Live.SetSubtitlesVerification()
            End If

            If Type <> "None" Then
                SetSurfChannelVerification(Type)
            End If

            'Fetch the channel surf up key
            chSurfDownKey = GetChannelSurfKey(False)
            chSurfTimeoutInMSec = GetChannelSurfTimeout()

            'Send the key
            _Utils.SendIR(chSurfDownKey, chSurfTimeoutInMSec)

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
    Overrides Function GetChannelSurfKey(Optional ByVal surfUp As Boolean = True) As String
        Dim surfUpKeyDefault As String = "SELECT_UP"
        Dim surfDownKeyDefault As String = "SELECT_DOWN"
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
            surfKey = _Utils.GetValueFromProject("CHANNEL_BAR", "CHANNEL_SURF_" + surfDirection + "_KEY")
        Catch
            'Taking default value
            _Utils.LogCommentWarning("CHANNEL_SURF_" + surfDirection + "_KEY in section CHANNEL_BAR is missing in your project configuration file. Please add it if it deviates from the default value!")

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
            chSurfTimeout = _Utils.GetValueFromProject("CHANNEL_BAR", "CHANNEL_SURF_TIMEOUT_SEC")
        Catch
            _Utils.LogCommentWarning("CHANNEL_SURF_TIMEOUT_SEC in section CHANNEL_BAR is missing in your project configuration file. Please add it if it deviates from the default value!")
            _Utils.LogCommentWarning("Taking default value from code instead - " + chSurfTimeout)
        End Try
        Return CInt(chSurfTimeout) * 1000
    End Function

    ''' <summary>
    '''   Selecting Event On Channel List By Pressing SELECT
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectEvent()

        _Utils.StartHideFailures("Selecting Event On Channel Bar")

        Try
            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("ACTION BAR") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify State Is ACTION BAR"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Unlocking Event From Channel Bar
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub UnlockEvent()
        _Utils.StartHideFailures("Unlocking Event From Channel Bar")

        Try
            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyDebugMessage("state", "NewPinState", 5, 2) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockFailure, "Failed To Verify Enter PIN Screen Reached"))
            End If

            _Utils.EnterPin("")

            _Utils.VerifyLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Tunning To Channel From Channel Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub DoTune()
        _Utils.StartHideFailures("Tunning To Channel From Channel Bar")

        Try
            _Utils.SendIR("SELECT")

            If Not _Utils.VerifyState("LIVE", 25) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify State Is LIVE"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Getting Event Name From Channel Bar
    ''' </summary>
    ''' <param name="EventName">Returned EventName</param>
    ''' <remarks></remarks>
    Public Overrides Sub GetEventName(ByRef EventName As String, ByVal IsNext As Boolean)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event Name From Channel Bar")

        Try
            _Utils.LogCommentInfo("ChannelBar.GetEventName : Getting Event Name From ChannelBar")

            Dim ReturnedValue As String = ""

            _Utils.GetEpgInfo("evtName", EventName)

            Msg = "Event Name : " + EventName

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Getting Channel Number From Channel Bar
    ''' </summary>
    ''' <param name="ChannelNumber">ByRef The Returned Channel Number</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetChannelNumber(ByRef ChannelNumber As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Channel Number From Channel Bar")

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
    '''   Getting Event Time Left To End
    ''' </summary>
    ''' <param name="TimeLeft">Returnes The Time Left Until End Of Event In Minutes</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventTimeLeft(ByRef TimeLeft As Integer, ByVal IsNext As Boolean)
        Dim EndTime As String = ""
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event Time Left To End")

        Try

            _Utils.GetEpgInfo("date", EPGDateTime)

            _Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)

            _Utils.GetEpgInfo("evttime", EndTime)

            GetEventEndTime(EndTime, False)

            TimeLeft = _Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(ReturnedEpgTime))

            Msg = "Time Left To End : " + TimeLeft.ToString

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Getting Event End Time From Channel Bar
    ''' </summary>
    ''' <param name="EndTime">Returns End Time From Channel Bar</param>
    ''' <param name="IsNext">If True Returns The Next Event End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventEndTime(ByRef EndTime As String, ByVal IsNext As Boolean)
        Dim EvTime As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Event End Time From Action Bar")

        Try

            _Utils.GetEpgInfo("evttime", EndTime)

            _Utils.ParseEventTime(EndTime, EndTime, IsStartTime:=False)

            Msg = "Event End Time : " + EndTime.ToString

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

End Class
